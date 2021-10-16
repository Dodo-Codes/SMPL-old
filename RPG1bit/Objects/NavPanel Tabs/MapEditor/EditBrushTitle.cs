using SMPL.Data;
using SMPL.Gear;

namespace RPG1bit
{
	public class EditBrushTitle : Object
	{
		public EditBrushTitle(string uniqueID, CreationDetails creationDetails) : base(uniqueID, creationDetails) { }

		public override void OnDisplay(Point screenPos)
		{
			Screen.DisplayText(Position, 1, Color.White, "tiles     rgb");
		}
	}
}
