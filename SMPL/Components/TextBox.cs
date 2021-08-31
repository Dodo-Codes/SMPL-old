using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using SMPL.Data;
using SMPL.Gear;

namespace SMPL.Components
{
	public class TextBox : Visual
	{
		internal static List<TextBox> texts = new();

		private static event Events.ParamsTwo<TextBox, string> OnTextChange, OnFontPathChange;
		private static event Events.ParamsTwo<TextBox, Identity<TextBox>> OnIdentityChange;
		private static event Events.ParamsTwo<TextBox, Point> OnPositionChange, OnOriginPercentChange, OnPositionChangeStart,
			OnOriginPercentChangeStart;
		private static event Events.ParamsTwo<TextBox, double> OnAngleChange, OnAngleChangeStart;
		private static event Events.ParamsTwo<TextBox, Size> OnScaleChange, OnScaleChangeStart;
		private static event Events.ParamsTwo<TextBox, uint> OnCharacterSizeChange;
		private static event Events.ParamsTwo<TextBox, Size> OnSpacingChange, OnSpacingChangeStart;
		private static event Events.ParamsOne<TextBox> OnVisibilityChange, OnPositionChangeEnd, OnCreate, OnBoldChange,
			OnItalicChange, OnOriginPercentChangeEnd, OnAngleChangeEnd, OnScaleChangeEnd, OnSpacingChangeEnd, OnDestroy, OnDisplay,
			OnUnderlineChange, OnStrikeThroughChange;
		private static event Events.ParamsTwo<TextBox, Family> OnFamilyChange;
		private static event Events.ParamsTwo<TextBox, Effects> OnEffectsChange;
		private static event Events.ParamsTwo<TextBox, Area> OnAreaChange;

		public static class CallWhen
		{
			public static void Create(Action<TextBox> method, uint order = uint.MaxValue) =>
				OnCreate = Events.Add(OnCreate, method, order);
			public static void IdentityChange(Action<TextBox, Identity<TextBox>> method,
				uint order = uint.MaxValue) => OnIdentityChange = Events.Add(OnIdentityChange, method, order);
			public static void FontPathChange(Action<TextBox, string> method, uint order = uint.MaxValue) =>
				OnFontPathChange = Events.Add(OnFontPathChange, method, order);
			public static void PositionChangeStart(Action<TextBox, Point> method, uint order = uint.MaxValue) =>
				OnPositionChangeStart = Events.Add(OnPositionChangeStart, method, order);
			public static void PositionChange(Action<TextBox, Point> method, uint order = uint.MaxValue) =>
				OnPositionChange = Events.Add(OnPositionChange, method, order);
			public static void PositionChangeEnd(Action<TextBox> method, uint order = uint.MaxValue) =>
				OnPositionChangeEnd = Events.Add(OnPositionChangeEnd, method, order);
			public static void AngleChangeStart(Action<TextBox, double> method, uint order = uint.MaxValue) =>
				OnAngleChangeStart = Events.Add(OnAngleChangeStart, method, order);
			public static void AngleChange(Action<TextBox, double> method, uint order = uint.MaxValue) =>
				OnAngleChange = Events.Add(OnAngleChange, method, order);
			public static void AngleChangeEnd(Action<TextBox> method, uint order = uint.MaxValue) =>
				OnAngleChangeEnd = Events.Add(OnAngleChangeEnd, method, order);
			public static void ScaleChangeStart(Action<TextBox, Size> method, uint order = uint.MaxValue) =>
				OnScaleChangeStart = Events.Add(OnScaleChangeStart, method, order);
			public static void ScaleChange(Action<TextBox, Size> method, uint order = uint.MaxValue) =>
				OnScaleChange = Events.Add(OnScaleChange, method, order);
			public static void ScaleChangeEnd(Action<TextBox> method, uint order = uint.MaxValue) =>
				OnScaleChangeEnd = Events.Add(OnScaleChangeEnd, method, order);
			public static void OriginPercentChangeStart(Action<TextBox, Point> method, uint order = uint.MaxValue) =>
				OnOriginPercentChangeStart = Events.Add(OnOriginPercentChangeStart, method, order);
			public static void OriginPercentChange(Action<TextBox, Point> method, uint order = uint.MaxValue) =>
				OnOriginPercentChange = Events.Add(OnOriginPercentChange, method, order);
			public static void OriginPercentChangeEnd(Action<TextBox> method, uint order = uint.MaxValue) =>
				OnOriginPercentChangeEnd = Events.Add(OnOriginPercentChangeEnd, method, order);
			public static void TextChange(Action<TextBox, string> method, uint order = uint.MaxValue) =>
				OnTextChange = Events.Add(OnTextChange, method, order);
			public static void CharacterSizeChange(Action<TextBox, uint> method, uint order = uint.MaxValue) =>
				OnCharacterSizeChange = Events.Add(OnCharacterSizeChange, method, order);
			public static void SpacingChangeStart(Action<TextBox, Size> method, uint order = uint.MaxValue) =>
				OnSpacingChangeStart = Events.Add(OnSpacingChangeStart, method, order);
			public static void SpacingChange(Action<TextBox, Size> method, uint order = uint.MaxValue) =>
				OnSpacingChange = Events.Add(OnSpacingChange, method, order);
			public static void SpacingChangeEnd(Action<TextBox> method, uint order = uint.MaxValue) =>
				OnSpacingChangeEnd = Events.Add(OnSpacingChangeEnd, method, order);
			public static void VisibilityChange(Action<TextBox> method, uint order = uint.MaxValue) =>
				OnVisibilityChange = Events.Add(OnVisibilityChange, method, order);
			public static void FamilyChange(Action<TextBox, Family> method, uint order = uint.MaxValue) =>
				OnFamilyChange = Events.Add(OnFamilyChange, method, order);
			public static void EffectsChange(Action<TextBox, Effects> method, uint order = uint.MaxValue) =>
				OnEffectsChange = Events.Add(OnEffectsChange, method, order);
			public static void AreaChange(Action<TextBox, Area> method, uint order = uint.MaxValue) =>
				OnAreaChange = Events.Add(OnAreaChange, method, order);
			public static void BoldChange(Action<TextBox> method, uint order = uint.MaxValue) =>
				OnBoldChange = Events.Add(OnBoldChange, method, order);
			public static void ItalicChange(Action<TextBox> method, uint order = uint.MaxValue) =>
				OnItalicChange = Events.Add(OnItalicChange, method, order);
			public static void UnderlineChange(Action<TextBox> method, uint order = uint.MaxValue) =>
				OnUnderlineChange = Events.Add(OnUnderlineChange, method, order);
			public static void StrikeThroughChange(Action<TextBox> method, uint order = uint.MaxValue) =>
				OnStrikeThroughChange = Events.Add(OnStrikeThroughChange, method, order);
			public static void Destroy(Action<TextBox> method, uint order = uint.MaxValue) =>
				OnDestroy = Events.Add(OnDestroy, method, order);
			public static void Display(Action<TextBox> method, uint order = uint.MaxValue) =>
				OnDisplay = Events.Add(OnDisplay, method, order);
		}

		private bool isDestroyed;
		public bool IsDestroyed
		{
			get { return isDestroyed; }
			set
			{
				if (isDestroyed == value || (Debug.CalledBySMPL == false && IsCurrentlyAccessible() == false)) return;
				isDestroyed = value;

				if (Identity != null) Identity.Dispose();
				Identity = null;
				texts.Remove(this);
				Dispose();
				if (Debug.CalledBySMPL == false) OnDestroy?.Invoke(this);
			}
		}
		private Identity<TextBox> identity;
		public Identity<TextBox> Identity
		{
			get { return AllAccess == Extent.Removed ? default : identity; }
			set
			{
				if (identity == value || (Debug.CalledBySMPL == false && IsCurrentlyAccessible() == false)) return;
				var prev = identity;
				identity = value;
				if (Debug.CalledBySMPL == false) OnIdentityChange?.Invoke(this, prev);
			}
		}

		private Point position, lastFramePos;
		public Point Position
		{
			get { return AllAccess == Extent.Removed ? Point.Invalid : position; }
			set
			{
				if (position == value || (Debug.CalledBySMPL == false && IsCurrentlyAccessible() == false)) return;
				var prev = position;
				position = value;
				if (Debug.CalledBySMPL == false) OnPositionChange?.Invoke(this, prev);
				else lastFramePos = position;
			}
		}
		private double angle, lastFrameAng;
		public double Angle
		{
			get { return AllAccess == Extent.Removed ? double.NaN : angle; }
			set
			{
				if (Angle == value ||
					(Debug.CalledBySMPL == false && IsCurrentlyAccessible() == false)) return;
				var prev = Angle;
				angle = value;
				if (Debug.CalledBySMPL == false) OnAngleChange?.Invoke(this, prev);
				else lastFrameAng = angle;
			}
		}
		private Size scale, lastFrameSc;
		public Size Scale
		{
			get { return AllAccess == Extent.Removed ? Size.Invalid : scale; }
			set
			{
				if (scale == value || (Debug.CalledBySMPL == false && IsCurrentlyAccessible() == false)) return;
				var prev = scale;
				scale = value;
				if (Debug.CalledBySMPL == false) OnScaleChange?.Invoke(this, prev);
				else lastFrameSc = scale;
			}
		}
		private Point originPercent, lastFrameOrPer;
		public Point OriginPercent
		{
			get { return AllAccess == Extent.Removed ? Point.Invalid : originPercent; }
			set
			{
				if (Debug.CalledBySMPL == false && IsCurrentlyAccessible() == false) return;
				value.X = Number.Limit(value.X, new Bounds(0, 100));
				value.Y = Number.Limit(value.Y, new Bounds(0, 100));
				if (originPercent == value) return;
				var prev = originPercent;
				originPercent = value;
				if (Debug.CalledBySMPL == false) OnOriginPercentChange?.Invoke(this, prev);
				else lastFrameOrPer = originPercent;
			}
		}

		private string fontPath;
		public string FontPath
		{
			get { return AllAccess == Extent.Removed ? default : fontPath; }
			set
			{
				if (fontPath == value || (Debug.CalledBySMPL == false && IsCurrentlyAccessible() == false)) return;
				if (File.fonts.ContainsKey(value) == false)
				{
					Debug.LogError(1, $"The font '{value}' is not loaded.\n" +
						$"Use '{nameof(File)}.{nameof(File.LoadAssets)} ({nameof(File)}.{nameof(File.Asset)}." +
						$"{nameof(File.Asset.Font)}, \"{value}\")' to load it.");
					return;
				}
				var prev = fontPath;
				fontPath = value;
				if (Debug.CalledBySMPL == false) OnFontPathChange?.Invoke(this, prev);
			}
		}
		private string displayText;
		public string Text
		{
			get { return AllAccess == Extent.Removed ? default : displayText; }
			set
			{
				if (displayText == value || (Debug.CalledBySMPL == false && IsCurrentlyAccessible() == false)) return;
				var prev = displayText;
				displayText = value;
				if (Debug.CalledBySMPL == false) OnTextChange?.Invoke(this, prev);
			}
		}
		private uint charSize;
		public uint CharacterSize
		{
			get { return AllAccess == Extent.Removed ? default : charSize; }
			set
			{
				if (charSize == value || (Debug.CalledBySMPL == false && IsCurrentlyAccessible() == false)) return;
				var prev = charSize;
				charSize = value;
				if (Debug.CalledBySMPL == false) OnCharacterSizeChange?.Invoke(this, prev);
			}
		}
		private Size spacing, lastFrameSp;
		public Size Spacing
		{
			get { return AllAccess == Extent.Removed ? Size.Invalid : spacing; }
			set
			{
				if (spacing == value || (Debug.CalledBySMPL == false && IsCurrentlyAccessible() == false)) return;
				var prev = spacing;
				spacing = value;
				if (Debug.CalledBySMPL == false) OnSpacingChange?.Invoke(this, prev);
				else lastFrameSp = spacing;
			}
		}
		private bool isBold, isItalic, isUnderlined, isStrikedThrough;
		public bool IsBold
		{
			get { return AllAccess != Extent.Removed && isBold; }
			set
			{
				if (isBold == value || (Debug.CalledBySMPL == false && IsCurrentlyAccessible() == false)) return;
				isBold = value;
				if (Debug.CalledBySMPL == false) OnBoldChange?.Invoke(this);
			}
		}
		public bool IsItalic
		{
			get { return AllAccess != Extent.Removed && isItalic; }
			set
			{
				if (isItalic == value || (Debug.CalledBySMPL == false && IsCurrentlyAccessible() == false)) return;
				isItalic = value;
				if (Debug.CalledBySMPL == false) OnBoldChange?.Invoke(this);
			}
		}
		public bool IsUnderlined
		{
			get { return AllAccess != Extent.Removed && isUnderlined; }
			set
			{
				if (isUnderlined == value || (Debug.CalledBySMPL == false && IsCurrentlyAccessible() == false)) return;
				isUnderlined = value;
				if (Debug.CalledBySMPL == false) OnUnderlineChange?.Invoke(this);
			}
		}
		public bool IsStrikedThrough
		{
			get { return AllAccess != Extent.Removed && isStrikedThrough; }
			set
			{
				if (isStrikedThrough == value || (Debug.CalledBySMPL == false && IsCurrentlyAccessible() == false)) return;
				isStrikedThrough = value;
				if (Debug.CalledBySMPL == false) OnStrikeThroughChange?.Invoke(this);
			}
		}

		public TextBox(string uniqueID) : base()
		{
			if (Identity<TextBox>.CannotCreate(uniqueID)) return;
			// fixing the access since the ComponentAccess' constructor depth leads to here => user has no access but this file has
			// in other words - the depth gets 1 deeper with inheritence ([3]User -> [2]Sprite/Text -> [1]Visual -> [0]Access)
			// and usually it goes as [2]User -> [1]Component -> [0]Access
			accessPaths.Clear(); // abandon ship
			GrantAccessToFile(Debug.CurrentFilePath(1)); // grant the user access

			Identity = new(this, uniqueID);
			texts.Add(this);

			Text = "Hello World!";
			CharacterSize = 64;
			Spacing = new Size(4, 4);
			Scale = new Size(0.3, 0.3);
			OriginPercent = new Point(50, 50);
			//Position = new Point(50, 50);

			OnCreate?.Invoke(this);
		}
		internal void Update()
		{
			if (Gate.EnterOnceWhile($"{creationFrame}-{rand}-text-pos-start", lastFramePos != position))
				OnPositionChangeStart?.Invoke(this, lastFramePos);
			if (Gate.EnterOnceWhile($"{creationFrame}-{rand}-text-pos-end", lastFramePos == position))
				if (creationFrame + 1 != Performance.frameCount) OnPositionChangeEnd?.Invoke(this);
			//=============================
			if (Gate.EnterOnceWhile($"{creationFrame}-{rand}-text-ang-start", lastFrameAng != Angle))
				OnAngleChangeStart?.Invoke(this, lastFrameAng);
			if (Gate.EnterOnceWhile($"{creationFrame}-{rand}-text-ang-end", lastFrameAng == Angle))
				if (creationFrame + 1 != Performance.frameCount) OnAngleChangeEnd?.Invoke(this);
			//=============================
			if (Gate.EnterOnceWhile($"{creationFrame}-{rand}-text-sc-start", lastFrameSc != Scale))
				OnScaleChangeStart?.Invoke(this, lastFrameSc);
			if (Gate.EnterOnceWhile($"{creationFrame}-{rand}-text-sc-end", lastFrameSc == Scale))
				if (creationFrame + 1 != Performance.frameCount) OnScaleChangeEnd?.Invoke(this);
			//=============================
			if (Gate.EnterOnceWhile($"{creationFrame}-{rand}-text-orper-start", lastFrameOrPer != originPercent))
				OnOriginPercentChangeStart?.Invoke(this, lastFrameOrPer);
			if (Gate.EnterOnceWhile($"{creationFrame}-{rand}-text-orper-end", lastFrameOrPer == originPercent))
				if (creationFrame + 1 != Performance.frameCount) OnOriginPercentChangeEnd?.Invoke(this);
			//=============================
			if (Gate.EnterOnceWhile($"{creationFrame}-{rand}-text-sp-start", lastFrameSp != spacing))
				OnSpacingChangeStart?.Invoke(this, lastFrameSp);
			if (Gate.EnterOnceWhile($"{creationFrame}-{rand}-text-sp-end", lastFrameSp == spacing))
				if (creationFrame + 1 != Performance.frameCount) OnSpacingChangeEnd?.Invoke(this);
			//=============================
			lastFramePos = position;
			lastFrameAng = Angle;
			lastFrameSc = Scale;
			lastFrameOrPer = originPercent;
			lastFrameSp = spacing;
		}
		internal static void TriggerOnVisibilityChange(TextBox instance) => OnVisibilityChange?.Invoke(instance);
		internal static void TriggerOnFamilyChange(TextBox i, Family f) => OnFamilyChange?.Invoke(i, f);
		internal static void TriggerOnEffectsChange(TextBox i, Effects e) => OnEffectsChange?.Invoke(i, e);
		internal static void TriggerOnAreaChange(TextBox i, Area a) => OnAreaChange?.Invoke(i, a);
		internal void UpdateAllData()
		{
			if (Text == null || Text.Trim() == "") return;
			Area.text.Font = FontPath == null || File.fonts.ContainsKey(FontPath) == false ? null : File.fonts[FontPath];
			Area.text.DisplayedString = Text;
			Area.text.CharacterSize = CharacterSize;
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
			bounds.Height += CharacterSize / 2;
			var bottomRight = Point.To(new Vector2f(bounds.Left + bounds.Width, bounds.Top + bounds.Height));
			Area.text.Origin = Point.From(bottomRight * (OriginPercent / 100));

			var o = OriginPercent / 100;
			var ownerOrOff = Point.From(new Point(Area.Size.W * o.X, Area.Size.H * o.Y));
			Area.text.Position = Point.From(Position) + ownerOrOff;
			Area.text.Rotation = (float)Angle;
			Area.text.Scale = Size.From(Scale);
		}

		public void Display(Camera camera)
		{
			if (Debug.CalledBySMPL == false && IsCurrentlyAccessible() == false) return;
			if (Window.DrawNotAllowed() || masking != null || IsHidden) return;
			if (Area == null || Area.text == null)
			{
				Debug.LogError(1, $"Cannot display the textBox instance '{Identity.UniqueID}' because it has no Area.\n" +
					$"Make sure the textBox instance has an Area before displaying it.");
				return;
			}

			UpdateAllData();

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
			if (Debug.CalledBySMPL == false) OnDisplay?.Invoke(this);
		}
	}
}
