using SMPL.Data;

namespace RPG1bit
{
	public class Bag : SlotsExtender
	{
		public Bag(string uniqueID, CreationDetails creationDetails) : base(uniqueID, creationDetails)
		{
			TileIndexes = new(44, 4) { C = Color.Brown + 30 };
			CanCarryOnBack = true;
			SlotsPosition = new Point(0, 12);
		}

		public override void OnItemInfoDisplay()
		{
			NavigationPanel.Tab.Texts["item-info"] = $"\t\t\t  {nameof(Bag)}\n\n\tA small bag, great for\n\t  carrying essentials.\n" +
				$"\tCan fit up to [{Stats["slots"]} {GetStatName("items", Stats["slots"])}].";
		}
	}
}
