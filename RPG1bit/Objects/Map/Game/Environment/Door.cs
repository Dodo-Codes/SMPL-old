using SMPL.Data;

namespace RPG1bit
{
	public class Door : Object
	{
		public bool Locked { get; set; }
		private bool close;

		public Door(string uniqueID, CreationDetails creationDetails) : base(uniqueID, creationDetails)
		{
			Locked = TileIndexes.Y == 23;
		}

		public override void OnAdvanceTime()
		{
			var justClosed = false;
			if (close)
			{
				TileIndexes += new Point(-1, 0);
				close = false;
				justClosed = true;
			}

			var player = (Player)PickByUniqueID("player");
			if (player.Position == Position) TileIndexes += new Point(1, 0);
			else if (player.PreviousPosition == Position) close = true;

			var playerHasKey = false;
			for (int i = 0; i < player.ItemUIDs.Count; i++)
				if (PickByUniqueID(player.ItemUIDs[i]) is Key)
				{
					playerHasKey = true;
					break;
				}
			if (player.Position == Position && justClosed == false && Locked && playerHasKey == false)
			{
				player.Position = player.PreviousPosition;
				TileIndexes -= new Point(1, 0);
				NavigationPanel.Tab.Textbox.Text = "This door is locked.";
			}
		}
	}
}
