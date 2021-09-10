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

		private Dictionary<Side, Point[]> texCoords = new();
		private Dictionary<Side, Data.Color> colors = new();
		private string texturePath;
		private double lightAngle, lightDepth;

		[JsonProperty]
		public double Depth { get; set; } = 100;
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
				if (prev == null) SetTextureCoordinatesDefault();
			}
		}
		[JsonProperty]
		public bool IsPyramid { get; set; }

		public ShapePseudo3D(string uniqueID) : base(uniqueID)
		{
			for (int i = 0; i < 6; i++)
				texCoords[(Side)i] = new Point[] { new Point(0, 0), new Point(100, 100) };
			SetColorTintLight(60, 100);

			if (cannotCreate) { ErrorAlreadyHasUID(uniqueID); Destroy(); }
		}
		public override void Destroy()
		{
			if (ErrorIfDestroyed()) return;
			base.Destroy();
		}

		public void Display(Camera camera)
		{
			if (ErrorIfDestroyed()) return;
			if (Window.DrawNotAllowed() || visualMaskingUID != null || IsHidden) return;
			var Area = (Area)PickByUniqueID(AreaUniqueID);
			if (Area == null || Area.IsDestroyed || Area.sprite == null)
			{
				Debug.LogError(1,
					$"Cannot display the {nameof(ShapePseudo3D)} instance '{UniqueID}' because it has no {nameof(Components.Area)}.\n" +
					$"Make sure the {nameof(ShapePseudo3D)} instance has an {nameof(Components.Area)} before displaying it.");
				return;
			}

			Area.sprite.Position = Point.From(Area.Position);
			Area.sprite.Rotation = (float)Area.Angle;
			Area.sprite.Scale = new Vector2f(1, 1);
			Area.sprite.Origin = new Vector2f(
					(float)(Area.Size.W * (Area.OriginPercent.X / 100)),
					(float)(Area.Size.H * (Area.OriginPercent.Y / 100)));

			var angle = Number.To360(lightAngle - Area.Angle);
			var botSide = Number.IsBetween(angle, new Number.Range(0, 180), true, true);
			var leftSide = Number.IsBetween(angle, new Number.Range(90, 270), true, true);

			colors[Side.Far] = Data.Color.GrayDark;
			colors[Side.Left] = Shade(Number.Map(angle, new Number.Range(angle > 180 ? 360 : 0, 180), new Number.Range(50, 255)));
			colors[Side.Right] = Shade(Number.Map(angle, new Number.Range(180, botSide ? 0 : 360), new Number.Range(50, 255)));

			if (leftSide) colors[Side.Down] = Shade(Number.Map(angle, new Number.Range(270, 90), new Number.Range(50, 255)));
			else
			{
				if (botSide) colors[Side.Down] = Shade(Number.Map(angle, new Number.Range(0, 90), new Number.Range(150, 255)));
				else colors[Side.Down] = Shade(Number.Map(angle, new Number.Range(270, 360), new Number.Range(50, 150)));
			}

			if (leftSide) colors[Side.Up] = Shade(Number.Map(angle, new Number.Range(90, 270), new Number.Range(50, 255)));
			else
			{
				if (botSide == false) colors[Side.Up] = Shade(Number.Map(angle, new Number.Range(360, 270), new Number.Range(150, 255)));
				else colors[Side.Up] = Shade(Number.Map(angle, new Number.Range(90, 0), new Number.Range(50, 150)));
			}
			colors[Side.Near] = Shade(Number.Map(lightDepth, new Number.Range(0, Depth * 2), new Number.Range(50, 255)));

			var topL = P(Area.sprite.Position);
			var topR = P(Area.sprite.Position + Point.From(new Point(Area.Size.W, 0)));
			var botR = P(Area.sprite.Position + Point.From(new Point(Area.Size.W, Area.Size.H)));
			var botL = P(Area.sprite.Position + Point.From(new Point(0, Area.Size.H)));
			var mid = P(Area.sprite.Position + Point.From(new Point(Area.Size.W / 2, Area.Size.H / 2)));

			var tl = L(topL);
			var tr = L(topR);
			var br = L(botR);
			var bl = L(botL);
			var ml = L(mid).EndPosition;

			topL.Color = colors[Side.Far];
			topR.Color = colors[Side.Far];
			botR.Color = colors[Side.Far];
			botL.Color = colors[Side.Far];
			var quads = new Dictionary<string, Quad>()
			{ { "far", new Quad(
				new Corner(topL, texCoords[Side.Far][0].X, texCoords[Side.Far][0].Y),
				new Corner(topR, texCoords[Side.Far][1].X, texCoords[Side.Far][0].Y),
				new Corner(botR, texCoords[Side.Far][1].X, texCoords[Side.Far][1].Y),
				new Corner(botL, texCoords[Side.Far][0].X, texCoords[Side.Far][1].Y)) } };

			var dists = new SortedDictionary<double, Quad>();
			// left
			tl.EndPosition = new Point(tl.EndPosition.X, tl.EndPosition.Y) { Color = colors[Side.Left] };
			bl.EndPosition = new Point(bl.EndPosition.X, bl.EndPosition.Y) { Color = colors[Side.Left] };
			bl.StartPosition = new Point(bl.StartPosition.X, bl.StartPosition.Y) { Color = colors[Side.Left] };
			tl.StartPosition = new Point(tl.StartPosition.X, tl.StartPosition.Y) { Color = colors[Side.Left] };
			dists.Add(99_999_999.0 - Point.Distance(camera.Position, Point.PercentTowardTarget(topL, botL, new Size(50, 50))) + 0.02,
				new Quad(
					new Corner(IsPyramid ? ml : tl.EndPosition, texCoords[Side.Left][0].X, texCoords[Side.Left][0].Y),
					new Corner(IsPyramid ? ml : bl.EndPosition, texCoords[Side.Left][1].X, texCoords[Side.Left][0].Y),
					new Corner(bl.StartPosition, texCoords[Side.Left][1].X, texCoords[Side.Left][1].Y),
					new Corner(tl.StartPosition, texCoords[Side.Left][0].X, texCoords[Side.Left][1].Y)));
			// right
			br.EndPosition = new Point(br.EndPosition.X, br.EndPosition.Y) { Color = colors[Side.Right] };
			tr.EndPosition = new Point(tr.EndPosition.X, tr.EndPosition.Y) { Color = colors[Side.Right] };
			tr.StartPosition = new Point(tr.StartPosition.X, tr.StartPosition.Y) { Color = colors[Side.Right] };
			br.StartPosition = new Point(br.StartPosition.X, br.StartPosition.Y) { Color = colors[Side.Right] };
			dists.Add(99_999_999.0 - Point.Distance(camera.Position, Point.PercentTowardTarget(topR, botR, new Size(50, 50))) + 0.06,
					new Quad(
						new Corner(IsPyramid ? ml : br.EndPosition, texCoords[Side.Right][0].X, texCoords[Side.Right][0].Y),
						new Corner(IsPyramid ? ml : tr.EndPosition, texCoords[Side.Right][1].X, texCoords[Side.Right][0].Y),
						new Corner(tr.StartPosition, texCoords[Side.Right][1].X, texCoords[Side.Right][1].Y),
						new Corner(br.StartPosition, texCoords[Side.Right][0].X, texCoords[Side.Down][1].Y)));
			// up
			tr.EndPosition = new Point(tr.EndPosition.X, tr.EndPosition.Y) { Color = colors[Side.Up] };
			tl.EndPosition = new Point(tl.EndPosition.X, tl.EndPosition.Y) { Color = colors[Side.Up] };
			tl.StartPosition = new Point(tl.StartPosition.X, tl.StartPosition.Y) { Color = colors[Side.Up] };
			tr.StartPosition = new Point(tr.StartPosition.X, tr.StartPosition.Y) { Color = colors[Side.Up] };
			dists.Add(99_999_999.0 - Point.Distance(camera.Position, Point.PercentTowardTarget(topR, topL, new Size(50, 50))) + 0.08,
					new Quad(
						new Corner(IsPyramid ? ml : tr.EndPosition, texCoords[Side.Up][0].X, texCoords[Side.Up][0].Y),
						new Corner(IsPyramid ? ml : tl.EndPosition, texCoords[Side.Up][1].X, texCoords[Side.Up][0].Y),
						new Corner(tl.StartPosition, texCoords[Side.Up][1].X, texCoords[Side.Up][1].Y),
						new Corner(tr.StartPosition, texCoords[Side.Up][0].X, texCoords[Side.Down][1].Y)));
			// bot
			bl.EndPosition = new Point(bl.EndPosition.X, bl.EndPosition.Y) { Color = colors[Side.Down] };
			br.EndPosition = new Point(br.EndPosition.X, br.EndPosition.Y) { Color = colors[Side.Down] };
			br.StartPosition = new Point(br.StartPosition.X, br.StartPosition.Y) { Color = colors[Side.Down] };
			bl.StartPosition = new Point(bl.StartPosition.X, bl.StartPosition.Y) { Color = colors[Side.Down] };
			dists.Add(99_999_999.0 - Point.Distance(camera.Position, Point.PercentTowardTarget(botR, botL, new Size(50, 50))) + 0.04,
					new Quad(
						new Corner(IsPyramid ? ml : bl.EndPosition, texCoords[Side.Down][0].X, texCoords[Side.Down][0].Y),
						new Corner(IsPyramid ? ml : br.EndPosition, texCoords[Side.Down][1].X, texCoords[Side.Down][0].Y),
						new Corner(br.StartPosition, texCoords[Side.Down][1].X, texCoords[Side.Down][1].Y),
						new Corner(bl.StartPosition, texCoords[Side.Down][0].X, texCoords[Side.Down][1].Y)));
			foreach (var kvp in dists)
				quads.Add($"{kvp.Key}", kvp.Value);

			if (IsPyramid == false)
			{
				tl.EndPosition = new Point(tl.EndPosition.X, tl.EndPosition.Y) { Color = colors[Side.Near] };
				tr.EndPosition = new Point(tr.EndPosition.X, tr.EndPosition.Y) { Color = colors[Side.Near] };
				br.EndPosition = new Point(br.EndPosition.X, br.EndPosition.Y) { Color = colors[Side.Near] };
				bl.EndPosition = new Point(bl.EndPosition.X, bl.EndPosition.Y) { Color = colors[Side.Near] };
				quads.Add("near", new Quad(
					new Corner(tl.EndPosition, texCoords[Side.Near][0].X, texCoords[Side.Near][0].Y),
					new Corner(tr.EndPosition, texCoords[Side.Near][1].X, texCoords[Side.Near][0].Y),
					new Corner(br.EndPosition, texCoords[Side.Near][1].X, texCoords[Side.Near][1].Y),
					new Corner(bl.EndPosition, texCoords[Side.Near][0].X, texCoords[Side.Near][1].Y)));
			};

			var texture = TexturePath != null && Assets.textures.ContainsKey(TexturePath) ? Assets.textures[TexturePath] : null;
			camera.rendTexture.Draw(Components.Sprite.QuadsToVerts(quads).Item2, PrimitiveType.Quads, new RenderStates(texture));

			Point P(Vector2f vec) => Point.To(Area.sprite.Transform.TransformPoint(vec));
			Line L(Point p)
			{
				var ang = Number.AngleBetweenPoints(camera.Position, p);
				var len = Point.Distance(camera.Position, p);
				var sz = (camera.Size.W + camera.Size.H) / 1000;
				return new Line(p, Point.MoveAtAngle(p, ang, len * (Depth / 300) / sz, Gear.Time.Unit.Tick));
			}
			Data.Color Shade(double s) => new Data.Color(s, s, s);
		}
		public void SetSideTextureCoordinates(Side side, Point topLeft, Point downRight)
		{
			texCoords[side] = new Point[] { topLeft, downRight };
		}
		public void SetTextureCoordinatesDefault()
		{
			if (ErrorIfNoTexture()) return;

			var area = AreaUniqueID == null ? (Area)PickByUniqueID(AreaUniqueID) : null;
			var sz = TexturePath == null || Assets.textures.ContainsKey(TexturePath) == false ?
				(area == null ? new Size(100, 100) : area.Size) :
				new Size(Assets.textures[TexturePath].Size.X, Assets.textures[TexturePath].Size.Y);

			for (int i = 0; i < 6; i++)
				texCoords[(Side)i] = new Point[] { new Point(0, 0), new Point(sz.W, sz.H) };
		}
		public void SetSideColorTint(Side side, Data.Color color)
		{
			colors[side] = color;
		}
		public void SetColorTint(Data.Color color)
		{
			if (ErrorIfNoTexture()) return;
			for (int i = 0; i < 6; i++)
				colors[(Side)i] = color;
		}
		public void SetColorTintLight(double angle, double depth)
		{
			lightAngle = angle;
			lightDepth = depth;
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
