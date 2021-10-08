using SMPL.Gear;

namespace RPG1bit
{
	public class SlotHandLeft : Object
	{
		public SlotHandLeft(string uniqueID, CreationDetails creationDetails) : base(uniqueID, creationDetails) { }

		public override void OnDroppedUpon()
		{
			var asd = HoldingObject;
		}
	}
}
