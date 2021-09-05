using SFML.System;
using System;
using System.Collections.Generic;
using SMPL.Data;
using SMPL.Gear;

namespace SMPL.Components
{
	public class Effects : Component
	{
		private static void AddMask(List<Visual> list, Visual component, Visual owner, bool add)
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

		private readonly uint creationFrame;
		private double progress, maskColorBounds, gamma, desaturation, inversion, contrast, brightness, replaceColorBounds,
			outlineWidth, blinkSpeed, blinkOpacity, blurOpacity, earthquakeOpacity, waterOpacity, edgeThickness, edgeSensitivity,
			pixelateOpacity, pixelateStrength, ignoreDark, ignoreBright;
		private Mask maskType;
		private Color maskColor, bgColor, replaceColor, replaceWithColor, color, outlineColor, fillColor, edgeColor, gridColorX,
			gridColorY;
		private Size blurStrength, earthquakeStrength, waterStrength, waterSpeed, gridCellSpacing, gridCellSize, windStrength,
			windSpeed, vibrateStrength, sinStrength, cosStrength, sinSpeed, cosSpeed;

		//================

		internal List<Visual> masks = new();
		internal Visual owner;
		internal SFML.Graphics.Shader shader;

		internal void SetDefaults()
		{
			shader = new("shaders.vert", null, "shaders.frag");
			color = Color.White;

			FillColor = Color.White;

			outlineColor = Color.Black;
			shader.SetUniform("OutlineRed", 0f);
			shader.SetUniform("OutlineGreen", 0f);
			shader.SetUniform("OutlineBlue", 0f);
			shader.SetUniform("OutlineOpacity", 1f);

			replaceColorBounds = 1;
			shader.SetUniform("ReplaceMargin", 0.004f);

			maskColorBounds = 1;
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
					var texture = spr.TexturePath == null || Assets.textures.ContainsKey(spr.TexturePath) == false ?
						null : Assets.textures[spr.TexturePath];

					var w = maskVertArr.Bounds.Width;
					var h = maskVertArr.Bounds.Height;
					spr.Area.sprite.Position = Point.From(spr.Area.Position);
					spr.Area.sprite.Rotation = (float)spr.Area.Angle;
					spr.Area.sprite.Scale = new Vector2f((float)spr.Area.Size.W / w, (float)spr.Area.Size.H / h);
					spr.Area.sprite.Origin = new Vector2f((float)(w * o.X), (float)(h * o.Y));

					owner.Area.sprite.Position = Point.From(owner.Area.Position);
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

		//==============

		public enum Mask
		{
			None, In, Out
		}

		public Mask MaskType
		{
			get { return ErrorIfDestroyed() ? Mask.None : maskType; }
			set
			{
				if (ErrorIfDestroyed()) return;
				shader.SetUniform("HasMask", value == Mask.In || value == Mask.Out);
				shader.SetUniform("MaskOut", value == Mask.Out);
				maskType = value;
			}
		}
		public Color MaskColor
		{
			get { return ErrorIfDestroyed() ? Color.Invalid : maskColor; }
			set
			{
				if (ErrorIfDestroyed()) return;
				maskColor = value;
				shader.SetUniform("MaskRed", (float)value.R / 255f);
				shader.SetUniform("MaskGreen", (float)value.G / 255f);
				shader.SetUniform("MaskBlue", (float)value.B / 255f);
				shader.SetUniform("MaskOpacity", (float)value.A / 255f);
			}
		}
		public double MaskColorBounds
		{
			get { return ErrorIfDestroyed() ? double.NaN : maskColorBounds; }
			set
			{
				if (ErrorIfDestroyed()) return;
				maskColorBounds = value;
				shader.SetUniform("MaskMargin", (float)value / 255f);
			}
		}
		public double Progress
		{
			get { return ErrorIfDestroyed() ? double.NaN : progress; }
			set
			{
				if (ErrorIfDestroyed()) return;
				progress = value;
				shader.SetUniform("Time", (float)value);
			}
		}
		public double GammaPercent
		{
			get { return ErrorIfDestroyed() ? double.NaN : gamma; }
			set
			{
				if (ErrorIfDestroyed()) return;
				gamma = value;
				shader.SetUniform("Gamma", (float)value / 100f);
			}
		}
		public double DesaturationPercent
		{
			get { return ErrorIfDestroyed() ? double.NaN : desaturation; }
			set
			{
				if (ErrorIfDestroyed()) return;
				desaturation = value;
				shader.SetUniform("Desaturation", (float)value / 100f);
			}
		}
		public double InversionPercent
		{
			get { return ErrorIfDestroyed() ? double.NaN : inversion; }
			set
			{
				if (ErrorIfDestroyed()) return;
				inversion = value;
				shader.SetUniform("Inversion", (float)value / 100f);
			}
		}
		public double ContrastPercent
		{
			get { return ErrorIfDestroyed() ? double.NaN : contrast; }
			set
			{
				if (ErrorIfDestroyed()) return;
				contrast = value;
				shader.SetUniform("Contrast", (float)value / 100f);
			}
		}
		public double BrightnessPercent
		{
			get { return ErrorIfDestroyed() ? double.NaN : brightness; }
			set
			{
				if (ErrorIfDestroyed()) return;
				brightness = value;
				shader.SetUniform("Brightness", (float)value / 100f);
			}
		}
		public double ReplacedColorBounds
		{
			get { return ErrorIfDestroyed() ? double.NaN : replaceColorBounds; }
			set
			{
				if (ErrorIfDestroyed()) return;
				replaceColorBounds = value;
				shader.SetUniform("ReplaceMargin", (float)value / 255f);
			}
		}
		public Color ReplacedColor
		{
			get { return ErrorIfDestroyed() ? Color.Invalid : replaceColor; }
			set
			{
				if (ErrorIfDestroyed()) return;
				replaceColor = value;
				shader.SetUniform("ReplaceRed", (float)value.R / 255f);
				shader.SetUniform("ReplaceGreen", (float)value.G / 255f);
				shader.SetUniform("ReplaceBlue", (float)value.B / 255f);
				shader.SetUniform("ReplaceOpacity", (float)value.A / 255f);
			}
		}
		public Color ReplaceWithColor
		{
			get { return ErrorIfDestroyed() ? Color.Invalid : replaceWithColor; }
			set
			{
				if (ErrorIfDestroyed()) return;
				replaceWithColor = value;
				shader.SetUniform("ReplaceWithRed", (float)value.R / 255f);
				shader.SetUniform("ReplaceWithGreen", (float)value.G / 255f);
				shader.SetUniform("ReplaceWithBlue", (float)value.B / 255f);
				shader.SetUniform("ReplaceWithOpacity", (float)value.A / 255f);
			}
		}
		public Color BackgroundColor
		{
			get { return ErrorIfDestroyed() ? Color.Invalid : bgColor; }
			set { if (ErrorIfDestroyed() == false) bgColor = value; }
		}
		public Color TintColor
		{
			get
			{
				return ErrorIfDestroyed() ? Color.Invalid :
					Color.To(owner is TextBox ? (owner as TextBox).Area.text.FillColor : (owner as Sprite).Area.sprite.Color);
			}
			set { if (ErrorIfDestroyed() == false) color = value; }
		}
		public Color OutlineColor
		{
			get { return ErrorIfDestroyed() ? Color.Invalid : outlineColor; }
			set
			{
				if (ErrorIfDestroyed()) return;
				outlineColor = value;
				if (owner is TextBox) return;
				shader.SetUniform("OutlineRed", (float)value.R / 255f);
				shader.SetUniform("OutlineGreen", (float)value.G / 255f);
				shader.SetUniform("OutlineBlue", (float)value.B / 255f);
				shader.SetUniform("OutlineOpacity", (float)value.A / 255f);
			}
		}
		public Color FillColor
		{
			get { return ErrorIfDestroyed() ? Color.Invalid : fillColor; }
			set
			{
				if (ErrorIfDestroyed()) return;
				if (owner is Sprite && creationFrame == Performance.frameCount) return;
				fillColor = value;
				if (owner is TextBox) return;
				shader.SetUniform("FillRed", (float)value.R / 255f);
				shader.SetUniform("FillGreen", (float)value.G / 255f);
				shader.SetUniform("FillBlue", (float)value.B / 255f);
				shader.SetUniform("FillOpacity", (float)value.A / 255f);
			}
		}
		public double OutlineWidth
		{
			get { return ErrorIfDestroyed() ? double.NaN : outlineWidth; }
			set
			{
				if (ErrorIfDestroyed()) return;
				outlineWidth = value;
				if (owner is TextBox) return;
				shader.SetUniform("OutlineOffset", (float)value / 500f);
			}
		}
		public double ProgressiveBlinkSpeed
		{
			get { return ErrorIfDestroyed() ? double.NaN : blinkSpeed; }
			set
			{
				if (ErrorIfDestroyed()) return;
				blinkSpeed = value;
				shader.SetUniform("BlinkSpeed", (float)value);
			}
		}
		public double ProgressiveBlinkOpacityPercent
		{
			get { return ErrorIfDestroyed() ? double.NaN : blinkOpacity; }
			set
			{
				if (ErrorIfDestroyed()) return;
				blinkOpacity = value;
				shader.SetUniform("BlinkOpacity", (float)value / 100f);
			}
		}
		public double BlurOpacityPercent
		{
			get { return ErrorIfDestroyed() ? double.NaN : blurOpacity; }
			set
			{
				if (ErrorIfDestroyed()) return;
				blurOpacity = value;
				shader.SetUniform("BlurOpacity", (float)value / 100f);
			}
		}
		public Size BlurStrength
		{
			get { return ErrorIfDestroyed() ? Size.Invalid : blurStrength; }
			set
			{
				if (ErrorIfDestroyed()) return;
				blurStrength = value;
				shader.SetUniform("BlurStrengthX", (float)value.W / 200f);
				shader.SetUniform("BlurStrengthY", (float)value.H / 200f);
			}
		}
		public double ProgressiveEarthquakeOpacityPercent
		{
			get { return ErrorIfDestroyed() ? double.NaN : earthquakeOpacity; }
			set
			{
				if (ErrorIfDestroyed()) return;
				earthquakeOpacity = value;
				shader.SetUniform("EarthquakeOpacity", (float)value / 100f);
			}
		}
		public Size ProgressiveEarthquakeStrength
		{
			get { return ErrorIfDestroyed() ? Size.Invalid : earthquakeStrength; }
			set
			{
				if (ErrorIfDestroyed()) return;
				earthquakeStrength = value;
				shader.SetUniform("EarthquakeStrengthX", (float)value.W / 300f);
				shader.SetUniform("EarthquakeStrengthY", (float)value.H / 300f);
			}
		}
		public Size ProgressiveWaterStrength
		{
			get { return ErrorIfDestroyed() ? Size.Invalid : waterStrength; }
			set
			{
				if (ErrorIfDestroyed()) return;
				waterStrength = value;
				shader.SetUniform("WaterStrengthX", (float)value.W / 10f);
				shader.SetUniform("WaterStrengthY", (float)value.H / 10f);
			}
		}
		public Size ProgressiveWaterSpeed
		{
			get { return ErrorIfDestroyed() ? Size.Invalid : waterSpeed; }
			set
			{
				if (ErrorIfDestroyed()) return;
				waterSpeed = value;
				shader.SetUniform("WaterSpeedX", (float)value.W / 40f);
				shader.SetUniform("WaterSpeedY", (float)value.H / 40f);
			}
		}
		public double ProgressiveWaterOpacityPercent
		{
			get { return ErrorIfDestroyed() ? double.NaN : waterOpacity; }
			set
			{
				if (ErrorIfDestroyed()) return;
				waterOpacity = value;
				shader.SetUniform("WaterOpacity", (float)value / 100f);
			}
		}
		public Color EdgeColor
		{
			get { return ErrorIfDestroyed() ? Color.Invalid : edgeColor; }
			set
			{
				if (ErrorIfDestroyed()) return;
				edgeColor = value;
				shader.SetUniform("EdgeRed", (float)value.R / 255f);
				shader.SetUniform("EdgeGreen", (float)value.G / 255f);
				shader.SetUniform("EdgeBlue", (float)value.B / 255f);
				shader.SetUniform("EdgeOpacity", (float)value.A / 255f);
			}
		}
		public double EdgeSensitivity
		{
			get { return ErrorIfDestroyed() ? double.NaN : edgeSensitivity; }
			set
			{
				if (ErrorIfDestroyed()) return;
				edgeSensitivity = value;
				shader.SetUniform("EdgeSensitivity", (100f - (float)value) / 160f);
			}
		}
		public double EdgeThickness
		{
			get { return ErrorIfDestroyed() ? double.NaN : edgeThickness; }
			set
			{
				if (ErrorIfDestroyed()) return;
				edgeThickness = value;
				shader.SetUniform("EdgeThickness", 1f - (float)value / 500f);
			}
		}
		public double PixelateOpacityPercent
		{
			get { return ErrorIfDestroyed() ? double.NaN : pixelateOpacity; }
			set
			{
				if (ErrorIfDestroyed()) return;
				pixelateOpacity = value;
				shader.SetUniform("PixelateOpacity", (float)value / 100f);
			}
		}
		public double PixelateStrength
		{
			get { return ErrorIfDestroyed() ? double.NaN : pixelateStrength; }
			set
			{
				if (ErrorIfDestroyed()) return;
				pixelateStrength = value;
				shader.SetUniform("PixelateStrength", (float)value / 100f);
			}
		}
		public double IgnoreDarkPercent
		{
			get { return ErrorIfDestroyed() ? double.NaN : ignoreDark; }
			set
			{
				if (ErrorIfDestroyed()) return;
				ignoreDark = value;
				shader.SetUniform("IgnoreDark", (float)value / 100f);
			}
		}
		public double IgnoreBrightPercent
		{
			get { return ErrorIfDestroyed() ? double.NaN : ignoreBright; }
			set
			{
				if (ErrorIfDestroyed()) return;
				ignoreBright = value;
				shader.SetUniform("IgnoreBright", (float)value / 100f);
			}
		}
		public Size GridCellSize
		{
			get { return ErrorIfDestroyed() ? Size.Invalid : gridCellSize; }
			set
			{
				if (ErrorIfDestroyed()) return;
				gridCellSize = value;
				shader.SetUniform("GridCellWidth", (float)value.W * 2f);
				shader.SetUniform("GridCellHeight", (float)value.H * 2f);
			}
		}
		public Size GridCellSpacing
		{
			get { return ErrorIfDestroyed() ? Size.Invalid : gridCellSpacing; }
			set
			{
				if (ErrorIfDestroyed()) return;
				gridCellSpacing = value;
				shader.SetUniform("GridCellSpacingX", (float)value.W / 5f);
				shader.SetUniform("GridCellSpacingY", (float)value.H / 5f);
			}
		}
		public Color GridColorX
		{
			get { return ErrorIfDestroyed() ? Color.Invalid : gridColorX; }
			set
			{
				if (ErrorIfDestroyed()) return;
				gridColorX = value;
				shader.SetUniform("GridRedX", (float)value.R / 255f);
				shader.SetUniform("GridGreenX", (float)value.G / 255f);
				shader.SetUniform("GridBlueX", (float)value.B / 255f);
				shader.SetUniform("GridOpacityX", (float)value.A / 255f);
			}
		}
		public Color GridColorY
		{
			get { return ErrorIfDestroyed() ? Color.Invalid : gridColorY; }
			set
			{
				if (ErrorIfDestroyed()) return;
				gridColorY = value;
				shader.SetUniform("GridRedY", (float)value.R / 255f);
				shader.SetUniform("GridGreenY", (float)value.G / 255f);
				shader.SetUniform("GridBlueY", (float)value.B / 255f);
				shader.SetUniform("GridOpacityY", (float)value.A / 255f);
			}
		}
		public Size ProgressiveWindStrength
		{
			get { return ErrorIfDestroyed() ? Size.Invalid : windStrength; }
			set
			{
				if (ErrorIfDestroyed()) return;
				windStrength = value;
				shader.SetUniform("WindStrengthX", (float)value.W / 5f);
				shader.SetUniform("WindStrengthY", (float)value.H / 5f);
			}
		}
		public Size ProgressiveWindSpeed
		{
			get { return ErrorIfDestroyed() ? Size.Invalid : windSpeed; }
			set
			{
				if (ErrorIfDestroyed()) return;
				windSpeed = value;
				shader.SetUniform("WindSpeedX", (float)value.W / 8f);
				shader.SetUniform("WindSpeedY", (float)value.H / 8f);
			}
		}
		public Size ProgressiveVibrateStrength
		{
			get { return ErrorIfDestroyed() ? Size.Invalid : vibrateStrength; }
			set
			{
				if (ErrorIfDestroyed()) return;
				vibrateStrength = value;
				shader.SetUniform("VibrateStrengthX", (float)value.W / 10f);
				shader.SetUniform("VibrateStrengthY", (float)value.H / 10f);
			}
		}
		public Size ProgressiveWaveStrengthSin
		{
			get { return ErrorIfDestroyed() ? Size.Invalid : sinStrength; }
			set
			{
				if (ErrorIfDestroyed()) return;
				sinStrength = value;
				shader.SetUniform("SinStrengthX", (float)value.W);
				shader.SetUniform("SinStrengthY", (float)value.H);
			}
		}
		public Size ProgressiveWaveStrengthCos
		{
			get { return ErrorIfDestroyed() ? Size.Invalid : cosStrength; }
			set
			{
				if (ErrorIfDestroyed()) return;
				cosStrength = value;
				shader.SetUniform("CosStrengthX", (float)value.W);
				shader.SetUniform("CosStrengthY", (float)value.H);
			}
		}
		public Size ProgressiveWaveSpeedSin
		{
			get { return ErrorIfDestroyed() ? Size.Invalid : sinSpeed; }
			set
			{
				if (ErrorIfDestroyed()) return;
				sinSpeed = value;
				shader.SetUniform("SinSpeedX", (float)value.W / 10f);
				shader.SetUniform("SinSpeedY", (float)value.H / 10f);
			}
		}
		public Size ProgressiveWaveSpeedCos
		{
			get { return ErrorIfDestroyed() ? Size.Invalid : cosSpeed; }
			set
			{
				if (ErrorIfDestroyed()) return;
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

		public void AddMasks(params Visual[] masks)
		{
			if (ErrorIfDestroyed() == false)
				for (int i = 0; i < masks.Length; i++)
					AddMask(this.masks, masks[i], owner, true);
		}
		public void RemoveMasks(params Visual[] masks)
		{
			if (ErrorIfDestroyed() == false)
				for (int i = 0; i < masks.Length; i++)
					AddMask(this.masks, masks[i], owner, false);
		}
		public Visual[] Masks => masks.ToArray();
		public Visual MaskTarget => owner.masking;

		public Effects(string uniqueID) : base(uniqueID)
		{
			creationFrame = Performance.FrameCount;
			if (cannotCreate) { ErrorAlreadyHasUID(uniqueID); Destroy(); }
		}
		public override void Destroy()
		{
			if (ErrorIfDestroyed()) return;
			masks.Clear();
			owner.Effects = null;
			owner = null;
			shader.Dispose();
			shader = null;
			base.Destroy();
		}
	}
}
