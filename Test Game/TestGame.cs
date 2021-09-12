using SMPL.Gear;
using SMPL.Data;

namespace TestGame
{
   public class TestGame : Game
   {
      public static void Main() => Start(new TestGame(), new Size(3, 3));

      public override void OnGameCreated()
      {
         new Player();
      }
   }
}
