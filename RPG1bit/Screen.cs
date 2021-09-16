using SMPL.Components;
using SMPL.Data;
using SMPL.Gear;

namespace RPG1bit
{
	public static class Screen
	{
		public static Sprite Sprite { get; set; }
		public static Area Area { get; set; }

		private static Point prevCursorPos;

		private static void CreateAndInitializeScreen()
		{
			Camera.CallWhen.Display(OnDisplay);
			Game.CallWhen.GameIsRunning(Always);

			Area = new("map-area");
			Area.Size = Camera.WorldCamera.Size;

			Sprite = new("map-sprite");
			Sprite.AreaUniqueID = Area.UniqueID;
			Sprite.TexturePath = "Assets\\graphics.png";
			Sprite.SetQuadGrid("cell", new(32, 18), new(16, 16));
			Sprite.RemoveQuad("map-sprite");

			for (int y = 0; y < 18; y++)
				for (int x = 0; x < 32; x++)
				{
					var quadID = $"cell {x} {y}";
					var quad = Sprite.GetQuad(quadID);

					Sprite.RemoveQuad(quadID);

					quad.TileGridWidth = new Size(1, 1);
					quad.SetTextureCropTile(new(0, 0));
					Sprite.SetQuad($"{quadID} 2", quad);
					Sprite.SetQuad($"{quadID} 1", quad);
					Sprite.SetQuad($"{quadID} 0", quad);
				}
		}
		public static void Create()
		{
			CreateAndInitializeScreen();

			NavigationPanel.CreateButtons();
			NavigationPanel.Info.Create();
			Map.CreateUIButtons();
			Hoverer.Create();
		}
		public static void Display()
		{
			NavigationPanel.Display();
			NavigationPanel.Info.Display();

			Map.Display();
			Object.DisplayAllObjects();
		}

		public static Point GetCellAtCursorPosition()
		{
			var size = new Point(Camera.WorldCamera.Size.W, Camera.WorldCamera.Size.H);
			var topRight = Camera.WorldCamera.Position - size / 2;
			var botRight = Camera.WorldCamera.Position + size / 2;
			var x = Number.Map(Mouse.Cursor.PositionWindow.X, new(topRight.X, botRight.X), new(0, 32));
			var y = Number.Map(Mouse.Cursor.PositionWindow.Y, new(topRight.Y, botRight.Y), new(0, 18));
			return new Point((int)x, (int)y);
		}
		public static void EditCell(Point cellIndexes, Point tileIndexes, int depth, Color color)
		{
			var id = $"cell {cellIndexes.X} {cellIndexes.Y} {depth}";
			if (Sprite.HasQuad(id) == false) return;
			var q = Sprite.GetQuad(id);
			q.SetTextureCropTile(tileIndexes);
			q.SetColor(color, color, color, color);
			Sprite.SetQuad(id, q);
		}

		private static void Always()
		{
			if (Sprite == null || NavigationPanel.Info.Textbox == null || Window.CurrentState == Window.State.Minimized) return;

			var mousePos = GetCellAtCursorPosition();
			if (mousePos != prevCursorPos)
			{
				NavigationPanel.Info.Textbox.Text = "";
				NavigationPanel.Info.Textbox.Scale = new(0.35, 0.35);
				NavigationPanel.Info.ShowClickableIndicator(false);

				for (int i = 0; i < 3; i++)
				{
					var quadID = $"cell {mousePos.X} {mousePos.Y} {i}";
					if (Sprite.HasQuad(quadID) == false) continue;

					var quad = Sprite.GetQuad(quadID);
					var coord = quad.CornerA.TextureCoordinate;
					var tileIndex = coord / new Point(quad.TileSize.W + quad.TileGridWidth.W, quad.TileSize.H + quad.TileGridWidth.H);
					var description = Object.descriptions[tileIndex.IsInvalid ? new(0, 0) : tileIndex];
					var defaultDescr = Object.descriptions[new(0, 0)];
					var sep = i != 0 ? "\n" : "";

					if (NavigationPanel.Info.Textbox.Text != "" && (description == defaultDescr || description == "")) break;
					NavigationPanel.Info.Textbox.Text += $"{sep}{description}";
				}
			}
			prevCursorPos = mousePos;
		}
		private static void OnDisplay(Camera camera)
		{
			if (Sprite == null) return;
			Sprite.Display(camera);
		}
	}
}
