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
			var sl = new SegmentedLine("test", 50, 50, 50);
		}
		void OnDraw(Camera camera)
		{
			if (Performance.FrameCount % 20 == 0) Window.Title = $"Test Game ({Performance.FPS:F2} FPS)";

			var sl = Identity<SegmentedLine>.PickByUniqueID("test");
			//sl.OriginPosition = new Point(-100, -100);
			sl.TargetPosition = Mouse.CursorPositionWindow;
			sl.Display(camera);
		}
	}
}