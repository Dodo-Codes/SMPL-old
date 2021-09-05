using SMPL.Components;
using SMPL.Data;
using SMPL.Gear;
using System.Collections.Generic;

namespace SMPL.Prefabs
{
	public class Rope : Component
	{
		internal static List<Rope> ropes = new();
		internal List<P> points = new();
		internal List<Stick> sticks = new();

		internal static void Update()
		{
			for (int r = 0; r < ropes.Count; r++)
			{
				for (int i = 0; i < ropes[r].points.Count; i++)
				{
					if (ropes[r].points[i].locked) continue;
					var positionBeforeUpdate = ropes[r].points[i].position;
					ropes[r].points[i].position += ropes[r].points[i].position - ropes[r].points[i].prevPosition;
					ropes[r].points[i].position += new Point(0, 1) + new Point(0, 0.00001) * Performance.DeltaTime * Performance.DeltaTime;
					ropes[r].points[i].prevPosition = positionBeforeUpdate;
				}

				for (int i = 0; i < 50; i++)
					for (int j = 0; j < ropes[r].sticks.Count; j++)
					{
						var stickCentre = (ropes[r].sticks[j].pA.position + ropes[r].sticks[j].pB.position) / 2;
						var stickDir = Point.Normalize(ropes[r].sticks[j].pA.position - ropes[r].sticks[j].pB.position);
						if (ropes[r].sticks[j].pA.locked == false)
							ropes[r].sticks[j].pA.position = stickCentre + stickDir * ropes[r].sticks[j].length / 2;
						if (ropes[r].sticks[j].pB.locked == false)
							ropes[r].sticks[j].pB.position = stickCentre - stickDir * ropes[r].sticks[j].length / 2;
					}
			}
		}

		//========================

		public Rope(string uniqueID) : base(uniqueID)
		{
			ropes.Add(this);

			points.Add(new P() { locked = true, position = new Point(0, -100) });
			points.Add(new P() { locked = false, position = new Point(50, -100) });
			points.Add(new P() { locked = false, position = new Point(50, -50) });
			points.Add(new P() { locked = false, position = new Point(0, -50) });
			sticks.Add(new Stick(points[0], points[1]));
			sticks.Add(new Stick(points[1], points[2]));
			sticks.Add(new Stick(points[2], points[3]));

			points.Add(new P() { locked = true, position = new Point(50, -100) });
			points.Add(new P() { locked = false, position = new Point(100, -100) });
			points.Add(new P() { locked = false, position = new Point(100, -50) });
			points.Add(new P() { locked = false, position = new Point(50, -50) });
			sticks.Add(new Stick(points[4], points[5]));
			sticks.Add(new Stick(points[5], points[6]));
			sticks.Add(new Stick(points[6], points[7]));

			points.Add(new P() { locked = true, position = new Point(0, -75) });
			sticks.Add(new Stick(points[0], points[7]));

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

		public void Display(Camera camera)
		{
			for (int i = 0; i < sticks.Count; i++)
				new Line(sticks[i].pA.position, sticks[i].pB.position).Display(camera);
		}
	}
	internal class P
	{
		public Point position, prevPosition;
		public bool locked;
	}
	internal class Stick
	{
		public P pA, pB;
		public double length;

		public Stick(P pA, P pB)
		{
			this.pA = pA;
			this.pB = pB;
			length = Point.Distance(pA.position, pB.position);
		}
	}
}
