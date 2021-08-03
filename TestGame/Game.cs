using SMPL;

namespace TestGame
{
	public class Game : SMPL.Game
	{
		public static void Main() => Run(new Game(), new Size(2, 2));

      public override void OnStart()
      {
         new Player();
      }
   }
}
