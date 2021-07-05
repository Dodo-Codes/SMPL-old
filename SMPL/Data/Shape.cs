using System.Collections.Generic;
using System.Linq;

namespace SMPL
{
	public struct Shape
	{
		public Point PointA { get; set; }
		public Point PointB { get; set; }
		public Point PointC { get; set; }
		public Point PointD { get; set; }

		public Shape(Point pointA, Point pointB, Point pointC, Point pointD)
		{
			PointA = pointA;
			PointB = pointB;
			PointC = pointC;
			PointD = pointD;
		}
	}
}
