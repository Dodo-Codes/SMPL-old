using SMPL.Gear;

namespace RPG1bit
{
	public class ExitGame : Object
	{
		public ExitGame(string uniqueID, CreationDetails creationDetails) : base(uniqueID, creationDetails) { }
		public override void OnLeftClicked()
		{
			FileSystem.DeleteAllFiles("Chunks");
			Window.Close();
		}
		public override void OnHovered() => NavigationPanel.Info.Textbox.Text = "[LEFT CLICK] Exit the game.";
	}
}
