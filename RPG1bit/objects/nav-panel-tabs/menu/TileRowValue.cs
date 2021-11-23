using SMPL.Data;
using SMPL.Gear;
using System.Linq;

namespace RPG1bit
{
	public class TileRowValue : GameObject
	{
		public TileRowValue(string uniqueID, CreationDetails creationDetails) : base(uniqueID, creationDetails) { }

		public override void OnHovered()
		{
			var descr = NavigationPanel.Info.Textbox.Text;
			if (Screen.GetCellIndexesAtPosition(Screen.GetCellAtCursorPosition(), 1) == new Point(1, 22))
				descr = "Background color";

			NavigationPanel.Info.Textbox.Text = $"[MIDDLE CLICK] Pick a brush from tile\n{descr}";
		}
		public override void OnDisplay(Point screenPos)
		{
			var listSize = GameObjectList.Lists["brush-tiles"].Size;
			var worldEditorTiles = WorldEditor.TileList;
			var scrollIndex = GameObjectList.Lists["brush-tiles"].scrollIndex;
			var y = (int)((screenPos.Y - 3 + scrollIndex) * listSize.W);
			for (int i = 0; i < listSize.W; i++)
			{
				if (y + i >= worldEditorTiles.Length) return;
				Screen.EditCell(screenPos + new Point(i, 0), worldEditorTiles[y + i], 1, worldEditorTiles[y + i].Color);
			}
		}
	}
}
