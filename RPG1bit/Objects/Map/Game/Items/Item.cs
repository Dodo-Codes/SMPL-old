using SMPL.Data;

namespace RPG1bit
{
	public class Item : Object
	{
		public string OwnerUID { get; set; }

		public double[] Positives { get; set; } = new double[2];
		public double[] Negatives { get; set; } = new double[2];

		public bool CanHold { get; set; }
		public bool CanWearOnHead { get; set; }
		public bool CanWearOnBody { get; set; }
		public bool CanWearOnFeet { get; set; }
		public bool CanCarryOnBack { get; set; }
		public bool CanCarryOnWaist { get; set; }
		public bool CanCarryInBag { get; set; }
		public bool CanCarryInQuiver { get; set; }

		public Item(string uniqueID, CreationDetails creationDetails) : base(uniqueID, creationDetails) { }

		public override void OnDragStart() => UpdateSlotsColor(true);
		public override void OnDragEnd() => UpdateSlotsColor(false);

		public override void OnRightClicked() => ShowInfo();
		public override void OnLeftClicked() => ShowInfo();
		public virtual void OnItemInfoDisplay() { }

		private void ShowInfo()
		{
			ItemStats.DisplayedItemUID = UniqueID;
			OnItemInfoDisplay();
			NavigationPanel.Tab.Open("item-info", "item info");
		}
		private static void UpdateSlotsColor(bool holding)
		{
			var item = (Item)HoldingObject;

			SetTileIndex((ItemSlot)PickByUniqueID("head"), item.CanWearOnHead);
			SetTileIndex((ItemSlot)PickByUniqueID("body"), item.CanWearOnBody);
			SetTileIndex((ItemSlot)PickByUniqueID("feet"), item.CanWearOnFeet);
			SetTileIndex((ItemSlot)PickByUniqueID("hand-left"), item.CanHold);
			SetTileIndex((ItemSlot)PickByUniqueID("hand-right"), item.CanHold);
			SetTileIndex((ItemSlot)PickByUniqueID("carry-back"), item.CanCarryOnBack);
			SetTileIndex((ItemSlot)PickByUniqueID("carry-waist"), item.CanCarryOnWaist);
			Screen.Display();

			void SetTileIndex(ItemSlot itemSlot, bool canCarry)
			{
				itemSlot.TileIndexes = new Point(itemSlot.TileIndexes.X, itemSlot.TileIndexes.Y)
					{ C = holding ? (canCarry ? Color.Green : Color.Red) : Color.Gray };
			}
		}
	}
}
