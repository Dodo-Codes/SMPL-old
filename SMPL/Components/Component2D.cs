using SFML.Graphics;
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

		internal Point position, lastFramePos;
		public Point Position
		{
			get { return position; }
			set
			{
				if (Camera.WorldCamera.TransformComponent == this) return;
				//localPosition = PositionToLocal(value);
				if (position == value) return;

				var delta = value - position;
				position = value;

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
			get { return angle; }
			set
			{
				if (Camera.WorldCamera.TransformComponent == this) return;
				//localAngle = AngleToLocal(value);
				if (angle == value) return;

				var delta = value - angle;
				angle = value;

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
				if (size == value || Camera.WorldCamera.TransformComponent == this) return;
				var delta = value - size;
				size = value;

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

				if (Debug.currentMethodIsCalledByUser == false) { lastFrameOrPer = originPercent; return; }
				var n = D(instances); foreach (var kvp in n) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++)
						e[i].On2DOriginateSetup(this, delta); }
				var n1 = D(instances); foreach (var kvp in n1) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++)
						e[i].On2DOriginate(this, delta); }
			}
		}

		public Point LocalPosition { get; set; }
		public double LocalAngle { get; set; }
		public Size LocalSize { get; set; }

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

		public Point PositionToLocal(Point position)
		{
			if (family != null && family.Parent != null)
			{
				var pos = family.Parent.transform.sprite.InverseTransform.TransformPoint(Point.From(position));
				return Point.To(pos);
			}
			else return position;
		}
		public Point PositionFromLocal(Point position)
		{
			if (family != null && family.Parent != null)
			{
				var pos = family.Parent.transform.sprite.Transform.TransformPoint(Point.From(position));
				return Point.To(pos);
			}
			else return position;
		}
		public double AngleToLocal(double angle)
		{
			return family != null && family.Parent != null ? -(family.Parent.transform.LocalAngle - angle) : angle;
		}
		public double AngleFromLocal(double angle)
		{
			return family != null && family.Parent != null ? family.Parent.transform.LocalAngle + angle : angle;
		}

		public Component2D()
		{
			transforms.Add(this);
			creationFrame = Time.FrameCount;
			rand = Number.Random(new Bounds(-9999, 9999), 5);

			var n = D(instances); foreach (var kvp in n) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++)
					e[i].On2DCreateSetup(this); }
			var n1 = D(instances); foreach (var kvp in n1) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++)
					e[i].On2DCreate(this); }
		}
	}
}
