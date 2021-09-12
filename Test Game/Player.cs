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
			Assets.Load(Assets.Type.Texture, "Tileset.png");

			var area = new Area("area");
			Camera.CallWhen.Display(OnDisplay);
		}
		private void OnDisplay(Camera camera)
		{
			if (Performance.FrameCount % 10 == 0) Window.Title = $"SMPL Game (FPS: {Performance.FPS:F2})";

			if (Assets.AreLoaded("Tileset.png") == false) return;

			var test = (Area)Thing.PickByUniqueID("area");
			test.Position = new Point(test.Position.X, test.Position.Y) { Color = Color.Green };
			test.Display(camera, 10, Color.Red);
		}
	}
}