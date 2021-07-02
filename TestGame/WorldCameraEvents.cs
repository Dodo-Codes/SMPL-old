using SMPL;

namespace TestGame
{
	public class WorldCameraEvents : SMPL.WorldCameraEvents
	{
		public override void OnDraw()
		{
			var line = new Line(new Point(0, 0), new Point(100, 100));
			DrawLines(line);
		}
	}
}
