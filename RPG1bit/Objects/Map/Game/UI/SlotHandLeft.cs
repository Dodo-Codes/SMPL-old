using SMPL.Gear;

namespace RPG1bit
{
	public class SlotHandLeft : Object
	{
		public SlotHandLeft(CreationDetails creationDetails) : base(creationDetails) { }

		public override void OnDroppedUpon()
		{
			var asd = HoldingObject;
		}
	}
}
