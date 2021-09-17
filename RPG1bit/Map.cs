using RPG1bit.Objects;
using SMPL.Data;
using static RPG1bit.Object;

namespace RPG1bit
{
	public static class Map
	{
		public static Point CameraPosition { get; set; }

		public static void Display()
		{
			var p = new Point(0, 0) + new Point(9, 9) - CameraPosition;
			if (p.X < 1 || p.X > 17) return;
			if (p.Y < 1 || p.Y > 17) return;
		}
		public static void CreateUIButtons()
		{
			for (int x = 0; x < 18; x++)
			{
				Screen.EditCell(new(x, 0), new(4, 22), 1, Color.Brown);
				Screen.EditCell(new(x, 0), new(1, 22), 0, Color.BrownDark);
			}
			for (int y = 0; y < 18; y++)
			{
				Screen.EditCell(new(0, y), new(4, 22), 1, Color.Brown);
				Screen.EditCell(new(0, y), new(1, 22), 0, Color.BrownDark);
			}

			new ExitGame(new CreationDetails()
			{ Name = "head", Position = new(0, 0) { Color = Color.Gray }, TileIndexes = new Point[] { new(5, 22) }, Height = 1 });
			new ExitGame(new CreationDetails()
			{ Name = "torso", Position = new(0, 1) { Color = Color.Gray }, TileIndexes = new Point[] { new(6, 22) }, Height = 1 });
			new ExitGame(new CreationDetails()
			{ Name = "feet", Position = new(0, 2) { Color = Color.Gray }, TileIndexes = new Point[] { new(7, 22) }, Height = 1 });

			new ExitGame(new CreationDetails()
			{ Name = "hand-left", Position = new(2, 0) { Color = Color.Gray }, TileIndexes = new Point[] { new(8, 22) }, Height = 1 });
			new ExitGame(new CreationDetails()
			{ Name = "hand-right", Position = new(3, 0) { Color = Color.Gray }, TileIndexes = new Point[] { new(9, 22) }, Height = 1 });

			new ExitGame(new CreationDetails()
			{ Name = "carry-back", Position = new(0, 4) { Color = Color.Gray }, TileIndexes = new Point[] { new(10, 22) }, Height = 1 });
			new ExitGame(new CreationDetails()
			{ Name = "carry-waist", Position = new(0, 5) { Color = Color.Gray }, TileIndexes = new Point[] { new(11, 22) }, Height = 1 });

			new ExitGame(new CreationDetails()
			{ Name = "helmet", Position = new(0, 0) { Color = Color.White }, TileIndexes = new Point[] { new(32, 0) }, Height = 2 });
		}
	}
}
