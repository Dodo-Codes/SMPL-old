using SMPL;

namespace TestGame
{
	public class Game : SMPL.Game
	{
		public static void Main() => Run(new Game(), new Size(1, 1));

      public override void OnStart()
      {
         new Player();
         new Minimap(new Point(), new Size(500, 500));
         Multiplayer.ConnectClient($"asd", Multiplayer.SameDeviceIP);

         //Multiplayer.StartServer();
      }
   }
}
