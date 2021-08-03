using SMPL;

namespace TestGame
{
	public class Player : Events
	{
		private Component2D Component2D { get; set; }
		private ComponentSprite ComponentSprite { get; set; }
		private ComponentHitbox ComponentHitbox2 { get; set; }

		public Player()
		{
			Subscribe(this, 0);
			Component2D = new()
			{
				Size = new Size(200, 200)
			};
			File.LoadAsset(File.Asset.Texture, "test2.png");
			File.LoadAsset(File.Asset.Texture, "penka.png");
			File.LoadAsset(File.Asset.Font, "Munro.ttf");
		}
      public override void OnAssetsLoadingEnd()
      {
			if (Gate.EnterOnceWhile("penka.png", File.AssetIsLoaded("penka.png")))
			{
				ComponentSprite = new(Component2D, "penka.png");
				Component2D.ComponentHitbox = new();
				ComponentHitbox2 = new();

				Component2D.ComponentHitbox.SetLine("top", new Line(new Point(0, 0), new Point(200, 0)));
				Component2D.ComponentHitbox.SetLine("right", new Line(new Point(200, 0), new Point(200, 200)));
				Component2D.ComponentHitbox.SetLine("bottom", new Line(new Point(200, 200), new Point(0, 200)));
				Component2D.ComponentHitbox.SetLine("left", new Line(new Point(0, 200), new Point(0, 0)));

				ComponentHitbox2.SetLine("top", new Line(new Point(-300, -300), new Point(300, -300)));
				ComponentHitbox2.SetLine("right", new Line(new Point(300, -300), new Point(300, 300)));
				ComponentHitbox2.SetLine("bottom", new Line(new Point(300, 300), new Point(-300, 300)));
				ComponentHitbox2.SetLine("left", new Line(new Point(-300, 300), new Point(-300, -300)));
				//ComponentHitbox2.SetLine("t", new Line(new Point(-300, -300), new Point(200, -50)));
				//ComponentHitbox2.SetLine("f", new Line(new Point(200, 50), new Point(-300, 300)));
			}
      }
		public override void OnDraw(Camera camera)
      {
			Component2D.Angle++;
			if (ComponentSprite != null)
         {
				ComponentSprite.Draw(camera);
				Component2D.ComponentHitbox.Draw(camera);
				ComponentHitbox2.Draw(camera);
			}
			Component2D.Position = new Point(-200, 0);
			if (ComponentHitbox2 != null)
			{
				var p2 = ComponentHitbox2.MiddlePoint;
				p2.Color = Color.Blue;
				p2.Draw(camera);
				Console.Log(ComponentHitbox2.Contains(Component2D.ComponentHitbox));
				var p = Component2D.ComponentHitbox.MiddlePoint;
				p.Color = Color.Black;
				p.Draw(camera);
			}
		}
	}
}
