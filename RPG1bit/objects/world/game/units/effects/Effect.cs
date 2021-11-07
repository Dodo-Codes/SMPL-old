using Newtonsoft.Json;
using SMPL.Data;

namespace RPG1bit
{
	public abstract class Effect : Object
	{
		[JsonProperty]
		public string OwnerUID { get; set; }
		[JsonProperty]
		public int[] Length { get; set; } = new int[2];
		[JsonProperty]
		public int Value { get; set; }

		public Effect(string uniqueID, CreationDetails creationDetails) : base(uniqueID, creationDetails) { }

		public override void OnDisplay(Point screenPos)
		{
			var name = GetType().Name;
			Screen.DisplayText(screenPos + new Point(1, 0), 1, TileIndexes.C, name);
			Screen.EditCell(screenPos, new(37, 21), 1, Color.Gray);
			Screen.EditCell(screenPos, TileIndexes, 2, TileIndexes.C);
		}
		public override void OnAdvanceTime()
		{
			OnTrigger();
			Length[0]++;
			if (Length[0] >= Length[1])
			{
				var owner = (Unit)PickByUniqueID(OwnerUID);
				owner.EffectUIDs.Remove(UniqueID);
				Destroy();
				return;
			}
		}
		public abstract void OnTrigger();
	}
}
