using SMPL.Data;

namespace RPG1bit
{
	public class ItemStats : Object
	{
		public static string DisplayedItemUID { get; set; }

		public ItemStats(string uniqueID, CreationDetails creationDetails) : base(uniqueID, creationDetails) { }

		public override void OnDisplay(Point screenPos)
		{
			var isPositive = Name == "positives";
			var item = (Item)PickByUniqueID(DisplayedItemUID);
			var prime = isPositive ? item.Positives[0] : item.Negatives[0];
			var minor = isPositive ? item.Positives[1] : item.Negatives[1];
			var color = isPositive ? Color.Green : Color.Red;

			Screen.DisplayText(screenPos, 1, color, $" prime {prime}");
			Screen.DisplayText(screenPos + new Point(0, 1), 1, color - 100, $" minor {minor}");
			Screen.EditCell(screenPos, isPositive ? new(36, 20) : new(37, 20), 1, color);
			Screen.EditCell(screenPos + new Point(0, 1), isPositive ? new(36, 20) : new(37, 20), 1, color - 100);
		}
	}
}
