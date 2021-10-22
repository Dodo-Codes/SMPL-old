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
			public bool IsEncrypted { get; set; }

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
				IsEncrypted = default;
			}

			public static class Event
			{
				public static class Subscribe
				{
					public static void SaveStart(string thingUID, uint order = uint.MaxValue) =>
						Events.Enable(Events.Type.DataSlotSaveStart, thingUID, order);
					public static void SaveUpdate(string thingUID, uint order = uint.MaxValue) =>
						Events.Enable(Events.Type.DataSlotSaveUpdate, thingUID, order);
					public static void SaveEnd(string thingUID, uint order = uint.MaxValue) =>
						Events.Enable(Events.Type.DataSlotSaveEnd, thingUID, order);
				}
				public static class Unsubscribe
				{
					public static void SaveStart(string thingUID) =>
						Events.Disable(Events.Type.DataSlotSaveStart, thingUID);
					public static void SaveUpdate(string thingUID) =>
						Events.Disable(Events.Type.DataSlotSaveUpdate, thingUID);
					public static void SaveEnd(string thingUID) =>
						Events.Disable(Events.Type.DataSlotSaveEnd, thingUID);
				}
			}

			public void Save()
			{
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
		private static bool assetLoadBegin, assetLoadUpdate, assetLoadEnd, slotSaveStart, slotSaveUpdate, slotSaveEnd;
		private static readonly List<QueuedAsset> queuedAssets = new();
		private static readonly List<DataSlot> queuedSaveSlots = new();
		public static double LoadPercent { get; private set; }

		public static class Event
		{
			public static class Subscribe
			{
				public static void LoadStart(string thingUID, uint order = uint.MaxValue) =>
					Events.Enable(Events.Type.LoadStart, thingUID, order);
				public static void LoadUpdate(string thingUID, uint order = uint.MaxValue) =>
					Events.Enable(Events.Type.LoadUpdate, thingUID, order);
				public static void LoadEnd(string thingUID, uint order = uint.MaxValue) =>
					Events.Enable(Events.Type.LoadEnd, thingUID, order);
			}
			public static class Unsubscribe
			{
				public static void LoadStart(string thingUID) =>
					Events.Disable(Events.Type.LoadStart, thingUID);
				public static void LoadUpdate(string thingUID) =>
					Events.Disable(Events.Type.LoadUpdate, thingUID);
				public static void LoadEnd(string thingUID) =>
					Events.Disable(Events.Type.LoadEnd, thingUID);
			}
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

		private static void CreateAndStartLoadingThread()
		{
			loadingThread = new(new ThreadStart(LoadQueuedResources));
			loadingThread.Name = "ResourcesLoading";
			loadingThread.IsBackground = true;
			loadingThread.Start();
		}
		private static void UpdateMainThread()
		{
			if (assetLoadBegin) Events.Notify(Events.Type.LoadStart, new());
			if (assetLoadUpdate) Events.Notify(Events.Type.LoadUpdate, new());
			if (assetLoadEnd) Events.Notify(Events.Type.LoadEnd, new());
			if (slotSaveStart) Events.Notify(Events.Type.DataSlotSaveStart, new());
			if (slotSaveUpdate) Events.Notify(Events.Type.DataSlotSaveUpdate, new());
			if (slotSaveEnd) Events.Notify(Events.Type.DataSlotSaveEnd, new());

			assetLoadBegin = false;
			assetLoadUpdate = false;
			assetLoadEnd = false;
			slotSaveStart = false;
			slotSaveUpdate = false;
			slotSaveEnd = false;
		}
		private static void LoadQueuedResources()
		{
			while (Window.window.IsOpen)
			{
				if (Performance.Boost == false) Thread.Sleep(1);

				LoadPercent = 100;
				var loadedCount = 0;
				assetLoadBegin = true;
				slotSaveStart = true;

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
											str = File.ReadAllText(path);
											if (str[0] != '{')
												str = Data.Text.Decrypt(str, 'H');
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
					assetLoadBegin = false;
					assetLoadUpdate = false;
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
							var str = JsonConvert.SerializeObject(curQueuedSaveSlots[i], Formatting.Indented);
							if (curQueuedSaveSlots[i].IsEncrypted)
								str = Data.Text.Encrypt(str, 'H');
							File.WriteAllText(path, str);
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
					slotSaveEnd = true;
				}

				void UpdateCounter()
				{
					loadedCount++;
					var percent = Number.ToPercent(loadedCount, new Number.Range(0, queuedAssets.Count + queuedSaveSlots.Count));
					LoadPercent = percent;
					assetLoadUpdate = queuedAssets.Count > 0;
					slotSaveUpdate = queuedSaveSlots.Count > 0;
				}
			}
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
