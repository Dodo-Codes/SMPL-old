using SFML.Graphics;
using SFML.System;

namespace SMPL
{
	public class TextComponent
	{
		internal TransformComponent transform;
		//public Effects Effects { get; set; }
		internal SFML.Graphics.Text text;

		private Point position;
		public Point Position
		{
			get { return position; }
			set
			{
				position = value;
				var t = new SFML.Graphics.Text("12", text.Font);
				t.CharacterSize = text.CharacterSize;
				var d = (t.FindCharacterPos(1) - t.FindCharacterPos(0)).X;
				text.Position = Point.From(value) + new Vector2f(d, 0);
				t.Dispose();
			}
		}
		public double Angle
		{
			get { return text.Rotation; }
			set { text.Rotation = (float)value; }
		}
		public Size Scale
		{
			get { return Size.To(text.Scale); }
			set { text.Scale = Size.From(value); }
		}
		private string path;
		public string FontPath
		{
			get { return path; }
			set
			{
				if (File.fonts.ContainsKey(value) == false)
				{
					Debug.LogError(2, $"The font at '{value}' is not loaded.\n" +
						$"Use '{nameof(File)}.{nameof(File.LoadAsset)} ({nameof(File)}.{nameof(File.Asset)}." +
						$"{nameof(File.Asset.Font)}, \"{value}\")' to load it.");
					return;
				}
				text.Font = File.fonts[value];
				path = value;
			}
		}

		public bool IsHidden { get; set; }
		private Point boxOriginPercent;
		public Point BoxOriginPercent
		{
			get { return boxOriginPercent; }
			set
			{
				value.X = Number.Limit(value.X, new Bounds(0, 100));
				value.Y = Number.Limit(value.Y, new Bounds(0, 100));
				boxOriginPercent = new Point(value.X, value.Y);
			}
		}
		private Point originPercent;
		public Point OriginPercent
		{
			get { return originPercent; }
			set
			{
				value.X = Number.Limit(value.X, new Bounds(0, 100));
				value.Y = Number.Limit(value.Y, new Bounds(0, 100));
				originPercent = new Point(value.X, value.Y);

				text.DisplayedString += "\n";
				var pos = new Point();
				for (uint i = 0; i < Text.Length; i++)
				{
					var p = text.FindCharacterPos(i + 1);
					if (pos.X < p.X) pos.X = p.X;
					if (pos.Y < p.Y) pos.Y = p.Y;
				}
				text.Origin = Point.From(pos * (value / 100));
				text.DisplayedString = text.DisplayedString.Remove(Text.Length - 1, 1);
			}
		}
		public uint CharacterSize
		{
			get { return text.CharacterSize; }
			set
			{
				text.CharacterSize = value;
				Position = position;
			}
		}
		public string Text
		{
			get { return text.DisplayedString; }
			set { text.DisplayedString = value; OriginPercent = originPercent; }
		}
		public Color Color
		{
			get { return Color.To(text.FillColor); }
			set { text.FillColor = Color.From(value); }
		}
		public Color OutlineColor
		{
			get { return Color.To(text.OutlineColor); }
			set { text.OutlineColor = Color.From(value); }
		}
		public double OutlineThickness
		{
			get { return text.OutlineThickness; }
			set { text.OutlineThickness = (float)value; }
		}
		private Size spacing;
		public Size Spacing
		{
			get { return spacing; }
			set
			{
				spacing = value;
				text.LetterSpacing = (float)value.Width / 4;
				text.LineSpacing = (float)value.Height / 16 + (float)CharacterSize / 112;
			}
		}

		public TextComponent(TransformComponent transformComponent, string fontPath = "folder/font.ttf")
		{
			transform = transformComponent;
			//Effects = new(this);
			text = new();
			FontPath = fontPath;
			text.DisplayedString = "Hello World!\nHello World!Hello World!";
			CharacterSize = 64;
			Color = Color.White;
			OutlineColor = Color.Black;
			Position = new Point();
			Spacing = new Size(4, 4);
		}
		public void Draw(Camera camera)
		{
			if (Window.DrawNotAllowed() || IsHidden || text == null ||
				text.Font == null || transform == null) return;

			var rend = new RenderTexture((uint)transform.Size.Width, (uint)transform.Size.Height);
			//rend.SetView(camera.view);
			rend.Clear(new SFML.Graphics.Color(255, 0, 0));
			rend.Draw(text);
			rend.Display();
			var sprite = new Sprite(rend.Texture)
			{
				Position = Point.From(transform.Position),
				Rotation = (float)transform.Angle,
				Origin = new Vector2f(
					(float)(transform.Size.Width * (BoxOriginPercent.X / 100)),
					(float)(transform.Size.Height * (BoxOriginPercent.Y / 100)))
			};
			camera.rendTexture.Draw(sprite);//, new RenderStates(Effects.shader));

			rend.Dispose();
			sprite.Dispose();
		}
		//public void DrawBounds(Camera camera, double thickness, Color color)
		//{
		//	var b = Text.text.GetGlobalBounds();
		//	var c = Color.From(color);
		//	var thf = (float)thickness;
		//	var off = 16;
		//	var left = new Vertex[]
		//	{
		//		new Vertex(new Vector2f(b.Left - thf - off, b.Top - thf), c),
		//		new Vertex(new Vector2f(b.Left + thf - off, b.Top - thf), c),
		//		new Vertex(new Vector2f(b.Left + thf - off, b.Top + thf + b.Height), c),
		//		new Vertex(new Vector2f(b.Left - thf - off, b.Top + thf + b.Height), c),
		//	};
		//	var right = new Vertex[]
		//	{
		//		new Vertex(new Vector2f(b.Left - thf - off + b.Width, b.Top - thf), c),
		//		new Vertex(new Vector2f(b.Left + thf - off + b.Width, b.Top - thf), c),
		//		new Vertex(new Vector2f(b.Left + thf - off + b.Width, b.Top + thf + b.Height), c),
		//		new Vertex(new Vector2f(b.Left - thf - off + b.Width, b.Top + thf + b.Height), c),
		//	};
		//	var up = new Vertex[]
		//	{
		//		new Vertex(new Vector2f(b.Left - thf - off, b.Top - thf), c),
		//		new Vertex(new Vector2f(b.Left + thf - off + b.Width, b.Top - thf), c),
		//		new Vertex(new Vector2f(b.Left + thf - off + b.Width, b.Top + thf), c),
		//		new Vertex(new Vector2f(b.Left - thf - off, b.Top + thf), c),
		//	};
		//	var bot = new Vertex[]
		//	{
		//		new Vertex(new Vector2f(b.Left - thf - off, b.Top - thf + b.Height), c),
		//		new Vertex(new Vector2f(b.Left + thf - off + b.Width, b.Top - thf + b.Height), c),
		//		new Vertex(new Vector2f(b.Left + thf - off + b.Width, b.Top + thf + b.Height), c),
		//		new Vertex(new Vector2f(b.Left - thf - off, b.Top + thf + b.Height), c),
		//	};
		//	camera.rendTexture.Draw(left, PrimitiveType.Quads);
		//	camera.rendTexture.Draw(right, PrimitiveType.Quads);
		//	camera.rendTexture.Draw(up, PrimitiveType.Quads);
		//	camera.rendTexture.Draw(bot, PrimitiveType.Quads);
		//}
	}
}
