using SMPL.Gear;

namespace RPG1bit
{
	public class EquipSlot : Object
	{
		public EquipSlot(string uniqueID, CreationDetails creationDetails) : base(uniqueID, creationDetails) { }

		public override void OnDroppedUpon()
		{
			var asd = HoldingObject;
		}
	}
}
