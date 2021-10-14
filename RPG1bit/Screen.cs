using SMPL.Components;
using SMPL.Data;
using SMPL.Gear;

namespace RPG1bit
{
	public class Screen : Thing
	{
		public static Sprite Sprite { get; set; }
		public static Area Area { get; set; }

		public Screen(string uniqueID) : base(uniqueID)
		{
			Camera.Event.Subscribe.Display(uniqueID);

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
					var c = new Color();

					Sprite.RemoveQuad(quadID);

					quad.TileGridWidth = new Size(1, 1);
					quad.SetTextureCropTile(new(0, 0));
					quad.SetColor(c, c, c, c);
					for (int i = 0; i < 4; i++)
					{
						if (i > 1 && (x == 0 || y == 0 || x > 18)) break;
						Sprite.SetQuad($"{i} {quadID}", quad);
					}
				}
		}

		public static void Display()
		{
			NavigationPanel.Display();
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
		public static Point GetCellIndexesAtPosition(Point position, int depth)
		{
			var id = $"{depth} cell {position.X} {position.Y}";
			var quad = Sprite.GetQuad(id);
			var coord = quad.CornerA.TextureCoordinate;
			return new(coord.X / 17, coord.Y / 17) { Color = quad.CornerA.Position.Color };
		}
		public static void EditCell(Point cellIndexes, Point tileIndexes, int depth, Color color)
		{
			var id = $"{depth} cell {cellIndexes.X} {cellIndexes.Y}";
			var q = Sprite.GetQuad(id);
			q.SetTextureCropTile(tileIndexes);
			q.SetColor(color, color, color, color);
			Sprite.SetQuad(id, q);
		}
		public static void DisplayText(Point position, int depth, Color color, string text)
		{
			if (text == null) return;
			text = text.ToLower();
			for (int i = 0; i < text.Length; i++)
			{
				var x = position.X + i;
				var isNumber = Text.IsNumber(text[i].ToString());
				var curX = 35 + text[i] - (isNumber ? 48 : 97);
				var tileIndexes = new Point(curX, isNumber ? 17 : 18);

				if (curX > 47) tileIndexes += new Point(-13, 1);
				if (tileIndexes.X == -30) tileIndexes = new(0, 0);
				EditCell(new Point(x, position.Y), tileIndexes, depth, color);
			}
		}
		public static bool CellIsOnScreen(Point cell, bool isUI)
		{
			if (isUI == false)
			{
				var camPos = Map.CameraPosition;
				return cell.X >= camPos.X - 8 && cell.X <= camPos.X + 8 && cell.Y >= camPos.Y - 8 && cell.Y <= camPos.Y + 8;
			}
			return cell.X >= 0 && cell.Y >= 0 && cell.X <= 31 && cell.Y <= 18;
		}

		public override void OnCameraDisplay(Camera camera)
		{
			if (Sprite == null) return;
			Sprite.Display(camera);
		}
	}
}
