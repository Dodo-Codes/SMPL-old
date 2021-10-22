using SMPL.Data;
using SMPL.Gear;
using System.Collections.Generic;

namespace RPG1bit
{
	public class MapEditor : Object
	{
		public static readonly Point[] tiles = new Point[]
		{
			new(01, 22) { C = Color.White },				// background
			new(00, 22) { C = Color.Red },				// barrier
			new(24, 08) { C = Color.White },				// player spawn tile
			new(), new(), new(), new(), new(), new(),
			new(05, 00) { C = Color.Grass },				// grass
			new(00, 02) { C = Color.Grass },				// bushes
			new(15, 03) { C = Color.Yellow },			// flowers
			new(05, 01) { C = Color.LeafSummer },		// oak trees
			new(01, 01) { C = Color.LeafSummer },		// pine trees
			new(07, 02) { C = Color.Grass },				// palm
			new(06, 02) { C = Color.Grass },				// cactus
			new(04, 02) { C = Color.Gray + 50},		// rocks
			new(),
			// water
			new(12, 3) { C = Color.Water }, new(8, 03) { C = Color.Water }, new(07, 4) { C = Color.Water },
			new(10, 4) { C = Color.Water }, new(13, 4) { C = Color.Water }, new(14, 4) { C = Color.Water },
			new(14, 4) { C = Color.Water }, new(14, 4) { C = Color.Water }, new(7, 05) { C = Color.Water },
			new(11, 3) { C = Color.Water }, new(07, 3) { C = Color.Water }, new(09, 3) { C = Color.Water },
			new(11, 4) { C = Color.Water }, new(08, 5) { C = Color.Water }, new(14, 4) { C = Color.Water },
			new(14, 4) { C = Color.Water }, new(14, 4) { C = Color.Water }, new(09, 5) { C = Color.Water },
			new(09, 4) { C = Color.Water }, new(10, 3) { C = Color.Water }, new(08, 4) { C = Color.Water },
			new(12, 4) { C = Color.Water }, new(11, 5) { C = Color.Water }, new(14, 4) { C = Color.Water },
			new(14, 4) { C = Color.Water }, new(14, 4) { C = Color.Water }, new(10, 5) { C = Color.Water },
			// bridge
			new(12, 05) { C = Color.Wood - 30 }, new(13, 05) { C = Color.Wood - 30 },
			new(13, 05) { C = Color.Wood - 30 }, new(13, 05) { C = Color.Wood - 30 },
			new(13, 05) { C = Color.Wood - 30 }, new(14, 05) { C = Color.Wood - 30 },
			new(15, 05) { C = Color.Wood - 30 }, new(), new(16, 05) { C = Color.Wood - 30 },
			// dirt road
			new(10, 01) { C = Color.Wood - 30 }, new(12, 00) { C = Color.Wood - 30 },
			new(08, 01) { C = Color.Wood - 30 }, new(12, 00) { C = Color.Wood - 30 },
			new(08, 01) { C = Color.Wood - 30 }, new(08, 01) { C = Color.Wood - 30 },
			new(08, 01) { C = Color.Wood - 30 }, new(11, 01) { C = Color.Wood - 30 },
			new(08, 00) { C = Color.Wood - 30 }, new(09, 00) { C = Color.Wood - 30 },
			new(11, 00) { C = Color.Wood - 30 }, new(14, 01) { C = Color.Wood - 30 },
			new(13, 00) { C = Color.Wood - 30 }, new(15, 00) { C = Color.Wood - 30 },
			new(14, 01) { C = Color.Wood - 30 }, new(09, 01) { C = Color.Wood - 30 },
			new(09, 00) { C = Color.Wood - 30 }, new(09, 00) { C = Color.Wood - 30 },
			new(13, 01) { C = Color.Wood - 30 }, new(12, 00) { C = Color.Wood - 30 },
			new(14, 00) { C = Color.Wood - 30 }, new(12, 00) { C = Color.Wood - 30 },
			new(14, 00) { C = Color.Wood - 30 }, new(14, 00) { C = Color.Wood - 30 },
			new(14, 00) { C = Color.Wood - 30 }, new(12, 01) { C = Color.Wood - 30 },
			new(10, 00) { C = Color.Wood - 30 },

			// stone road
			new(11, 02) { C = Color.Gray + 50 }, new(09, 02) { C = Color.Gray + 50 },
			new(17, 02) { C = Color.Gray + 50 }, new(09, 02) { C = Color.Gray + 50 },
			new(17, 02) { C = Color.Gray + 50 }, new(17, 02) { C = Color.Gray + 50 },
			new(17, 02) { C = Color.Gray + 50 }, new(12, 02) { C = Color.Gray + 50 },
			new(15, 01) { C = Color.Gray + 50 }, new(16, 01) { C = Color.Gray + 50 },
			new(08, 02) { C = Color.Gray + 50 }, new(14, 03) { C = Color.Gray + 50 },
			new(10, 02) { C = Color.Gray + 50 }, new(16, 02) { C = Color.Gray + 50 },
			new(14, 03) { C = Color.Gray + 50 }, new(13, 03) { C = Color.Gray + 50 },
			new(16, 01) { C = Color.Gray + 50 }, new(16, 01) { C = Color.Gray + 50 },
			new(14, 02) { C = Color.Gray + 50 }, new(09, 02) { C = Color.Gray + 50 },
			new(15, 02) { C = Color.Gray + 50 }, new(09, 02) { C = Color.Gray + 50 },
			new(15, 02) { C = Color.Gray + 50 }, new(15, 02) { C = Color.Gray + 50 },
			new(15, 02) { C = Color.Gray + 50 }, new(13, 02) { C = Color.Gray + 50 },
			new(17, 01) { C = Color.Gray + 50 },

			// signs
			new(01, 07) { C = Color.Wood }, new(00, 07) { C = Color.Wood }, new(02, 07) { C = Color.Wood },
			new(), new(),

			// fences
			new(00, 03) { C = Color.Wood - 30 }, new(01, 03) { C = Color.Wood - 30 }, new(02, 03) { C = Color.Wood - 30 },
			new(03, 03) { C = Color.Wood },
			new(05, 03) { C = Color.Gray }, new(06, 03) { C = Color.Gray }, new(00, 04) { C = Color.Gray + 50},
			new(), new(), new(06, 05) { C = Color.Gray }, new(02, 04) { C = Color.Gray },
			new(05, 04) { C = Color.Gray + 50 }, new(03, 04) { C = Color.Gray + 50 },

			// roofs
			new(28, 22) { C = new(165, 75, 75) }, new(29, 22) { C = new(165, 75, 75) }, new(30, 22) { C = new(165, 75, 75) },
			new(31, 22) { C = new(165, 75, 75) }, new(32, 22) { C = new(165, 75, 75) },
			new(37, 22) { C = Color.Wood - 30 },
			new(), new(), new(),

			// walls / floors
			new(33, 22) { C = new(188, 74, 60) }, // brick
			new(34, 22) { C = Color.Wood - 30 }, new(35, 22) { C = Color.Wood - 30 },
			new(36, 22) { C = Color.Wood - 30 },
			new(), new(), new(), new(), new(),

			// doors
			new(40, 22) { C = Color.Wood - 30 }, new(38, 22) { C = Color.Wood - 30 }, new(42, 22) { C = Color.Wood - 30 },
			new(44, 22) { C = Color.Wood - 30 }, new(46, 22) { C = Color.Gray + 50 },
			new(), new(), new(), new(),
			// locked doors
			new(40, 23) { C = Color.Wood - 30 }, new(38, 23) { C = Color.Wood - 30 }, new(42, 23) { C = Color.Wood - 30 },
			new(44, 23) { C = Color.Wood - 30 }, new(46, 23) { C = Color.Gray + 50 },
		};
		public static readonly Dictionary<Point, Point[]> randomTiles = new()
		{
			{ new(05, 00), new Point[] { new(04, 00), new(5, 0), new(6, 0), new(7, 0) } }, // grass
			{ new(00, 02), new Point[] { new(00, 02), new(1, 2), new(2, 2), new(3, 2) } }, // bushes
			{ new(15, 03), new Point[] { new(15, 03), new(16, 03) } }, // flowers
			{ new(05, 01), new Point[] { new(03, 01), new(4, 1), new(5, 1), new(6, 1), new(7, 1) } }, // oak trees
			{ new(01, 01), new Point[] { new(00, 01), new(1, 1), new(2, 1) } }, // pine trees
			{ new(06, 02), new Point[] { new(05, 02), new(6, 2) } }, // cactus
		};
		public static List<Point> SpecialTiles => new() { new(00, 22), new(24, 08) };															  
		public static List<Point> RoofTiles => new() { new(28, 22), new(29, 22), new(30, 22), new(31, 22), new(32, 22), new(37, 22) };
		public static List<Point> DoorTiles => new()
		{
			new(05, 04), new(03, 04), new(00, 04), new(03, 03), new(40, 22), new(38, 22), new(42, 22), new(44, 22), new(46, 22),
			new(40, 23), new(38, 23), new(42, 23), new(44, 23), new(46, 23),
		};
		public static Point Brush { get; set; } = new(1, 22);

		public MapEditor(string uniqueID, CreationDetails creationDetails) : base(uniqueID, creationDetails) { }

		public override void OnHovered() => NavigationPanel.Info.Textbox.Text = "Start a new map edit session.";
		public override void OnLeftClicked()
		{
			Map.DestroyAllSessionObjects();
			Map.CurrentSession = Map.Session.MapEdit;
			Map.DisplayNavigationPanel();

			Map.RawData.Clear();

			Map.CreateUIButtons();
			CreateTab();
			NavigationPanel.Tab.Open("map-editor", "edit brush");
		}

		public static void CreateTab()
		{
			if (Gate.EnterOnceWhile("map-editor-brush", true))
			{
				var list = new ObjectList("brush-tiles", new()
				{
					Position = new(19, 2),
					Height = 1,
					AppearOnTab = "map-editor",
					IsInTab = true,
					IsUI = true
				}, new(9, 11));
				for (int i = 0; i < 20; i++)
				{
					list.Objects.Add(new TileRowValue($"tile-row-{i}", new()
					{
						Position = new(-10, 0),
						Height = 1,
						AppearOnTab = "map-editor",
						IsInTab = true,
						Name = i.ToString()
					})); ;
				}

				new ColorSamples("brush-color-samples", new()
				{
					Position = new(28, 2),
					Height = 1,
					Name = "samples",
					AppearOnTab = "map-editor",
					IsInTab = true,
					IsLeftClickable = true,
					IsUI = true
				}, 12, true);
				new ColorPick("brush-r", new()
				{
					Position = new(29, 2) { C = Color.Red },
					Height = 1,
					Name = "r",
					AppearOnTab = "map-editor",
					IsInTab = true,
					IsLeftClickable = true,
					IsUI = true
				}, 12, true);
				new ColorPick("brush-g", new()
				{
					Position = new(30, 2) { C = Color.Green },
					Height = 1,
					Name = "g",
					AppearOnTab = "map-editor",
					IsInTab = true,
					IsLeftClickable = true,
					IsUI = true
				}, 12, true);
				new ColorPick("brush-b", new()
				{
					Position = new(31, 2) { C = Color.Blue },
					Height = 1,
					Name = "b",
					AppearOnTab = "map-editor",
					IsInTab = true,
					IsLeftClickable = true,
					IsUI = true
				}, 12, true);
			}
		}
		public static void EditCurrentTile()
		{
			var LMB = Mouse.ButtonIsPressed(Mouse.Button.Left);
			var clickPos = Map.ScreenToMapPosition(LMB ? Base.LeftClickPosition : Base.RightClickPosition);
			var mousePos = Screen.GetCellAtCursorPosition();
			var pos = Map.ScreenToMapPosition(mousePos);
			var hoveredTile = Screen.GetCellIndexesAtPosition(mousePos, SwitchHeight.BrushHeight);

			// sign hardcoding
			if (TileIsSign(hoveredTile))
			{
				// overwrite / delete
				var heights = objects.ContainsKey(pos) ? objects[pos] : new();
				for (int i = 0; i < heights.Count; i++)
					if (heights[i].IsUI == false && (heights[i].Height == SwitchHeight.BrushHeight || heights[i] is Sign))
						heights[i].Destroy();
			}
			if (TileIsSign(Brush) && LMB)
			{
				var sign = new Sign($"{pos}", new CreationDetails()
				{
					Name = "Sign",
					Position = new(pos.X, pos.Y) { C = Brush.C },
					Height = SwitchHeight.BrushHeight,
					TileIndexes = new Point[] { Brush },
				});
				sign.OnHovered();
			}

			if (TileIsSign(Brush) == false)
			{
				var isSpecial = SpecialTiles.Contains(Brush);
				if (Keyboard.KeyIsPressed(Keyboard.Key.LeftShift) && clickPos != pos && isSpecial == false)
				{
					var dirY = pos.Y > clickPos.Y ? 1 : -1;
					var dirX = pos.X > clickPos.X ? 1 : -1;
					for (double y = clickPos.Y; y != pos.Y + dirY; y += dirY)
						for (double x = clickPos.X; x != pos.X + dirX; x += dirX)
						{
							var p = new Point(x, y);
							if (Map.RawData.ContainsKey(p) == false)
								Map.RawData[p] = new Point[4];
							Map.RawData[p][SwitchHeight.BrushHeight] = LMB ? GetRandomBrushTile() : new(0, 0);
						}
				}
				if (Map.RawData.ContainsKey(pos) == false)
					Map.RawData[pos] = new Point[4];
				Map.RawData[pos][isSpecial ? 3 : SwitchHeight.BrushHeight] = LMB ? GetRandomBrushTile() : new(0, 0);
			}

			Screen.Display();

			bool TileIsSign(Point p) => p.Y == 7 && (p.X == 0 || p.X == 1 || p.X == 2);
		}
		public static void PickCurrentTile()
		{
			if (Map.CurrentSession != Map.Session.MapEdit) return;
			var mousePos = Screen.GetCellAtCursorPosition();
			var brushTilesAreHovered = ObjectList.Lists["brush-tiles"].IsHovered();
			if (brushTilesAreHovered == false && Map.IsHovered() == false) return;
			var indexes = Screen.GetCellIndexesAtPosition(mousePos, brushTilesAreHovered ? 1 : SwitchHeight.BrushHeight);
			if (indexes == new Point(0, 0)) return;

			Brush = indexes;
			Screen.EditCell(new Point(0, 4), Brush, 1, Brush.C);
			ColorPick.UpdateBrushColorPickers();
			Screen.Display();
		}

		public override void OnMouseButtonRelease(Mouse.Button button)
		{
			base.OnMouseButtonRelease(button);
			var mousePos = Screen.GetCellAtCursorPosition();
			if (Map.CurrentSession == Map.Session.MapEdit && mousePos.X < 18)
				NavigationPanel.Tab.Open("map-editor", "edit brush");
		}
		public static Point GetRandomBrushTile()
		{
			if (randomTiles.ContainsKey(Brush) == false) return Brush;

			var randomTile = randomTiles[Brush][(int)Probability.Randomize(new(0, randomTiles[Brush].Length - 1))];
			randomTile.C = Brush.C;
			return randomTile;
		}
	}
}
