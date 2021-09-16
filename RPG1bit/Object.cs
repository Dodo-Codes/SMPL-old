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
			{ new Point(33, 15), "Graphics 1-Bit Pack by kenney.nl\n" +
				"Font DPComic by cody@zone38.net\n" +
				"Music by opengameart.org/users/yubatake\n" +
				"Music by opengameart.org/users/avgvsta\n" +
				$"Game {NavigationPanel.Info.GameVersion} & SFX(software: Bfxr) by dodo" },
			{ new Point(00, 00), "Void." },
			{ new Point(01, 22), "" }, // background color
			{ new Point(04, 22), "Game navigation panel." },
			{ new Point(00, 23), "Game navigation panel." },
			{ new Point(01, 23), "Information box." },
			{ new Point(37, 20), "Minimize the game." },
			{ new Point(40, 13), "Exit the game." },
			{ new Point(38, 16), "Adjust the sound effects volume." },
			{ new Point(39, 16), "Adjust the music volume." },
			{ new Point(42, 16), "Save/Load a game session." },
			{ new Point(43, 16), "Start a new singleplayer session." },
			{ new Point(44, 16), "Start a new multiplayer session." },

			{ new Point(05, 22), "Head." },
			{ new Point(06, 22), "Torso." },
			{ new Point(07, 22), "Feet." },

			{ new Point(05, 00), "Grass." },
			{ new Point(06, 00), "Grass." },
			{ new Point(07, 00), "Grass." },
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

		public Object(CreationDetails creationDetails)
		{
			Mouse.CallWhen.ButtonPress(OnButtonClicked);
			Mouse.CallWhen.ButtonRelease(OnButtonRelease);
			Game.CallWhen.GameIsRunning(Always);

			Name = creationDetails.Name;
			TileIndex = creationDetails.TileIndexes.Length == 0 ? creationDetails.TileIndexes[0] :
				creationDetails.TileIndexes[(int)Probability.Randomize(new(0, creationDetails.TileIndexes.Length - 1))];
			Position = creationDetails.Position;

			Screen.EditCell(Position, TileIndex, 0, Position.Color);
		}
		public static void DisplayAllObjects()
		{
			foreach (var kvp in objects)
				for (int i = 0; i < kvp.Value.Count; i++)
				{
					var c = kvp.Value[i].Position.Color;
					var pos = kvp.Value[i].Position;

					//if (pos.X < 0 || pos.X > 18) continue;
					//if (pos.Y < 0 || pos.Y > 18) continue;

					var quadID = $"cell {pos} {i}";
					if (Screen.Sprite.HasQuad(quadID) == false) continue;
					var quad = Screen.Sprite.GetQuad(quadID);
					quad.TileSize = new(16, 16);
					quad.TileGridWidth = new Size(1, 1);
					quad.SetColor(c, c, c, c);
					quad.SetTextureCropTile(kvp.Value[i].TileIndex);
					Screen.Sprite.SetQuad(quadID, quad);
				}
		}

		private void Always()
		{
			if (Screen.Sprite == null || NavigationPanel.Info.Textbox == null || Window.CurrentState == Window.State.Minimized) return;
			if (Gate.EnterOnceWhile($"on-hover-{Position}", Screen.GetCellAtCursorPosition() == Position))
			{
				var mousePos = Screen.GetCellAtCursorPosition();
				var quad = Screen.Sprite.GetQuad($"cell {mousePos.X} {mousePos.Y} 0");
				var coord = quad.CornerA.TextureCoordinate;
				var tileIndex = coord / new Point(quad.TileSize.W + quad.TileGridWidth.W, quad.TileSize.H + quad.TileGridWidth.H);

				NavigationPanel.Info.Textbox.Scale = new(0.35, 0.35);
				NavigationPanel.Info.Textbox.Text = descriptions[tileIndex.IsInvalid ? new(0, 0) : tileIndex];
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
