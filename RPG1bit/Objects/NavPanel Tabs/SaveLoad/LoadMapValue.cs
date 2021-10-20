using SMPL.Data;
using SMPL.Gear;

namespace RPG1bit
{
	public class LoadMapValue : Object
	{
		public LoadMapValue(string uniqueID, CreationDetails creationDetails) : base(uniqueID, creationDetails) { }

		public override void OnLeftClicked()
		{
			Map.LoadMap(Map.Session.MapEdit, Name);

			if (Map.CurrentSession == Map.Session.MapEdit)
				NavigationPanel.Tab.Open("map-editor", "edit brush");
		}
		public override void OnRightClicked()
		{
			FileSystem.DeleteFiles($"Maps\\{Name}.mapdata");

			if (ObjectList.Lists.ContainsKey("load-list")) RemoveFromList(ObjectList.Lists["load-list"]);
			if (ObjectList.Lists.ContainsKey("load-map-list")) RemoveFromList(ObjectList.Lists["load-map-list"]);

			void RemoveFromList(ObjectList list)
			{
				for (int i = 0; i < list.Objects.Count; i++)
				{
					if (list.Objects[i].Name != Name) continue;
					list.Objects.Remove(list.Objects[i]);
					list.ScrollToTop();
					return;
				}
			}
		}
		public override void OnHovered()
		{
			NavigationPanel.Info.Textbox.Text = $"[LMB] Load / [RMB] Delete\nMap Edit session: '{Name.ToUpper()}'";
			NavigationPanel.Info.ShowClickableIndicator();
			NavigationPanel.Info.ShowLeftClickableIndicator();
		}
		public override void OnDisplay(Point screenPos)
		{
			Screen.EditCell(screenPos, TileIndexes, 1, Color.White);
			Screen.DisplayText(screenPos + new Point(1, 0), 1, Color.White, Name);
		}
	}
}
