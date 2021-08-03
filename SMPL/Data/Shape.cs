using SFML.Graphics;
using System.Collections.Generic;

namespace SMPL
{
	public struct Shape
	{
		public Point PointA { get; set; }
		public Point PointB { get; set; }
		public Point PointC { get; set; }
		public Point PointD { get; set; }

		public Shape(Point pointA, Point pointB, Point pointC, Point pointD)
		{
			PointA = pointA;
			PointB = pointB;
			PointC = pointC;
			PointD = pointD;
		}
		public void Draw(Camera camera)
		{
			if (Window.DrawNotAllowed()) return;

			var points = new List<Point>() { PointA, PointB, PointC, PointD };
			var vert = new Vertex[points.Count];
			for (int i = 0; i < points.Count; i++)
			{
				var p = points[i];
				vert[i] = new Vertex(Point.From(p), Color.From(p.Color));
			}

			camera.rendTexture.Draw(vert, PrimitiveType.Quads);
		}
	}
}
