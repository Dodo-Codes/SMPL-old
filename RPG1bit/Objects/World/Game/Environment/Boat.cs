using SMPL.Data;
using SMPL.Gear;

namespace RPG1bit
{
	public class Boat : GameObject, IRecreatable, IRidable
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

			if ((prevIsWater == false && currIsWater == false))
				return;

			var changingRidable = false;
			var objs = objects[player.Position];

			for (int i = 0; i < objs.Count; i++)
				if (objs[i] != this && objs[i] is IRidable)
					changingRidable = true;

			index = 0;
			if (player.Position.Y > player.PreviousPosition.Y) index = 1;
			else if (player.Position.X < player.PreviousPosition.X) index = 2;
			else if (player.Position.X > player.PreviousPosition.X) index = 3;

			if ((prevIsWater && currIsWater == false) || changingRidable ||
				(Keyboard.KeyIsPressed(Keyboard.Key.ShiftLeft) && player.PullingUID == null))
			{
				TileIndexes = new(8 + index, 19) { Color = TileIndexes.Color };
				Position = player.PreviousPosition;
				return;
			}
			Position = player.Position;
			TileIndexes = new(12 + index, 23) { Color = TileIndexes.Color };
		}
		public Point GetPlayerTile() => new(16 + index, 23);
	}
}
