using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;

namespace SMPL
{
	public class ComponentSprite : ComponentVisual
	{
		internal static List<ComponentSprite> sprites = new();
		internal Image image;
		internal Texture rawTexture, rawTextureShader;
		internal byte[] rawTextureData;

		private static event Events.ParamsTwo<ComponentSprite, string> OnCreate;
		private static event Events.ParamsOne<ComponentSprite> OnVisibilityChange, OnRepeatingChange, OnSmoothnessChange,
			OnOffsetPercentChangeEnd, OnSizePercentChangeEnd;
		private static event Events.ParamsTwo<ComponentSprite, ComponentFamily> OnFamilyChange;
		private static event Events.ParamsTwo<ComponentSprite, ComponentEffects> OnEffectsChange;
		private static event Events.ParamsTwo<ComponentSprite, ComponentIdentity<ComponentSprite>> OnIdentityChange;
		private static event Events.ParamsTwo<ComponentSprite, Point> OnOffsetPercentChange, OnOffsetPercentChangeStart;
		private static event Events.ParamsTwo<ComponentSprite, Size> OnSizePercentChange, OnSizePercentChangeStart,
			OnGridSizeChange;

		public static class CallWhen
		{
			public static void Create(Action<ComponentSprite, string> method, uint order = uint.MaxValue) =>
				OnCreate = Events.Add(OnCreate, method, order);
			public static void IdentityChange(Action<ComponentSprite, ComponentIdentity<ComponentSprite>> method,
				uint order = uint.MaxValue) => OnIdentityChange = Events.Add(OnIdentityChange, method, order);
			public static void VisibilityChange(Action<ComponentSprite> method, uint order = uint.MaxValue) =>
				OnVisibilityChange = Events.Add(OnVisibilityChange, method, order);
			public static void FamilyChange(Action<ComponentSprite, ComponentFamily> method, uint order = uint.MaxValue) =>
				OnFamilyChange = Events.Add(OnFamilyChange, method, order);
			public static void EffectsChange(Action<ComponentSprite, ComponentEffects> method, uint order = uint.MaxValue) =>
				OnEffectsChange = Events.Add(OnEffectsChange, method, order);
			public static void RepeatingChange(Action<ComponentSprite> method, uint order = uint.MaxValue) =>
				OnRepeatingChange = Events.Add(OnRepeatingChange, method, order);
			public static void SmoothnessChange(Action<ComponentSprite> method, uint order = uint.MaxValue) =>
				OnSmoothnessChange = Events.Add(OnSmoothnessChange, method, order);
			public static void OffsetPercentChangeStart(Action<ComponentSprite, Point> method, uint order = uint.MaxValue) =>
				OnOffsetPercentChangeStart = Events.Add(OnOffsetPercentChangeStart, method, order);
			public static void OffsetPercentChange(Action<ComponentSprite, Point> method, uint order = uint.MaxValue) =>
				OnOffsetPercentChange = Events.Add(OnOffsetPercentChange, method, order);
			public static void OffsetPercentChangeEnd(Action<ComponentSprite> method, uint order = uint.MaxValue) =>
				OnOffsetPercentChangeEnd = Events.Add(OnOffsetPercentChangeEnd, method, order);
			public static void SizePercentChangeStart(Action<ComponentSprite, Size> method, uint order = uint.MaxValue) =>
				OnSizePercentChangeStart = Events.Add(OnSizePercentChangeStart, method, order);
			public static void SizePercentChange(Action<ComponentSprite, Size> method, uint order = uint.MaxValue) =>
				OnSizePercentChange = Events.Add(OnSizePercentChange, method, order);
			public static void SizePercentChangeEnd(Action<ComponentSprite> method, uint order = uint.MaxValue) =>
				OnSizePercentChangeEnd = Events.Add(OnSizePercentChangeEnd, method, order);
			public static void GridSizeChange(Action<ComponentSprite, Size> method, uint order = uint.MaxValue) =>
				OnGridSizeChange = Events.Add(OnGridSizeChange, method, order);
		}

		private ComponentIdentity<ComponentSprite> identity;
		public ComponentIdentity<ComponentSprite> Identity
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

		public bool IsRepeated
		{
			get { return !IsNotLoaded() && transform.sprite.Texture.Repeated; }
			set
			{
				if (transform.sprite.Texture.Repeated == value || IsNotLoaded() ||
					(Debug.CurrentMethodIsCalledByUser && IsCurrentlyAccessible() == false)) return;
				transform.sprite.Texture.Repeated = value;
				OnRepeatingChange?.Invoke(this);
			}
		}
		public bool IsSmooth
		{
			get { return !IsNotLoaded() && transform.sprite.Texture.Smooth; }
			set
			{
				if (transform.sprite.Texture.Smooth == value || IsNotLoaded() ||
					(Debug.CurrentMethodIsCalledByUser && IsCurrentlyAccessible() == false)) return;
				transform.sprite.Texture.Smooth = value;
				OnSmoothnessChange?.Invoke(this);
			}
		}
		public string TexturePath { get; private set; }
		private Point offsetPercent, lastFrameOffPer;
		public Point OffsetPercent
		{
			get { return IsNotLoaded() ? default : offsetPercent; }
			set
			{
				if (offsetPercent == value || IsNotLoaded() ||
					(Debug.CurrentMethodIsCalledByUser && IsCurrentlyAccessible() == false)) return;
				var prev = offsetPercent;
				offsetPercent = value;
				var rect = transform.sprite.TextureRect;
				var sz = transform.sprite.Texture.Size;
				transform.sprite.TextureRect = new IntRect(
					(int)(sz.X * (offsetPercent.X / 100)), (int)(sz.Y * (offsetPercent.Y / 100)),
					rect.Width, rect.Height);
				if (Debug.CurrentMethodIsCalledByUser == false) return;
				OnOffsetPercentChange?.Invoke(this, prev);
			}
		}
		private Size sizePercent, lastFrameSzPer;
		public Size SizePercent
		{
			get { return IsNotLoaded() ? default : sizePercent; }
			set
			{
				if (sizePercent == value || IsNotLoaded() ||
					(Debug.CurrentMethodIsCalledByUser && IsCurrentlyAccessible() == false)) return;
				var prev = sizePercent;
				sizePercent = value;
				value /= 100;

				var sz = transform.sprite.Texture.Size;
				var textRect = transform.sprite.TextureRect;
				transform.sprite.TextureRect = new IntRect(textRect.Left, textRect.Top, (int)(sz.X * value.W), (int)(sz.Y * value.H));
				if (Debug.CurrentMethodIsCalledByUser == false) return;
				OnSizePercentChange?.Invoke(this, prev);
			}
		}
		private Size gridSize;
		public Size GridSize
		{
			get { return IsNotLoaded() ? default : gridSize; }
			set
			{
				if (gridSize == value || IsNotLoaded() ||
					(Debug.CurrentMethodIsCalledByUser && IsCurrentlyAccessible() == false)) return;
				var prev = gridSize;
				gridSize = value;
				transform.OriginPercent = transform.OriginPercent;
				if (Debug.CurrentMethodIsCalledByUser == false) return;
				OnGridSizeChange?.Invoke(this, prev);
			}
		}

		public ComponentSprite(Component2D component2D, string texturePath = "folder/texture.png") : base(component2D)
		{
			// fixing the access since the ComponentAccess' constructor depth leads to here => user has no access but this file has
			// in other words - the depth gets 1 deeper with inheritence ([3]User -> [2]Sprite/Text -> [1]Visual -> [0]Access)
			// and usually it goes as [2]User -> [1]Component -> [0]Access
			GrantAccessToFile(Debug.CurrentFilePath(1)); // grant the user access
			DenyAccessToFile(Debug.CurrentFilePath(0)); // abandon ship
			if (File.textures.ContainsKey(texturePath) == false)
			{
				Debug.LogError(1, $"The texture at '{texturePath}' is not loaded.\n" +
					$"Use '{nameof(File)}.{nameof(File.LoadAsset)}({nameof(File)}.{nameof(File.Asset)}." +
					$"{nameof(File.Asset.Texture)}, \"{texturePath}\")' to load it.");
				return;
			}
			sprites.Add(this);

			TexturePath = texturePath;
			var texture = File.textures[texturePath];
			transform.sprite.Texture = texture;

			image = new Image(transform.sprite.Texture.CopyToImage());
			rawTextureData = image.Pixels;
			rawTexture = new Texture(image);
			image.FlipVertically();
			rawTextureShader = new Texture(image);

			transform.sprite.Texture.Repeated = true;
			sizePercent = new Size(100, 100);
			lastFrameSzPer = sizePercent;
			HasLoadedAssetFile = true;

			OnCreate?.Invoke(this, texturePath);
		}
		internal void Update()
      {
			if (Gate.EnterOnceWhile($"{creationFrame}-{rand}-sprite-off-start", lastFrameOffPer != offsetPercent))
				OnOffsetPercentChangeStart?.Invoke(this, lastFrameOffPer);
			if (Gate.EnterOnceWhile($"{creationFrame}-{rand}-sprite-off-end", lastFrameOffPer == offsetPercent))
				if (creationFrame + 1 != Performance.frameCount) OnOffsetPercentChangeEnd?.Invoke(this);
			//=============================
			if (Gate.EnterOnceWhile($"{creationFrame}-{rand}-sprite-size-start", lastFrameSzPer != sizePercent))
				OnSizePercentChangeStart?.Invoke(this, lastFrameSzPer);
			if (Gate.EnterOnceWhile($"{creationFrame}-{rand}-sprite-size-end", lastFrameSzPer == sizePercent))
				if (creationFrame + 1 != Performance.frameCount) OnSizePercentChangeEnd?.Invoke(this);
			//=============================
			lastFrameOffPer = offsetPercent;
			lastFrameSzPer = sizePercent;
		}
		internal static void TriggerOnVisibilityChange(ComponentSprite instance) => OnVisibilityChange?.Invoke(instance);
		internal static void TriggerOnFamilyChange(ComponentSprite i, ComponentFamily f) => OnFamilyChange?.Invoke(i, f);
		internal static void TriggerOnEffectsChange(ComponentSprite i, ComponentEffects e) => OnEffectsChange?.Invoke(i, e);

		public void Display(ComponentCamera camera)
		{
			if (Debug.CurrentMethodIsCalledByUser && IsCurrentlyAccessible() == false) return;
			if (Window.DrawNotAllowed() || IsNotLoaded() || masking != null || IsHidden || transform == null ||
				transform.sprite == null || transform.sprite.Texture == null) return;

			transform.sprite.Position = new Vector2f();
			transform.sprite.Rotation = 0;
			transform.sprite.Scale = new Vector2f(1, 1);
			transform.sprite.Origin = new Vector2f(0, 0);

			transform.sprite.Texture = rawTexture;
			var drawMaskResult = Effects.DrawMasks(transform.sprite);
			transform.sprite.Texture = drawMaskResult.Texture;

			var w = transform.sprite.TextureRect.Width;
			var h = transform.sprite.TextureRect.Height;
			var p = transform.OriginPercent / 100;
			var x = w * (float)p.X * ((float)GridSize.W / 2f) + (w * (float)p.X / 2f);
			var y = h * (float)p.Y * ((float)GridSize.H / 2f) + (h * (float)p.Y / 2f);
			transform.sprite.Origin = new Vector2f(x, y);

			transform.sprite.Position = Point.From(transform.Position);
			transform.sprite.Rotation = (float)transform.Angle;
			transform.sprite.Scale = new Vector2f(
				(float)transform.Size.W / rawTexture.Size.X,
				(float)transform.Size.H / rawTexture.Size.Y);

			Effects.shader.SetUniform("Texture", transform.sprite.Texture);
			Effects.shader.SetUniform("RawTexture", rawTextureShader);

			var pos = transform.sprite.Position;
			for (int j = 0; j < GridSize.H + 1; j++)
			{
				for (int i = 0; i < GridSize.W + 1; i++)
				{
					var p1 = transform.sprite.Transform.TransformPoint(new Vector2f((pos.X + w) * i, (pos.Y + h) * j));

					transform.sprite.Position = p1;
					camera.rendTexture.Draw(transform.sprite, new RenderStates(Effects.shader));
					transform.sprite.Position = pos;
				}
			}
			drawMaskResult.Dispose();
		}
	}
}
