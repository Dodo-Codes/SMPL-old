using SMPL.Gear;

namespace RPG1bit.Objects
{
	public class ExitGame : Object
	{
		public ExitGame(CreationDetails creationDetails) : base(creationDetails) { }
		protected override void OnLeftClicked() => Window.Close();
		protected override void OnHovered() => NavigationPanel.Info.ShowClickableIndicator();
	}
}
