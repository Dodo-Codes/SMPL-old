using SMPL.Components;
using SMPL.Data;
using SMPL.Gear;

namespace TestGame
{
	public class Player
	{
		Point[] points;
		public Player()
		{
			File.LoadAssets(File.Asset.Texture, "penka.png", "explosive.jpg");
			Camera.CallWhen.Display(OnDraw);
			File.CallWhen.AssetLoadEnd(OnAssetLoadEnd);
		}
		void OnAssetLoadEnd()
		{
			var mouse = new Sprite("mouse");
			var penka = new Sprite("penka");
			mouse.Area = new Area("mouse-tr");
			penka.Area = new Area("penka-tr");
			mouse.TexturePath = "explosive.jpg";
			penka.TexturePath = "penka.png";
			penka.Area.Size = new Size(5, 5);
			mouse.Area.Size = new Size(5, 5);

			double Random() => Number.Random(new Bounds(-50, 50), 3);
			Point R() => new(Random(), Random());
			points = new Point[] { new Point(0, 0), new Point(50, 0), new Point(100, 0) };
		}
		void OnDraw(Camera camera)
		{
			if (Performance.FrameCount % 20 == 0) Window.Title = $"Test Game ({Performance.FPS:F2} FPS)";

			var mouse = Identity<Sprite>.PickByUniqueID("mouse");
			//var penka = Identity<Sprite>.PickByUniqueID("penka");
			if (mouse == null) return;

			Point.Constrain(points, Mouse.CursorPositionWindow);
			//var points = Point.Constrain(new Point(0, 0), Mouse.CursorPositionWindow, 50, 50);
			for (int i = 0; i < points.Length - 1; i++)
			{
				new Line(points[i], points[i + 1]).Display(camera);
			}

			//mouse.Display(camera);
			//penka.Display(camera);
		}
	}
}