using SMPL.Data;
using SMPL.Gear;
using System.Collections.Generic;
using System.Linq;

namespace RPG1bit
{
	public class SwitchType : Object
	{
		private static int index;
		private static readonly Point[] graphicIndexes = MapEditor.randomTiles.Keys.ToArray();
		public static Point BrushType => graphicIndexes[index];

		public SwitchType(string uniqueID, CreationDetails creationDetails) : base(uniqueID, creationDetails) { }
		public override void OnHovered() => NavigationPanel.Info.Textbox.Text = "Change the brush type.";
		public override void OnLeftClicked()
		{
			index += Name == "tile-up" ? 1 : -1;
			if (index == graphicIndexes.Length) index = 0;
			else if (index == -1) index = graphicIndexes.Length - 1;
			UpdateIndicator();
		}

		public static void UpdateIndicator()
		{
			Screen.EditCell(new(0, 4), graphicIndexes[index], 1, SwitchColor.BrushColor);
		}
		public static void SelectHoveredTileType()
		{
			var indexes = Screen.GetCellIndexesAtPosition(Screen.GetCellAtCursorPosition(), SwitchHeight.BrushHeight);
			for (int i = 0; i < graphicIndexes.Length; i++)
				for (int j = 0; j < MapEditor.randomTiles[graphicIndexes[i]].Length; j++)
					if (MapEditor.randomTiles[graphicIndexes[i]][j] == indexes)
						index = i;
		}
	}
}
