using SFML.Graphics;

namespace SMPL
{
	public class Texture
	{
		internal SpriteComponent parent;
		public bool IsRepeated
		{
			get { return parent.sprite.Texture.Repeated; }
			set { parent.sprite.Texture.Repeated = value; }
		}
		public bool IsSmooth
		{
			get { return parent.sprite.Texture.Smooth; }
			set { parent.sprite.Texture.Smooth = value; }
		}
		private string path;
		public string Path
		{
			get { return path; }
			set
			{
				if (File.textures.ContainsKey(value) == false)
				{
					Debug.LogError(2, $"The texture at '{value}' is not loaded.\n" +
						$"Use '{nameof(File)}.{nameof(File.LoadAsset)} ({nameof(File)}.{nameof(File.Asset)}." +
						$"{nameof(File.Asset.Texture)}, \"{value}\")' to load it.");
					return;
				}
				parent.sprite.Texture = File.textures[value];
				//parent.image = new Image(parent.sprite.Texture.CopyToImage());
				//if (parent.Effects.MaskType != Effects.Mask.In) parent.image.FlipVertically();
				//parent.rawTextureData = parent.image.Pixels;
				//parent.image.FlipVertically();
				//parent.rawTexture = new SFML.Graphics.Texture(parent.image);
				//parent.Effects.shader.SetUniform("texture", parent.sprite.Texture);
				//parent.Effects.shader.SetUniform("raw_texture", parent.rawTexture);
				path = value;
			}
		}
		private Point offsetPercent;
		public Point OffsetPercent
		{
			get { return offsetPercent; }
			set
			{
				offsetPercent = value;
				var rect = parent.sprite.TextureRect;
				var sz = parent.sprite.Texture.Size;
				parent.sprite.TextureRect = new IntRect(
					(int)(sz.X * (offsetPercent.X / 100)), (int)(sz.Y * (offsetPercent.Y / 100)),
					rect.Width, rect.Height);
			}
		}
		private Size sizePercent;
		public Size SizePercent
		{
			get { return sizePercent; }
			set
			{
				sizePercent = value;
				value /= 100;

				var sz = parent.sprite.Texture.Size;
				var textRect = parent.sprite.TextureRect;
				parent.sprite.TextureRect = new IntRect(
					textRect.Left, textRect.Top,
					(int)(sz.X * value.Width), (int)(sz.Y * value.Height));
			}
		}

		public Texture(SpriteComponent parent, string texturePath = "folder/texture.png")
		{
			this.parent = parent;
			Path = texturePath;
			IsRepeated = true;
			SizePercent = new Size(100, 100);
		}
	}
}
