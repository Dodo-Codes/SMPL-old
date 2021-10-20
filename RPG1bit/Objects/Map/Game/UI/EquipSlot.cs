using SMPL.Gear;

namespace RPG1bit
{
	public class ItemSlot : Object
	{
		public ItemSlot(string uniqueID, CreationDetails creationDetails) : base(uniqueID, creationDetails) { }

		public override void OnDroppedUpon()
		{
			var asd = HoldingObject;
		}
	}
}
