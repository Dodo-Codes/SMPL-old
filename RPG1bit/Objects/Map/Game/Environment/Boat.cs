using SMPL.Data;

namespace RPG1bit
{
	public class Boat : Object
	{
		public Boat(string uniqueID, CreationDetails creationDetails) : base(uniqueID, creationDetails) { }

		public override void OnAdvanceTime()
		{
			var player = (Player)PickByUniqueID("player");
			if (player.PreviousPosition == Position)
				Position = player.Position;
		}
	}
}
