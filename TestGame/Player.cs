using SMPL;

namespace TestGame
{
	public class Player : Events
	{
		private ComponentIdentity<Player> ComponentIdentity { get; set; }
		private Component2D Component2D { get; set; }
		private ComponentSprite ComponentSprite { get; set; }
		private ComponentText ComponentText { get; set; }
		private ComponentText Mask { get; set; }
		private Component2D Mask2D { get; set; }

		public Player()
		{
			Subscribe(this, 0);
			ComponentIdentity = new(this, "player");
			Component2D = new(new Point(0, 0), 0, new Size(400, 400));
			Mask2D = new(new Point(0, 0), 0, new Size(300, 300));

			File.LoadAsset(File.Asset.Texture, "test2.png");
			File.LoadAsset(File.Asset.Texture, "penka.png");
			File.LoadAsset(File.Asset.Font, "Munro.ttf");
		}
      public override void OnAssetsLoadingEnd()
      {
			if (Gate.EnterOnceWhile("test2.png", File.AssetIsLoaded("test2.png")))
			{
				ComponentSprite = new(Component2D, "test2.png");
			}
			if (Gate.EnterOnceWhile("penka.png", File.AssetIsLoaded("penka.png")))
			{
			}
			if (Gate.EnterOnceWhile("Munro.ttf", File.AssetIsLoaded("Munro.ttf")))
			{
				Mask = new(Mask2D, "Munro.ttf");
				ComponentSprite.Effects.MaskAdd(Mask);
				ComponentSprite.Effects.MaskColor = Color.Red;
				ComponentSprite.Effects.MaskType = Effects.Mask.Out;
				Mask.Effects.WindStrength = new Size(50, 50);

			}
      }
		public override void OnDraw(Camera camera)
      {
			Mask2D.Position = Mouse.CursorPositionWindow;
			if (Mask != null)
			{
				Mask.Effects.Progress = Time.GameClock;
			}
         if (ComponentSprite != null)
         {
				ComponentSprite.Draw(camera);
			}
         if (ComponentText != null)
         {
				ComponentText.Text = $"{Mouse.CursorPositionWindow}";
				ComponentText.Draw(camera);
			}
			if (Mask != null)
			{
				Mask.Draw(camera);
			}
		}
      public override void OnKeyHold(Keyboard.Key key)
      {
			ComponentSprite.SizePercent += new Size(1, 0);
		}
		public override void OnSpriteResizeStart(ComponentSprite instance, Size delta)
		{
			Console.Log("start");
		}
		public override void OnSpriteResize(ComponentSprite instance, Size delta)
		{
			Console.Log(delta);
		}
		public override void OnSpriteResizeEnd(ComponentSprite instance)
		{
			Console.Log("end");
		}
	}
}
