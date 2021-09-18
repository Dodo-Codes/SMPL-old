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
	public class ShapePseudo3D : Visual
	{
		public enum Side { Far, Near, Left, Right, Up, Down }

		private readonly Dictionary<Side, Point[]> texCoords = new();
		private readonly Dictionary<Side, Data.Color> colors = new();
		private readonly List<Side> skippedSides = new();
		private string texturePath;
		private double lightAngle, lightDepth, percentZ;
		internal Line[] lines = new Line[5];

		[JsonProperty]
		public double PercentZ
		{
			get { return percentZ; }
			set { percentZ = Number.Limit(value, new Number.Range(double.NegativeInfinity, 100)); }
		}
		[JsonProperty]
		public double PercentDepth { get; set; } = 100;
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
				if (prev == null) SetSidesTextureCropDefault();
			}
		}
		[JsonProperty]
		public bool IsRepeated { get; set; }
		[JsonProperty]
		public bool IsSmooth { get; set; }
		[JsonProperty]
		public Size TileSize { get; set; } = new Size(32, 32);

		public ShapePseudo3D(string uniqueID) : base(uniqueID)
		{
			for (int i = 0; i < 6; i++)
				texCoords[(Side)i] = new Point[] { new Point(0, 0), new Point(100, 100) };
			for (int i = 0; i < 6; i++)
				colors[(Side)i] = Data.Color.White;

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
					$"Cannot display the {nameof(ShapePseudo3D)} instance '{UniqueID}' because it has no {nameof(Components.Area)}.\n" +
					$"Make sure the {nameof(ShapePseudo3D)} instance has an {nameof(Components.Area)} before displaying it.");
				return;
			}
			if (camera.Captures(this) == false || PercentZ < -10_000) return;

			UpdateSprite();

			// those lines are updated in the camera.Captures test
			var tl = lines[0];
			var tr = lines[1];
			var br = lines[2];
			var bl = lines[3];

			var quads = new SortedDictionary<string, Quad>();
			var dists = new SortedDictionary<double, Quad>();

			AddQuads();
			Draw();

			void UpdateSprite()
			{
				Area.sprite.Position = Point.From(Area.Position);
				Area.sprite.Rotation = (float)Area.Angle;
				Area.sprite.Scale = new Vector2f(1, 1);
				Area.sprite.Origin = new Vector2f(
						(float)(Area.Size.W * (Area.OriginPercent.X / 100)),
						(float)(Area.Size.H * (Area.OriginPercent.Y / 100)));
			}
			void AddQuads()
			{
				AddFaceQuad(Side.Far, tl.StartPosition, tr.StartPosition, br.StartPosition, bl.StartPosition);
				AddSideQuad(Side.Left, tl, bl, 0.02);
				AddSideQuad(Side.Right, br, tr, 0.04);
				AddSideQuad(Side.Up, tr, tl, 0.06);
				AddSideQuad(Side.Down, bl, br, 0.08);
				foreach (var kvp in dists)
					quads.Add($"{kvp.Key}", kvp.Value);
				AddFaceQuad(Side.Near, tl.EndPosition, tr.EndPosition, br.EndPosition, bl.EndPosition);

				void AddSideQuad(Side side, Line l, Line r, double distOffset)
				{
					l.EndPosition = new Point(l.EndPosition.X, l.EndPosition.Y) { Color = colors[side] }; // top left
					r.EndPosition = new Point(r.EndPosition.X, r.EndPosition.Y) { Color = colors[side] }; // top right
					r.StartPosition = new Point(r.StartPosition.X, r.StartPosition.Y) { Color = colors[side] }; // bottom right
					l.StartPosition = new Point(l.StartPosition.X, l.StartPosition.Y) { Color = colors[side] }; // bottom left
					if (skippedSides.Contains(side) == false)
					{
						dists.Add(99_999_999.0 - Point.Distance(camera.Position,
							Point.PercentTowardTarget(l.StartPosition, r.StartPosition, new Size(50, 50))) + distOffset,
						new Quad(
							new Corner(l.EndPosition, new Point(texCoords[side][0].X, texCoords[side][0].Y)), // top left
							new Corner(r.EndPosition, new Point(texCoords[side][1].X, texCoords[side][0].Y)), // top right
							new Corner(r.StartPosition, new Point(texCoords[side][1].X, texCoords[side][1].Y)), // bottom right
							new Corner(l.StartPosition, new Point(texCoords[side][0].X, texCoords[side][1].Y)))); // bottom left
					}
				}
				void AddFaceQuad(Side side, Point tl, Point tr, Point br, Point bl)
				{
					if (skippedSides.Contains(side) == false)
					{
						tl = new Point(tl.X, tl.Y) { Color = colors[side] };
						tr = new Point(tr.X, tr.Y) { Color = colors[side] };
						br = new Point(br.X, br.Y) { Color = colors[side] };
						bl = new Point(bl.X, bl.Y) { Color = colors[side] };
						quads.Add($"{side}", new Quad(
							new Corner(tl, new Point(texCoords[side][0].X, texCoords[side][0].Y)),
							new Corner(tr, new Point(texCoords[side][1].X, texCoords[side][0].Y)),
							new Corner(br, new Point(texCoords[side][1].X, texCoords[side][1].Y)),
							new Corner(bl, new Point(texCoords[side][0].X, texCoords[side][1].Y))));
					};
				}
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
			}
		}

		public void SetSideTextureCropCoordinates(Side side, Point topLeft, Point downRight)
		{
			if (ErrorIfNoTexture()) return;
			texCoords[side] = new Point[] { topLeft, downRight };
		}
		public void SetSideTextureCropTile(Side side, Point tileIndexes)
		{
			if (ErrorIfNoTexture()) return;
			texCoords[side] = new Point[]
			{
				new Point(TileSize.W, TileSize.H) * tileIndexes,
				new Point(TileSize.W, TileSize.H) * (tileIndexes + new Point(1, 1))
			};
		}
		public void SetSideTextureCropPercent(Side side, Point topLeftPercent, Point downRightPercent)
		{
			if (ErrorIfNoTexture()) return;
			topLeftPercent /= 100;
			downRightPercent /= 100;
			var sz = TexturePath != null && Assets.textures.ContainsKey(TexturePath) ? Assets.textures[TexturePath].Size :
				new Vector2u(100, 100);
			texCoords[side] = new Point[]
			{
				new Point(sz.X * topLeftPercent.X, sz.Y * topLeftPercent.Y),
				new Point(sz.X * downRightPercent.X, sz.Y * downRightPercent.Y)
			};
		}
		public void SetSidesTextureCropDefault()
		{
			if (ErrorIfNoTexture()) return;

			var sz = GetTextureSize();
			for (int i = 0; i < 6; i++)
				texCoords[(Side)i] = new Point[] { new Point(0, 0), new Point(sz.W, sz.H) };
		}

		public void SetSideColorTint(Side side, Data.Color color)
		{
			colors[side] = color;
		}
		public void SetColorTintLight(double angle, double depth)
		{
			lightAngle = angle;
			lightDepth = depth;

			var Area = (Area)PickByUniqueID(AreaUniqueID);
			if (Area == null || Area.IsDestroyed || Area.sprite == null)
			{
				Debug.LogError(1,
					$"Cannot tint the {nameof(ShapePseudo3D)} instance '{UniqueID}' because it has no {nameof(Components.Area)}.\n" +
					$"Make sure the {nameof(ShapePseudo3D)} instance has an " +
					$"{nameof(Components.Area)} before tinting it according to a light.");
				return;
			}

			var ang = Number.To360(lightAngle - Area.Angle);
			var botSide = Number.IsBetween(ang, new Number.Range(0, 180), true, true);
			var leftSide = Number.IsBetween(ang, new Number.Range(90, 270), true, true);

			colors[Side.Far] = Data.Color.GrayDark;
			if (skippedSides.Count > 0)
				colors[Side.Far] = Shade(Number.Map(lightDepth,
					new Number.Range(0, PercentDepth / skippedSides.Count * 2), new Number.Range(50, 255)));
			colors[Side.Left] = Shade(Number.Map(ang, new Number.Range(ang > 180 ? 360 : 0, 180), new Number.Range(50, 255)));
			colors[Side.Right] = Shade(Number.Map(ang, new Number.Range(180, botSide ? 0 : 360), new Number.Range(50, 255)));

			colors[Side.Down] = leftSide ? Shade(Number.Map(ang, new Number.Range(270, 90), new Number.Range(50, 255)))
					: botSide ? Shade(Number.Map(ang, new Number.Range(0, 90), new Number.Range(150, 255)))
					: Shade(Number.Map(ang, new Number.Range(270, 360), new Number.Range(50, 150)));

			colors[Side.Up] = leftSide ? Shade(Number.Map(ang, new Number.Range(90, 270), new Number.Range(50, 255)))
					: botSide == false ? Shade(Number.Map(ang, new Number.Range(360, 270), new Number.Range(150, 255)))
					: Shade(Number.Map(ang, new Number.Range(90, 0), new Number.Range(50, 150)));
			colors[Side.Near] = Shade(Number.Map(lightDepth, new Number.Range(0, PercentDepth * 2), new Number.Range(50, 255)));

			Data.Color Shade(double s) => new(s, s, s);
		}

		public void SetSideVisibility(Side side, bool display)
		{
			if (display && skippedSides.Contains(side)) skippedSides.Remove(side);
			else if (display == false && skippedSides.Contains(side) == false) skippedSides.Add(side);
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
		private Size GetTextureSize()
		{
			var area = AreaUniqueID == null ? (Area)PickByUniqueID(AreaUniqueID) : null;
			var sz = TexturePath == null || Assets.textures.ContainsKey(TexturePath) == false ?
				(area == null ? new Size(100, 100) : area.Size) :
				new Size(Assets.textures[TexturePath].Size.X, Assets.textures[TexturePath].Size.Y);
			return sz;
		}
		internal void UpdateLines(Camera camera)
		{
			var Area = (Area)PickByUniqueID(AreaUniqueID);
			if (Area == null) return;

			var mid = P(Area.sprite.Position + Point.From(new Point(Area.Size.W / 2, Area.Size.H / 2)));
			var topL = P(Area.sprite.Position);
			var topR = P(Area.sprite.Position + Point.From(new Point(Area.Size.W, 0)));
			var botR = P(Area.sprite.Position + Point.From(new Point(Area.Size.W, Area.Size.H)));
			var botL = P(Area.sprite.Position + Point.From(new Point(0, Area.Size.H)));

			var tl = new Line(topL, topL);
			var tr = new Line(topR, topR);
			var br = new Line(botR, botR);
			var bl = new Line(botL, botL);

			tl.StartPosition = ApplyZ(tl.StartPosition);
			tr.StartPosition = ApplyZ(tr.StartPosition);
			br.StartPosition = ApplyZ(br.StartPosition);
			bl.StartPosition = ApplyZ(bl.StartPosition);

			tl.EndPosition = ApplyHeight(tl.StartPosition);
			tr.EndPosition = ApplyHeight(tr.StartPosition);
			br.EndPosition = ApplyHeight(br.StartPosition);
			bl.EndPosition = ApplyHeight(bl.StartPosition);

			tl.EndPosition = ApplyHeightCorrection(tl.StartPosition, tl.EndPosition);
			tr.EndPosition = ApplyHeightCorrection(tr.StartPosition, tr.EndPosition);
			br.EndPosition = ApplyHeightCorrection(br.StartPosition, br.EndPosition);
			bl.EndPosition = ApplyHeightCorrection(bl.StartPosition, bl.EndPosition);


			lines[0] = tl;
			lines[1] = tr;
			lines[2] = br;
			lines[3] = bl;

			Point P(Vector2f vec)
			{
				return Point.To(Area.sprite.Transform.TransformPoint(vec));
			}
			Point ApplyZ(Point point)
			{
				return Point.PercentTowardTarget(point, camera.Position, new(PercentZ, PercentZ));
			}
			Point ApplyHeight(Point point)
			{
				return Point.PercentTowardTarget(point, camera.Position, new Size(PercentDepth, PercentDepth) / 10);
			}
			Point ApplyHeightCorrection(Point pointFar, Point pointNear)
			{
				var sz = new Size(PercentZ, PercentZ);
				sz.W = Number.Limit(sz.W, new Number.Range(-PercentDepth, 100));
				sz.H = Number.Limit(sz.H, new Number.Range(-PercentDepth, 100));
				return Point.PercentTowardTarget(pointNear, pointFar, sz);
			}
		}
	}
}
