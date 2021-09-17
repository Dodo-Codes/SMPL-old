using SMPL.Components;
using SMPL.Data;
using SMPL.Gear;
using SMPL.Prefabs;

namespace TestGame
{
	public class Player
	{
		public Player()
		{
			Assets.Load(Assets.Type.Font, "Munro.ttf");
			Camera.CallWhen.Display(OnDisplay);
		}
		private void OnDisplay(Camera camera)
		{
			if (Performance.FrameCount % 10 == 0) Window.Title = $"SMPL Game (FPS: {Performance.FPS:F2})";
			if (Assets.AreLoaded("Munro.ttf") == false) return;

			Camera.WorldCamera.Position = Mouse.Cursor.PositionWindow;
			Text.Display(camera, "Hello World!", "Munro.ttf");
		}
	}
}