using SMPL.Data;

namespace RPG1bit
{
	public class ItemStats : Object
	{
		public static string DisplayedItemUID { get; set; }

		public ItemStats(string uniqueID, CreationDetails creationDetails) : base(uniqueID, creationDetails) { }

		public override void OnDisplay(Point screenPos)
		{
			var item = (Item)PickByUniqueID(DisplayedItemUID);
			if (item == null)
				return;

			foreach (var kvp in item.Stats)
				Screen.DisplayText(screenPos, 1, Color.White, $"{kvp.Value} {Item.GetStatName(kvp.Key, kvp.Value)}");

			var x = item.Stats.Count == 0 ? 24 : 28;
			var off = x == 24 ? 1 : 0;
			Screen.EditCell(new(x + 2, 9), new(08, 22), 1, Color.Green);
			Screen.EditCell(new(x + 3, 9), new(09, 22), 1, Color.Green);

			Screen.EditCell(new(x - off, 10), new(05, 22), 1, item.CanWearOnHead ? Color.Green : Color.Red);
			Screen.EditCell(new(x - off, 11), new(06, 22), 1, item.CanWearOnBody ? Color.Green : Color.Red);
			Screen.EditCell(new(x - off, 12), new(07, 22), 1, item.CanWearOnFeet ? Color.Green : Color.Red);

			Screen.EditCell(new(x + 2, 11), new(10, 22), 1, item.CanCarryOnBack ? Color.Green : Color.Red);
			Screen.EditCell(new(x + 2, 12), new(11, 22), 1, item.CanCarryOnWaist ? Color.Green : Color.Red);

			Screen.EditCell(new(x + 3, 11), new(44, 04), 1, item.CanCarryInBag ? Color.Green : Color.Red);
			Screen.EditCell(new(x + 3, 12), new(43, 06), 1, item.CanCarryInQuiver ? Color.Green : Color.Red);
		}
	}
}
