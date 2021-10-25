using SMPL.Data;

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

		public override void OnItemInfoDisplay()
		{
			NavigationPanel.Tab.Texts["item-info"] = $"\t\t\t\t Key\n\nGreat for unlocking locked things.";
		}
	}
}
