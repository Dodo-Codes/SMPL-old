using SMPL.Data;
using SMPL.Gear;

namespace RPG1bit
{
	public static class Map
	{
		public static Point CameraPosition { get; set; }
		public static bool SessionIsOngoing { get; set; }

		public static void Display()
		{
			var p = new Point(0, 0) + new Point(9, 9) - CameraPosition;
			if (p.X < 1 || p.X > 17) return;
			if (p.Y < 1 || p.Y > 17) return;
		}
		public static void CreateUIButtons()
		{
			if (Gate.EnterOnceWhile("create-buttons", true))
			{
				new SlotHead(new Object.CreationDetails()
				{ Name = "head", Position = new(0, 0) { Color = Color.Gray }, TileIndexes = new Point[] { new(5, 22) }, Height = 1 });
				new SlotBody(new Object.CreationDetails()
				{ Name = "body", Position = new(0, 1) { Color = Color.Gray }, TileIndexes = new Point[] { new(6, 22) }, Height = 1 });
				new SlotFeet(new Object.CreationDetails()
				{ Name = "feet", Position = new(0, 2) { Color = Color.Gray }, TileIndexes = new Point[] { new(7, 22) }, Height = 1 });

				new SlotHandLeft(new Object.CreationDetails()
				{ Name = "hand-left", Position = new(2, 0) { Color = Color.Gray }, TileIndexes = new Point[] { new(8, 22) }, Height = 1 });
				new SlotHandRight(new Object.CreationDetails()
				{ Name = "hand-right", Position = new(3, 0) { Color = Color.Gray }, TileIndexes = new Point[] { new(9, 22) }, Height = 1 });

				new SlotBack(new Object.CreationDetails()
				{ Name = "carry-back", Position = new(0, 4) { Color = Color.Gray }, TileIndexes = new Point[] { new(10, 22) }, Height = 1 });
				new SlotWaist(new Object.CreationDetails()
				{ Name = "carry-waist", Position = new(0, 5) { Color = Color.Gray }, TileIndexes = new Point[] { new(11, 22) }, Height = 1 });
			}
		}
	}
}
