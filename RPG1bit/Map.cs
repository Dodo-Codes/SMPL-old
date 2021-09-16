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
			Screen.EditCell(p, new(5, 0), 0, Color.Green);
		}
		public static void CreateUIButtons()
		{
			new ExitGame(new CreationDetails() { Name = "head", Position = new(0, 0), TileIndexes = new Point[] { new(5, 22) } });
			new ExitGame(new CreationDetails() { Name = "torso", Position = new(0, 1), TileIndexes = new Point[] { new(6, 22) } });
			new ExitGame(new CreationDetails() { Name = "feet", Position = new(0, 2), TileIndexes = new Point[] { new(7, 22) } });
		}
	}
}
