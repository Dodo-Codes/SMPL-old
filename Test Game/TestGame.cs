using SMPL.Gear;
using SMPL.Data;
using SMPL.Components;

namespace TestGame
{
   public class TestGame : Game
   {
		public TestGame(string uniqueID) : base(uniqueID) { }
      public static void Main() => Start(new TestGame("test-game"), new(1, 1));

		public override void OnGameCreate()
		{

		}
	}
}
