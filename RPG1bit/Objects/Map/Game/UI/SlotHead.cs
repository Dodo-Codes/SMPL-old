using SMPL.Gear;

namespace RPG1bit
{
	public class SlotHead : Object
	{
		public SlotHead(string uniqueID, CreationDetails creationDetails) : base(uniqueID, creationDetails) { }

		public override void OnDroppedUpon()
		{
			var asd = HoldingObject;
		}
	}
}
