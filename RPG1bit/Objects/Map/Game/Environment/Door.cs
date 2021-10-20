using SMPL.Data;

namespace RPG1bit
{
	public class Door : Object
	{
		private bool close;

		public Door(string uniqueID, CreationDetails creationDetails) : base(uniqueID, creationDetails) { }

		public override void OnAdvanceTime()
		{
			if (close)
			{
				TileIndexes += new Point(-1, 0);
				close = false;
			}

			var player = (Player)PickByUniqueID("player");
			if (player.Position == Position) TileIndexes += new Point(1, 0);
			else if (player.PreviousPosition == Position) close = true;
		}
	}
}
