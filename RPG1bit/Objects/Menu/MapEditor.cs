using SMPL.Data;
using SMPL.Gear;
using System.Collections.Generic;

namespace RPG1bit
{
	public class MapEditor : Object
	{
		public static readonly Dictionary<Point, Point[]> randomTiles = new()
		{
			{ new(1, 22), new Point[] { new(1, 22) } }, // background
			{ new(5, 0), new Point[] { new(4, 0), new(5, 0), new(6, 0), new(7, 0) } }, // grass
			{ new(5, 1), new Point[] { new(3, 1), new(4, 1), new(5, 1), new(6, 1), new(7, 1) } }, // oak trees
			{ new(1, 1), new Point[] { new(0, 1), new(1, 1), new(2, 1) } }, // pine trees
			{ new(6, 2), new Point[] { new(5, 2), new(6, 2) } }, // cactus
			{ new(7, 2), new Point[] { new(7, 2) } }, // palm
			{ new(4, 2), new Point[] { new(4, 2) } }, // rocks
			{ new(0, 2), new Point[] { new(0, 2), new(1, 2), new(2, 2), new(3, 2) } }, // bushes
		};
		private static Point CurrentTile
		{
			get
			{
				var tiles = randomTiles[SwitchType.BrushType];
				var randomTileIndex = (int)Probability.Randomize(new Number.Range(0, tiles.Length - 1));
				var result = tiles[randomTileIndex];
				result.Color = SwitchColor.BrushColor;
				return result;
			}
		}

		public MapEditor(CreationDetails creationDetails) : base(creationDetails) { }
		protected override void OnHovered() => NavigationPanel.Info.Textbox.Text = "Start a new map edit session.";
		protected override void OnLeftClicked()
		{
			if (Map.CurrentSession != Map.Session.MapEdit) Map.DestroyAllSessionObjects();
			Map.CurrentSession = Map.Session.MapEdit;
			Map.DisplayNavigationPanel();

			Map.Data = new Point[100, 100, 3];

			Map.CreateUIButtons();
			Map.Display(); // for the map iteself
			DisplayAllObjects(); // for the ui
		}

		public static void EditCurrentTile()
		{
			var LMB = Mouse.ButtonIsPressed(Mouse.Button.Left);
			var clickPos = Map.ScreenToMapPosition(LMB ? LeftClickPosition : RightClickPosition);
			var mouseCell = Screen.GetCellAtCursorPosition();
			var pos = Map.ScreenToMapPosition(mouseCell);

			if (Keyboard.KeyIsPressed(Keyboard.Key.LeftShift) && clickPos != pos)
			{
				var dirY = pos.Y > clickPos.Y ? 1 : -1;
				var dirX = pos.X > clickPos.X ? 1 : -1;
				for (double y = clickPos.Y; y != pos.Y + dirY; y += dirY)
					for (double x = clickPos.X; x != pos.X + dirX; x += dirX)
						Map.Data[(int)x, (int)y, SwitchHeight.BrushHeight] = LMB ? CurrentTile : new(0, 0);
			}

			Map.Data[(int)pos.X, (int)pos.Y, SwitchHeight.BrushHeight] = LMB ? CurrentTile : new(0, 0);
			Map.Display();
		}
		public static void PickCurrentTile()
		{
			SwitchColor.SelectHoveredTileColor();
			SwitchColor.UpdateIndicator();
			SwitchType.SelectHoveredTileType();
			SwitchType.UpdateIndicator();
		}
	}
}
