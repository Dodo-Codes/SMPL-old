using SMPL.Gear;

namespace RPG1bit
{
	public class MoveCamera : Object
	{
		public MoveCamera(CreationDetails creationDetails) : base(creationDetails) { }

		protected override void OnDroppedUpon()
		{
			var asd = HoldingObject;
		}
	}
}
