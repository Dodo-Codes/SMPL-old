using SMPL;

namespace TestGame
{
	public class Player
	{
		public IdentityComponent<Player> IdentityComponent { get; set; }
		public TransformComponent TransformComponent { get; set; }
		public SpriteComponent SpriteComponent { get; set; }

		public Player()
		{
			IdentityComponent = new(this, "player");
			TransformComponent = new(new Point(), 0, new Size(400, 400));
			SpriteComponent = new(TransformComponent, "penka.png");
		}
	}
}
