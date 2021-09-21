using SMPL.Gear;

namespace RPG1bit
{
	public class MapEditor : Object
	{
		public MapEditor(CreationDetails creationDetails) : base(creationDetails) { }
		protected override void OnHovered() => NavigationPanel.Info.Textbox.Text = "Start a new map edit session.";
		protected override void OnLeftClicked()
		{
			if (Map.CurrentSession != Map.Session.MapEdit) Map.DestroyAllSessionObjects();
			Map.DisplayNavigationPanel();
			Map.CurrentSession = Map.Session.MapEdit;

			Map.CameraPosition = new(5, 0);
			Map.Data[0, 0] = new(5, 0);

			Map.CreateUIButtons();
			Map.Display(); // for the map iteself
			DisplayAllObjects(); // for the ui
		}
	}
}
