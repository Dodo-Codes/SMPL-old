using SMPL.Data;
using SMPL.Gear;
using System.Collections.Generic;

namespace RPG1bit
{
	public class MapEditor : Object
	{
		public static readonly Dictionary<Point, Point[]> randomTiles = new()
		{
			{ new(01, 22), new Point[] { new(01, 22) } }, // background
			{ new(05, 00), new Point[] { new(04, 00), new(5, 0), new(6, 0), new(7, 0) } }, // grass
			{ new(05, 01), new Point[] { new(03, 01), new(4, 1), new(5, 1), new(6, 1), new(7, 1) } }, // oak trees
			{ new(01, 01), new Point[] { new(00, 01), new(1, 1), new(2, 1) } }, // pine trees
			{ new(06, 02), new Point[] { new(05, 02), new(6, 2) } }, // cactus
			{ new(07, 02), new Point[] { new(07, 02) } }, // palm
			{ new(04, 02), new Point[] { new(04, 02) } }, // rocks
			{ new(00, 02), new Point[] { new(00, 02), new(1, 2), new(2, 2), new(3, 2) } }, // bushes

			// water
			{ new(07, 03), new Point[] { new(07, 03) } },
			{ new(08, 03), new Point[] { new(08, 03) } },
			{ new(09, 03), new Point[] { new(09, 03) } },
			{ new(10, 03), new Point[] { new(10, 03) } },
			{ new(11, 03), new Point[] { new(11, 03) } },
			{ new(12, 03), new Point[] { new(12, 03) } },
			{ new(07, 04), new Point[] { new(07, 04) } },
			{ new(08, 04), new Point[] { new(08, 04) } },
			{ new(09, 04), new Point[] { new(09, 04) } },
			{ new(10, 04), new Point[] { new(10, 04) } },
			{ new(11, 04), new Point[] { new(11, 04) } },
			{ new(12, 04), new Point[] { new(12, 04) } },
			{ new(13, 04), new Point[] { new(13, 04) } },
			{ new(14, 04), new Point[] { new(14, 04) } },
			{ new(07, 05), new Point[] { new(07, 05) } },
			{ new(08, 05), new Point[] { new(08, 05) } },
			{ new(09, 05), new Point[] { new(09, 05) } },
			{ new(10, 05), new Point[] { new(10, 05) } },
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

		public MapEditor(CreationDetails creationDetails) : base(creationDetails) { Assets.CallWhen.LoadEnd(OnLoadEnd); }

		private static void OnLoadEnd()
		{
			if (Assets.ValuesAreLoaded("camera-position", "map-data"))
			{
				var cameraPos = Text.FromJSON<Point>(Assets.GetValue("camera-position"));
				var mapData = Text.FromJSON<Point[,,]>(Assets.GetValue("map-data"));
				Map.Data = mapData;
				Map.CameraPosition = cameraPos;

				Map.Display(); // for the map iteself
				DisplayAllObjects(); // for the ui
			}
		}

		public override void OnHovered() => NavigationPanel.Info.Textbox.Text = "Start a new map edit session.";
		public override void OnLeftClicked()
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
			NavigationPanel.Tab.Close();
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
