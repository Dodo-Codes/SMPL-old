using SMPL.Data;
using SMPL.Gear;
using System.Collections.Generic;

namespace RPG1bit
{
	public class WorldObjectManager
	{
		public static void InitializeObjects()
		{
			if (Gate.EnterOnceWhile("create-item-info-tab", true))
			{
				new ItemStats("item-stats", new()
				{
					Position = new(19, 9),
					Height = 1,
					TileIndexes = new Point[] { new() },
					IsInTab = true,
					AppearOnTab = "item-info",
					IsUI = true,
					Name = "stats",
					IsKeptBetweenSessions = true,
				});
			}
			if (Gate.EnterOnceWhile("create-player-tab", true))
			{
				new PlayerStats("player-stats", new()
				{
					Position = new(19, 12),
					Height = 1,
					TileIndexes = new Point[] { new() },
					IsInTab = true,
					AppearOnTab = "player-stats",
					IsUI = true,
					Name = "self",
					IsKeptBetweenSessions = true,
				});
			}
			PlayerStats.Open();

			var freeTiles = new List<Point>();
			var playerTiles = new List<Point>();
			var playerWasLoaded = Thing.UniqueIDsExists(nameof(Player));
			var chunks = Thing.PickByTag(nameof(Chunk));
			foreach (Chunk chunk in chunks)
				foreach (var kvp in chunk.Data)
				{
					var pos = new Point(kvp.Key.X, kvp.Key.Y);
					if (playerWasLoaded == false && chunk.Data[pos][3] == World.TilePlayer)
						playerTiles.Add(pos);
					else if (chunk.Data[pos][3] == new Point(0, 0))
						freeTiles.Add(pos);
				}

			var randPoint = playerTiles.Count > 0 ?
				playerTiles[(int)Probability.Randomize(new(0, playerTiles.Count - 1))] :
				freeTiles[(int)Probability.Randomize(new(0, freeTiles.Count - 1))];

			var player = playerWasLoaded ? (Player)Thing.PickByUniqueID(nameof(Player)) :
				new Player(nameof(Player), new Object.CreationDetails()
				{
					Name = "Self",
					Position = randPoint,
					Height = 3,
					TileIndexes = new Point[] { World.PositionHasWaterAsHighest(randPoint) ? new(20, 23) : new(25, 0) }
				});

			World.CameraPosition = player.Position;
			player.PreviousPosition = player.Position;
			World.IsShowingRoofs = World.TileHasRoof(player.Position) == false;

			LoadAll<Storage>(); LoadAll<ItemPile>(); LoadAll<Bag>(); LoadAll<Quiver>(); LoadAll<Key>(); LoadAll<Map>();

			Screen.ScheduleDisplay();

			void LoadAll<T>()
			{
				if (Assets.ValuesAreLoaded(typeof(T).Name))
				{
					Text.FromJSON<T[]>(Assets.GetValue(typeof(T).Name));
					Assets.UnloadValues(typeof(T).Name);
				}
			}
		}
		public static void OnAdvanceTime()
		{
			TryToCreateObject<Door>();
			TryToCreateObject<Boat>();
			TryToCreateObject<Storage>();

			var player = (Player)Thing.PickByUniqueID(nameof(Player));
			var objsToDestroy = new List<Object>();
			foreach (var kvp in Object.objects)
				for (int i = 0; i < kvp.Value.Count; i++)
					if (kvp.Value[i] is IDeletableWhenFar && Point.Distance(player.Position, kvp.Value[i].Position) > 5)
						objsToDestroy.Add(kvp.Value[i]);

			for (int i = 0; i < objsToDestroy.Count; i++)
			{
				ChunkManager.SetTile(objsToDestroy[i].Position, objsToDestroy[i].Height, objsToDestroy[i].TileIndexes);
				objsToDestroy[i].Destroy();
			}
		}
		private static void TryToCreateObject<T>()
		{
			var player = (Player)Thing.PickByUniqueID(nameof(Player));
			var positions = new List<Point>()
			{
				player.Position + new Point(1, 0),
				player.Position - new Point(1, 0),
				player.Position + new Point(0, 1),
				player.Position - new Point(0, 1),
			};
			var type = typeof(T).Name;

			for (int p = 0; p < positions.Count; p++)
				for (int i = 0; i < 3; i++)
				{
					var tile = ChunkManager.GetTile(positions[p], i);
					var id = $"{type}-{positions[p]}-{i}";
					if (WorldEditor.Tiles[type].Contains(tile) && Thing.UniqueIDsExists(id) == false)
					{
						var obj = default(Object);
						if (typeof(T) == typeof(Door)) obj = new Door(id, new() { Name = "-" });
						else if (typeof(T) == typeof(Boat)) obj = new Boat(id, new() { Name = "-" });
						else if (typeof(T) == typeof(Storage)) obj = new Storage(id, new() { Name = "-" });

						obj.Position = positions[p];
						obj.Height = i;
						obj.TileIndexes = tile;
						obj.OnAdvanceTime();
						ChunkManager.SetTile(positions[p], i, new());
					}
				}
		}
	}
}
