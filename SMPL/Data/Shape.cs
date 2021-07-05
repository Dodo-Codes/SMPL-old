using System.Collections.Generic;
using System.Linq;

namespace SMPL
{
	public struct Shape
	{
		public bool Filled { get; set; }
		public Point PointA { get; set; }
		public Point PointB { get; set; }
		public Point PointC { get; set; }
		public Point PointD { get; set; }
		public int EdgeCount
		{
			get
			{
				var c = 0;
				c += PointA == default ? 0 : 1;
				c += PointB == default ? 0 : 1;
				c += PointC == default ? 0 : 1;
				c += PointD == default ? 0 : 1;
				return c;
			}
		}

		public Shape(Point pointA, Point pointB = default, Point pointC = default, Point pointD = default)
		{
			Filled = true;
			PointA = pointA;
			PointB = pointB;
			PointC = pointC;
			PointD = pointD;
		}
	}
}
