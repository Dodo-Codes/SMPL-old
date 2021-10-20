using SMPL.Gear;
using SMPL.Data;
using SMPL.Components;

namespace RPG1bit
{
	public class Player : Unit
	{
		public Point PreviousPosition { get; private set; }

		public Player(string uniqueID, CreationDetails creationDetails) : base(uniqueID, creationDetails)
		{
			PreviousPosition = Position;
		}
		public override void OnGameUpdate()
		{
			var mousePosMap = Map.ScreenToMapPosition(Screen.GetCellAtCursorPosition());
			Hoverer.TileIndexes = Map.IsHovered()
					 ? CanMoveIntoCell(mousePosMap) ? new(4, 23) : Hoverer.DefaultTileIndexes
					 : Hoverer.DefaultTileIndexes;

			Hoverer.Area.Angle = 0;
			if (CellIsInLeftReach(mousePosMap)) Hoverer.Area.Angle = 270;
			if (CellIsInRightReach(mousePosMap)) Hoverer.Area.Angle = 90;
			if (CellIsInUpReach(mousePosMap)) Hoverer.Area.Angle = 0;
			if (CellIsInDownReach(mousePosMap)) Hoverer.Area.Angle = 180;
		}
		public override void OnMouseButtonRelease(Mouse.Button button)
		{
			base.OnMouseButtonRelease(button);
			var mousePos = Screen.GetCellAtCursorPosition();
			if (Base.LeftClickPosition != mousePos) return;

			var mousePosMap = Map.ScreenToMapPosition(mousePos);
			var movement = new Point(0, 0);

			if (CellIsInLeftReach(mousePosMap)) movement = new Point(-1, 0);
			else if (CellIsInRightReach(mousePosMap)) movement = new Point(1, 0);
			else if (CellIsInUpReach(mousePosMap)) movement = new Point(0, -1);
			else if (CellIsInDownReach(mousePosMap)) movement = new Point(0, 1);

			PreviousPosition = Position;
			Move(movement);

			if (IsRoof(Position) == false && IsRoof(PreviousPosition))
				Map.IsShowingRoofs = true;
			else if (IsRoof(Position) && IsRoof(PreviousPosition) == false)
				Map.IsShowingRoofs = false;

			AdvanceTime();

			bool IsRoof(Point mapPos)
			{
				for (int i = 0; i < 3; i++)
				{
					var tile = Map.RawData.ContainsKey(mapPos) ? Map.RawData[mapPos][i] : new();
					if (MapEditor.RoofTiles.Contains(tile))
						return true;
				}
				return false;
			}
		}
	}
}
