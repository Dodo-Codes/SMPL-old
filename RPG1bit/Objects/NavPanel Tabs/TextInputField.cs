using SMPL.Data;
using SMPL.Gear;
using System.Collections.Generic;

namespace RPG1bit
{
	public class TextInputField : Object
	{
		public const int MAX_SYMBOLS = 12;

		public static bool Typing { get; set; }
		public string Value { get; set; } = "";
		public bool AcceptingInput { get; set; }

		private Point lastMousePos;

		public TextInputField(string uniqueID, CreationDetails creationDetails) : base(uniqueID, creationDetails)
		{
			Keyboard.Event.Subscribe.TextInput(uniqueID);
			Mouse.Event.Subscribe.ButtonPress(uniqueID);
			Game.Event.Subscribe.Update(uniqueID);
		}

		public override void OnGameUpdate()
		{
			var mousePos = Screen.GetCellAtCursorPosition();
			if (mousePos != lastMousePos && mousePos.X > Position.X - MAX_SYMBOLS - 1 &&
				mousePos.X < Position.X && mousePos.Y == Position.Y)
				OnHovered();
			lastMousePos = mousePos;
		}
		public override void OnMouseButtonPress(Mouse.Button button)
		{
			Typing = false;
			AcceptingInput = false;
			OnDisplay();
			var mousePos = Screen.GetCellAtCursorPosition();
			if (button != Mouse.Button.Left || mousePos.Y != Position.Y) return;
			for (int i = 0; i < MAX_SYMBOLS; i++)
			{
				var x = Position.X + i - MAX_SYMBOLS;
				if (x == mousePos.X)
				{
					AcceptingInput = true;
					Typing = true;
					OnDisplay();
					return;
				}
			}
		}
		public override void OnKeyboardTextInput(Keyboard.TextInput textInput)
		{
			if (AcceptingInput == false) return;
			switch (textInput.CurrentType)
			{
				case Keyboard.TextInput.Type.Text: break;
				case Keyboard.TextInput.Type.Backspace:
					{ Value = Value.Length == 0 ? Value : Value.Remove(Value.Length - 1); OnDisplay(); } return;
				case Keyboard.TextInput.Type.Enter:
					{
						OnSubmit();
						Typing = false;
						AcceptingInput = false;
						OnDisplay();
						return;
					}
				case Keyboard.TextInput.Type.Tab: return;
			}
			var isLetter = Text.IsLetters(textInput.Value);
			var isNumber = Text.IsNumber(textInput.Value);
			if (Value.Length == MAX_SYMBOLS || (isLetter == false && isNumber == false && textInput.Value != " ")) return;
			Value += textInput.Value.ToLower();
			OnDisplay();
			OnHovered();
		}
		public override void OnLeftClicked() => OnSubmit();
		public override void OnDisplay()
		{
			if (AppearOnTab != NavigationPanel.Tab.CurrentTabType) return;
			Screen.EditCell(Position, new(1, 22), 0, AcceptingInput ? Color.BrownLight : Color.Brown);
			for (int i = 0; i < MAX_SYMBOLS; i++)
			{
				var x = Position.X + i - MAX_SYMBOLS;
				Screen.EditCell(new Point(x, Position.Y), new(1, 22), 0, AcceptingInput ? Color.BrownLight : Color.Brown);
				if (Value.Length <= i) Screen.EditCell(new Point(x, Position.Y), new Point(13, 22), 1, Color.Gray);
			}
			Screen.DisplayText(Position - new Point(MAX_SYMBOLS, 0), 1, Color.White, Value);
		}

		protected virtual void OnSubmit() { }
	}
}
