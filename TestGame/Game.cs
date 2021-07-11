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
         Multiplayer.ConnectClient("asd", "dd");

         var msg = new Multiplayer.Message("test", "my test message", Multiplayer.Receivers.Client, "jojo");
         Multiplayer.SendMessage(msg);
         //Multiplayer.ConnectClient("asd", Multiplayer.SameDeviceIP);
      }
   }
}
