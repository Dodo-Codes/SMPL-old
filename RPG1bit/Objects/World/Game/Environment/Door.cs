using SMPL.Data;
using SMPL.Gear;

namespace RPG1bit
{
	public class Door : Object, IDeletableWhenFar
	{
		public bool Locked { get; set; }
		private bool update;

		public static void TryToCreate()
		{
			var player = (Player)PickByUniqueID(nameof(Player));
			var pos = player.Position;
			var type = nameof(Door);

			for (int i = 0; i < 3; i++)
			{
				var tile = ChunkManager.GetTile(pos, i);
				var id = $"{type}-{pos}-{i}";
				if (WorldEditor.Tiles[type].Contains(tile) && UniqueIDsExists(id) == false)
				{
					var obj = new Door(id, new()
					{
						Position = pos,
						Height = i,
						Name = "-",
						TileIndexes = new Point[] { tile }
					});
					obj.OnAdvanceTime();
					ChunkManager.SetTile(pos, i, new());
				}
			}
		}
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

			// can't peak inside for a single frame when door is locked
			if (player.Position == Position && Locked && PlayerHasKey(player) == false)
			{
				player.Position = player.PreviousPosition;
				if (MoveCamera.IsAnchored)
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
