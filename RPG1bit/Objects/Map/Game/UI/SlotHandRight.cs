using SMPL.Gear;

namespace RPG1bit
{
	public class SlotHandRight : Object
	{
		public SlotHandRight(CreationDetails creationDetails) : base(creationDetails) { }

		public override void OnDroppedUpon()
		{
			var asd = HoldingObject;
		}
	}
}
