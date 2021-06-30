namespace TestGame
{
	class KeyboardInput : SMPL.Keyboard
	{
		public override void OnTextInput(string textSymbol, bool isBackspace, bool isEnter, bool isTab)
		{
			SMPL.Console.Log(textSymbol, false);
			if (isTab)
			{
				SMPL.Console.Clear();
			}
		}
	}
}
