using System.Collections.Generic;
using System.Linq;
using static SMPL.Events;

namespace SMPL
{
	public class ComponentIdentity<T> : ComponentAccess
	{
		private static Dictionary<string, T> UniqueIDs { get; set; } = new();
		private static Dictionary<string, List<T>> TagObjs { get; set; } = new();
		private static Dictionary<T, List<string>> ObjTags { get; set; } = new();
		private T Instance { get; set; }

		public string UniqueID { get; private set; }
		public string[] Tags { get { return ObjTags[Instance].ToArray(); } }

		public ComponentIdentity(T instance, string uniqueID, params string[] tags)
		{
			if (UniqueIDs.ContainsKey(uniqueID))
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
			UniqueIDs.Add(uniqueID, Instance);
			ObjTags[instance] = new();
			AddTags(tags);

			var n = D(instances); foreach (var kvp in n) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++)
					e[i].OnIdentityCreateSetup(this); }
			var n1 = D(instances); foreach (var kvp in n1) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++)
					e[i].OnIdentityCreate(this); }
		}

		public void AddTags(params string[] tags)
		{
			if (Debug.currentMethodIsCalledByUser && IsCurrentlyAccessible() == false) return;
			for (int j = 0; j < tags.Length; j++)
			{
				if (ObjTags[Instance].Contains(tags[j])) continue;
				ObjTags[Instance].Add(tags[j]);
				if (TagObjs.ContainsKey(tags[j]) == false) TagObjs[tags[j]] = new List<T>();
				TagObjs[tags[j]].Add(Instance);

				var n = D(instances); foreach (var kvp in n) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++)
						e[i].OnIdentityTagAddSetup(this, tags[j]); }
				var n1 = D(instances); foreach (var kvp in n1) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++)
						e[i].OnIdentityTagAdd(this, tags[j]); }
			}
		}
		public void RemoveTags(params string[] tags)
		{
			if (Debug.currentMethodIsCalledByUser && IsCurrentlyAccessible() == false) return;
			for (int j = 0; j < tags.Length; j++)
			{
				if (ObjTags[Instance].Contains(tags[j]) == false) continue;
				ObjTags[Instance].Remove(tags[j]);
				TagObjs[tags[j]].Remove(Instance);
				if (TagObjs[tags[j]].Count == 0) TagObjs.Remove(tags[j]);

				var n = D(instances); foreach (var kvp in n) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++)
						e[i].OnIdentityTagRemoveSetup(this, tags[j]); }
				var n1 = D(instances); foreach (var kvp in n1) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++)
						e[i].OnIdentityTagRemove(this, tags[j]); }
			}
		}
		public bool HasTags(params string[] tags)
		{
			for (int i = 0; i < tags.Length; i++)
			{
				if (ObjTags[Instance].Contains(tags[i]) == false) return false;
			}
			return true;
		}

		public static string[] AllUniqueIDs { get { return UniqueIDs.Keys.ToArray(); } }
		public static T[] AllInstances { get { return UniqueIDs.Values.ToArray(); } }
		public static string[] AllTags { get { return TagObjs.Keys.ToArray(); } }

		public static bool TagsExist(params string[] tags)
		{
			for (int i = 0; i < tags.Length; i++)
			{
				if (TagObjs.ContainsKey(tags[i]) == false) return false;
			}
			return true;
		}
		public static T PickByUniqueID(string uniqueID) => UniqueIDs.ContainsKey(uniqueID) ? UniqueIDs[uniqueID] : default;
		public static T[] PickByTag(string tag) => TagObjs.ContainsKey(tag) ?
			TagObjs[tag].ToArray() : System.Array.Empty<T>();
	}
}
