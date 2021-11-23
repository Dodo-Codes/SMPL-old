using SFML.Graphics;
using SFML.System;
using System;
using SMPL.Components;
using SMPL.Gear;
using Time = SMPL.Gear.Time;

namespace SMPL.Data
{
	public struct Point
	{
		public static Point Invalid => new(double.NaN, double.NaN) { Color = Color.Invalid };
		public static Point One => new(1, 1);

		public double X { get; set; }
		public double Y { get; set; }
		public Color Color { get; set; }

		public Point(double x, double y)
		{
			Color = Color.White;
			if (x == -0)
				x = 0;
			if (y == -0)
				y = 0;

			X = x;
			Y = y;
		}
		public void Display(Camera camera, double width)
		{
			if (Window.DrawNotAllowed()) return;

			width /= 2;
			var topLeft = MoveAtAngle(this, 225, width, Time.Unit.Frame);
			var topRight = MoveAtAngle(this, 315, width, Time.Unit.Frame);
			var botLeft = MoveAtAngle(this, 135, width, Time.Unit.Frame);
			var botRight = MoveAtAngle(this, 45, width, Time.Unit.Frame);

			var vert = new Vertex[]
			{
				new(From(topLeft), Color.From(Color)),
				new(From(topRight), Color.From(Color)),
				new(From(botRight), Color.From(Color)),
				new(From(botLeft), Color.From(Color)),
			};
			camera.rendTexture.Draw(vert, PrimitiveType.Quads);
			Performance.DrawCallsPerFrame++;
			Performance.QuadDrawsPerFrame++;
			Performance.VertexDrawsPerFrame += 4;
		}

		public bool IsInvalid() => double.IsNaN(X) || double.IsNaN(Y);
		public static double Distance(Point pointA, Point pointB)
		{
			return Math.Sqrt(Math.Pow(pointB.X - pointA.X, 2) + Math.Pow(pointB.Y - pointA.Y, 2));
		}
		public static Point MoveAtAngle(Point point, double angle, double speed, Time.Unit timeUnit = Time.Unit.Second)
		{
			if (timeUnit == Gear.Time.Unit.Second) speed *= Performance.DeltaTime;
			var dir = Number.AngleToDirection(angle);

			point.X += dir.X * speed;
			point.Y += dir.Y * speed;
			return point;
		}
		public static Point MoveTowardTarget(Point point, Point targetPoint, double speed, Time.Unit timeUnit = Time.Unit.Second)
		{
			var ang = Number.AngleBetweenPoints(point, targetPoint);
			return MoveAtAngle(point, ang, speed, timeUnit);
		}
		public static Point PercentTowardTarget(Point point, Point targetPoint, Size percent)
		{
			var x = Number.FromPercent(percent.W, new Number.Range(point.X, targetPoint.X));
			var y = Number.FromPercent(percent.H, new Number.Range(point.Y, targetPoint.Y));
			return new Point(x, y) { Color = point.Color };
		}
		public static Point Pathfind(Point pointA, Point pointB, uint tries, params Line[] lines)
		{
			if (tries == 0)
				return Invalid;

			if (Crosses(pointA, pointB, pointB) == false)
				return pointB;

			var bestPoint = Invalid;
			var bestDist = double.MaxValue;
			for (int i = 0; i < tries; i++)
			{
				var randPoint = MoveAtAngle(pointA, i * (360 / tries), Distance(pointA, pointB), Time.Unit.Frame);
				var sumDist = Distance(pointA, randPoint) + Distance(randPoint, pointB);
				if (Crosses(pointA, randPoint, pointB) == false && sumDist < bestDist)
				{
					bestDist = sumDist;
					bestPoint = randPoint;
				}
			}

			for (int i = 0; i < tries; i++)
			{
				var curP = PercentTowardTarget(pointA, bestPoint, new(100 / tries * i, 100 / tries * i));
				if (Crosses(curP, pointB, pointB) == false)
					return curP;
			}

			return bestPoint;

			bool Crosses(Point p1, Point p2, Point p3)
			{
				for (int i = 0; i < lines.Length; i++)
				{
					var cross1 = lines[i].Crosses(new Line(p1, p2));
					var cross2 = lines[i].Crosses(new Line(p2, p3));
					if (cross1 || cross2)
						return true;
				}
				return false;
			}
		}

		internal static double Magnitude(Point p) => Number.SquareRoot(p.X * p.X + p.Y * p.Y);
		internal static Point Normalize(Point p)
		{
			var m = Magnitude(p);
			var result = new Point(p.X / m, p.Y / m) { Color = p.Color };
			return result.IsInvalid() ? Invalid : result;
		}

		public static Point operator +(Point a, Point b) => new(a.X + b.X, a.Y + b.Y) { Color = a.Color };
		public static Point operator -(Point a, Point b) => new(a.X - b.X, a.Y - b.Y) { Color = a.Color };
		public static Point operator *(Point a, Point b) => new(a.X * b.X, a.Y * b.Y) { Color = a.Color };
		public static Point operator /(Point a, Point b) => new(a.X / b.X, a.Y / b.Y) { Color = a.Color };
		public static bool operator ==(Point a, Point b) => a.X == b.X && a.Y == b.Y;
		public static bool operator !=(Point a, Point b) => a.X != b.X || a.Y != b.Y;
		public static Point operator /(Point a, double b) => new(a.X / b, a.Y / b) { Color = a.Color };
		public static Point operator *(Point a, double b) => new(a.X * b, a.Y * b) { Color = a.Color };

		public override bool Equals(object obj) => obj.GetType() == typeof(Point) && (Point)obj == this;
		public override int GetHashCode() => base.GetHashCode();

		public override string ToString() => $"{X} {Y}";

		internal static Point To(Vector2f point) => new(point.X, point.Y);
		internal static Vector2f From(Point point) => new((float)point.X, (float)point.Y);
	}
}
