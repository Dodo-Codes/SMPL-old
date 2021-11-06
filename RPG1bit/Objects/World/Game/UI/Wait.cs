using SMPL.Data;
using SMPL.Gear;

namespace RPG1bit
{
	public class Wait : Object
	{
		public Wait(string uniqueID, CreationDetails creationDetails) : base(uniqueID, creationDetails) { }
		public override void OnLeftClicked()
		{
			AdvanceTime();

			NavigationPanel.Tab.Textbox.Text = "Some time passes...";
		}
	}
}
