using SMPL.Components;
using SMPL.Data;
using SMPL.Gear;
using SMPL.Prefabs;
using System;

namespace TestGame
{
	public class Player
	{
		public Player()
		{
			Camera.CallWhen.Display(OnDisplay);
			Keyboard.CallWhen.TextInput(OnTextInput);
			Assets.Load(Assets.Type.Texture, "cape.jpg");
			new Cloth("test", new Point(-100, -100), new Size(200, 200)) { TexturePath = "cape.jpg" };
		}

		int j = 0;
		private void OnTextInput(string text, bool arg2, bool arg3, bool arg4)
		{
			switch (text)
			{
				case "q": j = 0; return;
				case "w": j = 1; return;
				case "e": j = 2; return;
				case "r": j = 3; return;
				case "t": j = 4; return;
				case "y": j = 5; return;
				case "u": j = 6; return;
				case "i": j = 7; return;
				case "o": j = 8; return;
				case "p": j = 9; return;
			}

			var cloth = (Cloth)Component.PickByUniqueID("test");
			cloth.Cut(int.Parse(text), j);
		}

		private void OnDisplay(Camera camera)
		{
			var cloth = (Cloth)Component.PickByUniqueID("test");
			var rope = (Ropes)Component.PickByUniqueID(cloth.RopesUniqueID);
			if (cloth == null || Assets.AreLoaded("cape.jpg") == false) return;
			rope.Force = Mouse.ButtonIsPressed(Mouse.Button.Middle)
					 ? new Size(-Mouse.CursorPositionWindow.X, -Mouse.CursorPositionWindow.Y) / 500
					 : new Size(0, 1);
			if (Mouse.ButtonIsPressed(Mouse.Button.Left)) rope.GetPoint("0 0").Position = Mouse.CursorPositionWindow;
			if (Mouse.ButtonIsPressed(Mouse.Button.Right)) rope.GetPoint("9 0").Position = Mouse.CursorPositionWindow;
			cloth.Display(camera);
		}
	}
}