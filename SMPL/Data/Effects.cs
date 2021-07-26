using SFML.Graphics;
using SFML.System;
using System.Collections.Generic;
using static SMPL.Events;

namespace SMPL
{
	public class Effects
	{
		//https://github.com/anissen/ld34/blob/master/assets/shaders/isolate_bright.glsl

		private readonly uint creationFrame;
		private readonly double rand;
		internal ComponentText textParent;
		internal ComponentSprite spriteParent;
		internal Shader shader;

		private double progress;
		public double Progress
		{
			get { return progress; }
			set { progress = value; shader.SetUniform("Time", (float)value); }
		}

		internal List<ComponentSprite> maskSprites = new();
		internal List<ComponentText> maskTexts = new();
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
				shader.SetUniform("MaskRed", (float)value.R / 255f);
				shader.SetUniform("MaskGreen", (float)value.G / 255f);
				shader.SetUniform("MaskBlue", (float)value.B / 255f);
				shader.SetUniform("MaskOpacity", (float)value.A / 255f);
			}
		}
		private float maskColorBounds, lastFrameMaskColB;
		public float MaskColorBounds
		{
			get { return maskColorBounds; }
			set
			{
				maskColorBounds = value;
				shader.SetUniform("MaskMargin", (float)value / 255f);
			}
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

		private float replaceColorBounds, lastFrameRepColB;
		public float ReplaceColorBounds
		{
			get { return replaceColorBounds; }
			set
			{
				replaceColorBounds = value;
				shader.SetUniform("ReplaceMargin", (float)value / 255f);
			}
		}
		private Color replaceColor, lastFrameRepCol;
		public Color ReplacedColor
		{
			get { return replaceColor; }
			set
			{
				replaceColor = value;
				shader.SetUniform("ReplaceRed", (float)value.R / 255f);
				shader.SetUniform("ReplaceGreen", (float)value.G / 255f);
				shader.SetUniform("ReplaceBlue", (float)value.B / 255f);
				shader.SetUniform("ReplaceOpacity", (float)value.A / 255f);
			}
		}
		private Color replaceWithColor, lastFrameRepWCol;
		public Color ReplaceWithColor
		{
			get { return replaceWithColor; }
			set
			{
				replaceWithColor = value;
				shader.SetUniform("ReplaceWithRed", (float)value.R / 255f);
				shader.SetUniform("ReplaceWithGreen", (float)value.G / 255f);
				shader.SetUniform("ReplaceWithBlue", (float)value.B / 255f);
				shader.SetUniform("ReplaceWithOpacity", (float)value.A / 255f);
			}
		}
		private Color color, lastFrameCol;
		public Color TintColor
		{
			get { return Color.To(spriteParent == null ? textParent.text.FillColor : spriteParent.sprite.Color); }
			set
			{
				if (color == value) return;
				var delta = Color.To(spriteParent == null ? textParent.text.FillColor : spriteParent.sprite.Color);
				color = value;
				var c = Color.From(value);
				if (spriteParent != null)
				{
					spriteParent.sprite.Color = c;
					var n = D(instances); foreach (var kvp in n) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++) e[i].OnSpriteTintRecolorSetup(spriteParent, delta); }
					var n1 = D(instances); foreach (var kvp in n1) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++) e[i].OnSpriteTintRecolor(spriteParent, delta); }
				}
				else
				{
					textParent.text.FillColor = c;
					var n = D(instances); foreach (var kvp in n) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++) e[i].OnTextTintRecolorSetup(textParent, delta); }
					var n1 = D(instances); foreach (var kvp in n1) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++) e[i].OnTextTintRecolor(textParent, delta); }
				}
			}
		}
		private Color outlineColor, lastFrameOutCol;
		public Color OutlineColor
		{
			get { return outlineColor; }
			set
			{
				if (outlineColor == value) return;
				var delta = outlineColor;
				outlineColor = value;
				if (textParent != null)
				{
					textParent.text.OutlineColor = Color.From(value);

					var n = D(instances); foreach (var kvp in n) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++) e[i].OnTextOutlineRecolorSetup(textParent, delta); }
					var n1 = D(instances); foreach (var kvp in n1) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++) e[i].OnTextOutlineRecolor(textParent, delta); }
				}
				else
				{
					shader.SetUniform("OutlineRed", (float)value.R / 255f);
					shader.SetUniform("OutlineGreen", (float)value.G / 255f);
					shader.SetUniform("OutlineBlue", (float)value.B / 255f);
					shader.SetUniform("OutlineOpacity", (float)value.A / 255f);

					var n = D(instances); foreach (var kvp in n) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++) e[i].OnSpriteOutlineRecolorSetup(spriteParent, delta); }
					var n1 = D(instances); foreach (var kvp in n1) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++) e[i].OnSpriteOutlineRecolor(spriteParent, delta); }
				}
			}
		}
		private Color fillColor;
		public Color FillColor
		{
			get { return fillColor; }
			set
			{
				fillColor = value;
				shader.SetUniform("FillRed", (float)value.R / 255f);
				shader.SetUniform("FillGreen", (float)value.G / 255f);
				shader.SetUniform("FillBlue", (float)value.B / 255f);
				shader.SetUniform("FillOpacity", (float)value.A / 255f);
			}
		}
		private double outlineWidth, lastFrameOutW;
		public double OutlineWidth
		{
			get { return outlineWidth; }
			set
			{
				if (value == outlineWidth) return;
				var delta = value - outlineWidth;
				outlineWidth = value;
				if (spriteParent != null)
				{
					shader.SetUniform("OutlineOffset", (float)value / 500f);
					var n = D(instances); foreach (var kvp in n) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++) e[i].OnSpriteOutlineResizeSetup(spriteParent, delta); }
					var n1 = D(instances); foreach (var kvp in n1) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++) e[i].OnSpriteOutlineResize(spriteParent, delta); }
				}
				else
				{
					textParent.text.OutlineThickness = (float)value;
					var n = D(instances); foreach (var kvp in n) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++) e[i].OnTextOutlineResizeSetup(textParent, delta); }
					var n1 = D(instances); foreach (var kvp in n1) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++) e[i].OnTextOutlineResize(textParent, delta); }
				}
			}
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
				shader.SetUniform("BlurStrengthX", (float)value.W / 200f);
				shader.SetUniform("BlurStrengthY", (float)value.H / 200f);
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
				shader.SetUniform("EarthquakeStrengthX", (float)value.W / 300f);
				shader.SetUniform("EarthquakeStrengthY", (float)value.H / 300f);
			}
		}
		private Size waterStrength;
		public Size WaterStrength
		{
			get { return waterStrength; }
			set
			{
				waterStrength = value;
				shader.SetUniform("WaterStrengthX", (float)value.W / 10f);
				shader.SetUniform("WaterStrengthY", (float)value.H / 10f);
			}
		}
		private Size waterSpeed;
		public Size WaterSpeed
		{
			get { return waterSpeed; }
			set
			{
				waterSpeed = value;
				shader.SetUniform("WaterSpeedX", (float)value.W / 40f);
				shader.SetUniform("WaterSpeedY", (float)value.H / 40f);
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
				shader.SetUniform("EdgeRed", (float)value.R / 255f);
				shader.SetUniform("EdgeGreen", (float)value.G / 255f);
				shader.SetUniform("EdgeBlue", (float)value.B / 255f);
				shader.SetUniform("EdgeOpacity", (float)value.A / 255f);
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
				shader.SetUniform("GridCellWidth", (float)value.W * 2f);
				shader.SetUniform("GridCellHeight", (float)value.H * 2f);
			}
		}
		private Size gridCellSpacing;
		public Size GridCellSpacing
		{
			get { return gridCellSpacing; }
			set
			{
				gridCellSpacing = value;
				shader.SetUniform("GridCellSpacingX", (float)value.W / 5f);
				shader.SetUniform("GridCellSpacingY", (float)value.H / 5f);
			}
		}
		private Color gridColorX;
		public Color GridColorX
		{
			get { return gridColorX; }
			set
			{
				gridColorX = value;
				shader.SetUniform("GridRedX", (float)value.R / 255f);
				shader.SetUniform("GridGreenX", (float)value.G / 255f);
				shader.SetUniform("GridBlueX", (float)value.B / 255f);
				shader.SetUniform("GridOpacityX", (float)value.A / 255f);
			}
		}
		private Color gridColorY;
		public Color GridColorY
		{
			get { return gridColorY; }
			set
			{
				gridColorY = value;
				shader.SetUniform("GridRedY", (float)value.R / 255f);
				shader.SetUniform("GridGreenY", (float)value.G / 255f);
				shader.SetUniform("GridBlueY", (float)value.B / 255f);
				shader.SetUniform("GridOpacityY", (float)value.A / 255f);
			}
		}
		private Size windStrength;
		public Size WindStrength
		{
			get { return windStrength; }
			set
			{
				windStrength = value;
				shader.SetUniform("WindStrengthX", (float)value.W / 5f);
				shader.SetUniform("WindStrengthY", (float)value.H / 5f);
			}
		}
		private Size windSpeed;
		public Size WindSpeed
		{
			get { return windSpeed; }
			set
			{
				windSpeed = value;
				shader.SetUniform("WindSpeedX", (float)value.W / 8f);
				shader.SetUniform("WindSpeedY", (float)value.H / 8f);
			}
		}
		private Size vibrateStrength;
		public Size VibrateStrength
		{
			get { return vibrateStrength; }
			set
			{
				vibrateStrength = value;
				shader.SetUniform("VibrateStrengthX", (float)value.W / 10f);
				shader.SetUniform("VibrateStrengthY", (float)value.H / 10f);
			}
		}
		private Size sinStrength;
		public Size WaveStrengthSin
		{
			get { return sinStrength; }
			set
			{
				sinStrength = value;
				shader.SetUniform("SinStrengthX", (float)value.W);
				shader.SetUniform("SinStrengthY", (float)value.H);
			}
		}
		private Size cosStrength;
		public Size WaveStrengthCos
		{
			get { return cosStrength; }
			set
			{
				cosStrength = value;
				shader.SetUniform("CosStrengthX", (float)value.W);
				shader.SetUniform("CosStrengthY", (float)value.H);
			}
		}
		private Size sinSpeed;
		public Size WaveSpeedSin
		{
			get { return sinSpeed; }
			set
			{
				sinSpeed = value;
				shader.SetUniform("SinSpeedX", (float)value.W / 10f);
				shader.SetUniform("SinSpeedY", (float)value.H / 10f);
			}
		}
		private Size cosSpeed;
		public Size WaveSpeedCos
		{
			get { return cosSpeed; }
			set
			{
				cosSpeed = value;
				shader.SetUniform("CosSpeedX", (float)value.W / 10f);
				shader.SetUniform("CosSpeedY", (float)value.H / 10f);
			}
		}

		//private Size stretchStrength;
		//public Size StretchStrength
		//{
		//	get { return stretchStrength; }
		//	set
		//	{
		//		stretchStrength = value;
		//		shader.SetUniform("StretchStrengthX", (float)value.W / 200f);
		//		shader.SetUniform("StretchStrengthY", (float)value.H / 200f);
		//	}
		//}
		//private Size stretchSpeed;
		//public Size StretchSpeed
		//{
		//	get { return stretchSpeed; }
		//	set
		//	{
		//		stretchSpeed = value;
		//		shader.SetUniform("StretchSpeedX", (float)value.W * 5f);
		//		shader.SetUniform("StretchSpeedY", (float)value.H * 5f);
		//	}
		//}
		//private double stretchOpacity;
		//public double StretchOpacityPercent
		//{
		//	get { return stretchOpacity; }
		//	set { stretchOpacity = value; shader.SetUniform("StretchOpacity", (float)value / 100f); }
		//}

		internal void Update()
		{
			if (Gate.EnterOnceWhile($"{creationFrame}-{rand}-col-start", lastFrameCol != color))
			{
				var delta = color - lastFrameCol;
				if (textParent != null)
				{
					var n = D(instances); foreach (var kvp in n) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++) e[i].OnTextTintRecolorStartSetup(textParent, delta); }
					var n1 = D(instances); foreach (var kvp in n1) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++) e[i].OnTextTintRecolorStart(textParent, delta); }
				}
				else
				{
					var n = D(instances); foreach (var kvp in n) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++) e[i].OnSpriteTintRecolorStartSetup(spriteParent, delta); }
					var n1 = D(instances); foreach (var kvp in n1) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++) e[i].OnSpriteTintRecolorStart(spriteParent, delta); }
				}
			}
			if (Gate.EnterOnceWhile($"{creationFrame}-{rand}-col-end", lastFrameCol == color))
			{
				if (creationFrame + 1 != Time.frameCount)
				{
					if (textParent != null)
					{
						var n = D(instances); foreach (var kvp in n) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++) e[i].OnTextTintRecolorEndSetup(textParent); }
						var n1 = D(instances); foreach (var kvp in n1) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++) e[i].OnTextTintRecolorEnd(textParent); }
					}
					else
					{
						var n = D(instances); foreach (var kvp in n) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++) e[i].OnSpriteTintRecolorEndSetup(spriteParent); }
						var n1 = D(instances); foreach (var kvp in n1) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++) e[i].OnSpriteTintRecolorEnd(spriteParent); }
					}
				}
			}
			//=============================
			if (Gate.EnterOnceWhile($"{creationFrame}-{rand}-outw-start", lastFrameOutW != OutlineWidth))
			{
				var delta = OutlineWidth - lastFrameOutW;
				if (textParent != null)
				{
					var n = D(instances); foreach (var kvp in n) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++) e[i].OnTextOutlineResizeStartSetup(textParent, delta); }
					var n1 = D(instances); foreach (var kvp in n1) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++) e[i].OnTextOutlineResizeStart(textParent, delta); }
				}
				else
				{
					var n = D(instances); foreach (var kvp in n) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++) e[i].OnSpriteOutlineResizeStartSetup(spriteParent, delta); }
					var n1 = D(instances); foreach (var kvp in n1) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++) e[i].OnSpriteOutlineResizeStart(spriteParent, delta); }
				}
			}
			if (Gate.EnterOnceWhile($"{creationFrame}-{rand}-outw-end", lastFrameOutW == OutlineWidth))
			{
				if (creationFrame + 1 != Time.frameCount)
				{
					if (textParent != null)
					{
						var n = D(instances); foreach (var kvp in n) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++) e[i].OnTextOutlineResizeEndSetup(textParent); }
						var n1 = D(instances); foreach (var kvp in n1) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++) e[i].OnTextOutlineResizeEnd(textParent); }
					}
					else
					{
						var n = D(instances); foreach (var kvp in n) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++) e[i].OnSpriteOutlineResizeEndSetup(spriteParent); }
						var n1 = D(instances); foreach (var kvp in n1) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++) e[i].OnSpriteOutlineResizeEnd(spriteParent); }
					}
				}
			}
			//=============================
			if (Gate.EnterOnceWhile($"{creationFrame}-{rand}-outcol-start", lastFrameOutCol != outlineColor))
			{
				var delta = outlineColor - lastFrameOutCol;
				if (textParent != null)
				{
					var n = D(instances); foreach (var kvp in n) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++) e[i].OnTextOutlineRecolorStartSetup(textParent, delta); }
					var n1 = D(instances); foreach (var kvp in n1) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++) e[i].OnTextOutlineRecolorStart(textParent, delta); }
				}
				else
				{
					var n = D(instances); foreach (var kvp in n) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++) e[i].OnSpriteOutlineRecolorStartSetup(spriteParent, delta); }
					var n1 = D(instances); foreach (var kvp in n1) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++) e[i].OnSpriteOutlineRecolorStart(spriteParent, delta); }
				}
			}
			if (Gate.EnterOnceWhile($"{creationFrame}-{rand}-outcol-end", lastFrameOutCol == outlineColor))
			{
				if (creationFrame + 1 != Time.frameCount)
				{
					if (textParent != null)
					{
						var n = D(instances); foreach (var kvp in n) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++) e[i].OnTextOutlineRecolorEndSetup(textParent); }
						var n1 = D(instances); foreach (var kvp in n1) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++) e[i].OnTextOutlineRecolorEnd(textParent); }
					}
					else
					{
						var n = D(instances); foreach (var kvp in n) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++) e[i].OnSpriteOutlineRecolorEndSetup(spriteParent); }
						var n1 = D(instances); foreach (var kvp in n1) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++) e[i].OnSpriteOutlineRecolorEnd(spriteParent); }
					}
				}
			}
			//=============================
			lastFrameOutW = OutlineWidth;
			lastFrameCol = color;
			lastFrameOutCol = outlineColor;
		}

		public void MaskAdd(ComponentSprite componentSprite)
		{
			if (AddMask(maskSprites, nameof(ComponentSprite), componentSprite))
			{
				if (spriteParent != null)
				{
					componentSprite.maskingSprite = spriteParent;
					spriteParent.Effects.shader.SetUniform("HasMask", true);
				}
				else
				{
					componentSprite.maskingText = textParent;
					textParent.Effects.shader.SetUniform("HasMask", true);
				}
			}
		}
		public void MaskAdd(ComponentText componentText)
		{
			if (AddMask(maskTexts, nameof(ComponentText), componentText))
			{
				if (spriteParent != null)
				{
					componentText.maskingSprite = spriteParent;
					spriteParent.Effects.shader.SetUniform("HasMask", true);
				}
				else
				{
					componentText.maskingText = textParent;
					textParent.Effects.shader.SetUniform("HasMask", true);
				}
			}
		}
		private static bool AddMask<T>(List<T> list, string name, T component)
		{
			if (component == null)
			{
				Debug.LogError(2, $"The instance of {name} cannot be 'null'.");
				return false;
			}
			if (list.Contains(component))
			{
				Debug.LogError(2, $"This instance of {name} is already added as a mask.");
				return false;
			}
			list.Add(component);
			return true;
		}

		public Effects(ComponentSprite parent)
		{
			creationFrame = Time.FrameCount;
			rand = Number.Random(new Bounds(-9999, 9999), 5);
			spriteParent = parent;
			SetDefaults();
		}
		public Effects(ComponentText parent)
		{
			creationFrame = Time.FrameCount;
			rand = Number.Random(new Bounds(-9999, 9999), 5);
			textParent = parent;
			textParent.text.FillColor = Color.From(Color.White);
			textParent.text.OutlineColor = Color.From(Color.Black);

			SetDefaults();
		}
		private void SetDefaults()
		{
			shader = new("shaders.vert", null, "shaders.frag");
			color = Color.White; lastFrameCol = Color.White;

			outlineColor = Color.Black; lastFrameOutCol = Color.Black;
			shader.SetUniform("OutlineRed", 0f);
			shader.SetUniform("OutlineGreen", 0f);
			shader.SetUniform("OutlineBlue", 0f);
			shader.SetUniform("OutlineOpacity", 1f);

			replaceColorBounds = 1; lastFrameRepColB = 1;
			shader.SetUniform("ReplaceMargin", 0.004f);

			maskColorBounds = 1; lastFrameMaskColB = 1;
			shader.SetUniform("MaskMargin", 0.004f);

			blinkSpeed = 20;
			shader.SetUniform("BlinkSpeed", 20f);

			blurStrength = new Size(10, 10);
			shader.SetUniform("BlurStrengthX", 0.05f);
			shader.SetUniform("BlurStrengthY", 0.05f);

			earthquakeStrength = new Size(5, 10);
			shader.SetUniform("EarthquakeStrengthX", 0.017f);
			shader.SetUniform("EarthquakeStrengthY", 0.034f);

			waterStrength = new Size(10, 10);
			WaterSpeed = new Size(5, 5);
			shader.SetUniform("WaterStrengthX", 1f);
			shader.SetUniform("WaterStrengthY", 1f);
			shader.SetUniform("WaterSpeedX", 0.125f);
			shader.SetUniform("WaterSpeedY", 0.125f);

			edgeSensitivity = 20;
			edgeThickness = 20;
			shader.SetUniform("EdgeSensitivity", 0.5f);
			shader.SetUniform("EdgeThickness", 0.096f);

			pixelateStrength = 5;
			shader.SetUniform("PixelateStrength", 0.05f);

			gridCellSize = new Size(5, 5);
			gridCellSpacing = new Size(25, 25);
			shader.SetUniform("GridCellWidth", 10f);
			shader.SetUniform("GridCellHeight", 10f);
			shader.SetUniform("GridCellSpacingX", 5f);
			shader.SetUniform("GridCellSpacingY", 5f);

			sinSpeed = new Size(0, 30);
			cosSpeed = new Size(0, 30);
			shader.SetUniform("SinSpeedX", 3f);
			shader.SetUniform("SinSpeedY", 3f);
			shader.SetUniform("CosSpeedX", 3f);
			shader.SetUniform("CosSpeedY", 3f);

			windSpeed = new Size(10, 10);
			shader.SetUniform("WindSpeedX", 1.25f);
			shader.SetUniform("WindSpeedY", 1.25f);
		}

		internal RenderTexture DrawMasks(Sprite spr)
		{
			var transform = textParent != null ? textParent.transform : spriteParent.transform;
			var sz = new Vector2u((uint)transform.Size.W, (uint)transform.Size.H);
			if (sz.X < spr.Texture.Size.X) sz.X = spr.Texture.Size.X;
			if (sz.Y < spr.Texture.Size.Y) sz.Y = spr.Texture.Size.Y;
			var rend = new RenderTexture(sz.X, sz.Y);
			var sc = new Vector2f((float)transform.Size.W / spr.Texture.Size.X, (float)transform.Size.H / spr.Texture.Size.Y);

			rend.Draw(spr);
			for (int i = 0; i < maskTexts.Count; i++)
			{
				var pos = Point.From(maskTexts[i].transform.Position) + maskTexts[i].GetOffset();
				maskTexts[i].text.Position = new Vector2f(pos.X, pos.Y);
				maskTexts[i].text.Rotation = (float)maskTexts[i].transform.Angle;
				maskTexts[i].text.Origin = new Vector2f(
					(float)(maskTexts[i].transform.Size.W * (maskTexts[i].transform.OriginPercent.X / 100)),
					(float)(maskTexts[i].transform.Size.H * (maskTexts[i].transform.OriginPercent.Y / 100)));
				rend.Draw(maskTexts[i].text);
			}
			for (int i = 0; i < maskSprites.Count; i++)
			{
				var pos = Point.From(maskSprites[i].transform.Position);
				var w = spr.TextureRect.Width;
				var h = spr.TextureRect.Height;
				var p = transform.OriginPercent / 100;
				var x = w * (float)p.X * ((float)maskSprites[i].GridSize.W / 2f) + (w * (float)p.X / 2f);
				var y = h * (float)p.Y * ((float)maskSprites[i].GridSize.H / 2f) + (h * (float)p.Y / 2f);

				maskSprites[i].sprite.Origin = new Vector2f(x, y);
				maskSprites[i].sprite.Position = new Vector2f(pos.X / sc.X, pos.Y / sc.Y);
				maskSprites[i].sprite.Rotation = (float)maskSprites[i].transform.Angle;
				maskSprites[i].sprite.Scale = new Vector2f(
					(float)maskSprites[i].transform.Size.W / maskSprites[i].rawTexture.Size.X,
					(float)maskSprites[i].transform.Size.H / maskSprites[i].rawTexture.Size.Y);

				rend.Draw(maskSprites[i].sprite, new RenderStates(maskSprites[i].Effects.shader));
			}
			rend.Display();
			return rend;
		}
	}
}
