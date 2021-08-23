using System;
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

		private static event Events.ParamsTwo<ComponentHitbox, ComponentIdentity<ComponentHitbox>> OnIdentityChange;
		private static event Events.ParamsOne<ComponentHitbox> OnCreate, OnRemoveAllIgnorance, OnRemoveAllLines;
		private static event Events.ParamsTwo<ComponentHitbox, ComponentHitbox> OnAddIgnorance, OnRemoveIgnorance;
		private static event Events.ParamsTwo<ComponentHitbox, string> OnSetLine, OnRemoveLine;

		public static class CallWhen
		{
			public static void IdentityChange(Action<ComponentHitbox, ComponentIdentity<ComponentHitbox>> method,
				uint order = uint.MaxValue) => OnIdentityChange = Events.Add(OnIdentityChange, method, order);
			public static void Create(Action<ComponentHitbox> method, uint order = uint.MaxValue) =>
				OnCreate = Events.Add(OnCreate, method, order);
			public static void SetLine(Action<ComponentHitbox, string> method, uint order = uint.MaxValue) =>
				OnSetLine = Events.Add(OnSetLine, method, order);
			public static void RemoveLine(Action<ComponentHitbox, string> method, uint order = uint.MaxValue) =>
				OnRemoveLine = Events.Add(OnRemoveLine, method, order);
			public static void AddIgnorance(Action<ComponentHitbox, ComponentHitbox> method, uint order = uint.MaxValue) =>
				OnAddIgnorance = Events.Add(OnAddIgnorance, method, order);
			public static void RemoveIgnorance(Action<ComponentHitbox, ComponentHitbox> method, uint order = uint.MaxValue) =>
				OnRemoveIgnorance = Events.Add(OnRemoveIgnorance, method, order);
			public static void RemoveAllIgnorance(Action<ComponentHitbox> method, uint order = uint.MaxValue) =>
				OnRemoveAllIgnorance = Events.Add(OnRemoveAllIgnorance, method, order);
			public static void RemoveAllLines(Action<ComponentHitbox> method, uint order = uint.MaxValue) =>
				OnRemoveAllLines = Events.Add(OnRemoveAllLines, method, order);
		}

		private ComponentIdentity<ComponentHitbox> identity;
		public ComponentIdentity<ComponentHitbox> Identity
		{
			get { return identity; }
			set
			{
				if (identity == value || (Debug.CalledBySMPL == false && IsCurrentlyAccessible() == false)) return;
				var prev = identity;
				identity = value;
				if (Debug.CalledBySMPL == false) OnIdentityChange?.Invoke(this, prev);
			}
		}

		public int LineCount => lines.Count;
		public Line[] Lines => lines.Values.ToArray();
		public string[] LineUniqueIDs => lines.Keys.ToArray();
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
			OnCreate?.Invoke(this);
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
		public void Display(ComponentCamera camera)
		{
			if (Debug.CalledBySMPL == false && IsCurrentlyAccessible() == false) return;
			foreach (var kvp in lines) kvp.Value.Display(camera);
		}

		public void AddIgnorance(params ComponentHitbox[] hitboxInstances)
		{
			if (Debug.CalledBySMPL == false && IsCurrentlyAccessible() == false) return;
			if (hitboxInstances == null) { Debug.LogError(1, "The ignored hitboxes cannot be 'null'."); return; }
			for (int i = 0; i < hitboxInstances.Length; i++)
			{
				if (ignores.Contains(hitboxInstances[i])) continue;
				ignores.Add(hitboxInstances[i]);
				if (Debug.CalledBySMPL == false) OnAddIgnorance?.Invoke(this, hitboxInstances[i]);
			}
		}
		public void RemoveIgnorance(params ComponentHitbox[] hitboxInstances)
		{
			if (Debug.CalledBySMPL == false && IsCurrentlyAccessible() == false) return;
			if (hitboxInstances == null) { Debug.LogError(1, "The ignored hitboxes cannot be 'null'."); return; }
			for (int i = 0; i < hitboxInstances.Length; i++)
			{
				if (ignores.Contains(hitboxInstances[i]) == false) continue;
				ignores.Remove(hitboxInstances[i]);
				if (Debug.CalledBySMPL == false) OnRemoveIgnorance?.Invoke(this, hitboxInstances[i]);
			}
		}
		public void RemoveAllIgnorance()
		{
			if (Debug.CalledBySMPL == false && IsCurrentlyAccessible() == false) return;
			if (Debug.CalledBySMPL == false) for (int i = 0; i < ignores.Count; i++) OnRemoveIgnorance?.Invoke(this, ignores[i]);
			ignores.Clear();
			if (Debug.CalledBySMPL == false) OnRemoveAllIgnorance?.Invoke(this);
		}
		public bool Ignores(params ComponentHitbox[] hitboxInstances)
		{
			if (hitboxInstances == null)
			{
				Debug.LogError(1, "The ignored hitboxes cannot be 'null'.");
				return false;
			}
			for (int i = 0; i < hitboxInstances.Length; i++)
				if (ignores.Contains(hitboxInstances[i]) == false)
					return false;
			return true;
		}
		public bool Overlaps(params ComponentHitbox[] hitboxInstances)
		{
			if (hitboxInstances == null)
			{
				Debug.LogError(1, "The hitbox instances cannot be 'null'.");
				return false;
			}
			for (int i = 0; i < hitboxInstances.Length; i++)
				if (hitboxInstances[i].Contains(this) == false)
					return false;
			return Contains(hitboxInstances) || Crosses(hitboxInstances);
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

		public void SetLine(string uniqueID, Line line)
		{
			if (Debug.CalledBySMPL == false && IsCurrentlyAccessible() == false) return;
			if (uniqueID == null)
			{
				Debug.LogError(1, $"The unique ID of a line cannot be 'null'.");
				return;
			}
			lines[uniqueID] = line;
			if (localLines.ContainsKey(uniqueID)) return;
			localLines[uniqueID] = line;
			if (Debug.CalledBySMPL == false) OnSetLine?.Invoke(this, uniqueID);
		}
		public void RemoveLines(params string[] uniqueIDs)
		{
			if (Debug.CalledBySMPL == false && IsCurrentlyAccessible() == false) return;
			if (uniqueIDs == null) { Debug.LogError(1, "The uniqueIDs cannot be 'null'."); return; }
			for (int i = 0; i < uniqueIDs.Length; i++)
			{
				if (lines.ContainsKey(uniqueIDs[i]) == false) continue;
				lines.Remove(uniqueIDs[i]);
				localLines.Remove(uniqueIDs[i]);
				if (Debug.CalledBySMPL == false) OnRemoveLine?.Invoke(this, uniqueIDs[i]);
			}
		}
		public void RemoveAllLines()
		{
			if (Debug.CalledBySMPL == false && IsCurrentlyAccessible() == false) return;
			if (Debug.CalledBySMPL == false) foreach (var kvp in lines) OnRemoveLine?.Invoke(this, kvp.Key);
			lines.Clear();
			localLines.Clear();
			if (Debug.CalledBySMPL == false) OnRemoveAllLines?.Invoke(this);
		}
		public bool Contains(params ComponentHitbox[] hitboxInstances)
		{
			if (hitboxInstances == null)
			{
				Debug.LogError(1, "The collection of ComponentHitbox instances cannot be 'null'.");
				return false;
			}
			for (int i = 0; i < hitboxInstances.Length; i++)
				if (contains.Contains(hitboxInstances[i]) == false)
					return false;
			return true;
		}
		public bool Crosses(params ComponentHitbox[] hitboxInstances)
		{
			if (hitboxInstances == null)
			{
				Debug.LogError(1, "The collection of ComponentHitbox instances cannot be 'null'.");
				return false;
			}
			for (int i = 0; i < hitboxInstances.Length; i++)
				if (crosses.Contains(hitboxInstances[i]) == false)
					return false;
			return true;
		}
		public Point[] CrossPoints(params ComponentHitbox[] hitboxInstances)
		{
			if (hitboxInstances == null)
			{
				Debug.LogError(1, "The collection of ComponentHitbox instances cannot be 'null'.");
				return System.Array.Empty<Point>();
			}
			if (Ignores(hitboxInstances)) return System.Array.Empty<Point>();
			var result = new List<Point>();
			foreach (var kvp in lines)
			{
				for (int i = 0; i < hitboxInstances.Length; i++)
				{
					foreach (var kvp2 in hitboxInstances[i].lines)
					{
						var p = kvp.Value.CrossPoint(kvp2.Value);
						if (p.IsInvalid || result.Contains(p)) continue;
						result.Add(p);
					}
				}
			}
			return result.ToArray();
		}
		public bool HasUniqueID(string uniqueID) => lines.ContainsKey(uniqueID);

		private bool Contains_(ComponentHitbox hitboxInstance)
		{
			if (hitboxInstance == null || hitboxInstance.lines.Count < 3 || ignores.Contains(hitboxInstance)) return false;
			var firstLine = new Line();
			foreach (var kvp in hitboxInstance.lines)
			{
				firstLine = kvp.Value;
				break;
			}
			var ray = new Line(firstLine.StartPosition, new Point(9_999, -99_999));
			var crossSum = 0;
			foreach (var kvp in lines) crossSum += ray.Crosses(kvp.Value) ? 1 : 0;
			return crossSum == 1 && Crosses(hitboxInstance) == false;
		}
		private bool Crosses_(ComponentHitbox hitboxInstance) => CrossPoints(hitboxInstance).Length > 0;
	}
}
