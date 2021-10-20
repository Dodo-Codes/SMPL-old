using SMPL.Data;
using SMPL.Gear;

namespace RPG1bit
{
	public class ShowRoofs : Tick
	{
		public ShowRoofs(string uniqueID, CreationDetails creationDetails) : base(uniqueID, creationDetails) { Value = true; }
		public override void OnHovered() => NavigationPanel.Info.Textbox.Text = "Toggle roofs visibility.";
		public override void OnValueChanged()
		{
			Map.IsShowingRoofs = Value;
			Screen.Display();
		}
	}
}
