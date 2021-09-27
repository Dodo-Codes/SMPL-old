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
			X = x;
			Y = y;
		}
		public void Display(Camera camera, double width)
		{
			if (Window.DrawNotAllowed()) return;

			width /= 2;
			var topLeft = MoveAtAngle(this, 225, width, Time.Unit.Tick);
			var topRight = MoveAtAngle(this, 315, width, Time.Unit.Tick);
			var botLeft = MoveAtAngle(this, 135, width, Time.Unit.Tick);
			var botRight = MoveAtAngle(this, 45, width, Time.Unit.Tick);

			var vert = new Vertex[]
			{
				new(From(topLeft), Color.From(Color)),
				new(From(topRight), Color.From(Color)),
				new(From(botRight), Color.From(Color)),
				new(From(botLeft), Color.From(Color)),
			};
			camera.rendTexture.Draw(vert, PrimitiveType.Quads);
		}

		public bool IsInvalid() => double.IsNaN(X) || double.IsNaN(Y);
		public static double Distance(Point pointA, Point pointB)
		{
			return Math.Sqrt(Math.Pow(pointB.X - pointA.X, 2) + Math.Pow(pointB.Y - pointA.Y, 2));
		}
		public static Point MoveAtAngle(Point point, double angle, double speed,
			Gear.Time.Unit timeUnit = Gear.Time.Unit.Second)
		{
			if (timeUnit == Gear.Time.Unit.Second) speed *= Performance.DeltaTime;
			var dir = Number.AngleToDirection(angle);

			point.X += dir.X * speed;
			point.Y += dir.Y * speed;
			return point;
		}
		public static Point MoveTowardTarget(Point point, Point targetPoint, double speed,
			Gear.Time.Unit timeUnit = Gear.Time.Unit.Second)
		{
			var ang = Number.AngleBetweenPoints(point, targetPoint);
			return MoveAtAngle(point, ang, speed, timeUnit);
		}
		public static Point PercentTowardTarget(Point point, Point targetPoint, Size percent)
		{
			var x = Number.FromPercent(percent.W, new Number.Range(point.X, targetPoint.X));
			var y = Number.FromPercent(percent.H, new Number.Range(point.Y, targetPoint.Y));
			return new Point(x, y);
		}

		internal static double Magnitude(Point p) => Number.SquareRoot(p.X * p.X + p.Y * p.Y);
		internal static Point Normalize(Point p)
		{
			var m = Magnitude(p);
			var result = new Point(p.X / m, p.Y / m);
			return result.IsInvalid() ? new Point(0, 0) : result;
		}

		public static Point operator +(Point a, Point b) => new(a.X + b.X, a.Y + b.Y);
		public static Point operator -(Point a, Point b) => new(a.X - b.X, a.Y - b.Y);
		public static Point operator *(Point a, Point b) => new(a.X * b.X, a.Y * b.Y);
		public static Point operator /(Point a, Point b) => new(a.X / b.X, a.Y / b.Y);
		public static bool operator ==(Point a, Point b) => a.X == b.X && a.Y == b.Y;
		public static bool operator !=(Point a, Point b) => a.X != b.X || a.Y != b.Y;
		public static Point operator /(Point a, double b) => new(a.X / b, a.Y / b);
		public static Point operator *(Point a, double b) => new(a.X * b, a.Y * b);

		public override bool Equals(object obj) => obj.GetType() == typeof(Point) && (Point)obj == this;
		public override int GetHashCode() => base.GetHashCode();

		public override string ToString() => $"{X} {Y}";

		internal static Point To(Vector2f point) => new(point.X, point.Y);
		internal static Vector2f From(Point point) => new((float)point.X, (float)point.Y);
	}
}
