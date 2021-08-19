using System;
using System.Collections.Generic;
using System.Linq;

namespace SMPL
{
	public class ComponentIdentity<T> : ComponentAccess
	{
		private static readonly Dictionary<string, T> uniqueIDs = new();
		private static readonly Dictionary<string, List<T>> tagObjs = new();
		private static readonly Dictionary<T, List<string>> objTags = new();
		private T Instance { get; set; }

		private static event Events.ParamsOne<ComponentIdentity<T>> OnRemoveAllTags;
		private static event Events.ParamsTwo<ComponentIdentity<T>, string> OnCreate, OnAddTag, OnRemoveTag;

		public static class CallWhen
		{
			public static void Create(Action<ComponentIdentity<T>, string> method, uint order = uint.MaxValue) =>
				OnCreate = Events.Add(OnCreate, method, order);
			public static void TagAdd(Action<ComponentIdentity<T>, string> method, uint order = uint.MaxValue) =>
				OnAddTag = Events.Add(OnAddTag, method, order);
			public static void TagRemove(Action<ComponentIdentity<T>, string> method, uint order = uint.MaxValue) =>
				OnRemoveTag = Events.Add(OnRemoveTag, method, order);
			public static void RemoveAllTags(Action<ComponentIdentity<T>> method, uint order = uint.MaxValue) =>
				OnRemoveAllTags = Events.Add(OnRemoveAllTags, method, order);
		}

		public string UniqueID { get; private set; }
		public string[] Tags => objTags[Instance].ToArray();

		public ComponentIdentity(T instance, string uniqueID) : base()
		{
			if (uniqueID == null)
			{
				Debug.LogError(1, $"Cannot create the identity of this instance ({instance}). " +
					$"The UniqueID cannot be 'null'.");
				return;
			}
			if (uniqueIDs.ContainsKey(uniqueID))
			{
				Debug.LogError(1, $"Cannot create the identity of this instance ({instance}). " +
					$"The UniqueID '{uniqueID}' already exists.");
				return;
			}
			Instance = instance;
			UniqueID = uniqueID;
			uniqueIDs.Add(uniqueID, Instance);
			objTags[instance] = new();
			OnCreate?.Invoke(this, UniqueID);
		}

		public void AddTags(params string[] tags)
		{
			if (Debug.CurrentMethodIsCalledByUser && IsCurrentlyAccessible() == false) return;
			for (int j = 0; j < tags.Length; j++)
			{
				if (objTags[Instance].Contains(tags[j])) continue;
				objTags[Instance].Add(tags[j]);
				if (tagObjs.ContainsKey(tags[j]) == false) tagObjs[tags[j]] = new List<T>();
				tagObjs[tags[j]].Add(Instance);
				OnAddTag?.Invoke(this, tags[j]);
			}
		}
		public void RemoveTags(params string[] tags)
		{
			if (Debug.CurrentMethodIsCalledByUser && IsCurrentlyAccessible() == false) return;
			for (int j = 0; j < tags.Length; j++)
			{
				if (objTags[Instance].Contains(tags[j]) == false) continue;
				objTags[Instance].Remove(tags[j]);
				tagObjs[tags[j]].Remove(Instance);
				if (tagObjs[tags[j]].Count == 0) tagObjs.Remove(tags[j]);
				OnRemoveTag?.Invoke(this, tags[j]);
			}
		}
		public void RemoveAllTags()
		{
			if (Debug.CurrentMethodIsCalledByUser && IsCurrentlyAccessible() == false) return;
			foreach (var kvp in tagObjs) OnRemoveTag?.Invoke(this, kvp.Key);
			tagObjs.Clear();
			objTags.Clear();
			OnRemoveAllTags?.Invoke(this);
		}
		public bool HasTags(params string[] tags)
		{
			if (tags == null) { Debug.LogError(1, "The tag collection cannot be 'null'."); return false; }
			for (int i = 0; i < tags.Length; i++)
				if (objTags[Instance].Contains(tags[i]) == false)
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
		public static T PickByUniqueID(string uniqueID) => uniqueIDs.ContainsKey(uniqueID) ? uniqueIDs[uniqueID] : default;
		public static T[] PickByTag(string tag) => tagObjs.ContainsKey(tag) ? tagObjs[tag].ToArray() : Array.Empty<T>();
	}
}
