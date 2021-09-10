﻿using System;
using System.Collections.Generic;
using System.Linq;
using SMPL.Data;
using SMPL.Gear;

namespace SMPL.Components
{
	public class Hitbox : Thing
	{
		private bool Contains_(string hitboxUID)
		{
			var hitbox = (Hitbox)PickByUniqueID(hitboxUID);
			if (hitboxUID == null || hitbox.lines.Count < 3 || ignoresUIDs.Contains(hitboxUID)) return false;
			var firstLine = new Line();
			foreach (var kvp in hitbox.lines)
			{
				firstLine = kvp.Value;
				break;
			}
			var ray = new Line(firstLine.StartPosition, new Point(9_999, -99_999));
			var crossSum = 0;
			foreach (var kvp in lines) crossSum += ray.Crosses(kvp.Value) ? 1 : 0;
			return crossSum == 1 && Crosses(hitboxUID) == false;
		}
		private bool Crosses_(string hitboxUID) => CrossPoints(hitboxUID).Length > 0;

		//==========

		internal static List<Hitbox> hitboxes = new();
		internal Dictionary<string, Line> localLines = new();
		internal Dictionary<string, Line> lines = new();
		internal List<string> crossesUIDs = new();
		internal List<string> containsUIDs = new();
		internal List<string> ignoresUIDs = new();

		internal static void Update()
		{
			for (int i = 0; i < hitboxes.Count; i++)
			{
				hitboxes[i].crossesUIDs.Clear();
				hitboxes[i].containsUIDs.Clear();
				for (int j = 0; j < hitboxes.Count; j++)
				{
					if (hitboxes[i] == hitboxes[j]) continue;

					if (hitboxes[j].Crosses_(hitboxes[i].UniqueID)) hitboxes[j].crossesUIDs.Add(hitboxes[i].UniqueID);
					if (hitboxes[j].Contains_(hitboxes[i].UniqueID)) hitboxes[j].containsUIDs.Add(hitboxes[i].UniqueID);
				}
			}
		}

		//==============

		public Line[] Lines => ErrorIfDestroyed() ? default : lines.Values.ToArray();
		public string[] LineUniqueIDs => ErrorIfDestroyed() ? default : lines.Keys.ToArray();
		public Point MiddlePoint
		{
			get
			{
				if (ErrorIfDestroyed()) return Point.Invalid;
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

		public Hitbox(string uniqueID) : base(uniqueID)
		{
			hitboxes.Add(this);
			if (cannotCreate) { ErrorAlreadyHasUID(uniqueID); Destroy(); }
		}
		public override void Destroy()
		{
			hitboxes.Remove(this);
			localLines = null;
			lines = null;
			crossesUIDs = null;
			containsUIDs = null;
			ignoresUIDs = null;
			base.Destroy();
		}

		public void Display(Camera camera, double width)
		{
			if (ErrorIfDestroyed()) return;
			foreach (var kvp in lines) kvp.Value.Display(camera, width);
		}

		public void AddIgnorance(params string[] hitboxInstances)
		{
			if (ErrorIfDestroyed()) return;
			if (hitboxInstances == null) { Debug.LogError(1, "The ignored hitboxes cannot be 'null'."); return; }
			for (int i = 0; i < hitboxInstances.Length; i++)
				if (ignoresUIDs.Contains(hitboxInstances[i]) == false)
					ignoresUIDs.Add(hitboxInstances[i]);
		}
		public void RemoveIgnorance(params string[] hitboxInstances)
		{
			if (ErrorIfDestroyed()) return;
			if (hitboxInstances == null) { Debug.LogError(1, "The ignored hitboxes cannot be 'null'."); return; }
			for (int i = 0; i < hitboxInstances.Length; i++)
				if (ignoresUIDs.Contains(hitboxInstances[i]))
					ignoresUIDs.Remove(hitboxInstances[i]);
		}
		public void RemoveAllIgnorance()
		{
			if (ErrorIfDestroyed() == false) ignoresUIDs.Clear();
		}
		public bool Ignores(params string[] hitboxInstances)
		{
			if (ErrorIfDestroyed()) return false;
			if (hitboxInstances == null)
			{
				Debug.LogError(1, "The ignored hitboxes cannot be 'null'.");
				return false;
			}
			for (int i = 0; i < hitboxInstances.Length; i++)
				if (ignoresUIDs.Contains(hitboxInstances[i]) == false)
					return false;
			return true;
		}

		public void SetLine(string uniqueID, Line line)
		{
			if (ErrorIfDestroyed()) return;
			if (uniqueID == null)
			{
				Debug.LogError(1, $"The unique ID of a line cannot be 'null'.");
				return;
			}
			lines[uniqueID] = line;
			if (localLines.ContainsKey(uniqueID)) return;
			localLines[uniqueID] = line;
		}
		public void RemoveLines(params string[] uniqueIDs)
		{
			if (ErrorIfDestroyed()) return;
			if (uniqueIDs == null) { Debug.LogError(1, "The uniqueIDs cannot be 'null'."); return; }
			for (int i = 0; i < uniqueIDs.Length; i++)
			{
				if (lines.ContainsKey(uniqueIDs[i]) == false) continue;
				lines.Remove(uniqueIDs[i]);
				localLines.Remove(uniqueIDs[i]);
			}
		}
		public void RemoveAllLines()
		{
			if (ErrorIfDestroyed()) return;
			lines.Clear();
			localLines.Clear();
		}
		public bool HasLineUniqueID(string uniqueID)
		{
			return ErrorIfDestroyed() == false && lines.ContainsKey(uniqueID);
		}
		public Line GetLine(string uniqueID)
		{
			if (ErrorIfDestroyed()) return Line.Invalid;
			if (HasLineUniqueID(uniqueID) == false)
			{
				Debug.LogError(1, $"A line with unique ID '{uniqueID}' was not found.");
				return Line.Invalid;
			}
			return lines[uniqueID];
		}

		public bool Overlaps(params string[] hitboxUniqueIDs)
		{
			if (ErrorIfDestroyed()) return false;
			if (hitboxUniqueIDs == null)
			{
				Debug.LogError(1, "The hitbox instances cannot be 'null'.");
				return false;
			}
			for (int i = 0; i < hitboxUniqueIDs.Length; i++)
			{
				var hitbox = (Hitbox)PickByUniqueID(hitboxUniqueIDs[i]);
				if (hitbox.Contains(UniqueID) == false) return false;
			}
			return Contains(hitboxUniqueIDs) || Crosses(hitboxUniqueIDs);
		}
		public bool Contains(params string[] hitboxUniqueIDs)
		{
			if (ErrorIfDestroyed()) return false;
			if (hitboxUniqueIDs == null)
			{
				Debug.LogError(1, "The collection of ComponentHitbox instances cannot be 'null'.");
				return false;
			}
			for (int i = 0; i < hitboxUniqueIDs.Length; i++)
				if (containsUIDs.Contains(hitboxUniqueIDs[i]) == false)
					return false;
			return true;
		}
		public bool Crosses(params string[] hitboxUniqueIDs)
		{
			if (ErrorIfDestroyed()) return false;
			if (hitboxUniqueIDs == null)
			{
				Debug.LogError(1, "The collection of ComponentHitbox instances cannot be 'null'.");
				return false;
			}
			for (int i = 0; i < hitboxUniqueIDs.Length; i++)
				if (crossesUIDs.Contains(hitboxUniqueIDs[i]) == false)
					return false;
			return true;
		}
		public Point[] CrossPoints(params string[] hitboxUniqueIDs)
		{
			if (ErrorIfDestroyed()) return Array.Empty<Point>();
			if (hitboxUniqueIDs == null)
			{
				Debug.LogError(1, "The collection of ComponentHitbox instances cannot be 'null'.");
				return System.Array.Empty<Point>();
			}
			if (Ignores(hitboxUniqueIDs)) return System.Array.Empty<Point>();
			var result = new List<Point>();
			foreach (var kvp in lines)
			{
				for (int i = 0; i < hitboxUniqueIDs.Length; i++)
				{
					var hitbox = (Hitbox)PickByUniqueID(hitboxUniqueIDs[i]);
					foreach (var kvp2 in hitbox.lines)
					{
						var p = kvp.Value.CrossPoint(kvp2.Value);
						if (p.IsInvalid || result.Contains(p)) continue;
						result.Add(p);
					}
				}
			}
			return result.ToArray();
		}
	}
}
