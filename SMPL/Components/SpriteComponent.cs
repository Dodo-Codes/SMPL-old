using SFML.Graphics;
using System;
using System.Collections.Generic;

namespace SMPL
{
	public class SpriteComponent
	{
		public enum Mask
		{
			None, In, Out
		}

		internal Image image;
		internal Texture rawTexture;
		internal byte[] rawTextureData;
		internal Sprite sprite = new();

		public TransformComponent TransformComponent { get; set; }
		public Effects Effects { get; set; } = new();
		private Mask maskType;
		public Mask MaskType
		{
			get { return maskType; }
			set
			{
				Effects.shader.SetUniform("HasMask", value == Mask.In || value == Mask.Out);
				Effects.shader.SetUniform("MaskOut", value == Mask.Out);
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
				Effects.shader.SetUniform("MaskRed", (float)value.Red / 255f);
				Effects.shader.SetUniform("MaskGreen", (float)value.Green / 255f);
				Effects.shader.SetUniform("MaskBlue", (float)value.Blue / 255f);
			}
		}
		public Color TintColor
		{
			get { return Color.To(sprite.Color); }
			set { sprite.Color = Color.From(value); }
		}

		private double time;
		public double Time
		{
			get { return time; }
			set { time = value; Effects.shader.SetUniform("Time", (float)value); }
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
				Effects.shader.SetUniform("texture", sprite.Texture);
				Effects.shader.SetUniform("raw_texture", rawTexture);
				texturePath = value;
			}
		}
		public SpriteComponent(TransformComponent transformComponent, string texturePath = "folder/texture.png")
		{
			TransformComponent = transformComponent;
			TexturePath = texturePath;
		}

		//public void SetEffect(Effect effect, double value)
		//{
		//	
		//}
		//internal void SetShaderArg(Effect effect, double rawValue, double value, bool usePercent1)
		//{
		//	var percent1 = Number.FromPercent(value, new Bounds(0, 1));
		//	effects[effect] = rawValue;
		//	shader.SetUniform($"{effect}".ToLower(), (float)(usePercent1 ? percent1 : value));
		//}

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
