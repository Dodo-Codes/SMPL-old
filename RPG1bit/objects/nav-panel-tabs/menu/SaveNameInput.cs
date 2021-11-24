using SMPL.Data;
using SMPL.Gear;
using System;
using System.Collections.Generic;
using System.IO;

namespace RPG1bit
{
	public class SaveNameInput : TextInputField
	{
		public SaveNameInput(string uniqueID, CreationDetails creationDetails) : base(uniqueID, creationDetails) { }
		public override void OnHovered()
		{
			var mousePos = Screen.GetCellAtCursorPosition();
			var result = mousePos == Position ? "[ENTER] Save the current session" : "Type anything...";
			if (World.CurrentSession == World.Session.None) result = "There is no currently ongoing\n   session that can be saved";
			else if (Value.Trim() == "") result = "Type a name before saving";
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
						var slot = new Assets.DataSlot($"sessions\\{name}.session")
						{
							ThingUniqueIDs = GetObjects<ISavable>(),
							IsCompressed = true
						};
						slot.SetValue(nameof(Player), Text.ToJSON(PickByUniqueID(nameof(Player))));
						slot.SetValue("world-name", World.CurrentWorldName);
						slot.Save();

						if (GameObjectList.Lists.ContainsKey("load-list")) AddToList(GameObjectList.Lists["load-list"]);

						break;
						void AddToList(GameObjectList list)
						{
							var uid = $"{name}-{Performance.FrameCount}-2";
							if (UniqueIDsExists(uid))
								return;
							var value = (GameObject)new LoadSingleSessionValue(uid, new CreationDetails()
							{
								Name = name,
								Position = new(-20, 0) { Color = new() },
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
						if (Directory.Exists($"worlds\\{name}"))
						{
							var worlds = Directory.GetFiles($"worlds\\{name}");
							for (int i = 0; i < worlds.Length; i++)
								File.Delete(worlds[i]);
						}

						Directory.CreateDirectory($"worlds\\{name}");
						var chunkNames = Directory.GetFiles("cache");
						for (int i = 0; i < chunkNames.Length; i++)
							File.Copy(chunkNames[i], $"worlds\\{name}\\{Path.GetFileName(chunkNames[i])}");

						var slot = new Assets.DataSlot($"worlds\\{name}\\{name}.worlddata");
						slot.SetValue("world-name", name);
						slot.SetValue("camera-position", Text.ToJSON(World.CameraPosition));
						slot.IsCompressed = true;
						slot.Save();

						if (GameObjectList.Lists.ContainsKey("load-world-list")) AddToList(GameObjectList.Lists["load-world-list"]);
						if (GameObjectList.Lists.ContainsKey("load-list")) AddToList(GameObjectList.Lists["load-list"]);

						break;

						void AddToList(GameObjectList list)
						{
							var uid = $"{name}-{list.UniqueID}";
							if (UniqueIDsExists(uid))
								return;
							var value = (GameObject)new LoadWorldValue(uid, new CreationDetails()
							{
								Name = name,
								Position = new(-20, 0),
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
								if (UniqueIDsExists(uid2))
									return;
								value = new StartSingleOnWorld(uid2, new CreationDetails()
								{
									Name = name,
									Position = new(-20, 0),
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

			bool Contains(List<GameObject> list, GameObject obj)
			{
				for (int i = 0; i < list.Count; i++)
					if (list[i].Name == obj.Name && list[i].GetType() == obj.GetType())
						return true;
				return false;
			}
		}
	}
}
