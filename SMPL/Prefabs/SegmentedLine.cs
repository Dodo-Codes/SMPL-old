using SMPL.Components;
using SMPL.Data;
using SMPL.Gear;
using System;
using System.Collections.Generic;

namespace SMPL.Prefabs
{
	public class SegmentedLine : Access
	{
		internal static List<SegmentedLine> lines = new();
		private Point[] points;
		public Point[] Points
		{
			get { Update(); return (Point[])points.Clone(); }
			private set { points = value; Update(); }
		}
		private Point originPosition;
		public Point OriginPosition
		{
			get { return AllAccess == Extent.Removed ? default : originPosition; }
			set
			{
				if (originPosition == value || (Debug.CalledBySMPL == false && IsCurrentlyAccessible() == false)) return;
				var prev = originPosition;
				originPosition = value;
				Update();
				//if (Debug.CalledBySMPL == false) OnTexturePathChange?.Invoke(this, prev);
			}
		}
		private Point targetPosition;
		public Point TargetPosition
		{
			get { return AllAccess == Extent.Removed ? default : targetPosition; }
			set
			{
				if (targetPosition == value || (Debug.CalledBySMPL == false && IsCurrentlyAccessible() == false)) return;
				var prev = targetPosition;
				targetPosition = value;
				Update();
				//if (Debug.CalledBySMPL == false) OnTexturePathChange?.Invoke(this, prev);
			}
		}

		private static event Events.ParamsTwo<SegmentedLine, Identity<SegmentedLine>> OnIdentityChange;
      private static event Events.ParamsTwo<SegmentedLine, string> OnCreate;
      private static event Events.ParamsOne<SegmentedLine> OnDestroy;

		public static class CallWhen
		{
			public static void Create(Action<SegmentedLine, string> method, uint order = uint.MaxValue) =>
				OnCreate = Events.Add(OnCreate, method, order);
			public static void IdentityChange(Action<SegmentedLine, Identity<SegmentedLine>> method,
				uint order = uint.MaxValue) => OnIdentityChange = Events.Add(OnIdentityChange, method, order);
			public static void Destroy(Action<SegmentedLine> method, uint order = uint.MaxValue) =>
				OnDestroy = Events.Add(OnDestroy, method, order);
		}

      private Identity<SegmentedLine> identity;
		public Identity<SegmentedLine> Identity
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

		private bool isDestroyed;
		public bool IsDestroyed
		{
			get { return isDestroyed; }
			set
			{
				if (isDestroyed == value || (Debug.CalledBySMPL == false && IsCurrentlyAccessible() == false)) return;
				isDestroyed = value;

				if (Identity != null) Identity.Dispose();
				lines.Remove(this);
				Identity = null;
				AllAccess = Extent.Removed;
				if (Debug.CalledBySMPL == false) OnDestroy?.Invoke(this);
			}
		}

		public SegmentedLine(string uniqueID, params double[] segmentLengths)
		{
			if (Identity<Audio>.CannotCreate(uniqueID)) return;
			Identity = new(this, uniqueID);

			GrantAccessToFile(Debug.CurrentFilePath(1)); // grant the user access
			DenyAccessToFile(Debug.CurrentFilePath(0)); // abandon ship

			lines.Add(this);

			originPosition = new Point(100, 100);

			points = new Point[segmentLengths.Length + 1];
			points[0] = originPosition;
			for (int i = 1; i < points.Length; i++)
				points[i] = points[i - 1] + new Point(segmentLengths[i - 1], 0);
		}
		public void Display(Camera camera)
		{
			for (int i = 0; i < points.Length - 1; i++)
				new Line(points[i], points[i + 1]).Display(camera);
		}

		internal void Update()
		{
			points[0] = originPosition;
			Constrain(points, targetPosition);
		}
		internal static void Constrain(Point[] points, Point targetPoint)
		{
			var originPoint = points[0];
			var segmentLengths = new double[points.Length - 1];
			for (int i = 0; i < points.Length - 1; i++)
				segmentLengths[i] = Magnitude(points[i + 1] - points[i]);

			for (int i = 0; i < 100; i++)
			{
				var startingFromTarget = i % 2 == 0;
				Array.Reverse(points);
				Array.Reverse(segmentLengths);
				points[0] = startingFromTarget ? targetPoint : originPoint;

				for (int j = 1; j < points.Length; j++)
				{
					var dir = Normalize(points[j] - points[j - 1]);
					points[j] = points[j - 1] + dir * segmentLengths[j - 1];
				}
				var dstToTarget = Magnitude(points[^1] - targetPoint);
				if (startingFromTarget == false && dstToTarget <= 0.01) return;
			}

			double Magnitude(Point p) => Number.SquareRoot(p.X * p.X + p.Y * p.Y);
			Point Normalize(Point p) { var m = Magnitude(p); return new Point(p.X / m, p.Y / m); }
		}
	}
}
