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
			var box = new ShapePseudo3D("box") { AreaUniqueID = "area" };
			Camera.CallWhen.Display(OnDisplay);
		}
		double x = 0;
		private void OnDisplay(Camera camera)
		{
			if (Performance.FrameCount % 20 == 0) Window.Title = $"SMPL Game (FPS: {Performance.FPS:F2})";

			if (Assets.AreLoaded("Tileset.png") == false) return;
			Camera.WorldCamera.Position = Mouse.CursorPositionWindow;
			var box = (ShapePseudo3D)Thing.PickByUniqueID("box");
			var area = (Area)Thing.PickByUniqueID("area");
			box.TexturePath = "Tileset.png";

			x += 1;
			for (int i = 0; i < 6; i++)
			{
				box.SetSideTextureCropTile((ShapePseudo3D.Side)i, new Point(4, 1));
			}
			box.IsRepeated = true;
			box.SetColorTintLight(x, 100);
			box.SetSideVisibility(ShapePseudo3D.Side.Up, false);
			box.SetSideVisibility(ShapePseudo3D.Side.Near, false);
			box.SetSideVisibility(ShapePseudo3D.Side.Left, false);
			box.SetSideVisibility(ShapePseudo3D.Side.Right, false);
			//area.Angle++;
			box.Display(camera);
		}
	}
}