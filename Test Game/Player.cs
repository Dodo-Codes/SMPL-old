using SMPL.Components;
using SMPL.Data;
using SMPL.Gear;

namespace TestGame
{
	public class Player
	{
		public Player()
		{
			File.LoadAsset(File.Asset.Texture, "penka.png", "explosive.jpg");
			Camera.CallWhen.Display(OnDraw);
			File.CallWhen.AssetLoadEnd(OnAssetLoadEnd);
		}
		void OnAssetLoadEnd()
		{
			Area.Create("test-tr", "mouse-tr");
			Sprite.Create("test");
			TextBox.Create("mouse");
			Effects.Create("test-eff", "mouse-eff");
			var mouse = Identity<Sprite>.PickByUniqueID("mouse");
			var test = Identity<Sprite>.PickByUniqueID("test");
			mouse.Area = Identity<Area>.PickByUniqueID("mouse-tr");
			test.Area = Identity<Area>.PickByUniqueID("test-tr");
			mouse.Effects = Identity<Effects>.PickByUniqueID("mouse-eff");
			test.Effects = Identity<Effects>.PickByUniqueID("test-eff");
			mouse.TexturePath = "penka.png";
			test.TexturePath = "explosive.jpg";

			test.SetQuadGrid("test", 10, 10);

			test.Effects.AddMasks(mouse);
			test.Effects.MaskColor = Color.Red;
			test.Effects.MaskType = Effects.Mask.In;
			test.Effects.MaskColorBounds = 100;
		}
		void OnDraw(Camera camera)
		{
			var spr = Identity<Sprite>.PickByUniqueID("test");
			var mouse = Identity<Sprite>.PickByUniqueID("mouse");
			if (spr == null) return;
			mouse.Area.Position = Mouse.CursorPositionWindow;
			mouse.Area.Size = new Size(200, 200);
			spr.Area.Size = new Size(800, 800);

			var quad = new Quad()
			{
				CornerA = new Corner(new Point(64, 64), 0, 0),
				CornerB = new Corner(new Point(128, 64), 32, 0),
				CornerC = new Corner(new Point(64, 128), 32, 32),
				CornerD = new Corner(new Point(64, 128), 0, 32),
			};
			spr.SetQuad("test 1 1", quad);

			spr.Display(camera);
			mouse.Display(camera);
		}
	}
}