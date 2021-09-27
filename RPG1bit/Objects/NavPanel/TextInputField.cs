using SMPL.Data;
using SMPL.Gear;
using System.Collections.Generic;

namespace RPG1bit
{
	public class TextInputField : Object
	{
		public string Value { get; set; } = "";

		public TextInputField(CreationDetails creationDetails) : base(creationDetails)
		{
			Keyboard.CallWhen.TextInput(OnTextInput);
		}

		private void OnTextInput(Keyboard.TextInput textInput)
		{
			if (AppearOnTab != NavigationPanel.Tab.CurrentTabType) return;
			switch (textInput.CurrentType)
			{
				case Keyboard.TextInput.Type.Text: break;
				case Keyboard.TextInput.Type.Backspace:
					{ Value = Value.Length == 0 ? Value : Value.Remove(Value.Length - 1); OnDisplay(); } return;
				case Keyboard.TextInput.Type.Enter: OnSubmit(); return;
				case Keyboard.TextInput.Type.Tab: return;
			}
			var isLetter = Text.IsLetters(textInput.Value);
			var isNumber = Text.IsNumber(textInput.Value);
			if (Value.Length == 10 || (isLetter == false && isNumber == false && textInput.Value != " ")) return;
			Value += textInput.Value.ToLower();
			OnDisplay();
			OnHovered();
		}
		protected override void OnLeftClicked() => OnSubmit();
		protected override void OnDisplay()
		{
			for (int i = 0; i < 10; i++)
			{
				var x = Position.X + i - 11;
				if (Value.Length <= i) Screen.EditCell(new Point(x, Position.Y), new Point(13, 22), 1, Color.Gray);
				else
				{
					var isNumber = Text.IsNumber(Value[i].ToString());
					var curX = 35 + Value[i] - (isNumber ? 48 : 97);
					var tileIndexes = new Point(curX, isNumber ? 17 : 18);
					if (curX > 47) tileIndexes += new Point(-13, 1);
					if (tileIndexes.X == -30) tileIndexes = new(0, 0);
					Screen.EditCell(new Point(x, Position.Y), tileIndexes, 1, Color.White);
				}
			}
		}

		protected virtual void OnSubmit() { }
	}
}
