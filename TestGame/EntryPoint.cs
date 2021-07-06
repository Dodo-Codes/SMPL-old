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
				timeEvents = new TimeEvents(),
				fileEvents = new FileEvents()
			};
			Game.Start(events);
		}
	}
}
