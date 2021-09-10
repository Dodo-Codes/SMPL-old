using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using SMPL.Data;
using SMPL.Gear;
using Newtonsoft.Json;
using System.Linq;

namespace SMPL.Components
{
	public class Sprite : Visual
	{
		private static event Events.ParamsOne<Sprite> OnDisplay;
		private static bool QuadError(string uniqueID)
		{
			if (uniqueID == null) { Debug.LogError(2, $"The quad's uniqueID cannot be 'null'."); return true; }
			return false;
		}

		private bool isRepeated, isSmooth;
		private string texturePath;

		// ===============

		internal static List<Sprite> sprites = new();
		[JsonProperty]
		internal Dictionary<string, Quad> quads = new();

		internal static (VertexArray, Vertex[]) QuadsToVerts(Dictionary<string, Quad> quads)
		{
			if (quads == null) return (null, null);
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
			return (vertArr, verts);
		}

		// ===============

		public static class CallWhen
		{
			public static void Display(Action<Sprite> method, uint order = uint.MaxValue) =>
				OnDisplay = Events.Add(OnDisplay, method, order);
		}
		[JsonProperty]
		public bool IsRepeated
		{
			get { return ErrorIfDestroyed() == false && isRepeated; }
			set { if (ErrorIfDestroyed() == false) isRepeated = value; }
		}
		[JsonProperty]
		public bool IsSmooth
		{
			get { return ErrorIfDestroyed() == false && isSmooth; }
			set { if (ErrorIfDestroyed() == false) isSmooth = value; }
		}
		[JsonProperty]
		public string TexturePath
		{
			get { return ErrorIfDestroyed() ? default : texturePath; }
			set
			{
				if (ErrorIfDestroyed() || value == null) return;
				var prev = texturePath;
				texturePath = value;
				if (Assets.textures.ContainsKey(value) == false)
				{
					Assets.NotLoadedError(1, Assets.Type.Texture, value);
					return;
				}
				if (prev == null) SetQuadDefault(UniqueID);
			}
		}
		public Quad[] Quads => quads.Values.ToArray();

		public Sprite(string uniqueID) : base(uniqueID)
		{
			SetQuadDefault(uniqueID);
			sprites.Add(this);
			if (cannotCreate) { ErrorAlreadyHasUID(uniqueID); Destroy(); }
		}
		public override void Destroy()
		{
			if (ErrorIfDestroyed()) return;
			quads = null;
			sprites.Remove(this);
			base.Destroy();
		}

		public void RemoveQuad(string uniqueID)
		{
			if (quads.ContainsKey(uniqueID) == false)
			{
				Debug.LogError(1, $"No {nameof(Quad)} with uniqueID '{uniqueID}' was found.");
				return;
			}
			quads.Remove(uniqueID);
		}
		public void SetQuadDefault(string uniqueID)
		{
			if (QuadError(uniqueID)) return;

			var area = AreaUniqueID == null ? (Area)PickByUniqueID(AreaUniqueID) : null;
			var sz = TexturePath == null || Assets.textures.ContainsKey(TexturePath) == false ?
				(area == null ? new Size(100, 100) : area.Size) :
				new Size(Assets.textures[TexturePath].Size.X, Assets.textures[TexturePath].Size.Y);
			var quad = new Quad(
				new Corner(new Point(0, 0), 0, 0),
				new Corner(new Point(sz.W, 0), sz.W, 0),
				new Corner(new Point(sz.W, sz.H), sz.W, sz.H),
				new Corner(new Point(0, sz.H), 0, sz.H));

			quads[uniqueID] = quad;
		}
		public void SetQuadGrid(string uniqueID, uint cellCountX, uint cellCountY)
		{
			if (QuadError(uniqueID)) return;

			var area = (Area)PickByUniqueID(AreaUniqueID);
			var areaSz = area == null ? new Size(100, 100) : area.Size;
			var sz = TexturePath == null || Assets.textures.ContainsKey(TexturePath) == false ? areaSz :
				new Size(Assets.textures[TexturePath].Size.X, Assets.textures[TexturePath].Size.Y);
			for (int y = 0; y < cellCountY; y++)
			{
				for (int x = 0; x < cellCountX; x++)
				{
					var quad = new Quad(
						new Corner(new Point(sz.W * x, sz.H * y), 0, 0),
						new Corner(new Point(sz.W * x + sz.W, sz.H * y), sz.W, 0),
						new Corner(new Point(sz.W * x + sz.W, sz.H * y + sz.H), sz.W, sz.H),
						new Corner(new Point(sz.W * x, sz.H * y + sz.H), 0, sz.H));
					quads[$"{uniqueID} {x} {y}"] = quad;
				}
			}
		}
		public void SetQuad(string uniqueID, Quad quad)
		{
			if (QuadError(uniqueID)) return;
			quads[uniqueID] = quad;
		}
		public Quad GetQuad(string uniqueID)
		{
			if (uniqueID == null || quads.ContainsKey(uniqueID) == false)
			{
				Debug.LogError(1, $"No {nameof(Quad)} was found with the uniqueID '{uniqueID}'.");
				return default;
			}
			return quads[uniqueID];
		}

		public void Display(Camera camera)
		{
			if (ErrorIfDestroyed()) return;
			if (Window.DrawNotAllowed() || visualMaskingUID != null || IsHidden) return;
			var Area = (Area)PickByUniqueID(AreaUniqueID);
			if (Area == null || Area.IsDestroyed || Area.sprite == null)
			{
				Debug.LogError(1, $"Cannot display the sprite instance '{UniqueID}' because it has no Area.\n" +
					$"Make sure the sprite instance has an Area before displaying it.");
				return;
			}

			var w = Area.sprite.TextureRect.Width;
			var h = Area.sprite.TextureRect.Height;
			var p = Area.OriginPercent / 100;
			var qtv = QuadsToVerts(quads);
			var vertArr = qtv.Item1;
			var verts = qtv.Item2;
			var Effects = (Effects)PickByUniqueID(EffectsUniqueID);
			var bounds = vertArr.Bounds;
			var rend = new RenderTexture((uint)bounds.Width, (uint)bounds.Height);
			var texture = TexturePath == null || Assets.textures.ContainsKey(TexturePath) == false ?
				null : Assets.textures[TexturePath];

			Area.DefaultSprite();

			rend.Clear(Data.Color.From(Effects == null ? new Data.Color() : Effects.BackgroundColor));
			rend.Draw(verts, PrimitiveType.Quads, new RenderStates(BlendMode.Alpha, Transform.Identity, texture, null));
			var t = Effects == null ? null : new Texture(rend.Texture);

			if (Effects != null)
			{
				Effects.shader?.SetUniform("RawTexture", t);
				Effects.DrawMasks(rend);
			}

			rend.Display();
			rend.Texture.Smooth = IsSmooth;
			rend.Texture.Repeated = IsRepeated;

			Area.sprite.TextureRect = new IntRect((int)bounds.Left, (int)bounds.Top, (int)bounds.Width, (int)bounds.Height);
			Area.sprite.Texture = rend.Texture;

			if (Effects != null) Effects.shader?.SetUniform("Texture", Area.sprite.Texture);

			Area.sprite.Origin = new Vector2f((float)(w * p.X), (float)(h * p.Y));
			Area.sprite.Position = Point.From(Area.Position);
			Area.sprite.Rotation = (float)Area.Angle;
			Area.sprite.Scale = new Vector2f(
				(float)Area.Size.W / Area.sprite.TextureRect.Width,
				(float)Area.Size.H / Area.sprite.TextureRect.Height);
			Area.sprite.Color = Data.Color.From(Effects == null ? Data.Color.White : Effects.TintColor);

			if (Effects == null) camera.rendTexture.Draw(Area.sprite);
			else camera.rendTexture.Draw(Area.sprite,
				new RenderStates(BlendMode.Alpha, Transform.Identity, null, Effects.shader));
			vertArr.Dispose();
			rend.Dispose();
			if (t != null) t.Dispose();
			OnDisplay?.Invoke(this);
		}
	}
}
