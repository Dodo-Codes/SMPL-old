using SMPL;

namespace TestGame
{
	public class Player : Events
	{
		public Player()
		{
			Subscribe(this, 0);

			File.LoadAsset(File.Asset.Texture, "penka.png");
		}
		public override void OnAssetsLoadingEnd()
		{
			if (File.AssetIsLoaded("penka.png"))
			{
				//Window.IconTexturePath = "penka.png";
			}
		}
		public override void OnEachFrame()
		{
			Camera.WorldCamera.Angle = 45;
		}
	}
}
