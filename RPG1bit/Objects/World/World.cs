using Newtonsoft.Json;
using SMPL.Components;
using SMPL.Data;
using SMPL.Gear;
using System.Collections.Generic;
using System.IO;

namespace RPG1bit
{
	public class World : Thing
	{
		public enum Session { None, Single, Multi, WorldEdit }

		public static Point TileBarrier => new(0, 22);
		public static Point TilePlayer => new(25, 0);

		public static string CurrentWorldName { get; private set; }
		public static bool IsShowingRoofs { get; set; } = true;

		public static Point CameraPosition { get; set; }
		public static Session CurrentSession { get; set; }

		private static bool startingSingle;

		public World(string uniqueID) : base(uniqueID)
		{
			Assets.Event.Subscribe.LoadEnd(uniqueID);
			Game.Event.Subscribe.Update(uniqueID);
		}
		public override void OnGameUpdate()
		{
			if (startingSingle == false || ChunkManager.HasQueuedLoad())
				return;

			startingSingle = false;
			WorldObjectManager.InitializeObjects();
		}
		public override void OnAssetsLoadEnd()
		{
			if (Assets.ValuesAreLoaded("world-name") && startingSingle == false && Assets.ValuesAreLoaded("chunk") == false)
			{
				Assets.UnloadValues("world-name");

				ChunkManager.DestroyAllChunks(true, true);

				var chunks = Directory.GetFiles($"worlds\\{CurrentWorldName}");
				for (int i = 0; i < chunks.Length; i++)
					if (chunks[i].Contains("worlddata") == false)
						File.Copy(chunks[i], $"cache\\{Path.GetFileName(chunks[i])}");

				ChunkManager.ScheduleLoadVisibleChunks();
				ChunkManager.UpdateChunks();

				if (CurrentSession == Session.Single)
					startingSingle = true;

				Screen.ScheduleDisplay();
			}

			if (CurrentSession == Session.WorldEdit)
			{
				if (Assets.ValuesAreLoaded("camera-position"))
				{
					CameraPosition = Text.FromJSON<Point>(Assets.GetValue("camera-position"));
					Assets.UnloadValues("camera-position");
				}
				WorldEditor.CreateTab();
				NavigationPanel.Tab.Open("world-editor", "edit brush");
			}
		}

		public static void CreateUIButtons()
		{
			if (Gate.EnterOnceWhile("camera-controls"))
			{
				new MoveCamera("camera", new()
				{
					Name = "camera",
					Position = new(0, 0),
					TileIndexes = new Point[] { new(19, 14) { Color = Color.Gray } },
					Height = 1,
					IsUI = true,
					IsKeptBetweenSessions = true
				});
			}
			if (CurrentSession == Session.Single)
			{
				new ItemSlot("slot-head", new()
				{
					Name = "On your head:",
					Position = new(0, 5),
					TileIndexes = new Point[] { new(5, 22) { Color = Color.Gray } },
					Height = 1,
					IsUI = true
				});
				new ItemSlot("slot-body", new()
				{
					Name = "On your body:",
					Position = new(0, 6),
					TileIndexes = new Point[] { new(6, 22) { Color = Color.Gray } },
					Height = 1,
					IsUI = true
				});
				new ItemSlot("slot-feet", new()
				{
					Name = "On your feet:",
					Position = new(0, 7),
					TileIndexes = new Point[] { new(7, 22) { Color = Color.Gray } },
					Height = 1,
					IsUI = true
				});

				new ItemSlot("hand-right", new()
				{
					Name = "In your right hand:",
					Position = new(0, 2),
					TileIndexes = new Point[] { new(9, 22) { Color = Color.Gray } },
					Height = 1,
					IsUI = true
				});
				new ItemSlot("hand-left", new()
				{
					Name = "In your left hand:",
					Position = new(0, 3),
					TileIndexes = new Point[] { new(8, 22) { Color = Color.Gray } },
					Height = 1,
					IsUI = true
				});

				new ItemSlot("carry-back", new()
				{
					Name = "On your back:",
					Position = new(0, 9),
					TileIndexes = new Point[] { new(10, 22) { Color = Color.Gray } },
					Height = 1,
					IsUI = true
				});
				new ItemSlot("carry-waist", new()
				{
					Name = "On your waist:",
					Position = new(0, 10),
					TileIndexes = new Point[] { new(11, 22) { Color = Color.Gray } },
					Height = 1,
					IsUI = true
				});

				for (int i = 0; i < ItemPile.MAX_COUNT; i++)
				{
					var slot = new ItemSlot($"ground-slot-{i}", new GameObject.CreationDetails()
					{
						Name = "On the ground:",
						Position = new(10 + i, 0),
						TileIndexes = new Point[] { new(7, 23) { Color = Color.Brown } },
						Height = 0,
						IsUI = true
					});
					Screen.EditCell(slot.Position, new(), 1, new());
				}
			}
			if (CurrentSession == Session.WorldEdit)
			{
				new SwitchHeight("brush-height", new()
				{
					Name = "height-up",
					Position = new(0, 5),
					Height = 1,
					IsUI = true,
					IsLeftClickable = true,
				});
				new ShowRoofs("show-roofs", new()
				{
					Name = "show-roofs",
					Position = new(0, 8) { Color = Color.Gray },
					Height = 1,
					IsUI = true,
					IsLeftClickable = true,
				});
			}
		}
		public static void Display()
		{
			for (double y = CameraPosition.Y - 8; y < CameraPosition.Y + 9; y++)
				for (double x = CameraPosition.X - 8; x < CameraPosition.X + 9; x++)
				{
					var pos = new Point(x, y);
					var chunkId = $"chunk-{ChunkManager.GetChunkCenterFromPosition(pos)}";
					var chunkExists = UniqueIDsExists(chunkId);
					var chunk = chunkExists ? (Chunk)PickByUniqueID(chunkId) : default;

					if (chunkExists == false || chunk.Data.ContainsKey(pos) == false)
					{
						for (int z = 0; z < 4; z++)
							Screen.EditCell(WorldToScreenPosition(pos), new(), z, new());
						continue;
					}

					var tilesInCoord = 0;
					for (int z = 0; z < 4; z++)
					{
						var color = chunk.Data[pos][z].Color;
						if (chunk.Data[pos][z] != new Point(0, 0)) tilesInCoord++;
						if ((chunk.Data[pos][z] == TilePlayer || chunk.Data[pos][z] == TileBarrier) &&
							CurrentSession == Session.Single)
							color = new();

						if (IsShowingRoofs == false && chunk.Data[pos][z].Color.A == 254)
							color.A = 100;

						Screen.EditCell(WorldToScreenPosition(pos), chunk.Data[pos][z], z, color);
					}
					if (tilesInCoord == 0)
						chunk.Data.Remove(pos);
				}
		}
		public static void DisplayNavigationPanel()
		{
			for (int x = 0; x < 18; x++)
			{
				Screen.EditCell(new(x, 0), new(1, 22), 0, Color.Brown / 2);
				Screen.EditCell(new(x, 0), new(4, 22), 1, Color.Brown);
				Screen.EditCell(new(x, 0), new(), 2, new());
				Screen.EditCell(new(x, 0), new(), 3, new());
			}
			for (int y = 0; y < 18; y++)
			{
				Screen.EditCell(new(0, y), new(1, 22), 0, Color.Brown / 2);
				Screen.EditCell(new(0, y), new(4, 22), 1, Color.Brown);
				Screen.EditCell(new(0, y), new(), 2, new());
				Screen.EditCell(new(0, y), new(), 3, new());
			}
			if (CurrentSession == Session.WorldEdit)
			{
				Screen.EditCell(new(0, 4), WorldEditor.Brush, 1, WorldEditor.Brush.Color);
				Screen.EditCell(new(0, 7), new(6, 23), 1, Color.Gray);

				Screen.EditCell(new(9, 0), new(41, 13), 1, Color.Gray);
				Screen.EditCell(new(10, 0), new(43, 13), 1, Color.Gray);
				Screen.EditCell(new(11, 0), new(42, 13), 1, Color.Gray);
			}
			else if (CurrentSession == Session.Single)
			{
				Screen.EditCell(new(4, 0), new(4, 23), 1, Color.Gray);
				Screen.EditCell(new(5, 0), new(5, 23), 1, Color.Gray);
				Screen.EditCell(new(6, 0), new(42, 12), 1, Color.Gray);

				for (int i = 0; i < 8; i++)
					Screen.EditCell(new(10 + i, 0), new(), 1, new());
			}
		}
		public static void LoadWorld(Session session, string name)
		{
			GameObject.DestroyAllSessionObjects();
			ChunkManager.DestroyAllChunks(true, true);
			NavigationPanel.Info.Textbox.Text = GameObject.descriptions[new(0, 23)];

			CurrentSession = session;
			CurrentWorldName = name;

			DisplayNavigationPanel();
			CreateUIButtons();

			Assets.Load(Assets.Type.DataSlot, $"worlds\\{name}\\{name}.worlddata");
		}

		public static bool IsHovered()
		{
			var mousePos = Screen.GetCellAtCursorPosition();
			return mousePos.X > 0 && mousePos.Y > 0 && mousePos.X < 18 && mousePos.Y < 18;
		}
		public static Point WorldToScreenPosition(Point worldPos)
		{
			return worldPos - CameraPosition + new Point(9, 9);
		}
		public static Point ScreenToWorldPosition(Point screenPos)
		{
			return CameraPosition + (screenPos - new Point(9, 9));
		}
		public static bool TileHasRoof(Point worldPos)
		{
			for (int i = 0; i < 3; i++)
			{
				var tile = ChunkManager.GetTile(worldPos, i);
				if (tile != default && tile != new Point(0, 0) && tile.Color.A != 255)
					return true;
			}
			return false;
		}
		public static bool PositionHasWaterAsHighest(Point worldPos)
		{
			var highest = new Point();
			var hasWater = false;
			for (int i = 0; i < 3; i++)
			{
				var tile = ChunkManager.GetTile(worldPos, i);
				if (tile != new Point())
				{
					if (WorldEditor.Tiles["water"].Contains(tile))
						hasWater = true;
					highest = tile;
				}
			}
			var isBoat = hasWater && WorldEditor.Tiles[nameof(Boat)].Contains(highest);
			return WorldEditor.Tiles["water"].Contains(highest) || isBoat;
		}
	}
}
