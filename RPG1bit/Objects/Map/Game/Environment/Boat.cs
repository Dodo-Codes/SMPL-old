using SMPL.Data;

namespace RPG1bit
{
	public class Boat : Object
	{
		public Boat(string uniqueID, CreationDetails creationDetails) : base(uniqueID, creationDetails) { }
		public override void OnHovered() => HoveredInfo = "Boat.";
		public override void OnAdvanceTime()
		{
			var player = (Player)PickByUniqueID("player");
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
