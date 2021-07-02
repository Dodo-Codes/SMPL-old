using System;

namespace SMPL
{
	public struct Line
	{
		private Point startPosition;
		public Point StartPosition
		{
			get { return startPosition; }
			set
			{
				startPosition = value;
				UpdateLength();
			}
		}
		private Point endPosition;
		public Point EndPosition
		{
			get { return endPosition; }
			set
			{
				endPosition = value;
				UpdateLength();
			}
		}
		public double Length { get; private set; }
		public Color StartColor { get; set; }
		public Color EndColor { get; set; }

		public Line(Point startPosition, Point endPosition)
		{
			StartColor = Color.White;
			EndColor = Color.White;
			Length = 0;
			this.startPosition = new Point(0, 0);
			this.endPosition = new Point(0, 0);
			StartPosition = startPosition;
			EndPosition = endPosition;
			UpdateLength();
		}
		private void UpdateLength() => Length = Point.GetDistance(StartPosition, EndPosition);

		public static Point GetCrossPoint(Line lineA, Line lineB)
		{
			var segmentsCross = false;
			var linesCross = false;
			var intersection = new Point(0, 0);
			var closestCrossPointToMe = new Point(0, 0);
			var closestCrossPointToLine = new Point(0, 0);

			GetCrossPointOfTwoLines(lineA.startPosition, lineA.EndPosition, lineB.StartPosition, lineB.EndPosition,
				out linesCross, out segmentsCross, out intersection, out closestCrossPointToMe, out closestCrossPointToLine);
			return segmentsCross ? intersection : new Point(double.NaN, double.NaN);
		}
		public static bool ContainsPoint(Line line, Point point)
		{
			var AB = line.Length;
			var AP = Point.GetDistance(line.StartPosition, point);
			var PB = Point.GetDistance(line.EndPosition, point);
			var sum = AP + PB;
			return Number.IsBetween(sum, new Bounds(AB - 0.01, AB + 0.01));
		}
		public static bool AreCrossing(Line lineA, Line lineB)
		{
			return GetCrossPoint(lineA, lineB) != new Point(double.NaN, double.NaN);
		}

		internal static void GetCrossPointOfTwoLines(Point startA, Point endA, Point startB, Point endB,
			 out bool lines_intersect, out bool segments_intersect,
			 out Point intersection,
			 out Point close_p1, out Point close_p2)
		{
			// Find the point of intersection between
			// the lines p1 --> p2 and p3 --> p4.

			intersection = new Point(double.NaN, double.NaN);
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
				close_p1 = new Point(float.NaN, float.NaN);
				close_p2 = new Point(float.NaN, float.NaN);
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
		//public static bool LineCrossesLine(Line lineA, Line lineB)
		//{
		//	return ccw(lineA.startPosition, lineB.startPosition, lineB.EndPosition) !=
		//		ccw(lineA.endPosition, lineB.StartPosition, lineB.EndPosition) &&
		//		ccw(lineA.StartPosition, lineA.EndPosition, lineB.StartPosition) !=
		//		ccw(lineA.StartPosition, lineA.EndPosition, lineB.EndPosition);
		//
		//	static bool ccw(Point a, Point b, Point c) => (c.Y - a.Y * (b.X - a.X) > (b.Y - a.Y) * (c.Y - a.Y));
		//}
		//public Point[] GetCrossPointsWithCircle(Circle circle)
		//{
		//	return circle.GetCrossPointsWithLine(this);
		//}
		//public bool IsCrossingCircle(Circle circle)
		//{
		//	return circle.IsCrossedByLine(this);
		//}
		//private static Point[] GetLineCircleCrossPoints(Point circlePosition, float circleRadius, Point pointA, Point pointB)
		//{
		//	var t = 0f;
		//	var dx = pointB.GetX() - pointA.GetX();
		//	var dy = pointB.GetY() - pointA.GetY();
		//	var cx = circlePosition.GetX();
		//	var cy = circlePosition.GetY();
		//	var r = circleRadius;
		//	var A = dx * dx + dy * dy;
		//	var B = 2 * (dx * (pointA.GetX() - cx) + dy * (pointA.GetY() - cy));
		//	var C = (pointA.GetX() - cx) * (pointA.GetX() - cx) + (pointA.GetY() - cy) * (pointA.GetY() - cy) - r * r;
		//	var det = B * B - 4 * A * C;
		//	var lineLength = Point.GetDistance(pointA, pointB);
		//
		//	if ((A <= 0.0000001) || (det < 0))
		//	{
		//		// no real solutions
		//		return new Point[0];
		//	}
		//	else if (det == 0)
		//	{
		//		// one solution
		//		t = -B / (2 * A);
		//		var point = new Point(pointA.GetX() + t * dx, pointA.GetY() + t * dy);
		//		if (Point.GetDistance(point, pointA) >= lineLength) return new Point[1] { point };
		//	}
		//	else
		//	{
		//		var result = new Point[2];
		//		// two solutions
		//		t = (float)((-B + Math.Sqrt(det)) / (2 * A));
		//		var point1 = new Point(pointA.GetX() + t * dx, pointA.GetY() + t * dy);
		//		if (Point.GetDistance(point1, pointA) <= lineLength) result[0] = point1;
		//
		//		t = (float)((-B - Math.Sqrt(det)) / (2 * A));
		//		var point2 = new Point(pointA.GetX() + t * dx, pointA.GetY() + t * dy);
		//		if (Point.GetDistance(point2, pointA) <= lineLength) result[1] = point2;
		//		return result;
		//	}
		//	return new Point[0];
		//}
		//private static Point[] GetCircleCircleCrossPoints(float cx0, float cy0, float radius0, float cx1, float cy1, float radius1)
		//{
		//	// Find the distance between the centers.
		//	float dx = cx0 - cx1;
		//	float dy = cy0 - cy1;
		//	double dist = Math.Sqrt(dx * dx + dy * dy);
		//
		//	// See how many solutions there are.
		//	if (dist > radius0 + radius1)
		//	{
		//		// No solutions, the circles are too far apart.
		//		return new Point[0];
		//	}
		//	else if (dist < Math.Abs(radius0 - radius1))
		//	{
		//		// No solutions, one circle contains the other.
		//		return new Point[0];
		//	}
		//	else if ((dist == 0) && (radius0 == radius1))
		//	{
		//		// No solutions, the circles coincide.
		//		return new Point[0];
		//	}
		//	else
		//	{
		//		var resultA = new Point[1];
		//		var resultA2 = new Point[1];
		//		var resultB = new Point[2];
		//		// Find a and h.
		//		double a = (radius0 * radius0 -
		//			 radius1 * radius1 + dist * dist) / (2 * dist);
		//		double h = Math.Sqrt(radius0 * radius0 - a * a);
		//
		//		// Find P2.
		//		double cx2 = cx0 + a * (cx1 - cx0) / dist;
		//		double cy2 = cy0 + a * (cy1 - cy0) / dist;
		//
		//		// Get the points P3.
		//		var point1 = new Point((float)(cx2 + h * (cy1 - cy0) / dist), (float)(cy2 - h * (cx1 - cx0) / dist));
		//		var point2 = new Point((float)(cx2 - h * (cy1 - cy0) / dist), (float)(cy2 + h * (cx1 - cx0) / dist));
		//		resultA[0] = point1;
		//		resultA2[0] = point2;
		//		resultB[0] = point1;
		//		resultB[1] = point2;
		//
		//		// See if we have 1 or 2 solutions.
		//		if (dist == radius0 + radius1) return resultA[0] == default ? resultA2 : resultA;
		//		return resultB;
		//	}
		//}
	}
}
