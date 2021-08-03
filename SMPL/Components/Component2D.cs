using SFML.Graphics;
using SFML.System;
using static SMPL.Events;

namespace SMPL
{
	public class Component2D
	{
		internal Sprite sprite = new();
		internal SFML.Graphics.Text text = new();
		internal ComponentFamily family;

		private readonly uint creationFrame;
		private readonly double rand;

		public ComponentHitbox ComponentHitbox { get; set; } = new();

		internal Point position, lastFramePos;
		public Point Position
		{
			get { return PositionFromLocal(LocalPosition); }
			set
			{
				if (Camera.WorldCamera.TransformComponent == this) return;
				localPosition = PositionToLocal(value);
				if (position == value) return;

				var delta = value - position;
				position = value;
				UpdateHitbox();

				if (Debug.currentMethodIsCalledByUser == false) { lastFramePos = position; return; }
				var n = D(instances); foreach (var kvp in n) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++)
						e[i].On2DMoveSetup(this, delta); }
				var n1 = D(instances); foreach (var kvp in n1) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++)
						e[i].On2DMove(this, delta); }
			}
		}
		internal double angle, lastFrameAng;
		public double Angle
		{
			get { return AngleFromLocal(LocalAngle); }
			set
			{
				if (Camera.WorldCamera.TransformComponent == this) return;
				localAngle = AngleToLocal(value);
				if (angle == value) return;

				var delta = value - angle;
				angle = value;
				UpdateHitbox();

				if (Debug.currentMethodIsCalledByUser == false) { lastFrameAng = angle; return; }
				var n = D(instances); foreach (var kvp in n) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++) e[i].On2DRotateSetup(this, delta); }
				var n1 = D(instances); foreach (var kvp in n1) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++) e[i].On2DRotate(this, delta); }
			}
		}
		internal Size size, lastFrameSz;
		public Size Size
		{
			get { return size; }
			set
			{
				if (Camera.WorldCamera.TransformComponent == this) return;
				localSize = SizeToLocal(value);
				if (size == value) return;
				var delta = value - size;
				size = value;
				UpdateHitbox();

				if (Debug.currentMethodIsCalledByUser == false) { lastFrameSz = size; return; }
				var n = D(instances); foreach (var kvp in n) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++)
						e[i].On2DResizeSetup(this, delta); }
				var n1 = D(instances); foreach (var kvp in n1) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++)
						e[i].On2DResize(this, delta); }
			}
		}
		internal Point originPercent, lastFrameOrPer;
		public Point OriginPercent
		{
			get { return originPercent; }
			set
			{
				value.X = Number.Limit(value.X, new Bounds(0, 100));
				value.Y = Number.Limit(value.Y, new Bounds(0, 100));
				if (originPercent == value || Camera.WorldCamera.TransformComponent == this) return;
				var delta = value - originPercent;
				originPercent = value;
				UpdateHitbox();

				if (Debug.currentMethodIsCalledByUser == false) { lastFrameOrPer = originPercent; return; }
				var n = D(instances); foreach (var kvp in n) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++)
						e[i].On2DOriginateSetup(this, delta); }
				var n1 = D(instances); foreach (var kvp in n1) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++)
						e[i].On2DOriginate(this, delta); }
			}
		}

		private Point localPosition;
		public Point LocalPosition
		{
			get { return localPosition; }
			set
			{
				if (Camera.WorldCamera.TransformComponent == this) return;
				position = PositionFromLocal(value);
				if (localPosition == value) return;
				var delta = value - localPosition;
				localPosition = value;
				UpdateHitbox();
			}
		}
		private double localAngle;
		public double LocalAngle
		{
			get { return localAngle; }
			set
			{
				if (Camera.WorldCamera.TransformComponent == this) return;
				angle = AngleFromLocal(value);
				if (localAngle == value) return;
				var delta = value - localAngle;
				localAngle = value;
				UpdateHitbox();
			}
		}
		private Size localSize;
		public Size LocalSize
		{
			get { return localSize; }
			set
			{
				if (Camera.WorldCamera.TransformComponent == this) return;
				size = SizeFromLocal(value);
				if (localSize == value) return;
				var delta = value - localSize;
				localSize = value;
				UpdateHitbox();
			}
		}

		public Component2D()
		{
			transforms.Add(this);
			creationFrame = Time.FrameCount;
			rand = Number.Random(new Bounds(-9999, 9999), 5);

			UpdateHitbox();

			var n = D(instances); foreach (var kvp in n) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++)
					e[i].On2DCreateSetup(this); }
			var n1 = D(instances); foreach (var kvp in n1) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++)
					e[i].On2DCreate(this); }
		}
		internal void Update()
      {
			if (Gate.EnterOnceWhile($"{creationFrame}-{rand}-pos-start", lastFramePos != position))
			{
				var n = D(instances); foreach (var kvp in n)
				{ var e = L(kvp.Value); for (int i = 0; i < e.Count; i++)
						e[i].On2DMoveStartSetup(this, position - lastFramePos); }
				var n1 = D(instances); foreach (var kvp in n1) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++)
						e[i].On2DMoveStart(this, position - lastFramePos); }
			}
         if (Gate.EnterOnceWhile($"{creationFrame}-{rand}-pos-end", lastFramePos == position))
         {
            if (creationFrame + 1 != Time.frameCount)
            {
					var n = D(instances); foreach (var kvp in n) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++)
							e[i].On2DMoveEndSetup(this); }
					var n1 = D(instances); foreach (var kvp in n1) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++) e[i].On2DMoveEnd(this); }
				}
         }
			//==============================
			if (Gate.EnterOnceWhile($"{creationFrame}-{rand}-ang-start", lastFrameAng != angle))
			{
				var n = D(instances); foreach (var kvp in n) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++)
						e[i].On2DRotateStartSetup(this, angle - lastFrameAng); }
				var n1 = D(instances); foreach (var kvp in n1) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++)
						e[i].On2DRotateStart(this, angle - lastFrameAng); }
			}
			if (Gate.EnterOnceWhile($"{creationFrame}-{rand}-ang-end", lastFrameAng == angle))
			{
				if (creationFrame + 1 != Time.frameCount)
				{
					var n = D(instances); foreach (var kvp in n) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++)
							e[i].On2DRotateEndSetup(this); }
					var n1 = D(instances); foreach (var kvp in n1) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++) e[i].On2DRotateEnd(this); }
				}
			}
			//=============================
			if (Gate.EnterOnceWhile($"{creationFrame}-{rand}-sz-start", lastFrameSz != size))
			{
				var n = D(instances); foreach (var kvp in n) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++)
						e[i].On2DResizeStartSetup(this, size - lastFrameSz); }
				var n1 = D(instances); foreach (var kvp in n1) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++)
						e[i].On2DResizeStart(this, size - lastFrameSz); }
			}
			if (Gate.EnterOnceWhile($"{creationFrame}-{rand}-sz-end", lastFrameSz == size))
			{
				if (creationFrame + 1 != Time.frameCount)
				{
					var n = D(instances); foreach (var kvp in n) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++)
							e[i].On2DResizeEndSetup(this); }
					var n1 = D(instances); foreach (var kvp in n1) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++) e[i].On2DResizeEnd(this); }
				}
			}
			//=============================
			if (Gate.EnterOnceWhile($"{creationFrame}-{rand}-orper-start", lastFrameOrPer != originPercent))
			{
				var n = D(instances); foreach (var kvp in n) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++)
						e[i].On2DOriginateStartSetup(this, originPercent - lastFrameOrPer); }
				var n1 = D(instances); foreach (var kvp in n1) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++)
						e[i].On2DOriginateStart(this, originPercent - lastFrameOrPer); }
			}
			if (Gate.EnterOnceWhile($"{creationFrame}-{rand}-orper-end", lastFrameOrPer == originPercent))
			{
				if (creationFrame + 1 != Time.frameCount)
				{
					var n = D(instances); foreach (var kvp in n) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++)
							e[i].On2DOriginateEndSetup(this); }
					var n1 = D(instances); foreach (var kvp in n1) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++) e[i].On2DOriginateEnd(this); }
				}
			}
			//=============================
			lastFramePos = position;
			lastFrameAng = angle;
			lastFrameSz = size;
			lastFrameOrPer = originPercent;
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
