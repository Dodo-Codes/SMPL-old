using SMPL.Data;
using SMPL.Gear;

namespace RPG1bit
{
	public class SwitchColor : Object
	{
		private static readonly Color[] colors = new Color[]
		{ Color.White, Color.GrayLight, Color.Gray, Color.GrayDark, Color.Black,
			Color.BlueLight, Color.Red, Color.Green, Color.Blue };
		private static int colorIndex;

		public static Color BrushColor => colors[colorIndex];

		public SwitchColor(CreationDetails creationDetails) : base(creationDetails) { }
		protected override void OnHovered() => NavigationPanel.Info.Textbox.Text = "Change the brush color.";
		protected override void OnLeftClicked()
		{
			colorIndex += Name == "color-up" ? 1 : -1;
			if (colorIndex == colors.Length) colorIndex = 0;
			else if (colorIndex == -1) colorIndex = colors.Length - 1;

			var indexes = Screen.GetCellIndexesAtPosition(new(0, 4), 1);
			Screen.EditCell(new(0, 4), indexes, 1, colors[colorIndex]);
		}
	}
}
