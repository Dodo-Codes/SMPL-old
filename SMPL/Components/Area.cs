using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using SMPL.Data;
using SMPL.Gear;

namespace SMPL.Components
{
	public class Area : Access
	{
		internal static List<Area> transforms = new();
		internal SFML.Graphics.Sprite sprite = new();
		internal SFML.Graphics.Text text = new();
		internal Family family;
		private readonly List<Hitbox> hitboxes = new();

		private readonly uint creationFrame;
		private readonly double rand;

		private static event Events.ParamsTwo<Area, Hitbox> OnHitboxAdd, OnHitboxRemove;
		private static event Events.ParamsTwo<Area, Identity<Area>> OnIdentityChange;
		private static event Events.ParamsOne<Area> OnHitboxesRemoveAll;
		private static event Events.ParamsOne<Area> OnCreate, OnPositionChangeEnd, OnSizeChangeEnd,
			OnOriginPercentChangeEnd, OnAngleChangeEnd, OnLocalAngleChangeEnd, OnLocalPositionChangeEnd, OnLocalSizeChangeEnd;
		private static event Events.ParamsTwo<Area, Point> OnPositionChange, OnPositionChangeStart,
			OnOriginPercentChange, OnOriginPercentChangeStart, OnLocalPositionChange, OnLocalPositionChangeStart;
		private static event Events.ParamsTwo<Area, double> OnAngleChange, OnAngleChangeStart, OnLocalAngleChange,
			OnLocalAngleChangeStart;
		private static event Events.ParamsTwo<Area, Size> OnSizeChange, OnSizeChangeStart, OnLocalSizeChange,
			OnLocalSizeChangeStart;

		public static class CallWhen
		{
			public static void Create(Action<Area> method, uint order = uint.MaxValue) =>
				OnCreate = Events.Add(OnCreate, method, order);
			public static void IdentityChange(Action<Area, Identity<Area>> method,
				uint order = uint.MaxValue) => OnIdentityChange = Events.Add(OnIdentityChange, method, order);
			public static void PositionChange(Action<Area, Point> method, uint order = uint.MaxValue) =>
				OnPositionChange = Events.Add(OnPositionChange, method, order);
			public static void PositionChangeStart(Action<Area, Point> method, uint order = uint.MaxValue) =>
				OnPositionChangeStart = Events.Add(OnPositionChangeStart, method, order);
			public static void PositionChangeEnd(Action<Area> method, uint order = uint.MaxValue) =>
				OnPositionChangeEnd = Events.Add(OnPositionChangeEnd, method, order);
			public static void AngleChange(Action<Area, double> method, uint order = uint.MaxValue) =>
				OnAngleChange = Events.Add(OnAngleChange, method, order);
			public static void AngleChangeStart(Action<Area, double> method, uint order = uint.MaxValue) =>
				OnAngleChangeStart = Events.Add(OnAngleChangeStart, method, order);
			public static void AngleChangeEnd(Action<Area> method, uint order = uint.MaxValue) =>
				OnAngleChangeEnd = Events.Add(OnAngleChangeEnd, method, order);
			public static void SizeChange(Action<Area, Size> method, uint order = uint.MaxValue) =>
				OnSizeChange = Events.Add(OnSizeChange, method, order);
			public static void SizeChangeStart(Action<Area, Size> method, uint order = uint.MaxValue) =>
				OnSizeChangeStart = Events.Add(OnSizeChangeStart, method, order);
			public static void SizeChangeEnd(Action<Area> method, uint order = uint.MaxValue) =>
				OnSizeChangeEnd = Events.Add(OnSizeChangeEnd, method, order);
			public static void OriginPercentChange(Action<Area, Point> method, uint order = uint.MaxValue) =>
				OnOriginPercentChange = Events.Add(OnOriginPercentChange, method, order);
			public static void OriginPercentChangeStart(Action<Area, Point> method, uint order = uint.MaxValue) =>
				OnOriginPercentChangeStart = Events.Add(OnOriginPercentChangeStart, method, order);
			public static void OriginPercentChangeEnd(Action<Area> method, uint order = uint.MaxValue) =>
				OnOriginPercentChangeEnd = Events.Add(OnOriginPercentChangeEnd, method, order);
			public static void LocalPositionChange(Action<Area, Point> method, uint order = uint.MaxValue) =>
				OnLocalPositionChange = Events.Add(OnLocalPositionChange, method, order);
			public static void LocalPositionChangeStart(Action<Area, Point> method, uint order = uint.MaxValue) =>
				OnLocalPositionChangeStart = Events.Add(OnLocalPositionChangeStart, method, order);
			public static void LocalPositionChangeEnd(Action<Area> method, uint order = uint.MaxValue) =>
				OnLocalPositionChangeEnd = Events.Add(OnLocalPositionChangeEnd, method, order);
			public static void LocalAngleChange(Action<Area, double> method, uint order = uint.MaxValue) =>
				OnLocalAngleChange = Events.Add(OnLocalAngleChange, method, order);
			public static void LocalAngleChangeStart(Action<Area, double> method, uint order = uint.MaxValue) =>
				OnLocalAngleChangeStart = Events.Add(OnLocalAngleChangeStart, method, order);
			public static void LocalAngleChangeEnd(Action<Area> method, uint order = uint.MaxValue) =>
				OnLocalAngleChangeEnd = Events.Add(OnLocalAngleChangeEnd, method, order);
			public static void LocalSizeChange(Action<Area, Size> method, uint order = uint.MaxValue) =>
				OnLocalSizeChange = Events.Add(OnLocalSizeChange, method, order);
			public static void LocalSizeChangeStart(Action<Area, Size> method, uint order = uint.MaxValue) =>
				OnLocalSizeChangeStart = Events.Add(OnLocalSizeChangeStart, method, order);
			public static void LocalSizeChangeEnd(Action<Area> method, uint order = uint.MaxValue) =>
				OnLocalSizeChangeEnd = Events.Add(OnLocalSizeChangeEnd, method, order);
			public static void AddHitbox(Action<Area, Hitbox> method, uint order = uint.MaxValue) =>
				OnHitboxAdd = Events.Add(OnHitboxAdd, method, order);
			public static void RemoveHitbox(Action<Area, Hitbox> method, uint order = uint.MaxValue) =>
				OnHitboxRemove = Events.Add(OnHitboxRemove, method, order);
			public static void RemoveAllHitboxes(Action<Area> method, uint order = uint.MaxValue) =>
				OnHitboxesRemoveAll = Events.Add(OnHitboxesRemoveAll, method, order);
		}

		public void AddHitboxes(params Hitbox[] hitboxInstances)
		{
			if (Debug.CalledBySMPL == false && IsCurrentlyAccessible() == false) return;
			if (hitboxInstances == null)
			{
				Debug.LogError(1, "The collection of ComponentHitbox instances cannot be 'null'.");
				return;
			}
			for (int i = 0; i < hitboxInstances.Length; i++)
			{
				if (hitboxes.Contains(hitboxInstances[i])) continue;
				hitboxes.Add(hitboxInstances[i]);
				if (Debug.CalledBySMPL == false) OnHitboxAdd?.Invoke(this, hitboxInstances[i]);
			}
		}
		public void RemoveHitboxes(params Hitbox[] hitboxInstances)
		{
			if (Debug.CalledBySMPL == false && IsCurrentlyAccessible() == false) return;
			if (hitboxInstances == null)
			{
				Debug.LogError(1, "The collection of ComponentHitbox instances cannot be 'null'.");
				return;
			}
			for (int i = 0; i < hitboxInstances.Length; i++)
			{
				if (hitboxes.Contains(hitboxInstances[i]) == false) continue;
				hitboxes.Remove(hitboxInstances[i]);
				if (Debug.CalledBySMPL == false) OnHitboxRemove?.Invoke(this, hitboxInstances[i]);
			}
		}
		public void RemoveAllHitboxes()
		{
			if (Debug.CalledBySMPL == false && IsCurrentlyAccessible() == false) return;
			if (Debug.CalledBySMPL == false) for (int i = 0; i < hitboxes.Count; i++) OnHitboxRemove?.Invoke(this, hitboxes[i]);
			hitboxes.Clear();
			if (Debug.CalledBySMPL == false) OnHitboxesRemoveAll?.Invoke(this);
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

		private Identity<Area> identity;
		public Identity<Area> Identity
		{
			get { return identity; }
			set
			{
				if (identity == value || (Debug.CalledBySMPL == false && IsCurrentlyAccessible() == false)) return;
				var prev = identity;
				identity = value;
				if (Debug.CalledBySMPL == false) OnIdentityChange?.Invoke(this, prev);
			}
		}

		internal Point position, lastFramePos;
		public Point Position
		{
			get { return PositionFromLocal(LocalPosition); }
			set
			{
				if (Debug.CalledBySMPL == false && IsCurrentlyAccessible() == false) return;
				localPosition = PositionToLocal(value);
				if (position == value) return;

				var prev = position;
				position = value;
				UpdateHitboxes();

				if (Debug.CalledBySMPL) { lastFramePos = position; return; }
				OnPositionChange?.Invoke(this, prev);
			}
		}
		internal double angle, lastFrameAng;
		public double Angle
		{
			get { return AngleFromLocal(LocalAngle); }
			set
			{
				if (Debug.CalledBySMPL == false && IsCurrentlyAccessible() == false) return;
				localAngle = AngleToLocal(value);
				if (angle == value) return;

				var prev = angle;
				angle = value;
				UpdateHitboxes();

				if (Debug.CalledBySMPL) { lastFrameAng = angle; return; }
				OnAngleChange?.Invoke(this, prev);
			}
		}
		internal Size size = new(100, 100), lastFrameSz = new(100, 100);
		public Size Size
		{
			get { return size; }
			set
			{
				if (Debug.CalledBySMPL == false && IsCurrentlyAccessible() == false) return;
				localSize = SizeToLocal(value);
				if (size == value) return;
				var prev = size;
				size = value;
				UpdateHitboxes();

				if (Debug.CalledBySMPL) { lastFrameSz = size; return; }
				OnSizeChange?.Invoke(this, prev);
			}
		}
		internal Point originPercent = new() { X = 50, Y = 50 }, lastFrameOrPer = new() { X = 50, Y = 50 };
		public Point OriginPercent
		{
			get { return originPercent; }
			set
			{
				if (Debug.CalledBySMPL == false && IsCurrentlyAccessible() == false) return;
				value.X = Number.Limit(value.X, new Bounds(0, 100));
				value.Y = Number.Limit(value.Y, new Bounds(0, 100));
				if (originPercent == value || Camera.WorldCamera.Display2D == this) return;
				var prev = originPercent;
				originPercent = value;
				UpdateHitboxes();

				if (Debug.CalledBySMPL) { lastFrameOrPer = originPercent; return; }
				OnOriginPercentChange?.Invoke(this, prev);
			}
		}

		private Point localPosition, lastFrameLocalPos;
		public Point LocalPosition
		{
			get { return localPosition; }
			set
			{
				if (Debug.CalledBySMPL == false && IsCurrentlyAccessible() == false) return;
				position = PositionFromLocal(value);
				if (localPosition == value) return;
				var prev = localPosition;
				localPosition = value;
				UpdateHitboxes();

				if (Debug.CalledBySMPL) { lastFrameLocalPos = localPosition; return; }
				OnLocalPositionChange?.Invoke(this, prev);
			}
		}
		private double localAngle, lastFrameLocalAng;
		public double LocalAngle
		{
			get { return localAngle; }
			set
			{
				if (Debug.CalledBySMPL == false && IsCurrentlyAccessible() == false) return;
				angle = AngleFromLocal(value);
				if (localAngle == value) return;
				var prev = localAngle;
				localAngle = value;
				UpdateHitboxes();

				if (Debug.CalledBySMPL) { lastFrameLocalAng = localAngle; return; }
				OnLocalAngleChange?.Invoke(this, prev);
			}
		}
		private Size localSize, lastFrameLocalSz;
		public Size LocalSize
		{
			get { return localSize; }
			set
			{
				if (Debug.CalledBySMPL == false && IsCurrentlyAccessible() == false) return;
				size = SizeFromLocal(value);
				if (localSize == value) return;
				var prev = localSize;
				localSize = value;
				UpdateHitboxes();

				if (Debug.CalledBySMPL) { lastFrameLocalSz = localSize; return; }
				OnLocalSizeChange?.Invoke(this, prev);
			}
		}

		public Area() : base()
		{
			transforms.Add(this);
			creationFrame = Performance.FrameCount;
			rand = Number.Random(new Bounds(-9999, 9999), 5);

			UpdateHitboxes();

			OnCreate?.Invoke(this);
		}
		internal void Update()
      {
			var c = creationFrame + 1 != Performance.frameCount;

			if (Gate.EnterOnceWhile($"{creationFrame}-{rand}-pos-start", lastFramePos != position))
				OnPositionChangeStart?.Invoke(this, lastFramePos);
         if (Gate.EnterOnceWhile($"{creationFrame}-{rand}-pos-end", lastFramePos == position) && c)
				OnPositionChangeEnd?.Invoke(this);
			//==============================
			if (Gate.EnterOnceWhile($"{creationFrame}-{rand}-ang-start", lastFrameAng != angle))
				OnAngleChangeStart?.Invoke(this, lastFrameAng);
			if (Gate.EnterOnceWhile($"{creationFrame}-{rand}-ang-end", lastFrameAng == angle) && c)
				OnAngleChangeEnd?.Invoke(this);
			//=============================
			if (Gate.EnterOnceWhile($"{creationFrame}-{rand}-sz-start", lastFrameSz != size))
				OnSizeChangeStart?.Invoke(this, lastFrameSz);
			if (Gate.EnterOnceWhile($"{creationFrame}-{rand}-sz-end", lastFrameSz == size) && c)
				OnSizeChangeEnd?.Invoke(this);
			//=============================
			if (Gate.EnterOnceWhile($"{creationFrame}-{rand}-orper-start", lastFrameOrPer != originPercent))
				OnOriginPercentChangeStart?.Invoke(this, lastFrameOrPer);
			if (Gate.EnterOnceWhile($"{creationFrame}-{rand}-orper-end", lastFrameOrPer == originPercent) && c)
				OnOriginPercentChangeEnd?.Invoke(this);
			//=============================
			if (Gate.EnterOnceWhile($"{creationFrame}-{rand}-loc-pos-start", lastFrameLocalPos != localPosition))
				OnLocalPositionChangeStart?.Invoke(this, lastFrameLocalPos);
			if (Gate.EnterOnceWhile($"{creationFrame}-{rand}-loc-pos-end", lastFrameLocalPos == localPosition) && c)
				OnLocalPositionChangeEnd?.Invoke(this);
			//==============================
			if (Gate.EnterOnceWhile($"{creationFrame}-{rand}-loc-ang-start", lastFrameLocalAng != localAngle))
				OnLocalAngleChangeStart?.Invoke(this, lastFrameLocalAng);
			if (Gate.EnterOnceWhile($"{creationFrame}-{rand}-loc-ang-end", lastFrameLocalAng == localAngle) && c)
				OnLocalAngleChangeEnd?.Invoke(this);
			//=============================
			if (Gate.EnterOnceWhile($"{creationFrame}-{rand}-loc-sz-start", lastFrameLocalSz != localSize))
				OnLocalSizeChangeStart?.Invoke(this, lastFrameLocalSz);
			if (Gate.EnterOnceWhile($"{creationFrame}-{rand}-loc-sz-end", lastFrameLocalSz == localSize) && c)
				OnLocalSizeChangeEnd?.Invoke(this);
			//=============================
			lastFramePos = position;
			lastFrameAng = angle;
			lastFrameSz = size;
			lastFrameOrPer = originPercent;
			lastFrameLocalAng = localAngle;
			lastFrameLocalPos = localPosition;
			lastFrameLocalSz = localSize;
		}
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

		public static Point PositionToParallax(Point position, Size parallaxPercent, Camera camera)
		{
			parallaxPercent += new Size(100, 100);
			var x = Number.FromPercent(parallaxPercent.W, new Bounds(-camera.Position.X, position.X));
			var y = Number.FromPercent(parallaxPercent.H, new Bounds(-camera.Position.Y, position.Y));
			return new Point(x, y);
		}
		public Point PositionFromLocal(Point localPosition)
		{
			return family == null || family.Parent == null ? localPosition :
				Point.To(family.Parent.transform.sprite.Transform.TransformPoint(Point.From(localPosition)));
		}
		public Point PositionToLocal(Point position)
		{
			return family == null || family.Parent == null ? position :
				Point.To(family.Parent.transform.sprite.InverseTransform.TransformPoint(Point.From(position)));
		}
		public static double AngleToParallax(double angle, double parallaxPercent, Camera camera)
		{
			parallaxPercent /= 100;
			return Number.MoveTowardAngle(angle, camera.Angle, (camera.Angle - angle) * parallaxPercent, Gear.Time.Unit.Tick);
		}
		public double AngleFromLocal(double localAngle)
		{
			return family == null || family.Parent == null ? localAngle :
				family.Parent.transform.localAngle + localAngle;
		}
		public double AngleToLocal(double angle)
		{
			return family == null || family.Parent == null ? angle :
				-(family.Parent.transform.localAngle - angle);
		}
		public static Size SizeToParallax(Size size, Size parallaxPercent, Camera camera)
		{
			parallaxPercent /= 100;
			var sc = (camera.Size / camera.startSize) * parallaxPercent;
			return size * sc;
		}
		public Size SizeFromLocal(Size localSize)
		{
			return family == null || family.Parent == null || family.owner is Text ? localSize :
				localSize + family.Parent.transform.Size;
		}
		public Size SizeToLocal(Size size)
		{
			return family == null || family.Parent == null || family.owner is Text ? size :
				size - family.Parent.transform.Size;
		}
	}
}
