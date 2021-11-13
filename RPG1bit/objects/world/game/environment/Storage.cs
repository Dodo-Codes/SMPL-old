using Newtonsoft.Json;
using SMPL.Data;
using System.Collections.Generic;

namespace RPG1bit
{
	public class Storage : Object, IInteractable, ITypeTaggable
	{
		private static readonly Dictionary<string, int> sizes = new()
		{
			{ "chest", 7 }, { "big drawer", 5 }, { "small drawer", 3 }
		};

		[JsonProperty]
		public bool Locked { get; set; }

		private bool opened;

		public static void TryToCreate()
		{
			var player = (Player)PickByUniqueID(nameof(Player));
			var pos = player.Position;
			var positions = new Point[] { pos + new Point(0, 1), pos + new Point(0, -1), pos + new Point(1, 0), pos + new Point(-1, 0) };

			Try("chest");
			Try("big drawer");
			Try("small drawer");

			void Try(string type)
			{
				for (int j = 0; j < positions.Length; j++)
					for (int i = 0; i < 3; i++)
					{
						var tile = ChunkManager.GetTile(positions[j], i);
						var id = $"{type}-{positions[j]}-{i}";
						if (WorldEditor.Tiles[type].Contains(tile) && UniqueIDsExists(id) == false)
						{
							var obj = new Storage(id, new()
							{
								Position = positions[j],
								Height = i,
								Name = type,
								TileIndexes = new Point[] { tile }
							});


							obj.OnAdvanceTime();
							ChunkManager.SetTile(positions[j], i, new());
						}
					}
			}
		}
		public Storage(string uniqueID, CreationDetails creationDetails) : base(uniqueID, creationDetails)
		{
			Locked = TileIndexes.Y == 23;

			NavigationPanel.Tab.Texts[uniqueID] = $"\nThe dusty {Name.ToLower()} cracks open\nto reveal all of its contents...";

			if (UniqueIDsExists($"{uniqueID}-item-slot-info"))
				return;

			var sz = sizes[Name];
			for (int y = 0; y < sz; y++)
				for (int x = 0; x < 7; x++)
				{
					if ((x + y) % 2 == 0)
						continue;

					var off = (7 - sz) / 2;
					new ItemSlot($"{uniqueID}-{x}-{y}", new()
					{
						Position = new(22 + x, 6 + y + off),
						Height = 1,
						TileIndexes = new Point[] { new(7, 23) { C = Color.Brown } },
						IsInTab = true,
						AppearOnTab = uniqueID,
						IsUI = true,
						Name = Name
					});
				}
		}
		public override void OnAdvanceTime()
		{
			var player = (Player)PickByUniqueID(nameof(Player));

			if (player.Position == Position)
			{
				player.Position = player.PreviousPosition;
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
						PlayerStats.Open($"This {Name.ToLower()} is locked.");
						return;
					}
				}
				NavigationPanel.Tab.Open(UniqueID, Name.ToLower());
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
			}
		}
	}
}
