using SMPL;

namespace TestGame
{
	public class Player
	{
		public Player()
		{
			File.LoadAsset(File.Asset.Texture, "penka.png");

			Events.OnStart += new(OnStart);
			Events.OnAudioStart += new(OnAudioStart);
		}
		public void OnStart()
		{

		}
		void OnAudioStart(Audio instance)
		{

		}
	}
}
