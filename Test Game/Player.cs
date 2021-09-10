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
			Assets.Load(Assets.Type.Texture, "mc.png");

			var area = new Area("area");
			var box = new ShapePseudo3D("box") { AreaUniqueID = "area" };
			Camera.CallWhen.Display(OnDisplay);
		}

		double ang = 0;
		private void OnDisplay(Camera camera)
		{
			if (Performance.FrameCount % 20 == 0) Window.Title = $"SMPL Game (FPS: {Performance.FPS:F2})";

			if (Assets.AreLoaded("mc.png") == false) return;
			Camera.WorldCamera.Position = Mouse.CursorPositionWindow;
			var box = (ShapePseudo3D)Thing.PickByUniqueID("box");
			var area = (Area)Thing.PickByUniqueID("area");
			box.TexturePath = "mc.png";
			box.SetSideTextureCoordinates(ShapePseudo3D.Side.Left, new Point(512, 0), new Point(1024, 512));
			box.SetSideTextureCoordinates(ShapePseudo3D.Side.Right, new Point(512, 0), new Point(1024, 512));
			box.SetSideTextureCoordinates(ShapePseudo3D.Side.Up, new Point(512, 0), new Point(1024, 512));
			box.SetSideTextureCoordinates(ShapePseudo3D.Side.Down, new Point(512, 0), new Point(1024, 512));
			box.SetSideTextureCoordinates(ShapePseudo3D.Side.Near, new Point(0, 0), new Point(512, 512));
			box.IsPyramid = true;
			area.Angle++;
			box.Display(camera);
		}
	}
}