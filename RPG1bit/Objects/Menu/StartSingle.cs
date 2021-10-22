using SMPL.Data;
using SMPL.Gear;
using System;

namespace RPG1bit
{
	public class StartSingle : Object
	{
		public StartSingle(string uniqueID, CreationDetails creationDetails) : base(uniqueID, creationDetails) { }
		public override void OnHovered() => NavigationPanel.Info.Textbox.Text = "Start a new singleplayer game session.  ";
		public override void OnLeftClicked()
		{
			CreateTab();
			NavigationPanel.Tab.Open("single", "singleplayer");
			Screen.Display();
		}

		public override void OnMouseButtonRelease(Mouse.Button button)
		{
			base.OnMouseButtonRelease(button);
			if (Map.CurrentSession != Map.Session.Single || Map.IsHovered() == false) return;
			NavigationPanel.Tab.Close();
		}
		private static void CreateTab()
		{
			if (Gate.EnterOnceWhile("create-single-tab", true))
			{
				var mapList = new ObjectList("load-map-list", new CreationDetails()
				{
					Name = "load-map-list",
					Position = new(19, 4),
					TileIndexes = new Point[] { new Point(0, 0) },
					Height = 1,
					IsUI = true,
					IsLeftClickable = true,
					IsInTab = true,
					AppearOnTab = "single",
				}, new Size(13, 9));

				var maps = FileSystem.GetFileNames(false, "Maps");
				for (int i = 0; i < maps.Length; i++)
					mapList.Objects.Add(new StartSingleOnMap(maps[i], new CreationDetails()
					{
						Name = maps[i],
						Position = new(-10, 0) { C = new() },
						TileIndexes = new Point[] { new Point(32, 15) },
						Height = 1,
						IsUI = true,
						IsLeftClickable = true,
						IsInTab = true,
						AppearOnTab = "single",
					}));

				var noMapsStr = ObjectList.Lists.ContainsKey("load-map-list") == false ||
					ObjectList.Lists["load-map-list"].Objects.Count == 0 ? "No maps were found." : "";
				NavigationPanel.Tab.Texts["single"] =
					$" Choose a map to start a\nnew singleplayer session on.\n\n\n\n    {noMapsStr}";
			}
			if (Gate.EnterOnceWhile("create-item-info-tab", true))
			{
				new ItemStats("strength", new()
				{
					Position = new(19, 9),
					Height = 1,
					TileIndexes = new Point[] { new() },
					IsInTab = true,
					AppearOnTab = "item-info",
					IsUI = true,
					Name = "positives"
				});
				new ItemStats("weakness", new()
				{
					Position = new(19, 12),
					Height = 1,
					TileIndexes = new Point[] { new() },
					IsInTab = true,
					AppearOnTab = "item-info",
					IsUI = true,
					Name = "negatives"
				});
				new ItemSlotInfo("able-to-carry-in", new()
				{
					Position = new(31, 2),
					Height = 1,
					TileIndexes = new Point[] { new() },
					IsInTab = true,
					AppearOnTab = "item-info",
					IsUI = true,
					Name = "able-to-carry"
				});
			}
		}
	}
}
