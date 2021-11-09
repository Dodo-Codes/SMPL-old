using SMPL.Data;

namespace RPG1bit
{
	public class Quiver : SlotsExtender
	{
		public Quiver(string uniqueID, CreationDetails creationDetails) : base(uniqueID, creationDetails)
		{
			CanCarryOnWaist = true;
			SlotsPosition = new Point(0, 16);
		}

		public override void OnItemInfoDisplay()
		{
			NavigationPanel.Tab.Texts["item-info"] =
				$"\t\t\t\tQuiver\n\n    Made for arrows and bolts\nbut can fit other things just fine.\n" +
				$"\t Can hold up to [{Stats["slots"]} {GetStatName("items", Stats["slots"])}].";
		}
	}
}
