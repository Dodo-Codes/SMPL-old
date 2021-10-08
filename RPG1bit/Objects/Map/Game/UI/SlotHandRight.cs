using SMPL.Gear;

namespace RPG1bit
{
	public class SlotHandRight : Object
	{
		public SlotHandRight(string uniqueID, CreationDetails creationDetails) : base(uniqueID, creationDetails) { }

		public override void OnDroppedUpon()
		{
			var asd = HoldingObject;
		}
	}
}
