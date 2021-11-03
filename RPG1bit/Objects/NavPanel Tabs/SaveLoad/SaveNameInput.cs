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
			var mousePos = Screen.GetCellAtCursorPosition();
			var result = mousePos == Position ? "[ENTER] Save the current session." : "Type anything...";
			if (World.CurrentSession == World.Session.None) result = "There is no currently ongoing\n   session that can be saved.";
			else if (Value.Trim() == "") result = "Type a name before saving.";
			NavigationPanel.Info.Textbox.Text = result;
			NavigationPanel.Info.ShowLeftClickableIndicator();
		}
		protected override void OnSubmit()
		{
			var name = Value.Trim();
			if (name == "") return;
			switch (World.CurrentSession)
			{
				case World.Session.None: return;
				case World.Session.Single:
					{
						var slot = new Assets.DataSlot($"Sessions\\{name}.session");
						slot.SetValue(nameof(Player), Text.ToJSON(PickByUniqueID(nameof(Player))));

						SaveAll<Chest>(); SaveAll<ItemPile>(); SaveAll<Bag>(); SaveAll<Key>(); SaveAll<Quiver>(); SaveAll<Map>();

						slot.SetValue("world-name", World.CurrentWorldName);

						void SaveAll<T>() => slot.SetValue(typeof(T).Name, Text.ToJSON(PickByTag(typeof(T).Name)));

						slot.IsCompressed = true;
						slot.Save();

						if (ObjectList.Lists.ContainsKey("load-list")) AddToList(ObjectList.Lists["load-list"]);

						break;
						void AddToList(ObjectList list)
						{
							var uid = $"{name}-{Performance.FrameCount}-2";
							if (UniqueIDsExits(uid))
								return;
							var value = (Object)new LoadSingleSessionValue(uid, new CreationDetails()
							{
								Name = name,
								Position = new(-10, 0) { C = new() },
								TileIndexes = new Point[] { new Point(14, 10) },
								Height = 1,
								IsUI = true,
								IsLeftClickable = true,
								IsRightClickable = true,
								IsInTab = true,
								AppearOnTab = "save-load",
							});

							if (Contains(list.Objects, value) == false)
							{
								list.Objects.Add(value);
								list.ScrollToBottom();
							}
						}
					}
				case World.Session.Multi: break;
				case World.Session.WorldEdit:
					{
						var slot = new Assets.DataSlot($"Worlds\\{name}.worlddata");
						var signs = new List<CompactSignData>();
						foreach (var kvp in objects)
							for (int i = 0; i < objects[kvp.Key].Count; i++)
								if (objects[kvp.Key][i] is Sign sign)
									signs.Add(new()
									{
										P = new(sign.Position.X, sign.Position.Y) { C = new(sign.Height, 0, 0, 0)},
										I = sign.TileIndexes,
										T = sign.Text
									});

						slot.SetValue("signs", Text.ToJSON(signs));
						slot.SetValue("camera-position", Text.ToJSON(World.CameraPosition));
						//slot.SetValue("world-data", Text.ToJSON(World.GetSavableData()));
						slot.IsCompressed = true;
						slot.Save();

						if (ObjectList.Lists.ContainsKey("load-world-list")) AddToList(ObjectList.Lists["load-world-list"]);
						if (ObjectList.Lists.ContainsKey("load-list")) AddToList(ObjectList.Lists["load-list"]);

						break;

						void AddToList(ObjectList list)
						{
							var uid = $"{name}-{list.UniqueID}";
							if (UniqueIDsExits(uid))
								return;
							var value = (Object)new LoadWorldValue(uid, new CreationDetails()
							{
								Name = name,
								Position = new(-10, 0) { C = new() },
								TileIndexes = new Point[] { new Point(47, 06) },
								Height = 1,
								IsUI = true,
								IsLeftClickable = true,
								IsRightClickable = true,
								IsInTab = true,
								AppearOnTab = "save-load",
							});
							if (list.Name == "load-world-list")
							{
								var uid2 = $"{name}-{list.UniqueID}-1";
								if (UniqueIDsExits(uid2))
									return;
								value = new StartSingleOnWorld(uid2, new CreationDetails()
								{
									Name = name,
									Position = new(-10, 0) { C = new() },
									TileIndexes = new Point[] { new Point(32, 15) },
									Height = 1,
									IsUI = true,
									IsLeftClickable = true,
									IsInTab = true,
									AppearOnTab = "single",
								});
							}

							if (Contains(list.Objects, value) == false)
							{
								list.Objects.Add(value);
								list.ScrollToBottom();
							}
						}
					}
			}
			Value = "";
			StartSingle.UpdateTab();
			SaveLoad.UpdateTab();
			Screen.ScheduleDisplay();

			bool Contains(List<Object> list, Object obj)
			{
				for (int i = 0; i < list.Count; i++)
					if (list[i].Name == obj.Name && list[i].GetType() == obj.GetType())
						return true;
				return false;
			}
		}
	}
}
