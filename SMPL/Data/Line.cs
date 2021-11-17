using SFML.Graphics;
using SMPL.Components;
using SMPL.Gear;

namespace SMPL.Data
{
	public struct Line
	{
		public static Line Invalid => new(Point.Invalid, Point.Invalid);

		internal Point startPosition;
		public Point StartPosition { get; set; }
		internal Point endPosition;
		public Point EndPosition { get; set; }
		public double Length => Point.Distance(StartPosition, EndPosition);
		public double Angle => Number.AngleBetweenPoints(StartPosition, EndPosition);

		public Line(Point startPosition, Point endPosition)
		{
			this.startPosition = new Point(0, 0);
			this.endPosition = new Point(0, 0);
			StartPosition = startPosition;
			EndPosition = endPosition;
		}
		internal static Point CrossPoint(Point A, Point B, Point C, Point D)
		{
			var a1 = B.Y - A.Y;
			var b1 = A.X - B.X;
			var c1 = a1 * (A.X) + b1 * (A.Y);
			var a2 = D.Y - C.Y;
			var b2 = C.X - D.X;
			var c2 = a2 * (C.X) + b2 * (C.Y);
			var determinant = a1 * b2 - a2 * b1;

			if (determinant == 0)
				return Point.Invalid;
			else
			{
				var x = (b2 * c1 - b1 * c2) / determinant;
				var y = (a1 * c2 - a2 * c1) / determinant;
				return new Point(x, y);
			}
		}

		public Point CrossPoint(Line line)
		{
			var p = CrossPoint(StartPosition, EndPosition, line.StartPosition, line.EndPosition);
			return ContainsPoint(p) && line.ContainsPoint(p) ? p : Point.Invalid;
		}
		public bool ContainsPoint(Point point)
		{
			var sum = Point.Distance(StartPosition, point) + Point.Distance(EndPosition, point);
			return Number.IsBetween(sum, new Number.Range(Length - 0.01, Length + 0.01));
		}
		public bool Crosses(Line line) => CrossPoint(line).IsInvalid() == false;

		public void Display(Camera camera, double width)
		{
			if (Window.DrawNotAllowed()) return;

			width /= 2;
			var startLeft = Point.MoveAtAngle(StartPosition, Angle - 90, width, Time.Unit.Frame);
			var startRight = Point.MoveAtAngle(StartPosition, Angle + 90, width, Time.Unit.Frame);
			var endLeft = Point.MoveAtAngle(EndPosition, Angle - 90, width, Time.Unit.Frame);
			var endRight = Point.MoveAtAngle(EndPosition, Angle + 90, width, Time.Unit.Frame);

			var vert = new Vertex[]
			{
				new(Point.From(startLeft), Color.From(StartPosition.C)),
				new(Point.From(startRight), Color.From(StartPosition.C)),
				new(Point.From(endRight), Color.From(EndPosition.C)),
				new(Point.From(endLeft), Color.From(EndPosition.C)),
			};
			camera.rendTexture.Draw(vert, PrimitiveType.Quads);
			Performance.DrawCallsPerFrame++;
			Performance.QuadDrawsPerFrame++;
			Performance.VertexDrawsPerFrame += 4;
		}
	}
}
