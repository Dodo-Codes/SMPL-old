using Newtonsoft.Json;
using SMPL.Data;
using SMPL.Gear;

namespace RPG1bit
{
	public class Item : Object, ITypeTaggable
	{
		[JsonProperty]
		public string OwnerUID { get; set; }
		[JsonProperty]
		public uint Quantity { get; set; } = 1;
		[JsonProperty]
		public double[] Positives { get; set; } = new double[2];
		[JsonProperty]
		public double[] Negatives { get; set; } = new double[2];
		[JsonProperty]
		public bool CanWearOnHead { get; set; }
		[JsonProperty]
		public bool CanWearOnBody { get; set; }
		[JsonProperty]
		public bool CanWearOnFeet { get; set; }
		[JsonProperty]
		public bool CanCarryOnBack { get; set; }
		[JsonProperty]
		public bool CanCarryOnWaist { get; set; }
		[JsonProperty]
		public bool CanCarryInBag { get; set; }
		[JsonProperty]
		public bool CanCarryInQuiver { get; set; }

		public Item(string uniqueID, CreationDetails creationDetails) : base(uniqueID, creationDetails) { }

		public override void OnDroppedUpon()
		{
			var type = GetType();
			var holdingType = HoldingObject.GetType();
			if (HoldingObject is Item item && (holdingType == type ||
				type.IsSubclassOf(holdingType) || holdingType.IsSubclassOf(holdingType)))
			{
				Quantity += item.Quantity;
				Drop();
				var owner = PickByUniqueID(item.OwnerUID);
				if (owner is ItemPile pile)
					pile.RemoveItem(item);
				item.Destroy();
				ShowInfo();
			}
		}

		public override void OnDragStart()
		{
			if (this is SlotsExtender se && (OwnerUID.Contains("carry") || OwnerUID.Contains("hand")))
				for (int i = 0; i < se.Positives[0]; i++)
				{
					var pos = se.SlotsPosition + new Point(0, i);
					var objs = objects.ContainsKey(pos) ? objects[pos] : new();
					for (int j = 0; j < objs.Count; j++)
						if (objs[j] is Item)
						{
							Drop();
							return;
						}
				}
			UpdateSlotsColor(true);
		}
		public override void OnDragEnd() => UpdateSlotsColor(false);

		public override void OnRightClicked()
		{
			if (Quantity < 2) return;

			if (PickByUniqueID(OwnerUID) is ItemPile)
				for (int i = 0; i < 8; i++)
				{
					var groundSlot = (ItemSlot)PickByUniqueID($"ground-slot-{i}");
					if (groundSlot.HasItem() == false)
					{
						groundSlot.Equip(Split(OnSplit(), groundSlot, Quantity));
						var player = (Player)PickByUniqueID(nameof(Player));
						var objs = objects[player.Position];
						for (int j = 0; j < objs.Count; j++)
							if (objs[j] is ItemPile pile)
								pile.UpdateItems();
						break;
					}
				}
			NavigationPanel.Tab.Close();
			Screen.Display();
		}
		public override void OnLeftClicked() => ShowInfo();

		public override void OnHovered()
		{
			HoveredInfo = Quantity > 1 ? $"{Name} x{Quantity}" : Name;
		}

		private void ShowInfo()
		{
			if (IsDestroyed)
				return;
			ItemStats.DisplayedItemUID = UniqueID;
			OnItemInfoDisplay();
			NavigationPanel.Tab.Open("item-info", "item info");
		}
		private static void UpdateSlotsColor(bool holding)
		{
			var item = (Item)HoldingObject;

			SetTileIndex((ItemSlot)PickByUniqueID("slot-head"), item.CanWearOnHead);
			SetTileIndex((ItemSlot)PickByUniqueID("slot-body"), item.CanWearOnBody);
			SetTileIndex((ItemSlot)PickByUniqueID("slot-feet"), item.CanWearOnFeet);
			SetTileIndex((ItemSlot)PickByUniqueID("hand-left"), true);
			SetTileIndex((ItemSlot)PickByUniqueID("hand-right"), true);
			SetTileIndex((ItemSlot)PickByUniqueID("carry-back"), item.CanCarryOnBack);
			SetTileIndex((ItemSlot)PickByUniqueID("carry-waist"), item.CanCarryOnWaist);
			SetTileIndex((ItemSlot)PickByUniqueID("extra-slot-0"), item.CanCarryInBag);
			SetTileIndex((ItemSlot)PickByUniqueID("extra-slot-1"), item.CanCarryInBag);
			SetTileIndex((ItemSlot)PickByUniqueID("extra-slot-2"), item.CanCarryInQuiver);
			SetTileIndex((ItemSlot)PickByUniqueID("extra-slot-3"), item.CanCarryInQuiver);
			Screen.Display();

			void SetTileIndex(ItemSlot itemSlot, bool canCarry)
			{
				if (itemSlot == null) return;
				itemSlot.TileIndexes = new Point(itemSlot.TileIndexes.X, itemSlot.TileIndexes.Y)
					{ C = holding ? (canCarry && itemSlot.HasItem() == false ? Color.Green : Color.Red) : Color.Gray };
			}
		}

		public override void OnDisplay(Point screenPos)
		{
			Screen.EditCell(screenPos, new(Quantity, 26), 3, Color.White);
		}

		public Item Split(Item item, ItemSlot slot, uint quantity)
		{
			item.Position = slot.Position;
			item.Quantity = (uint)Number.Round((double)quantity / 2, toward: Number.RoundToward.Down);
			Quantity = (uint)Number.Round((double)quantity / 2, toward: Number.RoundToward.Up);
			return item;
		}

		public virtual Item OnSplit() => default;
		public virtual void OnItemInfoDisplay() { }
		public virtual void OnPickup() { }
		public virtual void OnDrop() { }
		public virtual void OnStore() { }
	}
}
