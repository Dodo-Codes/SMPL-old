using SMPL.Data;
using SMPL.Gear;

namespace RPG1bit
{
	public class LoadMapValue : Object
	{
		public LoadMapValue(CreationDetails creationDetails) : base(creationDetails) { }

		public override void OnLeftClicked()
		{
			if (Assets.ValuesAreLoaded("map-data", "camera-position")) Assets.UnloadValues("map-data", "camera-position");
			Assets.Load(Assets.Type.DataSlot, $"Maps\\{Name}.mapdata");
			NavigationPanel.Tab.Close();
			NavigationPanel.Info.Textbox.Text = descriptions[new(0, 23)];
		}
		public override void OnHovered()
		{
			NavigationPanel.Info.Textbox.Text = $"Left click to load map: '{Name.ToUpper()}'";
			NavigationPanel.Info.ShowClickableIndicator();
			NavigationPanel.Info.ShowLeftClickableIndicator();
		}
	}
}
