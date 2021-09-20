using SMPL.Gear;

namespace RPG1bit
{
	public class MapEdit : Object
	{
		public MapEdit(CreationDetails creationDetails) : base(creationDetails) { }
		protected override void OnHovered() => NavigationPanel.Info.Textbox.Text = "Start a new map edit session.";
	}
}
