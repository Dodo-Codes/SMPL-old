using SMPL;

namespace TestGame
{
	public class Player : Events
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
			new Timer("timer-test", 0.5f);
			//https://github.com/anissen/ld34/blob/master/assets/shaders/isolate_bright.glsl
		}
		public override void OnEachFrame()
		{
			TransformComponent.Angle++;
		}
      public override void OnKeyPress(Keyboard.Key key)
      {
			Console.Log(key);
      }
      public override void OnTimerEnd(Timer timerInstance)
      {
			if (timerInstance.IdentityComponent.UniqueID == "timer-test")
         {
				timerInstance.Progress = 0;
         }
		}
   }
}
