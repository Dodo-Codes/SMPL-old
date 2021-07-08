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

		public SpriteComponent(TransformComponent transformComponent, string texturePath = "folder/texture.png")
		{
			TransformComponent = transformComponent;
			TexturePath = texturePath;
		}
	}
}
