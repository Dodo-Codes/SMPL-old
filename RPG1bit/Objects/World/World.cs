using Newtonsoft.Json;
using SMPL.Components;
using SMPL.Data;
using SMPL.Gear;
using System.Collections.Generic;

namespace RPG1bit
{
	public class World : Thing
	{
		public enum Session { None, Single, Multi, WorldEdit }

		public static Point TileBarrier => new(0, 22);
		public static Point TilePlayer => new(24, 8);

		public static Dictionary<Point, Point[]> RawData { get; set; } = new();
		public static string CurrentWorldName { get; private set; }
		public static bool IsShowingRoofs { get; set; } = true;

		public static Point CameraPosition { get; set; }
		public static Session CurrentSession { get; set; }

		public World(string uniqueID) : base(uniqueID)
		{
			Assets.Event.Subscribe.LoadEnd(uniqueID);
		}
		public override void OnAssetsLoadEnd()
		{
			if (Assets.ValuesAreLoaded("world-data"))
			{
				var worldData = Text.FromJSON<Dictionary<string, string>>(Assets.GetValue("world-data"));
				var signData = new List<CompactSignData>();
				if (Assets.ValuesAreLoaded("signs")) signData = Text.FromJSON<List<CompactSignData>>(Assets.GetValue("signs"));
				if (worldData != default)
					foreach (var kvp in worldData)
						RawData[Text.FromJSON<Point>(kvp.Key)] = Text.FromJSON<Point[]>(kvp.Value);
				if (UniqueIDsExits(nameof(Player)) == false)
					CameraPosition = Text.FromJSON<Point>(Assets.GetValue("camera-position"));

				for (int i = 0; i < signData.Count; i++)
				{
					new Sign($"{signData[i].P}-{i}-{signData[i].P.C.R}", new()
					{
						Position = signData[i].P,
						TileIndexes = new Point[] { signData[i].I },
						Height = (int)signData[i].P.C.R,
					})
					{ Text = signData[i].T };
				}

				if (CurrentSession == Session.Single && worldData != default)
				{
					WorldObjectManager.InitializeObjects();
					NavigationPanel.Tab.Close();
					Assets.UnloadAllValues();

					if (Gate.EnterOnceWhile("create-item-info-tab", true))
					{
						new ItemStats("strength", new()
						{
							Position = new(19, 9),
							Height = 1,
							TileIndexes = new Point[] { new() },
							IsInTab = true,
							AppearOnTab = "item-info",
							IsUI = true,
							Name = "positives",
							IsKeptBetweenSessions = true,
						});
						new ItemStats("weakness", new()
						{
							Position = new(19, 12),
							Height = 1,
							TileIndexes = new Point[] { new() },
							IsInTab = true,
							AppearOnTab = "item-info",
							IsUI = true,
							Name = "negatives",
							IsKeptBetweenSessions = true,
						});
						new ItemSlotInfo("able-to-carry-in", new()
						{
							Position = new(31, 2),
							Height = 1,
							TileIndexes = new Point[] { new() },
							IsInTab = true,
							AppearOnTab = "item-info",
							IsUI = true,
							Name = "able-to-carry",
							IsKeptBetweenSessions = true,
						});
					}
				}
				ChunkManager.UpdateChunks();
				Screen.ScheduleDisplay();
			}

			if (CurrentSession == Session.WorldEdit)
			{
				WorldEditor.CreateTab();
				NavigationPanel.Tab.Open("world-editor", "edit brush");
			}
		}

		public static void CreateUIButtons()
		{
			// these are not creating multiple times cuz they are destroyed beforehand
			new MoveCamera("camera-move-up", new()
			{
				Name = "camera-move-up",
				Position = new(0, 1),
				TileIndexes = new Point[] { new(19, 20) { C = Color.Gray } },
				Height = 1,
				IsUI = true,
				IsLeftClickable = true,
			}) { CurrentType = MoveCamera.Type.Up };
			new MoveCamera("camera-move-down", new()
			{
				Name = "camera-move-down",
				Position = new(0, 2),
				TileIndexes = new Point[] { new(21, 20) { C = Color.Gray } },
				Height = 1,
				IsUI = true,
				IsLeftClickable = true,
			}) { CurrentType = MoveCamera.Type.Down };
			new MoveCamera("camera-move-left", new()
			{
				Name = "camera-move-left",
				Position = new(1, 0),
				TileIndexes = new Point[] { new(22, 20) { C = Color.Gray } },
				Height = 1,
				IsUI = true,
				IsLeftClickable = true,
			}) { CurrentType = MoveCamera.Type.Left };
			new MoveCamera("camera-move-right", new()
			{
				Name = "camera-move-right",
				Position = new(2, 0),
				TileIndexes = new Point[] { new(20, 20) { C = Color.Gray } },
				Height = 1,
				IsUI = true,
				IsLeftClickable = true,
			}) { CurrentType = MoveCamera.Type.Right };

			if (CurrentSession == Session.Single)
			{
				new MoveCamera("camera-center", new()
			{
				Name = "camera-center",
				Position = new(0, 0),
				TileIndexes = new Point[] { new(19, 14) { C = Color.Gray } },
				Height = 1,
				IsUI = true,
				IsLeftClickable = true,
			}) { CurrentType = MoveCamera.Type.Center };
				new ItemSlot("slot-head", new()
				{
					Name = "On your head:",
					Position = new(0, 7),
					TileIndexes = new Point[] { new(5, 22) { C = Color.Gray } },
					Height = 1,
					IsUI = true
				});
				new ItemSlot("slot-body", new()
				{
					Name = "On your body:",
					Position = new(0, 8),
					TileIndexes = new Point[] { new(6, 22) { C = Color.Gray } },
					Height = 1,
					IsUI = true
				});
				new ItemSlot("slot-feet", new()
				{
					Name = "On your feet:",
					Position = new(0, 9),
					TileIndexes = new Point[] { new(7, 22) { C = Color.Gray } },
					Height = 1,
					IsUI = true
				});

				new ItemSlot("hand-left", new()
				{
					Name = "In your left hand:",
					Position = new(0, 5),
					TileIndexes = new Point[] { new(8, 22) { C = Color.Gray } },
					Height = 1,
					IsUI = true
				});
				new ItemSlot("hand-right", new()
				{
					Name = "In your right hand:",
					Position = new(0, 4),
					TileIndexes = new Point[] { new(9, 22) { C = Color.Gray } },
					Height = 1,
					IsUI = true
				});

				new ItemSlot("carry-back", new()
				{
					Name = "On your back:",
					Position = new(0, 11),
					TileIndexes = new Point[] { new(10, 22) { C = Color.Gray } },
					Height = 1,
					IsUI = true
				});
				new ItemSlot("carry-waist", new()
				{
					Name = "On your waist:",
					Position = new(0, 12),
					TileIndexes = new Point[] { new(11, 22) { C = Color.Gray } },
					Height = 1,
					IsUI = true
				});

				new Wait("wait", new()
				{
					Position = new(6, 0),
					Height = 1,
					TileIndexes = new Point[] { new(42, 12) { C = Color.Gray } },
					IsLeftClickable = true,
					IsUI = true,
					Name = "wait",
				});

				for (int i = 0; i < ItemPile.MAX_COUNT; i++)
				{
					var slot = new ItemSlot($"ground-slot-{i}", new Object.CreationDetails()
					{
						Name = "On the ground:",
						Position = new(10 + i, 0),
						TileIndexes = new Point[] { new(7, 23) { C = Color.Brown } },
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
					Position = new(0, 8) { C = Color.Gray },
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
					var chunkExists = UniqueIDsExits(chunkId);
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
						var color = chunk.Data[pos][z].C;
						if (chunk.Data[pos][z] != new Point(0, 0)) tilesInCoord++;
						if (IsShowingRoofs == false && WorldEditor.Tiles["roof"].Contains(chunk.Data[pos][z]))
							color = new();

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
				Screen.EditCell(new(0, 4), WorldEditor.Brush, 1, WorldEditor.Brush.C);
				Screen.EditCell(new(0, 7), new(6, 23), 1, Color.Gray);

				Screen.EditCell(new(9, 0), new(41, 13), 1, Color.Gray);
				Screen.EditCell(new(10, 0), new(43, 13), 1, Color.Gray);
				Screen.EditCell(new(11, 0), new(42, 13), 1, Color.Gray);
			}
			else if (CurrentSession == Session.Single)
			{
				Screen.EditCell(new(4, 0), new(4, 23), 1, Color.Gray);
				Screen.EditCell(new(5, 0), new(5, 23), 1, Color.Gray);

				for (int i = 0; i < 8; i++)
					Screen.EditCell(new(10 + i, 0), new(), 1, new());
			}
		}
		public static void LoadWorld(Session session, string name)
		{
			Assets.Load(Assets.Type.DataSlot, $"Worlds\\{name}.worlddata");
			NavigationPanel.Tab.Close();
			NavigationPanel.Info.Textbox.Text = Object.descriptions[new(0, 23)];

			Object.DestroyAllSessionObjects();
			CurrentSession = session;
			CurrentWorldName = name;

			DisplayNavigationPanel();
			CreateUIButtons();
		}

		public static Dictionary<string, string> GetSavableData()
		{
			var result = new Dictionary<string, string>();
			foreach (var kvp in RawData)
				result[Text.ToJSON(kvp.Key)] = Text.ToJSON(kvp.Value);
			return result;
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
			if (RawData.ContainsKey(worldPos) == false)
				return false;
			for (int i = 0; i < 3; i++)
			{
				var tile = RawData[worldPos][i];
				if (WorldEditor.Tiles["roof"].Contains(tile))
					return true;
			}
			return false;
		}
		public static bool PositionHasWaterAsHighest(Point worldPos)
		{
			var highest = new Point();
			for (int i = 0; i < 3; i++)
			{
				if (RawData[worldPos][i] != new Point())
					highest = RawData[worldPos][i];
			}
			return WorldEditor.Tiles["water"].Contains(highest);
		}
	}
}
