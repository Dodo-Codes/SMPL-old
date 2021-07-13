using SFML.Graphics;
using SFML.System;
using System.Collections.Generic;
using System.ComponentModel;

namespace SMPL
{
	public class SpriteComponent
	{
		internal Sprite sprite = new();

		internal TransformComponent transform;
		//public Effects Effects { get; set; }
		public Texture Texture { get; set; }

		public bool IsHidden { get; set; }
		private Size repeats;
		public Size Repeats
		{
			get { return repeats; }
			set
			{
				repeats = value;
				OriginPercent = originPercent;
			}
		}
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
			//Effects = new(this);
			Texture = new(this, texturePath);
		}

		public void Draw(Camera camera)
		{
			if (Window.DrawNotAllowed() || IsHidden || sprite == null ||
				sprite.Texture == null || transform == null) return;

			sprite.Position = Point.From(transform.Position);
			sprite.Rotation = (float)transform.Angle;
			sprite.Scale = new Vector2f(
				(float)transform.Size.Width / sprite.Texture.Size.X,
				(float)transform.Size.Height / sprite.Texture.Size.Y);

			var pos = sprite.Position;
			for (int j = 0; j < Repeats.Height + 1; j++)
			{
				for (int i = 0; i < Repeats.Width + 1; i++)
				{
					var w = sprite.TextureRect.Width;
					var h = sprite.TextureRect.Height;
					var p = sprite.Transform.TransformPoint(new Vector2f((pos.X + w) * i, (pos.Y + h) * j));

					sprite.Position = p;
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
