using SMPL.Components;
using SMPL.Data;
using SMPL.Gear;
using System.Collections.Generic;

namespace SMPL.Prefabs
{
	public class Rope : Access
	{
		internal static List<Rope> ropes = new();
		internal List<P> points = new();
		internal List<Stick> sticks = new();

		private Identity<Rope> identity;
		public Identity<Rope> Identity
		{
			get { return AllAccess == Extent.Removed ? default : identity; }
			set
			{
				if (identity == value || (Debug.CalledBySMPL == false && IsCurrentlyAccessible() == false)) return;
				var prev = identity;
				identity = value;
				//if (Debug.CalledBySMPL == false) OnIdentityChange?.Invoke(this, prev);
			}
		}

		private bool isDestroyed;
		public bool IsDestroyed
		{
			get { return isDestroyed; }
			set
			{
				if (isDestroyed == value || (Debug.CalledBySMPL == false && IsCurrentlyAccessible() == false)) return;
				isDestroyed = value;
				if (Identity != null) Identity.Dispose();
				Identity = null;
				ropes.Remove(this);
				//if (Debug.CalledBySMPL == false) OnDestroy?.Invoke(this);
			}
		}

		public Rope(string uniqueID)
		{
			if (Identity<Sprite>.CannotCreate(uniqueID)) return;
			GrantAccessToFile(Debug.CurrentFilePath(1)); // grant the user access

			ropes.Add(this);
			Identity = new(this, uniqueID);

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
		}

		internal void Update()
		{
			for (int i = 0; i < points.Count; i++)
			{
				if (points[i].locked) continue;
				var positionBeforeUpdate = points[i].position;
				points[i].position += points[i].position - points[i].prevPosition;
				points[i].position += new Point(0, 1) + new Point(0, 0.00001) * Performance.DeltaTime * Performance.DeltaTime;
				points[i].prevPosition = positionBeforeUpdate;
			}

			for (int i = 0; i < 50; i++)
				for (int j = 0; j < sticks.Count; j++)
				{
					var stickCentre = (sticks[j].pA.position + sticks[j].pB.position) / 2;
					var stickDir = Point.Normalize(sticks[j].pA.position - sticks[j].pB.position);
					if (sticks[j].pA.locked == false) sticks[j].pA.position = stickCentre + stickDir * sticks[j].length / 2;
					if (sticks[j].pB.locked == false) sticks[j].pB.position = stickCentre - stickDir * sticks[j].length / 2;
				}
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
