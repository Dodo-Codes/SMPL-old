using SMPL.Data;
using SMPL.Gear;
using System;
using System.Collections.Generic;

namespace RPG1bit
{
	public class SaveNameInput : TextInputField
	{
		public SaveNameInput(string uniqueID, CreationDetails creationDetails) : base(uniqueID, creationDetails) { }
		public override void OnHovered()
		{
			NavigationPanel.Info.ShowLeftClickableIndicator();
			NavigationPanel.Info.ShowClickableIndicator();
			var mousePos = Screen.GetCellAtCursorPosition();
			var result = mousePos == Position ? "[ENTER] Save the current session." : "Type anything...";
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
				case Map.Session.Single:
					{
						var slot = new Assets.DataSlot($"Sessions\\{name}.session");
						slot.SetValue("player", Text.ToJSON((Player)PickByUniqueID("player")));
						slot.SetValue("map-name", Map.CurrentMapName);
						slot.Save();

						if (ObjectList.Lists.ContainsKey("load-list")) AddToList(ObjectList.Lists["load-list"]);

						break;
						void AddToList(ObjectList list)
						{
							if (IsOverwriting(list)) return;

							var value = (Object)new LoadSingleSessionValue(name, new CreationDetails()
							{
								Name = name,
								Position = new(-10, 0) { Color = new() },
								TileIndexes = new Point[] { new Point(14, 10) },
								Height = 1,
								IsUI = true,
								IsLeftClickable = true,
								IsRightClickable = true,
								IsInTab = true,
								AppearOnTab = NavigationPanel.Tab.Type.SaveLoad,
							});

							list.Objects.Add(value);
							list.ScrollToBottom();
						}
					}
				case Map.Session.Multi: break;
				case Map.Session.MapEdit:
					{
						var slot = new Assets.DataSlot($"Maps\\{name}.mapdata");
						var offset = new Point();
						var mapData = MapEditor.GetSavableMapData(Map.RawData, out offset);
						var signs = new List<Sign>();
						foreach (var kvp in objects)
							for (int i = 0; i < objects[kvp.Key].Count; i++)
								if (objects[kvp.Key][i] is Sign)
									signs.Add((Sign)objects[kvp.Key][i]);

						slot.SetValue("signs", Text.ToJSON(signs));
						slot.SetValue("camera-position", Text.ToJSON(Map.CameraPosition));
						slot.SetValue("map-offset", Text.ToJSON(offset));
						slot.SetValue("map-data", Text.ToJSON(mapData));
						slot.Save();

						if (ObjectList.Lists.ContainsKey("load-map-list")) AddToList(ObjectList.Lists["load-map-list"]);
						if (ObjectList.Lists.ContainsKey("load-list")) AddToList(ObjectList.Lists["load-list"]);

						break;

						void AddToList(ObjectList list)
						{
							if (IsOverwriting(list)) return;

							var value = (Object)new LoadMapValue(name, new CreationDetails()
							{
								Name = name,
								Position = new(-10, 0) { Color = new() },
								TileIndexes = new Point[] { new Point(47, 06) },
								Height = 1,
								IsUI = true,
								IsLeftClickable = true,
								IsRightClickable = true,
								IsInTab = true,
								AppearOnTab = NavigationPanel.Tab.Type.SaveLoad,
							});
							if (list.Name == "load-map-list")
							{
								value = new StartSingleOnMap(name, new CreationDetails()
								{
									Name = name,
									Position = new(-10, 0) { Color = new() },
									TileIndexes = new Point[] { new Point(32, 15) },
									Height = 1,
									IsUI = true,
									IsLeftClickable = true,
									IsInTab = true,
									AppearOnTab = NavigationPanel.Tab.Type.Single,
								});
							}

							list.Objects.Add(value);
							list.ScrollToBottom();
						}
					}
				bool IsOverwriting(ObjectList list)
					{
						for (int i = 0; i < list.Objects.Count; i++)
							if (list.Objects[i].Name == name)
								return true;
						return false;
					}
			}
			Value = "";
			SaveLoad.UpdateTab();
			Screen.Display();
		}
	}
}
