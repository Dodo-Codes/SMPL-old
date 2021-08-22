using SMPL;

namespace TestGame
{
	public class Player
	{
		ComponentSprite ComponentSprite;

		public Player()
		{
			Keyboard.CallWhen.KeyPress(OnKeyPress);
			Keyboard.CallWhen.KeyRelease(OnKeyRelease);
			File.LoadAsset(File.Asset.Texture, "big.jpg");
		}
		void OnKeyPress(Keyboard.Key key)
		{
			ComponentSprite = new ComponentSprite(new Component2D(), "big.jpg");
		}
		void OnKeyRelease(Keyboard.Key key)
		{
			ComponentSprite.AllAccess = ComponentAccess.Access.Removed;
		}
	}
}