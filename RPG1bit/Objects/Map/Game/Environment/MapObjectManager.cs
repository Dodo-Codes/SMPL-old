using SMPL.Data;
using SMPL.Gear;
using System.Collections.Generic;

namespace RPG1bit
{
	public class MapObjectManager
	{
		public static void InitializeObjects()
		{
			var freeTile = new Point(0, 0);
			var playerTiles = new List<Point>();
			foreach (var kvp in Map.RawData)
			{
				var pos = new Point(kvp.Key.X, kvp.Key.Y);
				if (Map.RawData[pos][3] == Map.TilePlayer)
				{
					playerTiles.Add(pos);
					Map.RawData[pos][3] = default;
				}
				else if (Map.RawData[pos][3] == Map.TileBarrier)
				{
					var tile = Map.TileBarrier;
					tile.C = new Color();
					Map.RawData[pos][3] = tile;
				}
				else if (Map.RawData[pos][3] == new Point(0, 0))
					freeTile = pos;
			}

			var randPoint = playerTiles.Count > 0 ?
				playerTiles[(int)Probability.Randomize(new(0, playerTiles.Count - 1))] : freeTile;

			var player = Assets.ValuesAreLoaded("player") == false
						? new Player("player", new Object.CreationDetails()
						{
							Name = "player",
							Position = randPoint,
							Height = 3,
							TileIndexes = new Point[] { new(25, 0) }
						})
						: Text.FromJSON<Player>(Assets.GetValue("player"));
			Map.CameraPosition = player.Position;
		}

		public static void OnAdvanceTime()
		{
			var player = (Player)Object.PickByUniqueID("player");
			var pos = player.Position;

			for (int i = 0; i < 3; i++)
			{
				if (MapEditor.Tiles["door"].Contains(Map.RawData[pos][i]) && Object.UniqueIDsExits($"door-{pos}-{i}") == false)
				{
					var door = new Door($"door-{pos}-{i}", new()
					{
						Position = pos,
						Height = i,
						Name = "Door",
						TileIndexes = new Point[] { Map.RawData[pos][i] }
					});
					door.OnAdvanceTime();
				}
				else if (MapEditor.Tiles["boat"].Contains(Map.RawData[pos][i]) && Object.UniqueIDsExits($"boat-{pos}-{i}") == false)
				{
					var boat = new Boat($"boat-{pos}-{i}", new()
					{
						Position = pos,
						Height = i,
						Name = "Boat",
						TileIndexes = new Point[] { Map.RawData[pos][i] }
					});
					Map.RawData[pos][i] = new();
					boat.OnAdvanceTime();
				}
				else if (MapEditor.Tiles["chest"].Contains(Map.RawData[pos][i]) && Object.UniqueIDsExits($"chest-{pos}-{i}") == false)
				{
					var chest = new Chest($"chest-{pos}-{i}", new()
					{
						Position = pos,
						Height = i,
						Name = "Chest",
						TileIndexes = new Point[] { Map.RawData[pos][i] }
					});
					Map.RawData[pos][i] = new();
					chest.OnAdvanceTime();
				}
			}

			var objsToDestroy = new List<Object>();
			foreach (var kvp in Object.objects)
				for (int i = 0; i < kvp.Value.Count; i++)
					if (kvp.Value[i] is DeletableWhenFar && Point.Distance(player.Position, kvp.Value[i].Position) > 5)
						objsToDestroy.Add(kvp.Value[i]);
			for (int i = 0; i < objsToDestroy.Count; i++)
			{
				Map.RawData[objsToDestroy[i].Position][objsToDestroy[i].Height] = objsToDestroy[i].TileIndexes;
				objsToDestroy[i].Destroy();
			}
		}
	}
}
