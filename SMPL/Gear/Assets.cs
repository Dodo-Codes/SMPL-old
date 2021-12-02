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
		[JsonObject(MemberSerialization.OptIn)]
		public struct DataSlot
		{
			public const string DIRECTORY = "SavedData";

			[JsonProperty]
			internal Dictionary<string, string> values;
			[JsonProperty]
			internal List<Thing> things;
			
			public string FilePath { get; set; }
			public bool IsCompressed { get; set; }

			public List<uint> ThingUIDs { get; set; }

			public DataSlot(string filePath)
			{
				values = default;
				things = new();
				ThingUIDs = new();
				FilePath = filePath;
				IsCompressed = default;
			}

			public void Save()
			{
				if (ThingUIDs != null)
					for (int i = 0; i < ThingUIDs.Count; i++)
						things.Add(Thing.Pick(ThingUIDs[i]));

				queuedSaveSlots.Add(this);
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
		internal static Dictionary<string, Texture> textures = new();
		internal static Dictionary<string, Font> fonts = new();
		internal static Dictionary<string, Sound> sounds = new();
		internal static Dictionary<string, Music> music = new();
		internal static Dictionary<string, string> values = new();

		private static Thread loadingThread;
		private static readonly List<string> assetLoadEnd = new(), slotSaveEnd = new();
		private static readonly List<QueuedAsset> queuedAssets = new();
		private static readonly List<DataSlot> queuedSaveSlots = new();
		public static double LoadPercent { get; private set; }

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
		public static void UnloadValues(params string[] keys)
		{
			for (int i = 0; i < keys.Length; i++)
			{
				if (values.ContainsKey(keys[i]) == false)
				{
					Debug.LogError(1, $"No value was found at key '{keys[i]}'.");
					return;
				}
				values.Remove(keys[i]);
			}
		}
		public static void UnloadAllValues()
		{
			values.Clear();
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

				// File.Exists does not accept file extensions, just names so that's a workaround
				if (filePaths[i].Contains('\\') &&
					Directory.GetFiles(Path.GetDirectoryName(filePaths[i])).ToList().Contains(filePaths[i]) == false)
				{
					Debug.LogError(1, $"No file was found on path '{filePaths[i]}'.");
					return;
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
		public static bool ValuesAreLoaded(params string[] keys)
		{
			if (keys == null) return false;
			for (int i = 0; i < keys.Length; i++)
				if (values.ContainsKey(keys[i]) == false)
					return false;
			return true;
		}

		public static string GetValue(string key)
		{
			if (values.ContainsKey(key) == false)
			{
				Debug.LogError(1, $"The key '{key}' was not found.");
				return default;
			}
			var result = default(string);
			if (Statics.TryCast(values[key], out result) == false)
				Debug.LogError(1, $"Could not retrieve the value from key '{key}'.");
			return result;
		}

		public static void CreateTexture(Data.Color[,] pixels, string id)
		{
			if (textures.ContainsKey(id))
			{
				Debug.LogError(1, $"Cannot create texture with id '{id}' because a texture is loaded on such path.");
				return;
			}

			var px = new SFML.Graphics.Color[pixels.GetLength(0), pixels.GetLength(1)];
			for (int y = 0; y < pixels.GetLength(1); y++)
				for (int x = 0; x < pixels.GetLength(0); x++)
					px[x, y] = Data.Color.From(pixels[x, y]);
			var img = new Image(px);
			var texture = new Texture(img);
			textures[id] = texture;
			img.Dispose();
		}

		private static void CreateAndStartLoadingThread()
		{
			loadingThread = new(new ThreadStart(WorkOnQueuedResources));
			loadingThread.Name = "ResourcesLoading";
			loadingThread.IsBackground = true;
			loadingThread.Start();
		}
		private static void WorkOnQueuedResources()
		{
			while (Window.window.IsOpen)
			{
				if (Performance.Boost == false) Thread.Sleep(1);

				LoadPercent = 100;
				var loadedCount = 0;

				try
				{
					if (queuedAssets != null && queuedAssets.Count > 0)
					{
						// thread-safe local list in case the main thread queues something while this one foreaches the list
						var curQueuedAssets = new List<QueuedAsset>(queuedAssets);
						for (int i = 0; i < curQueuedAssets.Count; i++)
						{
							var asset = curQueuedAssets[i].asset;
							var path = curQueuedAssets[i].path;
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
												str = File.ReadAllText(path);
												if (str[0] != '{')
													str = Data.Text.Decompress(str);
												var slot = str.ToObject<DataSlot>();

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
							queuedAssets.Remove(curQueuedAssets[i]);
							assetLoadEnd.Add(curQueuedAssets[i].path);
							Thread.Sleep(1);
						}
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
								var str = curQueuedSaveSlots[i].ToJSON();
								if (curQueuedSaveSlots[i].IsCompressed)
									str = Data.Text.Compress(str);
								File.WriteAllText(path, str);
							}
							catch (Exception)
							{
								Debug.LogError(-1, $"Failed to save {nameof(DataSlot)} asset in file '{path}'.");
								continue;
							}
							UpdateCounter();
							slotSaveEnd.Remove(curQueuedSaveSlots[i].FilePath);
							Thread.Sleep(1);
						}
					}
				}
				catch (Exception)
				{
					Debug.LogError(-1, $"Could not save or load some of the queued {nameof(Assets)}.");
					return;
				}
				void UpdateCounter()
				{
					loadedCount++;
					var percent = Number.ToPercent(loadedCount, new Number.Range(0, queuedAssets.Count + queuedSaveSlots.Count));
					LoadPercent = percent;
				}
			}
		}
		private static void UpdateMainThread()
		{
			for (int i = 0; i < assetLoadEnd.Count; i++)
				Events.Notify(Game.Event.AssetLoadEnd, new() { String = new string[] { assetLoadEnd[i] } });
			for (int i = 0; i < slotSaveEnd.Count; i++)
				Events.Notify(Game.Event.AssetDataSlotSaveEnd, new() { String = new string[] { slotSaveEnd[i] } });

			assetLoadEnd.Clear();
			slotSaveEnd.Clear();
		}

		internal static void Initialize()
		{
			CreateAndStartLoadingThread();
		}
		internal static void Update()
		{
			UpdateMainThread();
		}

		internal static void NotLoadedError(int depth, Type type, string path)
		{
			Debug.LogError(depth + 1, $"The {type} at '{path}' is not loaded.\n" +
				$"Use '{nameof(Assets)}.{nameof(Load)}(...)' to load it.");
		}
	}
}
