using SMPL;

namespace TestGame
{
	public class TimeEvents : SMPL.TimeEvents
	{
		//Camera camera;
		//public override void OnStart()
		//{
		//	camera = new Minimap(new Point(0, 0), new Size(150, 150));
		//	camera.Draw.BackgroundColor = Color.DarkRed;
		//	camera.Zoom = 0.3;
		//	camera.Draw.Position = new Point(300, 0);
		//}
		public override void OnEachTick()
		{
			//Camera.WorldCamera.Zoom += 0.001;
			//Camera.WorldCamera.ViewAngle++;
			Camera.WorldCamera.DrawComponent.Zoom += 0.01;
			Camera.WorldCamera.DrawComponent.Angle += 1;
		}
	}
}
