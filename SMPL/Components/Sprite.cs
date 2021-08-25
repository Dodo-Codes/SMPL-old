using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using SMPL.Data;
using SMPL.Gear;

namespace SMPL.Components
{
	public class Sprite : Visual
	{
		internal static List<Sprite> sprites = new();
		internal Dictionary<string, Quad> quads = new();

		private static event Events.ParamsTwo<Sprite, string> OnCreate, OnTexturePathChange;
		private static event Events.ParamsOne<Sprite> OnVisibilityChange, OnRepeatingChange, OnSmoothnessChange,
			OnOffsetPercentChangeEnd, OnSizePercentChangeEnd, OnDestroy, OnDisplay;
		private static event Events.ParamsTwo<Sprite, Family> OnFamilyChange;
		private static event Events.ParamsTwo<Sprite, Effects> OnEffectsChange;
		private static event Events.ParamsTwo<Sprite, Identity<Sprite>> OnIdentityChange;
		private static event Events.ParamsTwo<Sprite, Point> OnOffsetPercentChange, OnOffsetPercentChangeStart;
		private static event Events.ParamsTwo<Sprite, Size> OnSizePercentChange, OnSizePercentChangeStart,
			OnGridSizeChange;

		public static class CallWhen
		{
			public static void Create(Action<Sprite, string> method, uint order = uint.MaxValue) =>
				OnCreate = Events.Add(OnCreate, method, order);
			public static void TexturePathChange(Action<Sprite, string> method, uint order = uint.MaxValue) =>
				OnTexturePathChange = Events.Add(OnTexturePathChange, method, order);
			public static void IdentityChange(Action<Sprite, Identity<Sprite>> method,
				uint order = uint.MaxValue) => OnIdentityChange = Events.Add(OnIdentityChange, method, order);
			public static void VisibilityChange(Action<Sprite> method, uint order = uint.MaxValue) =>
				OnVisibilityChange = Events.Add(OnVisibilityChange, method, order);
			public static void FamilyChange(Action<Sprite, Family> method, uint order = uint.MaxValue) =>
				OnFamilyChange = Events.Add(OnFamilyChange, method, order);
			public static void EffectsChange(Action<Sprite, Effects> method, uint order = uint.MaxValue) =>
				OnEffectsChange = Events.Add(OnEffectsChange, method, order);
			public static void RepeatingChange(Action<Sprite> method, uint order = uint.MaxValue) =>
				OnRepeatingChange = Events.Add(OnRepeatingChange, method, order);
			public static void SmoothnessChange(Action<Sprite> method, uint order = uint.MaxValue) =>
				OnSmoothnessChange = Events.Add(OnSmoothnessChange, method, order);
			public static void OffsetPercentChangeStart(Action<Sprite, Point> method, uint order = uint.MaxValue) =>
				OnOffsetPercentChangeStart = Events.Add(OnOffsetPercentChangeStart, method, order);
			public static void OffsetPercentChange(Action<Sprite, Point> method, uint order = uint.MaxValue) =>
				OnOffsetPercentChange = Events.Add(OnOffsetPercentChange, method, order);
			public static void OffsetPercentChangeEnd(Action<Sprite> method, uint order = uint.MaxValue) =>
				OnOffsetPercentChangeEnd = Events.Add(OnOffsetPercentChangeEnd, method, order);
			public static void SizePercentChangeStart(Action<Sprite, Size> method, uint order = uint.MaxValue) =>
				OnSizePercentChangeStart = Events.Add(OnSizePercentChangeStart, method, order);
			public static void SizePercentChange(Action<Sprite, Size> method, uint order = uint.MaxValue) =>
				OnSizePercentChange = Events.Add(OnSizePercentChange, method, order);
			public static void SizePercentChangeEnd(Action<Sprite> method, uint order = uint.MaxValue) =>
				OnSizePercentChangeEnd = Events.Add(OnSizePercentChangeEnd, method, order);
			public static void GridSizeChange(Action<Sprite, Size> method, uint order = uint.MaxValue) =>
				OnGridSizeChange = Events.Add(OnGridSizeChange, method, order);
			public static void Destroy(Action<Sprite> method, uint order = uint.MaxValue) =>
				OnDestroy = Events.Add(OnDestroy, method, order);
			public static void Display(Action<Sprite> method, uint order = uint.MaxValue) =>
				OnDisplay = Events.Add(OnDisplay, method, order);
		}

		private Identity<Sprite> identity;
		public Identity<Sprite> Identity
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

		private bool isDestroyed, isRepeated, isSmooth;
		public bool IsDestroyed
		{
			get { return isDestroyed; }
			set
			{
				if (isDestroyed == value || (Debug.CalledBySMPL == false && IsCurrentlyAccessible() == false)) return;
				isDestroyed = value;

				if (Identity != null)
				{
					Identity<Sprite>.uniqueIDs.Remove(Identity.UniqueID);
					if (Identity<Sprite>.objTags.ContainsKey(this))
					{
						Identity.RemoveAllTags();
						Identity<Sprite>.objTags.Remove(this);
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
			get { return AllAccess == Extent.Removed ? default : isRepeated; }
			set
			{
				if (isRepeated == value || (Debug.CalledBySMPL == false && IsCurrentlyAccessible() == false)) return;
				isRepeated = value;
				if (Debug.CalledBySMPL == false) OnRepeatingChange?.Invoke(this);
			}
		}
		public bool IsSmooth
		{
			get { return AllAccess == Extent.Removed ? default : isSmooth; }
			set
			{
				if (isSmooth == value || (Debug.CalledBySMPL == false && IsCurrentlyAccessible() == false)) return;
				isSmooth = value;
				if (Debug.CalledBySMPL == false) OnSmoothnessChange?.Invoke(this);
			}
		}
		private string texturePath;
		public string TexturePath
		{
			get { return AllAccess == Extent.Removed ? default : texturePath; }
			set
			{
				if (texturePath == value || (Debug.CalledBySMPL == false && IsCurrentlyAccessible() == false)) return;
				if (File.textures.ContainsKey(value) == false)
				{
					Debug.LogError(1, $"The texture at '{value}' is not loaded.\n" +
						$"Use '{nameof(File)}.{nameof(File.LoadAsset)}({nameof(File)}.{nameof(File.Asset)}." +
						$"{nameof(File.Asset.Texture)}, \"{value}\")' to load it.");
					return;
				}
				var prev = texturePath;
				texturePath = value;
				if (Debug.CalledBySMPL == false) OnTexturePathChange?.Invoke(this, prev);
			}
		}

		public static void Create(string uniqueID, Area component2D, string texturePath = "folder/texture.png")
		{
			if (Identity<Sprite>.CannotCreate(uniqueID)) return;
			var instance = new Sprite(component2D, texturePath);
			instance.Identity = new(instance, uniqueID);
			instance.SetQuadDefault(uniqueID);
		}
		public void SetQuadDefault(string uniqueID)
		{
			var sz = TexturePath == null || File.textures.ContainsKey(TexturePath) == false ? transform.Size :
				new Size(File.textures[TexturePath].Size.X, File.textures[TexturePath].Size.Y);
			var quad = new Quad()
			{
				CornerA = new Corner(new Point(0, 0), 0, 0),
				CornerB = new Corner(new Point(sz.W, 0), sz.W, 0),
				CornerC = new Corner(new Point(sz.W, sz.H), sz.W, sz.H),
				CornerD = new Corner(new Point(0, sz.H), 0, sz.H),
			};

			SetQuad(uniqueID, quad);
		}
		public void SetQuadGrid(string uniqueID, uint cellCountX, uint cellCountY)
		{
			var sz = TexturePath == null || File.textures.ContainsKey(TexturePath) == false ? transform.Size :
				new Size(File.textures[TexturePath].Size.X, File.textures[TexturePath].Size.Y);
			for (int y = 0; y < cellCountY; y++)
			{
				for (int x = 0; x < cellCountX; x++)
				{
					var quad = new Quad()
					{
						CornerA = new Corner(new Point(sz.W * x, sz.H * y), 0, 0),
						CornerB = new Corner(new Point(sz.W * x + sz.W, sz.H * y), sz.W, 0),
						CornerC = new Corner(new Point(sz.W * x + sz.W, sz.H * y + sz.H), sz.W, sz.H),
						CornerD = new Corner(new Point(sz.W * x, sz.H * y + sz.H), 0, sz.H),
					};
					SetQuad($"{uniqueID} {x} {y}", quad);
				}
			}
		}
		private Sprite(Area component2D, string texturePath) : base(component2D)
		{
			// fixing the access since the ComponentAccess' constructor depth leads to here => user has no access but this file has
			// in other words - the depth gets 1 deeper with inheritence ([3]User -> [2]Sprite/Text -> [1]Visual -> [0]Access)
			// and usually it goes as [2]User -> [1]Component -> [0]Access
			GrantAccessToFile(Debug.CurrentFilePath(2)); // grant the user access
			DenyAccessToFile(Debug.CurrentFilePath(0)); // abandon ship
			sprites.Add(this);
			OnCreate?.Invoke(this, texturePath);
			if (File.textures.ContainsKey(texturePath) == false)
			{
				Debug.LogError(2, $"The texture at '{texturePath}' is not loaded.\n" +
					$"Use '{nameof(File)}.{nameof(File.LoadAsset)}({nameof(File)}.{nameof(File.Asset)}." +
					$"{nameof(File.Asset.Texture)}, \"{texturePath}\")' to load it.");
				return;
			}
			TexturePath = texturePath;
		}
		internal void Update()
		{

		}
		internal static void TriggerOnVisibilityChange(Sprite instance) => OnVisibilityChange?.Invoke(instance);
		internal static void TriggerOnFamilyChange(Sprite i, Family f) => OnFamilyChange?.Invoke(i, f);
		internal static void TriggerOnEffectsChange(Sprite i, Effects e) => OnEffectsChange?.Invoke(i, e);

		public void SetQuad(string uniqueID, Quad quad)
		{
			if (uniqueID == null)
			{
				Debug.LogError(1, $"The quad's uniqueID cannot be 'null'.");
				return;
			}
			quads[uniqueID] = quad;
		}
		public void Display(Camera camera)
		{
			if (Debug.CalledBySMPL == false && IsCurrentlyAccessible() == false) return;
			if (Window.DrawNotAllowed() || masking != null || IsHidden || transform == null ||
				transform.sprite == null) return;

			var w = transform.sprite.TextureRect.Width;
			var h = transform.sprite.TextureRect.Height;
			var p = transform.OriginPercent / 100;
			var vertArr = new VertexArray();
			var verts = new Vertex[quads.Count * 4];
			var i = 0;
			foreach (var kvp in quads)
			{
				vertArr.Append(kvp.Value.CornerA.Vertex);
				vertArr.Append(kvp.Value.CornerB.Vertex);
				vertArr.Append(kvp.Value.CornerC.Vertex);
				vertArr.Append(kvp.Value.CornerD.Vertex);
				verts[i * 4 + 0] = kvp.Value.CornerA.Vertex;
				verts[i * 4 + 1] = kvp.Value.CornerB.Vertex;
				verts[i * 4 + 2] = kvp.Value.CornerC.Vertex;
				verts[i * 4 + 3] = kvp.Value.CornerD.Vertex;
				i++;
			}
			var bounds = vertArr.Bounds;
			var rend = new RenderTexture((uint)bounds.Width, (uint)bounds.Height);
			var texture = TexturePath == null || File.textures.ContainsKey(TexturePath) == false ?
				null : File.textures[TexturePath];

			transform.sprite.Position = new Vector2f();
			transform.sprite.Rotation = 0;
			transform.sprite.Scale = new Vector2f(1, 1);
			transform.sprite.Origin = new Vector2f(0, 0);

			rend.Clear(Data.Color.From(Effects == null ? new Data.Color() : Effects.BackgroundColor));
			rend.Draw(verts, PrimitiveType.Quads, new RenderStates(BlendMode.Alpha, Transform.Identity, texture, null));
			rend.Display();
			rend.Texture.Smooth = IsSmooth;
			rend.Texture.Repeated = IsRepeated;

			transform.sprite.TextureRect = new IntRect((int)bounds.Left, (int)bounds.Top, (int)bounds.Width, (int)bounds.Height);
			transform.sprite.Texture = rend.Texture;

			if (Effects != null) Effects.shader.SetUniform("Texture", transform.sprite.Texture);
			//if (Effects != null) Effects.shader.SetUniform("RawTexture", rawTextureShader);

			transform.sprite.Origin = new Vector2f((float)(w * p.X), (float)(h * (float)p.Y));
			transform.sprite.Position = Point.From(transform.Position);
			transform.sprite.Rotation = (float)transform.Angle;
			transform.sprite.Scale = new Vector2f(
				(float)transform.Size.W / transform.sprite.TextureRect.Width,
				(float)transform.Size.H / transform.sprite.TextureRect.Height);

			if (Effects == null) camera.rendTexture.Draw(transform.sprite);
			else camera.rendTexture.Draw(transform.sprite,
				new RenderStates(BlendMode.Alpha, Transform.Identity, null, Effects.shader));
			vertArr.Dispose();
			rend.Dispose();
		}
	}
}
