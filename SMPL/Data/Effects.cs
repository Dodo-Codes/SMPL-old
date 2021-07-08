using SFML.Graphics;

namespace SMPL
{
	public class Effects
	{
		internal Shader shader;
		internal Sprite sprite = new();


		public enum Mask
		{
			None, In, Out
		}
		private Mask maskType;
		public Mask MaskType
		{
			get { return maskType; }
			set
			{
				shader.SetUniform("HasMask", value == Mask.In || value == Mask.Out);
				shader.SetUniform("MaskOut", value == Mask.Out);
				maskType = value;
			}
		}
		private Color maskColor;
		public Color MaskColor
		{
			get { return maskColor; }
			set
			{
				maskColor = value;
				shader.SetUniform("MaskRed", (float)value.Red / 255f);
				shader.SetUniform("MaskGreen", (float)value.Green / 255f);
				shader.SetUniform("MaskBlue", (float)value.Blue / 255f);
			}
		}
		public Color TintColor
		{
			get { return Color.To(sprite.Color); }
			set { sprite.Color = Color.From(value); }
		}

		private double progress;
		public double Progress
		{
			get { return progress; }
			set { progress = value; shader.SetUniform("Time", (float)value); }
		}

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
		private Size waterStrength;
		public Size WaterStrength
		{
			get { return waterStrength; }
			set
			{
				waterStrength = value;
				shader.SetUniform("WaterStrengthX", (float)value.Width / 10f);
				shader.SetUniform("WaterStrengthY", (float)value.Height / 10f);
			}
		}
		private Size waterSpeed;
		public Size WaterSpeed
		{
			get { return waterSpeed; }
			set
			{
				waterSpeed = value;
				shader.SetUniform("WaterSpeedX", (float)value.Width / 40f);
				shader.SetUniform("WaterSpeedY", (float)value.Height / 40f);
			}
		}
		private double waterOpacity;
		public double WaterOpacityPercent
		{
			get { return waterOpacity; }
			set { waterOpacity = value; shader.SetUniform("WaterOpacity", (float)value / 100f); }
		}
		private Color edgeColor;
		public Color EdgeColor
		{
			get { return edgeColor; }
			set
			{
				edgeColor = value;
				shader.SetUniform("EdgeRed", (float)value.Red / 255f);
				shader.SetUniform("EdgeGreen", (float)value.Green / 255f);
				shader.SetUniform("EdgeBlue", (float)value.Blue / 255f);
				shader.SetUniform("EdgeOpacity", (float)value.Alpha / 255f);
			}
		}
		private double edgeSensitivity;
		public double EdgeSensitivity
		{
			get { return edgeSensitivity; }
			set { edgeSensitivity = value; shader.SetUniform("EdgeSensitivity", (100f - (float)value) / 160f); }
		}
		private double edgeThickness;
		public double EdgeThickness
		{
			get { return edgeThickness; }
			set { edgeThickness = value; shader.SetUniform("EdgeThickness", 1f - (float)value / 500f); }
		}
		private double pixelateOpacity;
		public double PixelateOpacityPercent
		{
			get { return pixelateOpacity; }
			set { pixelateOpacity = value; shader.SetUniform("PixelateOpacity", (float)value / 100f); }
		}
		private double pixelateStrength;
		public double PixelateStrength
		{
			get { return pixelateStrength; }
			set { pixelateStrength = value; shader.SetUniform("PixelateStrength", (float)value / 100f); }
		}
		private double ignoreDark;
		public double IgnoreDarkPercent
		{
			get { return ignoreDark; }
			set { ignoreDark = value; shader.SetUniform("IgnoreDark", (float)value / 100f); }
		}
		private double ignoreBright;
		public double IgnoreBrightPercent
		{
			get { return ignoreBright; }
			set { ignoreBright = value; shader.SetUniform("IgnoreBright", (float)value / 100f); }
		}
		private Size gridCellSize;
		public Size GridCellSize
		{
			get { return gridCellSize; }
			set
			{
				gridCellSize = value;
				shader.SetUniform("GridCellWidth", (float)value.Width * 2f);
				shader.SetUniform("GridCellHeight", (float)value.Height * 2f);
			}
		}
		private Size gridCellSpacing;
		public Size GridCellSpacing
		{
			get { return gridCellSpacing; }
			set
			{
				gridCellSpacing = value;
				shader.SetUniform("GridCellSpacingX", (float)value.Width / 5f);
				shader.SetUniform("GridCellSpacingY", (float)value.Height / 5f);
			}
		}
		private Color gridColorX;
		public Color GridColorX
		{
			get { return gridColorX; }
			set
			{
				gridColorX = value;
				shader.SetUniform("GridRedX", (float)value.Red / 255f);
				shader.SetUniform("GridGreenX", (float)value.Green / 255f);
				shader.SetUniform("GridBlueX", (float)value.Blue / 255f);
				shader.SetUniform("GridOpacityX", (float)value.Alpha / 255f);
			}
		}
		private Color gridColorY;
		public Color GridColorY
		{
			get { return gridColorY; }
			set
			{
				gridColorY = value;
				shader.SetUniform("GridRedY", (float)value.Red / 255f);
				shader.SetUniform("GridGreenY", (float)value.Green / 255f);
				shader.SetUniform("GridBlueY", (float)value.Blue / 255f);
				shader.SetUniform("GridOpacityY", (float)value.Alpha / 255f);
			}
		}
		private Size windStrength;
		public Size WindStrength
		{
			get { return windStrength; }
			set
			{
				windStrength = value;
				shader.SetUniform("WindStrengthX", (float)value.Width / 5f);
				shader.SetUniform("WindStrengthY", (float)value.Height / 5f);
			}
		}
		private Size windSpeed;
		public Size WindSpeed
		{
			get { return windSpeed; }
			set
			{
				windSpeed = value;
				shader.SetUniform("WindSpeedX", (float)value.Width / 8f);
				shader.SetUniform("WindSpeedY", (float)value.Height / 8f);
			}
		}
		private Size vibrateStrength;
		public Size VibrateStrength
		{
			get { return vibrateStrength; }
			set
			{
				vibrateStrength = value;
				shader.SetUniform("VibrateStrengthX", (float)value.Width / 10f);
				shader.SetUniform("VibrateStrengthY", (float)value.Height / 10f);
			}
		}
		private Size sinStrength;
		public Size WaveStrengthSin
		{
			get { return sinStrength; }
			set
			{
				sinStrength = value;
				shader.SetUniform("SinStrengthX", (float)value.Width);
				shader.SetUniform("SinStrengthY", (float)value.Height);
			}
		}
		private Size cosStrength;
		public Size WaveStrengthCos
		{
			get { return cosStrength; }
			set
			{
				cosStrength = value;
				shader.SetUniform("CosStrengthX", (float)value.Width);
				shader.SetUniform("CosStrengthY", (float)value.Height);
			}
		}
		private Size sinSpeed;
		public Size WaveSpeedSin
		{
			get { return sinSpeed; }
			set
			{
				sinSpeed = value;
				shader.SetUniform("SinSpeedX", (float)value.Width / 10f);
				shader.SetUniform("SinSpeedY", (float)value.Height / 10f);
			}
		}
		private Size cosSpeed;
		public Size WaveSpeedCos
		{
			get { return cosSpeed; }
			set
			{
				cosSpeed = value;
				shader.SetUniform("CosSpeedX", (float)value.Width / 10f);
				shader.SetUniform("CosSpeedY", (float)value.Height / 10f);
			}
		}

		//private Size stretchStrength;
		//public Size StretchStrength
		//{
		//	get { return stretchStrength; }
		//	set
		//	{
		//		stretchStrength = value;
		//		shader.SetUniform("StretchStrengthX", (float)value.Width / 200f);
		//		shader.SetUniform("StretchStrengthY", (float)value.Height / 200f);
		//	}
		//}
		//private Size stretchSpeed;
		//public Size StretchSpeed
		//{
		//	get { return stretchSpeed; }
		//	set
		//	{
		//		stretchSpeed = value;
		//		shader.SetUniform("StretchSpeedX", (float)value.Width * 5f);
		//		shader.SetUniform("StretchSpeedY", (float)value.Height * 5f);
		//	}
		//}
		//private double stretchOpacity;
		//public double StretchOpacityPercent
		//{
		//	get { return stretchOpacity; }
		//	set { stretchOpacity = value; shader.SetUniform("StretchOpacity", (float)value / 100f); }
		//}

		public Effects()
		{
			shader = new("shaders.vert", null, "shaders.frag");

			OutlineOffset = 10;
			BlinkSpeed = 20;
			BlurStrength = new Size(10, 10);
			EarthquakeStrength = new Size(5, 10);
			WaterStrength = new Size(10, 10);
			WaterSpeed = new Size(5, 5);
			EdgeSensitivity = 20;
			EdgeThickness = 20;
			PixelateStrength = 5;
			GridCellSize = new Size(5, 5);
			GridCellSpacing = new Size(25, 25);
			WindSpeed = new Size(10, 10);
			WaveSpeedCos = new Size(0, 30);
			WaveSpeedSin = new Size(0, 30);

			//Wind.Speed.Set(10, 10);
			//Wave.Speed.Set(20, 20, 20, 20);
		}
	}
}
