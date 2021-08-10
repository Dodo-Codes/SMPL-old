using SMPL;

namespace TestGame
{
	public class Player
	{
		public Player()
		{
			File.LoadAsset(File.Asset.Sound, "whistle.wav");

			Time.AlwaysCall(Always, 0);
		}
		void Always()
		{
			Console.Log("always");
		}
	}
}
