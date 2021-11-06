using SMPL.Data;
using SMPL.Gear;
using System.IO;

namespace RPG1bit
{
	public class LoadWorldValue : Object
	{
		public LoadWorldValue(string uniqueID, CreationDetails creationDetails) : base(uniqueID, creationDetails) { }

		public override void OnLeftClicked()
		{
			World.LoadWorld(World.Session.WorldEdit, Name);

			if (World.CurrentSession == World.Session.WorldEdit)
				NavigationPanel.Tab.Open("world-editor", "edit brush");
		}
		public override void OnRightClicked()
		{
			var name = $"deleted\\{Name}";
			while (Directory.Exists(name))
				name += "0";
			var files = Directory.GetFiles($"worlds\\{Name}");
			for (int i = 0; i < files.Length; i++)
				if (files[i] == $"worlds\\{Name}\\{Name}.worlddata")
					File.Move($"worlds\\{Name}\\{Name}.worlddata", $"worlds\\{Name}\\{Path.GetFileName(name)}.worlddata");
			Directory.Move($"worlds\\{Name}", name);

			if (ObjectList.Lists.ContainsKey("load-list")) RemoveFromList(ObjectList.Lists["load-list"]);
			if (ObjectList.Lists.ContainsKey("load-world-list")) RemoveFromList(ObjectList.Lists["load-world-list"]);
			StartSingle.UpdateTab();
			SaveLoad.UpdateTab();

			void RemoveFromList(ObjectList list)
			{
				for (int i = 0; i < list.Objects.Count; i++)
				{
					if (list.Objects[i].Name != Name) continue;
					if (list.UniqueID == "load-list" && list.Objects[i] is not LoadWorldValue) continue;
					list.Objects.Remove(list.Objects[i]);
					list.ScrollToTop();
					return;
				}
			}
		}
		public override void OnHovered()
		{
			NavigationPanel.Info.Textbox.Text = $"World Edit session: '{Name.ToUpper()}'\n\n[LEFT CLICK] Load\n[RIGHT CLICK] Delete";
			NavigationPanel.Info.ShowLeftClickableIndicator();
			NavigationPanel.Info.ShowRightClickableIndicator();
		}
		public override void OnDisplay(Point screenPos)
		{
			Screen.EditCell(screenPos, TileIndexes, 1, Color.Brown + 100);
			Screen.DisplayText(screenPos + new Point(1, 0), 1, Color.White, Name);
		}
	}
}
