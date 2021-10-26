using SMPL.Data;
using SMPL.Gear;

namespace RPG1bit
{
	public class Key : Item
	{
		public Key(string uniqueID, CreationDetails creationDetails) : base(uniqueID, creationDetails)
		{
			CanCarryOnWaist = true;
			CanCarryInBag = true;
			CanCarryInQuiver = true;
		}

		public override Item OnSplit(ItemSlot s, uint q) => Split(CloneObject(this, $"{UniqueID}-{Performance.FrameCount}"), s, q);
		public override void OnItemInfoDisplay()
		{
			NavigationPanel.Tab.Texts["item-info"] = $"\t\t\t\tKey x{Quantity}\n\nGreat for unlocking locked things.";
		}
	}
}
