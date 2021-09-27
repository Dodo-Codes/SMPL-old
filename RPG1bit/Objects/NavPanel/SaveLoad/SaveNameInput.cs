using SMPL.Data;
using SMPL.Gear;

namespace RPG1bit
{
	public class SaveNameInput : TextInputField
	{
		public SaveNameInput(CreationDetails creationDetails) : base(creationDetails) { }
		protected override void OnHovered()
		{
			var result = "Save the current session.";
			if (Map.CurrentSession == Map.Session.None) result = "There is no session that is\n    currently ongoing.";
			else if (Value.Trim() == "") result = "Type a name before saving.";
			NavigationPanel.Info.Textbox.Text = result;
		}
		protected override void OnSubmit()
		{
			var name = Value.Trim();
			if (name == "") return;
			switch (Map.CurrentSession)
			{
				case Map.Session.None: return;
				case Map.Session.Single: break;
				case Map.Session.Multi: break;
				case Map.Session.MapEdit:
					{
						var slot = new SaveSlot() { Values = new string[] { Text.ToJSON(Map.CameraPosition), Text.ToJSON(Map.Data) } };
						slot.Save(name);
						break;
					}
			}
			Value = "";
			OnDisplay();
		}
	}
}
