using SMPL.Components;
using SMPL.Data;
using SMPL.Gear;
using SMPL.Prefabs;

namespace RacerPseudo3D
{
	public class Road : Thing
	{
		public static Area PavementArea { get; set; }
		public static Area BorderLeftArea { get; set; }
		public static Area BorderRightArea { get; set; }

		public static ShapePseudo3D Pavement { get; set; }
		public static ShapePseudo3D BorderLeft { get; set; }
		public static ShapePseudo3D BorderRight { get; set; }

		public Road(string uniqueID) : base(uniqueID)
		{
			Assets.Load(Assets.Type.Texture, "road.jpg", "border.png");
			Camera.Event.Subscribe.Display(uniqueID);

			PavementArea = new Area("pavement-area");
			BorderLeftArea = new Area("border-left-area");
			BorderRightArea = new Area("border-right-area");

			PavementArea.Position = new(0, 200);
			PavementArea.Size = new(100, 100);
			BorderLeftArea.Size = new(100, 100);
			BorderLeftArea.Position = new(-50, 170);
			BorderRightArea.Position = new(50, 170);

			Pavement = new ShapePseudo3D("pavement");
			BorderLeft = new ShapePseudo3D("border-left");
			BorderRight = new ShapePseudo3D("border-right");

			BorderLeft.TexturePath = "border.png";
			BorderLeft.SetSidesTextureCropDefault();
			BorderLeft.SetSideTextureCropPercent(ShapePseudo3D.Side.Near, new(100, 100), new(0, 0));
			BorderRight.TexturePath = "border.png";
			BorderRight.SetSidesTextureCropDefault();

			Pavement.TexturePath = "road.jpg";
			Pavement.PercentZ = 95;
			Pavement.PercentDepth = 500_000;
			PavementArea.Size = new(200, 10);

			BorderLeft.PercentDepth = 50;
			BorderLeft.PercentZ = -50;

			//Pavement.SetSideVisibility(ShapePseudo3D.Side.Far, false);
			//BorderRight.SetSideVisibility(ShapePseudo3D.Side.Far, false);
			//BorderLeft.SetSideVisibility(ShapePseudo3D.Side.Far, false);

			Pavement.AreaUniqueID = PavementArea.UniqueID;
			BorderLeft.AreaUniqueID = BorderLeftArea.UniqueID;
			BorderRight.AreaUniqueID = BorderRightArea.UniqueID;
		}

		public override void OnCameraDisplay(Camera camera)
		{
			Camera.WorldCamera.Position = Mouse.Cursor.PositionWindow;

			Pavement.Display(camera);
			BorderLeft.Display(camera);
			BorderRight.Display(camera);
		}
	}
}
