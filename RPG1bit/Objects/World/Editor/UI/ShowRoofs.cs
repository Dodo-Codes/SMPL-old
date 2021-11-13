using SMPL.Data;
using SMPL.Gear;

namespace RPG1bit
{
	public class ShowRoofs : Tick
	{
		public ShowRoofs(string uniqueID, CreationDetails creationDetails) : base(uniqueID, creationDetails)
		{
			Value = true;
			OnValueChanged();
		}
		public override void OnHovered() => NavigationPanel.Info.Textbox.Text = "[LEFT CLICK] to toggle roofs visibility";
		public override void OnValueChanged()
		{
			World.IsShowingRoofs = Value;
			Screen.ScheduleDisplay();
		}
	}
}
