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
			OnOffsetPercentChangeEnd, OnSizePercentChangeEnd, OnDestroy, OnDisplay;
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
			public static void Destroy(Action<ComponentSprite> method, uint order = uint.MaxValue) =>
				OnDestroy = Events.Add(OnDestroy, method, order);
			public static void Display(Action<ComponentSprite> method, uint order = uint.MaxValue) =>
				OnDisplay = Events.Add(OnDisplay, method, order);
		}

		private ComponentIdentity<ComponentSprite> identity;
		public ComponentIdentity<ComponentSprite> Identity
		{
			get { return AllAccess == Access.Removed ? default : identity; }
			set
			{
				if (identity == value || (Debug.CalledBySMPL == false && IsCurrentlyAccessible() == false)) return;
				var prev = identity;
				identity = value;
				if (Debug.CalledBySMPL == false) OnIdentityChange?.Invoke(this, prev);
			}
		}

		private bool isDestroyed;
		public bool IsDestroyed
		{
			get { return isDestroyed; }
			set
			{
				if (isDestroyed == value || (Debug.CalledBySMPL == false && IsCurrentlyAccessible() == false)) return;
				isDestroyed = value;

				if (image != null) image.Dispose();
				if (rawTexture != null) rawTexture.Dispose();
				if (rawTextureShader != null) rawTextureShader.Dispose();

				if (Identity != null)
				{
					ComponentIdentity<ComponentSprite>.uniqueIDs.Remove(Identity.UniqueID);
					if (ComponentIdentity<ComponentSprite>.objTags.ContainsKey(this))
					{
						Identity.RemoveAllTags();
						ComponentIdentity<ComponentSprite>.objTags.Remove(this);
					}
					Identity.instance = null;
					Identity = null;
				}
				sprites.Remove(this);
				Dispose();
				if (Debug.CalledBySMPL == false) OnDestroy?.Invoke(this);
			}
		}
		public bool IsRepeated
		{
			get { return AllAccess != Access.Removed && transform.sprite.Texture.Repeated; }
			set
			{
				if (transform.sprite.Texture.Repeated == value ||
					(Debug.CalledBySMPL == false && IsCurrentlyAccessible() == false)) return;
				transform.sprite.Texture.Repeated = value;
				if (Debug.CalledBySMPL == false) OnRepeatingChange?.Invoke(this);
			}
		}
		public bool IsSmooth
		{
			get { return AllAccess != Access.Removed && transform.sprite.Texture.Smooth; }
			set
			{
				if (transform.sprite.Texture.Smooth == value ||
					(Debug.CalledBySMPL == false && IsCurrentlyAccessible() == false)) return;
				transform.sprite.Texture.Smooth = value;
				if (Debug.CalledBySMPL == false) OnSmoothnessChange?.Invoke(this);
			}
		}
		public string TexturePath { get; private set; }
		private Point offsetPercent, lastFrameOffPer;
		public Point OffsetPercent
		{
			get { return AllAccess == Access.Removed ? new Point(double.NaN, double.NaN) : offsetPercent; }
			set
			{
				if (offsetPercent == value ||
					(Debug.CalledBySMPL == false && IsCurrentlyAccessible() == false)) return;
				var prev = offsetPercent;
				offsetPercent = value;
				var rect = transform.sprite.TextureRect;
				var sz = transform.sprite.Texture.Size;
				transform.sprite.TextureRect = new IntRect(
					(int)(sz.X * (offsetPercent.X / 100)), (int)(sz.Y * (offsetPercent.Y / 100)),
					rect.Width, rect.Height);
				if (Debug.CalledBySMPL == false) OnOffsetPercentChange?.Invoke(this, prev);
			}
		}
		private Size sizePercent, lastFrameSzPer;
		public Size SizePercent
		{
			get { return AllAccess == Access.Removed ? new Size(double.NaN, double.NaN) : sizePercent; }
			set
			{
				if (sizePercent == value ||
					(Debug.CalledBySMPL == false && IsCurrentlyAccessible() == false)) return;
				var prev = sizePercent;
				sizePercent = value;
				value /= 100;

				var sz = transform.sprite.Texture.Size;
				var textRect = transform.sprite.TextureRect;
				transform.sprite.TextureRect = new IntRect(textRect.Left, textRect.Top, (int)(sz.X * value.W), (int)(sz.Y * value.H));
				if (Debug.CalledBySMPL == false) OnSizePercentChange?.Invoke(this, prev);
			}
		}
		private Size gridSize;
		public Size GridSize
		{
			get { return AllAccess == Access.Removed ? new Size(double.NaN, double.NaN) : gridSize; }
			set
			{
				if (gridSize == value ||
					(Debug.CalledBySMPL == false && IsCurrentlyAccessible() == false)) return;
				var prev = gridSize;
				gridSize = value;
				transform.OriginPercent = transform.OriginPercent;
				if (Debug.CalledBySMPL == false) OnGridSizeChange?.Invoke(this, prev);
			}
		}

		public static void Create(string uniqueID, Component2D component2D, string texturePath = "folder/texture.png")
		{
			if (ComponentIdentity<ComponentSprite>.CannotCreate(uniqueID)) return;
			var instance = new ComponentSprite(component2D, texturePath);
			instance.Identity = new(instance, uniqueID);
		}
		private ComponentSprite(Component2D component2D, string texturePath = "folder/texture.png") : base(component2D)
		{
			// fixing the access since the ComponentAccess' constructor depth leads to here => user has no access but this file has
			// in other words - the depth gets 1 deeper with inheritence ([3]User -> [2]Sprite/Text -> [1]Visual -> [0]Access)
			// and usually it goes as [2]User -> [1]Component -> [0]Access
			GrantAccessToFile(Debug.CurrentFilePath(2)); // grant the user access
			DenyAccessToFile(Debug.CurrentFilePath(0)); // abandon ship
			if (File.textures.ContainsKey(texturePath) == false)
			{
				Debug.LogError(2, $"The texture at '{texturePath}' is not loaded.\n" +
					$"Use '{nameof(File)}.{nameof(File.LoadAsset)}({nameof(File)}.{nameof(File.Asset)}." +
					$"{nameof(File.Asset.Texture)}, \"{texturePath}\")' to load it.");
				IsDestroyed = true;
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
			if (Debug.CalledBySMPL == false && IsCurrentlyAccessible() == false) return;
			if (Window.DrawNotAllowed() || masking != null || IsHidden || transform == null ||
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
			if (Debug.CalledBySMPL == false) OnDisplay?.Invoke(this);
		}
		public void DisplayTest(ComponentCamera camera)
		{
			var verts = new Vertex[]
			{
				new Vertex(new Vector2f(0, 0), Color.From(Color.White), new Vector2f(0,  0)),
				new Vertex(new Vector2f(64, 0), Color.From(Color.White), new Vector2f(64, 0)),
				new Vertex(new Vector2f(64, 64), Color.From(Color.White), new Vector2f(64, 64)),
				new Vertex(new Vector2f(0, 64), Color.From(Color.White), new Vector2f(0,  64)),
				new Vertex(new Vector2f(64 + 0,  64 + 0 ), Color.From(Color.White), new Vector2f(0,  0)),
				new Vertex(new Vector2f(64 + 64, 64 + 0 ), Color.From(Color.White), new Vector2f(64, 0)),
				new Vertex(new Vector2f(64 + 64, 64 + 64), Color.From(Color.White), new Vector2f(64, 64)),
				new Vertex(new Vector2f(64 + 0,  64 + 64), Color.From(Color.White), new Vector2f(0,  64)),
			};
			var vertArr = new VertexArray();
			for (int i = 0; i < verts.Length; i++)
			{
				vertArr.Append(verts[i]);
			}
			var bounds = vertArr.Bounds;
			vertArr.Dispose();

			transform.sprite.Position = new Vector2f();
			transform.sprite.Rotation = 0;
			transform.sprite.Scale = new Vector2f(1, 1);
			transform.sprite.Origin = new Vector2f(0, 0);

			var rend = new RenderTexture((uint)bounds.Width, (uint)bounds.Height);
			rend.Draw(verts, PrimitiveType.Quads,
				new RenderStates(BlendMode.Alpha, Transform.Identity, File.textures["explosive.jpg"], null));
			rend.Display();
			transform.sprite.TextureRect = new IntRect((int)bounds.Left, (int)bounds.Top, (int)bounds.Width, (int)bounds.Height);
			transform.sprite.Texture = rend.Texture;

			Effects.shader.SetUniform("Texture", transform.sprite.Texture);
			Effects.shader.SetUniform("RawTexture", rawTextureShader);

			var w = transform.sprite.TextureRect.Width;
			var h = transform.sprite.TextureRect.Height;
			var p = transform.OriginPercent / 100;

			transform.sprite.Origin = new Vector2f((float)(w * p.X), (float)(h * (float)p.Y));
			transform.sprite.Position = Point.From(transform.Position);
			transform.sprite.Rotation = (float)transform.Angle;
			transform.sprite.Scale = new Vector2f(
				(float)transform.Size.W / transform.sprite.TextureRect.Width,
				(float)transform.Size.H / transform.sprite.TextureRect.Height);

			camera.rendTexture.Draw(transform.sprite,
				new RenderStates(BlendMode.Alpha, Transform.Identity, null, Effects.shader));
			rend.Dispose();
		}
	}
}
