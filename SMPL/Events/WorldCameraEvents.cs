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
		}

		public void DrawLines(params Line[] lines) => Camera.WorldCamera.DrawLines(lines);
		public void DrawShapes(params Shape[] shapes) => Camera.WorldCamera.DrawShapes(shapes);
		public void DrawPoints(params Point[] points) => Camera.WorldCamera.DrawPoints(points);
		public void DrawSprites(params SpriteComponent[] spriteComponents) => Camera.WorldCamera.DrawSprites(spriteComponents);
		public virtual void OnDraw() { }
	}
}
