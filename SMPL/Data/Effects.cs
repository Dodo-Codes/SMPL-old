using SFML.Graphics;

namespace SMPL
{
	public class Effects
	{
		internal Shader shader;

		private double gamma;
		public double GammaPercent
		{
			get { return gamma; }
			set { gamma = value; shader.SetUniform("Gamma", (float)value / 100f); }
		}
		private double desaturation;
		public double DesaturationPercent
		{
			get { return desaturation; }
			set { desaturation = value; shader.SetUniform("Desaturation", (float)value / 100f); }
		}
		private double inversion;
		public double InversionPercent
		{
			get { return inversion; }
			set { inversion = value; shader.SetUniform("Inversion", (float)value / 100f); }
		}
		private double contrast;
		public double ContrastPercent
		{
			get { return contrast; }
			set { contrast = value; shader.SetUniform("Contrast", (float)value / 100f); }
		}
		private double brightness;
		public double BrightnessPercent
		{
			get { return brightness; }
			set { brightness = value; shader.SetUniform("Brightness", (float)value / 100f); }
		}

		private Color fillColor;
		public Color FillColor
		{
			get { return fillColor; }
			set
			{
				fillColor = value;
				shader.SetUniform("FillRed", (float)value.Red / 255f);
				shader.SetUniform("FillGreen", (float)value.Green / 255f);
				shader.SetUniform("FillBlue", (float)value.Blue / 255f);
				shader.SetUniform("FillOpacity", (float)value.Alpha / 255f);
			}
		}
		private Color outlineColor;
		public Color OutlineColor
		{
			get { return outlineColor; }
			set
			{
				outlineColor = value;
				shader.SetUniform("OutlineRed", (float)value.Red / 255f);
				shader.SetUniform("OutlineGreen", (float)value.Green / 255f);
				shader.SetUniform("OutlineBlue", (float)value.Blue / 255f);
				shader.SetUniform("OutlineOpacity", (float)value.Alpha / 255f);
			}
		}
		private double outlineOffset;
		public double OutlineOffset
		{
			get { return outlineOffset; }
			set { outlineOffset = value; shader.SetUniform("OutlineOffset", (float)value / 500f); }
		}
		private double blinkSpeed;
		public double BlinkSpeed
		{
			get { return blinkSpeed; }
			set { blinkSpeed = value; shader.SetUniform("BlinkSpeed", (float)value); }
		}
		private double blinkOpacity;
		public double BlinkOpacityPercent
		{
			get { return blinkOpacity; }
			set { blinkOpacity = value; shader.SetUniform("BlinkOpacity", (float)value / 100f); }
		}
		private double blurOpacity;
		public double BlurOpacityPercent
		{
			get { return blurOpacity; }
			set { blurOpacity = value; shader.SetUniform("BlurOpacity", (float)value / 100f); }
		}
		private Size blurStrength;
		public Size BlurStrength
		{
			get { return blurStrength; }
			set
			{
				blurStrength = value;
				shader.SetUniform("BlurStrengthX", (float)value.Width / 200f);
				shader.SetUniform("BlurStrengthY", (float)value.Height / 200f);
			}
		}
		private double earthquakeOpacity;
		public double EarthquakeOpacityPercent
		{
			get { return earthquakeOpacity; }
			set { earthquakeOpacity = value; shader.SetUniform("EarthquakeOpacity", (float)value / 100f); }
		}
		private Size earthquakeStrength;
		public Size EarthquakeStrength
		{
			get { return earthquakeStrength; }
			set
			{
				earthquakeStrength = value;
				shader.SetUniform("EarthquakeStrengthX", (float)value.Width / 300f);
				shader.SetUniform("EarthquakeStrengthY", (float)value.Height / 300f);
			}
		}

		public Effects()
		{
			shader = new("shaders.vert", null, "shaders.frag");

			OutlineOffset = 10;
			BlinkSpeed = 10;
			BlurStrength = new Size(10, 10);
			EarthquakeStrength = new Size(5, 10);
		}
	}
}
