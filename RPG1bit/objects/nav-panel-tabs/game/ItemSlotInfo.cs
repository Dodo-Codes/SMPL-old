using SMPL.Data;

namespace RPG1bit
{
	public class ItemSlotInfo : Object
	{
		public ItemSlotInfo(string uniqueID, CreationDetails creationDetails) : base(uniqueID, creationDetails) { }

		public override void OnDisplay(Point screenPos)
		{
			var item = (Item)PickByUniqueID(ItemStats.DisplayedItemUID);
			if (item == null)
				return;
			Screen.EditCell(new(30, 9), new(08, 22), 1, Color.Green);
			Screen.EditCell(new(31, 9), new(09, 22), 1, Color.Green);

			Screen.EditCell(new(28, 10), new(05, 22), 1, item.CanWearOnHead ? Color.Green : Color.Red);
			Screen.EditCell(new(28, 11), new(06, 22), 1, item.CanWearOnBody ? Color.Green : Color.Red);
			Screen.EditCell(new(28, 12), new(07, 22), 1, item.CanWearOnFeet ? Color.Green : Color.Red);

			Screen.EditCell(new(30, 11), new(10, 22), 1, item.CanCarryOnBack ? Color.Green : Color.Red);
			Screen.EditCell(new(30, 12), new(11, 22), 1, item.CanCarryOnWaist ? Color.Green : Color.Red);

			Screen.EditCell(new(31, 11), new(44, 04), 1, item.CanCarryInBag ? Color.Green : Color.Red);
			Screen.EditCell(new(31, 12), new(43, 06), 1, item.CanCarryInQuiver ? Color.Green : Color.Red);
		}
	}
}
