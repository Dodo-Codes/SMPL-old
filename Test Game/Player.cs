using SMPL.Components;
using SMPL.Data;
using SMPL.Gear;

namespace TestGame
{
	public class Player
	{
		public Player()
		{
			File.LoadAsset(File.Asset.Texture, "explosive.jpg");
			Camera.CallWhen.Display(OnDraw);
			File.CallWhen.AssetLoadEnd(OnAssetLoadEnd);
		}
		void OnAssetLoadEnd()
		{
			Area.Create("test-tr");
			var tr = Identity<Area>.PickByUniqueID("test-tr");
			Sprite.Create("test", tr, "explosive.jpg");
		}
		void OnDraw(Camera camera)
		{
			var spr = Identity<Sprite>.PickByUniqueID("test");
			var tr = Identity<Area>.PickByUniqueID("test-tr");
			if (spr == null) return;
			//tr.Angle++;
			tr.Size = new Size(800, 800);
			spr.Display(camera);

			//quad.Display(camera);
		}
	}
}