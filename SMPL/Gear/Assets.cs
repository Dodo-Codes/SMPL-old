using Newtonsoft.Json;
using SFML.Audio;
using SFML.Graphics;
using SMPL.Components;
using SMPL.Data;
using SMPL.Prefabs;
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
		public struct DataSlot
		{
			public const string DIRECTORY = "SavedData";

			[JsonProperty]
			internal Dictionary<string, string> values;
			public string FilePath { get; set; }

			public Area[] Areas { get; set; }
			public Audio[] Audios { get; set; }
			public Camera[] Cameras { get; set; }
			public Effects[] Effects { get; set; }
			public Family[] Families { get; set; }
			public Hitbox[] Hitboxes { get; set; }
			public Components.Sprite[] Sprites { get; set; }
			public Textbox[] Textboxes { get; set; }
			public Components.Timer[] Timers { get; set; }

			public Cloth[] Clothes { get; set; }
			public Ropes[] Ropes { get; set; }
			public SegmentedLine[] SegmentedLines { get; set; }

			public Probability.Table[] ProbabilityTables { get; set; }

			public DataSlot(string filePath)
			{
				values = default; Areas = default; Audios = default; Cameras = default; Effects = default; Families = default;
				Hitboxes = default; Sprites = default; Textboxes = default; Timers = default; Clothes = default; Ropes = default;
				SegmentedLines = default; ProbabilityTables = default;
				FilePath = filePath;
			}

			public void SetValue(string key, string value)
			{
				if (values == null) values = new();
				values[key] = value;
			}
		}

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
		public enum Type { Texture, Font, Music, Sound, DataSlot }

		private static event Events.ParamsZero OnAssetLoadStart, OnAssetLoadUpdate, OnAssetLoadEnd;
		internal static Dictionary<string, Texture> textures = new();
		internal static Dictionary<string, Font> fonts = new();
		internal static Dictionary<string, Sound> sounds = new();
		internal static Dictionary<string, Music> music = new();
		internal static Dictionary<string, string> values = new();

		private static Thread loadingThread;
		private static bool assetLoadBegin, assetLoadUpdate, assetLoadEnd;
		private static readonly List<QueuedAsset> queuedAssets = new();
		private static readonly List<DataSlot> queuedSaveSlots = new();
		public static double PercentLoaded { get; private set; }

		public static class CallWhen
		{
			public static void LoadStart(Action method, uint order = uint.MaxValue) =>
				OnAssetLoadStart = Events.Add(OnAssetLoadStart, method, order);
			public static void LoadUpdate(Action method, uint order = uint.MaxValue) =>
				OnAssetLoadUpdate = Events.Add(OnAssetLoadUpdate, method, order);
			public static void LoadEnd(Action method, uint order = uint.MaxValue) =>
				OnAssetLoadEnd = Events.Add(OnAssetLoadEnd, method, order);
		}

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
							fonts[filePaths[i]].Dispose();
							fonts.Remove(filePaths[i]);
							break;
						}
					case Type.Music:
						{
							if (NotLoaded(music, filePaths[i], "Audio")) return;
							music[filePaths[i]].Dispose();
							music.Remove(filePaths[i]);
							break;
						}
					case Type.Sound:
						{
							if (NotLoaded(sounds, filePaths[i], "Audio")) return;
							sounds[filePaths[i]].Dispose();
							sounds.Remove(filePaths[i]);
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
		public static void UnloadValue(string key)
		{
			if (values.ContainsKey(key) == false)
			{
				Debug.LogError(1, $"No value was found at key '{key}'.");
				return;
			}
			values.Remove(key);
		}
		public static bool AreLoaded(params string[] filePaths)
		{
			for (int i = 0; i < filePaths.Length; i++)
			{
				if (filePaths[i] == null || textures.ContainsKey(filePaths[i]) == false && fonts.ContainsKey(filePaths[i]) == false &&
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
					case Type.Music: { if (AlreadyLoaded(music, filePaths[i], "Music")) return; break; }
					case Type.Sound: { if (AlreadyLoaded(sounds, filePaths[i], "Sound")) return; break; }
					case Type.DataSlot: break;
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
				Debug.LogError(2, $"Cannot load {name} asset '{path}' because it is already loaded.");
				return true;
			}
		}
		public static void SaveDataSlots(params DataSlot[] dataSlots)
		{
			for (int i = 0; i < dataSlots.Length; i++)
				queuedSaveSlots.Add(dataSlots[i]);
		}
		public static bool ValueIsLoaded(string key)
		{
			return key != null && values.ContainsKey(key);
		}

		public static string GetValue(string key)
		{
			var result = default(string);
			if (Statics.TryCast(Assets.values[key], out result) == false)
				Debug.LogError(1, $"Could not retrieve the value from index '{key}'.");
			return result;
		}

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
				var loadedCount = 0;
				assetLoadBegin = true;

				if (queuedAssets != null && queuedAssets.Count > 0)
				{
					// thread-safe local list in case the main thread queues something while this one foreaches the list
					var curQueuedAssets = new List<QueuedAsset>(queuedAssets);
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
								case Type.Sound: sounds[path] = new Sound(new SoundBuffer(path)); break;
								case Type.Music: music[path] = new Music(path); break;
								case Type.DataSlot:
									{
										var str = "";
										try
										{
											str = System.IO.File.ReadAllText(path);
											str = Data.Text.Decrypt(str, 'H', true);
											var slot = JsonConvert.DeserializeObject<DataSlot>(str);

											if (slot.values == null) continue;
											foreach (var kvp in slot.values)
												values[kvp.Key] = kvp.Value;
										}
										catch (System.Exception)
										{
											Debug.LogError(1, $"Error loading {nameof(DataSlot)} from file '{path}'.");
											continue;
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
						UpdateCounter();
						Thread.Sleep(1);
					}
					queuedAssets.Clear(); // done loading, clear queue
					assetLoadEnd = true;
				}
				if (queuedSaveSlots != null && queuedSaveSlots.Count > 0)
				{
					var curQueuedSaveSlots = new List<DataSlot>(queuedSaveSlots);
					for (int i = 0; i < curQueuedSaveSlots.Count; i++)
					{
						var path = curQueuedSaveSlots[i].FilePath;
						try
						{
							Directory.CreateDirectory(Path.GetDirectoryName(path));
							var str = JsonConvert.SerializeObject(curQueuedSaveSlots[i]);
							str = Data.Text.Encrypt(str, 'H', true);
							System.IO.File.WriteAllText(path, str);
						}
						catch (Exception)
						{
							Debug.LogError(-1, $"Failed to save {nameof(DataSlot)} asset from file '{path}'.");
							continue;
						}
						UpdateCounter();
						Thread.Sleep(1);
					}
					queuedSaveSlots.Clear(); // done loading, clear queue
					assetLoadEnd = true;
				}

				void UpdateCounter()
				{
					loadedCount++;
					var percent = Number.ToPercent(loadedCount, new Number.Range(0, queuedAssets.Count + queuedSaveSlots.Count));
					PercentLoaded = percent;
					assetLoadUpdate = true;
				}
			}
		}

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
	}
}
