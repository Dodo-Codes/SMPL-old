using Newtonsoft.Json;
using SMPL.Gear;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SMPL.Gear
{
	[JsonObject(MemberSerialization.OptIn)]
	public class Thing
	{
		private string uniqueID;

		//===========

		internal static readonly Dictionary<string, Thing> uniqueIDs = new();
		internal static readonly Dictionary<string, List<Thing>> tagObjs = new();
		internal static readonly Dictionary<Thing, List<string>> objTags = new();
		internal bool cannotCreate;

		internal bool ErrorIfDestroyed()
		{
			if (IsDestroyed)
			{
				Debug.LogError(2, $"This {nameof(Thing)} is destroyed.");
				return true;
			}
			return false;
		}
		internal static void ErrorAlreadyHasUID(string uniqueID)
		{
			Debug.LogError(2, $"A {nameof(Thing)} with uniqueID '{uniqueID}' already exists.\n" +
				$"The newly created {nameof(Thing)} was destroyed because of this.");
		}

		// ======

		public static string[] AllUniqueIDs => uniqueIDs.Keys.ToArray();
		public static Thing[] AllInstances => uniqueIDs.Values.ToArray();
		public static string[] AllTags => tagObjs.Keys.ToArray();

		public static bool TagsExist(params string[] tags)
		{
			for (int i = 0; i < tags.Length; i++)
				if (tagObjs.ContainsKey(tags[i]) == false)
					return false;
			return true;
		}
		public static bool UniqueIDsExits(params string[] uniqueIDs)
		{
			for (int i = 0; i < uniqueIDs.Length; i++)
				if (Thing.uniqueIDs.ContainsKey(uniqueIDs[i]) == false)
					return false;
			return true;
		}
		public static Thing PickByUniqueID(string uniqueID)
		{
			return uniqueID != null && uniqueIDs.ContainsKey(uniqueID) ? uniqueIDs[uniqueID] : default;
		}
		public static Thing[] PickByTag(string tag) => tagObjs.ContainsKey(tag) ? tagObjs[tag].ToArray() : Array.Empty<Thing>();

		// ======

		[JsonProperty]
		public bool IsDestroyed { get; private set; }
		[JsonProperty]
		public string UniqueID
		{
			get { return ErrorIfDestroyed() ? default : uniqueID; }
			set
			{
				var old = uniqueID;
				uniqueID = value;
				if (old != null) uniqueIDs.Remove(old);
				uniqueIDs[value] = this;
			}
		}
		[JsonProperty]
		public string[] Tags => ErrorIfDestroyed() ? Array.Empty<string>() : objTags[this].ToArray();

		public Thing(string uniqueID)
		{
			if (uniqueIDs.ContainsKey(uniqueID)) { cannotCreate = true; return; }

			UniqueID = uniqueID;
			objTags[this] = new();
		}
		public Thing Clone(string uniqueID)
		{
			if (uniqueIDs.ContainsKey(uniqueID))
			{
				Debug.LogError(1, $"Cannot clone because a {nameof(Thing)} with uniqueID '{uniqueID}' already exists.");
				return default;
			}

			var clone = (Thing)MemberwiseClone();
			clone.UniqueID = uniqueID;
			objTags[clone] = new(objTags[this]);
			return clone;
		}
		public virtual void Destroy()
		{
			if (ErrorIfDestroyed()) return;
			IsDestroyed = true;
			if (cannotCreate) return;
			uniqueIDs.Remove(UniqueID);
			if (objTags.ContainsKey(this))
			{
				RemoveAllTags();
				objTags.Remove(this);
			}
		}

		public void AddTags(params string[] tags)
		{
			if (ErrorIfDestroyed()) return;
			for (int j = 0; j < tags.Length; j++)
			{
				if (objTags[this].Contains(tags[j])) continue;
				objTags[this].Add(tags[j]);
				if (tagObjs.ContainsKey(tags[j]) == false) tagObjs[tags[j]] = new List<Thing>();
				tagObjs[tags[j]].Add(this);
			}
		}
		public void RemoveTags(params string[] tags)
		{
			if (ErrorIfDestroyed()) return;
			for (int j = 0; j < tags.Length; j++)
			{
				if (objTags[this].Contains(tags[j]) == false) continue;
				objTags[this].Remove(tags[j]);
				tagObjs[tags[j]].Remove(this);
				if (tagObjs[tags[j]].Count == 0) tagObjs.Remove(tags[j]);
			}
		}
		public void RemoveAllTags()
		{
			if (ErrorIfDestroyed()) return;
			tagObjs.Clear();
			objTags.Clear();
		}
		public bool HasTags(params string[] tags)
		{
			if (tags == null) { Debug.LogError(1, "The tag collection cannot be 'null'."); return false; }
			for (int i = 0; i < tags.Length; i++)
				if (objTags[this].Contains(tags[i]) == false)
					return false;
			return true;
		}

		public virtual void OnMouseButtonDoubleClick(Mouse.Button button) { }
		public virtual void OnMouseButtonPress(Mouse.Button button) { }
		public virtual void OnMouseButtonRelease(Mouse.Button button) { }
		public virtual void OnMouseButtonHold(Mouse.Button button) { }
		public virtual void OnMouseWheelScroll(Mouse.Wheel wheel, double delta) { }
		public virtual void OnMouseCursorWindowEnter() { }
		public virtual void OnMouseCursorWindowLeave() { }
	}
}
