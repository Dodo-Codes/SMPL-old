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
			File.LoadAsset(File.Asset.Texture, "penka.png");
			Camera.CallWhen.Display(OnDraw);
			File.CallWhen.AssetLoadEnd(OnAssetLoadEnd);
			Keyboard.CallWhen.KeyPress(OnKeyPress);
			Keyboard.CallWhen.KeyRelease(OnKeyRelease);
		}
		void OnKeyPress(Keyboard.Key key)
		{
			var mouse = Identity<TextBox>.PickByUniqueID("mouse");
			var penka = Identity<Sprite>.PickByUniqueID("penka");
			mouse.Family.Parent = penka;
		}
		void OnKeyRelease(Keyboard.Key key)
		{
			var mouse = Identity<TextBox>.PickByUniqueID("mouse");
			var penka = Identity<Sprite>.PickByUniqueID("penka");
			mouse.Family.Parent = null;
		}
		void OnAssetLoadEnd()
		{
			Area.Create("mouse-tr", "penka-tr");
			TextBox.Create("mouse");
			Sprite.Create("penka");
			Effects.Create("mouse-eff", "penka-eff");
			Family.Create("mouse-fam", "penka-fam");
			var mouse = Identity<TextBox>.PickByUniqueID("mouse");
			var penka = Identity<Sprite>.PickByUniqueID("penka");
			mouse.Area = Identity<Area>.PickByUniqueID("mouse-tr");
			mouse.FontPath = "Munro.ttf";
			penka.Area = Identity<Area>.PickByUniqueID("penka-tr");
			penka.TexturePath = "penka.png";
			mouse.Effects = Identity<Effects>.PickByUniqueID("mouse-eff");
			penka.Effects = Identity<Effects>.PickByUniqueID("penka-eff");

			penka.Area.Size = new Size(500, 500);
			mouse.Area.Size = new Size(300, 300);
			mouse.Scale = new Size(1, 1);

			penka.Family = Identity<Family>.PickByUniqueID("penka-fam");
			mouse.Family = Identity<Family>.PickByUniqueID("mouse-fam");

			//penka.Effects.MaskColor = Color.Red;
			//penka.Effects.MaskType = Effects.Mask.None;
			//penka.Effects.MaskColorBounds = 50;
		}
		void OnDraw(Camera camera)
		{
			if (Performance.FrameCount % 20 == 0) Window.Title = $"Test Game ({Performance.FPS:F2} FPS)";

			var mouse = Identity<TextBox>.PickByUniqueID("mouse");
			var penka = Identity<Sprite>.PickByUniqueID("penka");
			if (mouse == null) return;
			mouse.Area.Angle++;
			penka.Area.Size += new Size(1, 1);
			penka.Area.Position = Mouse.CursorPositionWindow;
			penka.Display(camera);
			mouse.Display(camera);
			Console.Log(mouse.Area.Size);
		}
	}
}