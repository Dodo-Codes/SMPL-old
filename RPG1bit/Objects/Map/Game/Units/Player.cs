using SMPL.Gear;
using SMPL.Data;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace RPG1bit
{
	public class Player : Unit
	{
		[JsonProperty]
		public Point PreviousPosition { get; private set; }

		public Player(string uniqueID, CreationDetails creationDetails) : base(uniqueID, creationDetails)
		{
			PreviousPosition = Position;

			Map.IsShowingRoofs = Map.TileHasRoof(Position) == false;
		}
		public override void OnGameUpdate()
		{
			base.OnGameUpdate();

			var mousePosMap = Map.ScreenToMapPosition(Screen.GetCellAtCursorPosition());
			Hoverer.TileIndexes = Hoverer.DefaultTileIndexes;

			if (Map.IsHovered() && CellIsInReach(mousePosMap))
			{
				if (CanMoveIntoCell(mousePosMap))
				{
					Hoverer.TileIndexes = new(4, 23);
					Hoverer.Area.Angle = 0;
					if (CellIsInLeftReach(mousePosMap)) Hoverer.Area.Angle = 270;
					if (CellIsInRightReach(mousePosMap)) Hoverer.Area.Angle = 90;
					if (CellIsInUpReach(mousePosMap)) Hoverer.Area.Angle = 0;
					if (CellIsInDownReach(mousePosMap)) Hoverer.Area.Angle = 180;
				}

				var objs = objects.ContainsKey(mousePosMap) ? objects[mousePosMap] : new();
				for (int i = 0; i < objs.Count; i++)
					if (objs[i] is IInteractable)
					{
						Hoverer.TileIndexes = new(27, 14);
						return;
					}
			}
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
			if (movement != new Point() && Move(movement))
			{
				TileIndexes = Map.PositionHasWaterAsHighest(Position) ? new(20, 23) : new(25, 0);
				AdvanceTime();
			}

			if (IsRoof(Position) == false && IsRoof(PreviousPosition))
				Map.IsShowingRoofs = true;
			else if (IsRoof(Position) && IsRoof(PreviousPosition) == false)
				Map.IsShowingRoofs = false;

			bool IsRoof(Point mapPos)
			{
				for (int i = 0; i < 3; i++)
				{
					var tile = Map.RawData.ContainsKey(mapPos) ? Map.RawData[mapPos][i] : new();
					if (MapEditor.Tiles["roof"].Contains(tile))
						return true;
				}
				return false;
			}
		}
	}
}
