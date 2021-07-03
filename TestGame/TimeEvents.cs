using SMPL;

namespace TestGame
{
	public class TimeEvents : SMPL.TimeEvents
	{
		Camera camera;
		public override void OnStart()
		{
			camera = new Minimap(new Point(0, 0), new Size(150, 150))
			{
				BackgroundColor = Color.DarkRed,
				Zoom = 0.3,
				Position = new Point(300, 0)
			};
		}
		public override void OnEachTick()
		{
			//Camera.WorldCamera.Zoom += 0.001;
			//Camera.WorldCamera.ViewAngle++;
			//Camera.WorldCamera.Zoom -= 0.001;
			camera.Angle += 1;
			Camera.WorldCamera.Angle += 1;
		}
	}
}
