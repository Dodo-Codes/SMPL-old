using Newtonsoft.Json;
using SMPL.Components;
using SMPL.Data;
using SMPL.Gear;
using System.Collections.Generic;

namespace SMPL.Prefabs
{
	public class Ropes : Thing
	{
		internal static List<Ropes> ropes = new();
		[JsonProperty]
		internal Dictionary<string, RopePoint> points = new();
		[JsonProperty]
		internal Dictionary<string, Stick> sticks = new();

		internal static void Update()
		{
			for (int r = 0; r < ropes.Count; r++)
			{
				var isOnCamera = false;
				foreach (var kvp in Camera.sortedCameras)
					for (int i = 0; i < kvp.Value.Count; i++)
					{
						if (kvp.Value[i].Captures(ropes[r]))
						{
							isOnCamera = true;
							break;
						}
					}
				if (isOnCamera == false) continue;

				foreach (var kvp in ropes[r].points)
				{
					if (kvp.Value.IsLocked) continue;
					var prev = kvp.Value.Position;
					var force = new Point(ropes[r].Force.W, ropes[r].Force.H);
					kvp.Value.Position += kvp.Value.Position - kvp.Value.prevPosition;
					kvp.Value.Position += force;
					kvp.Value.prevPosition = prev;
				}

				for (int i = 0; i < 5; i++)
					foreach (var kvp2 in ropes[r].sticks)
					{
						var stickCentre = (kvp2.Value.pA.Position + kvp2.Value.pB.Position) / 2;
						var stickDir = Point.Normalize(kvp2.Value.pA.Position - kvp2.Value.pB.Position);
						if (kvp2.Value.pA.IsLocked == false) kvp2.Value.pA.Position = stickCentre + stickDir * kvp2.Value.length / 2;
						if (kvp2.Value.pB.IsLocked == false) kvp2.Value.pB.Position = stickCentre - stickDir * kvp2.Value.length / 2;
					}
			}
		}

		//========================

		public Size Force { get; set; } = new Size(0, 1);
		public string TexturePath { get; set; }

		public Ropes(string uniqueID) : base(uniqueID)
		{
			ropes.Add(this);

			if (cannotCreate) { ErrorAlreadyHasUID(uniqueID); Destroy(); }
		}
		public override void Destroy()
		{
			if (ErrorIfDestroyed()) return;
			ropes.Remove(this);
			points = null;
			sticks = null;
			base.Destroy();
		}

		public void Display(Camera camera, double width)
		{
			if (camera.Captures(this) == false) return;
			foreach (var kvp in sticks)
				new Line(kvp.Value.pA.Position, kvp.Value.pB.Position).Display(camera, width);
		}

		public void SetPoint(string ID, RopePoint ropePoint)
		{
			points[ID] = ropePoint;
		}
		public RopePoint GetPoint(string ID)
		{
			if (points.ContainsKey(ID) == false)
			{
				Debug.LogError(1, $"No {nameof(RopePoint)} was found with ID '{ID}'");
				return default;
			}
			return points[ID];
		}
		public void SetPointConnection(string ID, string firstPointID, string secondPointID)
		{
			if (points.ContainsKey(firstPointID) == false || points.ContainsKey(secondPointID) == false)
			{
				Debug.LogError(1, $"No connection was made because at least one of the points was not found by its ID.");
				return;
			}
			sticks[ID] = new Stick(points[firstPointID], points[secondPointID]);
		}
		public void RemovePointConnection(string ID)
		{
			if (sticks.ContainsKey(ID) == false)
			{
				Debug.LogError(1, $"No connection with ID '{ID}' was found.");
				return;
			}
			sticks.Remove(ID);
		}
	}
	public class RopePoint
	{
		internal Point prevPosition;

		//==============

		public Point Position { get; set; }
		public bool IsLocked { get; set; }

		public RopePoint(Point position, bool isLocked)
		{
			Position = position;
			IsLocked = isLocked;
		}
	}
	internal class Stick
	{
		public RopePoint pA, pB;
		public double length;

		public Stick(RopePoint pA, RopePoint pB)
		{
			this.pA = pA;
			this.pB = pB;
			length = Point.Distance(pA.Position, pB.Position);
		}
	}
}
