﻿using Newtonsoft.Json;
using SMPL.Data;
using SMPL.Gear;
using System.Collections.Generic;

namespace RPG1bit
{
	public static class Map
	{
		public enum Session { None, Single, Multi, MapEdit }

		public static Size Size => new(1000, 1000);
		public static Point[,,] RawData { get; set; } = DefaultRawData;
		public static Point[,,] DefaultRawData => new Point[(int)Size.W, (int)Size.H, 4];

		private static Point cameraPosition = new(Size.W / 2, Size.H / 2);
		public static Point CameraPosition
		{
			get { return cameraPosition; }
			set
			{
				cameraPosition = new Point(
					Number.Limit(value.X, new Number.Range(8, RawData.GetLength(1) - 9)),
					Number.Limit(value.Y, new Number.Range(8, RawData.GetLength(0) - 9)));
			}
		}
		public static Session CurrentSession { get; set; }

		public static void Initialize()
		{
			Assets.CallWhen.LoadEnd(OnLoadEnd);
		}
		private static void OnLoadEnd()
		{
			if (Assets.ValuesAreLoaded("camera-position", "map-data", "map-offset"))
			{
				var cameraPos = Text.FromJSON<Point>(Assets.GetValue("camera-position"));
				var mapData = Text.FromJSON<Point[,,]>(Assets.GetValue("map-data"));
				var mapOffset = Text.FromJSON<Point>(Assets.GetValue("map-offset"));
				if (mapData != default) InsertMapData(mapData, mapOffset);
				if (cameraPos != default) CameraPosition = cameraPos;

				Display(); // for the map iteself
				Object.DisplayAllObjects(); // for the ui
			}
		}

		public static void CreateUIButtons()
		{
			// these are not creating multiple times cuz they are destroyed beforehand
			new MoveCamera(new Object.CreationDetails()
			{
				Name = "camera-move-up",
				Position = new(0, 1) { Color = Color.Gray },
				TileIndexes = new Point[] { new(19, 20) },
				Height = 1,
				IsUI = true,
				IsLeftClickable = true,
			}) { CurrentType = MoveCamera.Type.Up };
			new MoveCamera(new Object.CreationDetails()
			{
				Name = "camera-move-down",
				Position = new(0, 2) { Color = Color.Gray },
				TileIndexes = new Point[] { new(21, 20) },
				Height = 1,
				IsUI = true,
				IsLeftClickable = true,
			}) { CurrentType = MoveCamera.Type.Down };
			new MoveCamera(new Object.CreationDetails()
			{
				Name = "camera-move-left",
				Position = new(1, 0) { Color = Color.Gray },
				TileIndexes = new Point[] { new(22, 20) },
				Height = 1,
				IsUI = true,
				IsLeftClickable = true,
			}) { CurrentType = MoveCamera.Type.Left };
			new MoveCamera(new Object.CreationDetails()
			{
				Name = "camera-move-right",
				Position = new(2, 0) { Color = Color.Gray },
				TileIndexes = new Point[] { new(20, 20) },
				Height = 1,
				IsUI = true,
				IsLeftClickable = true,
			}) { CurrentType = MoveCamera.Type.Right };
			new MoveCamera(new Object.CreationDetails()
			{
				Name = "camera-center",
				Position = new(0, 0) { Color = Color.Gray },
				TileIndexes = new Point[] { new(19, 14) },
				Height = 1,
				IsUI = true,
				IsLeftClickable = true,
			}) { CurrentType = MoveCamera.Type.Center };

			if (Gate.EnterOnceWhile("game-buttons", CurrentSession == Session.Single || CurrentSession == Session.Multi))
			{
				new SlotHead(new Object.CreationDetails()
				{
					Name = "head",
					Position = new(0, 7) { Color = Color.Gray },
					TileIndexes = new Point[] { new(5, 22) },
					Height = 1,
					IsUI = true
				});
				new SlotBody(new Object.CreationDetails()
				{
					Name = "body",
					Position = new(0, 8) { Color = Color.Gray },
					TileIndexes = new Point[] { new(6, 22) },
					Height = 1,
					IsUI = true
				});
				new SlotFeet(new Object.CreationDetails()
				{
					Name = "feet",
					Position = new(0, 9) { Color = Color.Gray },
					TileIndexes = new Point[] { new(7, 22) },
					Height = 1,
					IsUI = true
				});

				new SlotHandLeft(new Object.CreationDetails()
				{
					Name = "hand-left",
					Position = new(0, 5) { Color = Color.Gray },
					TileIndexes = new Point[] { new(8, 22) },
					Height = 1,
					IsUI = true
				});
				new SlotHandRight(new Object.CreationDetails()
				{
					Name = "hand-right",
					Position = new(0, 4) { Color = Color.Gray },
					TileIndexes = new Point[] { new(9, 22) },
					Height = 1,
					IsUI = true
				});

				new SlotBack(new Object.CreationDetails()
				{
					Name = "carry-back",
					Position = new(0, 11) { Color = Color.Gray },
					TileIndexes = new Point[] { new(10, 22) },
					Height = 1,
					IsUI = true
				});
				new SlotWaist(new Object.CreationDetails()
				{
					Name = "carry-waist",
					Position = new(0, 12) { Color = Color.Gray },
					TileIndexes = new Point[] { new(11, 22) },
					Height = 1,
					IsUI = true
				});
			}
			if (Gate.EnterOnceWhile("map-editor-buttons", CurrentSession == Session.MapEdit))
			{
				new SwitchType(new Object.CreationDetails()
				{
					Name = "tile-up",
					Position = new(0, 6) { Color = Color.Gray },
					TileIndexes = new Point[] { new(23, 20) },
					Height = 1,
					IsUI = true,
					IsLeftClickable = true,
				});
				new SwitchType(new Object.CreationDetails()
				{
					Name = "tile-down",
					Position = new(0, 8) { Color = Color.Gray },
					TileIndexes = new Point[] { new(25, 20) },
					Height = 1,
					IsUI = true,
					IsLeftClickable = true,
				});
				new SwitchColor(new Object.CreationDetails()
				{
					Name = "color-up",
					Position = new(0, 10) { Color = Color.Gray },
					TileIndexes = new Point[] { new(23, 20) },
					Height = 1,
					IsUI = true,
					IsLeftClickable = true,
				});
				new SwitchColor(new Object.CreationDetails()
				{
					Name = "color-down",
					Position = new(0, 12) { Color = Color.Gray },
					TileIndexes = new Point[] { new(25, 20) },
					Height = 1,
					IsUI = true,
					IsLeftClickable = true,
				});
				new SwitchHeight(new Object.CreationDetails()
				{
					Name = "height-up",
					Position = new(0, 14) { Color = Color.Gray },
					TileIndexes = new Point[] { new(23, 20) },
					Height = 1,
					IsUI = true,
					IsLeftClickable = true,
				});
				new SwitchHeight(new Object.CreationDetails()
				{
					Name = "height-down",
					Position = new(0, 17) { Color = Color.Gray },
					TileIndexes = new Point[] { new(25, 20) },
					Height = 1,
					IsUI = true,
					IsLeftClickable = true,
				});
			}
		}
		public static void DestroyAllSessionObjects()
		{
			if (CurrentSession == Session.None) return;

			var objsToDestroy = new List<Object>();
			foreach (var kvp in Object.objects)
				for (int i = 0; i < kvp.Value.Count; i++)
				{
					if (kvp.Value[i].IsUI && kvp.Value[i].Position.X > 18) continue;
					objsToDestroy.Add(kvp.Value[i]);
				}

			for (int i = 0; i < objsToDestroy.Count; i++)
				objsToDestroy[i].Destroy();
		}
		
		public static void Display()
		{
			for (int y = (int)CameraPosition.Y - 8; y < CameraPosition.Y + 9; y++)
				for (int x = (int)CameraPosition.X - 8; x < CameraPosition.X + 9; x++)
				{
					var scrPos = MapToScreenPosition(new(x, y));
					for (int z = 0; z < 4; z++)
						Screen.EditCell(scrPos, RawData[x, y, z], z, RawData[x, y, z].Color);
				}
		}
		public static void DisplayNavigationPanel()
		{
			for (int x = 0; x < 18; x++)
			{
				Screen.EditCell(new(x, 0), new(4, 22), 1, Color.Brown);
				Screen.EditCell(new(x, 0), new(1, 22), 0, Color.BrownDark);
			}
			for (int y = 0; y < 18; y++)
			{
				Screen.EditCell(new(0, y), new(4, 22), 1, Color.Brown);
				Screen.EditCell(new(0, y), new(1, 22), 0, Color.BrownDark);
			}
			if (CurrentSession == Session.MapEdit)
			{
				Screen.EditCell(new(7, 0), new(25, 0), 1, Color.Gray);

				Screen.EditCell(new(0, 4), new(5, 0), 1, Color.White);
				Screen.EditCell(new(0, 7), new(41, 19), 1, Color.Gray);
				Screen.EditCell(new(0, 11), new(37, 18), 1, Color.Gray);
				Screen.EditCell(new(0, 15), new(42, 18), 1, Color.Gray);

				Screen.EditCell(new(9, 0), new(41, 13), 1, Color.Gray);
				Screen.EditCell(new(10, 0), new(43, 13), 1, Color.Gray);
				Screen.EditCell(new(11, 0), new(42, 13), 1, Color.Gray);

				Screen.EditCell(new(0, 16), new(36 + SwitchHeight.BrushHeight, 17), 1, Color.Gray);
				Screen.EditCell(new(0, 4), SwitchType.BrushType, 1, SwitchColor.BrushColor);
			}
		}
		public static void LoadMap(Session session, string name)
		{
			if (Assets.ValuesAreLoaded("map-data", "camera-position")) Assets.UnloadValues("map-data", "camera-position");
			Assets.Load(Assets.Type.DataSlot, $"Maps\\{name}.mapdata");
			NavigationPanel.Tab.Close();
			NavigationPanel.Info.Textbox.Text = Object.descriptions[new(0, 23)];

			if (CurrentSession != session) DestroyAllSessionObjects();
			CurrentSession = session;
			DisplayNavigationPanel();
			CreateUIButtons();
		}
		public static void InsertMapData(Point[,,] data, Point offset)
		{
			RawData = DefaultRawData;
			for (int y = 0; y < data.GetLength(1); y++)
				for (int x = 0; x < data.GetLength(0); x++)
					for (int z = 0; z < 3; z++)
						RawData[(int)(x + offset.X), (int)(y + offset.Y), z] = data[x, y, z];
		}

		public static bool IsHovered()
		{
			var mousePos = Screen.GetCellAtCursorPosition();
			return mousePos.X > 0 && mousePos.Y > 0 && mousePos.X < 18 && mousePos.Y < 18;
		}
		public static Point MapToScreenPosition(Point mapPos)
		{
			return mapPos - CameraPosition + new Point(9, 9);
		}
		public static Point ScreenToMapPosition(Point screenPos)
		{
			return CameraPosition + (screenPos - new Point(9, 9));
		}
	}
}
