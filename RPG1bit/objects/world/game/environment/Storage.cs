using Newtonsoft.Json;
using SMPL.Data;
using SMPL.Gear;
using System.Collections.Generic;

namespace RPG1bit
{
	public class Storage : GameObject, IInteractable, ITypeTaggable, ISolid, IRecreatable
	{
		[JsonProperty]
		public bool Locked { get; set; }
		[JsonProperty]
		private bool opened, justCreated, canBeOpened;
		private int size;

		public Storage(string uniqueID, CreationDetails creationDetails) : base(uniqueID, creationDetails)
		{
			Locked = TileIndexes.Y == 23;

			NavigationPanel.Tab.Texts[uniqueID] = $"\nThe storage reveals\n" +
				$"all of its contents...";

			if (UniqueIDsExists($"{uniqueID}-item-slot-info"))
				return;

			justCreated = true;
		}

		public override void Destroy()
		{
			for (int y = 0; y < size; y++)
				for (int x = 0; x < 7; x++)
				{
					if ((x + y) % 2 == 0)
						continue;

					var id = $"{UniqueID}-{x}-{y}";
					if (UniqueIDsExists(id))
						PickByUniqueID(id).Destroy();
				}
			base.Destroy();
		}
		public override void OnAdvanceTime()
		{
			if (justCreated)
			{
				justCreated = false;
				canBeOpened = false;
				IsPullableByUnit = true;
				size = 7;
				Name = "chest";

				if (Is(5, 7)) { Name = "big drawer"; size = 5; canBeOpened = true; IsPullableByUnit = false; }
				else if (Is(7, 7)) { Name = "small drawer"; size = 3; canBeOpened = true; IsPullableByUnit = false; }
				else if (Is(8, 18) || Is(9, 18)) { Name = "cart"; size = 7; }
				else if (Is(6, 17) || Is(7, 17) || Is(8, 17) || Is(9, 17)) { Name = "chariot"; size = 5; PullRequiredType = nameof(Rope); }
				else if (Is(6, 16) || Is(7, 16) || Is(8, 16) || Is(9, 16)) { Name = "wheelbarrow"; size = 3; }
				else { canBeOpened = true; IsPullableByUnit = false; } // chest

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
							TileIndexes = new Point[] { new(7, 23) { Color = Color.Brown } },
							IsInTab = true,
							AppearOnTab = UniqueID,
							IsUI = true,
							Name = Name
						}) { WorldPosition = Position };
					}
			}

			var player = (Player)PickByUniqueID(nameof(Player));
			if (player.Position == Position)
			{
				if (canBeOpened && Locked)
				{
					var playerHasKey = player.HasItem<Key>();
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
			if (PulledByUnitUID != null)
			{
				var unit = (Unit)PickByUniqueID(PulledByUnitUID);

				//if (Name == "cart")
				//{
				//	var objs = objects[unit.Position];
				//	for (int i = 0; i < objs.Count; i++)
				//	
				//}

				var angle = (int)Number.AngleBetweenPoints(Position, unit.Position);
				var angles = new Dictionary<string, Dictionary<int, Point>>()
				{
					{ "cart", new() { { 90, new(8, 18) }, { 270, new(8, 18) }, { 0, new(9, 18) }, { 180, new(9, 18) } } },
					{ "chariot", new() { { 90, new(6, 17) }, { 270, new(7, 17) }, { 0, new(8, 17) }, { 180, new(9, 17) } } },
					{ "wheelbarrow", new() { { 90, new(8, 16) }, { 270, new(9, 16) }, { 0, new(7, 16) }, { 180, new(6, 16) } } },
				};
				var tile = angles[Name][angle];
				TileIndexes = new Point(tile.X, tile.Y) { Color = TileIndexes.Color };
			}

			void Close()
			{
				TileIndexes -= new Point(1, 0);
				opened = false;
			}
		}
	}
}
