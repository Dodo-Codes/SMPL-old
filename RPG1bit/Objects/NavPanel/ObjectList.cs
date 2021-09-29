using SMPL.Data;
using SMPL.Gear;
using System.Collections.Generic;

namespace RPG1bit
{
	public class ObjectList : Object
	{
		public Size Size { get; private set; }
		public List<Object> Objects { get; set; } = new();
		private int scrollIndex;
		private Point lastMousePos;

		public ObjectList(CreationDetails creationDetails, Size size) : base(creationDetails)
		{
			Mouse.CallWhen.ButtonPress(OnButtonPress);
			Size = size;
			Game.CallWhen.Running(Always);
		}

		private void Always()
		{
			if (AppearOnTab != NavigationPanel.Tab.CurrentTabType) return;

			var mousePos = Screen.GetCellAtCursorPosition();
			if (mousePos != lastMousePos)
			{
				if (Objects.Count > Size.H)
				{
					if (mousePos == new Point(Position.X + Size.W / 2, Position.Y))
						NavigationPanel.Info.Textbox.Text = "Scroll up the list.";
					else if (mousePos == new Point(Position.X + Size.W / 2, Position.Y + Size.H))
						NavigationPanel.Info.Textbox.Text = "Scroll down the list.";
				}
				if (IsHovered())
				{
					var index = (int)(mousePos.Y - Position.Y + scrollIndex) - 1;
					if (Objects.Count > index) Objects[index].OnHovered();
				}
			}
			lastMousePos = mousePos;
		}
		private void OnButtonPress(Mouse.Button button)
		{
			var mousePos = Screen.GetCellAtCursorPosition();
			if (mousePos == new Point(Position.X + Size.W / 2, Position.Y))
				scrollIndex -= scrollIndex == 0 ? 0 : 1;
			else if (mousePos == new Point(Position.X + Size.W / 2, Position.Y + Size.H))
				scrollIndex += scrollIndex == Objects.Count || Objects.Count <= Size.H + scrollIndex - 1 ? 0 : 1;
			else if (IsHovered())
			{
				var index = (int)(mousePos.Y - Position.Y + scrollIndex) - 1;
				if (Objects.Count <= index) return;
				if (button == Mouse.Button.Left) Objects[index].OnLeftClicked();
				else if (button == Mouse.Button.Right)
				{
					if (Objects[index] is LoadMapValue) FileSystem.DeleteFiles($"Maps\\{Objects[index].Name}.mapdata");
					Objects.RemoveAt(index);
				}
			}
			NavigationPanel.Display();
			NavigationPanel.Info.Display();
			DisplayAllObjects();
		}
		public override void OnDisplay()
		{
			if (AppearOnTab != NavigationPanel.Tab.CurrentTabType) return;
			for (int y = 0; y < (int)Size.H + 1; y++)
				for (int x = 0; x < (int)Size.W + 1; x++)
				{
					if (Objects.Count < y) { y = 1000; break; }
					Screen.EditCell(Position + new Point(x, y), new Point(1, 22), 0,
						y == 0 || y == Size.H ? Color.BrownDark : (y + scrollIndex) % 2 == 0 ? Color.Brown : Color.BrownLight);
				}
			for (int i = 0; i < Objects.Count; i++)
			{
				var y = Position.Y + i - scrollIndex + 1;
				if (y > Position.Y + Size.H - 1) break;
				if (y < Position.Y + 1) continue;
				var pos = new Point(Position.X + 1, y);
				Screen.DisplayText(pos, 2, Color.White, Objects[i].Name);
			}
			if (Objects.Count > Size.H)
			{
				Screen.EditCell(Position + new Point(Size.W / 2, 0), new Point(23, 20), 1, Color.White);
				Screen.EditCell(Position + new Point(Size.W / 2, Size.H), new Point(25, 20), 1, Color.White);
			}
		}
		public override void OnHovered()
		{
			var mousePos = Screen.GetCellAtCursorPosition();
			var index = (int)(mousePos.Y - Position.Y + scrollIndex);
			if (Objects.Count <= index) return;
			Objects[index].OnHovered();
		}

		public bool IsHovered()
		{
			var mousePos = Screen.GetCellAtCursorPosition();
			return mousePos.X >= Position.X && mousePos.X <= Position.X + Size.W &&
				mousePos.Y >= Position.Y + 1 && mousePos.Y <= Position.Y + Size.H - 1;
		}
	}
}
