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
			var cloth = new Cloth("test", new Point(-50, -50), new Size(100, 100));
			Camera.CallWhen.Display(OnDisplay);
		}
		private void OnDisplay(Camera camera)
		{
			if (Performance.FrameCount % 10 == 0) Window.Title = $"SMPL Game (FPS: {Performance.FPS:F2})";

			if (Assets.AreLoaded("Tileset.png") == false) return;

			var cloth = (Cloth)Thing.PickByUniqueID("test");
			cloth.TexturePath = "Tileset.png";
			//cloth.SetTextureCropDefault();
			var rope = (Ropes)Thing.PickByUniqueID(cloth.RopesUniqueID);
			var p = Mouse.CursorPositionWindow;
			rope.Force = Mouse.ButtonIsPressed(Mouse.Button.Left) ? new Size(p.X, p.Y) / 500 : new Size(0, 1);
			cloth.SetTextureCropCoordinates(new Point(320 - 32, 64), new Size(32, 32));
			cloth.Display(camera);
			Console.Log(camera.Captures(rope));
			Camera.WorldCamera.Position = p;
		}
	}
}