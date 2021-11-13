using SMPL.Data;
using SMPL.Gear;
using System.Linq;

namespace RPG1bit
{
	public class PlayerSkill : Object, ITypeTaggable
	{
		public int Value { get; set; }
		public string Description { get; set; }

		public PlayerSkill(string uniqueID, CreationDetails creationDetails) : base(uniqueID, creationDetails) { }

		public override void OnHovered() => NavigationPanel.Info.Textbox.Text = Description;
		public override void OnDisplay(Point screenPos)
		{
			Screen.DisplayText(screenPos, 1, Color.White, $"{Name} {Value}");
		}
	}
}
