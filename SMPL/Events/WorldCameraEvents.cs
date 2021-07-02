using SFML.Graphics;
using SFML.System;

namespace SMPL
{
	public abstract class WorldCameraEvents
	{
		internal static WorldCameraEvents instance;

		internal void Subscribe()
		{
			instance = this;

			Camera.WorldCamera = new(new Point(0, 0), new Size(Window.window.Size.X, Window.window.Size.Y));
			Window.window.SetView(Camera.WorldCamera.view);
		}

		public void DrawLines(params Line[] lines) => Camera.WorldCamera.DrawLines(lines);
		public virtual void OnDraw() { }
	}
}
