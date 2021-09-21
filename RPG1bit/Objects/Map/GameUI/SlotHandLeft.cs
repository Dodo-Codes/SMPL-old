using SMPL.Gear;

namespace RPG1bit
{
	public class SlotHandLeft : Object
	{
		public SlotHandLeft(CreationDetails creationDetails) : base(creationDetails) { }

		protected override void OnDroppedUpon()
		{
			var asd = HoldingObject;
		}
	}
}
