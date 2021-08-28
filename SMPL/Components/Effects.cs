using SFML.System;
using System;
using System.Collections.Generic;
using SMPL.Data;
using SMPL.Gear;

namespace SMPL.Components
{
	public class Effects : Access
	{
		private readonly uint creationFrame;
		private readonly double rand;
		internal Visual owner;
		internal SFML.Graphics.Shader shader;

		private static event Events.ParamsTwo<Effects, Color> OnBackgroundColorChange, OnBackgroundColorChangeStart;
		private static event Events.ParamsOne<Effects> OnBackgroundColorChangeEnd;
		private static event Events.ParamsTwo<Effects, Identity<Effects>> OnIdentityChange;

		public static class CallWhen
		{
			public static void IdentityChange(Action<Effects, Identity<Effects>> method,
				uint order = uint.MaxValue) => OnIdentityChange = Events.Add(OnIdentityChange, method, order);
			public static void BackgroundColorChangeStart(Action<Effects, Color> method, uint order = uint.MaxValue) =>
				OnBackgroundColorChangeStart = Events.Add(OnBackgroundColorChangeStart, method, order);
			public static void BackgroundColorChange(Action<Effects, Color> method, uint order = uint.MaxValue) =>
				OnBackgroundColorChange = Events.Add(OnBackgroundColorChange, method, order);
			public static void BackgroundColorChangeEnd(Action<Effects> method, uint order = uint.MaxValue) =>
				OnBackgroundColorChangeEnd = Events.Add(OnBackgroundColorChangeEnd, method, order);
		}

		private Identity<Effects> identity;
		public Identity<Effects> Identity
		{
			get { return AllAccess == Extent.Removed ? default : identity; }
			set
			{
				if (identity == value || (Debug.CalledBySMPL == false && IsCurrentlyAccessible() == false)) return;
				var prev = identity;
				identity = value;
				if (Debug.CalledBySMPL == false) OnIdentityChange?.Invoke(this, prev);
			}
		}

		private double progress;
		public double Progress
		{
			get { return progress; }
			set { progress = value; shader.SetUniform("Time", (float)value); }
		}

		internal List<Visual> masks = new();
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

		private double replaceColorBounds, lastFrameRepColB;
		public double ReplacedColorBounds
		{
			get { return replaceColorBounds; }
			set
			{
				replaceColorBounds = value;
				shader.SetUniform("ReplaceMargin", (float)value / 255f);
			}
		}
		private Color bgColor, lastFrameBgCol;
		public Color BackgroundColor
		{
			get { return AllAccess == Extent.Removed ? Color.Invalid : bgColor; }
			set
			{
				if (bgColor == value || (Debug.CalledBySMPL == false && IsCurrentlyAccessible() == false)) return;
				var prev = bgColor;
				bgColor = value;
				//if (Debug.CalledBySMPL == false) OnBackgroundColorChange?.Invoke(this, prev);
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
			get { return Color.To(owner is TextBox ?
				(owner as TextBox).Area.text.FillColor : (owner as Sprite).Area.sprite.Color); }
			set
			{
				if (color == value) return;
				var prev = color;
				color = value;
			}
		}
		private Color outlineColor, lastFrameOutCol;
		public Color OutlineColor
		{
			get { return outlineColor; }
			set
			{
				if (outlineColor == value) return;
				var prev = outlineColor;
				outlineColor = value;
				if (owner is TextBox) return;
				shader.SetUniform("OutlineRed", (float)value.R / 255f);
				shader.SetUniform("OutlineGreen", (float)value.G / 255f);
				shader.SetUniform("OutlineBlue", (float)value.B / 255f);
				shader.SetUniform("OutlineOpacity", (float)value.A / 255f);
			}
		}
		private Color fillColor;
		public Color FillColor
		{
			get { return fillColor; }
			set
			{
				if (owner is Sprite && creationFrame == Performance.frameCount) return;
				fillColor = value;
				if (owner is TextBox) return;
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
				if (owner is TextBox) return;
				shader.SetUniform("OutlineOffset", (float)value / 500f);
			}
		}
		private double blinkSpeed;
		public double ProgressiveBlinkSpeed
		{
			get { return blinkSpeed; }
			set { blinkSpeed = value; shader.SetUniform("BlinkSpeed", (float)value); }
		}
		private double blinkOpacity;
		public double ProgressiveBlinkOpacityPercent
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
		public double ProgressiveEarthquakeOpacityPercent
		{
			get { return earthquakeOpacity; }
			set { earthquakeOpacity = value; shader.SetUniform("EarthquakeOpacity", (float)value / 100f); }
		}
		private Size earthquakeStrength;
		public Size ProgressiveEarthquakeStrength
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
		public Size ProgressiveWaterStrength
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
		public Size ProgressiveWaterSpeed
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
		public double ProgressiveWaterOpacityPercent
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
		public Size ProgressiveWindStrength
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
		public Size ProgressiveWindSpeed
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
		public Size ProgressiveVibrateStrength
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
		public Size ProgressiveWaveStrengthSin
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
		public Size ProgressiveWaveStrengthCos
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
		public Size ProgressiveWaveSpeedSin
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
		public Size ProgressiveWaveSpeedCos
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
			var textParent = owner is TextBox ? owner as TextBox : null;
			var spriteParent = owner is Sprite ? owner as Sprite : null;

			if (Gate.EnterOnceWhile($"{creationFrame}-{rand}-col-start", lastFrameCol != color))
			{
				var delta = color - lastFrameCol;
				if (textParent != null)
				{

				}
				else
				{

				}
			}
			if (Gate.EnterOnceWhile($"{creationFrame}-{rand}-col-end", lastFrameCol == color))
			{
				if (creationFrame + 1 != Performance.frameCount)
				{
					if (textParent != null)
					{

					}
					else
					{

					}
				}
			}
			//=============================
			if (Gate.EnterOnceWhile($"{creationFrame}-{rand}-outw-start", lastFrameOutW != OutlineWidth))
			{
				var delta = OutlineWidth - lastFrameOutW;
				if (textParent != null)
				{

				}
				else
				{

				}
			}
			if (Gate.EnterOnceWhile($"{creationFrame}-{rand}-outw-end", lastFrameOutW == OutlineWidth))
			{
				if (creationFrame + 1 != Performance.frameCount)
				{
					if (textParent != null)
					{

					}
					else
					{

					}
				}
			}
			//=============================
			if (Gate.EnterOnceWhile($"{creationFrame}-{rand}-outcol-start", lastFrameOutCol != outlineColor))
			{
				var delta = outlineColor - lastFrameOutCol;
				if (textParent != null)
				{

				}
				else
				{

				}
			}
			if (Gate.EnterOnceWhile($"{creationFrame}-{rand}-outcol-end", lastFrameOutCol == outlineColor))
			{
				if (creationFrame + 1 != Performance.frameCount)
				{
					if (textParent != null)
					{

					}
					else
					{

					}
				}
			}
			//=============================
			if (Gate.EnterOnceWhile($"{creationFrame}-{rand}-text-bgCol-start", lastFrameBgCol != bgColor))
				//OnBackgroundColorChangeStart?.Invoke(this, lastFrameBgCol);
			if (Gate.EnterOnceWhile($"{creationFrame}-{rand}-text-bgCol-end", lastFrameBgCol == bgColor))
				//if (creationFrame + 1 != Performance.frameCount) OnBackgroundColorChangeEnd?.Invoke(this);
			//=============================
			lastFrameOutW = OutlineWidth;
			lastFrameCol = color;
			lastFrameOutCol = outlineColor;
			lastFrameMaskColB = maskColorBounds;
			lastFrameRepColB = replaceColorBounds;
			lastFrameRepCol = replaceColor;
			lastFrameRepWCol = replaceWithColor;
			lastFrameBgCol = bgColor;
		}

		public void AddMasks(params Visual[] masks)
		{
			for (int i = 0; i < masks.Length; i++) AddMask(this.masks, masks[i], owner, true);
		}
		public void RemoveMasks(params Visual[] masks)
		{
			for (int i = 0; i < masks.Length; i++) AddMask(this.masks, masks[i], owner, false);
		}
		public Visual[] Masks => masks.ToArray();
		public Visual MaskTarget => owner.masking;
		private static void AddMask(List<Visual> list,
			Visual component, Visual owner, bool add)
		{
			if (add)
			{
				if (component == null)
				{
					Debug.LogError(2, $"The mask cannot be 'null'.");
					return;
				}
				if (list.Contains(component))
				{
					Debug.LogError(2, $"The instance of this mask is already added to this target.");
					return;
				}
				list.Add(component);
			}
			else
			{
				if (list.Contains(component) == false)
				{
					Debug.LogError(2, $"This instance is not a mask to this target.");
					return;
				}
				list.Remove(component);
				if (component is TextBox)
				{
					var t = component as TextBox;
					t.Area.text.Position = new Vector2f();
					t.Area.text.Rotation = 0;
					t.Area.text.Scale = new Vector2f(1, 1);
				}
			}
			component.masking = add ? owner : null;
		}

		public static void Create(params string[] uniqueIDs)
		{
			for (int i = 0; i < uniqueIDs.Length; i++)
			{
				if (Identity<Effects>.CannotCreate(uniqueIDs[i])) return;
				var instance = new Effects();
				instance.Identity = new(instance, uniqueIDs[i]);
			}
		}
		private Effects()
		{
			GrantAccessToFile(Debug.CurrentFilePath(2)); // grant the user access
			DenyAccessToFile(Debug.CurrentFilePath(1)); // abandon ship
			creationFrame = Performance.FrameCount;
			rand = Number.Random(new Bounds(-9999, 9999), 5);
		}
		internal void SetDefaults()
		{
			shader = new("shaders.vert", null, "shaders.frag");
			color = Color.White; lastFrameCol = Color.White;

			FillColor = Color.White;

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
			waterSpeed = new Size(5, 5);
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

		internal void DrawMasks(SFML.Graphics.RenderTexture rend)
		{
			for (int i = 0; i < masks.Count; i++)
			{
				if (masks[i].IsHidden) continue;

				var sc = new Vector2f((float)owner.Area.Size.W / rend.Size.X, (float)owner.Area.Size.H / rend.Size.Y);
				var o = masks[i].Area.OriginPercent / 100;

				if (masks[i] is Sprite)
				{
					var spr = (masks[i] as Sprite);
					var maskQuads = spr.quads;
					var maskQtv = Sprite.QuadsToVerts(maskQuads);
					var maskVertArr = maskQtv.Item1;
					var maskVerts = maskQtv.Item2;
					var texture = spr.TexturePath == null || File.textures.ContainsKey(spr.TexturePath) == false ?
						null : File.textures[spr.TexturePath];

					var w = maskVertArr.Bounds.Width;
					var h = maskVertArr.Bounds.Height;
					spr.Area.sprite.Position = Point.From(spr.Area.Position);
					spr.Area.sprite.Rotation = (float)spr.Area.Angle;
					spr.Area.sprite.Scale = new Vector2f((float)spr.Area.Size.W / w, (float)spr.Area.Size.H / h);
					spr.Area.sprite.Origin = new Vector2f((float)(w * o.X), (float)(h * o.Y));

					owner.Area.sprite.Position = Point.From(owner.Area.position);
					owner.Area.sprite.Rotation = (float)owner.Area.Angle;
					owner.Area.sprite.Scale = new Vector2f(1f, 1f);
					owner.Area.sprite.Origin = new Vector2f(0, 0);
					for (int j = 0; j < maskVerts.Length; j++)
					{
						var pp = spr.Area.sprite.Transform.TransformPoint(maskVerts[j].Position);
						var p = owner.Area.sprite.InverseTransform.TransformPoint(pp);
						var ownerOrOff = Point.From(new Point(rend.Size.X * o.X, rend.Size.Y * o.Y));

						maskVerts[j].Position = new Vector2f(p.X / sc.X, p.Y / sc.Y) + ownerOrOff;
					}

					rend.Draw(maskVerts, SFML.Graphics.PrimitiveType.Quads, new SFML.Graphics.RenderStates
						(SFML.Graphics.BlendMode.Alpha, SFML.Graphics.Transform.Identity, texture, null));
				}
				else
				{
					var t = masks[i] as TextBox;
					var dist = Point.Distance(t.Area.Position, owner.Area.Position);
					var atAng = Number.AngleBetweenPoints(owner.Area.Position, t.Area.Position);
					var pos = Point.From(Point.MoveAtAngle(
						owner.Area.Position, atAng - owner.Area.Angle, dist, Gear.Time.Unit.Tick));
					var ownerOrOff = Point.From(new Point(rend.Size.X * o.X, rend.Size.Y * o.Y));

					t.UpdateAllData();

					t.Area.text.Position = new Vector2f(pos.X / sc.X, pos.Y / sc.Y) + ownerOrOff;
					t.Area.text.Rotation = (float)(t.Area.Angle - owner.Area.Angle);
					t.Area.text.Scale = new Vector2f(t.Area.text.Scale.X / sc.X, t.Area.text.Scale.Y / sc.Y);
					rend.Draw(t.Area.text);
				}
			}
		}
	}
}
