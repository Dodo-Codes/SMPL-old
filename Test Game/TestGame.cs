using SMPL.Components;
using SMPL.Data;
using SMPL.Gear;
using SMPL.Prefabs;

namespace TestGame
{
   public class TestGame : Game
   {
		public TestGame(string uniqueID) : base(uniqueID) { }
      public static void Main() => Start(new TestGame("test-game"), new(1, 1));

		public override void OnGameCreate()
		{
			Event.Subscribe.Update(UniqueID);
			Multiplayer.ConnectClient("test", "192.168.1.10");
		}
		public override void OnGameUpdate()
		{
			if (Performance.FrameCount == 3000)
				Multiplayer.SendMessage(new(Multiplayer.Message.Toward.Server, "test", "this is a test message", true));
		}
	}
}
