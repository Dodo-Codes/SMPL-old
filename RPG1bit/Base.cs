using SMPL.Gear;
using SMPL.Data;
using System.IO;

namespace RPG1bit
{
	class Base : Game
	{
		public static Point prevCursorPos = new(-1, 0);
		public static Point LeftClickPosition { get; private set; }
		public static Point RightClickPosition { get; private set; }

		public Base(string uniqueID) : base(uniqueID) { }
		static void Main() => Start(new Base("game"), new Size(1, 1));
		public override void OnGameCreate()
		{
			Assets.Load(Assets.Type.Texture, "Assets\\graphics.png");
			Assets.Load(Assets.Type.Font, "Assets\\font.ttf");
			Window.Event.Subscribe.Focus("game");
			Assets.Event.Subscribe.LoadEnd("game");
			Window.Title = "Violint";
			Window.CurrentState = Window.State.Fullscreen;
			Mouse.Cursor.IsHidden = true;

			Directory.CreateDirectory("chunks");
			Directory.CreateDirectory("deleted");
			Directory.CreateDirectory("sessions");
			Directory.CreateDirectory("worlds");

			Event.Subscribe.Update(UniqueID, 0);
			Mouse.Event.Subscribe.ButtonPress(UniqueID);
			Mouse.Event.Subscribe.ButtonRelease(UniqueID);
		}

		public override void OnWindowFocus() => Window.CurrentState = Window.State.Fullscreen;
		public override void OnAssetsLoadEnd()
		{
			if (Gate.EnterOnceWhile("graphics-and-font-loaded", Assets.AreLoaded("Assets\\graphics.png", "Assets\\font.ttf")))
			{
				new Screen("screen");
				new NavigationPanel.Tab("tab");
				new NavigationPanel.Info("info");
				new NavigationPanel("nav-panel");
				new Hoverer("hoverer");
				new World("world");
				new ChunkManager("chunk-manager");
				Screen.ScheduleDisplay();
			}
		}

		public override void OnGameUpdate()
		{
			if (Screen.Sprite == null || NavigationPanel.Info.Textbox == null || Window.CurrentState == Window.State.Minimized) return;
			var mousePos = Screen.GetCellAtCursorPosition();
			if (Screen.CellIsOnScreen(mousePos, true) == false) return;

			if (World.CurrentSession == World.Session.None && mousePos.X < 18)
			{
				NavigationPanel.Info.Textbox.Scale = new(0.6, 0.6);
				NavigationPanel.Info.Textbox.Text = $"{Window.Title} {NavigationPanel.Info.GameVersion}";
				return;
			}
			if (mousePos != prevCursorPos)
			{
				if (World.CurrentSession == World.Session.WorldEdit && World.IsHovered())
				{
					if (Mouse.ButtonIsPressed(Mouse.Button.Left) || Mouse.ButtonIsPressed(Mouse.Button.Right))
						WorldEditor.EditCurrentTile();
				}
				NavigationPanel.Info.Update();
			}
			prevCursorPos = mousePos;
		}
		public override void OnMouseButtonPress(Mouse.Button button)
		{
			var mousePos = Screen.GetCellAtCursorPosition();
			if (button == Mouse.Button.Left) LeftClickPosition = mousePos;
			if (button == Mouse.Button.Right) RightClickPosition = mousePos;

			if (button == Mouse.Button.ExtraButton1)
			{
				var map = new Map("item-map", new()
				{
					Name = "Map",
					Position = new(-10, 0),
					Height = 2,
					TileIndexes = new Point[] { new(32, 15) { C = Color.Wood + 60 } },
					IsUI = true,
					IsDragable = true,
					IsRightClickable = true,
					IsLeftClickable = true,
				});
				var map2 = new Map("item-map2", new()
				{
					Name = "Map",
					Position = new(-10, 0),
					Height = 2,
					TileIndexes = new Point[] { new(32, 15) { C = Color.Wood + 60 } },
					IsUI = true,
					IsDragable = true,
					IsRightClickable = true,
					IsLeftClickable = true,
				});
				var key = new Key("item-key", new()
				{
					Name = "Key",
					Position = new(-10, 0),
					Height = 2,
					TileIndexes = new Point[]
				{ new(32, 11) { C = Color.Gray }, new(33, 11) { C = Color.Gray }, new(34, 11) { C = Color.Gray } },
					IsUI = true,
					IsDragable = true,
					IsRightClickable = true,
					IsLeftClickable = true,
				})
				{ Quantity = 8 };
				var quiver = new Quiver("quiver", new()
				{
					Name = "Quiver",
					Position = new(-10, 0),
					Height = 2,
					TileIndexes = new Point[] { new(42, 6) { C = Color.Brown + 30 } },
					IsUI = true,
					IsDragable = true,
					IsRightClickable = true,
					IsLeftClickable = true,
				})
				{ Positives = new double[] { 1, 0 } };
				var bag = new Bag("bag", new()
				{
					Name = "Bag",
					Position = new(-10, 0),
					Height = 2,
					TileIndexes = new Point[] { new(44, 4) { C = Color.Brown + 30 } },
					IsUI = true,
					IsDragable = true,
					IsRightClickable = true,
					IsLeftClickable = true,
				})
				{ Positives = new double[] { 2, 0 } };
				var pile = new ItemPile("item-pile", new()
				{
					Position = new Point(7, 0),
					Height = 3,
					TileIndexes = new Point[] { new(8, 23) },
					Name = "item-pile",
				});
				pile.AddItem(key);
				pile.AddItem(quiver);
				pile.AddItem(bag);
				pile.AddItem(map);
				pile.AddItem(map2);
			}
			else if (button == Mouse.Button.ExtraButton2)
			{
				var bleed = new Bleed($"bleed-{Performance.FrameCount}", new()
				{
					Position = new(-10, 0),
					Height = 1,
					TileIndexes = new Point[] { new() },
					Name = "bleed",
					AppearOnTab = "player-stats",
					IsUI = true,
					IsInTab = true,
				})
				{
					Value = -1,
					Duration = new int[2] { 0, 10 },
				};
				var player = (Player)PickByUniqueID(nameof(Player));
				player.ApplyEffect(bleed);
			}

			if ((World.IsHovered() || NavigationPanel.Tab.IsHovered()) && button == Mouse.Button.Middle)
				WorldEditor.PickCurrentTile();
			if (World.CurrentSession != World.Session.WorldEdit || World.IsHovered() == false)
				return;
			if (button == Mouse.Button.Left || button == Mouse.Button.Right)
				WorldEditor.EditCurrentTile();
		}
	}
}
