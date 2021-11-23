using SMPL.Gear;

namespace RPG1bit
{
	public class MinimizeGame : GameObject
	{
		public MinimizeGame(string uniqueID, CreationDetails creationDetails) : base(uniqueID, creationDetails) { }
		public override void OnLeftClicked() => Window.CurrentState = Window.State.Minimized;
		public override void OnHovered() => NavigationPanel.Info.Textbox.Text = "[LEFT CLICK] to minimize the game";
	}
}
