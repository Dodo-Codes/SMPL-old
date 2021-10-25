using SMPL.Data;
using System.Collections.Generic;

namespace RPG1bit
{
	public class ItemPile : Object
	{
		public const int MAX_COUNT = 8;
		private List<string> ItemUIDs { get; set; } = new();

		public ItemPile(string uniqueID, CreationDetails creationDetails) : base(uniqueID, creationDetails) { }

		public void AddItem(Item item)
		{
			ItemUIDs.Add(item.UniqueID);
			item.OwnerUID = UniqueID;
		}
		public void RemoveItem(Item item)
		{
			ItemUIDs.Remove(item.UniqueID);
			item.OwnerUID = null;
		}
		public override void OnAdvanceTime()
		{
			var player = (Player)PickByUniqueID("player");

			if (ItemUIDs.Count == 0)
				Destroy();

			for (int i = 0; i < ItemUIDs.Count; i++)
			{
				var item = (Item)PickByUniqueID(ItemUIDs[i]);
				item.Position = new Point(Position == player.Position ? 10 + i : -10, 0);
			}
		}
		public override void OnDisplay(Point screenPos)
		{
			var hide = Map.TileHasRoof(Map.ScreenToMapPosition(screenPos)) && Map.IsShowingRoofs;
			var xOffset = 0;
			switch (ItemUIDs.Count)
			{
				case 3: case 4: xOffset = 1; break;
				case 5: case 6: xOffset = 2; break;
				case 7: case 8: xOffset = 3; break;
			}
			var tile = new Point(8 + xOffset, 23.0);
			Screen.EditCell(screenPos, hide ? new() : tile, 3, hide ? new() : Color.White);
		}
	}
}
