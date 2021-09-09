using SMPL.Components;
using SMPL.Data;
using SMPL.Gear;
using SMPL.Prefabs;
using System;

namespace TestGame
{
	public class Player
	{
		public Player()
		{
			var spr = new Sprite("test");
			new Effects("eff");
			new Area("arr");
			spr.EffectsUniqueID = "eff";
			spr.AreaUniqueID = "arr";
			Camera.CallWhen.Display(OnDisplay);
			Assets.Load(Assets.Type.Texture, "cape.jpg");
		}

      private void OnDisplay(Camera camera)
      {
			if (Assets.AreLoaded("cape.jpg") == false) return;
			var spr = (Sprite)Component.PickByUniqueID("test");
			spr.TexturePath = "cape.jpg";
			var eff = (Effects)Component.PickByUniqueID("eff");
			eff.TintColor = Color.Blue;
			spr.Display(camera);
      }
   }
}