using Newtonsoft.Json;
using SMPL.Data;
using SMPL.Gear;

namespace RPG1bit
{
	public class Chest : Object, IInteractable, ITypeTaggable
	{
		[JsonProperty]
		public bool Locked { get; set; }
		private bool opened;

		public static void TryToCreate()
		{
			var player = (Player)PickByUniqueID(nameof(Player));
			var pos = player.Position;
			var positions = new Point[] { pos + new Point(0, 1), pos + new Point(0, -1), pos + new Point(1, 0), pos + new Point(-1, 0) };
			var type = nameof(Chest).ToLower();

			for (int j = 0; j < positions.Length; j++)
				for (int i = 0; i < 3; i++)
				{
					var tile = ChunkManager.GetTile(positions[j], i);
					var id = $"{type}-{positions[j]}-{i}";
					if (WorldEditor.Tiles[type].Contains(tile) && UniqueIDsExists(id) == false)
					{
						var obj = new Chest(id, new()
						{
							Position = positions[j],
							Height = i,
							Name = "-",
							TileIndexes = new Point[] { tile }
						});
						obj.OnAdvanceTime();
						ChunkManager.SetTile(positions[j], i, new());
					}
				}
		}
		public Chest(string uniqueID, CreationDetails creationDetails) : base(uniqueID, creationDetails)
		{
			AddTags("chest");
			Locked = TileIndexes.Y == 23;

			NavigationPanel.Tab.Texts[uniqueID] = "\nThe dusty chest cracks open\nto reveal all of its contents...";

			if (UniqueIDsExists($"{uniqueID}-item-slot-info"))
				return;

			for (int y = 0; y < 7; y++)
				for (int x = 0; x < 7; x++)
				{
					if ((x + y) % 2 == 0)
						continue;
					new ItemSlot($"{uniqueID}-{x}-{y}", new()
					{
						Position = new(22 + x, 6 + y),
						Height = 1,
						TileIndexes = new Point[] { new(7, 23) { C = Color.Brown } },
						IsInTab = true,
						AppearOnTab = uniqueID,
						IsUI = true,
						Name = "chest-slot"
					});
				}
		}
		public override void OnAdvanceTime()
		{
			var player = (Player)PickByUniqueID(nameof(Player));

			if (player.Position == Position)
			{
				player.Position = player.PreviousPosition;
				if (MoveCamera.IsAnchored)
					World.CameraPosition = player.Position;

				if (Locked)
				{
					var playerHasKey = false;
					for (int i = 0; i < player.ItemUIDs.Count; i++)
						if (PickByUniqueID(player.ItemUIDs[i]) is Key)
						{
							playerHasKey = true;
							break;
						}
					if (opened)
						Close();
					if (playerHasKey == false)
					{
						NavigationPanel.Tab.Textbox.Text = "This chest is locked.";
						return;
					}
				}
				NavigationPanel.Tab.Open(UniqueID, "chest");
				Screen.ScheduleDisplay();

				if (opened)
					return;

				TileIndexes += new Point(1, 0);
				opened = true;
			}
			else if (opened)
			{
				var pos = player.Position;
				player.Position = player.PreviousPosition;
				if (player.CellIsInReach(Position))
					Close();
				player.Position = pos;
			}
			void Close()
			{
				TileIndexes -= new Point(1, 0);
				opened = false;
				NavigationPanel.Tab.Close();
			}
		}
	}
}
