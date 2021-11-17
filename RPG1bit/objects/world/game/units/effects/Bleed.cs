using SMPL.Data;
using SMPL.Gear;

namespace RPG1bit
{
	public class Bleed : Effect
	{
		public Bleed(string uniqueID, CreationDetails creationDetails) : base(uniqueID, creationDetails)
		{
			TileIndexes = new(27, 22) { C = Color.Blood };
		}

		public override void OnHovered()
		{
			NavigationPanel.Info.Textbox.Text =
				$"Bleeding: This unit's state changes by {Value}\n" +
				$"\teach turn for the next {Duration[1] - Duration[0]} turns";
		}
		public override void Destroy()
		{
			Gate.Remove($"{OwnerUID}-trail");
			base.Destroy();
		}
		public override void OnTrigger()
		{
			var unit = (Unit)PickByUniqueID(OwnerUID);
			if (Gate.EnterOnceWhile($"{OwnerUID}-trail", true))
			{
				new Trail($"{UniqueID}-trail", new()
				{
					Position = unit.Position,
					Height = 3,
					Name = "trail",
					TileIndexes = new Point[] { new(23, 23) { C = Color.Blood } }
				})
				{
					OwnerUID = OwnerUID,
					Duration = Duration[1],
					Length = 3
				};
			}

			unit.Health[0] += Value;
		}
	}
}
