using SMPL.Data;

namespace RPG1bit
{
	public class Chest : DeletableWhenFar
	{
		public bool Locked { get; set; }

		public Chest(string uniqueID, CreationDetails creationDetails) : base(uniqueID, creationDetails)
		{
			Locked = TileIndexes.Y == 23;
		}

		public override void OnRightClicked()
		{
			var player = (Player)PickByUniqueID("player");

			if (Locked)
			{
				var playerHasKey = false;
				for (int i = 0; i < player.ItemUIDs.Count; i++)
					if (PickByUniqueID(player.ItemUIDs[i]) is Key)
					{
						playerHasKey = true;
						break;
					}
				if (playerHasKey == false)
				{
					NavigationPanel.Tab.Textbox.Text = "This chest is locked.";
					return;
				}
			}
			TileIndexes += new Point(1, 0);
			Open();
		}
		public override void OnAdvanceTime()
		{
			var player = (Player)PickByUniqueID("player");

			if (player.CellIsInReach(Position))
				return;

			TileIndexes -= new Point(1, 0);
		}
		public void Open()
		{

		}
	}
}
