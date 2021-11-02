using SMPL.Gear;
using SMPL.Data;
using SMPL.Components;
using Newtonsoft.Json;

namespace RPG1bit
{
	public struct CompactSignData
	{
		public Point P { get; set; } // pos
		public Point I { get; set; } // tile index values
		public string T { get; set; } // text
	}

	public class Sign : Object
	{
		public const int CHAR_LIMIT = 400;
		[JsonProperty]
		public string Text { get; set; } = "\"Type anything while hovering me...\"";

		public Sign(string uniqueID, CreationDetails creationDetails) : base(uniqueID, creationDetails)
		{
			Keyboard.Event.Subscribe.TextInput(uniqueID);
		}
		public override void OnHovered()
		{
			TextInputField.Typing = true;
			NavigationPanel.Info.Textbox.Text = Text;
		}
		public override void OnUnhovered()
		{
			TextInputField.Typing = false;
		}
		public override void OnKeyboardTextInput(Keyboard.TextInput textInput)
		{
			var mousePos = World.ScreenToWorldPosition(Screen.GetCellAtCursorPosition());
			if (World.CurrentSession != World.Session.WorldEdit || mousePos != Position) return;

			switch (textInput.CurrentType)
			{
				case Keyboard.TextInput.Type.Text:
					Text += textInput.Value[0] < 255 && Text.Length < CHAR_LIMIT ? textInput.Value : ""; break;
				case Keyboard.TextInput.Type.Backspace: Text = Text.Length > 0 ? Text.Remove(Text.Length - 1) : ""; break;
				case Keyboard.TextInput.Type.Enter: Text += Text.Length < CHAR_LIMIT ? "\n" : ""; break;
				case Keyboard.TextInput.Type.Tab: break;
			}
			OnHovered();
		}
	}
}
