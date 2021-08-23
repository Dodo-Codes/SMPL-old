using SMPL;

namespace TestGame
{
	public class Player
	{
		public Player()
		{
			Keyboard.CallWhen.KeyHold(OnKeyHold);
			Keyboard.CallWhen.KeyRelease(OnKeyRelease);
			File.LoadAsset(File.Asset.Font, "Munro.ttf");
		}
		void OnKeyHold(Keyboard.Key key)
		{
			ComponentText.Create($"{Performance.FrameCount}", new Component2D(), "Munro.ttf");
			ComponentIdentity<ComponentText>.PickByUniqueID($"{Performance.FrameCount}").IsDestroyed = true;
		}
		void OnKeyRelease(Keyboard.Key key)
		{
			//File.UnloadAsset(File.Asset.Texture, "big.jpg");
			//var spr = ComponentIdentity<ComponentSprite>.PickByUniqueID("test");
			//spr.IsDestroyed = true;
			//Console.Log(spr.SizePercent);
			//spr.IsDestroyed = false;
		}
	}
}