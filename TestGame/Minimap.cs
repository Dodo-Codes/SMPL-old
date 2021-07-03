using SMPL;

namespace TestGame
{
	public class Minimap : Camera
	{
		public Minimap(Point viewPosition, Size viewSize) :
			base(viewPosition, viewSize) { }

		public override void OnDraw()
		{
			var line = new Line(new Point(0, 0), new Point(100, 0));
			var line2 = new Line(new Point(100, 0), new Point(100, 100));
			var line3 = new Line(new Point(100, 100), new Point(0, 100));
			var line4 = new Line(new Point(0, 100), new Point(0, 0));
			DrawLines(line, line2, line3, line4);
		}
	}
}
