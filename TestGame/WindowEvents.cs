using SMPL;

namespace TestGame
{
	public class WindowEvents : SMPL.WindowEvents
	{
		public override void OnDraw()
		{
			Camera.WorldCamera.TakePicture();
		}
	}
}
