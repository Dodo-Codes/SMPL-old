using SMPL;

namespace TestGame
{
	public class Minimap : Camera
	{
		public Minimap(Point viewPosition, Size viewSize) :
			base(viewPosition, viewSize)
		{
			IdentityComponent = new(this, "minimap");
		}

		public override void OnDraw()
		{
			var p1 = new Point(0, 0);
			var p2 = new Point(100, 0);
			var p3 = new Point(100, 100);
			var p4 = new Point(-100, 200);

			p1.Color = Color.LightRed;
			p3.Color = Color.DarkBlue;

			var shape = new Shape(p1, p2, p3, p4);

			DrawShapes(shape);
		}
	}
}
