using SFML.Graphics;
using SFML.System;

namespace SMPL
{
	public class TextComponent
	{
		internal Font font;
		internal SFML.Graphics.Text text;

		internal TransformComponent transform;
		//public Effects Effects { get; set; }

		public bool IsHidden { get; set; }
		private Point originPercent;
		public Point OriginPercent
		{
			get { return originPercent; }
			set
			{
				originPercent = value;
				var b = text.GetGlobalBounds();

				value.X = Number.Limit(value.X, new Bounds(0, 100));
				value.Y = Number.Limit(value.Y, new Bounds(0, 100));
				var p = value / 100;
				var x = b.Width * (float)p.X + (b.Width * (float)p.X / 2f);
				var y = b.Height * (float)p.Y + (b.Height * (float)p.Y / 2f);

				text.Origin = new Vector2f(x, y);
			}
		}

		public TextComponent(TransformComponent transformComponent, string fontPath = "folder/font.ttf")
		{
			transform = transformComponent;
			//Effects = new(this);
			font = new(fontPath);
			text = new("WWWWWWWWWWWWW\nHello World!", font);
			transform.Size = new Size(100, 100);
			text.CharacterSize = 64;
		}

		public void Draw(Camera camera)
		{
			if (Window.DrawNotAllowed() || IsHidden || text == null ||
				text.Font == null || transform == null) return;

			text.Position = Point.From(transform.Position);
			text.Rotation = (float)transform.Angle;

			var rend = new RenderTexture((uint)transform.Size.Width, (uint)transform.Size.Height);
			//rend.SetView(camera.view);
			rend.Draw(text);
			rend.Display();
			var sprite = new Sprite(rend.Texture);
			camera.rendTexture.Draw(sprite);//, new RenderStates(Effects.shader));

			rend.Dispose();
			sprite.Dispose();
		}
		public void DrawBounds(Camera camera, double thickness, Color color)
		{
			var b = text.GetGlobalBounds();
			var c = Color.From(color);
			var thf = (float)thickness;
			var off = 16;
			var left = new Vertex[]
			{
				new Vertex(new Vector2f(b.Left - thf - off, b.Top - thf), c),
				new Vertex(new Vector2f(b.Left + thf - off, b.Top - thf), c),
				new Vertex(new Vector2f(b.Left + thf - off, b.Top + thf + b.Height), c),
				new Vertex(new Vector2f(b.Left - thf - off, b.Top + thf + b.Height), c),
			};
			var right = new Vertex[]
			{
				new Vertex(new Vector2f(b.Left - thf - off + b.Width, b.Top - thf), c),
				new Vertex(new Vector2f(b.Left + thf - off + b.Width, b.Top - thf), c),
				new Vertex(new Vector2f(b.Left + thf - off + b.Width, b.Top + thf + b.Height), c),
				new Vertex(new Vector2f(b.Left - thf - off + b.Width, b.Top + thf + b.Height), c),
			};
			var up = new Vertex[]
			{
				new Vertex(new Vector2f(b.Left - thf - off, b.Top - thf), c),
				new Vertex(new Vector2f(b.Left + thf - off + b.Width, b.Top - thf), c),
				new Vertex(new Vector2f(b.Left + thf - off + b.Width, b.Top + thf), c),
				new Vertex(new Vector2f(b.Left - thf - off, b.Top + thf), c),
			};
			var bot = new Vertex[]
			{
				new Vertex(new Vector2f(b.Left - thf - off, b.Top - thf + b.Height), c),
				new Vertex(new Vector2f(b.Left + thf - off + b.Width, b.Top - thf + b.Height), c),
				new Vertex(new Vector2f(b.Left + thf - off + b.Width, b.Top + thf + b.Height), c),
				new Vertex(new Vector2f(b.Left - thf - off, b.Top + thf + b.Height), c),
			};
			camera.rendTexture.Draw(left, PrimitiveType.Quads);
			camera.rendTexture.Draw(right, PrimitiveType.Quads);
			camera.rendTexture.Draw(up, PrimitiveType.Quads);
			camera.rendTexture.Draw(bot, PrimitiveType.Quads);
		}
	}
}
