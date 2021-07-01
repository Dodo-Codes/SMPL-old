using SFML.System;
using System;

namespace SMPL
{
	public static class Rotation
	{
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
		public static double AngleToTarget(double angle, double targetAngle, double speed, Time.Unit timeUnit = Time.Unit.Second)
		{
			angle = To360(angle);
			targetAngle = To360(targetAngle);
			speed = Math.Abs(speed);
			var difference = angle - targetAngle;

			// stops the rotation with an else when close enough
			// prevents the rotation from staying behind after the stop
			var checkedSpeed = speed;
			if (timeUnit == Time.Unit.Second)
			{
				checkedSpeed *= Time.TickDeltaTime;
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
