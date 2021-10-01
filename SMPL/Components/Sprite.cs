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

		private bool isRepeated, isSmooth;
		private string texturePath;

		// ===============

		internal static List<Sprite> sprites = new();
		[JsonProperty]
		internal SortedDictionary<string, Quad> quads = new();

		internal static (VertexArray, Vertex[]) QuadsToVerts(SortedDictionary<string, Quad> quads)
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
			if (ErrorIfDestroyed() || QuadError(uniqueID)) return;
			quads.Remove(uniqueID);
		}
		public void SetQuadDefault(string uniqueID)
		{
			if (ErrorIfDestroyed()) return;

			var area = AreaUniqueID == null ? (Area)PickByUniqueID(AreaUniqueID) : null;
			var sz = TexturePath == null || Assets.textures.ContainsKey(TexturePath) == false ?
				(area == null ? new Size(100, 100) : area.Size) :
				new Size(Assets.textures[TexturePath].Size.X, Assets.textures[TexturePath].Size.Y);
			var quad = new Quad(
				new Corner(new Point(0, 0), new Point(0, 0)),
				new Corner(new Point(sz.W, 0), new Point(sz.W, 0)),
				new Corner(new Point(sz.W, sz.H), new Point(sz.W, sz.H)),
				new Corner(new Point(0, sz.H), new Point(0, sz.H)));

			quads[uniqueID] = quad;
		}
		public void SetQuadGrid(string uniqueID, Point tileCount, Size tileSize)
		{
			if (ErrorIfDestroyed()) return;

			var sz = tileSize;
			for (int y = 0; y < tileCount.Y; y++)
			{
				for (int x = 0; x < tileCount.X; x++)
				{
					var quad = new Quad(
						new Corner(new Point(sz.W * x, sz.H * y), new Point(0, 0)),
						new Corner(new Point(sz.W * x + sz.W, sz.H * y), new Point(sz.W, 0)),
						new Corner(new Point(sz.W * x + sz.W, sz.H * y + sz.H), new Point(sz.W, sz.H)),
						new Corner(new Point(sz.W * x, sz.H * y + sz.H), new Point(0, sz.H)));
					quad.TileSize = tileSize;
					quads[$"{uniqueID} {x} {y}"] = quad;
				}
			}
		}
		public void SetQuad(string uniqueID, Quad quad)
		{
			if (ErrorIfDestroyed()) return;
			quads[uniqueID] = quad;
		}
		public Quad GetQuad(string uniqueID)
		{
			return ErrorIfDestroyed() || QuadError(uniqueID) ? default : quads[uniqueID];
		}
		public void SetQuadTextureCropPercent(string uniqueID, Point topLeftPercent, Point downRightPercent)
		{
			if (ErrorIfDestroyed() || QuadError(uniqueID) || ErrorIfNoTexture()) return;
			topLeftPercent /= 100;
			downRightPercent /= 100;
			var sz = GetSize();
			var quad = GetQuad(uniqueID);
			quad.CornerA = new Corner(quad.CornerA.Position, new Point(sz.W * topLeftPercent.X, sz.H * topLeftPercent.Y));
			quad.CornerB = new Corner(quad.CornerB.Position, new Point(sz.W * downRightPercent.X, sz.H * topLeftPercent.Y));
			quad.CornerC = new Corner(quad.CornerC.Position, new Point(sz.W * downRightPercent.X, sz.H * downRightPercent.Y));
			quad.CornerD = new Corner(quad.CornerD.Position, new Point(sz.W * topLeftPercent.X, sz.H * downRightPercent.Y));
			SetQuad(uniqueID, quad);
		}
		public bool HasQuad(string uniqueID)
		{
			return uniqueID != null && quads.ContainsKey(uniqueID);
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

			UpdateSprite();
			Draw();
			Dispose();
			OnDisplay?.Invoke(this);

			void UpdateSprite()
			{
				var w = Area.sprite.TextureRect.Width;
				var h = Area.sprite.TextureRect.Height;
				var p = Area.OriginPercent / 100;
				Area.sprite.Color = Data.Color.From(Effects == null ? Data.Color.White : Effects.TintColor);
				Area.sprite.Origin = new Vector2f((float)(w * p.X), (float)(h * p.Y));
				Area.sprite.Position = Point.From(Area.Position);
				Area.sprite.Rotation = (float)Area.Angle;
				Area.sprite.Scale = new Vector2f(
					(float)Area.Size.W / Area.sprite.TextureRect.Width,
					(float)Area.Size.H / Area.sprite.TextureRect.Height);
			}
			void Draw()
			{
				if (Effects == null) camera.rendTexture.Draw(Area.sprite);
				else camera.rendTexture.Draw(Area.sprite,
					new RenderStates(BlendMode.Alpha, Transform.Identity, null, Effects.shader));
				Performance.DrawCallsPerFrame++;
			}
			void Dispose()
			{
				vertArr.Dispose();
				rend.Dispose();
				if (t != null) t.Dispose();
			}
		}

		private bool ErrorIfNoTexture()
		{
			if (TexturePath == null)
			{
				Debug.LogError(2, $"No texture is taken into account. Use the {nameof(TexturePath)} property if texture is needed.");
				return true;
			}
			if (Assets.textures.ContainsKey(TexturePath) == false)
			{
				Assets.NotLoadedError(2, Assets.Type.Texture, TexturePath);
				return true;
			}
			return false;
		}
		private bool QuadError(string uniqueID)
		{
			if (uniqueID == null) { Debug.LogError(2, $"The quad's uniqueID cannot be 'null'."); return true; }
			if (quads.ContainsKey(uniqueID) == false)
			{ Debug.LogError(2, $"No {nameof(Quad)} with uniqueID '{uniqueID}' was found."); return true; }
			return false;
		}
		private Size GetSize()
		{
			var area = (Area)PickByUniqueID(AreaUniqueID);
			var areaSz = area == null ? new Size(100, 100) : area.Size;
			var sz = TexturePath == null || Assets.textures.ContainsKey(TexturePath) == false ? areaSz :
				new Size(Assets.textures[TexturePath].Size.X, Assets.textures[TexturePath].Size.Y);
			return sz;
		}
	}
}
