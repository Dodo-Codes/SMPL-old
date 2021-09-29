using SMPL.Gear;

namespace RPG1bit
{
	public class ExitGame : Object
	{
		public ExitGame(CreationDetails creationDetails) : base(creationDetails) { }
		public override void OnLeftClicked() => Window.Close();
		public override void OnHovered() => NavigationPanel.Info.Textbox.Text = "Exit the game.";
	}
}
