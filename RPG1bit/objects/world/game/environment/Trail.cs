﻿using SMPL.Data;
using SMPL.Gear;
using System.Collections.Generic;

namespace RPG1bit
{
	public class Trail : GameObject
	{
		private readonly List<Point> points = new();

		public string OwnerUID { get; set; }
		public int Length { get; set; }
		public int Duration { get; set; }

		public Trail(string uniqueID, CreationDetails creationDetails) : base(uniqueID, creationDetails) { }

		public override void OnGameUpdate()
		{
			base.OnGameUpdate();
			if (Gate.EnterOnceWhile($"{UniqueID}-updt", true))
				OnAdvanceTime();
		}
		public override void OnAdvanceTime()
		{
			Duration--;
			if (UniqueIDsExists(OwnerUID))
			{
				var owner = (GameObject)PickByUniqueID(OwnerUID);
				if (Duration > 0)
				{
					Position = owner.Position;
					points.Add(Position);
				}
			}

			if (points.Count > Length + 1 || Duration <= 0)
				points.RemoveAt(0);
			if (points.Count == 0)
				Destroy();
		}
		public override void OnDisplay(Point screenPos)
		{
			for (int i = 0; i < points.Count; i++)
			{
				if (World.TileHasRoof(points[i]) && World.IsShowingRoofs)
					continue;

				Screen.EditCell(World.WorldToScreenPosition(points[i]), TileIndexes, 3, TileIndexes.Color);
			}
		}
	}
}
