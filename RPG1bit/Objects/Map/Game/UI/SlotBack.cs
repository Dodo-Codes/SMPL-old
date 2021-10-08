using SMPL.Gear;

namespace RPG1bit
{
	public class SlotBack : Object
	{
		public SlotBack(string uniqueID, CreationDetails creationDetails) : base(uniqueID, creationDetails) { }

		public override void OnDroppedUpon()
		{
			var asd = HoldingObject;
		}
	}
}
