using SMPL;

namespace TestGame
{
	public class Player
	{
		Component2D component2D = new();

		public Player()
		{
			Component2D.CallOnAngleChange(Change);
			Component2D.CallOnAngleChangeStart(Start);
			Component2D.CallOnAngleChangeEnd(End);
			Keyboard.CallOnKeyHold(KeyPressed);
		}

		void KeyPressed(Keyboard.Key key)
		{
			component2D.Angle++;
		}
		void Start(Component2D component2D, double prevAngle)
		{
			Console.Log("start");
		}
		void Change(Component2D component2D, double prevAngle)
		{
			Console.Log("change");
		}
		void End(Component2D component2D)
		{
			Console.Log("end");
		}
	}
}
