using SMPL;

namespace TestGame
{
	public class Game : SMPL.Game
	{
		public static void Main() => Run(new Game(), new Size(1, 1));

      public override void OnStart()
      {
         var p = new Player();

         var driveInfo = Hardware.GetDriveInfo();
      }
   }
}
