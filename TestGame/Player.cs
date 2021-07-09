using SMPL;

namespace TestGame
{
	public class Player : TimeEvents
	{
		public IdentityComponent<Player> IdentityComponent { get; set; }
		public TransformComponent TransformComponent { get; set; }
		public SpriteComponent SpriteComponent { get; set; }

		public Player()
		{
			Subscribe(this);
			IdentityComponent = new(this, "player");
			TransformComponent = new(new Point(), 0, new Size(400, 300));
			SpriteComponent = new(TransformComponent, "fire.png");

			SpriteComponent.OriginPercent = new Point(50, 50);
			SpriteComponent.Repeats = new Size(0, 0);
			//https://github.com/anissen/ld34/blob/master/assets/shaders/isolate_bright.glsl
		}
		public override void OnEachFrame()
		{
			TransformComponent.Angle++;
			SpriteComponent.Effects.Progress = Time.GameClock;
		}
	}
}
