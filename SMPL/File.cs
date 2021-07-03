using SFML.Audio;
using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Threading;
using System.IO;

namespace SMPL
{
	public static class File
	{
		public enum AssetType
		{
			Texture, Font, Sound, Music
		}
		public static double PercentLoaded { get; private set; }
		public static string Directory { get { return AppDomain.CurrentDomain.BaseDirectory; } }

		internal static bool assetLoadBegin, assetLoadUpdate, assetLoadEnd;
		internal static List<QueuedAsset> queuedAssets = new();
		internal static Dictionary<string, Texture> textures = new();
		internal static Dictionary<string, Font> fonts = new();
		internal static Dictionary<string, Sound> sounds = new();
		internal static Dictionary<string, Music> music = new();

		internal static void Initialize()
		{

		}
		internal static void UpdateMainThreadAssets()
		{
			if (assetLoadBegin) FileEvents.instance.OnLoadingStart();
			assetLoadBegin = false;

			if (assetLoadUpdate) FileEvents.instance.OnLoadingUpdate();
			assetLoadUpdate = false;

			if (assetLoadEnd) FileEvents.instance.OnLoadingEnd();
			assetLoadEnd = false;
		}
		// thread for loading resources
		internal static void LoadQueuedResources()
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
				foreach (var queuedAsset in curQueuedAssets)
				{
					var asset = queuedAsset.asset;
					var path = queuedAsset.path;
					path = path.Replace('/', '\\');
					var fullPath = $"{Directory}{path}";
					try
					{
						switch (asset)
						{
							case AssetType.Texture: textures[path] = new Texture(path); break;
							case AssetType.Font: fonts[path] = new Font(path); break;
							case AssetType.Sound: sounds[path] = new Sound(new SoundBuffer(path)); break;
							case AssetType.Music: music[path] = new Music(path); break;
						}
					}
					catch (Exception)
					{
						Debug.LogError(1, queuedAsset.error);
						continue;
					}
					loadedCount++;
					var percent = Number.ToPercent(loadedCount, new Bounds(0, queuedAssets.Count));
					PercentLoaded = percent;
					assetLoadUpdate = true;
					Thread.Sleep(1);
				}
				queuedAssets.Clear(); // done loading, clear queue
				assetLoadEnd = true;
			}
		}
		internal static string CreateDirectoryForFile(string filePath)
		{
			filePath = filePath.Replace('/', '\\');
			var path = filePath.Split('\\');
			var full = $"{File.Directory}{filePath}";
			var curPath = File.Directory;
			for (int i = 0; i < path.Length - 1; i++)
			{
				var p = $"{curPath}\\{path[i]}";
				if (System.IO.Directory.Exists(p) == false) System.IO.Directory.CreateDirectory(p);

				curPath = p;
			}
			return full;
		}
		internal struct QueuedAsset
		{
			public string path;
			public AssetType asset;
			public string error;

			public QueuedAsset(string path, AssetType asset, string error)
			{
				this.path = path;
				this.asset = asset;
				this.error = error;
			}
		}

		/// <summary>
		/// Creates or overwrites a file at <paramref name="filePath"/> and fills it with <paramref name="text"/>. Any <paramref name="text"/> can be retrieved from a file with <see cref="Load"/>.<br></br><br></br>
		/// This is a slow operation - do not call frequently.
		/// </summary>
		public static void Save(string text, string filePath = "folder/file.extension")
		{
			var full = CreateDirectoryForFile(filePath);

			try
			{
				System.IO.File.WriteAllText(full, text);
			}
			catch (Exception)
			{
				Debug.LogError(1, $"Could not save file '{full}'.");
				return;
			}
		}
		/// <summary>
		/// Reads all the text from the file at <paramref name="filePath"/> and returns it as a <see cref="string"/> if successful. Returns <paramref name="null"/> otherwise. A text can be saved to a file with <see cref="Save"/>.<br></br><br></br>
		/// This is a slow operation - do not call frequently.
		/// </summary>
		public static string Load(string filePath = "folder/file.extension")
		{
			filePath = filePath.Replace('/', '\\');
			var full = $"{Directory}\\{filePath}";

			if (System.IO.Directory.Exists(full) == false)
			{
				Console.Log($"Could not load file '{full}'. Directory/file not found.");
				return default;
			}
			try
			{
				return System.IO.File.ReadAllText(full);
			}
			catch (Exception)
			{
				Console.Log($"Could not load file '{full}'.");
				return default;
			}
		}
	}
}
