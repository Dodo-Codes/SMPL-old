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
			var player = (Player)Object.PickByUniqueID(nameof(Player));
			var pos = player.Position;
			var type = nameof(Door).ToLower();

			for (int i = 0; i < 3; i++)
			{
				var tile = World.RawData.ContainsKey(pos) ? World.RawData[pos][i] : new();
				var id = $"{type}-{pos}-{i}";
				if (WorldEditor.Tiles[type].Contains(tile) && Object.UniqueIDsExits(id) == false)
				{
					var obj = new Door(id, new()
					{
						Position = pos,
						Height = i,
						Name = "-",
						TileIndexes = new Point[] { World.RawData[pos][i] }
					});
					obj.OnAdvanceTime();
					World.RawData[pos][i] = new();
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

			var playerHasKey = false;
			for (int i = 0; i < player.ItemUIDs.Count; i++)
				if (PickByUniqueID(player.ItemUIDs[i]) is Key)
				{
					playerHasKey = true;
					break;
				}
			if (player.Position == Position && Locked && playerHasKey == false)
			{
				player.Position = player.PreviousPosition;
				if (MoveCamera.IsAnchored)
					World.CameraPosition = player.PreviousPosition;
				TileIndexes -= new Point(1, 0);
				NavigationPanel.Tab.Textbox.Text = "This door is locked.";
			}
		}
		public override void OnAdvanceTime() => update = true;
	}
}
