using SMPL.Data;
using SMPL.Gear;
using System.Collections.Generic;
using System.IO;

namespace RPG1bit
{
	public class LoadSingleSessionValue : GameObject
	{
		public LoadSingleSessionValue(string uniqueID, CreationDetails creationDetails) : base(uniqueID, creationDetails)
		{
			Assets.Event.Subscribe.LoadEnd(uniqueID, 0);
		}

		public override void OnAssetsLoadEnd()
		{
			if (Gate.EnterOnceWhile("load-worlddd", Assets.ValuesAreLoaded("world-name")))
				World.LoadWorld(World.Session.Single, Assets.GetValue("world-name"));
		}
		public override void OnLeftClicked()
		{
			DestroyAllSessionObjects();
			Assets.Load(Assets.Type.DataSlot, $"sessions\\{Name}\\{Name}.session");
		}
		public override void OnRightClicked()
		{
			File.Move($"sessions\\{Name}\\{Name}.session", $"deleted\\{Name}\\{Name}.session");

			if (GameObjectList.Lists.ContainsKey("load-list")) RemoveFromList(GameObjectList.Lists["load-list"]);
			StartSingle.UpdateTab();
			SaveLoad.UpdateTab();

			void RemoveFromList(GameObjectList list)
			{
				for (int i = 0; i < list.Objects.Count; i++)
				{
					if (list.Objects[i].Name != Name) continue;
					if (list.UniqueID == "load-list" && list.Objects[i] is not LoadSingleSessionValue) continue;
					list.Objects.Remove(list.Objects[i]);
					list.ScrollToTop();
					return;
				}
			}
		}
		public override void OnHovered()
		{
			NavigationPanel.Info.Textbox.Text = $"Game session: '{Name.ToUpper()}'\n\n[LEFT CLICK] to load\n[RIGHT CLICK] to delete";
			NavigationPanel.Info.ShowLeftClickableIndicator();
		}
		public override void OnDisplay(Point screenPos)
		{
			Screen.EditCell(screenPos, TileIndexes, 1, Color.Fire);
			Screen.DisplayText(screenPos + new Point(1, 0), 1, Color.White, Name);
		}
	}
}
