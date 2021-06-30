using SFML.System;
using System;

namespace SMPL.Data
{
	public struct Direction
	{
		private double x, y;

		public double X
		{
			get { return x; }
			set
			{
				x = value;
				Normalize();
			}
		}
		public double Y
		{
			get { return y; }
			set
			{
				y = value;
				Normalize();
			}
		}

		public Direction(double x, double y)
		{
			this.x = 0;
			this.y = 0;
			X = x;
			Y = y;
		}
		public Direction(Rotation.Sample rotationSample)
		{
			x = 0;
			y = 0;

			var dir = Rotation.SampleToDirection(rotationSample);
			X = dir.X;
			Y = dir.Y;
		}
		public Direction(double angle)
		{
			x = 0;
			y = 0;

			var dir = Rotation.AngleToDirection(angle);
			X = dir.X;
			Y = dir.Y;
		}

		internal void Normalize()
		{
			var dist = (float)Math.Sqrt(x * x + y * y);
			x /= dist;
			y /= dist;
		}

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
