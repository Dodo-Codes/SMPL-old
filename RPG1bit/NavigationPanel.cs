using SMPL.Components;
using SMPL.Data;
using SMPL.Gear;
using System.Collections.Generic;
using static RPG1bit.Object;

namespace RPG1bit
{
	public class NavigationPanel : Thing
	{
		public class Tab : Thing
		{
			public static Textbox Textbox { get; set; }
			public static Area Area { get; set; }
			public static Effects Effects { get; set; }

			public static string CurrentTabType { get; set; }
			public static string Title { get; set; }
			public static Dictionary<string, string> Texts { get; set; } = new();

			public Tab(string uniqueID) : base(uniqueID)
			{
				Camera.Event.Subscribe.Display(uniqueID);

				Area = new("tab-area");
				Effects = new("tab-effects");
				Effects.OutlineWidth = 10;
				Effects.BackgroundColor = new();

				var sz = Camera.WorldCamera.Size / 2;
				Area.Position = new Point(60 * 25.5, 60 * 7.9) - new Point(sz.W, sz.H);
				Area.Size = new(60 * 13, 60 * 12);

				Textbox = new("tab-textbox");
				Textbox.AreaUniqueID = "tab-area";
				Textbox.EffectsUniqueID = "tab-effects";
				Textbox.FontPath = "Assets\\font.ttf";
				Textbox.CharacterSize = 128;
				Textbox.Spacing = new(4, 0.5);
				Textbox.Scale = new(0.4, 0.4);
				Textbox.OriginPercent = new(50, 0);
				Textbox.Text = "";
			}

			public static void Close()
			{
				CurrentTabType = null;
				Title = "";
				Textbox.Text = "";
				Screen.Display();
			}
			public static void Open(string type, string title)
			{
				Close();
				CurrentTabType = type;
				Title = title;
				Textbox.Text = Texts.ContainsKey(type) ? Texts[type] : "";
				Screen.Display();
			}
			public static bool IsHovered()
			{
				var mousePos = Screen.GetCellAtCursorPosition();
				return mousePos.X > 18 && mousePos.Y > 1 && mousePos.X < 32 && mousePos.Y < 14;
			}

			public override void OnCameraDisplay(Camera camera)
			{
				if (Textbox == null) return;
				Textbox.Display(camera);
			}
		}
		public class Info : Thing
		{
			public const string GameVersion = "v0.1";
			public static Textbox Textbox { get; set; }
			public static Area Area { get; set; }
			public static Effects Effects { get; set; }

			private static bool leftClickable, rightClickable, dragable;

			public Info(string uniqueID) : base(uniqueID)
			{
				Camera.Event.Subscribe.Display(uniqueID);

				Area = new("info-area");
				Effects = new("info-effects");
				Effects.OutlineWidth = 10;

				var sz = Camera.WorldCamera.Size / 2;
				Area.Position = new Point(60 * 25.2, 60 * 16.6) - new Point(sz.W, sz.H);
				Area.Size = new(60 * 12.3, 60 * 4);

				Textbox = new("info-textbox");
				Textbox.AreaUniqueID = "info-area";
				Textbox.EffectsUniqueID = "info-effects";
				Textbox.FontPath = "Assets\\font.ttf";
				Textbox.CharacterSize = 128;
				Textbox.Spacing = new(4, -6);
				Textbox.Scale = new(0.35, 0.35);
				Textbox.Text = "";
			}
			public static void Display()
			{
				for (int y = 15; y < 18; y++)
					for (int x = 19; x < 32; x++)
						Screen.EditCell(new Point(x, y), new Point(1, 23), 1, new Color());
				ShowLeftClickableIndicator(leftClickable, dragable);
				ShowRightClickableIndicator(rightClickable);
			}
			public static void Update()
			{
				Textbox.Text = "";
				Textbox.Scale = new(0.35, 0.35);
				ShowLeftClickableIndicator(false);
				ShowRightClickableIndicator(false);

				var mousePos = Screen.GetCellAtCursorPosition();
				var objName = "";
				var objs = objects.ContainsKey(mousePos) ? objects[mousePos] : new List<Object>();

				for (int j = 0; j < objs.Count; j++)
						objName = objs[j].HoveredInfo + (string.IsNullOrEmpty(objs[j].HoveredInfo) ? "" : "\n");

				for (int i = 0; i < 4; i++)
				{
					var quadID = $"{i} cell {mousePos.X} {mousePos.Y}";
					if (Screen.Sprite.HasQuad(quadID) == false) continue;
					var quad = Screen.Sprite.GetQuad(quadID);
					var coord = quad.CornerA.TextureCoordinate;
					var tileIndex = coord / new Point(quad.TileSize.W + quad.TileGridWidth.W, quad.TileSize.H + quad.TileGridWidth.H);
					var key = tileIndex.IsInvalid() ? new(0, 0) : tileIndex;
					var description = descriptions.ContainsKey(key) ? descriptions[key] : "";
					var sep = i != 0 && description != "" && Textbox.Text != "" ? "\n" : "";
					var isPanel = mousePos.X > 18 && key == new Point();

					if (mousePos.Y > 0)
					{
						if (key == Map.TileBarrier) description = "Unwalkable terrain.";
						else if (key == Map.TilePlayer) description = "Player tile.";
					}
					if (Textbox.Text == "" && isPanel) description = "Game navigation panel.";

					if (Textbox.Text != "" && description == descriptions[new(0, 0)]) continue;
					Textbox.Text = $"{description}{sep}{Textbox.Text}";
				}
				Textbox.Text = $"{objName}{Textbox.Text}";
			}
			public override void OnCameraDisplay(Camera camera)
			{
				if (Textbox == null) return;
				Textbox.Display(camera);
			}

			public static void ShowLeftClickableIndicator(bool show = true, bool drag = false)
			{
				leftClickable = show;
				Screen.EditCell(new Point(31, 15), new Point(29, 15), 1, show || drag ? Color.White : new Color());
				Screen.EditCell(new Point(30, 15), new Point(2, 22), 1, show ? Color.White : new Color());
				dragable = drag;
				if (dragable)
				{
					var both = leftClickable ? new Point(2, 23) : new Point(3, 22);
					Screen.EditCell(new Point(30, 15), both, 1, Color.White);
				}
			}
			public static void ShowRightClickableIndicator(bool show = true)
			{
				rightClickable = show;
				Screen.EditCell(new Point(31, 17), new Point(30, 15), 1, show ? Color.White : new Color());
				Screen.EditCell(new Point(30, 17), new Point(2, 22), 1, show ? Color.White : new Color());
			}
		}

		public NavigationPanel(string uniqueID) : base(uniqueID)
		{
			new StartSingle("start-singleplayer", new CreationDetails()
			{
				Name = "start-singleplayer",
				Position = new(19, 0),
				TileIndexes = new Point[] { new(43, 16) { C = Color.Gray } },
				Height = 1,
				IsLeftClickable = true,
				IsUI = true,
			});
			new StartMulti("start-multiplayer", new CreationDetails()
			{
				Name = "start-multiplayer",
				Position = new(20, 0),
				TileIndexes = new Point[] { new(44, 16) { C = Color.Gray / 1.2 } },
				Height = 1,
				IsUI = true,
			});
			new SaveLoad("save-load", new CreationDetails()
			{
				Name = "save-load",
				Position = new(23, 0),
				TileIndexes = new Point[] { new(42, 16) { C = Color.Gray } },
				Height = 1,
				IsLeftClickable = true,
				IsUI = true,
			});
			new MapEditor("map-editor-button", new CreationDetails()
			{
				Name = "map-editor",
				Position = new(21, 0),
				TileIndexes = new Point[] { new(47, 06) { C = Color.Gray } },
				Height = 1,
				IsUI = true,
				IsConfirmingClick = true,
				IsLeftClickable = true
			});

			new AdjustVolume("adjust-sound-volume", new CreationDetails()
			{
				Name = "Sound effects",
				Position = new(26, 0),
				TileIndexes = new Point[] { new(38, 16) { C = new Color(175, 175, 175) } },
				Height = 1,
				IsUI = true,
				IsLeftClickable = true,
			});
			new AdjustVolume("adjust-music-volume", new CreationDetails()
			{
				Name = "Music",
				Position = new(27, 0),
				TileIndexes = new Point[] { new(39, 16) { C = new Color(175, 175, 175) } },
				Height = 1,
				IsUI = true,
				IsLeftClickable = true,
			});

			new MinimizeGame("-", new CreationDetails()
			{
				Name = "-",
				Position = new(30, 0),
				TileIndexes = new Point[] { new(37, 20) { C = Color.Gray } },
				Height = 1,
				IsUI = true,
				IsLeftClickable = true,
			});
			new ExitGame("x", new CreationDetails()
			{
				Name = "x",
				Position = new(31, 0),
				TileIndexes = new Point[] { new(40, 13) { C = Color.Red - 50 } },
				Height = 1,
				IsLeftClickable = true,
				IsUI = true,
				IsConfirmingClick = true,
			});
		}
		public static void Display()
		{
			for (int y = 0; y < 18; y++)
				for (int x = 18; x < 32; x++)
				{
					Screen.EditCell(new Point(x, y), new Point(1, 22), 0, Color.Brown / 2);
					Screen.EditCell(new Point(x, y), new Point(0, 0), 1, new());
				}

			for (int x = 18; x < 32; x++)
			{
				Screen.EditCell(new Point(x, 1), new Point(4, 22), 0, Color.Brown);
				Screen.EditCell(new Point(x, 14), new Point(4, 22), 0, Color.Brown);
			}
			for (int y = 0; y < 18; y++)
				Screen.EditCell(new Point(18, y), new Point(4, 22), 0, Color.Brown);

			Screen.DisplayText(new(19, 1), 1, Color.Gray + 125, Tab.Title);
			Screen.EditCell(new Point(28, 0), new Point(33, 15), 1, Color.Gray);
			Screen.EditCell(new Point(22, 0), new Point(4, 22), 0, Color.Brown);
			Screen.EditCell(new Point(29, 0), new Point(4, 22), 0, Color.Brown);

			Info.Display();
		}
	}
}
