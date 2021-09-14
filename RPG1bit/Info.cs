using SMPL.Components;
using SMPL.Data;
using SMPL.Gear;
using System;

namespace RPG1bit
{
	public static class Info
	{
		public static Textbox Textbox { get; set; }
		public static Area Area { get; set; }

		public static void Create()
		{
			Camera.CallWhen.Display(OnDisplay);

			for (int y = 14; y < 17; y++)
				for (int x = 19; x < 31; x++)
					Screen.EditCell(new Point(x, y), new Point(0, 23), 0, Color.White);

			Area = new("info-area");

			var sz = Camera.WorldCamera.Size / 2;
			Area.Position = new Point(60 * 25, 60 * 15.6) - new Point(sz.W, sz.H);
			Area.Size = new(60 * 12, 60 * 4);

			Textbox = new("info-textbox");
			Textbox.AreaUniqueID = "info-area";
			Textbox.FontPath = "Assets\\font.ttf";
			Textbox.CharacterSize = 128;
		}

		private static void OnDisplay(Camera camera)
		{
			if (Textbox == null) return;

			Textbox.Display(camera);
		}
	}
}
