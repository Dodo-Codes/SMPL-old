using SMPL.Components;
using SMPL.Gear;

namespace TestGame
{
   public class TestGame : Game
   {
      public static void Main() => Start(new TestGame(), new(1, 1));

		uint uid;
		public override void OnGameCreate()
		{
			Subscribe(Event.CameraDisplay);
			Subscribe(Event.AssetLoadEnd);

			Assets.Load(Assets.Type.Font, "Munro.ttf");
			Assets.Load(Assets.Type.Texture, "explosive.jpg");
		}
		public override void OnAssetLoadEnd(string path)
		{
			if (path == "explosive.jpg")
			{
				var spr = new Sprite();
				var area = new Area();
				spr.TexturePath = "explosive.jpg";
				spr.AreaUID = area.UID;
				uid = spr.UID;
			}
		}
		public override void OnCameraDisplay(Camera camera)
		{
			var spr = (Sprite)Pick(uid);
			if (spr != null)
			{
				var area = (Area)Pick(spr.AreaUID);
				area.Angle++;
				spr.Display(camera);
			}
			Debug.Display();
		}
	}
	public class Test : Thing
	{

	}
}
