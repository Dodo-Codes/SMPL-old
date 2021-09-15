using RPG1bit.Objects;
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
		}

		public static readonly Dictionary<Point, List<Object>> objects = new();
		public static readonly Dictionary<Point, string> descriptions = new()
		{
			{ new Point(33, 15), "Graphics: 1-Bit Pack by kenney.nl\n" +
				"Font: DPComic by cody@zone38.net\n" +
				$"Game: {Window.Title} ({Info.GameVersion}) by dodo codes" },
			{ new Point(00, 00), "Void." },
			{ new Point(04, 22), "Game navigation panel." },
			{ new Point(00, 23), "Game navigation panel." },
			{ new Point(01, 23), "Information box." },
			{ new Point(37, 20), "Minimize the game." },
			{ new Point(40, 13), "Exit the game." },
			{ new Point(38, 16), "Adjust the sound effects volume." },
			{ new Point(39, 16), "Adjust the music volume." },
			{ new Point(42, 16), "Save/Load a game session." },
			{ new Point(43, 16), "Start a singleplayer session." },
			{ new Point(44, 16), "Start a multiplayer session." },
		};

		public string Name { get; }
		public Point TileIndex { get; }

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

		public static void CreateAllObjects()
		{
			new ExitGame(new CreationDetails()
			{
				Name = "x",
				Position = new(31, 0) { Color = Color.RedDark },
				TileIndexes = new Point[] { new(40, 13) },
			});
			new MinimizeGame(new CreationDetails()
			{
				Name = "-",
				Position = new(30, 0) { Color = Color.Gray },
				TileIndexes = new Point[] { new(37, 20) },
			});
			new AdjustSound(new CreationDetails()
			{
				Name = "adjust-sound",
				Position = new(26, 0) { Color = Color.Gray },
				TileIndexes = new Point[] { new(38, 16) },
			});
			new AdjustMusic(new CreationDetails()
			{
				Name = "adjust-music",
				Position = new(27, 0) { Color = Color.Gray },
				TileIndexes = new Point[] { new(39, 16) },
			});
			new ExitGame(new CreationDetails()
			{
				Name = "start-singleplayer",
				Position = new(19, 0) { Color = Color.Gray },
				TileIndexes = new Point[] { new(43, 16) },
			});
			new ExitGame(new CreationDetails()
			{
				Name = "start-multiplayer",
				Position = new(20, 0) { Color = Color.Gray },
				TileIndexes = new Point[] { new(44, 16) },
			});
			new ExitGame(new CreationDetails()
			{
				Name = "save-load",
				Position = new(21, 0) { Color = Color.Gray },
				TileIndexes = new Point[] { new(42, 16) },
			});
		}
		public Object(CreationDetails creationDetails)
		{
			Mouse.CallWhen.ButtonPress(OnButtonClicked);
			Mouse.CallWhen.ButtonRelease(OnButtonRelease);
			Game.CallWhen.GameIsRunning(Always);

			Name = creationDetails.Name;
			TileIndex = creationDetails.TileIndexes.Length == 0 ? creationDetails.TileIndexes[0] :
				creationDetails.TileIndexes[(int)Probability.Randomize(new(0, creationDetails.TileIndexes.Length - 1))];
			Position = creationDetails.Position;
		}
		public static void ShowClickableIndicator(bool show = true)
		{
			Screen.EditCell(new Point(31, 17), show ? new Point(29, 15) : new Point(0, 23), 0, Color.White);
		}

		private void Always()
		{
			if (Screen.Sprite == null || Info.Textbox == null || Window.CurrentState == Window.State.Minimized) return;
			if (Gate.EnterOnceWhile($"on-hover-{Position}", Screen.GetCellAtCursorPosition() == Position))
			{
				var mousePos = Screen.GetCellAtCursorPosition();
				var quad = Screen.Sprite.GetQuad($"cell {mousePos.X} {mousePos.Y} 0");
				var coord = quad.CornerA.TextureCoordinate;
				var tileIndex = coord / new Point(quad.TileSize.W + quad.TileGridWidth.W, quad.TileSize.H + quad.TileGridWidth.H);

				Info.Textbox.Scale = new(0.35, 0.35);
				Info.Textbox.Text = descriptions[tileIndex.IsInvalid ? new(0, 0) : tileIndex];
				OnHovered();
			}
		}

		private void OnButtonRelease(Mouse.Button button)
		{
			var mousePos = Screen.GetCellAtCursorPosition();
			if (button == Mouse.Button.Left && Position == mousePos && Position == leftClickPos) OnLeftClicked();
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
	}
}
