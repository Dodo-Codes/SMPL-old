using SMPL.Gear;
using SMPL.Data;
using System.Collections.Generic;

namespace RPG1bit
{
	public class Player : Unit
	{
		public Point PreviousPosition { get; private set; }

		public Player(string uniqueID, CreationDetails creationDetails) : base(uniqueID, creationDetails)
		{
			PreviousPosition = Position;

			Map.IsShowingRoofs = Map.TileHasRoof(Position) == false;

			var key = new Key("item-key", new()
			{
				Name = "Key",
				Position = new(-10, 0),
				Height = 2,
				TileIndexes = new Point[]
				{ new(32, 11) { C = Color.Gray }, new(33, 11) { C = Color.Gray }, new(34, 11) { C = Color.Gray } },
				IsUI = true,
				IsDragable = true,
				IsRightClickable = true,
				IsLeftClickable = true,
			});
			var quiver = new Quiver("quiver", new()
			{
				Name = "Quiver",
				Position = new(-10, 0),
				Height = 2,
				TileIndexes = new Point[] { new(42, 6) { C = Color.Brown + 30 } },
				IsUI = true,
				IsDragable = true,
				IsRightClickable = true,
				IsLeftClickable = true,
			}) { Positives = new double[] { 1, 0 } };
			var bag = new Bag("bag", new()
			{
				Name = "Bag",
				Position = new(-10, 0),
				Height = 2,
				TileIndexes = new Point[] { new(44, 4) { C = Color.Brown + 30 } },
				IsUI = true,
				IsDragable = true,
				IsRightClickable = true,
				IsLeftClickable = true,
			}) { Positives = new double[] { 2, 0 } };
			var pile = new ItemPile("item-pile", new()
			{
				Position = Position + new Point(0, -2),
				Height = 3,
				TileIndexes = new Point[] { new(8, 23) },
				Name = "item-pile",
			});
			pile.AddItem(key);
			pile.AddItem(quiver);
			pile.AddItem(bag);
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
			if (movement != new Point() && Move(movement)) AdvanceTime();

			if (IsRoof(Position) == false && IsRoof(PreviousPosition))
				Map.IsShowingRoofs = true;
			else if (IsRoof(Position) && IsRoof(PreviousPosition) == false)
				Map.IsShowingRoofs = false;

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
