using SMPL.Gear;
using SMPL.Data;

namespace RPG1bit
{
	public class SaveLoad : Object
	{
		public SaveLoad(CreationDetails creationDetails) : base(creationDetails) { }
		protected override void OnHovered() => NavigationPanel.Info.Textbox.Text = "Save/Load a session.";
		protected override void OnLeftClicked()
		{
			for (int i = 0; i < 10; i++)
			{
				Screen.EditCell(new Point(19 + i, 2), new Point(13, 22), 1, Color.Gray);
			}
		}
	}
}
