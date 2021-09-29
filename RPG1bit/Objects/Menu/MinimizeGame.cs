using SMPL.Gear;

namespace RPG1bit
{
	public class MinimizeGame : Object
	{
		public MinimizeGame(CreationDetails creationDetails) : base(creationDetails) { }
		public override void OnLeftClicked() => Window.CurrentState = Window.State.Minimized;
		public override void OnHovered() => NavigationPanel.Info.Textbox.Text = "Minimize the game.";
	}
}
