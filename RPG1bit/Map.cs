using SMPL.Components;
using SMPL.Data;
using SMPL.Gear;

namespace RPG1bit
{
	public static class Map
	{
		public static Sprite Sprite { get; set; }
		public static Area Area { get; set; }
		public static Point CameraPosition { get; set; }

		public static void Create()
		{
			Assets.CallWhen.LoadEnd(OnGraphicsLoaded);
			Camera.CallWhen.Display(OnDisplay);
			Game.CallWhen.GameIsRunning(Always);
		}

		private static void Always()
		{
			if (Performance.FrameCount % 10 == 0) Window.Title = $"SMPL Game (FPS: {Performance.FPS:F2})";
		}

		public static void Display()
		{
			for (int y = 0; y < 18; y++)
				for (int x = 0; x < 32; x++)
				{
					var pos = CameraPosition + new Point(x, y);
					if (Object.objects.ContainsKey(pos) == false) continue;
					var obj = Object.objects[pos];
					for (int i = 0; i < obj.Count; i++)
					{
						var quadID = $"cell {x} {y} {i}";
						var quad = Sprite.GetQuad(quadID);
						quad.TileSize = new(16, 16);
						quad.TileGridWidth = new Size(1, 1);
						quad.SetTextureCropTile(obj[i].TileIndex);
						Sprite.SetQuad(quadID, quad);
					}
				}
			DisplayUI();
		}
		public static void DisplayUI()
		{
			EditQuad(new Point(29, 0), new Point (37, 20), 0);
			EditQuad(new Point(30, 0), new Point (47, 16), 0);
			EditQuad(new Point(31, 0), new Point (40, 13), 0);
		}

		public static Point GetCellAtCursorPosition()
		{
			var size = new Point(Camera.WorldCamera.Size.W, Camera.WorldCamera.Size.H);
			var topRight = Camera.WorldCamera.Position - size / 2;
			var botRight = Camera.WorldCamera.Position + size / 2;
			var x = Number.Map(Mouse.CursorPositionWindow.X, new(topRight.X, botRight.X), new(0, 32));
			var y = Number.Map(Mouse.CursorPositionWindow.Y, new(topRight.Y, botRight.Y), new(0, 18));
			return new Point((int)x, (int)y) + CameraPosition;
		}

		private static void OnDisplay(Camera camera)
		{
			if (Sprite == null) return;
			Sprite.Display(camera);
		}
		private static void OnGraphicsLoaded()
		{
			if (Assets.AreLoaded("graphics.png") == false) return;

			Area = new("map-area");
			Area.Size = Camera.WorldCamera.Size;

			Sprite = new("map-sprite");
			Sprite.AreaUniqueID = Area.UniqueID;
			Sprite.TexturePath = "graphics.png";
			Sprite.SetQuadGrid("cell", 32, 18);
			Sprite.RemoveQuad("map-sprite");

			for (int y = 0; y < 18; y++)
				for (int x = 0; x < 32; x++)
				{
					var quadID = $"cell {x} {y}";
					var quad = Sprite.GetQuad(quadID);

					Sprite.RemoveQuad(quadID);

					quad.TileSize = new(16, 16);
					quad.TileGridWidth = new Size(1, 1);
					quad.SetTextureCropTile(new(0, 0));
					Sprite.SetQuad($"{quadID} 0", quad);
				}

			Display();
		}
		public static void EditQuad(Point quadIndexes, Point tileIndexes, int depth)
		{
			var id = $"cell {quadIndexes.X} {quadIndexes.Y} {depth}";
			var q = Sprite.GetQuad(id);
			q.SetTextureCropTile(tileIndexes);
			Sprite.SetQuad(id, q);
		}
	}
}
