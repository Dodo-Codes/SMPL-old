using SMPL.Data;
using SMPL.Gear;
using System.Linq;

namespace RPG1bit
{
	public class PlayerSkill : GameObject, ITypeTaggable
	{
		public int Value { get; set; }
		public string Description { get; set; }

		public PlayerSkill(string uniqueID, CreationDetails creationDetails) : base(uniqueID, creationDetails) { }

		public override void OnHovered() => NavigationPanel.Info.Textbox.Text = Description;
		public override void OnDisplay(Point screenPos)
		{
			var value = $"{Value}";
			value = value.Length == 1 ? $"0{value}" : value;
			Screen.DisplayText(screenPos + new Point(1, 0), 1, new Color(0, Number.Map(Value, new(0, 100), new(100, 255)), 0), value);
			Screen.EditCell(screenPos, new(37, 21), 1, Color.Gray);
			Screen.EditCell(screenPos, TileIndexes, 2, TileIndexes.Color);
		}
	}
}
