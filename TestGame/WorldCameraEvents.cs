using SMPL;

namespace TestGame
{
	public class WorldCameraEvents : SMPL.WorldCameraEvents
	{
		public override void OnDraw()
		{
			var player = IdentityComponent<Player>.PickByUniqueID("player");
			if (player == null) return;
			DrawSprites(player.SpriteComponent);
		}
	}
}
