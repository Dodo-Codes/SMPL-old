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
			TransformComponent = new(new Point(), 0, new Size(200, 300));
			SpriteComponent = new(TransformComponent, "fire.png");

			SpriteComponent.Effects.EarthquakeOpacityPercent = 100;
		}
		public override void OnEachFrame()
		{
			SpriteComponent.Time = Time.GameClock;
		}
	}
}
