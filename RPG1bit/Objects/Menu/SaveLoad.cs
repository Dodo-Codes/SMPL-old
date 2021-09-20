using SMPL.Gear;

namespace RPG1bit
{
	public class SaveLoad : Object
	{
		public SaveLoad(CreationDetails creationDetails) : base(creationDetails) { }
		protected override void OnHovered() => NavigationPanel.Info.Textbox.Text = "Save/Load a session.";
	}
}
