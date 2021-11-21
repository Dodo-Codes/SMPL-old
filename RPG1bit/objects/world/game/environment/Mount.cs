using SMPL.Data;
using SMPL.Gear;

namespace RPG1bit
{
	public class Mount : Object, IDeletableWhenFar, IRidable
	{
		private int index;
		private Point baseTileIndexes;

		public Mount(string uniqueID, CreationDetails creationDetails) : base(uniqueID, creationDetails)
		{
			IsPullableByUnit = true;
			if (Name == "Horse") baseTileIndexes = new(27, 7);
		}

		public override void OnAdvanceTime()
		{
			var player = (Player)PickByUniqueID(nameof(Player));

			if (player.PreviousPosition != Position && player.Position != Position)
				return;

			if (player.PreviousPosition == player.Position)
				return;

			if (World.PositionHasWaterAsHighest(player.Position) ||
				(Keyboard.KeyIsPressed(Keyboard.Key.ShiftLeft) && player.PullingUID == null))
				return;

			var changingRidable = false;
			var objs = objects[player.Position];

			for (int i = 0; i < objs.Count; i++)
				if (objs[i] != this && objs[i] is IRidable)
					changingRidable = true;

			var vertical = (int)(TileIndexes.X - baseTileIndexes.X);
			index = 0;
			if (player.Position.X > player.PreviousPosition.X) index = 1;
			else if (player.Position.Y > player.PreviousPosition.Y) index = vertical;
			else if (player.Position.Y < player.PreviousPosition.Y) index = vertical;

			if (changingRidable)
			{
				Position = player.PreviousPosition;
				return;
			}
			Position = player.Position;
			TileIndexes = new(baseTileIndexes.X + index, baseTileIndexes.Y) { C = TileIndexes.C };
		}
		public Point GetPlayerTile() => new(18 + index, 23);
	}
}
