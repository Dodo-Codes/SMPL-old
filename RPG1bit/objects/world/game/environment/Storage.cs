using Newtonsoft.Json;
using SMPL.Data;
using SMPL.Gear;
using System.Collections.Generic;

namespace RPG1bit
{
	public class Storage : PlayerFollower, IInteractable, ITypeTaggable
	{
		[JsonProperty]
		public bool Locked { get; set; }
		private bool opened, justCreated, canBeOpened;

		public Storage(string uniqueID, CreationDetails creationDetails) : base(uniqueID, creationDetails)
		{
			Locked = TileIndexes.Y == 23;

			NavigationPanel.Tab.Texts[uniqueID] = $"\nThe storage reveals\n" +
				$"all of its contents...";

			if (UniqueIDsExists($"{uniqueID}-item-slot-info"))
				return;

			justCreated = true;
		}
		public override void OnAdvanceTime()
		{
			if (justCreated)
			{
				justCreated = false;
				canBeOpened = false;
				var size = 7;
				Name = "chest";

				if (Is(5, 7)) { Name = "big drawer"; size = 5; canBeOpened = true; }
				else if (Is(7, 7)) { Name = "small drawer"; size = 3; canBeOpened = true; }
				else if (Is(8, 18) || Is(9, 18)) { Name = "cart"; size = 7; }
				else if (Is(6, 17) || Is(7, 17) || Is(8, 17) || Is(9, 17)) { Name = "chariot"; size = 5; }
				else if (Is(6, 16) || Is(7, 16) || Is(8, 16) || Is(9, 16)) { Name = "wheelbarrow"; size = 3; }
				else canBeOpened = true;

				bool Is(int x, int y) => TileIndexes == new Point(x, y);


				for (int y = 0; y < size; y++)
					for (int x = 0; x < 7; x++)
					{
						if ((x + y) % 2 == 0)
							continue;

						var off = (7 - size) / 2;
						new ItemSlot($"{UniqueID}-{x}-{y}", new()
						{
							Position = new(22 + x, 6 + y + off),
							Height = 1,
							TileIndexes = new Point[] { new(7, 23) { C = Color.Brown } },
							IsInTab = true,
							AppearOnTab = UniqueID,
							IsUI = true,
							Name = Name
						});
					}
			}

			if (Keyboard.KeyIsPressed(Keyboard.Key.ControlLeft))
				base.OnAdvanceTime();

			var player = (Player)PickByUniqueID(nameof(Player));
			if (player.Position == Position)
			{
				player.Position = player.PreviousPosition;
				World.CameraPosition = player.Position;

				if (canBeOpened && Locked)
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
						PlayerStats.Open($"This {Name.ToLower()} is locked.");
						return;
					}
				}
				NavigationPanel.Tab.Open(UniqueID, Name.ToLower());
				Screen.ScheduleDisplay();

				if (canBeOpened && opened)
					return;

				if (canBeOpened)
				{
					TileIndexes += new Point(1, 0);
					opened = true;
				}
			}
			else if (canBeOpened && opened)
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
			}
		}
	}
}
