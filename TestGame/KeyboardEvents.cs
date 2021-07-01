namespace TestGame
{
	class KeyboardEvents : SMPL.KeyboardEvents
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
