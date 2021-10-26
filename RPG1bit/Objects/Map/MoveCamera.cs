using SMPL.Components;
using SMPL.Data;
using SMPL.Gear;

namespace RPG1bit
{
	public class MoveCamera : Object
	{
		public static bool IsAnchored { get; set; }
		public enum Type { Center, Left, Right, Up, Down }
		public Keyboard.Key Key { get; private set; }
		private Type currentType;
		public Type CurrentType
		{
			get { return currentType; }
			set
			{
				currentType = value;
				switch (CurrentType)
				{
					case Type.Center: Key = Keyboard.Key.Space; break;
					case Type.Left: Key = Keyboard.Key.A; break;
					case Type.Right: Key = Keyboard.Key.D; break;
					case Type.Up: Key = Keyboard.Key.W; break;
					case Type.Down: Key = Keyboard.Key.S; break;
				}
			}
		}
		private readonly Point[] directions = new Point[] { new(), new(-1, 0), new(1, 0), new(0, -1), new(0, 1) };

		public MoveCamera(string uniqueID, CreationDetails creationDetails) : base(uniqueID, creationDetails)
		{
			Keyboard.Event.Subscribe.TextInput(uniqueID);
			Keyboard.Event.Subscribe.KeyPress(uniqueID);
		}

		public override void OnKeyboardKeyPress(Keyboard.Key key)
		{
			if (TextInputField.Typing || Map.CurrentSession != Map.Session.Single ||
				key != Keyboard.Key.Space) return;
			Execute();
		}
		public override void OnKeyboardTextInput(Keyboard.TextInput textInput)
		{
			if (TextInputField.Typing || Map.CurrentSession == Map.Session.None ||
				textInput.Value.ToLower() != Key.ToString().ToLower()) return;
			Execute();
		}

		public override void OnHovered()
		{
			NavigationPanel.Info.Textbox.Text = $" [{Key}] Move the view: {CurrentType}.";
			if (CurrentType == Type.Center)
			{
				var anchorStr = IsAnchored ? "detach it from" : "attach it to";
				NavigationPanel.Info.Textbox.Text = $" [{Key.ToString().ToUpper()}] Center the view and\n  {anchorStr} the character.";
			}
		}
		public override void OnLeftClicked()
		{
			Execute();
			OnHovered();
		}

		private void Execute()
		{
			Map.CameraPosition += directions[(int)CurrentType];
			var player = ((Object)PickByUniqueID("player"));
			if (CurrentType == Type.Center && player != null)
			{
				IsAnchored = !IsAnchored;
				Map.CameraPosition = player.Position;
			}
			Screen.Display();
		}
	}
}
