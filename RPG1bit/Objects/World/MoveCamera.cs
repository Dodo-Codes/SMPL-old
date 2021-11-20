using SMPL.Components;
using SMPL.Data;
using SMPL.Gear;

namespace RPG1bit
{
	public class MoveCamera : Object
	{
		private readonly Point[] directions = new Point[] { new(), new(-1, 0), new(1, 0), new(0, -1), new(0, 1) };

		public MoveCamera(string uniqueID, CreationDetails creationDetails) : base(uniqueID, creationDetails)
		{
			Keyboard.Event.Subscribe.TextInput(uniqueID);
		}

		public override void OnKeyboardKeyPress(Keyboard.Key key)
		{
			if (TextInputField.Typing == false && World.CurrentSession != World.Session.None)
			{
				switch (key)
				{
					case Keyboard.Key.Space:
						{
							if (World.CurrentSession == World.Session.Single)
							{
								var player = ((Object)PickByUniqueID(nameof(Player)));
								World.CameraPosition = player.Position;
							}
							break;
						}
					case Keyboard.Key.ArrowLeft: World.CameraPosition += new Point(-1, 0); break;
					case Keyboard.Key.ArrowRight: World.CameraPosition += new Point(1, 0); break;
					case Keyboard.Key.ArrowDown: World.CameraPosition += new Point(0, 1); break;
					case Keyboard.Key.ArrowUp: World.CameraPosition += new Point(0, -1); break;
					default: return;
				}
				
				ChunkManager.UpdateChunks();
				Screen.ScheduleDisplay();
			}
		}

		public override void OnHovered()
		{
			var anchor = World.CurrentSession == World.Session.Single ? $"\nor [SPACE] to center the view" : "";
			NavigationPanel.Info.Textbox.Text = $"[ARROW KEYS] to move the view{anchor}";
		}
	}
}
