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

			File.LoadAsset(File.Asset.Texture, "fire.png");
		}
      public override void OnAssetsLoadingEnd()
      {
			if (File.AssetIsLoaded("fire.png") == false) return;
			SpriteComponent = new(TransformComponent, "fire.png");
      }
      public override void OnDraw(Camera camera)
      {
			if (SpriteComponent == null) return;
			SpriteComponent.Draw(camera);
		}
	}
}
