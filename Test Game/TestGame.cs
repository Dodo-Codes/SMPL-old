using SMPL.Gear;
using SMPL.Data;
using SMPL.Components;

namespace TestGame
{
   public class TestGame : Game
   {
		public TestGame(string uniqueID) : base(uniqueID) { }
      public static void Main() => Start(new TestGame("test-game"), new(3, 3));

		public override void OnGameCreate()
		{
			new Player("test");
		}
	}
}
