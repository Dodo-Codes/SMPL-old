namespace SMPL.Data
{
	public class Point
	{
		public double X, Y;

		public Point(double x, double y)
		{
			X = x;
			Y = y;
		}

		public static Point operator +(Point a, Point b) => new Point(a.X + b.Y, a.X + b.Y);
		public static Point operator -(Point a, Point b) => new Point(a.X - b.X, a.Y - b.Y);
		public static Point operator *(Point a, Point b) => new Point(a.X * b.X, a.Y * b.Y);
		public static Point operator /(Point a, Point b) => new Point(a.X / b.X, a.Y / b.Y);
		public static Point operator /(Point a, float b) => new Point(a.X / b, a.Y / b);
		public static Point operator *(Point a, float b) => new Point(a.X * b, a.Y * b);
		public static bool operator ==(Point a, Point b) => a.X == b.X && a.Y == b.Y;
		public static bool operator !=(Point a, Point b) => a.X != b.X && a.Y != b.Y;

		/// <summary>
		/// This default <see cref="object"/> method is not implemented.
		/// </summary>
		public override bool Equals(object obj) => default;
		/// <summary>
		/// This default <see cref="object"/> method is not implemented.
		/// </summary>
		public override int GetHashCode() => default;
	}
}
