using SMPL.Data;
using SMPL.Gear;
using System.Collections.Generic;
using System.IO;

namespace RPG1bit
{
	public class WorldObjectManager : Thing
	{
		private static readonly List<string> cachedUIDs = new();

		public WorldObjectManager(string uniqueID) : base(uniqueID)
		{
			Assets.DataSlot.Event.Subscribe.SaveEnd(uniqueID);
		}

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
			var playerWasLoaded = UniqueIDsExists(nameof(Player));
			var chunks = PickByTag(nameof(Chunk));
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

			var player = playerWasLoaded ? (Player)PickByUniqueID(nameof(Player)) :
				new Player(nameof(Player), new GameObject.CreationDetails()
				{
					Name = "Self",
					Position = randPoint,
					Height = 3,
					TileIndexes = new Point[] { World.PositionHasWaterAsHighest(randPoint) ? new(20, 23) : new(25, 0) }
				});

			World.CameraPosition = player.Position;
			player.PreviousPosition = player.Position;
			World.IsShowingRoofs = World.TileHasRoof(player.Position) == false;

			Screen.ScheduleDisplay();
		}
		public static void OnAdvanceTime()
		{
			foreach (var kvp in WorldEditor.Tiles)
				if (char.IsUpper(kvp.Key[0]))
					TryToCreate(kvp.Key);

			var player = (Player)PickByUniqueID(nameof(Player));
			var objsToDestroy = new List<GameObject>();
			var UIDsToCache = new Dictionary<Point, List<string>>();

			foreach (var kvp in GameObject.objects)
			{
				var cacheUIDs = new List<string>();
				var cachePos = new Point();
				for (int i = 0; i < kvp.Value.Count; i++)
				{
					var dist = Point.Distance(player.Position, kvp.Value[i].Position);
					cachePos = kvp.Value[i].Position;

					if (kvp.Value[i] is IRecreatable && dist > 10)
						objsToDestroy.Add(kvp.Value[i]);

					if (kvp.Value[i] is ICachable && kvp.Value[i] is not Player)
					{
						if (kvp.Value[i] is Item item && (item.OwnerUID.Contains("storage") || item.OwnerUID.Contains("item-pile")))
						{
							if (UniqueIDsExists(item.OwnerUID) == false)
								continue;

							var worldPos = ((GameObject)PickByUniqueID(item.OwnerUID)).Position;
							cachePos = worldPos;
							if (Point.Distance(player.Position, worldPos) > 10)
							{
								cacheUIDs.Add(item.UniqueID);
								cachedUIDs.Add(item.UniqueID);
							}
						}
						else if (dist > 10)
						{
							cacheUIDs.Add(kvp.Value[i].UniqueID);
							cachedUIDs.Add(kvp.Value[i].UniqueID);
						}
					}
				}
				if (cacheUIDs.Count > 0)
				{
					if (UIDsToCache.ContainsKey(cachePos) == false)
						UIDsToCache[cachePos] = new();
					for (int i = 0; i < cacheUIDs.Count; i++)
						UIDsToCache[cachePos].Add(cacheUIDs[i]);
				}
			}
			foreach (var kvp in UIDsToCache)
			{
				var slot = new Assets.DataSlot($"cache\\{kvp.Key}.cache")
				{
					ThingUniqueIDs = kvp.Value,
					IsCompressed = false
				};
				slot.SetValue("cache", "");
				slot.Save();
			}
			for (int i = 0; i < objsToDestroy.Count; i++)
			{
				ChunkManager.SetTile(objsToDestroy[i].Position, objsToDestroy[i].Height, objsToDestroy[i].TileIndexes);
				objsToDestroy[i].Destroy();
			}

			var cache = Directory.GetFiles("cache");
			,
		}
		private static void TryToCreate(string type)
		{
			var player = (Player)PickByUniqueID(nameof(Player));
			var positions = new List<Point>()
			{
				player.Position + new Point(1, 0),
				player.Position - new Point(1, 0),
				player.Position + new Point(0, 1),
				player.Position - new Point(0, 1),
			};

			for (int p = 0; p < positions.Count; p++)
				for (int i = 0; i < 4; i++)
				{
					var tile = ChunkManager.GetTile(positions[p], i);
					var id = $"{type}-{positions[p]}-{i}";
					if (WorldEditor.Tiles[type].Contains(tile) && UniqueIDsExists(id) == false)
					{
						var obj = default(GameObject);
						if (type == typeof(Door).Name) obj = new Door(id, new() { Name = "-" });
						else if (type == typeof(Boat).Name) obj = new Boat(id, new() { Name = "-" });
						else if (type == typeof(Storage).Name) obj = new Storage(id, new() { Name = "-" });
						else if (type == typeof(Mount).Name)
						{
							var names = new Dictionary<Point, string>()
							{
								{ new(27, 7), "Horse" }, { new(28, 7), "Horse" },
							};
							obj = new Mount(id, new() { Name = names[tile] });
						}

						obj.Position = positions[p];
						obj.Height = i;
						obj.TileIndexes = tile;
						obj.OnAdvanceTime();
						ChunkManager.SetTile(positions[p], i, new());
					}
				}
		}

		public override void OnAssetsDataSlotSaveEnd()
		{
			for (int i = 0; i < cachedUIDs.Count; i++)
				(PickByUniqueID(cachedUIDs[i])).Destroy();

			cachedUIDs.Clear();
		}
	}
}
