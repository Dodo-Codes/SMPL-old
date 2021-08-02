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
				Size = new Size(-200, 200)
			};
			Mask2D = new()
			{
				Size = new Size(200, 200)
			};
			File.LoadAsset(File.Asset.Texture, "test2.png");
			File.LoadAsset(File.Asset.Texture, "penka.png");
			File.LoadAsset(File.Asset.Font, "Munro.ttf");
			Camera.WorldCamera.Position = new Point(500, 0);
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
			var par = Component2D.PositionToParallax(new Point(0, 0), new Size(50, 50), Camera.WorldCamera);
			var ang = Component2D.AngleToParallax(0, 100, Camera.WorldCamera);
			var sz = Component2D.SizeToParallax(new Size(100, 100), new Size(50, 50), Camera.WorldCamera);
			Console.Log(ang);
			Mask2D.Position = par;
			Mask2D.Angle = ang;
			Mask2D.Size = sz;
			Camera.WorldCamera.Angle++;
			Camera.WorldCamera.Size += new Size(0.16, 0.09) * 50;
			Camera.WorldCamera.Position = new Point(Mouse.CursorPositionWindow.X, Camera.WorldCamera.Position.Y);
			//Component2D.Position += new Point(0, 1);
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
