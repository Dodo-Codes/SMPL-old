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
			var r = new SegmentedLine("test", new Point(0, 0), 10, 10, 10, 10, 10, 10, 10, 10, 10);
			//Multiplayer.StartServer(false);
			Multiplayer.ConnectClient("test", Multiplayer.SameDeviceIP, false);
			Multiplayer.MessagesAreLogged = true;
		}
		void OnDraw(Camera camera)
		{
			Multiplayer.SendMessage(
				new Multiplayer.Message("test", $"{Performance.FrameCount}", Multiplayer.Receivers.ServerAndAllClients));
			if (Performance.FrameCount % 20 == 0) Window.Title = $"Test Game ({Performance.FPS:F2} FPS)";

			var r = Identity<SegmentedLine>.PickByUniqueID("test");
			r.TargetPosition = Mouse.CursorPositionWindow;
			r.Display(camera);
		}
	}
}