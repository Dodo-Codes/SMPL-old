using SFML.Graphics;
using System;
using System.Collections.Generic;

namespace SMPL
{
	public class SpriteComponent
	{
		public enum Effect
		{
			TintRed, TintGreen, TintBlue, Opacity,
			Gamma, Desaturation, Inversion, Contrast, Brightness,
			OutlineOpacity, OutlineOffset, OutlineRed, OutlineGreen, OutlineBlue,
			FillOpacity, FillRed, FillGreen, FillBlue,
			BlinkOpacity, BlinkSpeed,
			BlurOpacity, BlurStrengthX, BlurStrengthY,
			EarthquakeOpacity, EarthquakeStrengthX, EarthquakeStrengthY,
			StretchOpacity, StretchStrengthX, StretchStrengthY, StretchSpeedX, StretchSpeedY,
			WaterOpacity, WaterStrengthX, WaterStrengthY, WaterSpeedX, WaterSpeedY,
			EdgeOpacity, EdgeThreshold, EdgeThickness, EdgeRed, EdgeGreen, EdgeBlue,
			PixelateOpacity, PixelateThreshold,
			GridOpacityX, GridOpacityY, GridCellWidth, GridCellHeight, GridCellSpacingX, GridCellSpacingY,
			GridRedX, GridRedY, GridGreenX, GridGreenY, GridBlueX, GridBlueY,
			WindX, WindY, WindSpeedX, WindSpeedY,
			VibrateX, VibrateY,
			WaveSinX, WaveSinY, WaveCosX, WaveCosY, WaveSinSpeedX, WaveSinSpeedY, WaveCosSpeedX, WaveCosSpeedY,
		}
		public enum Mask
		{
			None, In, Out
		}
		internal Dictionary<Effect, double> effects = new();

		internal Image image;
		internal Dictionary<Effect, double> shaderArgs = new();
		internal Shader shader = new("shaders.vert", null, "shaders.frag");
		internal Texture rawTexture;
		internal byte[] rawTextureData;
		internal Sprite sprite = new();

		public TransformComponent TransformComponent { get; set; }
		private Mask maskType;
		public Mask MaskType
		{
			get { return maskType; }
			set
			{
				shader.SetUniform("hasmask", value == Mask.In || value == Mask.Out);
				shader.SetUniform("maskout", value == Mask.Out);
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
				shader.SetUniform("maskred", (float)value.Red / 255f);
				shader.SetUniform("maskgreen", (float)value.Green / 255f);
				shader.SetUniform("maskblue", (float)value.Blue / 255f);
			}
		}

		private string texturePath;
		public string TexturePath
		{
			get { return texturePath; }
			set
			{
				if (File.textures.ContainsKey(value) == false)
				{
					Debug.LogError(2, $"The texture at '{value}' is not loaded.\nUse " +
						$"'{nameof(File)}.{nameof(File.LoadAsset)}({nameof(File)}.{nameof(File.Asset)}." +
						$"{nameof(File.Asset.Texture)}, \"{value}\")' to load it.");
					return;
				}
				sprite.Texture = File.textures[value];
				image = new Image(sprite.Texture.CopyToImage());
				if (MaskType != Mask.In) image.FlipVertically();
				rawTextureData = image.Pixels;
				image.FlipVertically();
				rawTexture = new Texture(image);
				shader.SetUniform("texture", sprite.Texture);
				shader.SetUniform("raw_texture", rawTexture);
				texturePath = value;
			}
		}
		public SpriteComponent(TransformComponent transformComponent, string texturePath = "folder/texture.png")
		{
			var effects = (Effect[])Enum.GetValues(typeof(Effect));
			foreach (var effect in effects) this.effects.Add(effect, 0);
			TransformComponent = transformComponent;
			TexturePath = texturePath;
		}

		public void SetEffect(Effect effect, double value)
		{
			
		}
		internal void SetShaderArg(Effect effect, double rawValue, double value, bool usePercent1)
		{
			var percent1 = Number.FromPercent(value, new Bounds(0, 1));
			effects[effect] = rawValue;
			shader.SetUniform($"{effect}".ToLower(), (float)(usePercent1 ? percent1 : value));
		}

		//Blink.Speed.Set(100);
		//Outline.Offset.Set(20);
		//Stretch.Speed.Set(50, 50);
		//Blur.Strength.Set(50, 50);
		//Water.Strength.Set(50, 50);
		//Water.Speed.Set(50, 50);
		//Edge.Sensitivity.Set(50);
		//Edge.Thickness.Set(50);
		//Pixelate.Strength.Set(50);
		//Earthquake.Strength.Set(20, 30);
		//Grid.CellSize.Set(5, 5);
		//Grid.CellSpacing.Set(20, 20);
		//Wind.Speed.Set(10, 10);
		//Wave.Speed.Set(20, 20, 20, 20);
	}
}
