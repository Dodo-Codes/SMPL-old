using SMPL;

namespace TestGame
{
	public class TimeEvents : SMPL.TimeEvents
	{
		Camera camera;
		public override void OnStart()
		{
			camera = new Minimap(new Point(0, 0), new Size(500, 500));
			camera.TransformComponent.Position = new Point(300, 0);
			camera.BackgroundColor = Color.DarkRed;

		}
		public override void OnEachTick()
		{
			//Camera.WorldCamera.Angle++;
			//camera.Size /= new Size(1.001, 1.001);
			camera.Angle += 1;
			//camera.TransformComponent.Angle -= 1;
			//camera.TransformComponent.Position += new Point(1, 0);
			//Camera.WorldCamera.DrawComponent.Zoom += 0.001;
			//Camera.WorldCamera.DrawComponent.ViewAngle += 1;
		}
	}
}
