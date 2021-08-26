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
		private static event Events.ParamsOne<TextBox> OnVisibilityChange, OnPositionChangeEnd, OnCreate,
			OnOriginPercentChangeEnd, OnAngleChangeEnd, OnScaleChangeEnd, OnSpacingChangeEnd, OnDestroy, OnDisplay;
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

				if (Identity != null)
				{
					Identity<TextBox>.uniqueIDs.Remove(Identity.UniqueID);
					if (Identity<TextBox>.objTags.ContainsKey(this))
					{
						Identity.RemoveAllTags();
						Identity<TextBox>.objTags.Remove(this);
					}
					Identity.instance = null;
					Identity = null;
				}
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
				Area.text.Position = Point.From(value);
				if (Debug.CalledBySMPL == false) OnPositionChange?.Invoke(this, prev);
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
				Area.text.Rotation = (float)value;
				if (Debug.CalledBySMPL == false) OnAngleChange?.Invoke(this, prev);
			}
		}
		private Size scale, lastFrameSc;
		public Size Scale
		{
			get { return AllAccess == Extent.Removed ? Size.Invalid : scale; }
			set
			{
				var v = Size.From(value);
				if (Area.text.Scale == v ||
					(Debug.CalledBySMPL == false && IsCurrentlyAccessible() == false)) return;
				var delta = value - Size.To(Area.text.Scale);
				scale = value;
				Area.text.Scale = v;
				if (Debug.CalledBySMPL == false) OnScaleChange?.Invoke(this, scale);
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
				UpdateOrigin();
				if (Debug.CalledBySMPL == false) OnOriginPercentChange?.Invoke(this, prev);
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
						$"Use '{nameof(File)}.{nameof(File.LoadAsset)} ({nameof(File)}.{nameof(File.Asset)}." +
						$"{nameof(File.Asset.Font)}, \"{value}\")' to load it.");
					return;
				}
				var prev = fontPath;
				fontPath = value;
				Area.text.Font = File.fonts[fontPath];
				if (Debug.CalledBySMPL == false) OnFontPathChange?.Invoke(this, prev);
			}
		}
		public string DisplayText
		{
			get { return AllAccess == Extent.Removed ? default : Area.text.DisplayedString; }
			set
			{
				if (Area.text.DisplayedString == value ||
					(Debug.CalledBySMPL == false && IsCurrentlyAccessible() == false)) return;
				var prev = Area.text.DisplayedString;
				Area.text.DisplayedString = value;
				UpdateOrigin();
				if (Debug.CalledBySMPL == false) OnTextChange?.Invoke(this, prev);
			}
		}
		public uint CharacterSize
		{
			get { return AllAccess == Extent.Removed ? default : Area.text.CharacterSize; }
			set
			{
				if (Area.text.CharacterSize == value ||
					(Debug.CalledBySMPL == false && IsCurrentlyAccessible() == false)) return;
				var prev = Area.text.CharacterSize;
				Area.text.CharacterSize = value;
				Position = position;
				if (Debug.CalledBySMPL == false) OnCharacterSizeChange?.Invoke(this, prev);
			}
		}
		private Size spacing, lastFrameSp;
		public Size Spacing
		{
			get { return AllAccess == Extent.Removed ? Size.Invalid : spacing; }
			set
			{
				if (spacing == value ||
					(Debug.CalledBySMPL == false && IsCurrentlyAccessible() == false)) return;
				var prev = spacing;
				spacing = value;
				Area.text.LetterSpacing = (float)value.W / 4;
				Area.text.LineSpacing = (float)value.H / 16 + (float)CharacterSize / 112;
				if (Debug.CalledBySMPL) return;
				if (Debug.CalledBySMPL == false) OnSpacingChange?.Invoke(this, prev);
			}
		}

		public static void Create(string uniqueID, Area component2D, string fontPath = "folder/font.ttf")
		{
			if (Identity<TextBox>.CannotCreate(uniqueID)) return;
			var instance = new TextBox(component2D, fontPath);
			instance.Identity = new(instance, uniqueID);
		}
		private TextBox(Area component2D, string fontPath = "folder/font.ttf") : base()
		{
			// fixing the access since the ComponentAccess' constructor depth leads to here => user has no access but this file has
			// in other words - the depth gets 1 deeper with inheritence ([3]User -> [2]Sprite/Text -> [1]Visual -> [0]Access)
			// and usually it goes as [2]User -> [1]Component -> [0]Access
			GrantAccessToFile(Debug.CurrentFilePath(2)); // grant the user access
			DenyAccessToFile(Debug.CurrentFilePath(1)); // abandon ship
			texts.Add(this);
			Area.text.DisplayedString = "Hello World!";
			Area.text.CharacterSize = 20;
			Area.text.LetterSpacing = 1;
			Area.text.LineSpacing = (float)4 / 16 + (float)CharacterSize / 112;
			Area.text.FillColor = Data.Color.From(Data.Color.White);
			Area.text.OutlineColor = Data.Color.From(Data.Color.Black);

			spacing = new Size(4, 4);
			lastFrameSp = spacing;
			lastFrameSc = new Size(1, 1);

			FontPath = fontPath;

			OnCreate?.Invoke(this);
		}
		private void UpdateOrigin()
      {
			Area.text.DisplayedString += "\n";
			var pos = new Point();
			for (uint i = 0; i < DisplayText.Length; i++)
			{
				var p = Area.text.FindCharacterPos(i + 1);
				if (pos.X < p.X) pos.X = p.X;
				if (pos.Y < p.Y) pos.Y = p.Y;
			}
			Area.text.Origin = Point.From(pos * (originPercent / 100));
			Area.text.DisplayedString = Area.text.DisplayedString.Remove(DisplayText.Length - 1, 1);
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

		public void Display(Camera camera)
		{
			if (Debug.CalledBySMPL == false && IsCurrentlyAccessible() == false) return;
			if (Window.DrawNotAllowed() || masking != null || IsHidden || Area == null ||
				Area.text == null || Area.text.Font == null) return;

			var rend = new RenderTexture((uint)Number.Sign(Area.Size.W, false), (uint)Number.Sign(Area.Size.H, false));
			rend.Clear(new SFML.Graphics.Color(Data.Color.From(Effects == null ? new Data.Color() : Effects.BackgroundColor)));
			rend.Draw(Area.text);
			rend.Display();
			var sprite = new SFML.Graphics.Sprite(rend.Texture);
			//var drawMaskResult = Effects.DrawMasks(sprite);
			//sprite.Texture = drawMaskResult.Texture;

			if (Effects != null) Effects.shader.SetUniform("Texture", sprite.Texture);
			if (Effects != null) Effects.shader.SetUniform("RawTexture", rend.Texture);

			Area.sprite.Position = Point.From(Area.LocalPosition);
			Area.sprite.Rotation = (float)Area.LocalAngle;
			sprite.Position = Point.From(Area.Position);
			sprite.Rotation = (float)Area.Angle;
			sprite.Origin = new Vector2f(
					(float)(Area.Size.W * (Area.OriginPercent.X / 100)),
					(float)(Area.Size.H * (Area.OriginPercent.Y / 100)));

			var sc = Family == null || Family.Parent == null ? new Vector2f(1, 1) : Family.Parent.Area.sprite.Scale;
			sprite.Scale = new Vector2f(1 / sc.X, 1 / sc.Y);
			if (Effects == null) camera.rendTexture.Draw(sprite);
			else camera.rendTexture.Draw(sprite, new RenderStates(BlendMode.Alpha, Transform.Identity, null, Effects.shader));

			//drawMaskResult.Dispose();
			rend.Dispose();
			sprite.Dispose();
			if (Debug.CalledBySMPL == false) OnDisplay?.Invoke(this);
		}
	}
}
