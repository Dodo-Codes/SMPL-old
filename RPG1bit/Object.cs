using SMPL.Data;
using SMPL.Gear;
using System.Collections.Generic;

namespace RPG1bit
{
	public class Object
	{
		public static readonly Dictionary<Point, List<Object>> objects = new();

		public string Name { get; }
		public string Description { get; }
		public Point TileIndex { get; }

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
			new Object("Void", new(0, 0), new Point[] { new(0, 0) }, "The end of the world.");
			new Object("Grass", new(0, 1), new Point[] { new(5, 0), new(6, 0), new(7, 0) }, "Grass.");
		}

		public Object(string name, Point position, Point[] tileIndexes, string description)
		{
			Mouse.CallWhen.ButtonPress(OnLeftClicked);

			Name = name;
			Description = description;
			TileIndex = tileIndexes.Length == 0 ? tileIndexes[0] :
				tileIndexes[(int)Probability.Randomize(new(0, tileIndexes.Length - 1))];

			Position = position;
		}

		private void OnLeftClicked(Mouse.Button button)
		{
			if (button != Mouse.Button.Left || Map.GetCellAtCursorPosition() != Position) return;
			Console.Log(Position);
		}
	}
}
