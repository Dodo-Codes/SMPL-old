using SMPL;

namespace TestGame
{
	public class TimeEvents : SMPL.TimeEvents
	{
		Camera camera;
		public override void OnStart()
		{
			camera = new Minimap(new Point(0, 0), new Size(150, 150));
			camera.TransformComponent.Position = new Point(300, 0);
			camera.ViewComponent.BackgroundColor = Color.DarkRed;
			camera.ViewComponent.Angle = 45;
		}
		public override void OnEachTick()
		{
			camera.ViewComponent.Angle += 1;
			//camera.TransformComponent.Position += new Point(1, 0);
			//Camera.WorldCamera.DrawComponent.Zoom += 0.001;
			//Camera.WorldCamera.DrawComponent.ViewAngle += 1;
		}
	}
}
