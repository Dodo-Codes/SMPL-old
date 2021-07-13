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
		}
      public override void OnAssetsLoadingEnd()
      {
			if (File.AssetIsLoaded("Munro.ttf") == false) return;
			TextComponent = new(TransformComponent, "Munro.ttf");
      }
      public override void OnDraw(Camera camera)
      {
			if (TextComponent == null) return;
			TextComponent.Draw(camera);
			TextComponent.BoxOriginPercent = new Point(50, 50);
			TextComponent.Color = Color.Black;
			TextComponent.CharacterSize = 32;
			TextComponent.Position += new Point(0, -1);
		}
	}
}
