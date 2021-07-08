using SFML.Graphics;

namespace SMPL
{
	public class SpriteComponent
	{
		internal Image image;
		internal Texture rawTexture;
		internal byte[] rawTextureData;

		public TransformComponent TransformComponent { get; set; }
		public Effects Effects { get; set; } = new();

		public bool TextureIsRepeated
		{
			get { return Effects.sprite.Texture.Repeated; }
			set { Effects.sprite.Texture.Repeated = value; }
		}
		public bool TextureIsSmooth
		{
			get { return Effects.sprite.Texture.Smooth; }
			set { Effects.sprite.Texture.Smooth = value; }
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
				Effects.sprite.Texture = File.textures[value];
				image = new Image(Effects.sprite.Texture.CopyToImage());
				if (Effects.MaskType != Effects.Mask.In) image.FlipVertically();
				rawTextureData = image.Pixels;
				image.FlipVertically();
				rawTexture = new Texture(image);
				Effects.shader.SetUniform("texture", Effects.sprite.Texture);
				Effects.shader.SetUniform("raw_texture", rawTexture);
				texturePath = value;
			}
		}
		private Point textureOffsetPercent;
		public Point TextureOffsetPercent
		{
			get { return textureOffsetPercent; }
			set
			{
				textureOffsetPercent = value;
				var rect = Effects.sprite.TextureRect;
				var sz = Effects.sprite.Texture.Size;
				Effects.sprite.TextureRect = new IntRect(
					(int)(sz.X * (textureOffsetPercent.X / 100)), (int)(sz.Y * (textureOffsetPercent.Y / 100)),
					rect.Width, rect.Height);
			}
		}
		public Size Repeats { get; set; }
		private Point originPercent;
		public Point OriginPercent
		{
			get { return originPercent; }
			set
			{
				originPercent = value;
				//var sprite = data.objects[objectID].sprite;
				//var w = data.objects[objectID].sprite.TextureRect.Width;
				//
				//var repX = GetNumber(UnitNumber.RepeatAmountX);
				//
				//percentX = Simple.Number.Limited.Get(percentX, 0, 100);
				//					var pX = (float)percentX / 100;
				//var value = sprite.Texture == null ? 0 : w * pX * ((float)repX / 2) + (w * pX / 2);
				//
				//data.objects[objectID].numbers[UnitNumber.OriginPercentX] = percentX;
				//					if (sprite.Origin.X == value) return; // skip re-rendering the screen & replacing same value
				//
				//					data.objects[objectID].sprite.Origin = new Vector2f(value, sprite.Origin.Y);
			}
		}
		private Size textureSizePercent;
		public Size TextureSizePercent
		{
			get { return textureSizePercent; }
			set
			{
				textureSizePercent = value;
				//percentWidth /= 100;
				//
				//var sz = data.objects[objectID].sprite.Texture.Size;
				//var textRect = data.objects[objectID].sprite.TextureRect;
				//data.objects[objectID].sprite.TextureRect =
				//	new IntRect(textRect.Left, textRect.Top, (int)(sz.X * percentWidth), textRect.Height);
			}
		}

		public SpriteComponent(TransformComponent transformComponent, string texturePath = "folder/texture.png")
		{
			TransformComponent = transformComponent;
			TexturePath = texturePath;
			TextureIsRepeated = true;
		}
	}
}
