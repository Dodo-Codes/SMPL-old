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
					Map.RawData[pos][3] = new();
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

			var player = Assets.ValuesAreLoaded(nameof(Player)) == false
						? new Player(nameof(Player), new Object.CreationDetails()
						{
							Name = nameof(Player),
							Position = randPoint,
							Height = 3,
							TileIndexes = new Point[] { new(25, 0) }
						})
						: Text.FromJSON<Player>(Assets.GetValue(nameof(Player)));
			Map.CameraPosition = player.Position;

			LoadAll<Chest>();
			LoadAll<ItemPile>();
			LoadAll<Bag>();
			LoadAll<Quiver>();
			LoadAll<Key>();

			void LoadAll<T>()
			{
				if (Assets.ValuesAreLoaded(typeof(T).Name))
					Text.FromJSON<T[]>(Assets.GetValue(typeof(T).Name));
			}
		}

		public static void OnAdvanceTime()
		{
			Door.TryToCreate();
			Boat.TryToCreate();
			Chest.TryToCreate();

			var player = (Player)Object.PickByUniqueID(nameof(Player));
			var objsToDestroy = new List<Object>();
			foreach (var kvp in Object.objects)
				for (int i = 0; i < kvp.Value.Count; i++)
					if (kvp.Value[i] is IDeletableWhenFar && Point.Distance(player.Position, kvp.Value[i].Position) > 5)
						objsToDestroy.Add(kvp.Value[i]);
			for (int i = 0; i < objsToDestroy.Count; i++)
			{
				Map.RawData[objsToDestroy[i].Position][objsToDestroy[i].Height] = objsToDestroy[i].TileIndexes;
				objsToDestroy[i].Destroy();
			}
		}
	}
}
