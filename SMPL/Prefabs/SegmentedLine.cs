using Newtonsoft.Json;
using SMPL.Components;
using SMPL.Data;
using System;
using System.Collections.Generic;

namespace SMPL.Prefabs
{
	public class SegmentedLine : Component
	{
		private Point[] points;
		private Point originPosition, targetPosition;

		private static void Constrain(Point[] points, Point targetPoint)
		{
			var originPoint = points[0];
			var segmentLengths = new double[points.Length - 1];
			for (int i = 0; i < points.Length - 1; i++)
				segmentLengths[i] = Point.Magnitude(points[i + 1] - points[i]);

			for (int i = 0; i < 100; i++)
			{
				var startingFromTarget = i % 2 == 0;
				Array.Reverse(points);
				Array.Reverse(segmentLengths);
				points[0] = startingFromTarget ? targetPoint : originPoint;

				for (int j = 1; j < points.Length; j++)
				{
					var dir = Point.Normalize(points[j] - points[j - 1]);
					points[j] = points[j - 1] + dir * segmentLengths[j - 1];
				}
				var dstToTarget = Point.Magnitude(points[^1] - targetPoint);
				if (startingFromTarget == false && dstToTarget <= 0.01) return;
			}
		}

		// ========

		internal static List<SegmentedLine> lines = new();

		internal static void Update()
		{
			for (int i = 0; i < lines.Count; i++)
			{
				lines[i].points[0] = lines[i].originPosition;
				Constrain(lines[i].points, lines[i].targetPosition);
			}
		}

		// ========

		[JsonProperty]
		public Point[] Points
		{
			get { return ErrorIfDestroyed() ? Array.Empty<Point>() : (Point[])points.Clone(); }
			private set { if (ErrorIfDestroyed() == false) points = value; }
		}
		[JsonProperty]
		public Point OriginPosition
		{
			get { return ErrorIfDestroyed() ? Point.Invalid : originPosition; }
			private set { if (ErrorIfDestroyed() == false) originPosition = value; }
		}
		[JsonProperty]
		public Point TargetPosition
		{
			get { return ErrorIfDestroyed() ? Point.Invalid : targetPosition; }
			set { if (ErrorIfDestroyed() == false) targetPosition = value; }
		}

		public SegmentedLine(string uniqueID, Point originPosition, params double[] segmentLengths) : base(uniqueID)
		{
			lines.Add(this);

			OriginPosition = originPosition;

			points = new Point[segmentLengths.Length + 1];
			points[0] = originPosition;
			for (int i = 1; i < points.Length; i++)
				points[i] = points[i - 1] + new Point(segmentLengths[i - 1], 0);

			if (cannotCreate) { ErrorAlreadyHasUID(uniqueID); Destroy(); }
		}
		public override void Destroy()
		{
			if (ErrorIfDestroyed()) return;
			lines.Remove(this);
			points = null;
			base.Destroy();
		}

		public void Display(Camera camera, double width)
		{
			if (ErrorIfDestroyed() == false)
				for (int i = 0; i < points.Length - 1; i++)
					new Line(points[i], points[i + 1]).Display(camera, width);
		}
	}
}
