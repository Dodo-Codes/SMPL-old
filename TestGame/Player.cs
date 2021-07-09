using SMPL;

namespace TestGame
{
	public class Player : TimeEvents
	{
		public IdentityComponent<Player> IdentityComponent { get; set; }
		public TransformComponent TransformComponent { get; set; }
		public SpriteComponent SpriteComponent { get; set; }

		Timer timer;
		public Player()
		{
			Subscribe(this);
			IdentityComponent = new(this, "player");
			TransformComponent = new(new Point(), 0, new Size(400, 300));
			SpriteComponent = new(TransformComponent, "fire.png");

			SpriteComponent.OriginPercent = new Point(50, 50);
			SpriteComponent.Repeats = new Size(0, 0);
			timer = new Timer("timer-test", 0.5f);
			//https://github.com/anissen/ld34/blob/master/assets/shaders/isolate_bright.glsl
		}
		public override void OnEachFrame()
		{
			TransformComponent.Angle++;
		}
      public override void OnTimerEnd(string timerUniqueID, Timer timerInstance)
      {
			timerInstance.Countdown = timerInstance.Duration;
			Console.Log($"{timer.EndCount * timer.Countdown}");
		}
   }
}
