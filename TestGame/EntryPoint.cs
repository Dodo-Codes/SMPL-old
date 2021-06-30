namespace TestGame
{
	public class EntryPoint
	{
		public static void Main() => SMPL.Game.Start(
			window: new Window(),
			time: new Time());
	}
}
