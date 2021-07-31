using SMPL;

namespace TestGame
{
	public class Player : Events
	{
		private ComponentIdentity<Player> ComponentIdentity { get; set; }
		private Component2D Component2D { get; set; }
		private ComponentSprite ComponentSprite { get; set; }
		private ComponentText ComponentText { get; set; }
		private ComponentSprite Mask { get; set; }
		private Component2D Mask2D { get; set; }

		public Player()
		{
			Subscribe(this, 0);
			ComponentIdentity = new(this, "player");
			Component2D = new()
			{
				Size = new Size(100, 100)
			};
			Mask2D = new()
			{
				Size = new Size(100, 100)
			};
			File.LoadAsset(File.Asset.Texture, "test2.png");
			File.LoadAsset(File.Asset.Texture, "penka.png");
			File.LoadAsset(File.Asset.Font, "Munro.ttf");
		}
      public override void OnAssetsLoadingEnd()
      {
			if (Gate.EnterOnceWhile("test2.png", File.AssetIsLoaded("test2.png")))
			{

			}
			if (Gate.EnterOnceWhile("penka.png", File.AssetIsLoaded("penka.png")))
			{
				ComponentSprite = new(Component2D, "penka.png");
				Mask = new(Mask2D, "penka.png");
				Mask2D.LocalPosition = new Point(200, 0);
				//Mask2D.Position = new Point(100, 0);
			}
			if (Gate.EnterOnceWhile("Munro.ttf", File.AssetIsLoaded("Munro.ttf")))
			{
				//ComponentSprite.Effects.AddMask(Mask);
				//ComponentSprite.Effects.MaskColor = Color.Red;
				//ComponentSprite.Effects.MaskType = Effects.Mask.In;
			}
      }
		public override void OnDraw(Camera camera)
      {
			Component2D.LocalAngle++;
			//Component2D.Position += new Point(0, 1);
			//Mask2D.LocalPosition = new Point(100, 0);
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

		}
		public override void OnKeyPress(Keyboard.Key key)
      {
			Mask.Family.Parent = ComponentSprite;
		}
		public override void OnKeyRelease(Keyboard.Key key)
		{
			Mask.Family.Parent = null;
		}
		public override void OnSpriteResizeEnd(ComponentSprite instance)
		{
			Console.Log("end");
		}
	}
}
