using SMPL.Components;
using SMPL.Data;
using SMPL.Gear;

namespace TestGame
{
	public class Player
	{
		public Player()
		{
			File.LoadAsset(File.Asset.Texture, "penka.png", "big.jpg");
			Camera.CallWhen.Display(OnDraw);
			File.CallWhen.AssetLoadEnd(OnAssetLoadEnd);
		}
		void OnAssetLoadEnd()
		{
			Area.Create("test-tr", "mouse-tr");
			Sprite.Create("test", "mouse");
			Effects.Create("test-eff", "mouse-eff");
			var mouse = Identity<Sprite>.PickByUniqueID("mouse");
			var test = Identity<Sprite>.PickByUniqueID("test");
			mouse.Area = Identity<Area>.PickByUniqueID("mouse-tr");
			test.Area = Identity<Area>.PickByUniqueID("test-tr");
			mouse.Effects = Identity<Effects>.PickByUniqueID("mouse-eff");
			test.Effects = Identity<Effects>.PickByUniqueID("test-eff");
			mouse.TexturePath = "penka.png";
			test.TexturePath = "big.jpg";

			test.Effects.AddMasks(mouse);
			test.Effects.MaskColor = Color.Red;
			test.Effects.MaskType = Effects.Mask.In;
		}
		void OnDraw(Camera camera)
		{
			var spr = Identity<Sprite>.PickByUniqueID("test");
			var mouse = Identity<Sprite>.PickByUniqueID("mouse");
			if (spr == null) return;
			spr.Area.Position = Mouse.CursorPositionWindow;
			mouse.Area.Position += new Point(0.1, 0);
			//spr.Area.Angle++;
			//mouse.Area.Angle--;

			spr.Display(camera);
			mouse.Display(camera);
		}
	}
}