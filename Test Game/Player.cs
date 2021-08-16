using SMPL;

namespace TestGame
{
	public class Player : ComponentAccess
	{
		ComponentIdentity<Camera> id = new ComponentIdentity<Camera>(Camera.WorldCamera, "world");

		public Player()
		{

		}
	}
}
