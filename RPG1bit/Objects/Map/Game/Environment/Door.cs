using SMPL.Data;
using System;

namespace RPG1bit
{
	public class Door : Object, IDeletableWhenFar
	{
		public bool Locked { get; set; }
		private bool close;

		public static void TryToCreate()
		{
			var player = (Player)Object.PickByUniqueID(nameof(Player));
			var pos = player.Position;
			var type = nameof(Door).ToLower();

			for (int i = 0; i < 3; i++)
			{
				var tile = Map.RawData.ContainsKey(pos) ? Map.RawData[pos][i] : new();
				var id = $"{type}-{pos}-{i}";
				if (MapEditor.Tiles[type].Contains(tile) && Object.UniqueIDsExits(id) == false)
				{
					var obj = new Door(id, new()
					{
						Position = pos,
						Height = i,
						Name = "-",
						TileIndexes = new Point[] { Map.RawData[pos][i] }
					});
					obj.OnAdvanceTime();
					Map.RawData[pos][i] = new();
				}
			}
		}
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

			var player = (Player)PickByUniqueID(nameof(Player));
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
				if (MoveCamera.IsAnchored)
					Map.CameraPosition = player.PreviousPosition;
				TileIndexes -= new Point(1, 0);
				NavigationPanel.Tab.Textbox.Text = "This door is locked.";
			}
		}
	}
}
