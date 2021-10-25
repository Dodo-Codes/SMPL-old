using SMPL.Data;

namespace RPG1bit
{
	public class Item : Object
	{
		public string OwnerUID { get; set; }
		public int Quantity { get; set; }

		public double[] Positives { get; set; } = new double[2];
		public double[] Negatives { get; set; } = new double[2];

		public bool CanWearOnHead { get; set; }
		public bool CanWearOnBody { get; set; }
		public bool CanWearOnFeet { get; set; }
		public bool CanCarryOnBack { get; set; }
		public bool CanCarryOnWaist { get; set; }
		public bool CanCarryInBag { get; set; }
		public bool CanCarryInQuiver { get; set; }

		public Item(string uniqueID, CreationDetails creationDetails) : base(uniqueID, creationDetails) { }

		public override void OnDragStart()
		{
			if (this is SlotsExtender se && (OwnerUID.Contains("carry") || OwnerUID.Contains("hand")))
				for (int i = 0; i < se.Positives[0]; i++)
				{
					var objs = objects[se.SlotsPosition + new Point(0, i)];
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

		public override void OnRightClicked() => ShowInfo();
		public override void OnLeftClicked() => ShowInfo();

		private void ShowInfo()
		{
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
			Screen.EditCell(screenPos, new(0, 26), 3, Color.White);
		}

		public virtual void OnItemInfoDisplay() { }
		public virtual void OnPickup() { }
		public virtual void OnDrop() { }
	}
}
