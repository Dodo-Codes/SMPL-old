using SFML.Graphics;
using System.Collections.Generic;

namespace SMPL
{
	public class EffectComponent
	{
		public enum Type
		{
			TintRed, TintGreen, TintBlue, Opacity,
			Gamma, Desaturation, Inversion, Contrast, Brightness,
			Outline, OutlineOffset, OutlineRed, OutlineGreen, OutlineBlue,
			Fill, FillRed, FillGreen, FillBlue,
			Blink, BlinkSpeed,
			Blur, BlurStrengthX, BlurStrengthY,
			Earthquake, EarthquakeX, EarthquakeY,
			Stretch, StretchX, StretchY, StretchSpeedX, StretchSpeedY,
			Water, WaterStrengthX, WaterStrengthY, WaterSpeedX, WaterSpeedY,
			Edge, EdgeThreshold, EdgeThickness, EdgeRed, EdgeGreen, EdgeBlue,
			Pixelate, PixelateThreshold,
			GridX, GridY, GridCellWidth, GridCellHeight, GridCellSpacingX, GridCellSpacingY,
			GridRedX, GridRedY, GridGreenX, GridGreenY, GridBlueX, GridBlueY,
			WindX, WindY, WindSpeedX, WindSpeedY,
			VibrateX, VibrateY,
			WaveSinX, WaveSinY, WaveCosX, WaveCosY, WaveSinSpeedX, WaveSinSpeedY, WaveCosSpeedX, WaveCosSpeedY,
			MaskRed, MaskGreen, MaskBlue,
		}
		internal Dictionary<Type, double> effects = new();

		internal Image image;
		internal Dictionary<Type, double> shaderArgs = new();
		internal Shader shader = new("shaders.vert", null, "shaders.frag");
		internal Texture rawTexture;
		internal byte[] rawTextureData;

		public bool MasksIn { get; set; }

		internal void SetShaderArg(Type effect, double rawValue, double value, bool usePercent1)
		{
			var percent1 = Number.FromPercent(value, new Bounds(0, 1));
			var arg = "";
			effects[effect] = rawValue;
			shader.SetUniform(arg, (float)(usePercent1 ? percent1 : value));
		}
	}
}
