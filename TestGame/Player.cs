using SMPL;

namespace TestGame
{
	public class Player : Events
	{
		public IdentityComponent<Player> IdentityComponent { get; set; }
		public TransformComponent TransformComponent { get; set; }
		public TextComponent TextComponent { get; set; }

		public Player()
		{
			Subscribe(this);
			IdentityComponent = new(this, "player");
			TransformComponent = new(new Point(0, 0), 0, new Size(400, 200));

			File.LoadAsset(File.Asset.Font, "Munro.ttf");
			File.LoadAsset(File.Asset.Sound, "music.ogg");
		}
      public override void OnAssetsLoadingEnd()
      {
			if (File.AssetIsLoaded("Munro.ttf"))
			{
				TextComponent = new(TransformComponent, "Munro.ttf");
			}
         if (File.AssetIsLoaded("music.ogg"))
         {
				var sound = new Audio("music.ogg") { IsPlaying = true };
				sound.IdentityComponent = new(sound, "whistle");
			}
      }
		public override void OnDraw(Camera camera)
      {
			if (TextComponent == null) return;
			TextComponent.Draw(camera);
			TextComponent.BoxOriginPercent = new Point(50, 50);
			TextComponent.Color = Color.Black;
			TextComponent.CharacterSize = 32;
			TextComponent.Position = Point.MoveAtAngle(TextComponent.Position, 90, 10);
		}
		public override void OnKeyPress(Keyboard.Key key)
		{
			var music = IdentityComponent<Audio>.PickByUniqueID("whistle");
			if (key == Keyboard.Key.A) music.IsPlaying = true;
			if (key == Keyboard.Key.S) music.IsPaused = true;
			if (key == Keyboard.Key.D) music.IsPaused = false;
			if (key == Keyboard.Key.F) music.IsPlaying = false;
		}
		public override void OnEarlyAudioStart(Audio audio)
		{
			Console.Log("early");
		}
		public override void OnAudioStart(Audio audio)
		{
			Console.Log("start");
		}
		public override void OnLateAudioStart(Audio audio)
		{
			Console.Log("late");
		}
	}
}
