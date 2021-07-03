using SMPL;

namespace TestGame
{
	public class TimeEvents : SMPL.TimeEvents
	{
		Camera camera;
		public override void OnStart()
		{
			camera = new Minimap(new Point(0, 0), new Size(50, 50), new Point(0, 0), new Size(150, 150));
			camera.BackgroundColor = Color.DarkRed;
			camera.Zoom = 0.3;
			camera.Position = new Point(300, 0);
		}
		public override void OnEachTick()
		{
			camera.ViewAngle += 1;
			Camera.WorldCamera.Zoom += 0.001;
			Camera.WorldCamera.ViewAngle += 1;
		}
	}
}
