using System.Collections.Generic;
using Newtonsoft.Json;
using SFML.Graphics;
using SFML.System;
using SMPL.Data;
using SMPL.Gear;

namespace SMPL.Components
{
	public class Area : Thing
	{
		private static double biggestSize;
		private static readonly SFML.Graphics.Text coordinatesText = new();
		private static readonly Dictionary<Point, List<string>> chunks = new();
		[JsonProperty]
		private readonly List<string> hitboxUIDs = new();
		private Point localPosition, originPercent;
		private double localAngle;
		private Size localSize;

		//===============

		[JsonProperty]
		internal SFML.Graphics.Sprite sprite = new();
		[JsonProperty]
		internal SFML.Graphics.Text text = new();
		[JsonProperty]
		internal string familyUID;

		internal void UpdateHitboxes()
		{
			sprite.Position = Point.From(Position);
			sprite.Rotation = (float)Angle;
			for (int i = 0; i < hitboxUIDs.Count; i++)
			{
				var hitbox = (Hitbox)PickByUniqueID(hitboxUIDs[i]);
				var lines = hitbox.lines;
				foreach (var kvp in lines)
				{
					var localLine = hitbox.localLines[kvp.Key];
					var sp = Point.To(sprite.Transform.TransformPoint(Point.From(localLine.StartPosition)));
					var ep = Point.To(sprite.Transform.TransformPoint(Point.From(localLine.EndPosition)));
					hitbox.SetLine(kvp.Key, new Line(sp, ep));
				}
			}
		}
		internal void UpdateChunkInfo()
		{
			sprite.Position = Point.From(Position);
			sprite.Rotation = (float)Angle;

			var off = ChunkSize / 2;
			var newChunkPos = new Point((int)((Position.X + off) / ChunkSize), (int)((Position.Y + off) / ChunkSize)) * ChunkSize;

			if (ChunkPosition == newChunkPos) return;

			if (chunks.ContainsKey(ChunkPosition)) chunks[ChunkPosition].Remove(UniqueID);
			ChunkPosition = newChunkPos;
			if (chunks.ContainsKey(ChunkPosition) == false || chunks[ChunkPosition] == null) chunks[ChunkPosition] = new();
			chunks[ChunkPosition].Add(UniqueID);
		}
		internal void DefaultSprite()
		{
			sprite.Position = new Vector2f();
			sprite.Rotation = 0;
			sprite.Scale = new Vector2f(1, 1);
			sprite.Origin = new Vector2f(0, 0);
		}
		internal void UpdateSprite()
		{
			sprite.Position = Point.From(Position);
			sprite.TextureRect = new IntRect(
				new Vector2i((int)Position.X, (int)Position.Y),
				new Vector2i((int)Size.W, (int)Size.H));
			sprite.Rotation = (float)Angle;
			sprite.Scale = new Vector2f(1, 1);
			sprite.Origin = new Vector2f(
					(float)(Size.W * (OriginPercent.X / 100)),
					(float)(Size.H * (OriginPercent.Y / 100)));
		}

		//==============

		public string[] NeighbourAreaUniqueIDs
		{
			get
			{
				var result = new List<string>();
				for (int y = -1; y < 2; y++)
					for (int x = -1; x < 2; x++)
						result.AddRange(GetAreaUniqueIDsFromChunk(x, y));
				return result.ToArray();
			}
		}
		public static double ChunkSize { get; private set; } = 100;
		public Point ChunkPosition { get; private set; }

		[JsonProperty]
		public Point Position
		{
			get { return ErrorIfDestroyed() ? Point.Invalid : PositionFromLocal(LocalPosition); }
			set
			{
				if (ErrorIfDestroyed()) return;
				localPosition = PositionToLocal(value);
				UpdateHitboxes();
				UpdateChunkInfo();
			}
		}
		[JsonProperty]
		public double Angle
		{
			get { return ErrorIfDestroyed() ? double.NaN : AngleFromLocal(LocalAngle); }
			set
			{
				if (ErrorIfDestroyed()) return;
				localAngle = AngleToLocal(value);
				UpdateHitboxes();
				UpdateChunkInfo();
			}
		}
		[JsonProperty]
		public Size Size
		{
			get { return ErrorIfDestroyed() ? Size.Invalid : SizeFromLocal(LocalSize); }
			set
			{
				if (ErrorIfDestroyed()) return;
				localSize = SizeToLocal(value);

				if (UniqueID != Camera.WorldCameraAreaUID)
				{
					if (value.W > biggestSize) biggestSize = value.W;
					if (value.H > biggestSize) biggestSize = value.H;
					ChunkSize = biggestSize * 1.2;
				}

				UpdateHitboxes();
				UpdateChunkInfo();
			}
		}
		[JsonProperty]
		public Point OriginPercent
		{
			get { return ErrorIfDestroyed() ? Point.Invalid : originPercent; }
			set
			{
				if (ErrorIfDestroyed()) return;
				originPercent = new Point(
					Number.Limit(value.X, new Number.Range(0, 100)),
					Number.Limit(value.Y, new Number.Range(0, 100)));
				UpdateHitboxes();
				UpdateChunkInfo();
			}
		}

		[JsonProperty]
		public Point LocalPosition
		{
			get { return ErrorIfDestroyed() ? Point.Invalid : localPosition; }
			set
			{
				if (ErrorIfDestroyed()) return;
				localPosition = value;
				UpdateHitboxes();
				UpdateChunkInfo();
			}
		}
		[JsonProperty]
		public double LocalAngle
		{
			get { return ErrorIfDestroyed() ? double.NaN : localAngle; }
			set
			{
				if (ErrorIfDestroyed()) return;
				localAngle = value;
				UpdateHitboxes();
				UpdateChunkInfo();
			}
		}
		[JsonProperty]
		public Size LocalSize
		{
			get { return ErrorIfDestroyed() ? Size.Invalid : localSize; }
			set
			{
				if (ErrorIfDestroyed()) return;
				localSize = value;
				UpdateHitboxes();
				UpdateChunkInfo();
			}
		}

		public Area(string uniqueID) : base(uniqueID)
		{
			Size = new Size(100, 100);
			OriginPercent = new Point(50, 50);

			// trigger the chunk addition
			if (uniqueID != Camera.WorldCameraAreaUID)
			{
				Position = new Point(Area.ChunkSize * 3, 0);
				Position = new Point(0, 0);
			}

			UpdateHitboxes();
			if (cannotCreate) { ErrorAlreadyHasUID(uniqueID); Destroy(); }
		}
		public override void Destroy()
		{
			if (ErrorIfDestroyed()) return;
			hitboxUIDs.Clear();
			familyUID = null;
			sprite.Dispose();
			text.Dispose();
			base.Destroy();
		}

		public void Display(Camera camera, double width, Data.Color color)
		{
			if (ErrorIfDestroyed() || Window.DrawNotAllowed()) return;
			UpdateSprite();

			var tl = Point.To(sprite.Transform.TransformPoint(Point.From(Position)));
			var tr = Point.To(sprite.Transform.TransformPoint(Point.From(Position + new Point(Size.W, 0))));
			var br = Point.To(sprite.Transform.TransformPoint(Point.From(Position + new Point(Size.W, Size.H))));
			var bl = Point.To(sprite.Transform.TransformPoint(Point.From(Position + new Point(0, Size.H))));
			tl.Color = color;
			tr.Color = color;
			br.Color = color;
			bl.Color = color;
			new Line(tl, tr).Display(camera, width);
			new Line(tr, br).Display(camera, width);
			new Line(br, bl).Display(camera, width);
			new Line(bl, tl).Display(camera, width);
			Position.Display(camera, width);
		}

		public string[] GetAreaUniqueIDsFromChunk(int relativeX, int relativeY)
		{
			var p = ChunkPosition + new Point(relativeX * ChunkSize, relativeY * ChunkSize);
			return chunks.ContainsKey(p) == false ? System.Array.Empty<string>() : chunks[p].ToArray();
		}
		public static void DisplayChunks(Camera camera, double bordersWidth, Data.Color color, string coordinatesFont = null)
		{
			var area = (Area)PickByUniqueID(camera.AreaDisplayUniqueID);
			var cameraSquareSize = camera.Size.W > camera.Size.H ? camera.Size.W : camera.Size.H;
			var borderAmount = cameraSquareSize / ChunkSize;
			var topY = area.Position.Y - cameraSquareSize / 2;
			var botY = area.Position.Y + cameraSquareSize / 2;
			var leftX = area.Position.X - cameraSquareSize / 2;
			var rightX = area.Position.X + cameraSquareSize / 2;
			var borderOffset = (cameraSquareSize % ChunkSize) / 2;

			if ((int)(cameraSquareSize / ChunkSize) % 2 == 0) borderOffset += ChunkSize / 2;

			for (int y = 0; y < borderAmount; y++)
				for (int x = 0; x < borderAmount; x++)
				{
					var curX = (camera.Position.X - borderOffset) - (camera.Position.X % ChunkSize + x * ChunkSize) + cameraSquareSize / 2;
					var curY = (camera.Position.Y - borderOffset) - (camera.Position.Y % ChunkSize + y * ChunkSize) + cameraSquareSize / 2;
					var top = new Point(curX, topY + curY) { Color = color };
					var bot = new Point(curX, botY + curY) { Color = color };
					var left = new Point(leftX + curX, curY) { Color = color };
					var right = new Point(rightX + curX, curY) { Color = color };
					var off = ChunkSize / 2;

					new Line(top, bot).Display(camera, bordersWidth);
					new Line(left, right).Display(camera, bordersWidth);
					new Point(curX + off, curY + off) { Color = color }.Display(camera, bordersWidth * 4);

					if (coordinatesFont == null || Assets.fonts.ContainsKey(coordinatesFont) == false) continue;
					coordinatesText.Font = Assets.fonts[coordinatesFont];
					coordinatesText.Position = Point.From(new Point(curX, curY));
					coordinatesText.DisplayedString = $" {(int)(curX + off)}\n\n\n {(int)(curY + off)}";
					coordinatesText.CharacterSize = 120;
					coordinatesText.FillColor = Data.Color.From(color);
					var sc = ChunkSize / 600;
					coordinatesText.Scale = Size.From(new Size(sc, sc));
					camera.rendTexture.Draw(coordinatesText);
				}
		}
		public static Point PositionToParallax(Point position, Size parallaxPercent, Camera camera)
		{
			parallaxPercent += new Size(100, 100);
			var x = Number.FromPercent(parallaxPercent.W, new Number.Range(-camera.Position.X, position.X));
			var y = Number.FromPercent(parallaxPercent.H, new Number.Range(-camera.Position.Y, position.Y));
			return new Point(x, y);
		}
		public static double AngleToParallax(double angle, double parallaxPercent, Camera camera)
		{
			parallaxPercent /= 100;
			return Number.MoveTowardAngle(angle, camera.Angle, (camera.Angle - angle) * parallaxPercent, Gear.Time.Unit.Tick);
		}
		public static Size SizeToParallax(Size size, Size parallaxPercent, Camera camera)
		{
			parallaxPercent /= 100;
			var sc = (camera.Size / camera.startSize) * parallaxPercent;
			return size * sc;
		}

		public void AddHitboxes(params string[] hitboxUniqueIDs)
		{
			if (ErrorIfDestroyed()) return;
			if (hitboxUniqueIDs == null)
			{
				Debug.LogError(1, "The collection of ComponentHitbox instances cannot be 'null'.");
				return;
			}
			for (int i = 0; i < hitboxUniqueIDs.Length; i++)
				if (hitboxUIDs.Contains(hitboxUniqueIDs[i]) == false)
					hitboxUIDs.Add(hitboxUniqueIDs[i]);
		}
		public void RemoveHitboxes(params string[] hitboxUniqueIDs)
		{
			if (ErrorIfDestroyed()) return;
			if (hitboxUniqueIDs == null)
			{
				Debug.LogError(1, "The collection of ComponentHitbox instances cannot be 'null'.");
				return;
			}
			for (int i = 0; i < hitboxUniqueIDs.Length; i++)
				if (hitboxUIDs.Contains(hitboxUniqueIDs[i]))
					hitboxUIDs.Remove(hitboxUniqueIDs[i]);
		}
		public void RemoveAllHitboxes()
		{
			if (ErrorIfDestroyed()) return;
			hitboxUIDs.Clear();
		}
		public bool HasHitboxes(params string[] hitboxUniqueIDs)
		{
			if (hitboxUniqueIDs == null)
			{
				Debug.LogError(1, "The collection of ComponentHitbox instances cannot be 'null'.");
				return false;
			}
			for (int i = 0; i < hitboxUniqueIDs.Length; i++)
				if (hitboxUIDs.Contains(hitboxUniqueIDs[i]) == false)
					return false;
			return true;
		}

		public Point PositionFromLocal(Point localPosition)
		{
			var family = (Family)PickByUniqueID(familyUID);
			var parent = family == null ? null : (Visual)PickByUniqueID(family.VisualParentUniqueID);
			var parentArea = parent == null ? null : (Area)PickByUniqueID(parent.AreaUniqueID);
			return ErrorIfDestroyed() ? Point.Invalid :
				family == null || family.VisualParentUniqueID == null ? localPosition :
				Point.To(parentArea.sprite.Transform.TransformPoint(Point.From(localPosition)));
		}
		public Point PositionToLocal(Point position)
		{
			var family = (Family)PickByUniqueID(familyUID);
			var parent = family == null ? null : (Visual)PickByUniqueID(family.VisualParentUniqueID);
			var parentArea = parent == null ? null : (Area)PickByUniqueID(parent.AreaUniqueID);
			return ErrorIfDestroyed() ? Point.Invalid :
				family == null || parent == null ? position :
				Point.To(parentArea.sprite.InverseTransform.TransformPoint(Point.From(position)));
		}
		public double AngleFromLocal(double localAngle)
		{
			var family = (Family)PickByUniqueID(familyUID);
			var parent = family == null ? null : (Visual)PickByUniqueID(family.VisualParentUniqueID);
			var parentArea = parent == null ? null : (Area)PickByUniqueID(parent.AreaUniqueID);
			return ErrorIfDestroyed() ? double.NaN :
				family == null || parent == null ? localAngle :
				parentArea.localAngle + localAngle;
		}
		public double AngleToLocal(double angle)
		{
			var family = (Family)PickByUniqueID(familyUID);
			var parent = family == null ? null : (Visual)PickByUniqueID(family.VisualParentUniqueID);
			var parentArea = parent == null ? null : (Area)PickByUniqueID(parent.AreaUniqueID);
			return ErrorIfDestroyed() ? double.NaN :
				family == null || parent == null ? angle :
				-(parentArea.localAngle - angle);
		}
		public Size SizeFromLocal(Size localSize)
		{
			var family = (Family)PickByUniqueID(familyUID);
			var parent = family == null ? null : (Visual)PickByUniqueID(family.VisualParentUniqueID);
			var parentArea = parent == null ? null : (Area)PickByUniqueID(parent.AreaUniqueID);
			return ErrorIfDestroyed() ? Size.Invalid :
				family == null || parent == null ? localSize :
				localSize + parentArea.Size;
		}
		public Size SizeToLocal(Size size)
		{
			var family = (Family)PickByUniqueID(familyUID);
			var parent = family == null ? null : (Visual)PickByUniqueID(family.VisualParentUniqueID);
			var parentArea = parent == null ? null : (Area)PickByUniqueID(parent.AreaUniqueID);
			return ErrorIfDestroyed() ? Size.Invalid :
				family == null || parent == null ? size :
				size - parentArea.Size;
		}
	}
}
