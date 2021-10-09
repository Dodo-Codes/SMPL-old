using SMPL.Data;
using SMPL.Gear;

namespace RPG1bit
{
	public class Unit : Object
	{
		public Unit(string uniqueID, CreationDetails creationDetails) : base(uniqueID, creationDetails) { }

		public void Move(Point movement)
		{
			var futurePos = Position + movement;
			if (CanMoveIntoCell(futurePos) == false) return;

			Position = futurePos;
			if (UniqueID == "player" && MoveCamera.IsAnchored)
				Map.CameraPosition += movement;

			Map.Display();
			DisplayAllObjects();
		}
		public bool CellIsInReach(Point cell)
		{
			var left = cell.X == Position.X - 1 && cell.Y == Position.Y;
			var right = cell.X == Position.X + 1 && cell.Y == Position.Y;
			var top = cell.X == Position.X && cell.Y == Position.Y - 1;
			var down = cell.X == Position.X && cell.Y == Position.Y + 1;
			return left || right || top || down;
		}
		public bool CellIsInLeftReach(Point cell) => cell.X == Position.X - 1 && cell.Y == Position.Y;
		public bool CellIsInRightReach(Point cell) => cell.X == Position.X + 1 && cell.Y == Position.Y;
		public bool CellIsInUpReach(Point cell) => cell.X == Position.X && cell.Y == Position.Y - 1;
		public bool CellIsInDownReach(Point cell) => cell.X == Position.X && cell.Y == Position.Y + 1;

		public bool CanMoveIntoCell(Point cell)
		{
			return CellIsInReach(cell) && Map.RawData[(int)cell.X, (int)cell.Y, 3] != Map.TileBarrier;
		}
	}
}
