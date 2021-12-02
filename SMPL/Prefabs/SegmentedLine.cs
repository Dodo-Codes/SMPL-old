using Newtonsoft.Json;
using SMPL.Components;
using SMPL.Data;
using System;
using System.Collections.Generic;
using SMPL.Gear;

namespace SMPL.Prefabs
{
	public class SegmentedLine : Thing
	{
		private Point[] points;
		private Point originPosition, targetPosition;
		private readonly double[] lengths;

		private void Constrain()
		{
			var originPoint = points[0];

			for (int i = 0; i < 16; i++)
			{
				var startingFromTarget = i % 2 == 0;
				Array.Reverse(points);
				Array.Reverse(lengths);
				points[0] = startingFromTarget ? targetPosition : originPoint;

				for (int j = 1; j < points.Length; j++)
				{
					var dir = Point.Normalize(points[j] - points[j - 1]);
					points[j] = points[j - 1] + dir * lengths[j - 1];
				}
				var dstToTarget = Point.Magnitude(points[^1] - targetPosition);
				if (startingFromTarget == false && dstToTarget <= 0.01) return;
			}
		}

		// ========

		internal static List<SegmentedLine> lines = new();

		internal void Update()
		{
			points[0] = originPosition;
			Constrain();
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
			private set { if (ErrorIfDestroyed() == false) originPosition = value; Update(); }
		}
		[JsonProperty]
		public Point TargetPosition
		{
			get { return ErrorIfDestroyed() ? Point.Invalid : targetPosition; }
			set { if (ErrorIfDestroyed() == false) targetPosition = value; Update(); }
		}

		public SegmentedLine(Point originPosition, params double[] segmentLengths)
		{
			lines.Add(this);

			lengths = (double[])segmentLengths.Clone();
			points = new Point[segmentLengths.Length + 1];
			points[0] = originPosition;
			for (int i = 1; i < points.Length; i++)
				points[i] = points[i - 1] + new Point(segmentLengths[i - 1], 0);

			OriginPosition = originPosition;

			Update();
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
			if (ErrorIfDestroyed() || Window.DrawNotAllowed()) return;
			for (int i = 0; i < points.Length - 1; i++)
				new Line(points[i], points[i + 1]).Display(camera, width);
		}
	}
}
