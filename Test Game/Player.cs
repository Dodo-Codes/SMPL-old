using SMPL;

namespace TestGame
{
	public class Player
	{
		public Player()
		{
			File.CallWhen.AssetLoadEnd(AssetLoadEnd);
			ComponentCamera.CallWhen.Display(OnDisplay);

			File.LoadAsset(File.Asset.Font, "Munro.ttf");
		}
		void AssetLoadEnd()
		{
			var spr = new ComponentText(new Component2D(), "Munro.ttf");
			spr.Identity = new(spr, "explosive");
		}
		void OnDisplay(ComponentCamera camera)
		{
			var spr = ComponentIdentity<ComponentText>.PickByUniqueID("explosive");
			if (spr == null) return;
			spr.BackgroundColor = Color.Red;
			spr.Display(camera);
		}
	}
}