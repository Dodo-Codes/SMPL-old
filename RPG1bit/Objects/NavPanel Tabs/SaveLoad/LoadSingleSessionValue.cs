using SMPL.Data;
using SMPL.Gear;
using System.Collections.Generic;

namespace RPG1bit
{
	public class LoadSingleSessionValue : Object
	{
		public LoadSingleSessionValue(string uniqueID, CreationDetails creationDetails) : base(uniqueID, creationDetails)
		{
			Assets.Event.Subscribe.LoadEnd(uniqueID, 0);
		}

		public override void OnAssetsLoadEnd()
		{
			if (Assets.ValuesAreLoaded("map-name") == false) return;

			Map.DestroyAllSessionObjects();
			Map.LoadMap(Map.Session.Single, Assets.GetValue("map-name"));
		}
		public override void OnLeftClicked()
		{
			Assets.Load(Assets.Type.DataSlot, $"Sessions\\{Name}.session");
		}
		public override void OnRightClicked()
		{
			FileSystem.MoveFiles($"Deleted", $"Sessions\\{Name}.session");

			if (ObjectList.Lists.ContainsKey("load-list")) RemoveFromList(ObjectList.Lists["load-list"]);

			void RemoveFromList(ObjectList list)
			{
				for (int i = 0; i < list.Objects.Count; i++)
				{
					if (list.Objects[i].Name != Name || list.Objects[i] is not LoadSingleSessionValue) continue;
					list.Objects.Remove(list.Objects[i]);
					list.ScrollToTop();
					return;
				}
			}
		}
		public override void OnHovered()
		{
			NavigationPanel.Info.Textbox.Text = $"[LMB] Load / [RMB] Delete\nGame session: '{Name.ToUpper()}'";
			NavigationPanel.Info.ShowLeftClickableIndicator();
		}
		public override void OnDisplay(Point screenPos)
		{
			Screen.EditCell(screenPos, TileIndexes, 1, Color.White);
			Screen.DisplayText(screenPos + new Point(1, 0), 1, Color.White, Name);
		}
	}
}
