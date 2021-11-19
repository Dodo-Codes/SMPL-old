using Newtonsoft.Json;
using SMPL.Data;

namespace RPG1bit
{
	public class SlotsExtender : Item
	{
		[JsonProperty]
		public Point SlotsPosition { get; set; } = new Point(0, 12);

		public SlotsExtender(string uniqueID, CreationDetails creationDetails) : base(uniqueID, creationDetails) { }

		public override void OnPickup() => CreateSlots();
		public override void OnDrop() => DestroySlots();
		public override void OnStore() => DestroySlots();
		public override void Destroy()
		{
			base.Destroy();
			DestroySlots();
		}

		public void CreateSlots()
		{
			for (int i = 0; i < Stats["slots"]; i++)
				new ItemSlot($"extra-slot-{SlotsPosition.Y - 12 + i}", new()
				{
					Position = SlotsPosition + new Point(0, i),
					Height = 1,
					TileIndexes = new Point[] { new Point(TileIndexes.X + 1, TileIndexes.Y) { C = Color.Gray } },
					IsUI = true,
					Name = $"extra-slot-{i}"
				});
		}
		public void DestroySlots()
		{
			for (int i = 0; i < Stats["slots"]; i++)
			{
				var id = $"extra-slot-{SlotsPosition.Y - 12 + i}";
				if (UniqueIDsExists(id) == false)
					continue;
				var slot = (ItemSlot)PickByUniqueID(id);
				slot.Destroy();
			}
		}
	}
}
