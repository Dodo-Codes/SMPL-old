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
			NavigationPanel.Info.ShowLeftClickableIndicator();
		}
		public override void OnDisplay(Point screenPos)
		{
			Screen.EditCell(screenPos, TileIndexes, 1, Color.White);
			Screen.DisplayText(screenPos + new Point(1, 0), 1, Color.White, Name);
		}
	}
}
