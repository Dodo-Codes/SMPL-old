namespace SMPL
{
	public struct Size
	{
		public double Width { get; set; }
		public double Height { get; set; }

		public Size(double width, double height)
		{
			Width = width;
			Height = height;
		}
		public static Size Resize(Size size, double speed, Time.Unit timeUnit = Time.Unit.Second)
		{
			size.Width = Number.Move(size.Width, speed, timeUnit);
			size.Height = Number.Move(size.Height, speed, timeUnit);
			return size;
		}
		public static Size ResizeTowardTarget(Size size, Size targetSize, double speed, Time.Unit timeUnit = Time.Unit.Second)
		{
			size = Resize(size, speed, timeUnit);
			var sp = timeUnit == Time.Unit.Second ? speed * Time.DeltaTime * 2 : speed * 2;
			var dist = Point.GetDistance(new Point(size.Width, size.Height), new Point(targetSize.Width, targetSize.Height));
			if (dist < sp) size = targetSize;
			return size;
		}

		public override string ToString() => $"{Width} {Height}";
		/// <summary>
		/// This default <see cref="object"/> method is not implemented.
		/// </summary>
		public override bool Equals(object obj) => default;
		/// <summary>
		/// This default <see cref="object"/> method is not implemented.
		/// </summary>
		public override int GetHashCode() => default;

		public static Size operator +(Size a, Size b) =>	new(a.Width + b.Width, a.Height + b.Height);
		public static Size operator -(Size a, Size b) =>	new(a.Width - b.Width, a.Height - b.Height);
		public static Size operator *(Size a, Size b) =>	new(a.Width * b.Width, a.Height * b.Height);
		public static Size operator /(Size a, Size b) =>	new(a.Width / b.Width, a.Height / b.Height);
		public static bool operator ==(Size a, Size b) => a.Width == b.Width && a.Height == b.Height;
		public static bool operator !=(Size a, Size b) => a.Width != b.Width && a.Height != b.Height;
		public static Size operator *(Size a, double b) =>	new(a.Width * b,		  a.Height * b);
		public static Size operator /(Size a, double b) =>	new(a.Width / b,		  a.Height / b);
	}
}
