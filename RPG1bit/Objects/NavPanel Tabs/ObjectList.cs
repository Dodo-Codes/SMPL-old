using SMPL.Data;
using SMPL.Gear;
using System.Collections.Generic;

namespace RPG1bit
{
	public class ObjectList : Object
	{
		public static Dictionary<string, ObjectList> Lists { get; } = new();

		public Size Size { get; private set; }
		public List<Object> Objects { get; set; } = new();
		public int scrollIndex;
		private Point lastMousePos;

		public ObjectList(CreationDetails creationDetails, Size size) : base(creationDetails)
		{
			Lists[Name] = this;
			Mouse.CallWhen.ButtonRelease(OnButtonRelease);
			Mouse.CallWhen.WheelScroll(OnWheelScroll);
			Size = size;
			Game.CallWhen.Running(Always);
		}

		private void OnWheelScroll(Mouse.Wheel wheel, double delta)
		{
			if (wheel != Mouse.Wheel.Vertical || IsHovered() == false) return;
			if (delta == 1) ScrollUp();
			else if (delta == -1) ScrollDown();

			Screen.Display();
		}

		private void Always()
		{
			if (AppearOnTab != NavigationPanel.Tab.CurrentTabType) return;

			var mousePos = Screen.GetCellAtCursorPosition();
			if (mousePos != lastMousePos)
			{
				if (Objects.Count >= Size.H)
				{
					if (mousePos == new Point(Position.X + Size.W / 2, Position.Y) && scrollIndex > 0)
						NavigationPanel.Info.Textbox.Text = "Scroll up the list.";
					else if (mousePos == new Point(Position.X + Size.W / 2, Position.Y + Size.H) &&
						scrollIndex < Objects.Count - (int)Size.H + 1)
						NavigationPanel.Info.Textbox.Text = "Scroll down the list.";
				}
				if (IsHovered())
				{
					var index = (int)(mousePos.Y - Position.Y + scrollIndex) - 1;
					if (Objects.Count > index)
					{
						Objects[index].OnHovered();
						NavigationPanel.Info.ShowClickableIndicator(Objects[index].IsLeftClickable || Objects[index].IsRightClickable);
						NavigationPanel.Info.ShowLeftClickableIndicator(Objects[index].IsLeftClickable);
						NavigationPanel.Info.ShowRightClickableIndicator(Objects[index].IsRightClickable);
					}
				}
			}
			lastMousePos = mousePos;
		}
		private void OnButtonRelease(Mouse.Button button)
		{
			if (AppearOnTab != NavigationPanel.Tab.CurrentTabType) return;
			var mousePos = Screen.GetCellAtCursorPosition();
			if (button == Mouse.Button.Left && mousePos == new Point(Position.X + Size.W / 2, Position.Y) &&
				LeftClickPosition == mousePos)
				ScrollUp();
			else if (button == Mouse.Button.Left && mousePos == new Point(Position.X + Size.W / 2, Position.Y + Size.H) &&
				LeftClickPosition == mousePos)
				ScrollDown();
			else if (IsHovered())
			{
				var index = (int)(mousePos.Y - Position.Y + scrollIndex) - 1;
				if (Objects.Count <= index) return;
				if (button == Mouse.Button.Left && Objects[index].IsLeftClickable && LeftClickPosition.Y == mousePos.Y)
					Objects[index].OnLeftClicked();
				else if (button == Mouse.Button.Right && RightClickPosition.Y == mousePos.Y && Objects[index].IsLeftClickable)
					Objects[index].OnRightClicked();
			}
			Screen.Display();
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
				var iconIndexes = new Point(0, 0);
				if (Objects[i] is LoadMapValue) iconIndexes = new Point(47, 06);
				if (Objects[i] is StartSingleOnMap) iconIndexes = new Point(32, 15);
				Screen.EditCell(pos - new Point(1, 0), iconIndexes, 2, Color.White);
				Screen.DisplayText(pos, 2, Color.White, Objects[i].Name);
			}
			DisplayScrollArrows();
		}
		public void DisplayScrollArrows()
		{
			if (Objects.Count < Size.H) return;

			if (scrollIndex > 0) Screen.EditCell(Position + new Point(Size.W / 2, 0), new Point(23, 20), 1, Color.White);
			if (scrollIndex < Objects.Count - (int)Size.H + 1)
				Screen.EditCell(Position + new Point(Size.W / 2, Size.H), new Point(25, 20), 1, Color.White);
		}

		public void ScrollToTop()
		{
			scrollIndex = 0;
		}
		public void ScrollToBottom()
		{
			scrollIndex = Objects.Count - (int)Size.H + 1;
			if (scrollIndex < 0) scrollIndex = 0;
		}
		public void ScrollUp()
		{
			scrollIndex -= scrollIndex == 0 ? 0 : 1;
		}
		public void ScrollDown()
		{
			scrollIndex += scrollIndex == Objects.Count || Objects.Count <= Size.H + scrollIndex - 1 ? 0 : 1;
		}

		public bool IsHovered()
		{
			var mousePos = Screen.GetCellAtCursorPosition();
			return mousePos.X >= Position.X && mousePos.X <= Position.X + Size.W &&
				mousePos.Y >= Position.Y + 1 && mousePos.Y <= Position.Y + Size.H - 1;
		}
	}
}
