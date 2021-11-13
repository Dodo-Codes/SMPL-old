using SMPL.Data;

namespace RPG1bit
{
	public class AdjustVolume : Object
	{
		public int Percent { get; set; } = 50;

		public AdjustVolume(string uniqueID, CreationDetails creationDetails) : base(uniqueID, creationDetails) { }
		public override void OnHovered() => NavigationPanel.Info.Textbox.Text = $"[LEFT CLICK] Adjust the\n{Name.ToLower()} volume";
		public override void OnLeftClicked()
		{
			Percent += 10;
			if (Percent == 110) Percent = 0;
			var c = Number.Map(Percent, new Number.Range(0, 100), new Number.Range(100, 255));
			TileIndexes = new(TileIndexes.X, TileIndexes.Y) { C = new(c, c, c) };
			NavigationPanel.Info.Textbox.Text = $"{Name} volume: {Percent}%";
			ValueChanged();
		}

		public virtual void ValueChanged() { }
	}
}
