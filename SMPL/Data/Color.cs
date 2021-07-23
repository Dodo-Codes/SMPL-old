using System;

namespace SMPL
{
	public struct Color
	{
		public double R, G, B, A;

		public static Color White				{ get { return new Color(255, 255,	255,	255); } }
		public static Color Black				{ get { return new Color(0,	0,		0,		255); } }
		public static Color GrayLight			{ get { return new Color(175, 175,	175,	255); } }
		public static Color Gray				{ get { return new Color(125, 125,	125,	255); } }
		public static Color GrayDark			{ get { return new Color(75,	75,	75,	255); } }
		public static Color RedLight			{ get { return new Color(255, 125,	125,	255); } }
		public static Color Red					{ get { return new Color(255, 0,		0,		255); } }
		public static Color RedDark			{ get { return new Color(125, 0,		0,		255); } }
		public static Color GreenLight		{ get { return new Color(125, 255,	125,	255); } }
		public static Color Green				{ get { return new Color(0,	255,	0,		255); } }
		public static Color GreenDark			{ get { return new Color(0,	125,	0,		255); } }
		public static Color BlueLight			{ get { return new Color(125, 125,	255,	255); } }
		public static Color Blue				{ get { return new Color(0,	0,		255,	255); } }
		public static Color BlueDark			{ get { return new Color(0,	0,		125,	255); } }
		public static Color YellowLight		{ get { return new Color(255, 255,	125,	255); } }
		public static Color Yellow				{ get { return new Color(255, 255,	0,		255); } }
		public static Color YellowDark		{ get { return new Color(125, 125,	0,		255); } }
		public static Color MagentaLight		{ get { return new Color(255, 125,	255,	255); } }
		public static Color Magenta			{ get { return new Color(255, 0,		255,	255); } }
		public static Color MagentaDark		{ get { return new Color(125, 0,		125,	255); } }
		public static Color CyanLight			{ get { return new Color(125, 255,	255,	255); } }
		public static Color Cyan				{ get { return new Color(0,	255,	255,	255); } }
		public static Color CyanDark			{ get { return new Color(0,	125,	125,	255); } }

		public Color(double red, double green, double blue, double alpha = 255)
		{
			R = red;
			G = green;
			B = blue;
			A = alpha;
			To255();
		}
		public static Color Lighten(Color color, double speed, Time.Unit timeUnit = Time.Unit.Second)
		{
			color = TintRed(color, speed, timeUnit);
			color = TintGreen(color, speed, timeUnit);
			color = TintBlue(color, speed, timeUnit);
			color.To255();
			return color;
		}
		public static Color TintRed(Color color, double speed, Time.Unit timeUnit = Time.Unit.Second)
		{
			color.R = Number.Move(color.R, speed, timeUnit);
			color.To255();
			return color;
		}
		public static Color TintGreen(Color color, double speed, Time.Unit timeUnit = Time.Unit.Second)
		{
			color.G = Number.Move(color.G, speed, timeUnit);
			color.To255();
			return color;
		}
		public static Color TintBlue(Color color, double speed, Time.Unit timeUnit = Time.Unit.Second)
		{
			color.B = Number.Move(color.B, speed, timeUnit);
			color.To255();
			return color;
		}
		public static Color Appear(Color color, double speed, Time.Unit timeUnit = Time.Unit.Second)
		{
			color.A = Number.Move(color.A, speed, timeUnit);
			color.To255();
			return color;
		}
		public static Color TintTowardRed(Color color, double targetRed, double speed, Time.Unit timeUnit = Time.Unit.Second)
		{
			color = TintRed(color, color.R < targetRed ? speed : -speed, timeUnit);
			var dist = Math.Abs(color.R - targetRed);
			var sp = timeUnit == Time.Unit.Second ? speed * Time.DeltaTime * 2 : speed * 2;
			if (dist < sp) color.R = targetRed;
			return color;
		}
		public static Color TintTowardGreen(Color color, double targetGreen, double speed, Time.Unit timeUnit = Time.Unit.Second)
		{
			color = TintGreen(color, color.G < targetGreen ? speed : -speed, timeUnit);
			var dist = Math.Abs(color.G - targetGreen);
			var sp = timeUnit == Time.Unit.Second ? speed * Time.DeltaTime * 2 : speed * 2;
			if (dist < sp) color.G = targetGreen;
			return color;
		}
		public static Color TintTowardBlue(Color color, double targetBlue, double speed, Time.Unit timeUnit = Time.Unit.Second)
		{
			color = TintBlue(color, color.B < targetBlue ? speed : -speed, timeUnit);
			var dist = Math.Abs(color.B - targetBlue);
			var sp = timeUnit == Time.Unit.Second ? speed * Time.DeltaTime * 2 : speed * 2;
			if (dist < sp) color.B = targetBlue;
			return color;
		}
		public static Color AppearTowardAlpha(Color color, double targeto, double speed, Time.Unit timeUnit = Time.Unit.Second)
		{
			Appear(color, color.A < targeto ? speed : -speed, timeUnit);
			var dist = Math.Abs(color.A - targeto);
			if (dist < speed * Time.DeltaTime * 2) color.A = targeto;
			return color;
		}
		public static Color TintTowardColor(Color color, Color targetColor, double speed, Time.Unit timeUnit = Time.Unit.Second)
		{
			color = TintRed(color, targetColor.R > color.R ? speed : -speed, timeUnit);
			color = TintGreen(color, targetColor.G > color.G ? speed : -speed, timeUnit);
			color = TintBlue(color, targetColor.B > color.B ? speed : -speed, timeUnit);

			var rDist = Math.Abs(targetColor.R - color.R);
			var gDist = Math.Abs(targetColor.G - color.G);
			var bDist = Math.Abs(targetColor.B - color.B);

			speed *= Time.DeltaTime;
			if (rDist < speed * 2) color.R = targetColor.R;
			if (gDist < speed * 2) color.G = targetColor.G;
			if (bDist < speed * 2) color.B = targetColor.B;

			color.To255();
			return color;
		}

		internal void To255()
		{
			R = Number.Limit(R, new Bounds(0, 255));
			G = Number.Limit(G, new Bounds(0, 255));
			B = Number.Limit(B, new Bounds(0, 255));
			A = Number.Limit(A, new Bounds(0, 255));
		}
		internal static Color To(SFML.Graphics.Color col) => new Color(col.R, col.G, col.B, col.A);
		internal static SFML.Graphics.Color From(Color col)
		{
			return new SFML.Graphics.Color((byte)col.R, (byte)col.G, (byte)col.B, (byte)col.A);
		}

		public static bool operator ==(Color a, Color b) => a.R == b.R && a.G == b.G && a.B == b.B && a.A == b.A;
		public static bool operator !=(Color a, Color b) => a.R != b.R || a.G != b.G || a.B != b.B || a.A != b.A;
		public static Color operator -(Color a, Color b) => new Color(a.R - b.R, a.G - b.G, a.B - b.B, a.A - b.A);
		public static Color operator +(Color a, Color b) => new Color(a.R + b.R, a.G + b.G, a.B + b.B, a.A + b.A);

		public override string ToString() => $"{R} {G} {B} {A}";
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
