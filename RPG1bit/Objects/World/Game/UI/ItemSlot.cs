using SMPL.Data;
using SMPL.Gear;

namespace RPG1bit
{
	public class ItemSlot : Object
	{
		public ItemSlot(string uniqueID, CreationDetails creationDetails) : base(uniqueID, creationDetails) { }

		public override void OnHovered()
		{
			if (Name != "chest-slot")
				return;

			HoveredInfo = "...in the chest.";
		}
		public override void OnDroppedUpon()
		{
			if (HasItem()) return;

			var item = (Item)HoldingObject;
			if (UniqueID == "slot-head" && item.CanWearOnHead) Equip(item);
			else if (UniqueID == "slot-body" && item.CanWearOnBody) Equip(item);
			else if (UniqueID == "slot-feet" && item.CanWearOnFeet) Equip(item);
			else if (UniqueID == "hand-left" || UniqueID == "hand-right") Equip(item);
			else if (UniqueID == "carry-back" && item.CanCarryOnBack) Equip(item);
			else if (UniqueID == "carry-waist" && item.CanCarryOnWaist) Equip(item);
			else if (UniqueID == "extra-slot-0" && item.CanCarryInBag) Equip(item);
			else if (UniqueID == "extra-slot-1" && item.CanCarryInBag) Equip(item);
			else if (UniqueID == "extra-slot-2" && item.CanCarryInQuiver) Equip(item);
			else if (UniqueID == "extra-slot-3" && item.CanCarryInQuiver) Equip(item);
			else if (UniqueID.Contains("ground") || UniqueID.Contains("chest")) Equip(item);
		}
		public void Equip(Item item)
		{
			if (PickByUniqueID(item.OwnerUID) is ItemPile pile)
				pile.RemoveItem(item);

			item.Position = Position;
			item.OwnerUID = UniqueID;
			item.AppearOnTab = AppearOnTab;
			item.IsInTab = IsInTab;
			Screen.ScheduleDisplay();

			var player = (Player)PickByUniqueID(nameof(Player));
			if (UniqueID.Contains("ground"))
			{
				if (player.ItemUIDs.Contains(item.UniqueID))
					player.ItemUIDs.Remove(item.UniqueID);

				var objs = objects[player.Position];
				for (int i = 0; i < objs.Count; i++)
					if (objs[i] is ItemPile existingPile)
					{
						existingPile.AddItem(item);
						item.OnDrop();
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
				item.OnDrop();
			}
			else if (UniqueID.Contains("chest"))
			{
				if (player.ItemUIDs.Contains(item.UniqueID))
					player.ItemUIDs.Remove(item.UniqueID);
				item.OnStore();
			}
			else if (player.ItemUIDs.Contains(item.UniqueID) == false)
			{
				player.ItemUIDs.Add(item.UniqueID);
				item.OnPickup();
			}
		}
		public bool HasItem() => objects[Position].Count > 1 && objects[Position][1] is Item;
	}
}
