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
			var p1 = new Point(0.1, 0);
			p1.Color = Color.NormalMagenta;
			var shape = new Shape(p1, new Point(100, 0), new Point(100, 100), new Point(0, 100));

			DrawShapes(shape);
		}
	}
}
