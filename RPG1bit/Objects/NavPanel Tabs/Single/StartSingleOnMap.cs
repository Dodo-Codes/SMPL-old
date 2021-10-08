using SMPL.Data;
using SMPL.Gear;

namespace RPG1bit
{
	public class StartSingleOnMap : Object
	{
		public StartSingleOnMap(string uniqueID, CreationDetails creationDetails) : base(uniqueID, creationDetails) { }

		public override void OnLeftClicked()
		{
			Map.LoadMap(Map.Session.Single, Name);
		}
		public override void OnHovered()
		{
			NavigationPanel.Info.Textbox.Text = $"[LMB] Load\nMap: '{Name.ToUpper()}'";
			NavigationPanel.Info.ShowClickableIndicator();
			NavigationPanel.Info.ShowLeftClickableIndicator();
		}
	}
}
