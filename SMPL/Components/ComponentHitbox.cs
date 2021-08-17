using System.Collections.Generic;
using System.Linq;

namespace SMPL
{
	public class ComponentHitbox : ComponentAccess
	{
		private readonly uint creationFrame;
		private readonly double rand;

		internal static List<ComponentHitbox> hitboxes = new();

		internal Dictionary<string, Line> localLines = new();
		internal Dictionary<string, Line> lines = new();
		internal List<ComponentHitbox> crosses = new();
		internal List<ComponentHitbox> contains = new();
		internal List<ComponentHitbox> ignores = new();

		public int LineCount { get { return lines.Count; } }
		public Line[] Lines { get { return lines.Values.ToArray(); } }
		public string[] UniqueIDs { get { return lines.Keys.ToArray(); } }
		public Point MiddlePoint
		{
			get
			{
				var mostLeftPoint = new Point(float.PositiveInfinity, float.PositiveInfinity);
				var mostRightPoint = new Point(float.NegativeInfinity, float.NegativeInfinity);

				foreach (var kvp in lines)
				{
					var line = kvp.Value;

					if (line.StartPosition.X < mostLeftPoint.X) mostLeftPoint = line.StartPosition;
					if (line.EndPosition.X < mostLeftPoint.X) mostLeftPoint = line.EndPosition;
					if (line.StartPosition.X > mostRightPoint.X) mostRightPoint = line.StartPosition;
					if (line.EndPosition.X > mostRightPoint.X) mostRightPoint = line.EndPosition;

					if (line.StartPosition.Y < mostLeftPoint.Y) mostLeftPoint.Y = (line.StartPosition.Y);
					if (line.EndPosition.Y < mostLeftPoint.Y) mostLeftPoint.Y = (line.EndPosition.Y);
					if (line.StartPosition.Y > mostRightPoint.Y) mostRightPoint.Y = (line.StartPosition.Y);
					if (line.EndPosition.Y > mostRightPoint.Y) mostRightPoint.Y = (line.EndPosition.Y);
				}
				return Point.PercentTowardTarget(mostLeftPoint, mostRightPoint, new Size(50, 50));
			}
		}

		public ComponentHitbox() : base()
		{
			creationFrame = Performance.FrameCount;
			rand = Number.Random(new Bounds(-9999, 9999), 5);
			hitboxes.Add(this);
		}
		internal void Update()
		{
			crosses.Clear();
			contains.Clear();
			for (int i = 0; i < hitboxes.Count; i++)
			{
				if (this == hitboxes[i]) continue;

				if (Crosses_(hitboxes[i])) crosses.Add(hitboxes[i]);
				if (Contains_(hitboxes[i])) contains.Add(hitboxes[i]);
			}
		}
		public void Display(Camera camera)
		{
			if (Debug.CurrentMethodIsCalledByUser && IsCurrentlyAccessible() == false) return;
			foreach (var kvp in lines) kvp.Value.Display(camera);
		}

		public Line GetLine(string uniqueID)
		{
			if (HasUniqueID(uniqueID) == false)
			{
				Debug.LogError(1, $"A line with unique ID '{uniqueID}' was not found.");
				var p = new Point(double.NaN, double.NaN);
				return new Line(p, p);
			}
			return lines[uniqueID];
		}
		public bool Ignores(ComponentHitbox componentHitbox) => ignores.Contains(componentHitbox);
		public void AddIgnorance(ComponentHitbox componentHitbox)
		{
			if (Debug.CurrentMethodIsCalledByUser && IsCurrentlyAccessible() == false) return;
			if (componentHitbox == null)
			{
				Debug.LogError(1, "The ignored hitbox cannot be 'null'.");
				return;
			}
			if (ignores.Contains(componentHitbox))
			{
				Debug.LogError(1, "This hitbox is already being ignored.");
				return;
			}
			ignores.Add(componentHitbox);
		}
		public void RemoveIgnorance(ComponentHitbox componentHitbox)
		{
			if (Debug.CurrentMethodIsCalledByUser && IsCurrentlyAccessible() == false) return;
			if (ignores.Contains(componentHitbox) == false)
			{
				Debug.LogError(1, "The hitbox was not found.");
				return;
			}
			ignores.Remove(componentHitbox);
		}
		public bool Overlaps(ComponentHitbox componentHitbox)
		{
			return Contains(componentHitbox) || Crosses(componentHitbox) || componentHitbox.Contains(this);
		}

		public bool Contains(ComponentHitbox componentHitbox) => contains.Contains(componentHitbox);
		public bool Crosses(ComponentHitbox componentHitbox) => crosses.Contains(componentHitbox);
		private bool Contains_(ComponentHitbox componentHitbox)
		{
			if (componentHitbox == null || componentHitbox.lines.Count < 3 || ignores.Contains(componentHitbox)) return false;
			var firstLine = new Line();
			foreach (var kvp in componentHitbox.lines)
			{
				firstLine = kvp.Value;
				break;
			}
			var ray = new Line(firstLine.StartPosition, new Point(9_999, -99_999));
			var crossSum = 0;
			foreach (var kvp in lines) crossSum += ray.Crosses(kvp.Value) ? 1 : 0;
			return crossSum == 1 && Crosses(componentHitbox) == false;
		}
		private bool Crosses_(ComponentHitbox componentHitbox) => CrossPoints(componentHitbox).Length > 0;
		public Point[] CrossPoints(ComponentHitbox componentHitbox)
		{
			if (componentHitbox == null || ignores.Contains(componentHitbox)) return System.Array.Empty<Point>();
			var result = new List<Point>();
			foreach (var kvp in lines)
			{
				foreach (var kvp2 in componentHitbox.lines)
				{
					var p = kvp.Value.CrossPoint(kvp2.Value);
					if (p.IsInvalid || result.Contains(p)) continue;
					result.Add(p);
				}
			}
			return result.ToArray();
		}
		public bool HasUniqueID(string uniqueID) => lines.ContainsKey(uniqueID);
		public void SetLine(string uniqueID, Line line)
		{
			if (Debug.CurrentMethodIsCalledByUser && IsCurrentlyAccessible() == false) return;
			if (uniqueID == null)
			{
				Debug.LogError(1, $"The unique ID of a line cannot be 'null'.");
				return;
			}
			lines[uniqueID] = line;
			if (localLines.ContainsKey(uniqueID)) return;
			localLines[uniqueID] = line;
		}
		public void RemoveLine(string uniqueID)
		{
			if (Debug.CurrentMethodIsCalledByUser && IsCurrentlyAccessible() == false) return;
			if (lines.ContainsKey(uniqueID) == false)
			{
				Debug.LogError(1, $"A line with unique ID '{uniqueID}' was not found.");
				return;
			}
			lines.Remove(uniqueID);
			localLines.Remove(uniqueID);
		}
		public void RemoveAllLines()
		{
			if (Debug.CurrentMethodIsCalledByUser && IsCurrentlyAccessible() == false) return;
			lines.Clear();
			localLines.Clear();
		}
	}
}
