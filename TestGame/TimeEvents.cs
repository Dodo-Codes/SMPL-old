using SMPL;

namespace TestGame
{
	public class TimeEvents : SMPL.TimeEvents
	{
		Camera camera;
		public override void OnStart()
		{
			camera = new Minimap(new Point(-300, 0), new Size(100, 100), new Point(0, 0), new Size(200, 200));
			camera.BackgroundColor = Color.DarkRed;
			camera.Zoom = 0.3;
			camera.ViewPosition = new Point(300, 0);
		}
		public override void OnEachTick()
		{
			//Camera.WorldCamera.Zoom += 0.001;
			//Camera.WorldCamera.ViewAngle++;
			//Camera.WorldCamera.Zoom -= 0.001;
			camera.ViewAngle += 1;
			Camera.WorldCamera.ViewAngle += 1;
		}
	}
}
