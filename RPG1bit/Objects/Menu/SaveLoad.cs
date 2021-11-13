using SMPL.Gear;
using SMPL.Data;
using System.IO;

namespace RPG1bit
{
	public class SaveLoad : Object
	{
		public SaveLoad(string uniqueID, CreationDetails creationDetails) : base(uniqueID, creationDetails) { }
		public override void OnHovered() => NavigationPanel.Info.Textbox.Text = "[LEFT CLICK] to save or load a session";
		public override void OnLeftClicked()
		{
			UpdateTab();

			if (World.CurrentSession == World.Session.WorldEdit)
			{
				var chunks = PickByTag(nameof(Chunk));
				foreach (Chunk chunk in chunks)
					ChunkManager.SaveChunk(chunk, false);
			}
		}
		public static void UpdateTab()
		{
			CreateTab();

			var noSessionsStr = ObjectList.Lists.ContainsKey("load-list") == false ||
			ObjectList.Lists["load-list"].Objects.Count == 0 ? "No saved sessions were found" : "";
			NavigationPanel.Tab.Texts["save-load"] =
				$"Load a previously saved session\n\n\n\n {noSessionsStr}\n\n\n\n\n\n\n\n" +
				$"    Save the current session";

			NavigationPanel.Tab.Open("save-load", "save or load");
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
					AppearOnTab = "save-load",
					IsKeptBetweenSessions = true,
				});
				var worldList = new ObjectList("load-list", new CreationDetails()
				{
					Name = "load-list",
					Position = new(19, 3),
					TileIndexes = new Point[] { new Point(0, 0) },
					Height = 1,
					IsUI = true,
					IsLeftClickable = true,
					IsInTab = true,
					AppearOnTab = "save-load",
					IsKeptBetweenSessions = true,
				}, new Size(13, 8));

				var worlds = Directory.GetDirectories("worlds");
				var sessions = Directory.GetFiles("sessions");
				for (int i = 0; i < worlds.Length; i++)
					worldList.Objects.Add(new LoadWorldValue($"world--{worlds[i]}", new CreationDetails()
					{
						Name = worlds[i].Replace("worlds\\", ""),
						Position = new(-20, 0),
						TileIndexes = new Point[] { new Point(47, 06) },
						Height = 1,
						IsUI = true,
						IsLeftClickable = true,
						IsRightClickable = true,
						IsInTab = true,
						AppearOnTab = "save-load",
						IsKeptBetweenSessions = true,
					}));
				for (int i = 0; i < sessions.Length; i++)
					worldList.Objects.Add(new LoadSingleSessionValue($"session--{sessions[i]}", new CreationDetails()
					{
						Name = sessions[i].Replace("sessions\\", "").Replace(".session", ""),
						Position = new(-20, 0),
						TileIndexes = new Point[] { new Point(14, 10) },
						Height = 1,
						IsUI = true,
						IsLeftClickable = true,
						IsRightClickable = true,
						IsInTab = true,
						AppearOnTab = "save-load",
						IsKeptBetweenSessions = true,
					}));
			}
		}
	}
}
