using Newtonsoft.Json;
using SMPL.Components;
using SMPL.Data;
using SMPL.Gear;
using System.Collections.Generic;

namespace RPG1bit
{
	public class Map : Thing
	{
		public enum Session { None, Single, Multi, MapEdit }

		public static Point TileBarrier => new(0, 22);
		public static Point TilePlayer => new(24, 8);

		public static Dictionary<Point, Point[]> RawData { get; set; } = new();
		public static string CurrentMapName { get; private set; }
		public static bool IsShowingRoofs { get; set; } = true;

		public static Point CameraPosition { get; set; }
		public static Session CurrentSession { get; set; }

		public Map(string uniqueID) : base(uniqueID)
		{
			Assets.Event.Subscribe.LoadEnd(uniqueID);
		}
		public override void OnAssetsLoadEnd()
		{
			if (Assets.ValuesAreLoaded("map-data"))
			{
				var mapData = Text.FromJSON<Dictionary<string, string>>(Assets.GetValue("map-data"));
				var signData = new List<CompactSignData>();
				if (Assets.ValuesAreLoaded("signs")) signData = Text.FromJSON<List<CompactSignData>>(Assets.GetValue("signs"));
				if (mapData != default)
					foreach (var kvp in mapData)
						RawData[Text.FromJSON<Point>(kvp.Key)] = Text.FromJSON<Point[]>(kvp.Value);
				if (UniqueIDsExits("player") == false)
					CameraPosition = Text.FromJSON<Point>(Assets.GetValue("camera-position"));

				for (int i = 0; i < signData.Count; i++)
				{
					new Sign($"{signData[i].P}-{i}-{signData[i].P.C.R}", new()
					{
						Position = signData[i].P,
						TileIndexes = new Point[] { signData[i].I },
						Height = (int)signData[i].P.C.R,
					});
				}

				if (CurrentSession == Session.Single && mapData != default)
				{
					var freeTile = new Point(0, 0);
					var playerTiles = new List<Point>();
					foreach (var kvp in RawData)
					{
						var pos = new Point(kvp.Key.X, kvp.Key.Y);
						if (RawData[pos][3] == TilePlayer)
						{
							playerTiles.Add(pos);
							RawData[pos][3] = default;
						}
						else if (RawData[pos][3] == TileBarrier)
						{
							var tile = TileBarrier;
							tile.C = new Color();
							RawData[pos][3] = tile;
						}
						else if (RawData[pos][3] == new Point(0, 0))
							freeTile = pos;

						for (int i = 0; i < 3; i++)
						{
							if (MapEditor.DoorTiles.Contains(RawData[pos][i]))
							{
								new Door($"door-{pos}-{i}", new()
								{
									Position = pos,
									Height = i,
									Name = "Door",
									TileIndexes = new Point[] { RawData[pos][i] }
								});
							}
						}
					}

					var randPoint = playerTiles.Count > 0 ?
						playerTiles[(int)Probability.Randomize(new(0, playerTiles.Count - 1))] : freeTile;

					if (CurrentSession == Session.Single)
					{
						var player = default(Player);
						if (Assets.ValuesAreLoaded("player") == false)
						{
							player = new Player("player", new Object.CreationDetails()
							{
								Name = "player",
								Position = randPoint,
								Height = 3,
								TileIndexes = new Point[] { new(25, 0) }
							});
						}
						else
							player = Text.FromJSON<Player>(Assets.GetValue("player"));
						CameraPosition = player.Position;
						NavigationPanel.Tab.Close();
					}
					Assets.UnloadAllValues();
				}
				Screen.Display();
			}

			if (CurrentSession == Session.MapEdit)
			{
				MapEditor.CreateTab();
				NavigationPanel.Tab.Open("map-editor", "edit brush");
			}
		}

		public static void CreateUIButtons()
		{
			// these are not creating multiple times cuz they are destroyed beforehand
			new MoveCamera("camera-move-up", new Object.CreationDetails()
			{
				Name = "camera-move-up",
				Position = new(0, 1),
				TileIndexes = new Point[] { new(19, 20) { C = Color.Gray } },
				Height = 1,
				IsUI = true,
				IsLeftClickable = true,
			}) { CurrentType = MoveCamera.Type.Up };
			new MoveCamera("camera-move-down", new Object.CreationDetails()
			{
				Name = "camera-move-down",
				Position = new(0, 2),
				TileIndexes = new Point[] { new(21, 20) { C = Color.Gray } },
				Height = 1,
				IsUI = true,
				IsLeftClickable = true,
			}) { CurrentType = MoveCamera.Type.Down };
			new MoveCamera("camera-move-left", new Object.CreationDetails()
			{
				Name = "camera-move-left",
				Position = new(1, 0),
				TileIndexes = new Point[] { new(22, 20) { C = Color.Gray } },
				Height = 1,
				IsUI = true,
				IsLeftClickable = true,
			}) { CurrentType = MoveCamera.Type.Left };
			new MoveCamera("camera-move-right", new Object.CreationDetails()
			{
				Name = "camera-move-right",
				Position = new(2, 0),
				TileIndexes = new Point[] { new(20, 20) { C = Color.Gray } },
				Height = 1,
				IsUI = true,
				IsLeftClickable = true,
			}) { CurrentType = MoveCamera.Type.Right };

			if (CurrentSession == Session.Single || CurrentSession == Session.Multi)
			{
				new MoveCamera("camera-center", new Object.CreationDetails()
			{
				Name = "camera-center",
				Position = new(0, 0),
				TileIndexes = new Point[] { new(19, 14) { C = Color.Gray } },
				Height = 1,
				IsUI = true,
				IsLeftClickable = true,
			}) { CurrentType = MoveCamera.Type.Center };
				new EquipSlot("head", new Object.CreationDetails()
				{
					Name = "head",
					Position = new(0, 7),
					TileIndexes = new Point[] { new(5, 22) { C = Color.Gray } },
					Height = 1,
					IsUI = true
				});
				new EquipSlot("body", new Object.CreationDetails()
				{
					Name = "body",
					Position = new(0, 8),
					TileIndexes = new Point[] { new(6, 22) { C = Color.Gray } },
					Height = 1,
					IsUI = true
				});
				new EquipSlot("feet", new Object.CreationDetails()
				{
					Name = "feet",
					Position = new(0, 9),
					TileIndexes = new Point[] { new(7, 22) { C = Color.Gray } },
					Height = 1,
					IsUI = true
				});

				new EquipSlot("hand-left", new Object.CreationDetails()
				{
					Name = "hand-left",
					Position = new(0, 5),
					TileIndexes = new Point[] { new(8, 22) { C = Color.Gray } },
					Height = 1,
					IsUI = true
				});
				new EquipSlot("hand-right", new Object.CreationDetails()
				{
					Name = "hand-right",
					Position = new(0, 4),
					TileIndexes = new Point[] { new(9, 22) { C = Color.Gray } },
					Height = 1,
					IsUI = true
				});

				new EquipSlot("carry-back", new Object.CreationDetails()
				{
					Name = "carry-back",
					Position = new(0, 11),
					TileIndexes = new Point[] { new(10, 22) { C = Color.Gray } },
					Height = 1,
					IsUI = true
				});
				new EquipSlot("carry-waist", new Object.CreationDetails()
				{
					Name = "carry-waist",
					Position = new(0, 12),
					TileIndexes = new Point[] { new(11, 22) { C = Color.Gray } },
					Height = 1,
					IsUI = true
				});
			}
			if (CurrentSession == Session.MapEdit)
			{
				new SwitchHeight("brush-height", new Object.CreationDetails()
				{
					Name = "height-up",
					Position = new(0, 5),
					Height = 1,
					IsUI = true,
					IsLeftClickable = true,
				});
				new ShowRoofs("show-roofs", new Object.CreationDetails()
				{
					Name = "show-roofs",
					Position = new(0, 8) { C = Color.Gray },
					Height = 1,
					IsUI = true,
					IsLeftClickable = true,
				});
			}
		}
		public static void DestroyAllSessionObjects()
		{
			if (CurrentSession == Session.None) return;

			var objsToDestroy = new List<Object>();
			foreach (var kvp in Object.objects)
				for (int i = 0; i < kvp.Value.Count; i++)
				{
					if ((kvp.Value[i].IsUI && kvp.Value[i].Position.X > 18) || kvp.Value[i].IsInTab) continue;
					objsToDestroy.Add(kvp.Value[i]);
				}

			for (int i = 0; i < objsToDestroy.Count; i++)
				objsToDestroy[i].Destroy();
		}
		
		public static void Display()
		{
			if (RawData == null) return;
			for (double y = CameraPosition.Y - 8; y < CameraPosition.Y + 9; y++)
				for (double x = CameraPosition.X - 8; x < CameraPosition.X + 9; x++)
				{
					var pos = new Point(x, y);
					if (RawData.ContainsKey(pos) == false)
					{
						for (int z = 0; z < 4; z++)
							Screen.EditCell(MapToScreenPosition(pos), new(), z, new());
						continue;
					}

					var tilesInCoord = 0;
					for (int z = 0; z < 4; z++)
					{
						var color = RawData[pos][z].C;
						if (RawData[pos][z] != new Point(0, 0)) tilesInCoord++;
						if (IsShowingRoofs == false && MapEditor.RoofTiles.Contains(RawData[pos][z]))
							color = new();

						Screen.EditCell(MapToScreenPosition(pos), RawData[pos][z], z, color);
					}
					if (tilesInCoord == 0)
						RawData.Remove(pos);
				}
		}
		public static void DisplayNavigationPanel()
		{
			for (int x = 0; x < 18; x++)
			{
				Screen.EditCell(new(x, 0), new(4, 22), 1, Color.Brown);
				Screen.EditCell(new(x, 0), new(1, 22), 0, Color.Brown / 2);
			}
			for (int y = 0; y < 18; y++)
			{
				Screen.EditCell(new(0, y), new(4, 22), 1, Color.Brown);
				Screen.EditCell(new(0, y), new(1, 22), 0, Color.Brown / 2);
			}
			if (CurrentSession == Session.MapEdit)
			{
				Screen.EditCell(new(0, 4), MapEditor.Brush, 1, MapEditor.Brush.C);
				Screen.EditCell(new(0, 7), new(6, 23), 1, Color.Gray);

				Screen.EditCell(new(9, 0), new(41, 13), 1, Color.Gray);
				Screen.EditCell(new(10, 0), new(43, 13), 1, Color.Gray);
				Screen.EditCell(new(11, 0), new(42, 13), 1, Color.Gray);
			}
			else if (CurrentSession == Session.Single)
			{
				Screen.EditCell(new(4, 0), new(4, 23), 1, Color.Gray);
				Screen.EditCell(new(5, 0), new(5, 23), 1, Color.Gray);
			}
		}
		public static void LoadMap(Session session, string name)
		{
			Assets.Load(Assets.Type.DataSlot, $"Maps\\{name}.mapdata");
			NavigationPanel.Tab.Close();
			NavigationPanel.Info.Textbox.Text = Object.descriptions[new(0, 23)];

			DestroyAllSessionObjects();
			CurrentSession = session;
			CurrentMapName = name;

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
		public static Point MapToScreenPosition(Point mapPos)
		{
			return mapPos - CameraPosition + new Point(9, 9);
		}
		public static Point ScreenToMapPosition(Point screenPos)
		{
			return CameraPosition + (screenPos - new Point(9, 9));
		}
	}
}
