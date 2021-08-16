using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;

namespace SMPL
{
	public class Component2D : ComponentAccess
	{
		internal static List<Component2D> transforms = new();
		internal Sprite sprite = new();
		internal SFML.Graphics.Text text = new();
		internal ComponentFamily family;

		private readonly uint creationFrame;
		private readonly double rand;

		private static event Events.ParamsOne<Component2D> OnCreate, OnPositionChangeEnd, OnSizeChangeEnd,
			OnOriginPercentChangeEnd, OnAngleChangeEnd, OnLocalAngleChangeEnd, OnLocalPositionChangeEnd, OnLocalSizeChangeEnd;
		private static event Events.ParamsTwo<Component2D, Point> OnPositionChange, OnPositionChangeStart,
			OnOriginPercentChange, OnOriginPercentChangeStart, OnLocalPositionChange, OnLocalPositionChangeStart;
		private static event Events.ParamsTwo<Component2D, ComponentHitbox> OnHitboxChange;
		private static event Events.ParamsTwo<Component2D, double> OnAngleChange, OnAngleChangeStart, OnLocalAngleChange,
			OnLocalAngleChangeStart;
		private static event Events.ParamsTwo<Component2D, Size> OnSizeChange, OnSizeChangeStart, OnLocalSizeChange,
			OnLocalSizeChangeStart;

		public static void CallOnCreate(Action<Component2D> method, uint order = uint.MaxValue) =>
			OnCreate = Events.Add(OnCreate, method, order);
		public static void CallOnHitboxChange(Action<Component2D, ComponentHitbox> method, uint order = uint.MaxValue) =>
			OnHitboxChange = Events.Add(OnHitboxChange, method, order);
		public static void CallOnPositionChange(Action<Component2D, Point> method, uint order = uint.MaxValue) =>
			OnPositionChange = Events.Add(OnPositionChange, method, order);
		public static void CallOnPositionChangeStart(Action<Component2D, Point> method, uint order = uint.MaxValue) =>
			OnPositionChangeStart = Events.Add(OnPositionChangeStart, method, order);
		public static void CallOnPositionChangeEnd(Action<Component2D> method, uint order = uint.MaxValue) =>
			OnPositionChangeEnd = Events.Add(OnPositionChangeEnd, method, order);
		public static void CallOnAngleChange(Action<Component2D, double> method, uint order = uint.MaxValue) =>
			OnAngleChange = Events.Add(OnAngleChange, method, order);
		public static void CallOnAngleChangeStart(Action<Component2D, double> method, uint order = uint.MaxValue) =>
			OnAngleChangeStart = Events.Add(OnAngleChangeStart, method, order);
		public static void CallOnAngleChangeEnd(Action<Component2D> method, uint order = uint.MaxValue) =>
			OnAngleChangeEnd = Events.Add(OnAngleChangeEnd, method, order);
		public static void CallOnSizeChange(Action<Component2D, Size> method, uint order = uint.MaxValue) =>
			OnSizeChange = Events.Add(OnSizeChange, method, order);
		public static void CallOnSizeChangeStart(Action<Component2D, Size> method, uint order = uint.MaxValue) =>
			OnSizeChangeStart = Events.Add(OnSizeChangeStart, method, order);
		public static void CallOnSizeChangeEnd(Action<Component2D> method, uint order = uint.MaxValue) =>
			OnSizeChangeEnd = Events.Add(OnSizeChangeEnd, method, order);
		public static void CallOnOriginPercentChange(Action<Component2D, Point> method, uint order = uint.MaxValue) =>
			OnOriginPercentChange = Events.Add(OnOriginPercentChange, method, order);
		public static void CallOnOriginPercentChangeStart(Action<Component2D, Point> method, uint order = uint.MaxValue) =>
			OnOriginPercentChangeStart = Events.Add(OnOriginPercentChangeStart, method, order);
		public static void CallOnOriginPercentChangeEnd(Action<Component2D> method, uint order = uint.MaxValue) =>
			OnOriginPercentChangeEnd = Events.Add(OnOriginPercentChangeEnd, method, order);
		public static void CallOnLocalPositionChange(Action<Component2D, Point> method, uint order = uint.MaxValue) =>
			OnLocalPositionChange = Events.Add(OnLocalPositionChange, method, order);
		public static void CallOnLocalPositionChangeStart(Action<Component2D, Point> method, uint order = uint.MaxValue) =>
			OnLocalPositionChangeStart = Events.Add(OnLocalPositionChangeStart, method, order);
		public static void CallOnLocalPositionChangeEnd(Action<Component2D> method, uint order = uint.MaxValue) =>
			OnLocalPositionChangeEnd = Events.Add(OnLocalPositionChangeEnd, method, order);
		public static void CallOnLocalAngleChange(Action<Component2D, double> method, uint order = uint.MaxValue) =>
			OnLocalAngleChange = Events.Add(OnLocalAngleChange, method, order);
		public static void CallOnLocalAngleChangeStart(Action<Component2D, double> method, uint order = uint.MaxValue) =>
			OnLocalAngleChangeStart = Events.Add(OnLocalAngleChangeStart, method, order);
		public static void CallOnLocalAngleChangeEnd(Action<Component2D> method, uint order = uint.MaxValue) =>
			OnLocalAngleChangeEnd = Events.Add(OnLocalAngleChangeEnd, method, order);
		public static void CallOnLocalSizeChange(Action<Component2D, Size> method, uint order = uint.MaxValue) =>
			OnLocalSizeChange = Events.Add(OnLocalSizeChange, method, order);
		public static void CallOnLocalSizeChangeStart(Action<Component2D, Size> method, uint order = uint.MaxValue) =>
			OnLocalSizeChangeStart = Events.Add(OnLocalSizeChangeStart, method, order);
		public static void CallOnLocalSizeChangeEnd(Action<Component2D> method, uint order = uint.MaxValue) =>
			OnLocalSizeChangeEnd = Events.Add(OnLocalSizeChangeEnd, method, order);

		private ComponentHitbox componentHitbox;
		public ComponentHitbox ComponentHitbox
		{
			get { return componentHitbox; }
			set
			{
				if (componentHitbox == value || (Debug.currentMethodIsCalledByUser && IsCurrentlyAccessible() == false)) return;
				var prev = componentHitbox;
				componentHitbox = value;
				OnHitboxChange?.Invoke(this, prev);
			}
		}

		internal Point position, lastFramePos;
		public Point Position
		{
			get { return PositionFromLocal(LocalPosition); }
			set
			{
				if (Debug.currentMethodIsCalledByUser && IsCurrentlyAccessible() == false) return;
				localPosition = PositionToLocal(value);
				if (position == value) return;

				var prev = position;
				position = value;
				UpdateHitbox();

				if (Debug.currentMethodIsCalledByUser == false) { lastFramePos = position; return; }
				OnPositionChange?.Invoke(this, prev);
			}
		}
		internal double angle, lastFrameAng;
		public double Angle
		{
			get { return AngleFromLocal(LocalAngle); }
			set
			{
				if (Debug.currentMethodIsCalledByUser && IsCurrentlyAccessible() == false) return;
				localAngle = AngleToLocal(value);
				if (angle == value) return;

				var prev = angle;
				angle = value;
				UpdateHitbox();

				if (Debug.currentMethodIsCalledByUser == false) { lastFrameAng = angle; return; }
				OnAngleChange?.Invoke(this, prev);
			}
		}
		internal Size size = new(100, 100), lastFrameSz = new(100, 100);
		public Size Size
		{
			get { return size; }
			set
			{
				if (Debug.currentMethodIsCalledByUser && IsCurrentlyAccessible() == false) return;
				localSize = SizeToLocal(value);
				if (size == value) return;
				var prev = size;
				size = value;
				UpdateHitbox();

				if (Debug.currentMethodIsCalledByUser == false) { lastFrameSz = size; return; }
				OnSizeChange?.Invoke(this, prev);
			}
		}
		internal Point originPercent, lastFrameOrPer;
		public Point OriginPercent
		{
			get { return originPercent; }
			set
			{
				if (Debug.currentMethodIsCalledByUser && IsCurrentlyAccessible() == false) return;
				value.X = Number.Limit(value.X, new Bounds(0, 100));
				value.Y = Number.Limit(value.Y, new Bounds(0, 100));
				if (originPercent == value || Camera.WorldCamera.Component2D == this) return;
				var prev = originPercent;
				originPercent = value;
				UpdateHitbox();

				if (Debug.currentMethodIsCalledByUser == false) { lastFrameOrPer = originPercent; return; }
				OnOriginPercentChange?.Invoke(this, prev);
			}
		}

		private Point localPosition, lastFrameLocalPos;
		public Point LocalPosition
		{
			get { return localPosition; }
			set
			{
				if (Debug.currentMethodIsCalledByUser && IsCurrentlyAccessible() == false) return;
				position = PositionFromLocal(value);
				if (localPosition == value) return;
				var prev = localPosition;
				localPosition = value;
				UpdateHitbox();

				if (Debug.currentMethodIsCalledByUser == false) { lastFrameLocalPos = localPosition; return; }
				OnLocalPositionChange?.Invoke(this, prev);
			}
		}
		private double localAngle, lastFrameLocalAng;
		public double LocalAngle
		{
			get { return localAngle; }
			set
			{
				if (Debug.currentMethodIsCalledByUser && IsCurrentlyAccessible() == false) return;
				angle = AngleFromLocal(value);
				if (localAngle == value) return;
				var prev = localAngle;
				localAngle = value;
				UpdateHitbox();

				if (Debug.currentMethodIsCalledByUser == false) { lastFrameLocalAng = localAngle; return; }
				OnLocalAngleChange?.Invoke(this, prev);
			}
		}
		private Size localSize, lastFrameLocalSz;
		public Size LocalSize
		{
			get { return localSize; }
			set
			{
				if (Debug.currentMethodIsCalledByUser && IsCurrentlyAccessible() == false) return;
				size = SizeFromLocal(value);
				if (localSize == value) return;
				var prev = localSize;
				localSize = value;
				UpdateHitbox();

				if (Debug.currentMethodIsCalledByUser == false) { lastFrameLocalSz = localSize; return; }
				OnLocalSizeChange?.Invoke(this, prev);
			}
		}

		public Component2D()
		{
			transforms.Add(this);
			creationFrame = Performance.FrameCount;
			rand = Number.Random(new Bounds(-9999, 9999), 5);

			ComponentHitbox = new();
			UpdateHitbox();

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
		internal void UpdateHitbox()
		{
			sprite.Position = Point.From(Position);
			sprite.Rotation = (float)Angle;
			foreach (var kvp in ComponentHitbox.lines)
			{
				var localLine = ComponentHitbox.localLines[kvp.Key];
				var sp = Point.To(sprite.Transform.TransformPoint(Point.From(localLine.StartPosition)));
				var ep = Point.To(sprite.Transform.TransformPoint(Point.From(localLine.EndPosition)));
				ComponentHitbox.SetLine(kvp.Key, new Line(sp, ep));
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
			return Number.MoveTowardAngle(angle, camera.Angle, (camera.Angle - angle) * parallaxPercent, Time.Unit.Tick);
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
			return family == null || family.Parent == null || family.owner is ComponentText ? localSize :
				localSize + family.Parent.transform.Size;
		}
		public Size SizeToLocal(Size size)
		{
			return family == null || family.Parent == null || family.owner is ComponentText ? size :
				size - family.Parent.transform.Size;
		}
	}
}
