using SMPL.Components;
using SMPL.Data;
using SMPL.Gear;

namespace RPG1bit
{
	public class Hoverer : Thing
	{
		public static Sprite Sprite { get; set; }
		public static Area Area { get; set; }
		public static Color Color { get; set; } = Color.Cyan;
		public static Point DefaultCursorTileIndexes { get; set; } = new(36, 10);
		public static Point DefaultTileIndexes { get; set; } = new(37, 12);
		public static Point CursorTileIndexes { get; set; } = DefaultCursorTileIndexes;
		public static Point TileIndexes { get; set; } = DefaultTileIndexes;
		public static Color CursorColor { get; set; } = Color.White;
		public Hoverer(string uniqueID) : base(uniqueID)
		{
			Camera.Event.Subscribe.Display(UniqueID);

			Area = new("hoverer-area");
			Sprite = new("hoverer-sprite");
			Sprite.AreaUniqueID = Area.UniqueID;
			Area.Size = new(60, 60);
			Area.OriginPercent = new(0, 0);
		}

		public override void OnCameraDisplay(Camera camera)
		{
			if (Assets.AreLoaded("Assets\\graphics.png") == false) return;

			var onMainMenu = Map.CurrentSession == Map.Session.None && Screen.GetCellAtCursorPosition().X < 18;

			var mousePos = Screen.GetCellAtCursorPosition() * 60;
			var p = mousePos - new Point(Screen.Area.Size.W / 2, Screen.Area.Size.H / 2) + new Point(30, 30);

			if (p.X == -990) p += new Point(60, 0);
			if (p.Y == -570) p += new Point(0, 60);
			Area.Position = p;

			Sprite.TexturePath = "Assets\\graphics.png";
			var q = Sprite.GetQuad("hoverer-sprite");
			var c = Color;
			q.TileGridWidth = new(1, 1);
			q.TileSize = new(16, 16);
			var tileIndex = TileIndexes;
			if (Mouse.ButtonIsPressed(Mouse.Button.Left)) tileIndex = new(28, 14);
			else if (Mouse.ButtonIsPressed(Mouse.Button.Right)) tileIndex = new(29, 14);
			q.SetTextureCropTile(tileIndex);
			q.SetColor(c, c, c, c);
			Sprite.SetQuad("hoverer-sprite", q);
			Area.OriginPercent = new(50, 50);

			if (onMainMenu == false) Sprite.Display(camera);

			Area.OriginPercent = new(0, 0);
			Area.Angle = 0;
			Area.Position = Mouse.Cursor.PositionWindow;
			q.SetTextureCropTile(CursorTileIndexes);
			Area.OriginPercent = CursorTileIndexes == DefaultCursorTileIndexes ? new(0, 0) : new(50, 50);
			c = CursorColor;
			q.SetColor(c, c, c, c);
			Sprite.SetQuad("hoverer-sprite", q);

			Sprite.Display(camera);
		}
	}
}
