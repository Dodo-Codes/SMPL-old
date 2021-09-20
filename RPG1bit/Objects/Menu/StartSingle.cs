using SMPL.Data;
using System;

namespace RPG1bit
{
	public class StartSingle : Object
	{
		public StartSingle(CreationDetails creationDetails) : base(creationDetails) { }
		protected override void OnLeftClicked()
		{
			NavigationPanel.Info.Textbox.Text = descriptions[TileIndexes];
			Map.SessionIsOngoing = true;
			Map.CreateUIButtons();
			Screen.Display();
		}
		protected override void OnHovered() => NavigationPanel.Info.Textbox.Text = "Start a new singleplayer game session.  ";
	}
}
