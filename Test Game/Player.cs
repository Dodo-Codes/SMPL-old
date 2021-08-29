using SMPL.Components;
using SMPL.Data;
using SMPL.Gear;

namespace TestGame
{
	public class Player
	{
		public Player()
		{
			File.LoadAsset(File.Asset.Font, "Munro.ttf");
			File.LoadAsset(File.Asset.Texture, "penka.png", "explosive.jpg");
			Camera.CallWhen.Display(OnDraw);
			File.CallWhen.AssetLoadEnd(OnAssetLoadEnd);
			Keyboard.CallWhen.KeyPress(OnKeyPress);
			Keyboard.CallWhen.KeyRelease(OnKeyRelease);
		}
		void OnKeyPress(Keyboard.Key key)
		{
			var mouse = Identity<Sprite>.PickByUniqueID("mouse");
			var penka = Identity<Sprite>.PickByUniqueID("penka");
			mouse.Family.Parent = penka;
		}
		void OnKeyRelease(Keyboard.Key key)
		{
			var mouse = Identity<Sprite>.PickByUniqueID("mouse");
			var penka = Identity<Sprite>.PickByUniqueID("penka");
			mouse.Family.Parent = null;
		}
		void OnAssetLoadEnd()
		{
			var mouse = new Sprite("mouse");
			var penka = new Sprite("penka");
			mouse.Area = new Area("mouse-tr");
			mouse.TexturePath = "explosive.jpg";
			penka.Area = new Area("penka-tr");
			penka.TexturePath = "penka.png";
			mouse.Effects = new Effects("mouse-eff");
			penka.Effects = new Effects("penka-eff");

			penka.Area.Size = new Size(500, 500);
			mouse.Area.Size = new Size(300, 300);

			penka.Family = new Family("penka-fam");
			mouse.Family = new Family("mouse-fam");

		}
		void OnDraw(Camera camera)
		{
			if (Performance.FrameCount % 20 == 0) Window.Title = $"Test Game ({Performance.FPS:F2} FPS)";

			var mouse = Identity<Sprite>.PickByUniqueID("mouse");
			var penka = Identity<Sprite>.PickByUniqueID("penka");
			if (mouse == null) return;
			penka.Area.Angle++;
			penka.Area.Size += new Size(1, 1);
			mouse.Effects.BackgroundColor = Color.Red;
			penka.Area.Position = Mouse.CursorPositionWindow;
			penka.Display(camera);
			mouse.Display(camera);
		}
	}
}