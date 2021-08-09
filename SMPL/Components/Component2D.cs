using SFML.Graphics;
using SFML.System;
using static SMPL.Events;

namespace SMPL
{
	public class Component2D : ComponentAccess
	{
		internal Sprite sprite = new();
		internal SFML.Graphics.Text text = new();
		internal ComponentFamily family;

		private readonly uint creationFrame;
		private readonly double rand;

		private ComponentHitbox componentHitbox;
		public ComponentHitbox ComponentHitbox
		{
			get { return componentHitbox; }
			set
			{
				if (componentHitbox == value || (Debug.currentMethodIsCalledByUser && IsCurrentlyAccessible() == false)) return;
				componentHitbox = value;
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

				var delta = value - position;
				position = value;
				UpdateHitbox();

				if (Debug.currentMethodIsCalledByUser == false) { lastFramePos = position; return; }
				var n = D(instances); foreach (var kvp in n) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++) e[i].On2DMoveSetup(this, delta); }
				var n1 = D(instances); foreach (var kvp in n1) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++) e[i].On2DMove(this, delta); }
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

				var delta = value - angle;
				angle = value;
				UpdateHitbox();

				if (Debug.currentMethodIsCalledByUser == false) { lastFrameAng = angle; return; }
				var n = D(instances); foreach (var kvp in n) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++) e[i].On2DRotateSetup(this, delta); }
				var n1 = D(instances); foreach (var kvp in n1) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++) e[i].On2DRotate(this, delta); }
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
				if (Debug.currentMethodIsCalledByUser && IsCurrentlyAccessible() == false) return;
				value.X = Number.Limit(value.X, new Bounds(0, 100));
				value.Y = Number.Limit(value.Y, new Bounds(0, 100));
				if (originPercent == value || Camera.WorldCamera.Component2D == this) return;
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

		private Point localPosition, lastFrameLocalPos;
		public Point LocalPosition
		{
			get { return localPosition; }
			set
			{
				if (Debug.currentMethodIsCalledByUser && IsCurrentlyAccessible() == false) return;
				position = PositionFromLocal(value);
				if (localPosition == value) return;
				var delta = value - localPosition;
				localPosition = value;
				UpdateHitbox();

				if (Debug.currentMethodIsCalledByUser == false) { lastFrameLocalPos = localPosition; return; }
				var n = D(instances); foreach (var kvp in n) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++) e[i].On2DLocalMoveSetup(this, delta); }
				var n1 = D(instances); foreach (var kvp in n1) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++) e[i].On2DLocalMove(this, delta); }
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
				var delta = value - localAngle;
				localAngle = value;
				UpdateHitbox();

				if (Debug.currentMethodIsCalledByUser == false) { lastFrameLocalAng = localAngle; return; }
				var n = D(instances); foreach (var kvp in n) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++) e[i].On2DLocalRotateSetup(this, delta); }
				var n1 = D(instances); foreach (var kvp in n1) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++) e[i].On2DLocalRotate(this, delta); }
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
				var delta = value - localSize;
				localSize = value;
				UpdateHitbox();

				if (Debug.currentMethodIsCalledByUser == false) { lastFrameLocalSz = localSize; return; }
				var n = D(instances); foreach (var kvp in n) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++) e[i].On2DLocalResizeSetup(this, delta); }
				var n1 = D(instances); foreach (var kvp in n1) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++) e[i].On2DLocalResize(this, delta); }
			}
		}

		public Component2D()
		{
			transforms.Add(this);
			creationFrame = Performance.FrameCount;
			rand = Number.Random(new Bounds(-9999, 9999), 5);

			ComponentHitbox = new();
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
            if (creationFrame + 1 != Performance.frameCount)
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
				if (creationFrame + 1 != Performance.frameCount)
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
				if (creationFrame + 1 != Performance.frameCount)
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
				if (creationFrame + 1 != Performance.frameCount)
				{
					var n = D(instances); foreach (var kvp in n) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++)
							e[i].On2DOriginateEndSetup(this); }
					var n1 = D(instances); foreach (var kvp in n1) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++) e[i].On2DOriginateEnd(this); }
				}
			}
			//=============================
			if (Gate.EnterOnceWhile($"{creationFrame}-{rand}-loc-pos-start", lastFrameLocalPos != localPosition))
			{
				var n = D(instances); foreach (var kvp in n) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++) e[i].On2DLocalMoveStartSetup(this, localPosition - lastFrameLocalPos); }
				var n1 = D(instances); foreach (var kvp in n1) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++) e[i].On2DLocalMoveStart(this, localPosition - lastFrameLocalPos); }
			}
			if (Gate.EnterOnceWhile($"{creationFrame}-{rand}-loc-pos-end", lastFrameLocalPos == localPosition))
			{
				if (creationFrame + 1 != Performance.frameCount)
				{
					var n = D(instances); foreach (var kvp in n) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++) e[i].On2DLocalMoveEndSetup(this); }
					var n1 = D(instances); foreach (var kvp in n1) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++) e[i].On2DLocalMoveEnd(this); }
				}
			}
			//==============================
			if (Gate.EnterOnceWhile($"{creationFrame}-{rand}-loc-ang-start", lastFrameLocalAng != localAngle))
			{
				var n = D(instances); foreach (var kvp in n) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++) e[i].On2DLocalRotateStartSetup(this, localAngle - lastFrameLocalAng); }
				var n1 = D(instances); foreach (var kvp in n1) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++) e[i].On2DLocalRotateStart(this, localAngle - lastFrameLocalAng); }
			}
			if (Gate.EnterOnceWhile($"{creationFrame}-{rand}-loc-ang-end", lastFrameLocalAng == localAngle))
			{
				if (creationFrame + 1 != Performance.frameCount)
				{
					var n = D(instances); foreach (var kvp in n) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++) e[i].On2DLocalRotateEndSetup(this); }
					var n1 = D(instances); foreach (var kvp in n1) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++) e[i].On2DLocalRotateEnd(this); }
				}
			}
			//=============================
			if (Gate.EnterOnceWhile($"{creationFrame}-{rand}-loc-sz-start", lastFrameLocalSz != localSize))
			{
				var n = D(instances); foreach (var kvp in n) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++) e[i].On2DLocalResizeStartSetup(this, localSize - lastFrameLocalSz); }
				var n1 = D(instances); foreach (var kvp in n1) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++) e[i].On2DLocalResizeStart(this, localSize - lastFrameLocalSz); }
			}
			if (Gate.EnterOnceWhile($"{creationFrame}-{rand}-loc-sz-end", lastFrameLocalSz == localSize))
			{
				if (creationFrame + 1 != Performance.frameCount)
				{
					var n = D(instances); foreach (var kvp in n) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++) e[i].On2DLocalResizeEndSetup(this); }
					var n1 = D(instances); foreach (var kvp in n1) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++) e[i].On2DLocalResizeEnd(this); }
				}
			}
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
