using SMPL.Components;
using SMPL.Data;
using SMPL.Gear;

namespace RPG1bit
{
	public static class Screen
	{
		public static Sprite Sprite { get; set; }
		public static Area Area { get; set; }
		public static Point CameraPosition { get; set; }
		private static Point prevCursorPos;

		public static void Create()
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
					Sprite.SetQuad($"{quadID} 0", quad);
				}

			for (int y = 0; y < 18; y++)
				for (int x = 18; x < 32; x++)
					EditCell(new Point(x, y), new Point(4, 22), 0, Color.Brown);
			for (int y = 2; y < 14; y++)
				for (int x = 19; x < 32; x++)
					EditCell(new Point(x, y), new Point(0, 23), 0, Color.Brown);
				for (int x = 19; x < 32; x++)
					EditCell(new Point(x, 0), new Point(0, 23), 0, Color.Brown);

			EditCell(new Point(28, 0), new Point(33, 15), 0, Color.Gray);
		}
		public static void Display()
		{
			if (Window.CurrentState == Window.State.Minimized) return;
			foreach (var kvp in Object.objects)
				for (int i = 0; i < kvp.Value.Count; i++)
				{
					var c = kvp.Value[i].Position.Color;
					var quadID = $"cell {kvp.Value[i].Position.X} {kvp.Value[i].Position.Y} {i}";
					var quad = Sprite.GetQuad(quadID);
					quad.TileSize = new(16, 16);
					quad.TileGridWidth = new Size(1, 1);
					quad.SetColor(c, c, c, c);
					quad.SetTextureCropTile(kvp.Value[i].TileIndex);
					Sprite.SetQuad(quadID, quad);
				}
		}

		public static Point GetCellAtCursorPosition()
		{
			var size = new Point(Camera.WorldCamera.Size.W, Camera.WorldCamera.Size.H);
			var topRight = Camera.WorldCamera.Position - size / 2;
			var botRight = Camera.WorldCamera.Position + size / 2;
			var x = Number.Map(Mouse.Cursor.PositionWindow.X, new(topRight.X, botRight.X), new(0, 32));
			var y = Number.Map(Mouse.Cursor.PositionWindow.Y, new(topRight.Y, botRight.Y), new(0, 18));
			return new Point((int)x, (int)y) + CameraPosition;
		}
		public static void EditCell(Point cellIndexes, Point tileIndexes, int depth, Color color)
		{
			var id = $"cell {cellIndexes.X} {cellIndexes.Y} {depth}";
			var q = Sprite.GetQuad(id);
			q.SetTextureCropTile(tileIndexes);
			q.SetColor(color, color, color, color);
			Sprite.SetQuad(id, q);
		}

		private static void Always()
		{
			if (Sprite == null || Info.Textbox == null || Window.CurrentState == Window.State.Minimized) return;

			var mousePos = GetCellAtCursorPosition();
			if (mousePos != prevCursorPos)
			{
				var quad = Sprite.GetQuad($"cell {mousePos.X} {mousePos.Y} 0");
				var coord = quad.CornerA.TextureCoordinate;
				var tileIndex = coord / new Point(quad.TileSize.W + quad.TileGridWidth.W, quad.TileSize.H + quad.TileGridWidth.H);

				Info.Textbox.Scale = new(0.35, 0.35);
				Info.Textbox.Text = Object.descriptions[tileIndex.IsInvalid ? new(0, 0) : tileIndex];
				Object.ShowClickableIndicator(false);
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
