using SMPL.Gear;

namespace RPG1bit
{
	public class SlotBody : Object
	{
		public SlotBody(CreationDetails creationDetails) : base(creationDetails) { }

		protected override void OnDroppedUpon()
		{
			var asd = HoldingObject;
		}
	}
}
