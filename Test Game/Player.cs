using SMPL;

namespace TestGame
{
	public class Player
	{
		public Player()
		{
			File.LoadAsset(File.Asset.Texture, "explosive.jpg");
			ComponentCamera.CallWhen.Display(OnDraw);
			File.CallWhen.AssetLoadEnd(OnAssetLoadEnd);
		}
		void OnAssetLoadEnd()
		{
			var tr = new Component2D();
			tr.Identity = new ComponentIdentity<Component2D>(tr, "test-tr");
			ComponentSprite.Create("test", tr, "explosive.jpg");
		}
		void OnDraw(ComponentCamera camera)
		{
			var spr = ComponentIdentity<ComponentSprite>.PickByUniqueID("test");
			var tr = ComponentIdentity<Component2D>.PickByUniqueID("test-tr");
			if (spr == null) return;
			tr.Angle++;
			spr.DisplayTest(camera);
		}
	}
}