using SMPL.Components;
using SMPL.Data;
using SMPL.Gear;
using SMPL.Prefabs;

namespace TestGame
{
   public class TestGame : Game
   {
		public LayeredShape3D shape = new("shape");
		public Area area;

		public TestGame(string uniqueID) : base(uniqueID) { }
      public static void Main() => Start(new TestGame("test-game"), new(3, 3));

		public override void OnGameCreate()
		{
			Camera.Event.Subscribe.Display(UniqueID);
			Assets.Event.Subscribe.LoadEnd(UniqueID);

			Assets.Load(Assets.Type.Texture, "house.png");
		}
		public override void OnAssetsLoadEnd()
		{
			area = new Area("shape-area");
			shape.AreaUniqueID = area.UniqueID;
			shape.TexturePath = "house.png";
			shape.LayerStackCount = 8;
			for (int i = 0; i < 8; i++)
				shape.SetLayerTextureCropTile(i, new(i, 0));
		}

		public override void OnCameraDisplay(Camera camera)
		{
			if (shape.AreaUniqueID == null)
				return;

			camera.Position = Mouse.Cursor.PositionWindow;
			shape.Display(camera);
			area.Angle += 0.05;
			shape.LayerStackAngle = 45;
		}
	}
}
