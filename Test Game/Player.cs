using SMPL;

namespace TestGame
{
	public class Player
	{
		Component2D tr = new();
		ComponentSprite spr;

		public Player()
		{
			File.CallOnAssetLoadEnd(End);
			Camera.CallOnDisplay(OnDisplay);
			File.LoadAsset(File.Asset.Texture, "pictures\\objects\\Houses\\1.png");
		}
		void End()
		{
			spr = new(tr, "pictures\\objects\\Houses\\1.png");
		}
		void OnDisplay(Camera camera)
		{
			if (spr == null) return;
			spr.Display(camera);
		}
	}
}
