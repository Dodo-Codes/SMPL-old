using SFML.Graphics;
using SFML.System;
using System;
using SMPL.Components;
using SMPL.Gear;

namespace SMPL.Data
{
	public struct Point
	{
		public static Point Invalid => new(double.NaN, double.NaN) { Color = Color.Invalid };
		public static Point One => new(1, 1) { Color = Color.White };
		public static Point Zero => new(0, 0) { Color = Color.White };
		internal static Storage<int, double> randAngleConstrains = new();

		public double X { get; set; }
		public double Y { get; set; }
		public Color Color { get; set; }

		internal static void Initialize()
		{
			for (int i = 0; i < 200; i++)
				randAngleConstrains.Expand(i, i, Number.Random(new Bounds(0, 360), 1));
		}

		public Point(double x, double y)
		{
			Color = Color.White;
			X = x;
			Y = y;
		}
		public void Display(Camera camera)
		{
			if (Window.DrawNotAllowed()) return;
			var vert = new Vertex[] { new(From(this), Color.From(Color)) };
			camera.rendTexture.Draw(vert, PrimitiveType.Points);
		}

		public bool IsInvalid => double.IsNaN(X) || double.IsNaN(Y);
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
			var x = Number.FromPercent(percent.W, new Bounds(point.X, targetPoint.X));
			var y = Number.FromPercent(percent.H, new Bounds(point.Y, targetPoint.Y));
			return new Point(x, y);
		}

		public static Point[] Constrain(Point originPoint, Point targetPoint, params double[] segmentLengths)
		{
			var result = new Point[segmentLengths.Length + 1];
			result[0] = originPoint;

			for (int i = 1; i < result.Length; i++)
			{
				result[i] = new Point(i * segmentLengths[i - 1], 0);
			}
			Constrain(result, targetPoint);
			return result;
		}
		public static void Constrain(Point[] points, Point targetPoint)
		{
			var originPoint = points[0];
			var segmentLengths = new double[points.Length - 1];
			for (int i = 0; i < points.Length - 1; i++)
				segmentLengths[i] = Magnitude(points[i + 1] - points[i]);

			for (int i = 0; i < 100; i++)
			{
				var startingFromTarget = i % 2 == 0;
				Array.Reverse(points);
				Array.Reverse(segmentLengths);
				points[0] = startingFromTarget ? targetPoint : originPoint;

				for (int j = 1; j < points.Length; j++)
				{
					var dir = Normalize(points[j] - points[j - 1]);
					points[j] = points[j - 1] + dir * segmentLengths[j - 1];
				}
				var dstToTarget = Magnitude(points[^1] - targetPoint);
				if (startingFromTarget == false && dstToTarget <= 0.01) return;
			}

			double Magnitude(Point p) => Number.SquareRoot(p.X * p.X + p.Y * p.Y);
			Point Normalize(Point p) { var m = Magnitude(p); return new Point(p.X / m, p.Y / m); }
		}

		public static Point operator +(Point a, Point b) => new(a.X + b.X, a.Y + b.Y);
		public static Point operator -(Point a, Point b) => new(a.X - b.X, a.Y - b.Y);
		public static Point operator *(Point a, Point b) => new(a.X * b.X, a.Y * b.Y);
		public static Point operator /(Point a, Point b) => new(a.X / b.X, a.Y / b.Y);
		public static bool operator ==(Point a, Point b) => a.X == b.X && a.Y == b.Y;
		public static bool operator !=(Point a, Point b) => a.X != b.X || a.Y != b.Y;
		public static Point operator /(Point a, double b) => new(a.X / b, a.Y / b);
		public static Point operator *(Point a, double b) => new(a.X * b, a.Y * b);

		/// <summary>
		/// This default <see cref="object"/> method is not implemented.
		/// </summary>
		public override bool Equals(object obj) => default;
		/// <summary>
		/// This default <see cref="object"/> method is not implemented.
		/// </summary>
		public override int GetHashCode() => default;

		public override string ToString() => $"{X} {Y}";

		internal static Point To(Vector2f point) => new(point.X, point.Y);
		internal static Vector2f From(Point point) => new((float)point.X, (float)point.Y);
	}
}
