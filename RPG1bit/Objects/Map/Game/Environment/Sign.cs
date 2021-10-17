using SMPL.Gear;
using SMPL.Data;
using SMPL.Components;
using Newtonsoft.Json;

namespace RPG1bit
{
	public struct CompactSignData
	{
		public Point P { get; set; }
		public Point I { get; set; }
		public string T { get; set; }
	}

	public class Sign : Object
	{
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
			var mousePos = Map.ScreenToMapPosition(Screen.GetCellAtCursorPosition());
			if (Map.CurrentSession != Map.Session.MapEdit || mousePos != Position) return;

			switch (textInput.CurrentType)
			{
				case Keyboard.TextInput.Type.Text: Text += textInput.Value[0] < 255 ? textInput.Value : ""; break;
				case Keyboard.TextInput.Type.Backspace: Text = Text.Length > 0 ? Text.Remove(Text.Length - 1) : ""; break;
				case Keyboard.TextInput.Type.Enter: Text += "\n"; break;
				case Keyboard.TextInput.Type.Tab: break;
			}
			OnHovered();
		}
	}
}
