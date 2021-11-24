using SMPL.Data;
using SMPL.Gear;
using System.Collections.Generic;

namespace RPG1bit
{
	public class WorldEditor : GameObject
	{
		public static readonly Point[] TileList = new Point[]
		{
			new(01, 22) { Color = Color.White },				// background
			new(00, 22) { Color = Color.Red },				// barrier
			new(25, 00) { Color = Color.White },				// player spawn tile
			new(), new(), new(), new(), new(), new(),
			new(05, 00) { Color = Color.Grass },				// grass
			new(00, 02) { Color = Color.Grass },				// bushes
			new(15, 03) { Color = Color.Yellow },			// flowers
			new(05, 01) { Color = Color.LeafSummer },		// oak trees
			new(01, 01) { Color = Color.LeafSummer },		// pine trees
			new(07, 02) { Color = Color.Grass },				// palm
			new(06, 02) { Color = Color.Grass },				// cactus
			new(04, 02) { Color = Color.Gray + 50},			// rocks
			new(),
			// water
			new(12, 3) { Color = Color.Water }, new(8, 03) { Color = Color.Water }, new(07, 4) { Color = Color.Water },
			new(10, 4) { Color = Color.Water }, new(13, 4) { Color = Color.Water }, new(14, 4) { Color = Color.Water },
			new(14, 4) { Color = Color.Water }, new(14, 4) { Color = Color.Water }, new(7, 05) { Color = Color.Water },
			new(11, 3) { Color = Color.Water }, new(07, 3) { Color = Color.Water }, new(09, 3) { Color = Color.Water },
			new(11, 4) { Color = Color.Water }, new(08, 5) { Color = Color.Water }, new(14, 4) { Color = Color.Water },
			new(14, 4) { Color = Color.Water }, new(14, 4) { Color = Color.Water }, new(09, 5) { Color = Color.Water },
			new(09, 4) { Color = Color.Water }, new(10, 3) { Color = Color.Water }, new(08, 4) { Color = Color.Water },
			new(12, 4) { Color = Color.Water }, new(11, 5) { Color = Color.Water }, new(14, 4) { Color = Color.Water },
			new(14, 4) { Color = Color.Water }, new(14, 4) { Color = Color.Water }, new(10, 5) { Color = Color.Water },
			// bridge
			new(12, 05) { Color = Color.Wood - 30 }, new(13, 05) { Color = Color.Wood - 30 },
			new(13, 05) { Color = Color.Wood - 30 }, new(13, 05) { Color = Color.Wood - 30 },
			new(13, 05) { Color = Color.Wood - 30 }, new(14, 05) { Color = Color.Wood - 30 },
			new(15, 05) { Color = Color.Wood - 30 }, new(), new(16, 05) { Color = Color.Wood - 30 },
			// dirt road
			new(10, 01) { Color = Color.Wood - 30 }, new(12, 00) { Color = Color.Wood - 30 },
			new(08, 01) { Color = Color.Wood - 30 }, new(12, 00) { Color = Color.Wood - 30 },
			new(08, 01) { Color = Color.Wood - 30 }, new(08, 01) { Color = Color.Wood - 30 },
			new(08, 01) { Color = Color.Wood - 30 }, new(11, 01) { Color = Color.Wood - 30 },
			new(08, 00) { Color = Color.Wood - 30 }, new(09, 00) { Color = Color.Wood - 30 },
			new(11, 00) { Color = Color.Wood - 30 }, new(14, 01) { Color = Color.Wood - 30 },
			new(13, 00) { Color = Color.Wood - 30 }, new(15, 00) { Color = Color.Wood - 30 },
			new(14, 01) { Color = Color.Wood - 30 }, new(09, 01) { Color = Color.Wood - 30 },
			new(09, 00) { Color = Color.Wood - 30 }, new(09, 00) { Color = Color.Wood - 30 },
			new(13, 01) { Color = Color.Wood - 30 }, new(12, 00) { Color = Color.Wood - 30 },
			new(14, 00) { Color = Color.Wood - 30 }, new(12, 00) { Color = Color.Wood - 30 },
			new(14, 00) { Color = Color.Wood - 30 }, new(14, 00) { Color = Color.Wood - 30 },
			new(14, 00) { Color = Color.Wood - 30 }, new(12, 01) { Color = Color.Wood - 30 },
			new(10, 00) { Color = Color.Wood - 30 },

			// stone road
			new(11, 02) { Color = Color.Gray + 50 }, new(09, 02) { Color = Color.Gray + 50 },
			new(17, 02) { Color = Color.Gray + 50 }, new(09, 02) { Color = Color.Gray + 50 },
			new(17, 02) { Color = Color.Gray + 50 }, new(17, 02) { Color = Color.Gray + 50 },
			new(17, 02) { Color = Color.Gray + 50 }, new(12, 02) { Color = Color.Gray + 50 },
			new(15, 01) { Color = Color.Gray + 50 }, new(16, 01) { Color = Color.Gray + 50 },
			new(08, 02) { Color = Color.Gray + 50 }, new(14, 03) { Color = Color.Gray + 50 },
			new(10, 02) { Color = Color.Gray + 50 }, new(16, 02) { Color = Color.Gray + 50 },
			new(14, 03) { Color = Color.Gray + 50 }, new(13, 03) { Color = Color.Gray + 50 },
			new(16, 01) { Color = Color.Gray + 50 }, new(16, 01) { Color = Color.Gray + 50 },
			new(14, 02) { Color = Color.Gray + 50 }, new(09, 02) { Color = Color.Gray + 50 },
			new(15, 02) { Color = Color.Gray + 50 }, new(09, 02) { Color = Color.Gray + 50 },
			new(15, 02) { Color = Color.Gray + 50 }, new(15, 02) { Color = Color.Gray + 50 },
			new(15, 02) { Color = Color.Gray + 50 }, new(13, 02) { Color = Color.Gray + 50 },
			new(17, 01) { Color = Color.Gray + 50 },

			// signs
			new(01, 07) { Color = Color.Wood }, new(0, 7) { Color = Color.Wood }, new(2, 7) { Color = Color.Wood }, new(01, 08) { Color = Color.Wood },
			new(00, 08) { Color = Color.Wood },
			new(), new(), new(), new(),

			// fences
			new(00, 03) { Color = Color.Wood - 30 }, new(01, 03) { Color = Color.Wood - 30 }, new(02, 03) { Color = Color.Wood - 30 },
			new(03, 03) { Color = Color.Wood },
			new(), new(), new(), new(), new(),
			new(05, 03) { Color = Color.Gray }, new(06, 03) { Color = Color.Gray }, new(00, 04) { Color = Color.Gray + 50},
			new(), new(), new(06, 05) { Color = Color.Gray }, new(02, 04) { Color = Color.Gray },
			new(05, 04) { Color = Color.Gray + 50 }, new(03, 04) { Color = Color.Gray + 50 },

			// roofs
			new(28, 22) { Color = Color.Brick - 30 }, new(29, 22) { Color = Color.Brick - 30 }, new(30, 22) { Color = Color.Brick - 30 },
			new(31, 22) { Color = Color.Brick }, new(32, 22) { Color = Color.Brick },
			new(), new(), new(), new(),
			new(28, 23) { Color = Color.Brick - 30 }, new(29, 23) { Color = Color.Brick - 30 }, new(30, 23) { Color = Color.Brick - 30 },
			new(30, 23) { Color = Color.Brick - 30 }, new(30, 23) { Color = Color.Brick - 30 },
			new(), new(), new(), new(),

			// walls / floors
			new(33, 22) { Color = Color.Brick }, new(24, 23) { Color = Color.Brick }, new(25, 23) { Color = Color.Brick },
			new(26, 23) { Color = Color.Brick }, new(33, 22) { Color = Color.Brick },
			new(34, 22) { Color = Color.Wood - 30 }, new(35, 22) { Color = Color.Wood - 30 },
			new(36, 22) { Color = Color.Wood - 30 }, new(37, 23) { Color = Color.Wood - 30 },

			// windows
			new(31, 23), new(32, 23), new(33, 23), new(34, 23), new(35, 23), new(36, 23),
			new(), new(), new(),

			// doors
			new(40, 22) { Color = Color.Wood - 30 }, new(38, 22) { Color = Color.Wood - 30 }, new(42, 22) { Color = Color.Wood - 30 },
			new(44, 22) { Color = Color.Wood - 30 }, new(46, 22) { Color = Color.Gray + 50 },
			new(), new(), new(), new(),
			// locked doors
			new(40, 23) { Color = Color.Wood - 30 }, new(38, 23) { Color = Color.Wood - 30 }, new(42, 23) { Color = Color.Wood - 30 },
			new(44, 23) { Color = Color.Wood - 30 }, new(46, 23) { Color = Color.Gray + 50 },
			new(), new(), new(), new(),

			// storages
			new(21, 23) { Color = Color.Wood - 30 }, new(21, 24) { Color = Color.Wood - 30 }, // chest
			new(05, 07) { Color = Color.Wood - 30 }, new(07, 07) { Color = Color.Wood - 30 }, // drawer
			new(), new(), new(), new(), new(),

			// interiors
			new(03, 07) { Color = Color.Wood - 30 }, new(04, 07) { Color = Color.Wood - 30 }, // shelf
			new(09, 07) { Color = Color.Wood - 30 }, new(10, 07) { Color = Color.Wood - 30 }, // counter
			new(), new(14, 10) { Color = Color.Fire },
			new(07, 08) { Color = Color.Fire }, new(8, 8) { Color = Color.Fire }, new(9, 8) { Color = Color.Fire }, // fireplace

			// tables
			new(11, 07) { Color = Color.Wood - 30 }, new(12, 7) { Color = Color.Wood - 30 }, new(13, 7) { Color = Color.Wood - 30 },
			new(14, 07) { Color = Color.Wood - 30 }, new(15, 7) { Color = Color.Wood - 30 }, new(16, 7) { Color = Color.Wood - 30 },
			new(13, 08) { Color = Color.Wood - 30 }, new(), new(),
			new(10, 08) { Color = Color.Wood - 30 }, new(11, 8) { Color = Color.Wood - 30 }, new(12, 8) { Color = Color.Wood - 30 }, // chair
			new(), new(), new(), new(13, 9) { Color = Color.Wood - 30 }, new(), new(),
			new(04, 08) { Color = Color.Wood + 70 }, new(5, 8) { Color = Color.Wood + 70 }, new(6, 8) { Color = Color.Wood + 70 }, // bed
			new(), new(), new(), new(13, 10) { Color = Color.Wood - 30 },
			new(), new(), 

			// boats
			new(08, 19) { Color = Color.Wood - 30 }, new(09, 19) { Color = Color.Wood - 30 }, new(10, 19) { Color = Color.Wood - 30 },
			new(11, 19) { Color = Color.Wood - 30 },
			new(), new(), new(),

			// cart
			new(08, 18) { Color = Color.Wood - 30 }, new(9, 18) { Color = Color.Wood - 30 },

			// wheelbarrow
			new(06, 16) { Color = Color.Wood - 30 }, new(07, 16) { Color = Color.Wood - 30 }, new(08, 16) { Color = Color.Wood - 30 },
			new(09, 16) { Color = Color.Wood - 30 },
			new(),

			// chariot
			new(06, 17) { Color = Color.Wood - 30 }, new(07, 17) { Color = Color.Wood - 30 }, new(08, 17) { Color = Color.Wood - 30 },
			new(09, 17) { Color = Color.Wood - 30 },
		};
		public static readonly Dictionary<Point, Point[]> RandomTiles = new()
		{
			{ new(05, 00), new Point[] { new(04, 00), new(5, 0), new(6, 0), new(7, 0) } }, // grass
			{ new(00, 02), new Point[] { new(00, 02), new(1, 2), new(2, 2), new(3, 2) } }, // bushes
			{ new(15, 03), new Point[] { new(15, 03), new(16, 03) } }, // flowers
			{ new(05, 01), new Point[] { new(03, 01), new(4, 1), new(5, 1), new(6, 1), new(7, 1) } }, // oak trees
			{ new(01, 01), new Point[] { new(00, 01), new(1, 1), new(2, 1) } }, // pine trees
			{ new(06, 02), new Point[] { new(05, 02), new(6, 2) } }, // cactus
		};
		public static readonly Dictionary<string, List<Point>> Tiles = new()
		{
			{ "special", new() { new(0, 22), new(25, 0) } },
			{ "water", new() { new(7, 3), new(8, 3), new(9, 3), new(10, 3), new(11, 3), new(12, 3), new(7, 4), new(8, 4), new(9, 4),
				new(10, 4), new(11, 4), new(12, 4), new(13, 4), new(14, 4), new(7, 5), new(8, 5), new(9, 5), new(10, 5), new(11, 5) } },
			{ nameof(Door), new() { new(5, 4), new(3, 4), new(0, 4), new(3, 3), new(40, 22), new(38, 22), new(42, 22), new(44, 22),
				new(46, 22), new(40, 23), new(38, 23), new(42, 23), new(44, 23), new(46, 23) } },
			{ nameof(Boat), new() { new(8, 19), new(9, 19), new(10, 19), new(11, 19), new(12, 23), new(13, 23), new(14, 23),
				new(15, 23) } },
			{ nameof(Storage), new() {
					new(21, 23), new(22, 23), new(21, 24), new(22, 24), // chest
					new(05, 07), new(06, 07), // big drawer
					new(07, 07), new(08, 07), // small drawer
					new(08, 18), new(09, 18), // cart
					new(06, 16), new(07, 16), new(08, 16), new(09, 16), // wheelbarrow
					new(06, 17), new(07, 17), new(08, 17), new(09, 17) // chariot
			} },
			{ nameof(Mount), new() {
					new(27, 07), new(28, 07), // horse
			} },
		};
		public static Point Brush { get; set; } = new(1, 22);

		public WorldEditor(string uniqueID, CreationDetails creationDetails) : base(uniqueID, creationDetails) { }

		public override void OnHovered() => NavigationPanel.Info.Textbox.Text = "[LEFT CLICK] to start a new\n\t world edit session";
		public override void OnLeftClicked()
		{
			DestroyAllSessionObjects();
			World.CurrentSession = World.Session.WorldEdit;
			World.DisplayNavigationPanel();

			World.CameraPosition = new();
			ChunkManager.DestroyAllChunks(true, true);

			World.CreateUIButtons();
			CreateTab();
			NavigationPanel.Tab.Open("world-editor", "edit brush");
		}

		public static void CreateTab()
		{
			if (Gate.EnterOnceWhile("world-editor-brush", true))
			{
				var list = new GameObjectList("brush-tiles", new()
				{
					Position = new(19, 2),
					Height = 1,
					AppearOnTab = "world-editor",
					IsInTab = true,
					IsUI = true,
					IsKeptBetweenSessions = true,
				}, new(9, 11));
				for (int i = 0; i < 40; i++)
				{
					list.Objects.Add(new TileRowValue($"tile-row-{i}", new()
					{
						Position = new(-20, 0),
						Height = 1,
						AppearOnTab = "world-editor",
						IsInTab = true,
						IsUI = true,
						Name = i.ToString(),
						IsKeptBetweenSessions = true,
					})); ;
				}

				new ColorSamples("brush-color-samples", new()
				{
					Position = new(28, 2),
					Height = 1,
					Name = "samples",
					AppearOnTab = "world-editor",
					IsInTab = true,
					IsLeftClickable = true,
					IsUI = true,
					IsKeptBetweenSessions = true,
				}, 12, true);
				new ColorPick("brush-r", new()
				{
					Position = new(29, 2) { Color = Color.Red },
					Height = 1,
					Name = "r",
					AppearOnTab = "world-editor",
					IsInTab = true,
					IsLeftClickable = true,
					IsUI = true,
					IsKeptBetweenSessions = true,
				}, 12, true);
				new ColorPick("brush-g", new()
				{
					Position = new(30, 2) { Color = Color.Green },
					Height = 1,
					Name = "g",
					AppearOnTab = "world-editor",
					IsInTab = true,
					IsLeftClickable = true,
					IsUI = true,
					IsKeptBetweenSessions = true,
				}, 12, true);
				new ColorPick("brush-b", new()
				{
					Position = new(31, 2) { Color = Color.Blue },
					Height = 1,
					Name = "b",
					AppearOnTab = "world-editor",
					IsInTab = true,
					IsLeftClickable = true,
					IsUI = true,
					IsKeptBetweenSessions = true,
				}, 12, true);
			}
		}
		public static void EditCurrentTile()
		{
			var LMB = Mouse.ButtonIsPressed(Mouse.Button.Left);
			var clickPos = World.ScreenToWorldPosition(LMB ? Base.LeftClickPosition : Base.RightClickPosition);
			var mousePos = Screen.GetCellAtCursorPosition();
			var pos = World.ScreenToWorldPosition(mousePos);
			var hoveredTile = Screen.GetCellIndexesAtPosition(mousePos, SwitchHeight.BrushHeight);

			// sign hardcoding
			if (TileIsSign(hoveredTile))
			{
				// overwrite / delete
				var signs = PickByTag(nameof(Sign));
				for (int i = 0; i < signs.Length; i++)
				{
					var sign = (Sign)signs[i];
					if (sign.Position != pos || sign.Height != SwitchHeight.BrushHeight)
						continue;
					ChunkManager.GetOrCreateChunk(pos).ObjectUIDs.Remove(sign.UniqueID);
					sign.Destroy();
				}
			}
			if (TileIsSign(Brush) && LMB)
			{
				var sign = new Sign($"{pos}", new CreationDetails()
				{
					Name = "Sign",
					Position = new(pos.X, pos.Y) { Color = Brush.Color },
					Height = SwitchHeight.BrushHeight,
					TileIndexes = new Point[] { Brush },
				});
				sign.OnHovered();
				ChunkManager.GetOrCreateChunk(pos).ObjectUIDs.Add(sign.UniqueID);
			}

			if (TileIsSign(Brush) == false)
			{
				var isSpecial = Tiles["special"].Contains(Brush);
				if (Keyboard.KeyIsPressed(Keyboard.Key.ShiftLeft) && clickPos != pos && isSpecial == false)
				{
					var dirY = pos.Y > clickPos.Y ? 1 : -1;
					var dirX = pos.X > clickPos.X ? 1 : -1;
					for (double y = clickPos.Y; y != pos.Y + dirY; y += dirY)
						for (double x = clickPos.X; x != pos.X + dirX; x += dirX)
							ChunkManager.SetTile(new Point(x, y), SwitchHeight.BrushHeight, LMB ? GetRandomBrushTile() : new(0, 0));
				}
				ChunkManager.SetTile(pos, isSpecial ? 3 : SwitchHeight.BrushHeight, LMB ? GetRandomBrushTile() : new(0, 0));
			}

			Screen.ScheduleDisplay();

			bool TileIsSign(Point p) => p.Y == 7 && (p.X == 0 || p.X == 1 || p.X == 2);
		}
		public static void PickCurrentTile()
		{
			if (World.CurrentSession != World.Session.WorldEdit) return;
			var mousePos = Screen.GetCellAtCursorPosition();
			var brushTilesAreHovered = GameObjectList.Lists["brush-tiles"].IsHovered();
			if (brushTilesAreHovered == false && World.IsHovered() == false) return;
			var indexes = Screen.GetCellIndexesAtPosition(mousePos, brushTilesAreHovered ? 1 : SwitchHeight.BrushHeight);
			if (indexes == new Point(0, 0)) return;

			Brush = indexes;
			Screen.EditCell(new Point(0, 4), Brush, 1, Brush.Color);
			ColorPick.UpdateBrushColorPickers();
			Screen.ScheduleDisplay();
		}

		public override void OnMouseButtonRelease(Mouse.Button button)
		{
			base.OnMouseButtonRelease(button);
			var mousePos = Screen.GetCellAtCursorPosition();
			if (World.CurrentSession == World.Session.WorldEdit && mousePos.X < 18)
				NavigationPanel.Tab.Open("world-editor", "edit brush");
		}
		public static Point GetRandomBrushTile()
		{
			Brush = new(Brush.X, Brush.Y)
				{ Color = new(Brush.Color.R, Brush.Color.G, Brush.Color.B, Keyboard.KeyIsPressed(Keyboard.Key.AltLeft) ? 254 : 255) };
			if (RandomTiles.ContainsKey(Brush) == false)
				return Brush;

			var randomTile = RandomTiles[Brush][(int)Probability.Randomize(new(0, RandomTiles[Brush].Length - 1))];
			randomTile.Color = Brush.Color;
			return randomTile;
		}
	}
}
