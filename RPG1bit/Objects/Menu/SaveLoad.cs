using SMPL.Gear;
using SMPL.Data;

namespace RPG1bit
{
	public class SaveLoad : Object
	{
		public SaveLoad(CreationDetails creationDetails) : base(creationDetails) { }
		public override void OnHovered() => NavigationPanel.Info.Textbox.Text = "Save/Load a session.";
		public override void OnLeftClicked()
		{
			var noSessionsStr = ObjectList.Lists.ContainsKey("load-list") == false ||
				ObjectList.Lists["load-list"].Objects.Count == 0 ? "No saved sessions were found." : "";
			NavigationPanel.Tab.Open(NavigationPanel.Tab.Type.SaveLoad, "save or load");
			NavigationPanel.Tab.Textbox.Text = $"Load a previously saved session.\n\n\n\n {noSessionsStr}\n\n\n\n\n\n\n\n" +
				$"    Save the current session.";

			CreateTab();
			Screen.Display();
		}
		private static void CreateTab()
		{
			if (Gate.EnterOnceWhile("create-save-load-tab", true))
			{
				new SaveNameInput(new CreationDetails()
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
				var mapList = new ObjectList(new CreationDetails()
				{
					Name = "load-list",
					Position = new(19, 3),
					TileIndexes = new Point[] { new Point(0, 0) },
					Height = 1,
					IsUI = true,
					IsLeftClickable = true,
					IsInTab = true,
					AppearOnTab = NavigationPanel.Tab.Type.SaveLoad,
				}, new Size(12, 8));

				var maps = FileSystem.GetFileNames(false, "Maps");
				for (int i = 0; i < maps.Length; i++)
					mapList.Objects.Add(new LoadMapValue(new CreationDetails()
					{
						Name = maps[i],
						Position = new(-10, 0) { Color = new() },
						TileIndexes = new Point[] { new Point(1, 22) },
						Height = 1,
						IsUI = true,
						IsLeftClickable = true,
						IsRightClickable = true,
						IsInTab = true,
						AppearOnTab = NavigationPanel.Tab.Type.SaveLoad,
					}));
			}
		}
	}
}
