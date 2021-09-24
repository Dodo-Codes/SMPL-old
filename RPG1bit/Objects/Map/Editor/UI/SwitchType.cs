using SMPL.Data;
using SMPL.Gear;
using System.Collections.Generic;

namespace RPG1bit
{
	public class SwitchType : Object
	{
		private static int index;
		private static readonly Point[] graphicIndexes = new Point[] { new(5, 0) };
		public static Point BrushType => graphicIndexes[index];

		public SwitchType(CreationDetails creationDetails) : base(creationDetails) { }
		protected override void OnHovered() => NavigationPanel.Info.Textbox.Text = "Change the brush type.";
		protected override void OnLeftClicked()
		{
			index += Name == "tile-up" ? 1 : -1;
			if (index == graphicIndexes.Length) index = 0;
			else if (index == -1) index = graphicIndexes.Length - 1;

			Screen.EditCell(new(0, 4), graphicIndexes[index], 1, SwitchColor.BrushColor) ;
		}
	}
}
