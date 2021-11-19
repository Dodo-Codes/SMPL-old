using SMPL.Data;
using SMPL.Gear;

namespace RPG1bit
{
	public class Door : Object, IDeletableWhenFar
	{
		public bool Locked { get; set; }
		private bool update;

		public Door(string uniqueID, CreationDetails creationDetails) : base(uniqueID, creationDetails)
		{
			Locked = TileIndexes.Y == 23;
			Game.Event.Subscribe.LateUpdate(uniqueID);
		}
		public override void OnGameLateUpdate()
		{
			if (update == false)
				return;

			update = false;
			var player = (Player)PickByUniqueID(nameof(Player));
			if (player.Position == player.PreviousPosition)
				return;

			if (player.Position == Position)
				TileIndexes += new Point(1, 0);
			else if (player.PreviousPosition == Position)
				TileIndexes += new Point(-1, 0);

			if (player.Position == Position && Locked && PlayerHasKey(player) == false)
				TileIndexes -= new Point(1, 0);
			Screen.ScheduleDisplay();
		}
		public override void OnAdvanceTime()
		{
			var player = (Player)PickByUniqueID(nameof(Player));

			if (player.Position == Position && Locked && PlayerHasKey(player) == false)
			{
				player.Position = player.PreviousPosition;
				World.CameraPosition = player.PreviousPosition;
				PlayerStats.Open("This door is locked.");
			}
			update = true;
		}
		private static bool PlayerHasKey(Player player)
		{
			for (int i = 0; i < player.ItemUIDs.Count; i++)
				if (PickByUniqueID(player.ItemUIDs[i]) is Key)
					return true;
			return false;
		}
	}
}
