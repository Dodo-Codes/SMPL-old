using SMPL.Data;
using SMPL.Gear;

namespace RPG1bit
{
	public class SaveNameInput : TextInputField
	{
		public SaveNameInput(CreationDetails creationDetails) : base(creationDetails) { }
		public override void OnHovered()
		{
			NavigationPanel.Info.ShowLeftClickableIndicator();
			NavigationPanel.Info.ShowClickableIndicator();
			var mousePos = Screen.GetCellAtCursorPosition();
			var result = mousePos == Position ? "Save the current session." : "Type anything...";
			if (Map.CurrentSession == Map.Session.None) result = "There is no currently ongoing\n   session that can be saved.";
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
						var slot = new Assets.DataSlot($"Maps\\{name}.mapdata");
						slot.SetValue("camera-position", Text.ToJSON(Map.CameraPosition));
						slot.SetValue("map-data", Text.ToJSON(Map.Data));
						Assets.SaveDataSlots(slot);

						var objs = PickByPosition(new Point(19, 2));
						var list = default(ObjectList);
						var overwriting = false;
						for (int i = 0; i < objs.Count; i++)
							if (objs[i].Name == "load-list")
							{
								list = (ObjectList)objs[i];
								break;
							}
						for (int i = 0; i < list.Objects.Count; i++)
							if (list.Objects[i].Name == name)
							{
								overwriting = true;
								break;
							}
						if (overwriting) break;
						list.Objects.Add(new LoadMapValue(new CreationDetails()
						{
							Name = name,
							Position = new(-10, 0) { Color = new() },
							TileIndexes = new Point[] { new Point(1, 22) },
							Height = 1,
							IsUI = true,
							IsLeftClickable = true,
							IsRightClickable = true,
							IsInTab = true,
							AppearOnTab = NavigationPanel.Tab.Type.SaveLoad,
						}));
						list.ScrollToBottom();
						break;
					}
			}
			Value = "";
			OnDisplay();
			DisplayAllObjects();
		}
	}
}
