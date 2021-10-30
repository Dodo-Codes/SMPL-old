using SMPL.Data;
using SMPL.Gear;
using System.Linq;

namespace RPG1bit
{
	public class TileRowValue : Object
	{
		public TileRowValue(string uniqueID, CreationDetails creationDetails) : base(uniqueID, creationDetails) { }

		public override void OnHovered()
		{
			var descr = NavigationPanel.Info.Textbox.Text;
			if (descr == "Void.")
				return;
			else if (descr == "")
				descr = "Background color.";
			else if (descr == "Game navigation panel.")
				return;
			NavigationPanel.Info.Textbox.Text = $"[MIDDLE CLICK] Pick a brush from tile:\n{descr}";
		}
		public override void OnDisplay(Point screenPos)
		{
			var listSize = ObjectList.Lists["brush-tiles"].Size;
			var mapEditorTiles = MapEditor.TileList;
			var scrollIndex = ObjectList.Lists["brush-tiles"].scrollIndex;
			var y = (int)((screenPos.Y - 3 + scrollIndex) * listSize.W);
			for (int i = 0; i < listSize.W; i++)
			{
				if (y + i >= mapEditorTiles.Length) return;
				Screen.EditCell(screenPos + new Point(i, 0), mapEditorTiles[y + i], 1, mapEditorTiles[y + i].C);
			}
		}
	}
}
