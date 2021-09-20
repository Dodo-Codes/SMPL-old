using SMPL.Gear;

namespace RPG1bit
{
	public class SlotFeet : Object
	{
		public SlotFeet(CreationDetails creationDetails) : base(creationDetails) { }

		protected override void OnDroppedUpon()
		{
			var asd = HoldingObject;
		}
	}
}
