using SMPL.Gear;

namespace RPG1bit
{
	public class SlotBack : Object
	{
		public SlotBack(CreationDetails creationDetails) : base(creationDetails) { }

		public override void OnDroppedUpon()
		{
			var asd = HoldingObject;
		}
	}
}
