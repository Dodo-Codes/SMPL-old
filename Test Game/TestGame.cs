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
			Multiplayer.Event.Subscribe.ServerStart(UniqueID);
			Multiplayer.StartServer();
		}
		public override void OnMultiplayerServerStart()
		{
			Multiplayer.SendMessage(new(Multiplayer.Message.Toward.AllClients, "test", "this is a test message", true));
		}
	}
}
