using SMPL.Gear;

namespace RPG1bit
{
	public class SlotWaist : Object
	{
		public SlotWaist(CreationDetails creationDetails) : base(creationDetails) { }

		protected override void OnDroppedUpon()
		{
			var asd = HoldingObject;
		}
	}
}
