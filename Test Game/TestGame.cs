using SMPL.Gear;
using SMPL.Data;

namespace TestGame
{
	public class TestGame : Game
	{
      public static void Main()
      {
			try
			{
				Run(new TestGame(), new Size(3, 3));
			}
			catch (System.Exception ex)
			{
				Console.Log(ex.Message);
				throw;
			}
      }

      public override void OnGameCreated()
      {
         new Player();
      }
   }
}
