using SMPL;

namespace TestGame
{
	public class Minimap : Camera
	{
		public Minimap(Point viewPosition, Size viewSize) :
			base(viewPosition, viewSize)
		{
			IdentityComponent.Identify(this, "minimap");
		}

		public override void OnDraw()
		{
			var p1 = new Point(0, 0);
			p1.Color = Color.NormalRed;
			var p2 = new Point(100, 0);
			p2.Color = Color.NormalRed;
			var p3 = new Point(100, 100);
			p3.Color = Color.NormalRed;
			var p4 = new Point(0, 100);
			p4.Color = Color.NormalRed;
			var shape = new Shape(p1, p2, p3, p4);

			DrawShapes(shape);
		}
	}
}
