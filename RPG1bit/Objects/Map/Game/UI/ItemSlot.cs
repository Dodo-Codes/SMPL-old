using SMPL.Data;
using SMPL.Gear;

namespace RPG1bit
{
	public class ItemSlot : Object
	{
		public ItemSlot(string uniqueID, CreationDetails creationDetails) : base(uniqueID, creationDetails) { }

		public override void OnDroppedUpon()
		{
			var item = (Item)HoldingObject;
			if (UniqueID == "head" && item.CanWearOnHead) Equip(item);
			else if (UniqueID == "body" && item.CanWearOnBody) Equip(item);
			else if (UniqueID == "feet" && item.CanWearOnFeet) Equip(item);
			else if (UniqueID == "hand-left" && item.CanHold) Equip(item);
			else if (UniqueID == "hand-right" && item.CanHold) Equip(item);
			else if (UniqueID == "carry-back" && item.CanCarryOnBack) Equip(item);
			else if (UniqueID == "carry-waist" && item.CanCarryOnWaist) Equip(item);
			else if (UniqueID.Contains("ground")) Equip(item);
		}
		public void Equip(Item item)
		{
			if (PickByUniqueID(item.OwnerUID) is ItemPile pile)
				pile.RemoveItem(item);

			item.Position = Position;
			item.OwnerUID = UniqueID;

			var player = (Player)PickByUniqueID("player");
			if (UniqueID.Contains("ground"))
			{
				var objs = objects[player.Position];
				for (int i = 0; i < objs.Count; i++)
					if (objs[i] is ItemPile existingPile)
					{
						existingPile.AddItem(item);
						return;
					}

				var newPile = new ItemPile($"{player.Position}-{Performance.FrameCount}", new()
				{
					Position = player.Position,
					Height = 3,
					TileIndexes = new Point[] { new(8, 23) },
					Name = "item-pile",
				});
				newPile.AddItem(item);
				if (player.ItemUIDs.Contains(item.UniqueID))
					player.ItemUIDs.Remove(item.UniqueID);

				// the new pile renders on top of the player otherwise
				objects[newPile.Position].Remove(player);
				objects[newPile.Position].Add(player);
			}
			else if (player.ItemUIDs.Contains(item.UniqueID) == false)
				player.ItemUIDs.Add(item.UniqueID);
		}
	}
}
