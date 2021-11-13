﻿using SMPL.Data;
using SMPL.Gear;
using System;
using System.IO;

namespace RPG1bit
{
	public class StartSingle : Object
	{
		public StartSingle(string uniqueID, CreationDetails creationDetails) : base(uniqueID, creationDetails)
		{
			UpdateTab();
		}
		public override void OnHovered() => NavigationPanel.Info.Textbox.Text = "[LEFT CLICK] Start a new\nsingleplayer game session";
		public override void OnLeftClicked() => UpdateTab();
		public static void UpdateTab()
		{
			CreateTab();

			var noWorldsStr = ObjectList.Lists.ContainsKey("load-world-list") == false ||
			ObjectList.Lists["load-world-list"].Objects.Count == 0 ? "No worlds were found" : "";
			NavigationPanel.Tab.Texts["single"] =
				$" Choose a world to start a\nnew singleplayer session on\n\n\n\n    {noWorldsStr}";
			NavigationPanel.Tab.Open("single", "singleplayer");
		}

		private static void CreateTab()
		{
			if (Gate.EnterOnceWhile("create-single-tab"))
			{
				var worldList = new ObjectList("load-world-list", new CreationDetails()
				{
					Name = "load-world-list",
					Position = new(19, 4),
					TileIndexes = new Point[] { new Point(0, 0) },
					Height = 1,
					IsUI = true,
					IsLeftClickable = true,
					IsInTab = true,
					AppearOnTab = "single",
					IsKeptBetweenSessions = true,
				}, new Size(13, 9));

				var worlds = Directory.GetDirectories("worlds");
				for (int i = 0; i < worlds.Length; i++)
					worldList.Objects.Add(new StartSingleOnWorld($"worldp--{worlds[i]}", new CreationDetails()
					{
						Name = worlds[i].Replace("worlds\\", ""),
						Position = new(-10, 0) { C = new() },
						TileIndexes = new Point[] { new Point(32, 15) },
						Height = 1,
						IsUI = true,
						IsLeftClickable = true,
						IsInTab = true,
						AppearOnTab = "single",
						IsKeptBetweenSessions = true,
					}));
			}
		}
	}
}
