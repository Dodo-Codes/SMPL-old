using System;
using System.Collections.Generic;
using System.Linq;

namespace SMPL
{
	public class ComponentIdentity<T> : ComponentAccess
	{
		private static Dictionary<string, T> uniqueIDs = new();
		private static Dictionary<string, List<T>> tagObjs = new();
		private static Dictionary<T, List<string>> objTags = new();
		private T Instance { get; set; }

		public string UniqueID { get; private set; }
		public string[] Tags { get { return objTags[Instance].ToArray(); } }

		public ComponentIdentity(T instance, string uniqueID, params string[] tags) : base()
		{
			if (uniqueIDs.ContainsKey(uniqueID))
			{
				Debug.LogError(1, $"Cannot create the identity of this instance ({instance}). " +
					$"The UniqueID '{uniqueID}' already exists.");
				return;
			}
			if (uniqueID == null)
			{
				Debug.LogError(1, $"Cannot create the identity of this instance ({instance}). " +
					$"The UniqueID cannot be 'null'.");
				return;
			}
			Instance = instance;
			UniqueID = uniqueID;
			uniqueIDs.Add(uniqueID, Instance);
			objTags[instance] = new();
			AddTags(tags);

			OnCreate?.Invoke(this);
		}

		private static event Events.ParamsOne<ComponentIdentity<T>> OnCreate;
		private static event Events.ParamsTwo<ComponentIdentity<T>, string> OnTagAdd;
		private static event Events.ParamsTwo<ComponentIdentity<T>, string> OnTagRemove;

		public static void CallOnCreate(Action<ComponentIdentity<T>> method, uint order = uint.MaxValue) =>
			OnCreate = Events.Add(OnCreate, method, order);
		public static void CallOnTagAdd(Action<ComponentIdentity<T>, string> method, uint order = uint.MaxValue) =>
			OnTagAdd = Events.Add(OnTagAdd, method, order);
		public static void CallOnTagRemove(Action<ComponentIdentity<T>, string> method, uint order = uint.MaxValue) =>
			OnTagRemove = Events.Add(OnTagRemove, method, order);

		public void AddTags(params string[] tags)
		{
			if (Debug.CurrentMethodIsCalledByUser && IsCurrentlyAccessible() == false) return;
			for (int j = 0; j < tags.Length; j++)
			{
				if (objTags[Instance].Contains(tags[j])) continue;
				objTags[Instance].Add(tags[j]);
				if (tagObjs.ContainsKey(tags[j]) == false) tagObjs[tags[j]] = new List<T>();
				tagObjs[tags[j]].Add(Instance);

				OnTagAdd?.Invoke(this, tags[j]);
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

				OnTagRemove?.Invoke(this, tags[j]);
			}
		}
		public void RemoveAllTags()
		{
			if (Debug.CurrentMethodIsCalledByUser && IsCurrentlyAccessible() == false) return;
			tagObjs.Clear();
			objTags.Clear();
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
