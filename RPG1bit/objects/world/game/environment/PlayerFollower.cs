using SMPL.Components;
using SMPL.Data;
using System.Collections.Generic;

namespace RPG1bit
{
	public class PlayerFollower : GameObject
	{
		public bool Following { get; set; }

		public PlayerFollower(string uniqueID, CreationDetails creationDetails) : base(uniqueID, creationDetails) { }

		public override void OnAdvanceTime()
		{
			if (Following == false)
				return;
			var player = (Player)PickByUniqueID(nameof(Player));
			var lines = new List<Line>();

			for (double y = World.CameraPosition.Y - 8; y < World.CameraPosition.Y + 9; y++)
				for (double x = World.CameraPosition.X - 8; x < World.CameraPosition.X + 9; x++)
				{
					var isBarrier = ChunkManager.GetTile(new(x, y), 3) == World.TileBarrier;
					if (isBarrier == false)
						continue;

					var scrPos = RealPos(new(x, y));
					lines.Add(new(scrPos + new Point(-30, -30), scrPos + new Point(30, 30)));
					lines.Add(new(scrPos + new Point(30, -30), scrPos + new Point(-30, 30)));
				}
			var playerRealPos = RealPos(player.PreviousPosition);
			var pathPoint = Point.Pathfind(RealPos(Position), playerRealPos, 100, lines.ToArray());
			if (pathPoint.IsInvalid())
				return;
			var pathPointWorld = WorldPos(pathPoint);
			var longestDist = 0.0;
			var bestWay = new Point();

			if (pathPointWorld.X < Position.X && IsBarrier(new(-1, 0)) == false &&
				longestDist < Number.Sign(Position.X - pathPointWorld.X, false))
			{
				longestDist = Number.Sign(Position.X - pathPointWorld.X, false);
				bestWay = new Point(-1, 0);
			}
			if (pathPointWorld.X > Position.X && IsBarrier(new(1, 0)) == false &&
				longestDist < Number.Sign(Position.X - pathPointWorld.X, false))
			{
				longestDist = Number.Sign(Position.X - pathPointWorld.X, false);
				bestWay = new Point(1, 0);
			}
			if (pathPointWorld.Y < Position.Y && IsBarrier(new(0, -1)) == false &&
				longestDist < Number.Sign(Position.Y - pathPointWorld.Y, false))
			{
				longestDist = Number.Sign(Position.Y - pathPointWorld.Y, false);
				bestWay = new Point(0, -1);
			}
			if (pathPointWorld.Y > Position.Y && IsBarrier(new(0, 1)) == false &&
				longestDist < (int)Number.Sign(Position.Y - pathPointWorld.Y, false))
			{
				longestDist = Number.Sign(Position.Y - pathPointWorld.Y, false);
				bestWay = new Point(0, 1);
			}

			if (Position + bestWay == player.Position)
				return;
			Position += bestWay;

			Point RealPos(Point worldCell) => Screen.ScreenToRealPosition(World.WorldToScreenPosition(worldCell));
			Point WorldPos(Point realPos) => World.ScreenToWorldPosition(Screen.RealPositionToScreen(realPos));
			bool IsBarrier(Point offset) => ChunkManager.GetTile(Position + offset, 3) == World.TileBarrier;
		}
	}
}
