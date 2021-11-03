using SMPL.Data;
using System.Collections.Generic;
using Newtonsoft.Json;
using SMPL.Gear;

namespace RPG1bit
{
	public class ChunkManager : Thing
	{
		private static Point prevCenter;

		public ChunkManager(string uniqueID) : base(uniqueID)
		{
			Assets.Event.Subscribe.LoadEnd(uniqueID);
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
		public static void UpdateChunks()
		{
			var chunkCenter = GetChunkCenterFromPosition(World.CameraPosition);

			if (prevCenter.X < chunkCenter.X)
			{
				SaveChunkIfPossible(new Point(-2, -1));
				SaveChunkIfPossible(new Point(-2, 0));
				SaveChunkIfPossible(new Point(-2, 1));
				LoadChunkIfPossible(new Point(1, -1));
				LoadChunkIfPossible(new Point(1, 0));
				LoadChunkIfPossible(new Point(1, 1));
			}
			else if (chunkCenter.X < prevCenter.X)
			{
				SaveChunkIfPossible(new Point(2, -1));
				SaveChunkIfPossible(new Point(2, 0));
				SaveChunkIfPossible(new Point(2, 1));
				LoadChunkIfPossible(new Point(-1, -1));
				LoadChunkIfPossible(new Point(-1, 0));
				LoadChunkIfPossible(new Point(-1, 1));
			}
			else if (prevCenter.Y < chunkCenter.Y)
			{
				SaveChunkIfPossible(new Point(-1, -2));
				SaveChunkIfPossible(new Point(0, -2));
				SaveChunkIfPossible(new Point(1, -2));
				LoadChunkIfPossible(new Point(-1, 1));
				LoadChunkIfPossible(new Point(0, 1));
				LoadChunkIfPossible(new Point(1, 1));
			}
			else if (chunkCenter.Y < prevCenter.Y)
			{
				SaveChunkIfPossible(new Point(-1, 2));
				SaveChunkIfPossible(new Point(0, 2));
				SaveChunkIfPossible(new Point(1, 2));
				LoadChunkIfPossible(new Point(-1, -1));
				LoadChunkIfPossible(new Point(0, -1));
				LoadChunkIfPossible(new Point(1, -1));
			}

			prevCenter = chunkCenter;

			void SaveChunkIfPossible(Point offset)
			{
				var id = $"chunk-{chunkCenter + offset * 17}";
				if (UniqueIDsExits(id) == false)
					return;
				SaveChunk((Chunk)PickByUniqueID(id));
			}
			void LoadChunkIfPossible(Point offset)
			{
				var pos = chunkCenter + offset * 17;
				var id = $"chunk-{pos}";
				if (UniqueIDsExits(id))
					return;
				LoadChunk(pos);
			}
		}
		public static Point GetChunkCenterFromPosition(Point position)
		{
			return new Point((int)(((position.X + 8) / 17)) * 17, (int)(((position.Y + 8) / 17)) * 17);
		}

		private static void LoadChunk(Point center)
		{
			if (FileSystem.FilesExist($"Chunks\\{center}.chunkdata") == false)
				return;
			Assets.Load(Assets.Type.DataSlot, $"Chunks\\{center}.chunkdata");
		}
		private static void SaveChunk(Chunk chunk)
		{
			if (chunk.Data.Count == 0)
				return;
			var slot = new Assets.DataSlot($"Chunks\\{chunk.Center}.chunkdata");
			slot.SetValue("chunk-data", Text.ToJSON(chunk.GetSavableData()));
			slot.SetValue("chunk", Text.ToJSON(chunk));
			slot.IsCompressed = true;
			slot.Save();
			chunk.Destroy();
		}

		public override void OnAssetsLoadEnd()
		{
			if (Assets.ValuesAreLoaded("chunk") == false)
				return;

			var chunk = Text.FromJSON<Chunk>(Assets.GetValue("chunk"));
			var jsonChunkData = Text.FromJSON<Dictionary<string, string>>(Assets.GetValue("chunk-data"));
			foreach (var kvp in jsonChunkData)
			{
				var key = Text.FromJSON<Point>(kvp.Key);
				var value = Text.FromJSON<Point[]>(kvp.Value);
				chunk.Data[key] = value;
			}

			World.Display();
		}
	}
}
