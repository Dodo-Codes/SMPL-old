using SMPL.Components;
using SMPL.Data;
using SMPL.Gear;

namespace TestGame
{
	public class Player
	{
		public Player()
		{
			File.LoadAsset(File.Asset.Texture, "explosive.jpg");
			Camera.CallWhen.Display(OnDraw);
			File.CallWhen.AssetLoadEnd(OnAssetLoadEnd);
		}
		void OnAssetLoadEnd()
		{
			var tr = new Area();
			tr.Identity = new Identity<Area>(tr, "test-tr");
			Sprite.Create("test", tr);
			var spr = Identity<Sprite>.PickByUniqueID("test");
			Effects.Create("test-effects", spr);
			spr.TexturePath = "explosive.jpg";
			spr.Effects = Identity<Effects>.PickByUniqueID("test-effects");
			for (int i = 0; i < 5; i++)
			{
				for (int j = 0; j < 5; j++)
				{
					var quad = new Quad()
					{
						CornerA = new(new Point(j * 64, i * 64), 0, 0),
						CornerB = new(new Point(j * 64 + 64	, i * 64), 64, 0),
						CornerC = new(new Point(j * 64 + 64	, i * 64 + 64), 64, 64),
						CornerD = new(new Point(j * 64		, i * 64 + 64), 0, 64)
					};
					spr.SetQuad($"quad-{i}-{j}", quad);
				}
			}
		}
		void OnDraw(Camera camera)
		{
			var spr = Identity<Sprite>.PickByUniqueID("test");
			var tr = Identity<Area>.PickByUniqueID("test-tr");
			if (spr == null) return;
			//tr.Angle++;
			tr.Size = new Size(800, 800);
			spr.Effects.Progress = Time.Clock;
			spr.Effects.ProgressiveWaterOpacityPercent = 100;
			spr.IsRepeated = true;
			spr.Display(camera);

			//quad.Display(camera);
		}
	}
}