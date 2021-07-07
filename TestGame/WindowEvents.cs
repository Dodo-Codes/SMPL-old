using SMPL;

namespace TestGame
{
	public class WindowEvents : SMPL.WindowEvents
	{
		public override void OnCreated()
		{
			File.LoadAsset(File.Asset.Texture, "fire.png");
		}
	}
}
