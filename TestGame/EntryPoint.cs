namespace TestGame
{
	public class EntryPoint
	{
		public static void Main() => SMPL.Game.Start(
			window: new WindowEvents(),
			keyboard: new KeyboardInput(),
			time: new TimeEvents());
	}
}
