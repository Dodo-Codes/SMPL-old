using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using SMPL.Data;
using SMPL.Gear;

namespace SMPL.Components
{
	public class Textbox : Visual
	{
		private static event Events.ParamsOne<Textbox> OnDisplay;

		private Point position, originPercent;
		private double angle, charSize;
		private Size scale, spacing;
		private string fontPath, displayText;
		private bool isBold, isItalic, isUnderlined, isStrikedThrough;

		// ===============

		internal static List<Textbox> texts = new();
		internal void UpdateAllData()
		{
			if (Text == null || Text.Trim() == "") return;
			var Area = (Area)PickByUniqueID(AreaUniqueID);
			var Effects = (Effects)PickByUniqueID(EffectsUniqueID);
			Area.text.Font = FontPath == null || Assets.fonts.ContainsKey(FontPath) == false ? null : Assets.fonts[FontPath];
			Area.text.DisplayedString = Text;
			Area.text.CharacterSize = (uint)CharacterSize;
			Area.text.LetterSpacing = (float)Spacing.W / 4f;
			Area.text.LineSpacing = (float)Spacing.H / 16 + (float)CharacterSize / 112;
			Area.text.FillColor = Data.Color.From(Effects == null ? Data.Color.White : Effects.FillColor);
			Area.text.OutlineColor = Data.Color.From(Effects == null ? Data.Color.Black : Effects.OutlineColor);
			Area.text.OutlineThickness = Effects == null ? 0 : (float)Effects.OutlineWidth;

			Area.text.Style = (IsBold ? SFML.Graphics.Text.Styles.Bold : SFML.Graphics.Text.Styles.Regular) |
				(IsItalic ? SFML.Graphics.Text.Styles.Italic : SFML.Graphics.Text.Styles.Regular) |
				(IsUnderlined ? SFML.Graphics.Text.Styles.Underlined : SFML.Graphics.Text.Styles.Regular) |
				(IsStrikedThrough ? SFML.Graphics.Text.Styles.StrikeThrough : SFML.Graphics.Text.Styles.Regular);

			var bounds = Area.text.GetLocalBounds();
			bounds.Height += (uint)(CharacterSize / 2);
			var bottomRight = Point.To(new Vector2f(bounds.Left + bounds.Width, bounds.Top + bounds.Height));
			Area.text.Origin = Point.From(bottomRight * (OriginPercent / 100));

			var o = OriginPercent / 100;
			var ownerOrOff = Point.From(new Point(Area.Size.W * o.X, Area.Size.H * o.Y));
			Area.text.Position = Point.From(Position) + ownerOrOff;
			Area.text.Rotation = (float)Angle;
			Area.text.Scale = Size.From(Scale);
		}

		// =================

		public static class CallWhen
		{
			public static void Display(Action<Textbox> method, uint order = uint.MaxValue) =>
				OnDisplay = Events.Add(OnDisplay, method, order);
		}

		public Point Position
		{
			get { return ErrorIfDestroyed() ? Point.Invalid : position; }
			set { if (ErrorIfDestroyed() == false) position = value; }
		}
		public double Angle
		{
			get { return ErrorIfDestroyed() ? double.NaN : angle; }
			set { if (ErrorIfDestroyed() == false) angle = value; }
		}
		public Size Scale
		{
			get { return ErrorIfDestroyed() ? Size.Invalid : scale; }
			set { if (ErrorIfDestroyed() == false) scale = value; }
		}
		public Point OriginPercent
		{
			get { return ErrorIfDestroyed() ? Point.Invalid : originPercent; }
			set
			{
				if (ErrorIfDestroyed()) return;
				originPercent = new Point(Number.Limit(value.X, new Bounds(0, 100)), Number.Limit(value.Y, new Bounds(0, 100)));
			}
		}
		public string FontPath
		{
			get { return ErrorIfDestroyed() ? default : fontPath; }
			set
			{
				if (ErrorIfDestroyed()) return;
				if (Assets.fonts.ContainsKey(value) == false)
				{
					Assets.NotLoadedError(Assets.Type.Font, value);
					return;
				}
				fontPath = value;
			}
		}
		public string Text
		{
			get { return ErrorIfDestroyed() ? default : displayText; }
			set { if (ErrorIfDestroyed() == false)  displayText = value; }
		}
		public double CharacterSize
		{
			get { return ErrorIfDestroyed() ? double.NaN : charSize; }
			set { if (ErrorIfDestroyed() == false) charSize = value; }
		}
		public Size Spacing
		{
			get { return ErrorIfDestroyed() ? Size.Invalid : spacing; }
			set { if (ErrorIfDestroyed() == false) spacing = value; }
		}
		public bool IsBold
		{
			get { return ErrorIfDestroyed() == false && isBold; }
			set { if (ErrorIfDestroyed() == false) isBold = value; }
		}
		public bool IsItalic
		{
			get { return ErrorIfDestroyed() == false && isItalic; }
			set { if (ErrorIfDestroyed() == false) isItalic = value; }
		}
		public bool IsUnderlined
		{
			get { return ErrorIfDestroyed() == false && isUnderlined; }
			set { if (ErrorIfDestroyed() == false) isUnderlined = value; }
		}
		public bool IsStrikedThrough
		{
			get { return ErrorIfDestroyed() == false && isStrikedThrough; }
			set { if (ErrorIfDestroyed() == false) isStrikedThrough = value; }
		}

		public Textbox(string uniqueID) : base(uniqueID)
		{
			texts.Add(this);

			Text = "Hello World!";
			CharacterSize = 64;
			Spacing = new Size(4, 4);
			Scale = new Size(0.3, 0.3);
			OriginPercent = new Point(50, 50);
			if (cannotCreate) { ErrorAlreadyHasUID(uniqueID); Destroy(); }
		}
		public override void Destroy()
		{
			if (ErrorIfDestroyed()) return;
			texts.Remove(this);
			base.Destroy();
		}

		public void Display(Camera camera)
		{
			if (ErrorIfDestroyed()) return;
			if (Window.DrawNotAllowed() || visualMaskingUID != null || IsHidden) return;
			var Area = (Area)PickByUniqueID(AreaUniqueID);
			if (Area == null || Area.IsDestroyed || Area.text == null)
			{
				Debug.LogError(1, $"Cannot display the textBox instance '{UniqueID}' because it has no Area.\n" +
					$"Make sure the textBox instance has an Area before displaying it.");
				return;
			}

			UpdateAllData();

			var Effects = (Effects)PickByUniqueID(EffectsUniqueID);
			var bgc = Effects == null ? new Data.Color() : Effects.BackgroundColor;
			if (Area.text.Font == null) bgc = Data.Color.White;

			Area.sprite.Position = new Vector2f();
			Area.sprite.Rotation = 0;
			Area.sprite.Scale = new Vector2f(1, 1);
			Area.sprite.Origin = new Vector2f(0, 0);

			var rend = new RenderTexture((uint)Number.Sign(Area.Size.W, false), (uint)Number.Sign(Area.Size.H, false));
			rend.Clear(Data.Color.From(bgc));
			rend.Draw(Area.text, new RenderStates(Area.sprite.Transform));
			var t = Effects == null ? null : new Texture(rend.Texture);

			if (Effects != null)
			{
				Effects.shader.SetUniform("RawTexture", t);
				Effects.DrawMasks(rend);
			}
			
			rend.Display();
			//rend.Texture.Smooth = true;

			var o = Area.OriginPercent / 100;
			var ownerOrOff = Point.From(new Point(Area.Size.W * o.X, Area.Size.H * o.Y));
			Area.sprite.TextureRect = new IntRect(0, 0, (int)Area.Size.W, (int)Area.Size.H);
			Area.sprite.Texture = rend.Texture;
			if (Effects != null) Effects.shader.SetUniform("Texture", Area.sprite.Texture);

			Area.sprite.Position = Point.From(Area.Position);
			Area.sprite.Rotation = (float)Area.Angle;
			Area.sprite.Scale = new Vector2f(1, 1);
			Area.sprite.Origin = new Vector2f(
					(float)(Area.Size.W * (Area.OriginPercent.X / 100)),
					(float)(Area.Size.H * (Area.OriginPercent.Y / 100)));

			if (Effects == null) camera.rendTexture.Draw(Area.sprite);
			else camera.rendTexture.Draw(Area.sprite, new RenderStates(BlendMode.Alpha, Transform.Identity, null, Effects.shader));

			rend.Dispose();
			OnDisplay?.Invoke(this);
		}
	}
}
