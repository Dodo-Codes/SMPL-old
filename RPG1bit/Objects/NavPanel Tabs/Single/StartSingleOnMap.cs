using SMPL.Data;
using SMPL.Gear;

namespace RPG1bit
{
	public class StartSingleOnWorld : Object
	{
		public StartSingleOnWorld(string uniqueID, CreationDetails creationDetails) : base(uniqueID, creationDetails) { }

		public override void OnLeftClicked()
		{
			World.LoadWorld(World.Session.Single, Name);
		}
		public override void OnHovered()
		{
			NavigationPanel.Info.Textbox.Text = $"World: '{Name.ToUpper()}'\n\n[LEFT CLICK] Load";
			NavigationPanel.Info.ShowLeftClickableIndicator();
		}
		public override void OnDisplay(Point screenPos)
		{
			Screen.EditCell(screenPos, TileIndexes, 1, Color.White);
			Screen.DisplayText(screenPos + new Point(1, 0), 1, Color.White, Name);
		}
	}
}
