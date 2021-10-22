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
			var isBeneathRoof = Map.TileHasRoof(Map.ScreenToMapPosition(screenPos));
			Screen.EditCell(screenPos, isBeneathRoof? new() : new(8 + ItemUIDs.Count / 2, 23), 3, isBeneathRoof ? new() : Color.White);
		}
	}
}
