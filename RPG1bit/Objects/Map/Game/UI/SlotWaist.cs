using SMPL.Gear;

namespace RPG1bit
{
	public class SlotWaist : Object
	{
		public SlotWaist(string uniqueID, CreationDetails creationDetails) : base(uniqueID, creationDetails) { }

		public override void OnDroppedUpon()
		{
			var asd = HoldingObject;
		}
	}
}
