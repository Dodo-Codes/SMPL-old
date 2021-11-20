using SMPL.Data;

namespace RPG1bit
{
	public class Boat : Object, IDeletableWhenFar
	{
		private int index;

		public Boat(string uniqueID, CreationDetails creationDetails) : base(uniqueID, creationDetails)
		{
			IsPullableByUnit = true;
		}

		public override void OnAdvanceTime()
		{
			var player = (Player)PickByUniqueID(nameof(Player));

			if (player.PreviousPosition != Position && player.Position != Position)
				return;

			if (player.PreviousPosition == player.Position)
				return;

			var prevIsWater = World.PositionHasWaterAsHighest(player.PreviousPosition);
			var currIsWater = World.PositionHasWaterAsHighest(player.Position);

			if (prevIsWater == false && currIsWater == false)
				return;

			var changingBoats = false;
			var objs = objects[player.Position];

			for (int i = 0; i < objs.Count; i++)
				if (objs[i] != this && objs[i] is Boat)
					changingBoats = true;

			index = 0;
			if (player.Position.Y > player.PreviousPosition.Y) index = 1;
			else if (player.Position.X < player.PreviousPosition.X) index = 2;
			else if (player.Position.X > player.PreviousPosition.X) index = 3;

			if ((prevIsWater && currIsWater == false) || changingBoats)
			{
				TileIndexes = new(8 + index, 19) { C = TileIndexes.C };
				Position = player.PreviousPosition;
				return;
			}
			Position = player.Position;
			TileIndexes = new(12 + index, 23) { C = TileIndexes.C };
		}
		public Point GetPlayerTile() => new(16 + index, 23);
	}
}
