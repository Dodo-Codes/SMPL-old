using SMPL.Gear;

namespace RPG1bit
{
	public class SlotFeet : Object
	{
		public SlotFeet(CreationDetails creationDetails) : base(creationDetails) { }

		public override void OnDroppedUpon()
		{
			var asd = HoldingObject;
		}
	}
}
