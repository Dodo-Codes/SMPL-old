using SMPL.Components;
using SMPL.Data;
using SMPL.Gear;
using SMPL.Prefabs;

namespace TestGame
{
	public class Player
	{
		public Player()
		{
			Assets.Load(Assets.Type.Texture, "penka.png");
			Assets.CallWhen.LoadEnd(OnAssetsLoadEnd);
			Camera.CallWhen.Display(Display);
		}
		void OnAssetsLoadEnd()
		{
			var spr = new Sprite("test");
			spr.Area = new Area("test-1");
			spr.TexturePath = "penka.png";
		}
		void Display(Camera camera)
		{
			var spr = Component.PickByUniqueID("test4") as Sprite;
			if (spr == null) return;
			spr.Display(camera);
		}
	}
}