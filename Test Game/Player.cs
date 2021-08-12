using SMPL;

namespace TestGame
{
	public class Player
	{
		ComponentIdentity<Player> id;

		public Player()
		{
			ComponentIdentity<Player>.CallOnCreate(OnCreate);
			id = new(this, "player");
		}
		void OnCreate(ComponentIdentity<Player> id)
		{
			Console.Log(id.UniqueID);
		}
	}
}
