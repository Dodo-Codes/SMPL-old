using SMPL;

namespace TestGame
{
	public class FileEvents : SMPL.FileEvents
	{
		public override void OnLoadingEnd()
		{
			if (File.AssetIsLoaded("fire.png"))
			{
				new Player();
			}
		}
	}
}
