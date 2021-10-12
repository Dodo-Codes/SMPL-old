using SMPL.Gear;

namespace TestGame
{
   public class TestGame : Game
   {
		public TestGame(string uniqueID) : base(uniqueID) { }
      public static void Main() => Start(new TestGame("test-game"), new(1, 1));

		public override void OnGameCreate()
		{
			Event.Subscribe.Update(UniqueID, 0);
		}

		public override void OnGameUpdate()
		{
			Console.Log("hello");
		}
	}
}
