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
			Camera.WorldCamera.TransformComponent.Size = size;
			Window.window.SetView(Camera.WorldCamera.ViewComponent.view);
		}

		public void DrawLines(params Line[] lines) => Camera.WorldCamera.DrawLines(lines);
		public virtual void OnDraw() { }
	}
}
