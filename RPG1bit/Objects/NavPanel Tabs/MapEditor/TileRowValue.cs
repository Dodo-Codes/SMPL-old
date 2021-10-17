using SMPL.Data;
using SMPL.Gear;
using System.Linq;

namespace RPG1bit
{
	public class TileRowValue : Object
	{
		public TileRowValue(string uniqueID, CreationDetails creationDetails) : base(uniqueID, creationDetails) { }

		public override void OnDisplay(Point screenPos)
		{
			var mapEditorTiles = MapEditor.tiles;
			var scrollIndex = ObjectList.Lists["brush-tiles"].scrollIndex;
			var y = (int)((screenPos.Y - 4 + scrollIndex) * 9);
			for (int i = 0; i < 9; i++)
			{
				if (y + i >= mapEditorTiles.Length) return;
				Screen.EditCell(screenPos + new Point(i, 0), mapEditorTiles[y + i], 1, mapEditorTiles[y + i].C);
			}
		}
	}
}
