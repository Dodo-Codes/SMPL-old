using SMPL;

namespace TestGame
{
	public class Player
	{
		ComponentAudio spr;
		public Player()
		{
			File.CallWhen.AssetLoadEnd(AssetLoadEnd);
			ComponentAudio.CallWhen.Loop(OnLoop);
			Keyboard.CallWhen.KeyPress(OnKeyPress);

			File.LoadAsset(File.Asset.Music, "music.ogg");
		}
		void OnKeyPress(Keyboard.Key key)
		{
			spr.FileProgress = 0;
		}
		void AssetLoadEnd()
		{
			spr = new ComponentAudio("music.ogg");
			spr.IsPlaying = true;
			spr.IsLooping = true;
			Console.Log(spr.Duration);
		}
		void OnLoop(ComponentAudio audio)
		{
			Console.Log("loop");
		}
	}
}