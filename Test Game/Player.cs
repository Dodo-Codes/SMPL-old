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
			Camera.CallWhen.Display(OnDraw);
			var r = new Rope("test");
		}
		void OnDraw(Camera camera)
		{
			if (Performance.FrameCount % 20 == 0) Window.Title = $"Test Game ({Performance.FPS:F2} FPS)";

			var r = Identity<Rope>.PickByUniqueID("test");
			r.Display(camera);
		}
	}
}