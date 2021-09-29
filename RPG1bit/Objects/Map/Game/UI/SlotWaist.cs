using SMPL.Gear;

namespace RPG1bit
{
	public class SlotWaist : Object
	{
		public SlotWaist(CreationDetails creationDetails) : base(creationDetails) { }

		public override void OnDroppedUpon()
		{
			var asd = HoldingObject;
		}
	}
}
