using SMPL;

namespace TestGame
{
	public class WorldCameraEvents : SMPL.WorldCameraEvents
	{
		public override void OnDraw()
		{
			var shape = new Shape(new Point(0.1, 0), new Point(100, 0), new Point(100, 100), new Point(0, 100));

			DrawShapes(shape);
		}
	}
}
