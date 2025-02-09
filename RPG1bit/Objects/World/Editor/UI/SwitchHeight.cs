﻿using SMPL.Data;
using SMPL.Gear;

namespace RPG1bit
{
	public class SwitchHeight : GameObject
	{
		public static int BrushHeight { get; private set; }

		public SwitchHeight(string uniqueID, CreationDetails creationDetails) : base(uniqueID, creationDetails) { }
		public override void OnHovered() => NavigationPanel.Info.Textbox.Text = "[LEFT CLICK] to change the brush height";
		public override void OnLeftClicked()
		{
			BrushHeight++;
			if (BrushHeight == 3) BrushHeight = 0;
			OnDisplay(Position);
		}
		public override void OnDisplay(Point screenPos)
		{
			Screen.DisplayText(Position, 1, Color.Gray, $"{BrushHeight + 1}");
		}
	}
}
