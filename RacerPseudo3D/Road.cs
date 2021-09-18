using SMPL.Components;
using SMPL.Gear;
using SMPL.Prefabs;

namespace RacerPseudo3D
{
	public static class Road
	{
		public static Area PavementArea { get; set; }
		public static Area BorderLeftArea { get; set; }
		public static Area BorderRightArea { get; set; }

		public static ShapePseudo3D Pavement { get; set; }
		public static ShapePseudo3D BorderLeft { get; set; }
		public static ShapePseudo3D BorderRight { get; set; }

		public static void Create()
		{
			//Assets.Load(Assets.Type.Texture, "");
			Camera.CallWhen.Display(OnDisplay);

			PavementArea = new Area("pavement-area");
			BorderLeftArea = new Area("border-left-area");
			BorderRightArea = new Area("border-right-area");

			PavementArea.Position = new(0, 200);
			PavementArea.Size = new(100, 100);
			BorderLeftArea.Size = new(100, 100);
			BorderLeftArea.Position = new(-200, 50);
			BorderRightArea.Position = new(200, 50);

			Pavement = new ShapePseudo3D("pavement");
			BorderLeft = new ShapePseudo3D("border-left");
			BorderRight = new ShapePseudo3D("border-right");

			BorderLeft.PercentZ = 100;

			Pavement.AreaUniqueID = PavementArea.UniqueID;
			BorderLeft.AreaUniqueID = BorderLeftArea.UniqueID;
			BorderRight.AreaUniqueID = BorderRightArea.UniqueID;
		}
		private static void OnDisplay(Camera camera)
		{
			Camera.WorldCamera.Position = Mouse.Cursor.PositionWindow;

			Pavement.Display(camera);
			BorderLeft.Display(camera);
			BorderRight.Display(camera);
		}
	}
}
