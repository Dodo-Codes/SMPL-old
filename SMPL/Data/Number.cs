using SFML.System;
using SMPL.Gear;
using System;
using System.Globalization;
using System.Linq;

namespace SMPL.Data
{
	public static class Number
	{
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

		// =============

		public const double PI = 3.1415926535897931;

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
		public enum DataSizeConvertion
		{
			Bit_Byte, Bit_KB,
			Byte_Bit, Byte_KB, Byte_MB,
			KB_Bit, KB_Byte, KB_MB, KB_GB,
			MB_Byte, MB_KB, MB_GB, MB_TB,
			GB_KB, GB_MB, GB_TB,
			TB_MB, TB_GB
		}
		public enum AnimationType
		{
			BendWeak, // Sine
			Bend, // Cubic
			BendStrong, // Quint
			Circle, // Circ
			Elastic, // Elastic
			Swing, // Back
			Bounce // Bounce
		}
		public enum AnimationCurve { In, Out, InOut }

		public static double Cos(double number) => Math.Cos(number);
		public static double Sin(double number) => Math.Sin(number);
		public static double Tan(double number) => Math.Tan(number);
		public static double Power(double number, double power) => Math.Pow(number, power);
		public static double SquareRoot(double number) => Math.Sqrt(number);

		public static double Animate(double progressPercent, AnimationType animationType, AnimationCurve animationCurve)
		{
			var result = 0d;
			progressPercent /= 100;
			var x = progressPercent;
			switch (animationType)
			{
				case AnimationType.BendWeak:
					{
						result = animationCurve == AnimationCurve.In ? 1 - Cos(x * PI / 2) :
							animationCurve == AnimationCurve.Out ? 1 - Sin(x * PI / 2) :
							-(Cos(PI * x) - 1) / 2;
						break;
					}
				case AnimationType.Bend:
					{
						result = animationCurve == AnimationCurve.In ? x * x * x :
							animationCurve == AnimationCurve.Out ? 1 - Power(1 - x, 3) :
							(x < 0.5 ? 4 * x * x * x : 1 - Power(-2 * x + 2, 3) / 2);
						break;
					}
				case AnimationType.BendStrong:
					{
						result = animationCurve == AnimationCurve.In ? x * x * x * x :
							animationCurve == AnimationCurve.Out ? 1 - Power(1 - x, 5) :
							(x < 0.5 ? 16 * x * x * x * x * x : 1 - Power(-2 * x + 2, 5) / 2);
						break;
					}
				case AnimationType.Circle:
					{
						result = animationCurve == AnimationCurve.In ? 1 - SquareRoot(1 - Power(x, 2)) :
							animationCurve == AnimationCurve.Out ? SquareRoot(1 - Power(x - 1, 2)) :
							(x < 0.5 ? (1 - SquareRoot(1 - Power(2 * x, 2))) / 2 : (SquareRoot(1 - Power(-2 * x + 2, 2)) + 1) / 2);
						break;
					}
				case AnimationType.Elastic:
					{
						result = animationCurve == AnimationCurve.In ?
							(x == 0 ? 0 : x == 1 ? 1 : -Power(2, 10 * x - 10) * Sin((x * 10 - 10.75) * ((2 * PI) / 3))) :
							animationCurve == AnimationCurve.Out ?
							(x == 0 ? 0 : x == 1 ? 1 : Power(2, -10 * x) * Sin((x * 10 - 0.75) * (2 * PI) / 3) + 1) :
							(x == 0 ? 0 : x == 1 ? 1 : x < 0.5 ? -(Power(2, 20 * x - 10) * Sin((20 * x - 11.125) * (2 * PI) / 4.5)) / 2 :
							(Power(2, -20 * x + 10) * Sin((20 * x - 11.125) * (2 * PI) / 4.5)) / 2 + 1);
						break;
					}
				case AnimationType.Swing:
					{
						result = animationCurve == AnimationCurve.In ? 2.70158 * x * x * x - 1.70158 * x * x :
							animationCurve == AnimationCurve.Out ? 1 + 2.70158 * Power(x - 1, 3) + 1.70158 * Power(x - 1, 2) :
							(x < 0.5 ? (Power(2 * x, 2) * ((2.59491 + 1) * 2 * x - 2.59491)) / 2 :
							(Power(2 * x - 2, 2) * ((2.59491 + 1) * (x * 2 - 2) + 2.59491) + 2) / 2);
						break;
					}
				case AnimationType.Bounce:
					{
						result = animationCurve == AnimationCurve.In ? 1 - easeOutBounce(1 - x) :
							animationCurve == AnimationCurve.Out ? easeOutBounce(x) :
							(x < 0.5 ? (1 - easeOutBounce(1 - 2 * x)) / 2 : (1 + easeOutBounce(2 * x - 1)) / 2);
						break;
					}
			}
			return result * 100;

			double easeOutBounce(double x)
			{
				return x < 1 / 2.75 ? 7.5625 * x * x : x < 2 / 2.75 ? 7.5625 * (x -= 1.5 / 2.75) * x + 0.75 :
					x < 2.5 / 2.75 ? 7.5625 * (x -= 2.25 / 2.75) * x + 0.9375 : 7.5625 * (x -= 2.625 / 2.75) * x + 0.984375;
			}
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
			var lowerInt = Convert.ToInt32(bounds.Lower * Math.Pow(10, Precision(bounds.Lower * 500)));
			var upperInt = Convert.ToInt32(bounds.Upper * Math.Pow(10, Precision(bounds.Upper * 500)));
			var s = new Random(double.IsNaN(seed) ? Guid.NewGuid().GetHashCode() : (int)Round(seed));
			var randInt = s.Next((int)(lowerInt * precisionValue), (int)(upperInt * precisionValue) + 1);
			var result = randInt / precisionValue;

			return result / 500;
		}
		public static double Average(params double[] numbers)
		{
			return numbers == null ? double.NaN : numbers.Sum() / numbers.Length;
		}
		public static double Precision(double number)
		{
			var cultDecPoint = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
			var split = number.ToString().Split(cultDecPoint);
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
				var prec = Precision(number);
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
		public static double FromDataSize(double number, DataSizeConvertion dataSize)
		{
			return dataSize switch
			{
				DataSizeConvertion.Bit_Byte => number / 8,
				DataSizeConvertion.Bit_KB => number / 8000,
				DataSizeConvertion.Byte_Bit => number * 8,
				DataSizeConvertion.Byte_KB => number / 1024,
				DataSizeConvertion.Byte_MB => number / 1_048_576,
				DataSizeConvertion.KB_Bit => number * 8000,
				DataSizeConvertion.KB_Byte => number * 1024,
				DataSizeConvertion.KB_MB => number / 1024,
				DataSizeConvertion.KB_GB => number / 1_048_576,
				DataSizeConvertion.MB_Byte => number * 1_048_576,
				DataSizeConvertion.MB_KB => number * 1024,
				DataSizeConvertion.MB_GB => number / 1024,
				DataSizeConvertion.MB_TB => number / 1_048_576,
				DataSizeConvertion.GB_KB => number * 1_048_576,
				DataSizeConvertion.GB_MB => number * 1024,
				DataSizeConvertion.GB_TB => number / 1024,
				DataSizeConvertion.TB_MB => number * 1_048_576,
				DataSizeConvertion.TB_GB => number * 1024,
				_ => default,
			};
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
		public static double Move(double number, double speed, Gear.Time.Unit motion = Gear.Time.Unit.Second)
		{
			if (motion == Gear.Time.Unit.Second) speed *= Performance.DeltaTime;
			return number + speed;
		}
		public static double MoveToward(double number, double targetNumber, double speed,
			Gear.Time.Unit motion = Gear.Time.Unit.Second)
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
		public static double AngleBetweenPoints(Point point, Point targetPoint)
		{
			var dir = DirectionBetweenPoints(point, targetPoint);
			return DirectionToAngle(dir);
		}
		public static double MoveTowardAngle(double angle, double targetAngle, double speed,
			Gear.Time.Unit timeUnit = Gear.Time.Unit.Second)
		{
			angle = To360(angle);
			targetAngle = To360(targetAngle);
			speed = Math.Abs(speed);
			var difference = angle - targetAngle;

			// stops the rotation with an else when close enough
			// prevents the rotation from staying behind after the stop
			var checkedSpeed = speed;
			if (timeUnit == Gear.Time.Unit.Second) checkedSpeed *= Performance.DeltaTime;
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
	}
}
