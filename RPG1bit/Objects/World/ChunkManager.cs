using SMPL.Data;
using System.Collections.Generic;
using Newtonsoft.Json;
using SMPL.Gear;
using SMPL.Components;

namespace RPG1bit
{
	public class ChunkManager : Thing
	{
		private static Point prevCenter;
		private const double SIZE = 17;
		private static List<Point> queueLoad = new();
		private static List<Point> queueSave = new();

		public ChunkManager(string uniqueID) : base(uniqueID)
		{
			Assets.Event.Subscribe.LoadEnd(uniqueID);
			Assets.DataSlot.Event.Subscribe.SaveEnd(uniqueID);
			Camera.Event.Subscribe.Display(uniqueID);
			FileSystem.DeleteAllFiles("Chunks");
		}

		public static void SetTile(Point position, int height, Point tile)
		{
			var chunkCenter = GetChunkCenterFromPosition(position);
			var id = $"chunk-{chunkCenter}";
			if (UniqueIDsExits(id) == false)
				new Chunk(id) { Center = chunkCenter };
			var chunk = (Chunk)PickByUniqueID(id);

			if (chunk.Data.ContainsKey(position) == false)
				chunk.Data[position] = new Point[4];
			chunk.Data[position][height] = tile;
		}
		public static Point GetTile(Point position, int height)
		{
			var chunkCenter = GetChunkCenterFromPosition(position);
			var id = $"chunk-{chunkCenter}";
			var chunk = (Chunk)PickByUniqueID(id);
			if (UniqueIDsExits(id) == false || chunk.Data.ContainsKey(position) == false)
				return default;
			return chunk.Data[position][height];
		}
		public static Point GetChunkCenterFromPosition(Point position)
		{
			return new Point(Number.Round(position.X / SIZE) * SIZE, Number.Round(position.Y / SIZE) * SIZE);
		}

		public static void UpdateChunks()
		{
			var chunkCenter = GetChunkCenterFromPosition(World.CameraPosition);
			var startLoad = false;
			var startSave = false;

			if (prevCenter.X < chunkCenter.X) // right
			{
				SaveChunkIfPossible(new Point(-2, -1));
				SaveChunkIfPossible(new Point(-2,  0));
				SaveChunkIfPossible(new Point(-2,  1));
				LoadChunkIfPossible(new Point( 1, -1));
				LoadChunkIfPossible(new Point( 1,  0));
				LoadChunkIfPossible(new Point( 1,  1));
			}
			if (chunkCenter.X < prevCenter.X) // left
			{
				SaveChunkIfPossible(new Point( 2, -1));
				SaveChunkIfPossible(new Point( 2,  0));
				SaveChunkIfPossible(new Point( 2,  1));
				LoadChunkIfPossible(new Point(-1, -1));
				LoadChunkIfPossible(new Point(-1,  0));
				LoadChunkIfPossible(new Point(-1,  1));
			}
			if (prevCenter.Y < chunkCenter.Y) // down
			{
				SaveChunkIfPossible(new Point(-1, -2));
				SaveChunkIfPossible(new Point( 0, -2));
				SaveChunkIfPossible(new Point( 1, -2));
				LoadChunkIfPossible(new Point(-1,  1));
				LoadChunkIfPossible(new Point( 0,  1));
				LoadChunkIfPossible(new Point( 1,  1));
			}
			if (chunkCenter.Y < prevCenter.Y) // up
			{
				SaveChunkIfPossible(new Point(-1,  2));
				SaveChunkIfPossible(new Point( 0,  2));
				SaveChunkIfPossible(new Point( 1,  2));
				LoadChunkIfPossible(new Point(-1, -1));
				LoadChunkIfPossible(new Point( 0, -1));
				LoadChunkIfPossible(new Point( 1, -1));
			}

			if (startSave)
				SaveChunk((Chunk)PickByUniqueID($"chunk-{queueSave[0]}"));
			if (startLoad)
				LoadChunk(queueLoad[0]);

			prevCenter = chunkCenter;

			void SaveChunkIfPossible(Point offset)
			{
				var id = $"chunk-{chunkCenter + offset * SIZE}";
				if (UniqueIDsExits(id) == false)
					return;
				var chunk = (Chunk)PickByUniqueID(id);

				if (chunk.Data.Count == 0)
					return;

				queueSave.Add(chunk.Center);
				startSave = true;
			}
			void LoadChunkIfPossible(Point offset)
			{
				var pos = chunkCenter + offset * SIZE;
				var id = $"chunk-{pos}";
				if (UniqueIDsExits(id) || FileSystem.FilesExist($"Chunks\\{pos}.chunkdata") == false)
					return;

				queueLoad.Add(pos);
				startLoad = true;
			}
		}
		private static void SaveChunk(Chunk chunk)
		{
			if (chunk == null)
			{
				queueSave.RemoveAt(0);
				return;
			}
			var slot = new Assets.DataSlot($"Chunks\\{chunk.Center}.chunkdata");
			slot.SetValue($"chunk-data", Text.ToJSON(chunk.GetSavableData()));
			slot.SetValue($"chunk", Text.ToJSON(chunk));
			slot.IsCompressed = true;
			slot.Save();
			chunk.Destroy();
		}
		private static void LoadChunk(Point center)
		{
			Assets.Load(Assets.Type.DataSlot, $"Chunks\\{center}.chunkdata");
		}

		public override void OnAssetsDataSlotSaveEnd()
		{
			for (int i = 0; i < queueSave.Count; i++)
				if (FileSystem.FilesExist($"Chunks\\{queueSave[i]}.chunkdata"))
				{
					queueSave.Remove(queueSave[i]);
					break;
				}

			if (queueSave.Count > 0)
				SaveChunk((Chunk)PickByUniqueID($"chunk-{queueSave[0]}"));
		}
		public override void OnAssetsLoadEnd()
		{
			if (Assets.ValuesAreLoaded("chunk") == false)
				return;

			var chunk = Text.FromJSON<Chunk>(Assets.GetValue($"chunk"));
			var jsonChunkData = Text.FromJSON<Dictionary<string, string>>(Assets.GetValue($"chunk-data"));
			foreach (var kvp in jsonChunkData)
			{
				var key = Text.FromJSON<Point>(kvp.Key);
				var value = Text.FromJSON<Point[]>(kvp.Value);
				chunk.Data[key] = value;
			}

			Assets.UnloadAllValues();
			World.Display();
			queueLoad.Remove(chunk.Center);

			if (queueLoad.Count > 0)
				LoadChunk(queueLoad[0]);
		}

		public override void OnCameraDisplay(Camera camera)
		{
			Text.Display(camera,
				GetChunkCenterFromPosition(World.ScreenToWorldPosition(Screen.GetCellAtCursorPosition())),
				"Assets\\font.ttf");
		}
	}
}
