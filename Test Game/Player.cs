using SMPL.Gear;

namespace TestGame
{
	public class Player : Thing
	{
      public Player(string uniqueID) : base(uniqueID)
      {
			Mouse.Event.Subscribe.ButtonDoubleClick(uniqueID, 0);
      }

		public override void OnMouseButtonDoubleClick(Mouse.Button button)
		{
			Console.Log("test");
			Mouse.Event.Unsubscribe.ButtonDoubleClick(UniqueID);
		}
	}
}