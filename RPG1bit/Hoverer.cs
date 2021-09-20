using SMPL.Components;
using SMPL.Data;
using SMPL.Gear;

namespace RPG1bit
{
	public static class Hoverer
	{
		public static Sprite Sprite { get; set; }
		public static Area Area { get; set; }
		public static Point CursorTextureTileIndexes { get; set; } = new(36, 10);
		public static Color CursorTextureColor { get; set; } = Color.White;

		public static void Create()
		{
			Camera.CallWhen.Display(OnDisplay);

			Area = new("hoverer-area");
			Sprite = new("hoverer-sprite");
			Sprite.AreaUniqueID = Area.UniqueID;
			Area.Size = new(60, 60);
			Area.OriginPercent = new(0, 0);
		}

		private static void OnDisplay(Camera camera)
		{
			if (Assets.AreLoaded("Assets\\graphics.png") == false) return;

			var p = new Point(
				Number.Round(Mouse.Cursor.PositionWindow.X / 60 - 0.5) * 60,
				Number.Round(Mouse.Cursor.PositionWindow.Y / 60 - 0.5) * 60) -
				new Point(Area.Size.W / 2, Area.Size.H / 2);
			if (p.X == -1050) p += new Point(60, 0);
			if (p.Y == -630) p += new Point(0, 60);
			Area.Position = p + new Point(30, 30);

			Sprite.TexturePath = "Assets\\graphics.png";
			var q = Sprite.GetQuad("hoverer-sprite");
			var c = Color.Cyan;
			q.TileGridWidth = new(1, 1);
			q.TileSize = new(16, 16);
			var tileIndex = new Point(37,12);
			if (Mouse.ButtonIsPressed(Mouse.Button.Left)) tileIndex = new(28, 14);
			else if (Mouse.ButtonIsPressed(Mouse.Button.Right)) tileIndex = new(29, 14);
			q.SetTextureCropTile(tileIndex);
			q.SetColor(c, c, c, c);
			Sprite.SetQuad("hoverer-sprite", q);
			Area.OriginPercent = new(0, 0);

			Sprite.Display(camera);

			Area.Position = Mouse.Cursor.PositionWindow;
			q.SetTextureCropTile(CursorTextureTileIndexes);
			Area.OriginPercent = CursorTextureTileIndexes == new Point(36, 10) ? new(0, 0) : new(50, 50);
			c = CursorTextureColor;
			q.SetColor(c, c, c, c);
			Sprite.SetQuad("hoverer-sprite", q);

			Sprite.Display(camera);
		}
	}
}
