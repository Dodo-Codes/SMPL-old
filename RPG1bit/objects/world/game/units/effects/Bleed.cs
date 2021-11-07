using SMPL.Data;

namespace RPG1bit
{
	public class Bleed : Effect
	{
		public Bleed(string uniqueID, CreationDetails creationDetails) : base(uniqueID, creationDetails)
		{
			TileIndexes = new(27, 22) { C = Color.Red };
		}

		public override void OnTrigger()
		{
			var unit = (Unit)PickByUniqueID(OwnerUID);
			unit.Health[0]--;
		}
	}
}
