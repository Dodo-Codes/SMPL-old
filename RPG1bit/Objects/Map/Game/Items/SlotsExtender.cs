using SMPL.Data;

namespace RPG1bit
{
	public class SlotsExtender : Item
	{
		public Point SlotsPosition { get; set; } = new Point(0, 14);

		public SlotsExtender(string uniqueID, CreationDetails creationDetails) : base(uniqueID, creationDetails) { }

		public override void OnPickup()
		{
			for (int i = 0; i < Positives[0]; i++)
				new ItemSlot($"extra-slot-{SlotsPosition.Y - 14 + i}", new()
				{
					Position = SlotsPosition + new Point(0, i),
					Height = 1,
					TileIndexes = new Point[] { TileIndexes + new Point(1, 0) },
					IsUI = true,
					Name = $"extra-slot-{i}"
				});
		}
		public override void OnDrop() => DestroySlots();
		public override void Destroy()
		{
			base.Destroy();
			DestroySlots();
		}
		public void DestroySlots()
		{
			for (int i = 0; i < Positives[0]; i++)
			{
				var slot = (ItemSlot)PickByUniqueID($"extra-slot-{SlotsPosition.Y - 14 + i}");
				if (slot != null)
					slot.Destroy();
			}
		}
	}
}
