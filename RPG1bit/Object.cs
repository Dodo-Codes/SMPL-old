using SMPL.Data;
using SMPL.Gear;
using System.Collections.Generic;

namespace RPG1bit
{
	public class Object
	{
		public struct CreationDetails
		{
			public string Name { get; set; }
			public Point[] TileIndexes { get; set; }
			public Point Position { get; set; }
			public int Height { get; set; }
			public bool IsDragable { get; set; }
			public bool IsClickable { get; set; }
			public bool IsConfirmingClick { get; set; }
		}

		public static readonly Dictionary<Point, List<Object>> objects = new();
		public static readonly Dictionary<Point, string> descriptions = new()
		{
			{ new(33, 15), "Graphics 1-Bit Pack by kenney.nl\n" +
				"Font DPComic by cody@zone38.net\n" +
				"Music by opengameart.org/users/yubatake\n" +
				"Music by opengameart.org/users/avgvsta\n" +
				$"Game {NavigationPanel.Info.GameVersion} & SFX(software: Bfxr) by dodo" },
			{ new(01, 22), "" }, // background color
			{ new(00, 00), "Void." },
			{ new(04, 22), "Game navigation panel." },
			{ new(00, 23), "Game navigation panel." },
			{ new(29, 15), "Game navigation panel." },
			{ new(02, 22), "Game navigation panel." },
			{ new(03, 22), "Game navigation panel." },
			{ new(01, 23), "Information box." },
			// those objects' info is handled by their classes
			{ new(37, 20), "" }, { new(40, 13), "" }, { new(38, 16), "" }, { new(39, 16), "" }, { new(42, 16), "" }, { new(43, 16), "" },
			{ new(47, 06), "" },
			// those objects' info is handled by their classes

			{ new(44, 16), "Start a new multiplayer game session.\n(not available in this version)" },

			{ new(05, 22), "On your head:" },
			{ new(06, 22), "On your body:" },
			{ new(07, 22), "On your feet:" },
			{ new(08, 22), "In your left hand:" },
			{ new(09, 22), "In your right hand:" },
			{ new(10, 22), "On your back:" },
			{ new(11, 22), "On your waist:" },

			{ new(05, 00), "Grass." },
			{ new(06, 00), "Grass." },
			{ new(07, 00), "Grass." },

			{ new(32, 00), "Helmet." },
		};
		public static Object HoldingObject { get; set; }

		public string Name { get; }
		public Point TileIndexes { get; }
		public int Height { get; }
		public bool IsDragable { get; }
		public bool IsClickable { get; }
		public bool IsConfirmingClick { get; }

		private bool leftClicked;
		private static Point prevCursorPos = new(-1, 0);
		private static Point leftClickPos, rightClickPos;
		private Point position;
		public Point Position
		{
			get { return position; }
			set
			{
				position = value;
				if (objects.ContainsKey(value) == false)
				{
					objects.Add(value, new List<Object>() { this });
					return;
				}
				objects[position].Remove(this);
				objects[value].Add(this);
			}
		}

		public static void Initialize()
		{
			Game.CallWhen.GameIsRunning(StaticAlways, 0);
		}
		public Object(CreationDetails creationDetails)
		{
			Mouse.CallWhen.ButtonPress(OnButtonClicked);
			Mouse.CallWhen.ButtonRelease(OnButtonRelease);
			Game.CallWhen.GameIsRunning(Always);

			Name = creationDetails.Name;
			TileIndexes = creationDetails.TileIndexes.Length == 0 ? creationDetails.TileIndexes[0] :
				creationDetails.TileIndexes[(int)Probability.Randomize(new(0, creationDetails.TileIndexes.Length - 1))];
			Position = creationDetails.Position;
			Height = creationDetails.Height;
			IsDragable = creationDetails.IsDragable;
			IsClickable = creationDetails.IsClickable;
			IsConfirmingClick = creationDetails.IsConfirmingClick;
		}
		public static void DisplayAllObjects()
		{
			foreach (var kvp in objects)
				for (int i = 0; i < kvp.Value.Count; i++)
					Screen.EditCell(kvp.Value[i].Position, kvp.Value[i].TileIndexes, kvp.Value[i].Height, kvp.Value[i].Position.Color);
		}

		private void Always()
		{
			if (Screen.Sprite == null || NavigationPanel.Info.Textbox == null || Window.CurrentState == Window.State.Minimized) return;
			var cursorPos = Screen.GetCellAtCursorPosition();
			if (Gate.EnterOnceWhile($"on-hover-{Position}", cursorPos == Position))
			{
				var quad = Screen.Sprite.GetQuad($"{Height} cell {cursorPos.X} {cursorPos.Y}");
				var coord = quad.CornerA.TextureCoordinate;
				var tileIndex = coord / new Point(quad.TileSize.W + quad.TileGridWidth.W, quad.TileSize.H + quad.TileGridWidth.H);

				NavigationPanel.Info.Textbox.Scale = new(0.35, 0.35);
				NavigationPanel.Info.Textbox.Text = descriptions[tileIndex.IsInvalid ? new(0, 0) : tileIndex];
				NavigationPanel.Info.ShowClickableIndicator(IsClickable);
				NavigationPanel.Info.ShowDragableIndicator(IsDragable);
				NavigationPanel.Info.ShowLeftClickableIndicator(IsClickable || IsDragable);
				leftClicked = false;
				OnHovered();
			}
			if (IsDragable &&
				Gate.EnterOnceWhile($"on-unhover-{Position}", cursorPos != Position &&
				Mouse.ButtonIsPressed(Mouse.Button.Left) && Position == leftClickPos))
			{
				HoldingObject = this;
				Hoverer.CursorTextureTileIndexes = TileIndexes;
				Hoverer.CursorTextureColor = Position.Color;
				OnDragStart();
			}
		}
		private static void StaticAlways()
		{
			if (Screen.Sprite == null || NavigationPanel.Info.Textbox == null || Window.CurrentState == Window.State.Minimized) return;
			var mousePos = Screen.GetCellAtCursorPosition();
			if (mousePos != prevCursorPos)
			{
				NavigationPanel.Info.Textbox.Text = "";
				NavigationPanel.Info.Textbox.Scale = new(0.35, 0.35);
				NavigationPanel.Info.ShowClickableIndicator(false);
				NavigationPanel.Info.ShowDragableIndicator(false);
				NavigationPanel.Info.ShowLeftClickableIndicator(false);

				for (int i = 0; i < 3; i++)
				{
					var quadID = $"{i} cell {mousePos.X} {mousePos.Y}";
					var quad = Screen.Sprite.GetQuad(quadID);
					var coord = quad.CornerA.TextureCoordinate;
					var tileIndex = coord / new Point(quad.TileSize.W + quad.TileGridWidth.W, quad.TileSize.H + quad.TileGridWidth.H);
					var description = descriptions[tileIndex.IsInvalid ? new(0, 0) : tileIndex];
					var sep = i != 0 && description != "" && NavigationPanel.Info.Textbox.Text != "" ? "\n" : "";

					if (NavigationPanel.Info.Textbox.Text != "" && description == descriptions[new(0, 0)]) break;
					NavigationPanel.Info.Textbox.Text += $"{sep}{description}";
				}
			}
			prevCursorPos = mousePos;
		}

		private void OnButtonRelease(Mouse.Button button)
		{
			var mousePos = Screen.GetCellAtCursorPosition();
			if (button == Mouse.Button.Left)
			{
				if (IsClickable && Position == mousePos && Position == leftClickPos)
				{
					if (IsConfirmingClick && Map.SessionIsOngoing && leftClicked == false)
					{
						leftClicked = true;
						NavigationPanel.Info.Textbox.Text = "A session is currently ongoing.\n" +
							"Any unsaved progress will be lost.\n" +
							"Left click again to continue.";
					}
					else
					{
						leftClicked = false;
						OnLeftClicked();
					}
				}
				if (HoldingObject != null)
				{
					if (objects.ContainsKey(mousePos))
						for (int i = 0; i < objects[mousePos].Count; i++)
							objects[mousePos][i].OnDroppedUpon();
					HoldingObject.OnDragEnd();
					HoldingObject = null;
					Hoverer.CursorTextureTileIndexes = new(36, 10);
				}
			}
			if (button == Mouse.Button.Right && Position == mousePos && Position == rightClickPos) OnRightClicked();
		}
		private static void OnButtonClicked(Mouse.Button button)
		{
			if (button == Mouse.Button.Left) leftClickPos = Screen.GetCellAtCursorPosition();
			if (button == Mouse.Button.Right) rightClickPos = Screen.GetCellAtCursorPosition();
		}

		protected virtual void OnLeftClicked() { }
		protected virtual void OnRightClicked() { }
		protected virtual void OnHovered() { }
		protected virtual void OnDragStart() { }
		protected virtual void OnDragEnd() { }
		protected virtual void OnDroppedUpon() { }
	}
}
