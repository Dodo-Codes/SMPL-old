using SMPL.Gear;
using SMPL.Data;

namespace RPG1bit
{
	class Base : Game
	{
		private static Point prevCursorPos = new(-1, 0);
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
				new NavigationPanel("nav-panel");
				new NavigationPanel.Tab("tab");
				new NavigationPanel.Info("info");
				new Hoverer("hoverer");
				new Map("map");
				Screen.Display();
			}
		}

		public override void OnGameUpdate()
		{
			if (Screen.Sprite == null || NavigationPanel.Info.Textbox == null || Window.CurrentState == Window.State.Minimized) return;
			var mousePos = Screen.GetCellAtCursorPosition();
			if (Screen.CellIsOnScreen(mousePos, true) == false) return;

			if (Map.CurrentSession == Map.Session.None && mousePos.X < 18)
			{
				NavigationPanel.Info.Textbox.Scale = new(0.6, 0.6);
				NavigationPanel.Info.Textbox.Text = $"{Window.Title} {NavigationPanel.Info.GameVersion}";
				return;
			}
			if (mousePos != prevCursorPos)
			{
				if (Map.CurrentSession == Map.Session.MapEdit && Map.IsHovered())
				{
					if (Mouse.ButtonIsPressed(Mouse.Button.Left) || Mouse.ButtonIsPressed(Mouse.Button.Right))
						MapEditor.EditCurrentTile();
				}

				NavigationPanel.Info.Textbox.Text = "";
				NavigationPanel.Info.Textbox.Scale = new(0.35, 0.35);
				NavigationPanel.Info.ShowClickableIndicator(false);
				NavigationPanel.Info.ShowDragableIndicator(false);
				NavigationPanel.Info.ShowLeftClickableIndicator(false);
				NavigationPanel.Info.ShowRightClickableIndicator(false);

				for (int i = 0; i < 4; i++)
				{
					var quadID = $"{i} cell {mousePos.X} {mousePos.Y}";
					if (Screen.Sprite.HasQuad(quadID) == false) continue;
					var quad = Screen.Sprite.GetQuad(quadID);
					var coord = quad.CornerA.TextureCoordinate;
					var tileIndex = coord / new Point(quad.TileSize.W + quad.TileGridWidth.W, quad.TileSize.H + quad.TileGridWidth.H);
					var key = tileIndex.IsInvalid() ? new(0, 0) : tileIndex;
					var description = Object.descriptions.ContainsKey(key) ? Object.descriptions[key] : "";
					var sep = i != 0 && description != "" && NavigationPanel.Info.Textbox.Text != "" ? "\n" : "";

					if (mousePos.Y > 0)
					{
						if (key == Map.TileBarrier) description = "Unwalkable terrain.";
						else if (key == Map.TilePlayer) description = "Player tile.";
					}

					if (NavigationPanel.Info.Textbox.Text != "" && description == Object.descriptions[new(0, 0)]) continue;
					NavigationPanel.Info.Textbox.Text = $"{description}{sep}{NavigationPanel.Info.Textbox.Text}";
				}
			}
			prevCursorPos = mousePos;
		}
		public override void OnMouseButtonPress(Mouse.Button button)
		{
			var mousePos = Screen.GetCellAtCursorPosition();
			if (button == Mouse.Button.Left) LeftClickPosition = mousePos;
			if (button == Mouse.Button.Right) RightClickPosition = mousePos;

			if ((Map.IsHovered() || NavigationPanel.Tab.IsHovered()) && button == Mouse.Button.Middle)
				MapEditor.PickCurrentTile();
			if (Map.CurrentSession != Map.Session.MapEdit || Map.IsHovered() == false)
				return;
			if (button == Mouse.Button.Left || button == Mouse.Button.Right)
				MapEditor.EditCurrentTile();
		}
	}
}
