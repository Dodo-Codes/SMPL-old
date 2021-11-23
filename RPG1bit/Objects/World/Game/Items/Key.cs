using SMPL.Data;
using SMPL.Gear;

namespace RPG1bit
{
	public class Key : Item
	{
		public Key(string uniqueID, CreationDetails creationDetails) : base(uniqueID, creationDetails)
		{
			TileIndexes = new(32 + Probability.Randomize(new(0, 2)), 11) { Color = Color.Gray };
			MaxQuantity = 6;
			CanCarryOnWaist = true;
			CanCarryInBag = true;
			CanCarryInQuiver = true;
		}
		public override Item OnSplit() => CloneObject(this, $"{UniqueID}-{Performance.FrameCount}");
		public override void OnItemInfoDisplay()
		{
			NavigationPanel.Tab.Texts["item-info"] =
				$"\t\t\t\t{nameof(Key)} x{Quantity}/{MaxQuantity}\n\nGreat for unlocking locked things.";
		}
	}
}
