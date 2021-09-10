using SFML.Audio;
using SFML.Graphics;
using SMPL.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SMPL.Gear
{
	public static class Assets
	{
		private struct QueuedAsset
		{
			public string path;
			public Type asset;

			public QueuedAsset(string path, Type asset)
			{
				this.path = path;
				this.asset = asset;
			}
		}

		private static event Events.ParamsZero OnAssetLoadStart, OnAssetLoadUpdate, OnAssetLoadEnd;

		private static Thread loadingThread;
		private static bool assetLoadBegin, assetLoadUpdate, assetLoadEnd;
		private static readonly List<QueuedAsset> queuedAssets = new();

		private static void CreateAndStartLoadingThread()
		{
			loadingThread = new(new ThreadStart(LoadQueuedResources));
			loadingThread.Name = "ResourcesLoading";
			loadingThread.IsBackground = true;
			loadingThread.Start();
		}
		private static void UpdateMainThreadAboutLoading()
		{
			if (assetLoadBegin) OnAssetLoadStart?.Invoke();
			if (assetLoadUpdate) OnAssetLoadUpdate?.Invoke();
			if (assetLoadEnd) OnAssetLoadEnd?.Invoke();

			assetLoadBegin = false;
			assetLoadUpdate = false;
			assetLoadEnd = false;
		}
		private static void LoadQueuedResources()
		{
			while (Window.window.IsOpen)
			{
				Thread.Sleep(1);
				PercentLoaded = 100;
				if (queuedAssets == null || queuedAssets.Count == 0) continue;

				// thread-safe local list in case the main thread queues something while this one foreaches the list
				var curQueuedAssets = new List<QueuedAsset>(queuedAssets);
				var loadedCount = 0;
				assetLoadBegin = true;
				for (int i = 0; i < curQueuedAssets.Count; i++)
				{
					var asset = curQueuedAssets[i].asset;
					var path = curQueuedAssets[i].path;
					path = path.Replace('/', '\\');
					try
					{
						switch (asset)
						{
							case Type.Texture: textures[path] = new Texture(path); break;
							case Type.Font: fonts[path] = new Font(path); break;
							case Type.Audio:
								{
									music[path] = new Music(path);
									if (music[path].Duration.AsSeconds() < 20)
									{
										music[path].Dispose();
										music.Remove(path);
										sounds[path] = new Sound(new SoundBuffer(path));
									}
									break;
								}
						}
					}
					catch (Exception)
					{
						Debug.LogError(-1, $"Failed to load asset {asset} from file '{path}'.");
						continue;
					}
					loadedCount++;
					var percent = Number.ToPercent(loadedCount, new Number.Range(0, queuedAssets.Count));
					PercentLoaded = percent;
					assetLoadUpdate = true;
					Thread.Sleep(1);
				}
				queuedAssets.Clear(); // done loading, clear queue
				assetLoadEnd = true;
			}
		}

		// =================

		internal static Dictionary<string, Texture> textures = new();
		internal static Dictionary<string, Font> fonts = new();
		internal static Dictionary<string, Sound> sounds = new();
		internal static Dictionary<string, Music> music = new();

		internal static void Initialize()
		{
			CreateAndStartLoadingThread();
		}
		internal static void Update()
		{
			UpdateMainThreadAboutLoading();
		}

		internal static void NotLoadedError(int depth, Type type, string path)
		{
			Debug.LogError(depth + 1, $"The {type} at '{path}' is not loaded.\n" +
				$"Use '{nameof(Assets)}.{nameof(Load)}(...)' to load it.");
		}

		// =================

		public enum Type { Texture, Font, Audio }
		public static class CallWhen
		{
			public static void LoadStart(Action method, uint order = uint.MaxValue) =>
				OnAssetLoadStart = Events.Add(OnAssetLoadStart, method, order);
			public static void LoadUpdate(Action method, uint order = uint.MaxValue) =>
				OnAssetLoadUpdate = Events.Add(OnAssetLoadUpdate, method, order);
			public static void LoadEnd(Action method, uint order = uint.MaxValue) =>
				OnAssetLoadEnd = Events.Add(OnAssetLoadEnd, method, order);
		}

		public static double PercentLoaded { get; private set; }

		public static void Unload(Type asset, params string[] filePaths)
		{
			for (int i = 0; i < filePaths.Length; i++)
				switch (asset)
				{
					case Type.Texture:
						{
							if (NotLoaded(textures, filePaths[i], "Texture")) return;
							textures[filePaths[i]].Dispose();
							textures.Remove(filePaths[i]);
							break;
						}
					case Type.Font:
						{
							if (NotLoaded(fonts, filePaths[i], "Font")) return;
							fonts.Remove(filePaths[i]);
							break;
						}
					case Type.Audio:
						{
							if (sounds.ContainsKey(filePaths[i]) == false && NotLoaded(sounds, filePaths[i], "Audio") ||
								music.ContainsKey(filePaths[i]) == false && NotLoaded(music, filePaths[i], "Audio"))
									return;
							if (sounds.ContainsKey(filePaths[i]))
							{
								sounds[filePaths[i]].Dispose();
								sounds.Remove(filePaths[i]);
							}
							else if(music.ContainsKey(filePaths[i]))
							{
								music[filePaths[i]].Dispose();
								music.Remove(filePaths[i]);
							}
							break;
						}
				}

			bool NotLoaded<T>(Dictionary<string, T> dict, string filePath, string name)
			{
				if (dict.ContainsKey(filePath)) return false;
				Debug.LogError(2, $"Cannot unload {name} asset '{filePath}' since it is not loaded.");
				return true;
			}
		}
		public static bool AreLoaded(params string[] filePaths)
		{
			for (int i = 0; i < filePaths.Length; i++)
			{
				if (textures.ContainsKey(filePaths[i]) == false && fonts.ContainsKey(filePaths[i]) == false &&
					sounds.ContainsKey(filePaths[i]) == false && music.ContainsKey(filePaths[i]) == false)
					return false;
			}
			return true;
		}
		public static void Load(Type asset, params string[] filePaths)
		{
			for (int i = 0; i < filePaths.Length; i++)
			{
				switch (asset)
				{
					case Type.Texture: { if (AlreadyLoaded(textures, filePaths[i], "Texture")) return; break; }
					case Type.Font: { if (AlreadyLoaded(fonts, filePaths[i], "Font")) return; break; }
					case Type.Audio:
						{
							if (AlreadyLoaded(sounds, filePaths[i], "Audio")) return;
							else if (AlreadyLoaded(music, filePaths[i], "Audio")) return;
							break;
						}
				}
				queuedAssets.Add(new QueuedAsset()
				{
					asset = asset,
					path = filePaths[i]
				});
			}
			bool AlreadyLoaded<T>(Dictionary<string, T> dict, string path, string name)
			{
				if (dict.ContainsKey(path) == false) return false;
				Debug.LogError(2, $"Cannot load {name} asset '{path}' since it is already loaded.");
				return true;
			}
		}
	}
}
