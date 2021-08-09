using SFML.System;

namespace SMPL
{
	public struct Size
	{
		public double W { get; set; }
		public double H { get; set; }

		public Size(double width, double height)
		{
			W = width;
			H = height;
		}
		public static Size Resize(Size size, double speed, Time.Unit timeUnit = Time.Unit.Second)
		{
			size.W = Number.Move(size.W, speed, timeUnit);
			size.H = Number.Move(size.H, speed, timeUnit);
			return size;
		}
		public static Size ResizeTowardTarget(Size size, Size targetSize, double speed, Time.Unit timeUnit = Time.Unit.Second)
		{
			size = Resize(size, speed, timeUnit);
			var sp = timeUnit == Time.Unit.Second ? speed * Performance.DeltaTime * 2 : speed * 2;
			var dist = Point.Distance(new Point(size.W, size.H), new Point(targetSize.W, targetSize.H));
			if (dist < sp) size = targetSize;
			return size;
		}

		public override string ToString() => $"{W} {H}";
		/// <summary>
		/// This default <see cref="object"/> method is not implemented.
		/// </summary>
		public override bool Equals(object obj) => default;
		/// <summary>
		/// This default <see cref="object"/> method is not implemented.
		/// </summary>
		public override int GetHashCode() => default;

		public static Size operator +(Size a, Size b) => new(a.W + b.W, a.H + b.H);
		public static Size operator -(Size a, Size b) => new(a.W - b.W, a.H - b.H);
		public static Size operator *(Size a, Size b) => new(a.W * b.W, a.H * b.H);
		public static Size operator /(Size a, Size b) => new(a.W / b.W, a.H / b.H);
		public static bool operator ==(Size a, Size b) => a.W == b.W && a.H == b.H;
		public static bool operator !=(Size a, Size b) => a.W != b.W || a.H != b.H;
		public static Size operator *(Size a, double b) =>	new(a.W * b, a.H * b);
		public static Size operator /(Size a, double b) =>	new(a.W / b, a.H / b);

		internal static Size To(Vector2f size) => new(size.X, size.Y);
		internal static Vector2f From(Size size) => new((float)size.W, (float)size.H);
	}
}
