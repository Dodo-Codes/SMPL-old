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
			var prevPos = player.PreviousPosition;
			var prevPosObjs = Object.objects.ContainsKey(prevPos) ? Object.objects[prevPos] : new();

			for (int i = 0; i < 3; i++)
			{
				if (MapEditor.DoorTiles.Contains(Map.RawData[pos][i]))
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
				else if (MapEditor.BoatTiles.Contains(Map.RawData[pos][i]))
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
			}

			for (int i = 0; i < prevPosObjs.Count; i++)
			{
				if (prevPosObjs[i] is Door || prevPosObjs[i] is Boat)
				{
					Map.RawData[prevPosObjs[i].Position][prevPosObjs[i].Height] = prevPosObjs[i].TileIndexes;
					prevPosObjs[i].Destroy();
				}
			}
		}
	}
}
