using SMPL.Gear;
using System.IO;

namespace RPG1bit
{
	public class ExitGame : Object
	{
		public ExitGame(string uniqueID, CreationDetails creationDetails) : base(uniqueID, creationDetails) { }
		public override void OnLeftClicked()
		{
			ChunkManager.DestroyAllChunks(false, true);
			Window.Close();
		}
		public override void OnHovered() => NavigationPanel.Info.Textbox.Text = "[LEFT CLICK] Exit the game.";
	}
}
