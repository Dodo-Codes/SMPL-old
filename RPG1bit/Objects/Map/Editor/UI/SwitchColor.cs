using SMPL.Data;
using SMPL.Gear;

namespace RPG1bit
{
	public class SwitchColor : Object
	{
		private static readonly Color[] colors = new Color[]
		{
			Color.White, Color.GrayLight, Color.Gray, Color.GrayDark, Color.Black,
			Color.RedLight, Color.Red, Color.RedDark,
			Color.GreenLight, Color.Green, Color.GreenDark,
			Color.BlueLight, Color.Blue, Color.BlueDark,
			Color.BrownLight, Color.Brown, Color.BrownDark,
			Color.CyanLight, Color.Cyan, Color.CyanDark,
			Color.MagentaLight, Color.Magenta, Color.MagentaDark,
			Color.OrangeLight, Color.Orange, Color.OrangeDark,
			Color.PinkLight, Color.Pink, Color.PinkDark,
			Color.YellowLight, Color.Yellow, Color.YellowDark,
			Color.WaterLight, Color.Water, Color.WaterDark,
		};
		private static int colorIndex;

		public static Color BrushColor => colors[colorIndex];

		public SwitchColor(string uniqueID, CreationDetails creationDetails) : base(uniqueID, creationDetails) { }
		public override void OnHovered() => NavigationPanel.Info.Textbox.Text = "Change the brush color.";
		public override void OnLeftClicked()
		{
			colorIndex += Name == "color-up" ? 1 : -1;
			if (colorIndex == colors.Length) colorIndex = 0;
			else if (colorIndex == -1) colorIndex = colors.Length - 1;
			UpdateIndicator();
		}
		public static void UpdateIndicator()
		{
			var indexes = Screen.GetCellIndexesAtPosition(new(0, 4), 1);
			var bgIndexes = Screen.GetCellIndexesAtPosition(new(0, 4), 0);
			Screen.EditCell(new(0, 4), indexes, 1, colors[colorIndex]);
			Screen.EditCell(new(0, 4), bgIndexes, 0, colors[colorIndex] == Color.BrownDark ? Color.Brown : Color.BrownDark);
		}
		public static void SelectHoveredTileColor()
		{
			var indexes = Screen.GetCellIndexesAtPosition(Screen.GetCellAtCursorPosition(), SwitchHeight.BrushHeight);
			for (int i = 0; i < colors.Length; i++)
				if (colors[i] == indexes.Color)
					colorIndex = i;
		}
	}
}
