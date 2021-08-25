using SMPL.Gear;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SMPL.Components
{
	public class Identity<T> : Access
	{
		internal static readonly Dictionary<string, T> uniqueIDs = new();
		internal static readonly Dictionary<string, List<T>> tagObjs = new();
		internal static readonly Dictionary<T, List<string>> objTags = new();
		internal T instance;

		private static event Events.ParamsOne<Identity<T>> OnRemoveAllTags;
		private static event Events.ParamsTwo<Identity<T>, string> OnCreate, OnAddTag, OnRemoveTag;

		public static class CallWhen
		{
			public static void Create(Action<Identity<T>, string> method, uint order = uint.MaxValue) =>
				OnCreate = Events.Add(OnCreate, method, order);
			public static void TagAdd(Action<Identity<T>, string> method, uint order = uint.MaxValue) =>
				OnAddTag = Events.Add(OnAddTag, method, order);
			public static void TagRemove(Action<Identity<T>, string> method, uint order = uint.MaxValue) =>
				OnRemoveTag = Events.Add(OnRemoveTag, method, order);
			public static void RemoveAllTags(Action<Identity<T>> method, uint order = uint.MaxValue) =>
				OnRemoveAllTags = Events.Add(OnRemoveAllTags, method, order);
		}

		public string UniqueID { get; private set; }
		public string[] Tags => objTags[instance].ToArray();

		internal static bool CannotCreate(string uniqueID)
		{
			if (uniqueID == null)
			{
				Debug.LogError(2, $"Cannot create the identity of this '{typeof(T)}' instance\n" +
					$"because the uniqueID cannot be 'null'.");
				return true;
			}
			if (uniqueIDs.ContainsKey(uniqueID))
			{
				Debug.LogError(2, $"Cannot create the identity of this '{typeof(T)}' instance\n" +
					$"because the uniqueID '{uniqueID}' already exists.");
				return true;
			}
			return false;
		}
		public Identity(T instance, string uniqueID) : base()
		{
			this.instance = instance;
			UniqueID = uniqueID;
			uniqueIDs.Add(uniqueID, instance);
			objTags[instance] = new();
			OnCreate?.Invoke(this, UniqueID);
		}

		public void AddTags(params string[] tags)
		{
			if (Debug.CalledBySMPL == false && IsCurrentlyAccessible() == false) return;
			for (int j = 0; j < tags.Length; j++)
			{
				if (objTags[instance].Contains(tags[j])) continue;
				objTags[instance].Add(tags[j]);
				if (tagObjs.ContainsKey(tags[j]) == false) tagObjs[tags[j]] = new List<T>();
				tagObjs[tags[j]].Add(instance);
				if (Debug.CalledBySMPL == false) OnAddTag?.Invoke(this, tags[j]);
			}
		}
		public void RemoveTags(params string[] tags)
		{
			if (Debug.CalledBySMPL == false && IsCurrentlyAccessible() == false) return;
			for (int j = 0; j < tags.Length; j++)
			{
				if (objTags[instance].Contains(tags[j]) == false) continue;
				objTags[instance].Remove(tags[j]);
				tagObjs[tags[j]].Remove(instance);
				if (tagObjs[tags[j]].Count == 0) tagObjs.Remove(tags[j]);
				if (Debug.CalledBySMPL == false) OnRemoveTag?.Invoke(this, tags[j]);
			}
		}
		public void RemoveAllTags()
		{
			if (Debug.CalledBySMPL == false && IsCurrentlyAccessible() == false) return;
			if (Debug.CalledBySMPL == false) foreach (var kvp in tagObjs) OnRemoveTag?.Invoke(this, kvp.Key);
			tagObjs.Clear();
			objTags.Clear();
			if (Debug.CalledBySMPL == false) OnRemoveAllTags?.Invoke(this);
		}
		public bool HasTags(params string[] tags)
		{
			if (tags == null) { Debug.LogError(1, "The tag collection cannot be 'null'."); return false; }
			for (int i = 0; i < tags.Length; i++)
				if (objTags[instance].Contains(tags[i]) == false)
					return false;
			return true;
		}

		public static string[] AllUniqueIDs { get { return uniqueIDs.Keys.ToArray(); } }
		public static T[] AllInstances { get { return uniqueIDs.Values.ToArray(); } }
		public static string[] AllTags { get { return tagObjs.Keys.ToArray(); } }

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
				if (Identity<T>.uniqueIDs.ContainsKey(uniqueIDs[i]) == false)
					return false;
			return true;
		}
		public static T PickByUniqueID(string uniqueID) => uniqueIDs.ContainsKey(uniqueID) ? uniqueIDs[uniqueID] : default;
		public static T[] PickByTag(string tag) => tagObjs.ContainsKey(tag) ? tagObjs[tag].ToArray() : Array.Empty<T>();
	}
}
