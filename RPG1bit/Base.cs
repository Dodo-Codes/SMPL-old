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

			Directory.CreateDirectory("cache");
			Directory.CreateDirectory("deleted");
			Directory.CreateDirectory("sessions");
			Directory.CreateDirectory("worlds");

			Event.Subscribe.Update(UniqueID, 0);
			Mouse.Event.Subscribe.ButtonPress(UniqueID);
			Mouse.Event.Subscribe.ButtonRelease(UniqueID);
		}

		public override void OnWindowFocus() => Window.CurrentState = Window.State.Fullscreen;
		public override void OnAssetLoadEnd(string path)
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
				new WorldObjectManager("world-obj-manager");
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

			if (button == Mouse.Button.Left)
				LeftClickPosition = mousePos;
			if (button == Mouse.Button.Right)
				RightClickPosition = mousePos;

			if (button == Mouse.Button.ExtraButton1)
			{
				var map = new Map("item-map", new());
				var map2 = new Map("item-map2", new());
				var key = new Key("item-key", new())
				{
					Quantity = 8
				};
				var quiver = new Quiver("quiver", new());
				var bag = new Bag("bag", new());
				var rope = new Rope("rope", new());
				var pile = new ItemPile("item-pile", new())
				{
					Position = new Point(0, -5),
				};

				quiver.Stats["slots"] = 2;
				bag.Stats["slots"] = 4;

				pile.AddItem(key);
				pile.AddItem(quiver);
				pile.AddItem(bag);
				pile.AddItem(map);
				pile.AddItem(map2);
				pile.AddItem(rope);
			}
			else if (button == Mouse.Button.ExtraButton2)
			{
				var bleed = new Bleed($"bleed-{Performance.FrameCount}", new()
				{
					Position = new(-20, 0),
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
