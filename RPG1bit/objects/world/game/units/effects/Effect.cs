using Newtonsoft.Json;
using SMPL.Data;

namespace RPG1bit
{
	public abstract class Effect : Object
	{
		[JsonProperty]
		public string OwnerUID { get; set; }
		[JsonProperty]
		public int[] Duration { get; set; } = new int[2];
		[JsonProperty]
		public int Value { get; set; }

		public Effect(string uniqueID, CreationDetails creationDetails) : base(uniqueID, creationDetails) { }

		public override void OnDisplay(Point screenPos)
		{
			Screen.DisplayText(screenPos + new Point(1, 0), 1, Value < 0 ? Color.Red : Color.Green, $"{Duration[1] - Duration[0]}");
			Screen.EditCell(screenPos, new(37, 21), 1, Color.Gray);
			Screen.EditCell(screenPos, TileIndexes, 2, TileIndexes.C);
		}
		public override void OnAdvanceTime()
		{
			OnTrigger();
			Duration[0]++;
			PlayerStats.Open();
			if (Duration[0] >= Duration[1])
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
