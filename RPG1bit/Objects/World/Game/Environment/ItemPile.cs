using Newtonsoft.Json;
using SMPL.Data;
using System.Collections.Generic;

namespace RPG1bit
{
	public class ItemPile : Object, ITypeTaggable
	{
		public const int MAX_COUNT = 8;
		[JsonProperty]
		private List<string> ItemUIDs { get; set; } = new();

		public ItemPile(string uniqueID, CreationDetails creationDetails) : base(uniqueID, creationDetails)
		{
			Height = 3;
			TileIndexes = new(8, 23);
			Name = "item-pile";
		}

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
		public override void OnAdvanceTime() => UpdateItems();

		public void UpdateItems()
		{
			var player = (Player)PickByUniqueID(nameof(Player));

			if (ItemUIDs.Count == 0)
				Destroy();
			for (int i = 0; i < ItemUIDs.Count; i++)
			{
				var thing = PickByUniqueID(ItemUIDs[i]);
				var item = (Item)thing;
				item.Position = new Point(Position == player.Position ? 10 + i : -10, 0);
			}
		}
		public override void OnDisplay(Point screenPos)
		{
			var hide = World.TileHasRoof(World.ScreenToWorldPosition(screenPos)) && World.IsShowingRoofs;
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
