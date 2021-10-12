using SMPL.Gear;

namespace RacerPseudo3D
{
	public class EntryPoint : Game
	{
		static void Main() => Start(new EntryPoint("game"), new(1, 1));

		public EntryPoint(string uniqueID) : base(uniqueID) { }
		public override void OnGameCreate()
		{
			new Road("road");
		}
	}
}
