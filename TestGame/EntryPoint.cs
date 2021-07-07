using SMPL;

namespace TestGame
{
	public class EntryPoint
	{
		public static void Main()
		{
			var events = new Game.Events()
			{
				windowEvents = new WindowEvents(),
				worldCameraEvents = new WorldCameraEvents(),
				fileEvents = new FileEvents()
			};
			Game.Start(events);
		}
	}
}
