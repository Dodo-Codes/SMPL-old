using SFML.Graphics;
using SFML.System;
using static SMPL.Events;

namespace SMPL
{
	public class ComponentText
	{
		private readonly uint creationFrame;
		private readonly double rand;
		internal Component2D transform;
		//public Effects Effects { get; set; }
		internal SFML.Graphics.Text text;

		private Point position, lastFramePos;
		public Point Position
		{
			get { return position; }
			set
			{
				if (position == value) return;

				var delta = value - position;
				position = value;

				text.Position = Point.From(value) + GetOffset();

				var n = D(instances); foreach (var kvp in n) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++)
						e[i].OnTextMoveSetup(this, delta); }
				var n1 = D(instances); foreach (var kvp in n1) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++)
						e[i].OnTextMove(this, delta); }
			}
		}
		private double lastFrameAng;
		public double Angle
		{
			get { return text.Rotation; }
			set
			{
				if (text.Rotation == value) return;

				var delta = value - text.Rotation;
				text.Rotation = (float)value;

				var n = D(instances); foreach (var kvp in n) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++)
						e[i].OnTextRotateSetup(this, delta); }
				var n1 = D(instances); foreach (var kvp in n1) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++)
						e[i].OnTextRotate(this, delta); }
			}
		}
		private Size lastFrameSc;
		public Size Scale
		{
			get { return Size.To(text.Scale); }
			set
			{
				var v = Size.From(value);
				if (text.Scale == v) return;

				var delta = value - Size.To(text.Scale);
				text.Scale = v;
				var n = D(instances); foreach (var kvp in n) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++)
						e[i].OnTextRescaleSetup(this, delta); }
				var n1 = D(instances); foreach (var kvp in n1) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++)
						e[i].OnTextRescale(this, delta); }
			}
		}
		private string path;
		public string FontPath
		{
			get { return path; }
			private set
			{
				text.Font = File.fonts[value];
				path = value;
			}
		}
		private bool isHidden;
		public bool IsHidden
		{
			get { return isHidden; }
         set
         {
				if (isHidden == value) return;
				isHidden = value;
				var n = D(instances); foreach (var kvp in n) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++)
						e[i].OnTextVisibilityChangeSetup(this); }
				var n1 = D(instances); foreach (var kvp in n1) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++)
						e[i].OnTextVisibilityChange(this); }
			}
		}
		private Point originPercent, lastFrameOrPer;
		public Point OriginPercent
		{
			get { return originPercent; }
			set
			{
				value.X = Number.Limit(value.X, new Bounds(0, 100));
				value.Y = Number.Limit(value.Y, new Bounds(0, 100));
				if (originPercent == value) return;
				var delta = value - originPercent;
				originPercent = value;
				UpdateOrigin();

				var n = D(instances); foreach (var kvp in n) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++)
						e[i].OnTextOriginateSetup(this, delta); }
				var n1 = D(instances); foreach (var kvp in n1) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++)
						e[i].OnTextOriginate(this, delta); }
			}
		}
		public uint CharacterSize
		{
			get { return text.CharacterSize; }
			set
			{
				if (text.CharacterSize == value) return;
				var delta = value - text.CharacterSize;
				text.CharacterSize = value;
				Position = position;

				var n = D(instances); foreach (var kvp in n) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++)
						e[i].OnTextCharacterResizeSetup(this, (int)delta); }
				var n1 = D(instances); foreach (var kvp in n1) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++)
						e[i].OnTextCharacterResize(this, (int)delta); }
			}
		}
		private string lastFrameText;
		public string Text
		{
			get { return text.DisplayedString; }
			set
			{
				if (text.DisplayedString == value) return;
				var oldStr = text.DisplayedString;
				text.DisplayedString = value;
				UpdateOrigin();

				var n = D(instances); foreach (var kvp in n) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++)
						e[i].OnTextChangeSetup(this, oldStr); }
				var n1 = D(instances); foreach (var kvp in n1) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++)
						e[i].OnTextChange(this, oldStr); }
			}
		}
		private Color bgColor, lastFrameBgCol;
		public Color BackgroundColor
		{
			get { return bgColor; }
			set
			{
				if (bgColor == value) return;
				var delta = bgColor;
				bgColor = value;

				var n = D(instances); foreach (var kvp in n) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++) e[i].OnTextBackgroundRecolorSetup(this, delta); }
				var n1 = D(instances); foreach (var kvp in n1) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++) e[i].OnTextBackgroundRecolor(this, delta); }
			}
		}
		private Color color, lastFrameCol;
		public Color Color
		{
			get { return color; }
			set
			{
				if (color == value) return;
				var delta = Color.To(text.FillColor);
				color = value;
				text.FillColor = Color.From(value);

				var n = D(instances); foreach (var kvp in n) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++)
						e[i].OnTextRecolorSetup(this, delta); }
				var n1 = D(instances); foreach (var kvp in n1) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++)
						e[i].OnTextRecolor(this, delta); }
			}
		}
		private Color outlineColor, lastFrameOutCol;
		public Color OutlineColor
		{
			get { return outlineColor; }
			set
			{
				if (outlineColor == value) return;
				var delta = Color.To(text.OutlineColor);
				outlineColor = value;
				text.OutlineColor = Color.From(value);

				var n = D(instances); foreach (var kvp in n) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++)
						e[i].OnTextOutlineRecolorSetup(this, delta); }
				var n1 = D(instances); foreach (var kvp in n1) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++)
						e[i].OnTextOutlineRecolor(this, delta); }
			}
		}
		private double lastFrameOutW;
		public double OutlineWidth
		{
			get { return text.OutlineThickness; }
			set
			{
				if (text.OutlineThickness == value) return;
				var delta = value - text.OutlineThickness;
				text.OutlineThickness = (float)value;

				var n = D(instances); foreach (var kvp in n) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++)
						e[i].OnTextOutlineResizeSetup(this, delta); }
				var n1 = D(instances); foreach (var kvp in n1) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++)
						e[i].OnTextOutlineResize(this, delta); }
			}
		}
		private Size spacing, lastFrameSp;
		public Size Spacing
		{
			get { return spacing; }
			set
			{
				if (spacing == value) return;
				var delta = value - spacing;
				spacing = value;
				text.LetterSpacing = (float)value.W / 4;
				text.LineSpacing = (float)value.H / 16 + (float)CharacterSize / 112;

				var n = D(instances); foreach (var kvp in n) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++)
						e[i].OnTextSpacingResizeSetup(this, delta); }
				var n1 = D(instances); foreach (var kvp in n1) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++)
						e[i].OnTextSpacingResize(this, delta); }
			}
		}

		private Vector2f GetOffset()
      {
			var t = new SFML.Graphics.Text("12", text.Font);
			t.CharacterSize = text.CharacterSize;
			var d = new Vector2f((t.FindCharacterPos(1) - t.FindCharacterPos(0)).X, 0);
			t.Dispose();
			return d;
		}
		private void UpdateOrigin()
      {
			text.DisplayedString += "\n";
			var pos = new Point();
			for (uint i = 0; i < Text.Length; i++)
			{
				var p = text.FindCharacterPos(i + 1);
				if (pos.X < p.X) pos.X = p.X;
				if (pos.Y < p.Y) pos.Y = p.Y;
			}
			text.Origin = Point.From(pos * (originPercent / 100));
			text.DisplayedString = text.DisplayedString.Remove(Text.Length - 1, 1);
		}

		internal void Update()
      {
			if (Gate.EnterOnceWhile($"{creationFrame}-{rand}-text-pos-start", lastFramePos != position))
			{
				var n = D(instances); foreach (var kvp in n) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++) e[i].OnTextMoveStartSetup(this, position - lastFramePos); }
				var n1 = D(instances); foreach (var kvp in n1) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++) e[i].OnTextMoveStart(this, position - lastFramePos); }
			}
			if (Gate.EnterOnceWhile($"{creationFrame}-{rand}-text-pos-end", lastFramePos == position))
			{
				if (creationFrame + 1 != Time.frameCount)
				{
					var n = D(instances); foreach (var kvp in n) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++) e[i].OnTextMoveEndSetup(this); }
					var n1 = D(instances); foreach (var kvp in n1) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++) e[i].OnTextMoveEnd(this); }
				}
			}
			//=============================
			if (Gate.EnterOnceWhile($"{creationFrame}-{rand}-text-ang-start", lastFrameAng != Angle))
			{
				var n = D(instances); foreach (var kvp in n) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++) e[i].OnTextRotateStartSetup(this, Angle - lastFrameAng); }
				var n1 = D(instances); foreach (var kvp in n1) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++) e[i].OnTextRotateStart(this, Angle - lastFrameAng); }
			}
			if (Gate.EnterOnceWhile($"{creationFrame}-{rand}-text-ang-end", lastFrameAng == Angle))
			{
				if (creationFrame + 1 != Time.frameCount)
				{
					var n = D(instances); foreach (var kvp in n) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++) e[i].OnTextRotateEndSetup(this); }
					var n1 = D(instances); foreach (var kvp in n1) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++) e[i].OnTextRotateEnd(this); }
				}
			}
			//=============================
			if (Gate.EnterOnceWhile($"{creationFrame}-{rand}-text-sc-start", lastFrameSc != Scale))
			{
				var n = D(instances); foreach (var kvp in n) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++) e[i].OnTextRescaleStartSetup(this, Scale - lastFrameSc); }
				var n1 = D(instances); foreach (var kvp in n1) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++) e[i].OnTextRescaleStart(this, Scale - lastFrameSc); }
			}
			if (Gate.EnterOnceWhile($"{creationFrame}-{rand}-text-sc-end", lastFrameSc == Scale))
			{
				if (creationFrame + 1 != Time.frameCount)
				{
					var n = D(instances); foreach (var kvp in n) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++) e[i].OnTextRescaleEndSetup(this); }
					var n1 = D(instances); foreach (var kvp in n1) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++) e[i].OnTextRescaleEnd(this); }
				}
			}
			//=============================
			if (Gate.EnterOnceWhile($"{creationFrame}-{rand}-text-orper-start", lastFrameOrPer != originPercent))
			{
				var n = D(instances); foreach (var kvp in n) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++) e[i].OnTextOriginateStartSetup(this, originPercent - lastFrameOrPer); }
				var n1 = D(instances); foreach (var kvp in n1) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++) e[i].OnTextOriginateStart(this, originPercent - lastFrameOrPer); }
			}
			if (Gate.EnterOnceWhile($"{creationFrame}-{rand}-text-orper-end", lastFrameOrPer == originPercent))
			{
				if (creationFrame + 1 != Time.frameCount)
				{
					var n = D(instances); foreach (var kvp in n) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++) e[i].OnTextOriginateEndSetup(this); }
					var n1 = D(instances); foreach (var kvp in n1) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++) e[i].OnTextOriginateEnd(this); }
				}
			}
			//=============================
			if (Gate.EnterOnceWhile($"{creationFrame}-{rand}-text-change-start", lastFrameText != Text))
			{
				var n = D(instances); foreach (var kvp in n) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++) e[i].OnTextChangeStartSetup(this, lastFrameText); }
				var n1 = D(instances); foreach (var kvp in n1) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++) e[i].OnTextChangeStart(this, lastFrameText); }
			}
			if (Gate.EnterOnceWhile($"{creationFrame}-{rand}-text-change-end", lastFrameText == Text))
			{
				if (creationFrame + 1 != Time.frameCount)
				{
					var n = D(instances); foreach (var kvp in n) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++) e[i].OnTextChangeEndSetup(this); }
					var n1 = D(instances); foreach (var kvp in n1) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++) e[i].OnTextChangeEnd(this); }
				}
			}
			//=============================
			if (Gate.EnterOnceWhile($"{creationFrame}-{rand}-text-bgCol-start", lastFrameBgCol != bgColor))
			{
				var n = D(instances); foreach (var kvp in n) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++) e[i].OnTextBackgroundRecolorStartSetup(this, bgColor - lastFrameBgCol); }
				var n1 = D(instances); foreach (var kvp in n1) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++) e[i].OnTextBackgroundRecolorStart(this, bgColor - lastFrameBgCol); }
			}
			if (Gate.EnterOnceWhile($"{creationFrame}-{rand}-text-bgCol-end", lastFrameBgCol == bgColor))
			{
				if (creationFrame + 1 != Time.frameCount)
				{
					var n = D(instances); foreach (var kvp in n) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++) e[i].OnTextBackgroundRecolorEndSetup(this); }
					var n1 = D(instances); foreach (var kvp in n1) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++) e[i].OnTextBackgroundRecolorEnd(this); }
				}
			}
			//=============================
			if (Gate.EnterOnceWhile($"{creationFrame}-{rand}-text-col-start", lastFrameCol != color))
			{
				var n = D(instances); foreach (var kvp in n) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++) e[i].OnTextRecolorStartSetup(this, color - lastFrameCol); }
				var n1 = D(instances); foreach (var kvp in n1) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++) e[i].OnTextRecolorStart(this, color - lastFrameCol); }
			}
			if (Gate.EnterOnceWhile($"{creationFrame}-{rand}-text-col-end", lastFrameCol == color))
			{
				if (creationFrame + 1 != Time.frameCount)
				{
					var n = D(instances); foreach (var kvp in n) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++) e[i].OnTextRecolorEndSetup(this); }
					var n1 = D(instances); foreach (var kvp in n1) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++) e[i].OnTextRecolorEnd(this); }
				}
			}
			//=============================
			if (Gate.EnterOnceWhile($"{creationFrame}-{rand}-text-outcol-start", lastFrameOutCol != outlineColor))
			{
				var n = D(instances); foreach (var kvp in n) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++) e[i].OnTextOutlineRecolorStartSetup(this, outlineColor - lastFrameOutCol); }
				var n1 = D(instances); foreach (var kvp in n1) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++) e[i].OnTextOutlineRecolorStart(this, outlineColor - lastFrameOutCol); }
			}
			if (Gate.EnterOnceWhile($"{creationFrame}-{rand}-text-outcol-end", lastFrameOutCol == outlineColor))
			{
				if (creationFrame + 1 != Time.frameCount)
				{
					var n = D(instances); foreach (var kvp in n) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++) e[i].OnTextOutlineRecolorEndSetup(this); }
					var n1 = D(instances); foreach (var kvp in n1) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++) e[i].OnTextOutlineRecolorEnd(this); }
				}
			}
			//=============================
			if (Gate.EnterOnceWhile($"{creationFrame}-{rand}-text-outw-start", lastFrameOutW != OutlineWidth))
			{
				var n = D(instances); foreach (var kvp in n) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++) e[i].OnTextOutlineResizeStartSetup(this, OutlineWidth - lastFrameOutW); }
				var n1 = D(instances); foreach (var kvp in n1) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++) e[i].OnTextOutlineResizeStart(this, OutlineWidth - lastFrameOutW); }
			}
			if (Gate.EnterOnceWhile($"{creationFrame}-{rand}-text-outw-end", lastFrameOutW == OutlineWidth))
			{
				if (creationFrame + 1 != Time.frameCount)
				{
					var n = D(instances); foreach (var kvp in n) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++) e[i].OnTextOutlineResizeEndSetup(this); }
					var n1 = D(instances); foreach (var kvp in n1) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++) e[i].OnTextOutlineResizeEnd(this); }
				}
			}
			//=============================
			if (Gate.EnterOnceWhile($"{creationFrame}-{rand}-text-sp-start", lastFrameSp != spacing))
			{
				var n = D(instances); foreach (var kvp in n) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++) e[i].OnTextSpacingResizeStartSetup(this, spacing - lastFrameSp); }
				var n1 = D(instances); foreach (var kvp in n1) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++) e[i].OnTextSpacingResizeStart(this, spacing - lastFrameSp); }
			}
			if (Gate.EnterOnceWhile($"{creationFrame}-{rand}-text-sp-end", lastFrameSp == spacing))
			{
				if (creationFrame + 1 != Time.frameCount)
				{
					var n = D(instances); foreach (var kvp in n) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++) e[i].OnTextSpacingResizeEndSetup(this); }
					var n1 = D(instances); foreach (var kvp in n1) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++) e[i].OnTextSpacingResizeEnd(this); }
				}
			}
			//=============================
			lastFramePos = position;
			lastFrameAng = Angle;
			lastFrameSc = Scale;
			lastFrameOrPer = originPercent;
			lastFrameCol = color;
			lastFrameBgCol = bgColor;
			lastFrameOutCol = outlineColor;
			lastFrameOutW = OutlineWidth;
			lastFrameSp = spacing;
			lastFrameText = Text;
		}

		public ComponentText(Component2D component2D, string fontPath = "folder/font.ttf")
		{
			transform = component2D;
			texts.Add(this);
			creationFrame = Time.FrameCount;
			rand = Number.Random(new Bounds(-9999, 9999), 5);
			//Effects = new(this);
			text = new();
			if (File.fonts.ContainsKey(fontPath) == false)
			{
				Debug.LogError(1, $"The font at '{fontPath}' is not loaded.\n" +
					$"Use '{nameof(File)}.{nameof(File.LoadAsset)} ({nameof(File)}.{nameof(File.Asset)}." +
					$"{nameof(File.Asset.Font)}, \"{fontPath}\")' to load it.");
				return;
			}
			FontPath = fontPath;
			text.DisplayedString = "Hello World!";
			text.CharacterSize = 64;
			color = Color.White;
			outlineColor = Color.Black;
			text.FillColor = Color.From(Color.White);
			text.OutlineColor = Color.From(Color.Black);
			text.Position = GetOffset();
			spacing = new Size(4, 4);
			text.LetterSpacing = 1;
			text.LineSpacing = (float)4 / 16 + (float)CharacterSize / 112;
			lastFrameSc = new Size(1, 1);

			var n = D(instances); foreach (var kvp in n) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++)
					e[i].OnTextCreateSetup(this); }
			var n1 = D(instances); foreach (var kvp in n1) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++)
					e[i].OnTextCreate(this); }
		}
		public void Draw(Camera camera)
		{
			if (Window.DrawNotAllowed() || IsHidden || text == null ||
				text.Font == null || transform == null) return;

			var rend = new RenderTexture((uint)transform.Size.W, (uint)transform.Size.H);
			//rend.SetView(camera.view);
			rend.Clear(new SFML.Graphics.Color(Color.From(bgColor)));
			rend.Draw(text);
			rend.Display();
			var sprite = new Sprite(rend.Texture)
			{
				Position = Point.From(transform.Position),
				Rotation = (float)transform.Angle,
				Origin = new Vector2f(
					(float)(transform.Size.W * (transform.OriginPercent.X / 100)),
					(float)(transform.Size.H * (transform.OriginPercent.Y / 100)))
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
