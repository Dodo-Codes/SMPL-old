using SFML.Graphics;
using SFML.System;
using System.Windows.Forms;

namespace SMPL
{
	public abstract class WorldCameraEvents
	{
		internal static WorldCameraEvents instance;

		internal void Subscribe()
		{
			instance = this;

			var scrSize = Screen.PrimaryScreen.Bounds;
			var size = new Size(scrSize.Width, scrSize.Height);
			var pixelSize = size / 2;
			Camera.WorldCamera = new(new Point(0, 0), pixelSize);
			Window.window.SetView(Camera.WorldCamera.DrawComponent.view);
		}
		public void DrawLines(params Line[] lines) => Camera.WorldCamera.DrawLines(lines);
		public virtual void OnDraw() { }
		public static void Display()
		{
			Camera.WorldCamera.DrawComponent.RendTexture.Display();
			var viewSz = Camera.WorldCamera.DrawComponent.view.Size;
			var startSz = Camera.WorldCamera.DrawComponent.startSize;

			var Size = viewSz / 2;
			var tpos = new Vector2i((int)Size.X, (int)Size.Y);

			var ms = startSz / 2;
			var sz = new Vector2i((int)ms.Width, (int)ms.Height) * 2;

			var pos = startSz / 2;
			var tsz = (Vector2i)viewSz;
			var sc = startSz / new Size(viewSz.X, viewSz.Y);

			var sprite = new Sprite(Camera.WorldCamera.DrawComponent.RendTexture.Texture)
			{
				Position = new Vector2f((float)-pos.Width, (float)-pos.Height),
				TextureRect = new IntRect(-tpos, tsz),
				Scale = new Vector2f((float)sc.Width, (float)sc.Height)
			};
			Window.window.Draw(sprite);
			sprite.Dispose();
		}
	}
}
