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
			//Window.IsHidden = true;
			////Multiplayer.StartServer();
			//Multiplayer.ConnectClient("test", Multiplayer.SameDeviceIP);
			//Multiplayer.CallWhen.MessageReceive(OnMessageReceived);
			//
			//Time.CallWhen.Update(Always);
			//Camera.CallWhen.Display(OnDraw);
			//var r = new SegmentedLine("test", new Point(0, 0), 10, 10, 10, 10, 10, 10, 10, 10, 10);
			try
			{
				Console.Log(Debug.CurrentFileDirectory(2));
				Always();
			}
			catch (System.Exception ex)
			{
				Console.Log(ex.Message);
				throw;
			}
		}
		void Always()
		{
			Console.Log(Debug.CurrentFileDirectory(1));
			//Multiplayer.SendMessage(new Multiplayer.Message(Multiplayer.Message.Toward.ServerAndAllClients, "hi", "hello", false));
		}
		void OnMessageReceived(Multiplayer.Message msg)
		{
			//Console.Log(msg);
		}
		//void OnDraw(Camera camera)
		//{
		//	if (Performance.FrameCount % 20 == 0) Window.Title = $"Test Game ({Performance.FPS:F2} FPS)";
		//}
	}
}