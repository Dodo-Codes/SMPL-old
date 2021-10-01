using SFML.Graphics;
using SMPL.Components;
using SMPL.Gear;

namespace SMPL.Data
{
	public struct Line
	{
		public static Line Invalid => new(Point.Invalid, Point.Invalid);

		internal Point startPosition;
		public Point StartPosition { get; set; }
		internal Point endPosition;
		public Point EndPosition { get; set; }
		public double Length => Point.Distance(StartPosition, EndPosition);
		public double Angle => Number.AngleBetweenPoints(StartPosition, EndPosition);

		public Line(Point startPosition, Point endPosition)
		{
			this.startPosition = new Point(0, 0);
			this.endPosition = new Point(0, 0);
			StartPosition = startPosition;
			EndPosition = endPosition;
		}
		internal static void GetCrossPointOfTwoLines(Point startA, Point endA, Point startB, Point endB,
			 out bool lines_intersect, out bool segments_intersect,
			 out Point intersection,
			 out Point close_p1, out Point close_p2)
		{
			// Find the point of intersection between
			// the lines p1 --> p2 and p3 --> p4.

			intersection = Point.Invalid;
			// Get the segments' parameters.
			var dx12 = endA.X - startA.X;
			var dy12 = endA.Y - startA.Y;
			var dx34 = endB.X - startB.X;
			var dy34 = endB.Y - startB.Y;

			// Solve for t1 and t2
			var denominator = (dy12 * dx34 - dx12 * dy34);

			var t1 = ((startA.X - startB.X) * dy34 + (startB.Y - startA.Y) * dx34) / denominator;
			if (double.IsInfinity(t1))
			{
				// The lines are parallel (or close enough to it).
				lines_intersect = false;
				segments_intersect = false;
				close_p1 = Point.Invalid;
				close_p2 = Point.Invalid;
				return;
			}
			lines_intersect = true;

			var t2 = ((startB.X - startA.X) * dy12 + (startA.Y - startB.Y) * dx12) / -denominator;

			// Find the point of intersection.
			intersection = new Point(startA.X + dx12 * t1, startA.Y + dy12 * t1);

			// The segments intersect if t1 and t2 are between 0 and 1.
			segments_intersect = ((t1 >= 0) && (t1 <= 1) && (t2 >= 0) && (t2 <= 1));

			// Find the closest points on the segments.
			if (t1 < 0) t1 = 0;
			else if (t1 > 1) t1 = 1;

			if (t2 < 0) t2 = 0;
			else if (t2 > 1) t2 = 1;

			close_p1 = new Point(startA.X + dx12 * t1, startA.Y + dy12 * t1);
			close_p2 = new Point(startB.X + dx34 * t2, startB.Y + dy34 * t2);
		}

		public Point CrossPoint(Line line)
		{
			var segmentsCross = false;
			var linesCross = false;
			var intersection = new Point(0, 0);
			var closestCrossPointToMe = new Point(0, 0);
			var closestCrossPointToLine = new Point(0, 0);

			GetCrossPointOfTwoLines(startPosition, EndPosition, line.StartPosition, line.EndPosition,
				out linesCross, out segmentsCross, out intersection, out closestCrossPointToMe, out closestCrossPointToLine);
			return segmentsCross ? intersection : Point.Invalid;
		}
		public bool ContainsPoint(Point point)
		{
			var AB = Length;
			var AP = Point.Distance(StartPosition, point);
			var PB = Point.Distance(EndPosition, point);
			var sum = AP + PB;
			return Number.IsBetween(sum, new Number.Range(AB - 0.01, AB + 0.01));
		}
		public bool Crosses(Line line) => CrossPoint(line).IsInvalid() == false;

		public void Display(Camera camera, double width)
		{
			if (Window.DrawNotAllowed()) return;

			width /= 2;
			var startLeft = Point.MoveAtAngle(StartPosition, Angle - 90, width, Time.Unit.Tick);
			var startRight = Point.MoveAtAngle(StartPosition, Angle + 90, width, Time.Unit.Tick);
			var endLeft = Point.MoveAtAngle(EndPosition, Angle - 90, width, Time.Unit.Tick);
			var endRight = Point.MoveAtAngle(EndPosition, Angle + 90, width, Time.Unit.Tick);

			var vert = new Vertex[]
			{
				new(Point.From(startLeft), Color.From(StartPosition.Color)),
				new(Point.From(startRight), Color.From(StartPosition.Color)),
				new(Point.From(endRight), Color.From(EndPosition.Color)),
				new(Point.From(endLeft), Color.From(EndPosition.Color)),
			};
			camera.rendTexture.Draw(vert, PrimitiveType.Quads);
			Performance.DrawCallsPerFrame++;
		}
	}
}
