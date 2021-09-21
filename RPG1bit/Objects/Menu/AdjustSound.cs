using SMPL.Data;

namespace RPG1bit
{
	public class AdjustSound : Object
	{
		public static int Percent { get; set; } = 50;

		public AdjustSound(CreationDetails creationDetails) : base(creationDetails) { }
		protected override void OnHovered() => NavigationPanel.Info.Textbox.Text = "Adjust the sound effects volume.";
		protected override void OnLeftClicked()
		{
			Percent += 10;
			if (Percent == 110) Percent = 0;
			var c = Number.Map(Percent, new Number.Range(0, 100), new Number.Range(100, 255));
			Screen.EditCell(Position, TileIndexes, 2, new Color(c, c, c));
			NavigationPanel.Info.Textbox.Text = $"Sound effects volume: {Percent}%";
		}
	}
}
