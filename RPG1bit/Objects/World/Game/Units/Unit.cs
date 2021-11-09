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

		public Unit(string uniqueID, CreationDetails creationDetails) : base(uniqueID, creationDetails) { AddTags(nameof(Unit)); }

		public void ApplyEffect(Effect effect)
		{
			EffectUIDs.Add(effect.UniqueID);
			effect.OwnerUID = UniqueID;
			Screen.ScheduleDisplay();
		}
		public bool Move(Point movement)
		{
			var futurePos = Position + movement;
			if (CanMoveIntoCell(futurePos) == false) return false;

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
				if (ChunkManager.GetTile(cell, i) != new Point())
					isVoid = false;
				
			return isVoid == false && ChunkManager.GetTile(cell, 3) != World.TileBarrier && CellIsInReach(cell);
		}
	}
}
