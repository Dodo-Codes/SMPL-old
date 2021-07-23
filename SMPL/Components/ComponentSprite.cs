using SFML.Graphics;
using SFML.System;
using static SMPL.Events;

namespace SMPL
{
	public class ComponentSprite
	{
		internal Sprite sprite = new();

		internal Component2D transform;
		//public Effects Effects { get; set; }
		//internal Image image;
		//internal SFML.Graphics.Texture rawTexture;
		//internal byte[] rawTextureData;

		public bool IsRepeated
		{
			get { return sprite.Texture.Repeated; }
			set { sprite.Texture.Repeated = value; }
		}
		public bool IsSmooth
		{
			get { return sprite.Texture.Smooth; }
			set { sprite.Texture.Smooth = value; }
		}
		private string path;
		public string TexturePath
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
				var texture = File.textures[value];
				sprite.Texture = texture;

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
				var rect = sprite.TextureRect;
				var sz = sprite.Texture.Size;
				sprite.TextureRect = new IntRect(
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

				var sz = sprite.Texture.Size;
				var textRect = sprite.TextureRect;
				sprite.TextureRect = new IntRect(
					textRect.Left, textRect.Top,
					(int)(sz.X * value.W), (int)(sz.Y * value.H));
			}
		}

		public bool IsHidden { get; set; }
		private Size repeats;
		public Size Repeats
		{
			get { return repeats; }
			set
			{
				repeats = value;
				transform.OriginPercent = transform.OriginPercent;
			}
		}

		internal void Update()
      {

      }

		public ComponentSprite(Component2D component2D, string texturePath = "folder/texture.png")
		{
			sprites.Add(this);
			transform = component2D;
			//Effects = new(this);
			TexturePath = texturePath;
			IsRepeated = true;
			SizePercent = new Size(100, 100);
		}

		public void Draw(Camera camera)
		{
			if (Window.DrawNotAllowed() || IsHidden || sprite == null ||
				sprite.Texture == null || transform == null) return;

			var w = sprite.TextureRect.Width;
			var h = sprite.TextureRect.Height;
			var p = transform.OriginPercent / 100;
			var x = w * (float)p.X * ((float)Repeats.W / 2f) + (w * (float)p.X / 2f);
			var y = h * (float)p.Y * ((float)Repeats.H / 2f) + (h * (float)p.Y / 2f);

			sprite.Origin = new Vector2f(x, y);
			sprite.Position = Point.From(transform.Position);
			sprite.Rotation = (float)transform.Angle;
			sprite.Scale = new Vector2f(
				(float)transform.Size.W / sprite.Texture.Size.X,
				(float)transform.Size.H / sprite.Texture.Size.Y);

			var pos = sprite.Position;
			for (int j = 0; j < Repeats.H + 1; j++)
			{
				for (int i = 0; i < Repeats.W + 1; i++)
				{
					var p1 = sprite.Transform.TransformPoint(new Vector2f((pos.X + w) * i, (pos.Y + h) * j));

					sprite.Position = p1;
					camera.rendTexture.Draw(sprite);//, new RenderStates(Effects.shader));
					sprite.Position = pos;
				}
			}
		}
		public void DrawBounds(Camera camera, float thickness, Color color)
		{
			var b = sprite.GetGlobalBounds();
			var c = Color.From(color);
			var left = new Vertex[]
			{
				new Vertex(new Vector2f(b.Left - thickness, b.Top - thickness), c),
				new Vertex(new Vector2f(b.Left + thickness, b.Top - thickness), c),
				new Vertex(new Vector2f(b.Left + thickness, b.Top + thickness + b.Height), c),
				new Vertex(new Vector2f(b.Left - thickness, b.Top + thickness + b.Height), c),
			};
			camera.rendTexture.Draw(left, PrimitiveType.Quads);
		}
	}
}
