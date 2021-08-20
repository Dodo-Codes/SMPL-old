using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;

namespace SMPL
{
	public class ComponentText : ComponentVisual
	{
		internal static List<ComponentText> texts = new();

		private static event Events.ParamsTwo<ComponentText, string> OnCreate, OnTextChange;
		private static event Events.ParamsTwo<ComponentText, ComponentIdentity<ComponentText>> OnIdentityChange;
		private static event Events.ParamsTwo<ComponentText, Color> OnBackgroundColorChange, OnBackgroundColorChangeStart;
		private static event Events.ParamsTwo<ComponentText, Point> OnPositionChange, OnOriginPercentChange, OnPositionChangeStart,
			OnOriginPercentChangeStart;
		private static event Events.ParamsTwo<ComponentText, double> OnAngleChange, OnAngleChangeStart;
		private static event Events.ParamsTwo<ComponentText, Size> OnScaleChange, OnScaleChangeStart;
		private static event Events.ParamsTwo<ComponentText, uint> OnCharacterSizeChange;
		private static event Events.ParamsTwo<ComponentText, Size> OnSpacingChange, OnSpacingChangeStart;
		private static event Events.ParamsOne<ComponentText> OnVisibilityChange, OnBackgroundColorChangeEnd, OnPositionChangeEnd,
			OnOriginPercentChangeEnd, OnAngleChangeEnd, OnScaleChangeEnd, OnSpacingChangeEnd;
		private static event Events.ParamsTwo<ComponentText, ComponentFamily> OnFamilyChange;
		private static event Events.ParamsTwo<ComponentText, ComponentEffects> OnEffectsChange;

		public static class CallWhen
		{
			public static void Create(Action<ComponentText, string> method, uint order = uint.MaxValue) =>
				OnCreate = Events.Add(OnCreate, method, order);
			public static void IdentityChange(Action<ComponentText, ComponentIdentity<ComponentText>> method,
				uint order = uint.MaxValue) => OnIdentityChange = Events.Add(OnIdentityChange, method, order);
			public static void BackgroundColorChangeStart(Action<ComponentText, Color> method, uint order = uint.MaxValue) =>
				OnBackgroundColorChangeStart = Events.Add(OnBackgroundColorChangeStart, method, order);
			public static void BackgroundColorChange(Action<ComponentText, Color> method, uint order = uint.MaxValue) =>
				OnBackgroundColorChange = Events.Add(OnBackgroundColorChange, method, order);
			public static void BackgroundColorChangeEnd(Action<ComponentText> method, uint order = uint.MaxValue) =>
				OnBackgroundColorChangeEnd = Events.Add(OnBackgroundColorChangeEnd, method, order);
			public static void PositionChangeStart(Action<ComponentText, Point> method, uint order = uint.MaxValue) =>
				OnPositionChangeStart = Events.Add(OnPositionChangeStart, method, order);
			public static void PositionChange(Action<ComponentText, Point> method, uint order = uint.MaxValue) =>
				OnPositionChange = Events.Add(OnPositionChange, method, order);
			public static void PositionChangeEnd(Action<ComponentText> method, uint order = uint.MaxValue) =>
				OnPositionChangeEnd = Events.Add(OnPositionChangeEnd, method, order);
			public static void AngleChangeStart(Action<ComponentText, double> method, uint order = uint.MaxValue) =>
				OnAngleChangeStart = Events.Add(OnAngleChangeStart, method, order);
			public static void AngleChange(Action<ComponentText, double> method, uint order = uint.MaxValue) =>
				OnAngleChange = Events.Add(OnAngleChange, method, order);
			public static void AngleChangeEnd(Action<ComponentText> method, uint order = uint.MaxValue) =>
				OnAngleChangeEnd = Events.Add(OnAngleChangeEnd, method, order);
			public static void ScaleChangeStart(Action<ComponentText, Size> method, uint order = uint.MaxValue) =>
				OnScaleChangeStart = Events.Add(OnScaleChangeStart, method, order);
			public static void ScaleChange(Action<ComponentText, Size> method, uint order = uint.MaxValue) =>
				OnScaleChange = Events.Add(OnScaleChange, method, order);
			public static void ScaleChangeEnd(Action<ComponentText> method, uint order = uint.MaxValue) =>
				OnScaleChangeEnd = Events.Add(OnScaleChangeEnd, method, order);
			public static void OriginPercentChangeStart(Action<ComponentText, Point> method, uint order = uint.MaxValue) =>
				OnOriginPercentChangeStart = Events.Add(OnOriginPercentChangeStart, method, order);
			public static void OriginPercentChange(Action<ComponentText, Point> method, uint order = uint.MaxValue) =>
				OnOriginPercentChange = Events.Add(OnOriginPercentChange, method, order);
			public static void OriginPercentChangeEnd(Action<ComponentText> method, uint order = uint.MaxValue) =>
				OnOriginPercentChangeEnd = Events.Add(OnOriginPercentChangeEnd, method, order);
			public static void TextChange(Action<ComponentText, string> method, uint order = uint.MaxValue) =>
				OnTextChange = Events.Add(OnTextChange, method, order);
			public static void CharacterSizeChange(Action<ComponentText, uint> method, uint order = uint.MaxValue) =>
				OnCharacterSizeChange = Events.Add(OnCharacterSizeChange, method, order);
			public static void SpacingChangeStart(Action<ComponentText, Size> method, uint order = uint.MaxValue) =>
				OnSpacingChangeStart = Events.Add(OnSpacingChangeStart, method, order);
			public static void SpacingChange(Action<ComponentText, Size> method, uint order = uint.MaxValue) =>
				OnSpacingChange = Events.Add(OnSpacingChange, method, order);
			public static void SpacingChangeEnd(Action<ComponentText> method, uint order = uint.MaxValue) =>
				OnSpacingChangeEnd = Events.Add(OnSpacingChangeEnd, method, order);
			public static void VisibilityChange(Action<ComponentText> method, uint order = uint.MaxValue) =>
				OnVisibilityChange = Events.Add(OnVisibilityChange, method, order);
			public static void FamilyChange(Action<ComponentText, ComponentFamily> method, uint order = uint.MaxValue) =>
				OnFamilyChange = Events.Add(OnFamilyChange, method, order);
			public static void EffectsChange(Action<ComponentText, ComponentEffects> method, uint order = uint.MaxValue) =>
				OnEffectsChange = Events.Add(OnEffectsChange, method, order);
		}

		private ComponentIdentity<ComponentText> identity;
		public ComponentIdentity<ComponentText> Identity
		{
			get { return identity; }
			set
			{
				if (identity == value || (Debug.CurrentMethodIsCalledByUser && IsCurrentlyAccessible() == false)) return;
				var prev = identity;
				identity = value;
				OnIdentityChange?.Invoke(this, prev);
			}
		}

		private Color bgColor, lastFrameBgCol;
		public Color BackgroundColor
		{
			get { return bgColor; }
			set
			{
				if (bgColor == value || IsNotLoaded() ||
					(Debug.CurrentMethodIsCalledByUser && IsCurrentlyAccessible() == false)) return;
				var prev = bgColor;
				bgColor = value;
				OnBackgroundColorChange?.Invoke(this, prev);
			}
		}
		private Point position, lastFramePos;
		public Point Position
		{
			get { return position; }
			set
			{
				if (position == value || IsNotLoaded() ||
					(Debug.CurrentMethodIsCalledByUser && IsCurrentlyAccessible() == false)) return;
				var prev = position;
				position = value;
				transform.text.Position = Point.From(value);
				if (Debug.CurrentMethodIsCalledByUser == false) return;
				OnPositionChange?.Invoke(this, prev);
			}
		}
		private double angle, lastFrameAng;
		public double Angle
		{
			get { return angle; }
			set
			{
				if (Angle == value || IsNotLoaded() ||
					(Debug.CurrentMethodIsCalledByUser && IsCurrentlyAccessible() == false)) return;
				var prev = Angle;
				angle = value;
				transform.text.Rotation = (float)value;
				if (Debug.CurrentMethodIsCalledByUser == false) return;
				OnAngleChange?.Invoke(this, prev);
			}
		}
		private Size scale, lastFrameSc;
		public Size Scale
		{
			get { return scale; }
			set
			{
				var v = Size.From(value);
				if (transform.text.Scale == v || IsNotLoaded() ||
					(Debug.CurrentMethodIsCalledByUser && IsCurrentlyAccessible() == false)) return;
				var delta = value - Size.To(transform.text.Scale);
				scale = value;
				transform.text.Scale = v;
				if (Debug.CurrentMethodIsCalledByUser == false) return;
				OnScaleChange?.Invoke(this, scale);
			}
		}
		private Point originPercent, lastFrameOrPer;
		public Point OriginPercent
		{
			get { return originPercent; }
			set
			{
				if (Debug.CurrentMethodIsCalledByUser && IsCurrentlyAccessible() == false) return;
				value.X = Number.Limit(value.X, new Bounds(0, 100));
				value.Y = Number.Limit(value.Y, new Bounds(0, 100));
				if (originPercent == value || IsNotLoaded()) return;
				var prev = originPercent;
				originPercent = value;
				UpdateOrigin();
				if (Debug.CurrentMethodIsCalledByUser == false) return;
				OnOriginPercentChange?.Invoke(this, prev);
			}
		}

		public string FontPath { get; private set; }
		public string Text
		{
			get { return transform.text.DisplayedString; }
			set
			{
				if (transform.text.DisplayedString == value || IsNotLoaded() ||
					(Debug.CurrentMethodIsCalledByUser && IsCurrentlyAccessible() == false)) return;
				var prev = transform.text.DisplayedString;
				transform.text.DisplayedString = value;
				UpdateOrigin();
				OnTextChange?.Invoke(this, prev);
			}
		}
		public uint CharacterSize
		{
			get { return transform.text.CharacterSize; }
			set
			{
				if (transform.text.CharacterSize == value || IsNotLoaded() ||
					(Debug.CurrentMethodIsCalledByUser && IsCurrentlyAccessible() == false)) return;
				var prev = transform.text.CharacterSize;
				transform.text.CharacterSize = value;
				Position = position;
				OnCharacterSizeChange?.Invoke(this, prev);
			}
		}
		private Size spacing, lastFrameSp;
		public Size Spacing
		{
			get { return spacing; }
			set
			{
				if (spacing == value || IsNotLoaded() ||
					(Debug.CurrentMethodIsCalledByUser && IsCurrentlyAccessible() == false)) return;
				var prev = spacing;
				spacing = value;
				transform.text.LetterSpacing = (float)value.W / 4;
				transform.text.LineSpacing = (float)value.H / 16 + (float)CharacterSize / 112;
				if (Debug.CurrentMethodIsCalledByUser == false) return;
				OnSpacingChange?.Invoke(this, prev);
			}
		}

		public ComponentText(Component2D component2D, string fontPath = "folder/font.ttf") : base(component2D)
		{
			// fixing the access since the ComponentAccess' constructor depth leads to here => user has no access but this file has
			// in other words - the depth gets 1 deeper with inheritence ([3]User -> [2]Sprite/Text -> [1]Visual -> [0]Access)
			// and usually it goes as [2]User -> [1]Component -> [0]Access
			GrantAccessToFile(Debug.CurrentFilePath(1)); // grant the user access
			DenyAccessToFile(Debug.CurrentFilePath(0)); // abandon ship
			if (File.fonts.ContainsKey(fontPath) == false)
			{
				Debug.LogError(1, $"The font '{fontPath}' is not loaded.\n" +
					$"Use '{nameof(File)}.{nameof(File.LoadAsset)} ({nameof(File)}.{nameof(File.Asset)}." +
					$"{nameof(File.Asset.Font)}, \"{fontPath}\")' to load it.");
				return;
			}
			texts.Add(this);
			transform.text.DisplayedString = "Hello World!";
			transform.text.CharacterSize = 20;
			transform.text.LetterSpacing = 1;
			transform.text.LineSpacing = (float)4 / 16 + (float)CharacterSize / 112;
			transform.text.FillColor = Color.From(Color.White);
			transform.text.OutlineColor = Color.From(Color.Black);

			spacing = new Size(4, 4);
			lastFrameSp = spacing;
			lastFrameSc = new Size(1, 1);

			FontPath = fontPath;
			transform.text.Font = File.fonts[fontPath];
			HasLoadedAssetFile = true;

			OnCreate?.Invoke(this, fontPath);
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
			if (Gate.EnterOnceWhile($"{creationFrame}-{rand}-text-bgCol-start", lastFrameBgCol != bgColor))
				OnBackgroundColorChangeStart?.Invoke(this, lastFrameBgCol);
			if (Gate.EnterOnceWhile($"{creationFrame}-{rand}-text-bgCol-end", lastFrameBgCol == bgColor))
				if (creationFrame + 1 != Performance.frameCount) OnBackgroundColorChangeEnd?.Invoke(this);
			//=============================
			lastFramePos = position;
			lastFrameAng = Angle;
			lastFrameSc = Scale;
			lastFrameOrPer = originPercent;
			lastFrameSp = spacing;
			lastFrameBgCol = bgColor;
		}
		internal static void TriggerOnVisibilityChange(ComponentText instance) => OnVisibilityChange?.Invoke(instance);
		internal static void TriggerOnFamilyChange(ComponentText i, ComponentFamily f) => OnFamilyChange?.Invoke(i, f);
		internal static void TriggerOnEffectsChange(ComponentText i, ComponentEffects e) => OnEffectsChange?.Invoke(i, e);

		public void Display(ComponentCamera camera)
		{
			if (Debug.CurrentMethodIsCalledByUser && IsCurrentlyAccessible() == false) return;
			if (Window.DrawNotAllowed() || IsNotLoaded() || masking != null || IsHidden || transform == null ||
				transform.text == null || transform.text.Font == null) return;

			var rend = new RenderTexture((uint)Number.Sign(transform.Size.W, false), (uint)Number.Sign(transform.Size.H, false));
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
			camera.rendTexture.Draw(sprite, new RenderStates(Effects.shader));

			drawMaskResult.Dispose();
			rend.Dispose();
			sprite.Dispose();
		}
	}
}
