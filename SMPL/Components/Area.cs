using System.Collections.Generic;
using SMPL.Data;
using SMPL.Gear;

namespace SMPL.Components
{
	public class Area : Component
	{
		private readonly List<Hitbox> hitboxes = new();
		private Point localPosition, originPercent;
		private double localAngle;
		private Size localSize;

		//===============

		internal static List<Area> transforms = new();
		internal SFML.Graphics.Sprite sprite = new();
		internal SFML.Graphics.Text text = new();
		internal Family family;

		internal void UpdateHitboxes()
		{
			sprite.Position = Point.From(Position);
			sprite.Rotation = (float)Angle;
			for (int i = 0; i < hitboxes.Count; i++)
			{
				var lines = hitboxes[i].lines;
				foreach (var kvp in lines)
				{
					var localLine = hitboxes[i].localLines[kvp.Key];
					var sp = Point.To(sprite.Transform.TransformPoint(Point.From(localLine.StartPosition)));
					var ep = Point.To(sprite.Transform.TransformPoint(Point.From(localLine.EndPosition)));
					hitboxes[i].SetLine(kvp.Key, new Line(sp, ep));
				}
			}
		}

		//==============

		public Point Position
		{
			get { return ErrorIfDestroyed() ? Point.Invalid : PositionFromLocal(LocalPosition); }
			set
			{
				if (ErrorIfDestroyed()) return;
				localPosition = PositionToLocal(value);
				UpdateHitboxes();
			}
		}
		public double Angle
		{
			get { return ErrorIfDestroyed() ? double.NaN : AngleFromLocal(LocalAngle); }
			set
			{
				if (ErrorIfDestroyed()) return;
				localAngle = AngleToLocal(value);
				UpdateHitboxes();
			}
		}
		public Size Size
		{
			get { return ErrorIfDestroyed() ? Size.Invalid : SizeFromLocal(LocalSize); }
			set
			{
				if (ErrorIfDestroyed()) return;
				localSize = SizeToLocal(value);
				UpdateHitboxes();
			}
		}
		public Point OriginPercent
		{
			get { return ErrorIfDestroyed() ? Point.Invalid : originPercent; }
			set
			{
				if (ErrorIfDestroyed()) return;
				originPercent = new Point(Number.Limit(value.X, new Bounds(0, 100)), Number.Limit(value.Y, new Bounds(0, 100)));
				UpdateHitboxes();
			}
		}

		public Point LocalPosition
		{
			get { return ErrorIfDestroyed() ? Point.Invalid : localPosition; }
			set
			{
				if (ErrorIfDestroyed()) return;
				localPosition = value;
				UpdateHitboxes();
			}
		}
		public double LocalAngle
		{
			get { return ErrorIfDestroyed() ? double.NaN : localAngle; }
			set
			{
				if (ErrorIfDestroyed()) return;
				localAngle = value;
				UpdateHitboxes();
			}
		}
		public Size LocalSize
		{
			get { return ErrorIfDestroyed() ? Size.Invalid : localSize; }
			set
			{
				if (ErrorIfDestroyed()) return;
				localSize = value;
				UpdateHitboxes();
			}
		}

		public Area(string uniqueID) : base(uniqueID)
		{
			transforms.Add(this);

			Size = new Size(100, 100);
			OriginPercent = new Point(50, 50);

			UpdateHitboxes();
			if (cannotCreate) { ErrorAlreadyHasUID(uniqueID); Destroy(); }
		}
		public override void Destroy()
		{
			if (ErrorIfDestroyed()) return;
			transforms.Remove(this);
			hitboxes.Clear();
			family = null;
			sprite.Dispose();
			text.Dispose();
			base.Destroy();
		}

		public static Point PositionToParallax(Point position, Size parallaxPercent, Camera camera)
		{
			parallaxPercent += new Size(100, 100);
			var x = Number.FromPercent(parallaxPercent.W, new Bounds(-camera.Position.X, position.X));
			var y = Number.FromPercent(parallaxPercent.H, new Bounds(-camera.Position.Y, position.Y));
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

		public void AddHitboxes(params Hitbox[] hitboxInstances)
		{
			if (ErrorIfDestroyed()) return;
			if (hitboxInstances == null)
			{
				Debug.LogError(1, "The collection of ComponentHitbox instances cannot be 'null'.");
				return;
			}
			for (int i = 0; i < hitboxInstances.Length; i++)
				if (hitboxes.Contains(hitboxInstances[i]) == false) 
					hitboxes.Add(hitboxInstances[i]);
		}
		public void RemoveHitboxes(params Hitbox[] hitboxInstances)
		{
			if (ErrorIfDestroyed()) return;
			if (hitboxInstances == null)
			{
				Debug.LogError(1, "The collection of ComponentHitbox instances cannot be 'null'.");
				return;
			}
			for (int i = 0; i < hitboxInstances.Length; i++)
				if (hitboxes.Contains(hitboxInstances[i]))
					hitboxes.Remove(hitboxInstances[i]);
		}
		public void RemoveAllHitboxes()
		{
			if (ErrorIfDestroyed()) return;
			hitboxes.Clear();
		}
		public bool HasHitboxes(params Hitbox[] hitboxInstances)
		{
			if (hitboxInstances == null)
			{
				Debug.LogError(1, "The collection of ComponentHitbox instances cannot be 'null'.");
				return false;
			}
			for (int i = 0; i < hitboxes.Count; i++)
				if (hitboxes.Contains(hitboxInstances[i]) == false)
					return false;
			return true;
		}

		public Point PositionFromLocal(Point localPosition)
		{
			return ErrorIfDestroyed() ? Point.Invalid :
				family == null || family.Parent == null ? localPosition :
            Point.To(family.Parent.Area.sprite.Transform.TransformPoint(Point.From(localPosition)));
		}
		public Point PositionToLocal(Point position)
		{
			return ErrorIfDestroyed() ? Point.Invalid : 
				family == null || family.Parent == null ? position :
				Point.To(family.Parent.Area.sprite.InverseTransform.TransformPoint(Point.From(position)));
		}
		public double AngleFromLocal(double localAngle)
		{
			return ErrorIfDestroyed() ? double.NaN : 
				family == null || family.Parent == null ? localAngle :
				family.Parent.Area.localAngle + localAngle;
		}
		public double AngleToLocal(double angle)
		{
			return ErrorIfDestroyed() ? double.NaN : 
				family == null || family.Parent == null ? angle :
				-(family.Parent.Area.localAngle - angle);
		}
		public Size SizeFromLocal(Size localSize)
		{
			return ErrorIfDestroyed() ? Size.Invalid :
				family == null || family.Parent == null ? localSize :
				localSize + family.Parent.Area.Size;
		}
		public Size SizeToLocal(Size size)
		{
			return ErrorIfDestroyed() ? Size.Invalid :
				family == null || family.Parent == null ? size :
				size - family.Parent.Area.Size;
		}
	}
}
