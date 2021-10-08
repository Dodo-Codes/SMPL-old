using SMPL.Gear;

namespace RPG1bit
{
	public class SlotBody : Object
	{
		public SlotBody(string uniqueID, CreationDetails creationDetails) : base(uniqueID, creationDetails) { }

		public override void OnDroppedUpon()
		{
			var asd = HoldingObject;
		}
	}
}
