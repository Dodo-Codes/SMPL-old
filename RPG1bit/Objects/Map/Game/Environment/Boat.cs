using SMPL.Data;

namespace RPG1bit
{
	public class Boat : Object, IDeletableWhenFar
	{
		public static void TryToCreate()
		{
			var player = (Player)Object.PickByUniqueID(nameof(Player));
			var pos = player.Position;
			var type = nameof(Boat).ToLower();

			for (int i = 0; i < 3; i++)
			{
				var tile = Map.RawData.ContainsKey(pos) ? Map.RawData[pos][i] : new();
				var id = $"{type}-{pos}-{i}";
				if (MapEditor.Tiles[type].Contains(tile) && Object.UniqueIDsExits(id) == false)
				{
					var obj = new Boat(id, new()
					{
						Position = pos,
						Height = i,
						Name = "-",
						TileIndexes = new Point[] { Map.RawData[pos][i] }
					});
					obj.OnAdvanceTime();
					Map.RawData[pos][i] = new();
				}
			}
		}
		public Boat(string uniqueID, CreationDetails creationDetails) : base(uniqueID, creationDetails) { }

		public override void OnAdvanceTime()
		{
			var player = (Player)PickByUniqueID(nameof(Player));
			if (player.PreviousPosition != Position && player.Position != Position)
				return;

			var prevIsWater = Map.PositionHasWaterAsHighest(player.PreviousPosition);
			var currIsWater = Map.PositionHasWaterAsHighest(player.Position);

			var index = 0;
			if (player.Position.Y > player.PreviousPosition.Y) index = 1;
			else if (player.Position.X < player.PreviousPosition.X) index = 2;
			else if (player.Position.X > player.PreviousPosition.X) index = 3;

			if (prevIsWater && currIsWater == false)
			{
				TileIndexes = new(8 + index, 19) { C = TileIndexes.C };
				return;
			}
			Position = player.Position;
			player.TileIndexes = new(16 + index, 23) { C = player.TileIndexes.C };
			TileIndexes = new(12 + index, 23) { C = TileIndexes.C };
		}
	}
}
