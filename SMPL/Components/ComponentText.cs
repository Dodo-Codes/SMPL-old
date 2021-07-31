using SFML.Graphics;
using SFML.System;
using System.Collections.Generic;
using static SMPL.Events;

namespace SMPL
{
	public class ComponentText : ComponentVisual
	{
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

				transform.text.Position = Point.From(value);

				var n = D(instances); foreach (var kvp in n) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++)
						e[i].OnTextMoveSetup(this, delta); }
				var n1 = D(instances); foreach (var kvp in n1) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++)
						e[i].OnTextMove(this, delta); }
			}
		}
		private double lastFrameAng;
		public double Angle
		{
			get { return transform.text.Rotation; }
			set
			{
				if (transform.text.Rotation == value) return;

				var delta = value - transform.text.Rotation;
				transform.text.Rotation = (float)value;

				var n = D(instances); foreach (var kvp in n) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++)
						e[i].OnTextRotateSetup(this, delta); }
				var n1 = D(instances); foreach (var kvp in n1) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++)
						e[i].OnTextRotate(this, delta); }
			}
		}
		private Size lastFrameSc;
		public Size Scale
		{
			get { return Size.To(transform.text.Scale); }
			set
			{
				var v = Size.From(value);
				if (transform.text.Scale == v) return;

				var delta = value - Size.To(transform.text.Scale);
				transform.text.Scale = v;
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
				transform.text.Font = File.fonts[value];
				path = value;
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
			get { return transform.text.CharacterSize; }
			set
			{
				if (transform.text.CharacterSize == value) return;
				var delta = value - transform.text.CharacterSize;
				transform.text.CharacterSize = value;
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
			get { return transform.text.DisplayedString; }
			set
			{
				if (transform.text.DisplayedString == value) return;
				var oldStr = transform.text.DisplayedString;
				transform.text.DisplayedString = value;
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
				transform.text.LetterSpacing = (float)value.W / 4;
				transform.text.LineSpacing = (float)value.H / 16 + (float)CharacterSize / 112;

				var n = D(instances); foreach (var kvp in n) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++)
						e[i].OnTextSpacingResizeSetup(this, delta); }
				var n1 = D(instances); foreach (var kvp in n1) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++)
						e[i].OnTextSpacingResize(this, delta); }
			}
		}

		private void UpdateOrigin()
      {
			transform.text.DisplayedString += "\n";
			var pos = new Point();
			for (uint i = 0; i < Text.Length; i++)
			{
				var p = transform.text.FindCharacterPos(i + 1);
				if (pos.X < p.X) pos.X = p.X;
				if (pos.Y < p.Y) pos.Y = p.Y;
			}
			transform.text.Origin = Point.From(pos * (originPercent / 100));
			transform.text.DisplayedString = transform.text.DisplayedString.Remove(Text.Length - 1, 1);
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
			: base(component2D)
		{
			if (File.fonts.ContainsKey(fontPath) == false)
			{
				Debug.LogError(1, $"The font at '{fontPath}' is not loaded.\n" +
					$"Use '{nameof(File)}.{nameof(File.LoadAsset)} ({nameof(File)}.{nameof(File.Asset)}." +
					$"{nameof(File.Asset.Font)}, \"{fontPath}\")' to load it.");
				return;
			}
			texts.Add(this);
			transform.text.DisplayedString = "Hello World!";
			transform.text.CharacterSize = 64;
			transform.text.LetterSpacing = 1;
			transform.text.LineSpacing = (float)4 / 16 + (float)CharacterSize / 112;
			transform.text.FillColor = Color.From(Color.White);
			transform.text.OutlineColor = Color.From(Color.Black);

			spacing = new Size(4, 4);
			lastFrameText = transform.text.DisplayedString;
			lastFrameSp = spacing;
			lastFrameSc = new Size(1, 1);
			FontPath = fontPath;

			var n = D(instances); foreach (var kvp in n) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++)
					e[i].OnTextCreateSetup(this); }
			var n1 = D(instances); foreach (var kvp in n1) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++)
					e[i].OnTextCreate(this); }
		}
		public override void Draw(Camera camera)
		{
			if (Window.DrawNotAllowed() || masking != null || IsHidden || transform == null || transform.text == null ||
				transform.text.Font == null) return;

			var rend = new RenderTexture((uint)transform.Size.W, (uint)transform.Size.H);
			rend.Clear(new SFML.Graphics.Color(Color.From(BackgroundColor)));
			rend.Draw(transform.text);
			rend.Display();
			var sprite = new Sprite(rend.Texture);
			var drawMaskResult = Effects.DrawMasks(sprite);
			sprite.Texture = drawMaskResult.Texture;

			Effects.shader.SetUniform("Texture", sprite.Texture);
			Effects.shader.SetUniform("RawTexture", rend.Texture);

			transform.sprite.Position = Point.From(transform.LocalPosition);
			transform.sprite.Rotation = (float)transform.LocalAngle;
			sprite.Position = Point.From(transform.Position);
			sprite.Rotation = (float)transform.Angle;
			sprite.Origin = new Vector2f(
					(float)(transform.Size.W * (transform.OriginPercent.X / 100)),
					(float)(transform.Size.H * (transform.OriginPercent.Y / 100)));

			var sc = Family.Parent == null ? new Vector2f(1, 1) : Family.Parent.transform.sprite.Scale;
			sprite.Scale = new Vector2f(1 / sc.X, 1 / sc.Y);
			camera.rendTexture.Draw(sprite, GetRenderStates());

			drawMaskResult.Dispose();
			rend.Dispose();
			sprite.Dispose();
		}
	}
}
