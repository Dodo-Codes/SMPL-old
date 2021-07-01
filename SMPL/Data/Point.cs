using SFML.System;
using System;

namespace SMPL
{
	public class Point
	{
		public double X { get; set; }
		public double Y { get; set; }

		public Point(double x, double y)
		{
			X = x;
			Y = y;
		}
		public static double GetDistance(Point positionA, Point positionB)
		{
			return Math.Sqrt(Math.Pow(positionB.X - positionA.X, 2) + Math.Pow(positionB.Y - positionA.Y, 2));
		}

		public static Point operator +(Point a, Point b) => new Point(a.X + b.Y, a.X + b.Y);
		public static Point operator -(Point a, Point b) => new Point(a.X - b.X, a.Y - b.Y);
		public static Point operator *(Point a, Point b) => new Point(a.X * b.X, a.Y * b.Y);
		public static Point operator /(Point a, Point b) => new Point(a.X / b.X, a.Y / b.Y);
		public static bool operator ==(Point a, Point b) => a.X == b.X && a.Y == b.Y;
		public static bool operator !=(Point a, Point b) => a.X != b.X && a.Y != b.Y;
		public static Point operator /(Point a, float b) => new Point(a.X / b, a.Y / b);
		public static Point operator *(Point a, float b) => new Point(a.X * b, a.Y * b);

		/// <summary>
		/// This default <see cref="object"/> method is not implemented.
		/// </summary>
		public override bool Equals(object obj) => default;
		/// <summary>
		/// This default <see cref="object"/> method is not implemented.
		/// </summary>
		public override int GetHashCode() => default;

		public override string ToString() => $"{X} {Y}";
	}
}
