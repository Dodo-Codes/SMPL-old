using SMPL;

namespace TestGame
{
	public class TimeEvents : SMPL.TimeEvents
	{
		Camera camera;
		public override void OnStart()
		{
			//camera = new Minimap(new Point(0, 0), new Size(200, 200));
			//camera.TransformComponent.Position = new Point(0, 0);
			//camera.TransformComponent.Size = new Size(500, 500);
			//camera.TransformComponent.Angle = 45;
			//camera.BackgroundColor = Color.DarkRed;
			File.LoadAsset(File.Asset.Texture, "penka.png");
		}
		public override void OnEachTick()
		{
			//camera.Angle += 1;
			//camera.TransformComponent.Angle += 1;
			//camera.Snap("penka.png");
		}
	}
}
