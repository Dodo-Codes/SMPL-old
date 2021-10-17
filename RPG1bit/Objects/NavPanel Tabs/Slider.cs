using SMPL.Data;
using SMPL.Gear;
using System.Collections.Generic;

namespace RPG1bit
{
	public class Slider : Object
	{
		public int IndexValue { get; set; }
		public int Size { get; set; }
		public bool IsVertical { get; set; }

		private Point lastMousePos;

		public Slider(string uniqueID, CreationDetails creationDetails, int size, bool isVertical) : base(uniqueID, creationDetails)
		{
			Size = size;
			IsVertical = isVertical;
			Game.Event.Subscribe.Update(uniqueID);
			Mouse.Event.Subscribe.ButtonPress(uniqueID);
		}

		public override void OnMouseButtonPress(Mouse.Button button)
		{
			if (button != Mouse.Button.Left || IsHovered() == false) return;
			Trigger(Screen.GetCellAtCursorPosition());
		}
		public override void OnGameUpdate()
		{
			var mousePos = Screen.GetCellAtCursorPosition();
			if (mousePos != lastMousePos && IsHovered())
			{
				OnHovered();
				if (Mouse.ButtonIsPressed(Mouse.Button.Left))
					Trigger(mousePos);
			}
			lastMousePos = mousePos;
		}
		public override void OnDisplay(Point screenPos)
		{
			for (int i = 0; i < Size; i++)
			{
				var pos = screenPos + (IsVertical ? new Point(0, i) : new Point(i, 0));
				var tile = new Point(i == 0
						  ? IsVertical ? IndexValue == i ? 25 : 21 : (IndexValue = IndexValue == i ? 19 : 15)
						  : i == Size - 1
                         ? IsVertical ? IndexValue == i ? 24 : 26 : (IndexValue = IndexValue == i ? 20 : 18)
                         : IsVertical ? IndexValue == i ? 23 : 22 : (IndexValue = IndexValue == i ? 17 : 16), 22);
				Screen.EditCell(pos, tile, 1, Position.C);
				OnDisplayStep(pos, i);
			}
		}

		public bool IsHovered()
		{
			var mousePos = Screen.GetCellAtCursorPosition();
			return (IsVertical && mousePos.X == Position.X && mousePos.Y >= Position.Y && mousePos.Y <= Position.Y + Size) ||
				(IsVertical == false && mousePos.Y == Position.Y && mousePos.X >= Position.X && mousePos.X <= Position.X + Size);
		}

		private void Trigger(Point mousePos)
		{
			IndexValue = (int)(IsVertical ? mousePos.Y - Position.Y : mousePos.X - Position.X);
			OnIndexValueChanged();
			OnDisplay(Position);
		}
		protected virtual void OnDisplayStep(Point screenPos, int step) { }
		protected virtual void OnIndexValueChanged() { }
	}
}
