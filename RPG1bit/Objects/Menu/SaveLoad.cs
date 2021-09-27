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
			NavigationPanel.Tab.CurrentTabType = NavigationPanel.Tab.Type.SaveLoad;

			if (Gate.EnterOnceWhile("create-save-load-tab", true))
			{
				new SaveNameInput(new CreationDetails()
				{
					Name = "save-session",
					Position = new(30, 2),
					TileIndexes = new Point[] { new Point(42, 16) },
					Height = 1,
					IsUI = true,
					IsLeftClickable = true,
					IsInTab = true,
					AppearOnTab = NavigationPanel.Tab.Type.SaveLoad,
				});
			}
			DisplayAllObjects();
		}
	}
}
