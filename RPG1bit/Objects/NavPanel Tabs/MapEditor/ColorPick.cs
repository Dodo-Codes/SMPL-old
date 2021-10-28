using SMPL.Data;
using SMPL.Gear;
using System.Linq;

namespace RPG1bit
{
	public class ColorPick : Slider
	{
		public ColorPick(string uniqueID, CreationDetails creationDetails, int size, bool isVertical)
			: base(uniqueID, creationDetails, size, isVertical)
		{
			IndexValue = size - 1;
			AddTags("brush-color-pickers");
		}
		public override void OnHovered()
		{
			var str = "";
			switch (Name)
			{
				case "r": str = "red"; break;
				case "g": str = "green"; break;
				case "b": str = "blue"; break;
			}
			NavigationPanel.Info.Textbox.Text = $"[LEFT HOLD] Adjust the amount of {str}.";
		}

		protected override void OnIndexValueChanged()
		{
			var indicatorTile = Screen.GetCellIndexesAtPosition(new(0, 4), 1);
			var value = GetColorValue(IndexValue);
			var color = MapEditor.Brush.C;
			switch (Name)
			{
				case "r": color.R = value; break;
				case "g": color.G = value; break;
				case "b": color.B = value; break;
			}
			MapEditor.Brush = new Point(MapEditor.Brush.X, MapEditor.Brush.Y) { C = color };
			Screen.EditCell(new(0, 4), indicatorTile, 1, color);
		}
		protected override void OnDisplayStep(Point screenPos, int step)
		{
			var tile = Screen.GetCellIndexesAtPosition(screenPos, 1);
			var value = GetColorValue(step);
			var color = new Color(0, 0, 0);
			switch (Name)
			{
				case "r": color.R = value; break;
				case "g": color.G = value; break;
				case "b": color.B = value; break;
			}
			Screen.EditCell(screenPos, tile, 1, color);
		}
		private double GetColorValue(int index) => Number.Round(Number.Map(index, new(0, Size - 1), new(0, 255)));

		public static void UpdateBrushColorPickers()
		{
			var pickerR = (ColorPick)PickByUniqueID("brush-r");
			var pickerG = (ColorPick)PickByUniqueID("brush-g");
			var pickerB = (ColorPick)PickByUniqueID("brush-b");

			var color = MapEditor.Brush.C;
			pickerR.IndexValue = (int)Number.Map(color.R, new(0, 255), new(0, pickerR.Size - 1));
			pickerG.IndexValue = (int)Number.Map(color.G, new(0, 255), new(0, pickerG.Size - 1));
			pickerB.IndexValue = (int)Number.Map(color.B, new(0, 255), new(0, pickerB.Size - 1));
		}
	}
}
