using System;

namespace SMPL
{
	public struct Color
	{
		public double Red, Green, Blue, Alpha;

		public static Color White				{ get { return new Color(255, 255,	255,	255); } }
		public static Color Black				{ get { return new Color(0,	0,		0,		255); } }
		public static Color LightGray			{ get { return new Color(175, 175,	175,	255); } }
		public static Color NormalGray		{ get { return new Color(125, 125,	125,	255); } }
		public static Color DarkGray			{ get { return new Color(75,	75,	75,	255); } }
		public static Color LightRed			{ get { return new Color(255, 125,	125,	255); } }
		public static Color NormalRed			{ get { return new Color(255, 0,		0,		255); } }
		public static Color DarkRed			{ get { return new Color(125, 0,		0,		255); } }
		public static Color LightGreen		{ get { return new Color(125, 255,	125,	255); } }
		public static Color NormalGreen		{ get { return new Color(0,	255,	0,		255); } }
		public static Color DarkGreen			{ get { return new Color(0,	125,	0,		255); } }
		public static Color LightBlue			{ get { return new Color(125, 125,	255,	255); } }
		public static Color NormalBlue		{ get { return new Color(0,	0,		255,	255); } }
		public static Color DarkBlue			{ get { return new Color(0,	0,		125,	255); } }
		public static Color LightYellow		{ get { return new Color(255, 255,	125,	255); } }
		public static Color NormalYellow		{ get { return new Color(255, 255,	0,		255); } }
		public static Color DarkYellow		{ get { return new Color(125, 125,	0,		255); } }
		public static Color LightMagenta		{ get { return new Color(255, 125,	255,	255); } }
		public static Color NormalMagenta	{ get { return new Color(255, 0,		255,	255); } }
		public static Color DarkMagenta		{ get { return new Color(125, 0,		125,	255); } }
		public static Color LightCyan			{ get { return new Color(125, 255,	255,	255); } }
		public static Color NormalCyan		{ get { return new Color(0,	255,	255,	255); } }
		public static Color DarkCyan			{ get { return new Color(0,	125,	125,	255); } }

		public Color(double red, double green, double blue, double alpha = 255)
		{
			Red = red;
			Green = green;
			Blue = blue;
			Alpha = alpha;
			To255();
		}
		public static Color Lighten(Color color, double speed, Time.Unit timeUnit)
		{
			color = TintRed(color, speed, timeUnit);
			color = TintGreen(color, speed, timeUnit);
			color = TintBlue(color, speed, timeUnit);
			color.To255();
			return color;
		}
		public static Color TintRed(Color color, double speed, Time.Unit timeUnit)
		{
			color.Red = Number.Move(color.Red, speed, timeUnit);
			color.To255();
			return color;
		}
		public static Color TintGreen(Color color, double speed, Time.Unit timeUnit)
		{
			color.Green = Number.Move(color.Green, speed, timeUnit);
			color.To255();
			return color;
		}
		public static Color TintBlue(Color color, double speed, Time.Unit timeUnit)
		{
			color.Blue = Number.Move(color.Blue, speed, timeUnit);
			color.To255();
			return color;
		}
		public static Color Appear(Color color, double speed, Time.Unit timeUnit)
		{
			color.Alpha = Number.Move(color.Alpha, speed, timeUnit);
			color.To255();
			return color;
		}
		public static Color TintTowardRed(Color color, double targetRed, double speed, Time.Unit timeUnit)
		{
			color = TintRed(color, color.Red < targetRed ? speed : -speed, timeUnit);
			var dist = Math.Abs(color.Red - targetRed);
			var sp = timeUnit == Time.Unit.Second ? speed * Time.DeltaTime * 2 : speed * 2;
			if (dist < sp) color.Red = targetRed;
			return color;
		}
		public static Color TintTowardGreen(Color color, double targetGreen, double speed, Time.Unit timeUnit)
		{
			color = TintGreen(color, color.Green < targetGreen ? speed : -speed, timeUnit);
			var dist = Math.Abs(color.Green - targetGreen);
			var sp = timeUnit == Time.Unit.Second ? speed * Time.DeltaTime * 2 : speed * 2;
			if (dist < sp) color.Green = targetGreen;
			return color;
		}
		public static Color TintTowardBlue(Color color, double targetBlue, double speed, Time.Unit timeUnit)
		{
			color = TintBlue(color, color.Blue < targetBlue ? speed : -speed, timeUnit);
			var dist = Math.Abs(color.Blue - targetBlue);
			var sp = timeUnit == Time.Unit.Second ? speed * Time.DeltaTime * 2 : speed * 2;
			if (dist < sp) color.Blue = targetBlue;
			return color;
		}
		public static Color AppearTowardAlpha(Color color, double targeto, double speed, Time.Unit timeUnit)
		{
			Appear(color, color.Alpha < targeto ? speed : -speed, timeUnit);
			var dist = Math.Abs(color.Alpha - targeto);
			if (dist < speed * Time.DeltaTime * 2) color.Alpha = targeto;
			return color;
		}
		public static Color TintTowardColor(Color color, Color targetColor, double speed, Time.Unit timeUnit)
		{
			color = TintRed(color, targetColor.Red > color.Red ? speed : -speed, timeUnit);
			color = TintGreen(color, targetColor.Green > color.Green ? speed : -speed, timeUnit);
			color = TintBlue(color, targetColor.Blue > color.Blue ? speed : -speed, timeUnit);

			var rDist = Math.Abs(targetColor.Red - color.Red);
			var gDist = Math.Abs(targetColor.Green - color.Green);
			var bDist = Math.Abs(targetColor.Blue - color.Blue);

			speed *= Time.DeltaTime;
			if (rDist < speed * 2) color.Red = targetColor.Red;
			if (gDist < speed * 2) color.Green = targetColor.Green;
			if (bDist < speed * 2) color.Blue = targetColor.Blue;

			color.To255();
			return color;
		}

		internal void To255()
		{
			Red = Number.Limit(Red, new Bounds(0, 255));
			Green = Number.Limit(Green, new Bounds(0, 255));
			Blue = Number.Limit(Blue, new Bounds(0, 255));
			Alpha = Number.Limit(Alpha, new Bounds(0, 255));
		}

		public static bool operator ==(Color a, Color b)
		{
			return a.Red == b.Red && a.Green == b.Green && a.Blue == b.Blue && a.Alpha == b.Alpha;
		}
		public static bool operator !=(Color a, Color b)
		{
			return a.Red != b.Red || a.Green != b.Green || a.Blue != b.Blue || a.Alpha != b.Alpha;
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
