using SFML.System;
using System;

namespace SMPL.Data
{
	public static class Rotation
	{
		public enum Sample
		{
			Up, Down, Left, Right, UpRight, UpLeft, DownRight, DownLeft
		}

		public static Direction SampleToDirection(Sample rotationSample)
		{
			var vec = new Vector2f();
			switch (rotationSample)
			{
				case Sample.Up: vec = new Vector2f(0, -1); break;
				case Sample.Down: vec = new Vector2f(0, 1); break;
				case Sample.Left: vec = new Vector2f(-1, 0); break;
				case Sample.Right: vec = new Vector2f(1, 0); break;
				case Sample.UpRight: vec = new Vector2f(1, -1); break;
				case Sample.UpLeft: vec = new Vector2f(-1, -1); break;
				case Sample.DownRight: vec = new Vector2f(1, 1); break;
				case Sample.DownLeft: vec = new Vector2f(-1, 1); break;
			}
			return new Direction(vec.X, vec.Y);
		}
		public static double DirectionToAngle(Direction direction)
		{
			//Vector2 to Radians: atan2(Vector2.y, Vector2.x)
			//Radians to Angle: radians * (180 / Math.PI)

			var rad = (double)Math.Atan2(direction.Y, direction.X);
			return (float)(rad * (180 / Math.PI));
		}
		public static Direction AngleToDirection(double angle)
		{
			//Angle to Radians : (Math.PI / 180) * angle
			//Radians to Vector2 : Vector2.x = cos(angle) | Vector2.y = sin(angle)

			var rad = Math.PI / 180 * angle;
			var dir = new Vector2f((float)Math.Cos(rad), (float)Math.Sin(rad));

			return new Direction(dir.X, dir.Y);
		}
		public static Direction DirectionBetweenPoints(Point point, Point targetPoint)
		{
			var p = targetPoint - point;
			return new Direction(p.X, p.Y);
		}
		public static double AngleBetweenPoints(Point point, Point targetPoint)
		{
			var dir = DirectionBetweenPoints(point, targetPoint);
			return DirectionToAngle(dir);
		}
		public static double SampleToAngle(Sample rotationSample)
		{
			switch (rotationSample)
			{
				case Sample.Up: return 270;
				case Sample.Left: return 180;
				case Sample.Right: return 0;
				case Sample.Down: return 90;
				case Sample.UpLeft: return 225;
				case Sample.UpRight: return 315;
				case Sample.DownLeft: return 135;
				case Sample.DownRight: return 45;
				default: return 0;
			}
		}
		public static double AngleToTarget(double angle, double targetAngle, double speed,
			Number.TimeUnit timeUnit = Number.TimeUnit.Second)
		{
			angle = To360(angle);
			targetAngle = To360(targetAngle);
			speed = Math.Abs(speed);
			var difference = angle - targetAngle;

			// stops the rotation with an else when close enough
			// prevents the rotation from staying behind after the stop
			var checkedSpeed = speed;
			if (timeUnit == Number.TimeUnit.Second)
			{
				checkedSpeed *= Performance.Time.TickDeltaTime;
			}
			if (Math.Abs(difference) < checkedSpeed) angle = targetAngle;
			else if (difference > 0 && difference < 180) Number.Move(angle, -speed, timeUnit);
			else if (difference > -180 && difference < 0) Number.Move(angle, speed, timeUnit);
			else if (difference > -360 && difference < -180) Number.Move(angle, -speed, timeUnit);
			else if (difference > 180 && difference < 360) Number.Move(angle, speed, timeUnit);

			// detects speed greater than possible
			// prevents jiggle when passing 0-360 & 360-0 | simple to fix yet took me half a day
			if (Math.Abs(difference) > 360 - checkedSpeed) angle = targetAngle;

			return angle;
		}

		internal static double To360(double angle)
		{
			return ((angle % 360) + 360) % 360;
		}
	}
}
