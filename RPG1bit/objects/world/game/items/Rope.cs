using SMPL.Data;
using SMPL.Gear;

namespace RPG1bit
{
	public class Rope : Item
	{
		public Rope(string uniqueID, CreationDetails creationDetails) : base(uniqueID, creationDetails)
		{
			TileIndexes = new(11, 14) { C = Color.Brown + 50 };
			MaxQuantity = 2;
			CanCarryOnWaist = true;
			CanCarryOnBack = true;
			CanCarryInBag = true;
			CanCarryInQuiver = true;
		}
		public override Item OnSplit() => CloneObject(this, $"{UniqueID}-{Performance.FrameCount}");
		public override void OnItemInfoDisplay()
		{
			NavigationPanel.Tab.Texts["item-info"] =
				$"\t   {nameof(Rope)} x{Quantity}/{MaxQuantity}\n\nUseful for pulling things.";
		}
	}
}
