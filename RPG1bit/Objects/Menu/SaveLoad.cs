using SMPL.Gear;
using SMPL.Data;

namespace RPG1bit
{
	public class SaveLoad : Object
	{
		public SaveLoad(string uniqueID, CreationDetails creationDetails) : base(uniqueID, creationDetails) { }
		public override void OnHovered() => NavigationPanel.Info.Textbox.Text = "Save/Load a session.";
		public override void OnLeftClicked()
		{
			UpdateTab();
		}
		public static void UpdateTab()
		{
			CreateTab();
			NavigationPanel.Tab.Open(NavigationPanel.Tab.Type.SaveLoad, "save or load");
			Screen.Display();
		}

		private static void CreateTab()
		{
			if (Gate.EnterOnceWhile("create-save-load-tab", true))
			{
				new SaveNameInput("save-session", new CreationDetails()
				{
					Name = "save-session",
					Position = new(31, 13),
					TileIndexes = new Point[] { new Point(14, 22) },
					Height = 1,
					IsUI = true,
					IsLeftClickable = true,
					IsInTab = true,
					AppearOnTab = NavigationPanel.Tab.Type.SaveLoad,
				});
				var mapList = new ObjectList("load-list", new CreationDetails()
				{
					Name = "load-list",
					Position = new(19, 3),
					TileIndexes = new Point[] { new Point(0, 0) },
					Height = 1,
					IsUI = true,
					IsLeftClickable = true,
					IsInTab = true,
					AppearOnTab = NavigationPanel.Tab.Type.SaveLoad,
				}, new Size(13, 8));

				var maps = FileSystem.GetFileNames(false, "Maps");
				var sessions = FileSystem.GetFileNames(false, "Sessions");
				for (int i = 0; i < maps.Length; i++)
					mapList.Objects.Add(new LoadMapValue(maps[i], new CreationDetails()
					{
						Name = maps[i],
						Position = new(-10, 0) { C = new() },
						TileIndexes = new Point[] { new Point(47, 06) },
						Height = 1,
						IsUI = true,
						IsLeftClickable = true,
						IsRightClickable = true,
						IsInTab = true,
						AppearOnTab = NavigationPanel.Tab.Type.SaveLoad,
					}));
				for (int i = 0; i < sessions.Length; i++)
					mapList.Objects.Add(new LoadSingleSessionValue(sessions[i], new CreationDetails()
					{
						Name = sessions[i],
						Position = new(-10, 0) { C = new() },
						TileIndexes = new Point[] { new Point(14, 10) },
						Height = 1,
						IsUI = true,
						IsLeftClickable = true,
						IsRightClickable = true,
						IsInTab = true,
						AppearOnTab = NavigationPanel.Tab.Type.SaveLoad,
					}));

				var noSessionsStr = ObjectList.Lists.ContainsKey("load-list") == false ||
					ObjectList.Lists["load-list"].Objects.Count == 0 ? "No saved sessions were found." : "";
				NavigationPanel.Tab.Texts[NavigationPanel.Tab.Type.SaveLoad] =
					$"Load a previously saved session.\n\n\n\n {noSessionsStr}\n\n\n\n\n\n\n\n" +
					$"    Save the current session.";
			}
		}
	}
}
