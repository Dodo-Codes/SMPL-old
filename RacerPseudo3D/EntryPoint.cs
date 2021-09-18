using SMPL.Gear;

namespace RacerPseudo3D
{
	public class EntryPoint : Game
	{
		static void Main() => Start(new EntryPoint(), new(1, 1));

		public override void OnGameCreated()
		{
			Road.Create();
		}
	}
}
