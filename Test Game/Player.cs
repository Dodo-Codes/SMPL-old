using SMPL.Gear;

namespace TestGame
{
	public class Player : Thing
	{
      public Player(string uniqueID) : base(uniqueID)
      {
			Window.Event.Subscribe.Resize(uniqueID);
      }

		public override void OnWindowResize()
		{
			Console.Log(Window.CurrentState);
		}
	}
}