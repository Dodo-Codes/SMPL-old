using SMPL.Data;
using System.Collections.Generic;
using Newtonsoft.Json;
using SMPL.Gear;
using SMPL.Components;
using System.IO;

namespace RPG1bit
{
	public class ChunkManager : Thing
	{
		private static Point prevCenter;
		private const double SIZE = 17;
		private static readonly List<Point> queueLoad = new();
		private static readonly List<Point> queueSave = new();
		private static bool startSave, startLoad;

		public ChunkManager(string uniqueID) : base(uniqueID)
		{
			Assets.Event.Subscribe.LoadEnd(uniqueID);
			Assets.DataSlot.Event.Subscribe.SaveEnd(uniqueID);

			DestroyAllChunks(false, true);
		}

		public static void SetTile(Point position, int height, Point tile)
		{
			var chunk = GetOrCreateChunk(position);
			if (chunk.Data.ContainsKey(position) == false)
				chunk.Data[position] = new Point[4];
			chunk.Data[position][height] = tile;
		}
		public static Point GetTile(Point position, int height)
		{
			var id = $"chunk-{GetChunkCenterFromPosition(position)}";
			var chunk = (Chunk)PickByUniqueID(id);
			var tile = UniqueIDsExists(id) == false || chunk.Data.ContainsKey(position) == false ? default : chunk.Data[position][height];
			return tile;
		}
		public static Point GetChunkCenterFromPosition(Point position)
		{
			return new Point(Number.Round(position.X / SIZE) * SIZE, Number.Round(position.Y / SIZE) * SIZE);
		}
		public static Chunk GetOrCreateChunk(Point position)
		{
			var chunkCenter = GetChunkCenterFromPosition(position);
			var id = $"chunk-{chunkCenter}";
			if (UniqueIDsExists(id) == false)
				new Chunk(id) { Center = chunkCenter };
			return (Chunk)PickByUniqueID(id);
		}

		public static void ScheduleLoadVisibleChunks()
		{
			LoadChunkIfPossible(new Point(-1, -1)); LoadChunkIfPossible(new Point(0, -1)); LoadChunkIfPossible(new Point(1, -1));
			LoadChunkIfPossible(new Point(-1, 0)); LoadChunkIfPossible(new Point(0, 0)); LoadChunkIfPossible(new Point(1, 0));
			LoadChunkIfPossible(new Point(-1, 1)); LoadChunkIfPossible(new Point(0, 1)); LoadChunkIfPossible(new Point(1, 1));
		}
		public static void UpdateChunks()
		{
			var chunkCenter = GetChunkCenterFromPosition(World.CameraPosition);

			if (prevCenter.X < chunkCenter.X) // right
			{
				SaveChunkIfPossible(new Point(-2, -1));
				SaveChunkIfPossible(new Point(-2, 0));
				SaveChunkIfPossible(new Point(-2, 1));
				LoadChunkIfPossible(new Point(1, -1));
				LoadChunkIfPossible(new Point(1, 0));
				LoadChunkIfPossible(new Point(1, 1));
			}
			if (chunkCenter.X < prevCenter.X) // left
			{
				SaveChunkIfPossible(new Point(2, -1));
				SaveChunkIfPossible(new Point(2, 0));
				SaveChunkIfPossible(new Point(2, 1));
				LoadChunkIfPossible(new Point(-1, -1));
				LoadChunkIfPossible(new Point(-1, 0));
				LoadChunkIfPossible(new Point(-1, 1));
			}
			if (prevCenter.Y < chunkCenter.Y) // down
			{
				SaveChunkIfPossible(new Point(-1, -2));
				SaveChunkIfPossible(new Point(0, -2));
				SaveChunkIfPossible(new Point(1, -2));
				LoadChunkIfPossible(new Point(-1, 1));
				LoadChunkIfPossible(new Point(0, 1));
				LoadChunkIfPossible(new Point(1, 1));
			}
			if (chunkCenter.Y < prevCenter.Y) // up
			{
				SaveChunkIfPossible(new Point(-1, 2));
				SaveChunkIfPossible(new Point(0, 2));
				SaveChunkIfPossible(new Point(1, 2));
				LoadChunkIfPossible(new Point(-1, -1));
				LoadChunkIfPossible(new Point(0, -1));
				LoadChunkIfPossible(new Point(1, -1));
			}

			if (startSave)
			{
				startSave = false;
				SaveChunk((Chunk)PickByUniqueID($"chunk-{queueSave[0]}"), true);
			}
			if (startLoad)
			{
				startLoad = false;
				LoadChunk(queueLoad[0]);
			}

			prevCenter = chunkCenter;
		}
		private static void SaveChunkIfPossible(Point offset)
		{
			var chunkCenter = GetChunkCenterFromPosition(World.CameraPosition);
			var id = $"chunk-{chunkCenter + offset * SIZE}";
			if (UniqueIDsExists(id) == false)
				return;
			var chunk = (Chunk)PickByUniqueID(id);

			if (chunk.Data.Count == 0)
				return;

			queueSave.Add(chunk.Center);
			startSave = true;
		}
		private static void LoadChunkIfPossible(Point offset)
		{
			var chunkCenter = GetChunkCenterFromPosition(World.CameraPosition);
			var pos = chunkCenter + offset * SIZE;
			var id = $"chunk-{pos}";
			if (UniqueIDsExists(id) || File.Exists($"chunks\\{pos}.chunkdata") == false)
				return;

			queueLoad.Add(pos);
			startLoad = true;
		}
		public static void SaveChunk(Chunk chunk, bool destroy)
		{
			if (chunk == null)
			{
				queueSave.RemoveAt(0);
				return;
			}

			var objs = new List<Object>();
			for (int i = 0; i < chunk.ObjectUIDs.Count; i++)
			{
				var obj = (Object)PickByUniqueID(chunk.ObjectUIDs[i]);
				objs.Add(obj);
				if (destroy)
					obj.Destroy();
			}
			chunk.ObjectsJSON = Text.ToJSON(objs);

			var slot = new Assets.DataSlot($"chunks\\{chunk.Center}.chunkdata");
			slot.SetValue("chunk-data", Text.ToJSON(chunk.GetSavableData()));
			slot.SetValue("chunk", Text.ToJSON(chunk));
			slot.IsCompressed = true;
			slot.Save();
			if (destroy)
				chunk.Destroy();
		}
		public static void LoadChunk(Point center)
		{
			Assets.Load(Assets.Type.DataSlot, $"chunks\\{center}.chunkdata");
		}
		public static void DestroyAllChunks(bool runtime, bool savedData)
		{
			if (runtime)
			{
				var chunks = PickByTag(nameof(Chunk));
				for (int i = 0; i < chunks.Length; i++)
					chunks[i].Destroy();
			}
			if (savedData)
			{
				var chunkNames = Directory.GetFiles("chunks");
				for (int i = 0; i < chunkNames.Length; i++)
					File.Delete(chunkNames[i]);
			}
		}
		public static bool HasQueuedLoad() => queueLoad.Count > 0;
		public static bool HasQueuedSave() => queueSave.Count > 0;

		public override void OnAssetsDataSlotSaveEnd()
		{
			for (int i = 0; i < queueSave.Count; i++)
				if (File.Exists($"chunks\\{queueSave[i]}.chunkdata"))
				{
					queueSave.Remove(queueSave[i]);
					break;
				}

			if (queueSave.Count > 0)
				SaveChunk((Chunk)PickByUniqueID($"chunk-{queueSave[0]}"), true);
		}
		public override void OnAssetsLoadEnd()
		{
			if (Assets.ValuesAreLoaded("chunk") == false)
				return;

			var chunk = Text.FromJSON<Chunk>(Assets.GetValue("chunk"));
			var jsonChunkData = Text.FromJSON<Dictionary<string, string>>(Assets.GetValue($"chunk-data"));
			foreach (var kvp in jsonChunkData)
			{
				var key = Text.FromJSON<Point>(kvp.Key);
				var value = Text.FromJSON<Point[]>(kvp.Value);
				chunk.Data[key] = value;
			}

			Text.FromJSON<List<Object>>(chunk.ObjectsJSON);

			Assets.UnloadValues("chunk", "chunk-data");
			World.Display();
			queueLoad.Remove(chunk.Center);

			if (queueLoad.Count > 0)
				LoadChunk(queueLoad[0]);
		}
	}
}
