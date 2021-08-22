using SMPL;

namespace TestGame
{
	public class Player
	{
		ComponentSprite sprite;
		public Player()
		{
			Keyboard.CallWhen.KeyHold(OnKeyPress);
			Keyboard.CallWhen.KeyRelease(OnKeyRelease);
			File.LoadAsset(File.Asset.Texture, "big.jpg");
		}
		void OnKeyPress(Keyboard.Key key)
		{
			ComponentSprite.Create("test", new Component2D(), "big.jpg");
			sprite = ComponentIdentity<ComponentSprite>.PickByUniqueID("test");
			sprite.Destroy();
		}
		void OnKeyRelease(Keyboard.Key key)
		{

		}
	}
}