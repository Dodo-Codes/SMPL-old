using SMPL.Data;
using System;

namespace RPG1bit
{
	public class StartSingle : Object
	{
		public StartSingle(CreationDetails creationDetails) : base(creationDetails) { }
		protected override void OnHovered() => NavigationPanel.Info.Textbox.Text = "Start a new singleplayer game session.  ";
		protected override void OnLeftClicked()
		{
			Map.DestroyAllSessionObjects();
			Map.DisplayNavigationPanel();
			Map.CurrentSession = Map.Session.Single;

			new AdjustMusic(new CreationDetails()
			{
				Name = "test",
				Position = new Point(0, 0) { Color = Color.Red },
				TileIndexes = new Point[] { new(08, 22) },
				Height = 1,
				IsClickable = true,
			});
			Map.CreateUIButtons();
			Map.Display(); // for the map itself
			DisplayAllObjects(); // for the map ui
		}
	}
}
