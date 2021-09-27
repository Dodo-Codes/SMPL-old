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
						var slot = new Assets.DataSlot($"Maps\\{name}.map");
						slot.SetValue("camera-position", Text.ToJSON(Map.CameraPosition));
						slot.SetValue("map-data", Text.ToJSON(Map.Data));
						Assets.SaveDataSlots(slot);
						break;
					}
			}
			Value = "";
			OnDisplay();
		}
	}
}
