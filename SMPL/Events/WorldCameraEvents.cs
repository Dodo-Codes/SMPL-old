using SFML.Graphics;
using SFML.System;

namespace SMPL
{
	public abstract class WorldCameraEvents
	{
		internal void Subscribe()
		{
			Camera.WorldCamera = new();
			Camera.WorldCamera.Position = new Point(0, 0);
			Camera.WorldCamera.view = new View(new Vector2f(0, 0), new Vector2f(Window.window.Size.X, Window.window.Size.Y));
			Camera.WorldCamera.Zoom = 1;
		}
	}
}
