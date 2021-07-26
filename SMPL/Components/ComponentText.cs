using SFML.Graphics;
using SFML.System;
using System.Collections.Generic;
using static SMPL.Events;

namespace SMPL
{
	public class ComponentText
	{
		private readonly uint creationFrame;
		private readonly double rand;
		internal Component2D transform;
		internal SFML.Graphics.Text text;

		public Effects Effects { get; set; }

		internal ComponentSprite maskingSprite;
		internal ComponentText maskingText;
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

		internal Vector2f GetOffset()
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
			if (Gate.EnterOnceWhile($"{creationFrame}-{rand}-text-bgCol-start", lastFrameBgCol != bgColor))
			{
				var delta = bgColor - lastFrameBgCol;
				var n = D(instances); foreach (var kvp in n) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++) e[i].OnTextBackgroundRecolorStartSetup(this, delta); }
				var n1 = D(instances); foreach (var kvp in n1) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++) e[i].OnTextBackgroundRecolorStart(this, delta); }
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
			lastFramePos = position;
			lastFrameAng = Angle;
			lastFrameSc = Scale;
			lastFrameOrPer = originPercent;
			lastFrameSp = spacing;
			lastFrameText = Text;
			lastFrameBgCol = bgColor;
		}

		public ComponentText(Component2D component2D, string fontPath = "folder/font.ttf")
		{
			if (File.fonts.ContainsKey(fontPath) == false)
			{
				Debug.LogError(1, $"The font at '{fontPath}' is not loaded.\n" +
					$"Use '{nameof(File)}.{nameof(File.LoadAsset)} ({nameof(File)}.{nameof(File.Asset)}." +
					$"{nameof(File.Asset.Font)}, \"{fontPath}\")' to load it.");
				return;
			}
			transform = component2D;
			text = new();
			texts.Add(this);
			Effects = new(this);
			creationFrame = Time.FrameCount;
			rand = Number.Random(new Bounds(-9999, 9999), 5);
			FontPath = fontPath;
			text.DisplayedString = "Hello World!";
			lastFrameText = text.DisplayedString;
			text.CharacterSize = 64;
			text.Position = GetOffset();
			spacing = new Size(4, 4);
			lastFrameSp = spacing;
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
			var isMask = maskingSprite != null || maskingText != null;
			if (Window.DrawNotAllowed() || isMask || IsHidden || text == null ||
				text.Font == null || transform == null) return;

			var rend = new RenderTexture((uint)transform.Size.W, (uint)transform.Size.H);
			rend.Clear(new SFML.Graphics.Color(Color.From(BackgroundColor)));
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
			var drawMaskResult = Effects.DrawMasks(sprite);
			sprite.Texture = drawMaskResult.Texture;

			Effects.shader.SetUniform("Texture", sprite.Texture);
			Effects.shader.SetUniform("RawTexture", rend.Texture);

			camera.rendTexture.Draw(sprite, new RenderStates(Effects.shader));

			drawMaskResult.Dispose();
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
