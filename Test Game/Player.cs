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
			penka.Effects.AddMasks(mouse);
		}
		void OnKeyRelease(Keyboard.Key key)
		{
			var mouse = Identity<TextBox>.PickByUniqueID("mouse");
			var penka = Identity<Sprite>.PickByUniqueID("penka");
		}
		void OnAssetLoadEnd()
		{
			Area.Create("mouse-tr", "penka-tr");
			TextBox.Create("mouse");
			Sprite.Create("penka");
			Effects.Create("penka-eff");
			var mouse = Identity<TextBox>.PickByUniqueID("mouse");
			var penka = Identity<Sprite>.PickByUniqueID("penka");
			mouse.Area = Identity<Area>.PickByUniqueID("mouse-tr");
			mouse.FontPath = "Munro.ttf";
			penka.Area = Identity<Area>.PickByUniqueID("penka-tr");
			penka.TexturePath = "penka.png";
			penka.Effects = Identity<Effects>.PickByUniqueID("penka-eff");

			penka.Area.Size = new Size(500, 500);

			penka.Effects.MaskColor = Color.White;
			penka.Effects.MaskType = Effects.Mask.None;
		}
		void OnDraw(Camera camera)
		{
			var mouse = Identity<TextBox>.PickByUniqueID("mouse");
			var penka = Identity<Sprite>.PickByUniqueID("penka");
			if (mouse == null) return;
			mouse.Area.Position = Mouse.CursorPositionWindow;

			penka.Display(camera);
			mouse.Display(camera);
		}
	}
}