using System;
using SMPL.Gear;

namespace SMPL.Data
{
	public struct Color
	{
		public double R { get; set; }
		public double G { get; set; }
		public double B { get; set; }
		public double A { get; set; }

		public static Color Invalid		=> new(double.NaN, double.NaN, double.NaN, double.NaN);
		public static Color Transparent	=> new(0,	0,		0,		0);
		public static Color White			=> new(255, 255,	255);
		public static Color Black			=> new(0,	0,		0);
		public static Color Gray			=> new(125, 125,	125);
		public static Color Red				=> new(255, 0,		0);
		public static Color Green			=> new(0,	255,	0);
		public static Color Blue			=> new(0,	0,		255);
		public static Color Yellow			=> new(255, 255,	0);
		public static Color Magenta		=> new(255, 0,		255);
		public static Color Cyan			=> new(0,	255,	255);
		public static Color Pink			=> new(255, 175,	175);
		public static Color Orange			=> new(255, 125,	75);
		public static Color Brown			=> new(125, 75,	25);

		public static Color Water			=> new(0,	100,	150);
		public static Color Sky				=> new(135, 206,	235);
		public static Color Wood			=> new(186, 140,	100);
		public static Color Grass			=> new(52,	140,	50);
		public static Color LeafSummer	=> new(45,	90,	40);
		public static Color LeafAutumn	=> new(152,	72,	43);

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
			var sp = timeUnit == Time.Unit.Second ? speed * Performance.DeltaTime * 2 : speed * 2;
			if (dist < sp) color.R = targetRed;
			return color;
		}
		public static Color TintTowardGreen(Color color, double targetGreen, double speed, Time.Unit timeUnit = Time.Unit.Second)
		{
			color = TintGreen(color, color.G < targetGreen ? speed : -speed, timeUnit);
			var dist = Math.Abs(color.G - targetGreen);
			var sp = timeUnit == Time.Unit.Second ? speed * Performance.DeltaTime * 2 : speed * 2;
			if (dist < sp) color.G = targetGreen;
			return color;
		}
		public static Color TintTowardBlue(Color color, double targetBlue, double speed, Time.Unit timeUnit = Time.Unit.Second)
		{
			color = TintBlue(color, color.B < targetBlue ? speed : -speed, timeUnit);
			var dist = Math.Abs(color.B - targetBlue);
			var sp = timeUnit == Time.Unit.Second ? speed * Performance.DeltaTime * 2 : speed * 2;
			if (dist < sp) color.B = targetBlue;
			return color;
		}
		public static Color AppearTowardAlpha(Color color, double targeto, double speed, Time.Unit timeUnit = Time.Unit.Second)
		{
			Appear(color, color.A < targeto ? speed : -speed, timeUnit);
			var dist = Math.Abs(color.A - targeto);
			if (dist < speed * Performance.DeltaTime * 2) color.A = targeto;
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

			if (timeUnit == Time.Unit.Second) speed *= Performance.DeltaTime;
			if (rDist < speed * 2) color.R = targetColor.R;
			if (gDist < speed * 2) color.G = targetColor.G;
			if (bDist < speed * 2) color.B = targetColor.B;

			color.To255();
			return color;
		}

		public bool IsInvalid() => double.IsNaN(R) || double.IsNaN(G) || double.IsNaN(B) || double.IsNaN(A);

		internal void To255()
		{
			R = Number.Limit(R, new Number.Range(0, 255));
			G = Number.Limit(G, new Number.Range(0, 255));
			B = Number.Limit(B, new Number.Range(0, 255));
			A = Number.Limit(A, new Number.Range(0, 255));
		}
		internal static Color To(SFML.Graphics.Color col) => new(col.R, col.G, col.B, col.A);
		internal static SFML.Graphics.Color From(Color col)
		{
			return new SFML.Graphics.Color((byte)col.R, (byte)col.G, (byte)col.B, (byte)col.A);
		}

		public static bool operator ==(Color a, Color b) => a.R == b.R && a.G == b.G && a.B == b.B && a.A == b.A;
		public static bool operator !=(Color a, Color b) => a.R != b.R || a.G != b.G || a.B != b.B || a.A != b.A;
		public static Color operator -(Color a, Color b) => new(a.R - b.R, a.G - b.G, a.B - b.B, a.A - b.A);
		public static Color operator +(Color a, Color b) => new(a.R + b.R, a.G + b.G, a.B + b.B, a.A + b.A);

		public static Color operator -(Color a, double b) => new(a.R - b, a.G - b, a.B - b, a.A - b);
		public static Color operator +(Color a, double b) => new(a.R + b, a.G + b, a.B + b, a.A + b);
		public static Color operator /(Color a, double b) => new(a.R / b, a.G / b, a.B / b, a.A / b);
		public static Color operator *(Color a, double b) => new(a.R * b, a.G * b, a.B * b, a.A * b);

		public override string ToString() => $"{R} {G} {B} {A}";
		public override bool Equals(object obj) => base.Equals(obj);
		public override int GetHashCode() => base.GetHashCode();
	}
}
