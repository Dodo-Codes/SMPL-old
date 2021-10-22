using SMPL.Data;

namespace RPG1bit
{
	public class Key : Item
	{
		public Key(string uniqueID, CreationDetails creationDetails) : base(uniqueID, creationDetails)
		{
			CanHold = true;
			CanCarryOnWaist = true;
			CanCarryInBag = true;
			CanCarryInQuiver = true;
		}

		public override void OnItemInfoDisplay()
		{
			NavigationPanel.Tab.Texts["item-info"] = "A strange key.";
		}
	}
}
