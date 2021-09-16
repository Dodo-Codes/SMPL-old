using SMPL.Data;

namespace RPG1bit.Objects
{
	public class AdjustSound : Object
	{
		public static int Percent { get; set; } = 50;

		public AdjustSound(CreationDetails creationDetails) : base(creationDetails) { }
		protected override void OnHovered() => NavigationPanel.Info.ShowClickableIndicator();
		protected override void OnLeftClicked()
		{
			Percent += 10;
			if (Percent == 110) Percent = 0;
			Screen.EditCell(Position, TileIndex, 0,
				new Color(255, 255, 255, Number.Map(Percent, new Number.Range(0, 100), new Number.Range(125, 255))));
			NavigationPanel.Info.Textbox.Text = $"Sound effects volume: {Percent}%";
		}
	}
}
