using SMPL.Data;
using System;

namespace RPG1bit
{
	public class StartSingle : Object
	{
		public StartSingle(CreationDetails creationDetails) : base(creationDetails) { }
		public override void OnHovered() => NavigationPanel.Info.Textbox.Text = "Start a new singleplayer game session.  ";
		public override void OnLeftClicked()
		{
			if (Map.CurrentSession != Map.Session.Single) Map.DestroyAllSessionObjects();
			Map.CurrentSession = Map.Session.Single;
			Map.DisplayNavigationPanel();

			Map.CreateUIButtons();
			Map.Display(); // for the map itself
			DisplayAllObjects(); // for the map ui
		}
	}
}
