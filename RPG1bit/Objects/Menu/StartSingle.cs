﻿using SMPL.Data;
using SMPL.Gear;
using System;

namespace RPG1bit
{
	public class StartSingle : Object
	{
		public StartSingle(CreationDetails creationDetails) : base(creationDetails) { }
		public override void OnHovered() => NavigationPanel.Info.Textbox.Text = "Start a new singleplayer game session.  ";
		public override void OnLeftClicked()
		{
			var noMapsStr = ObjectList.Lists.ContainsKey("load-map-list") == false ||
				ObjectList.Lists["load-map-list"].Objects.Count == 0 ? "No maps were found." : "";
			NavigationPanel.Tab.Open(NavigationPanel.Tab.Type.Single, "singleplayer");
			NavigationPanel.Tab.Textbox.Text = $" Choose a map to start a\nnew singleplayer session on.\n\n\n\n    {noMapsStr}";

			CreateTab();
			Screen.Display();
		}
		private static void CreateTab()
		{
			if (Gate.EnterOnceWhile("create-single-tab", true))
			{
				var mapList = new ObjectList(new CreationDetails()
				{
					Name = "load-map-list",
					Position = new(19, 4),
					TileIndexes = new Point[] { new Point(0, 0) },
					Height = 1,
					IsUI = true,
					IsLeftClickable = true,
					IsInTab = true,
					AppearOnTab = NavigationPanel.Tab.Type.Single,
				}, new Size(12, 9));

				var maps = FileSystem.GetFileNames(false, "Maps");
				for (int i = 0; i < maps.Length; i++)
					mapList.Objects.Add(new StartSingleOnMap(new CreationDetails()
					{
						Name = maps[i],
						Position = new(-10, 0) { Color = new() },
						TileIndexes = new Point[] { new Point(1, 22) },
						Height = 1,
						IsUI = true,
						IsLeftClickable = true,
						IsInTab = true,
						AppearOnTab = NavigationPanel.Tab.Type.Single,
					}));
			}
		}
		private void Execute()
		{
			if (Map.CurrentSession != Map.Session.Single) Map.DestroyAllSessionObjects();
			Map.CurrentSession = Map.Session.Single;
			Map.DisplayNavigationPanel();

			Map.CreateUIButtons();
			Map.Display(); // for the map itself
			DisplayAllObjects(); // for the map ui
		}
	}
}
