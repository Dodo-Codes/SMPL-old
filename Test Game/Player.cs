using SMPL;

namespace TestGame
{
	public class Player
	{
		Component2D comp2D = new();
		ComponentHitbox hitbox = new();

		public Player()
		{
			Component2D.CallOnAddHitbox(OnHitboxAdd);
			comp2D.AddHitboxes(hitbox);
		}

		void OnHitboxAdd(Component2D comp2D, ComponentHitbox compHitbox)
		{
			Console.Log("test");
		}
	}
}