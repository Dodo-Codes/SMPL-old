using SMPL.Data;

namespace RPG1bit
{
	public class Bag : SlotsExtender
	{
		public Bag(string uniqueID, CreationDetails creationDetails) : base(uniqueID, creationDetails)
		{
			CanCarryOnBack = true;
			SlotsPosition = new Point(0, 14);
		}

		public override void OnItemInfoDisplay()
		{
			NavigationPanel.Tab.Texts["item-info"] = $"\t\t\t  Bag\n\n\tA small bag, great for\n\t  carrying essentials.\n" +
				$"Can fit up to [+PRIME] items.";
		}
	}
}
