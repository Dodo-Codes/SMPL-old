using Newtonsoft.Json;
using SMPL.Components;
using SMPL.Data;
using SMPL.Gear;
using SFML.System;
using System.Collections.Generic;
using SFML.Graphics;
using System.Linq;
using System;

namespace SMPL.Prefabs
{
	public class LayeredShape3D : Visual
	{
		private readonly Dictionary<int, Point[]> texCoords = new();
		private readonly Dictionary<int, Data.Color> colors = new();

		private string texturePath;
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
				if (prev == null)
					SetLayerTextureCropDefault();
			}
		}
		[JsonProperty]
		public bool IsRepeated { get; set; }
		[JsonProperty]
		public bool IsSmooth { get; set; }
		[JsonProperty]
		public Size TileSize { get; set; } = new Size(32, 32);
		[JsonProperty]
		public int LayerStackCount { get; set; } = 8;
		[JsonProperty]
		public int LayerStackSpacing { get; set; } = 10;
		[JsonProperty]
		public double LayerStackAngle { get; set; } = 270;

		public LayeredShape3D(string uniqueID) : base(uniqueID)
		{
			for (int i = 0; i < LayerStackCount; i++)
				texCoords[i] = new Point[] { new Point(0, 0), new Point(100, 100) };
			for (int i = 0; i < LayerStackCount; i++)
				colors[i] = Data.Color.White;

			if (cannotCreate) { ErrorAlreadyHasUID(uniqueID); Destroy(); }
		}
		public override void Destroy()
		{
			if (ErrorIfDestroyed()) return;
			base.Destroy();
		}

		public void Display(Camera camera)
		{
			if (ErrorIfDestroyed() || Window.DrawNotAllowed()) return;
			if (visualMaskingUID != null || IsHidden) return;
			var Area = (Area)PickByUniqueID(AreaUniqueID);
			if (Area == null || Area.IsDestroyed || Area.sprite == null)
			{
				Debug.LogError(1,
					$"Cannot display the {nameof(LayeredShape3D)} instance '{UniqueID}' because it has no {nameof(Components.Area)}.\n" +
					$"Make sure the {nameof(LayeredShape3D)} instance has an {nameof(Components.Area)} before displaying it.");
				return;
			}
			if (camera.Captures(this) == false) return;

			UpdateSprite();
			
			var quads = new SortedDictionary<string, Quad>();
			var tl = P(Area.sprite.Position);
			var tr = P(Area.sprite.Position + Point.From(new Point(Area.Size.W, 0)));
			var br = P(Area.sprite.Position + Point.From(new Point(Area.Size.W, Area.Size.H)));
			var bl = P(Area.sprite.Position + Point.From(new Point(0, Area.Size.H)));

			for (int i = 0; i < LayerStackCount; i++)
			{
				AddQuad(i,
					Point.MoveAtAngle(tl, LayerStackAngle, i * LayerStackSpacing, Gear.Time.Unit.Frame),
					Point.MoveAtAngle(tr, LayerStackAngle, i * LayerStackSpacing, Gear.Time.Unit.Frame),
					Point.MoveAtAngle(br, LayerStackAngle, i * LayerStackSpacing, Gear.Time.Unit.Frame),
					Point.MoveAtAngle(bl, LayerStackAngle, i * LayerStackSpacing, Gear.Time.Unit.Frame));
			}

			Draw();

			Point P(Vector2f vec)
			{
				return Point.To(Area.sprite.Transform.TransformPoint(vec));
			}
			void UpdateSprite()
			{
				Area.sprite.Position = Point.From(Area.Position);
				Area.sprite.Rotation = (float)Area.Angle;
				Area.sprite.Scale = new Vector2f(1, 1);
				Area.sprite.Origin = new Vector2f(
						(float)(Area.Size.W * (Area.OriginPercent.X / 100)),
						(float)(Area.Size.H * (Area.OriginPercent.Y / 100)));
			}
			void AddQuad(int layer, Point tl, Point tr, Point br, Point bl)
			{
				tl = new Point(tl.X, tl.Y) { Color = colors[layer] };
				tr = new Point(tr.X, tr.Y) { Color = colors[layer] };
				br = new Point(br.X, br.Y) { Color = colors[layer] };
				bl = new Point(bl.X, bl.Y) { Color = colors[layer] };
				quads.Add($"{layer}", new Quad(
					new Corner(tl, new Point(texCoords[layer][0].X, texCoords[layer][0].Y)),
					new Corner(tr, new Point(texCoords[layer][1].X, texCoords[layer][0].Y)),
					new Corner(br, new Point(texCoords[layer][1].X, texCoords[layer][1].Y)),
					new Corner(bl, new Point(texCoords[layer][0].X, texCoords[layer][1].Y))));
			}
			void Draw()
			{
				var texture = TexturePath != null && Assets.textures.ContainsKey(TexturePath) ? Assets.textures[TexturePath] : null;
				if (texture != null)
				{
					texture.Repeated = IsRepeated;
					texture.Smooth = IsSmooth;
				}
				camera.rendTexture.Draw(Components.Sprite.QuadsToVerts(quads).Item2, PrimitiveType.Quads, new RenderStates(texture));
				Performance.DrawCallsPerFrame++;
			}
		}

		public void SetLayerTextureCropDefault() 
		{
			if (ErrorIfNoTexture()) return;

			var sz = GetTextureSize();
			for (int i = 0; i < LayerStackCount; i++)
				texCoords[i] = new Point[] { new Point(0, 0), new Point(sz.W, sz.H) };
		}
		public void SetLayerTextureCropCoordinates(int layer, Point topLeft, Point downRight)
		{
			if (ErrorIfNoTexture()) return;
			texCoords[layer] = new Point[] { topLeft, downRight };
		}
		public void SetLayerTextureCropTile(int layer, Point tileIndexes)
		{
			if (ErrorIfNoTexture()) return;
			texCoords[layer] = new Point[]
			{
				new Point(TileSize.W, TileSize.H) * tileIndexes,
				new Point(TileSize.W, TileSize.H) * (tileIndexes + new Point(1, 1))
			};
		}
		public void SetLayerTextureCropPercent(int layer, Point topLeftPercent, Point downRightPercent)
		{
			if (ErrorIfNoTexture()) return;
			topLeftPercent /= 100;
			downRightPercent /= 100;
			var sz = TexturePath != null && Assets.textures.ContainsKey(TexturePath) ? Assets.textures[TexturePath].Size :
				new Vector2u(100, 100);
			texCoords[layer] = new Point[]
			{
				new Point(sz.X * topLeftPercent.X, sz.Y * topLeftPercent.Y),
				new Point(sz.X * downRightPercent.X, sz.Y * downRightPercent.Y)
			};
		}

		public void SetLayerColorTint(int layer, Data.Color color)
		{
			colors[layer] = color;
		}
		private Size GetTextureSize()
		{
			var area = AreaUniqueID == null ? (Area)PickByUniqueID(AreaUniqueID) : null;
			var sz = TexturePath == null || Assets.textures.ContainsKey(TexturePath) == false ?
				(area == null ? new Size(100, 100) : area.Size) :
				new Size(Assets.textures[TexturePath].Size.X, Assets.textures[TexturePath].Size.Y);
			return sz;
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
	}
}
