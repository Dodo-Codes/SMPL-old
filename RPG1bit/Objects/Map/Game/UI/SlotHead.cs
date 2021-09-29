using SMPL.Gear;

namespace RPG1bit
{
	public class SlotHead : Object
	{
		public SlotHead(CreationDetails creationDetails) : base(creationDetails) { }

		public override void OnDroppedUpon()
		{
			var asd = HoldingObject;
		}
	}
}
