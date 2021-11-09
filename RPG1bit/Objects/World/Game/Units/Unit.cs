using Newtonsoft.Json;
using SMPL.Data;
using SMPL.Gear;
using System.Collections.Generic;

namespace RPG1bit
{
	public class Unit : Object, IInteractable
	{
		[JsonProperty]
		public List<string> ItemUIDs { get; set; } = new();
		[JsonProperty]
		public List<string> EffectUIDs { get; set; } = new();
		[JsonProperty]
		public int[] Health { get; set; } = new int[2] { 30, 30 };
		public Point PreviousPosition { get; set; }

		public Unit(string uniqueID, CreationDetails creationDetails) : base(uniqueID, creationDetails)
		{
			PreviousPosition = Position;
			AddTags(nameof(Unit));
		}

		public void ApplyEffect(Effect effect)
		{
			EffectUIDs.Add(effect.UniqueID);
			effect.OwnerUID = UniqueID;
			Screen.ScheduleDisplay();
		}
		public bool Move(Point movement)
		{
			var futurePos = Position + movement;
			if (CanMoveIntoCell(futurePos) == false)
				return false;

			PreviousPosition = Position;
			Position = futurePos;
			if (UniqueID == nameof(Player) && MoveCamera.IsAnchored)
				World.CameraPosition += movement;
			return true;
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
			if (Health[0] <= 0)
				return false;

			var isVoid = true;
			for (int i = 0; i < 4; i++)
			{
				var tile = ChunkManager.GetTile(cell, i);
				if (tile == default && objects.ContainsKey(cell))
					for (int j = 0; j < objects[cell].Count; j++)
						if (objects[cell][j].Height == i)
							tile = objects[cell][j].TileIndexes;

				if (tile != new Point())
					isVoid = false;
			}
				
			return isVoid == false && ChunkManager.GetTile(cell, 3) != World.TileBarrier && CellIsInReach(cell);
		}
	}
}
