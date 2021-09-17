using SMPL.Gear;

namespace RPG1bit.Objects
{
	public class MinimizeGame : Object
	{
		public MinimizeGame(CreationDetails creationDetails) : base(creationDetails) { }
		protected override void OnLeftClicked() => Window.CurrentState = Window.State.Minimized;
		protected override void OnHovered()
		{
			NavigationPanel.Info.ShowClickableIndicator();
			NavigationPanel.Info.Textbox.Text = "Minimize the game.";
		}
	}
}
