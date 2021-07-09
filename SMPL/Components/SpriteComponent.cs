using SFML.Graphics;
using System.ComponentModel;

namespace SMPL
{
	public class SpriteComponent
	{
		internal Image image;
		internal SFML.Graphics.Texture rawTexture;
		internal byte[] rawTextureData;
		internal Sprite sprite = new();

		internal TransformComponent transform;
		public Effects Effects { get; set; }
		public Texture Texture { get; set; }

		public bool IsHidden { get; set; }
		public Size Repeats { get; set; }
		private Point originPercent;
		public Point OriginPercent
		{
			get { return originPercent; }
			set
			{
				originPercent = value;
				var w = sprite.TextureRect.Width;
				var h = sprite.TextureRect.Height;

				value.X = Number.Limit(value.X, new Bounds(0, 100));
				value.Y = Number.Limit(value.Y, new Bounds(0, 100));
				var p = value / 100;
				var x = w * (float)p.X * ((float)Repeats.Width / 2f) + (w * (float)p.X / 2f);
				var y = h * (float)p.Y * ((float)Repeats.Height / 2f) + (h * (float)p.Y / 2f);

				sprite.Origin = new SFML.System.Vector2f(x, y);
			}
		}

		public SpriteComponent(TransformComponent transformComponent, string texturePath = "folder/texture.png")
		{
			transform = transformComponent;
			Effects = new(this);
			Texture = new(this, texturePath);
		}
	}
}
