using SMPL.Components;
using SMPL.Data;
using SMPL.Gear;
using SMPL.Prefabs;

namespace TestGame
{
   public class TestGame : Game
   {
		Point pathPoint = new Point();

		public TestGame(string uniqueID) : base(uniqueID) { }
      public static void Main() => Start(new TestGame("test-game"), new(1, 1));

		public override void OnGameCreate()
		{
			Camera.Event.Subscribe.Display(UniqueID);
		}

		public override void OnCameraDisplay(Camera camera)
		{
			var p1 = Mouse.Cursor.PositionWindow;
			var p2 = new Point(300, 0);
			var walls = new Line[]
			{
				new(new(0, 0), new(50, 50)), new(new(50, 0), new(0, 50)),
				new(new(100, 100), new(200, 200)), new(new(200, 100), new(100, 200)),
			};

			if (Gate.EnterOnceWhile("t", Keyboard.KeyIsPressed(Keyboard.Key.Space)))
				pathPoint = Point.Pathfind(p1, p2, 1000, walls);

			var l1 = new Line(p1, pathPoint);
			var l2 = new Line(p2, pathPoint);

			for (int i = 0; i < walls.Length; i++)
				walls[i].Display(camera, 5);
			l1.Display(camera, 5);
			l2.Display(camera, 5);
		}
	}
}
