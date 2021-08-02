using SFML.System;
using System;
using System.Globalization;
using System.Linq;

namespace SMPL
{
	public static class Number
	{
		public enum Limitation
		{
			ClosestBound, Overflow
		}
		public enum RoundToward
		{
			Closest, Up, Down
		}
		public enum RoundWhen5
		{
			TowardEven, AwayFromZero, TowardZero, TowardNegativeInfinity, TowardPositiveInfinity
		}

		public static double Limit(double number, Bounds bounds, Limitation limitType = Limitation.ClosestBound)
		{
			if (limitType == Limitation.ClosestBound)
			{
				if (number < bounds.Lower)
				{
					return bounds.Lower;
				}
				else if (number > bounds.Upper)
				{
					return bounds.Upper;
				}
				return number;
			}
			else
			{
				bounds.Upper += 1;
				var a = number;
				a = Map(a);
				while (a < bounds.Lower)
				{
					a = Map(a);
				}
				return a;
				double Map(double b)
				{
					b = ((b % bounds.Upper) + bounds.Upper) % bounds.Upper;
					if (b < bounds.Lower) b = bounds.Upper - (bounds.Lower - b);
					return b;
				}
			}
		}
		public static double Sign(double number, bool signed)
		{
			return signed ? -Math.Abs(number) : Math.Abs(number);
		}
		public static double Random(Bounds bounds, double precision = 0, double seed = double.NaN)
		{
			precision = (int)Limit(precision, new Bounds(0, 5), Limitation.ClosestBound);
			var precisionValue = (double)Math.Pow(10, precision);
			var lowerInt = Convert.ToInt32(bounds.Lower * Math.Pow(10, GetPrecision(bounds.Lower)));
			var upperInt = Convert.ToInt32(bounds.Upper * Math.Pow(10, GetPrecision(bounds.Upper)));
			var s = new Random(double.IsNaN(seed) ? Guid.NewGuid().GetHashCode() : (int)Round(seed));
			var randInt = s.Next((int)(lowerInt * precisionValue), (int)(upperInt * precisionValue) + 1);
			var result = randInt / precisionValue;

			return result;
		}
		public static double Average(params double[] numbers)
		{
			return numbers == null ? double.NaN : numbers.Sum() / numbers.Length;
		}
		public static double GetPrecision(double number)
		{
			var cultDecPoint = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
			var split = number.ToString(cultDecPoint).Split();
			return split.Length > 1 ? split[1].Length : 0;
		}
		public static double Round(double number, double precision = 0,
			RoundToward toward = RoundToward.Closest,
			RoundWhen5 priority = RoundWhen5.AwayFromZero)
		{
			var midpoint = (MidpointRounding)priority;
			precision = (int)Limit(precision, new Bounds(0, 5), Limitation.ClosestBound);

			if (toward == RoundToward.Down || toward == RoundToward.Up)
			{
				var numStr = number.ToString();
				var prec = GetPrecision(number);
				if (prec > 0 && prec > precision)
				{
					var digit = toward == RoundToward.Down ? "1" : "9";
					numStr = numStr.Remove(numStr.Length - 1);
					numStr = $"{numStr}{digit}";
					number = double.Parse(numStr);
				}
			}

			return Math.Round(number, (int)precision, midpoint);
		}
		public static double ToPercent(double number, Bounds bounds)
		{
			return (number - bounds.Lower) * 100.0 / (bounds.Upper - bounds.Lower);
		}
		public static double FromPercent(double percent, Bounds bounds)
		{
			return (percent * (bounds.Upper - bounds.Lower) / 100) + bounds.Lower;
		}
		public static double FromText(string text)
		{
			var result = 0.0;
			text = text.Replace(',', '.');
			var parsed = double.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out result);

			return parsed ? result : double.NaN;
		}
		public static bool HasChance(double percent)
		{
			percent = Limit(percent, new Bounds(0, 100), Limitation.ClosestBound);
			var n = Random(new Bounds(1, 100), 0); // should not roll 0 so it doesn't return true with 0% (outside of roll)
			return n <= percent;
		}
		public static bool IsBetween(double number, Bounds bounds, bool inclusiveLower = false, bool inclusiveUpper = false)
		{
			var lower = false;
			var upper = false;
			lower = inclusiveLower ? bounds.Lower <= number : bounds.Lower < number;
			upper = inclusiveUpper ? bounds.Upper >= number : bounds.Upper > number;
			return lower && upper;
		}
		public static double Move(double number, double speed, Time.Unit motion = Time.Unit.Second)
		{
			if (motion == Time.Unit.Second) speed *= Time.DeltaTime;
			return number + speed;
		}
		public static double MoveToward(double number, double targetNumber, double speed, Time.Unit motion = Time.Unit.Second)
		{
			var goingPos = number < targetNumber;
			var result = Move(number, goingPos ? Sign(speed, false) : Sign(speed, true), motion);

			if (goingPos && result > targetNumber) return targetNumber;
			else if (goingPos == false && result < targetNumber) return targetNumber;
			return result;
		}
		public static double Map(double value, Bounds boundsA, Bounds boundsB)
		{
			return (value - boundsA.Lower) / (boundsA.Upper - boundsA.Lower) * (boundsB.Upper - boundsB.Lower) + boundsB.Lower;
		}
		public static double GetAngleBetweenPoints(Point point, Point targetPoint)
		{
			var dir = DirectionBetweenPoints(point, targetPoint);
			return DirectionToAngle(dir);
		}
		public static double MoveTowardAngle(double angle, double targetAngle, double speed, Time.Unit timeUnit = Time.Unit.Second)
		{
			angle = To360(angle);
			targetAngle = To360(targetAngle);
			speed = Math.Abs(speed);
			var difference = angle - targetAngle;

			// stops the rotation with an else when close enough
			// prevents the rotation from staying behind after the stop
			var checkedSpeed = speed;
			if (timeUnit == Time.Unit.Second) checkedSpeed *= Time.DeltaTime;
			if (Math.Abs(difference) < checkedSpeed) angle = targetAngle;
			else if (difference >= 0 && difference < 180) angle = Move(angle, -speed, timeUnit);
			else if (difference >= -180 && difference < 0) angle = Move(angle, speed, timeUnit);
			else if (difference >= -360 && difference < -180) angle = Move(angle, -speed, timeUnit);
			else if (difference >= 180 && difference < 360) angle = Move(angle, speed, timeUnit);

			// detects speed greater than possible
			// prevents jiggle when passing 0-360 & 360-0 | simple to fix yet took me half a day
			if (Math.Abs(difference) > 360 - checkedSpeed) angle = targetAngle;

			return angle;
		}

		internal static double DirectionToAngle(Vector2f direction)
		{
			//Vector2 to Radians: atan2(Vector2.y, Vector2.x)
			//Radians to Angle: radians * (180 / Math.PI)

			var rad = (double)Math.Atan2(direction.Y, direction.X);
			return (float)(rad * (180 / Math.PI));
		}
		internal static Vector2f AngleToDirection(double angle)
		{
			//Angle to Radians : (Math.PI / 180) * angle
			//Radians to Vector2 : Vector2.x = cos(angle) | Vector2.y = sin(angle)

			var rad = Math.PI / 180 * angle;
			var dir = new Vector2f((float)Math.Cos(rad), (float)Math.Sin(rad));

			return new Vector2f(dir.X, dir.Y);
		}
		internal static Vector2f DirectionBetweenPoints(Point point, Point targetPoint)
		{
			return Point.From(targetPoint - point);
		}
		internal static double To360(double angle)
		{
			return ((angle % 360) + 360) % 360;
		}
	}
}
