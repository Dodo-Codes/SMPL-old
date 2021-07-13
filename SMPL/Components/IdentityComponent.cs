using System.Collections.Generic;
using System.Linq;

namespace SMPL
{
	public class IdentityComponent<T>
	{
		private static Dictionary<string, T> UniqueIDs { get; set; } = new();
		private static Dictionary<string, List<T>> TagObjs { get; set; } = new();
		private static Dictionary<T, List<string>> ObjTags { get; set; } = new();
		private T Instance { get; set; }

		public string UniqueID { get; private set; }

		public IdentityComponent(T instance, string uniqueID, params string[] tags)
		{
			if (UniqueIDs.ContainsKey(uniqueID))
			{
				Debug.LogError(1, $"Cannot identify this instance ({instance}). The UniqueID '{uniqueID}' already exists.");
				return;
			}
			Instance = instance;
			UniqueID = uniqueID;
			UniqueIDs.Add(uniqueID, Instance);
			ObjTags[instance] = new();
			AddTags(tags);
		}

		public void AddTags(params string[] tags)
		{
			foreach (var tag in tags)
			{
				if (ObjTags[Instance].Contains(tag)) continue;
				ObjTags[Instance].Add(tag);
				if (TagObjs.ContainsKey(tag) == false) TagObjs[tag] = new List<T>();
				TagObjs[tag].Add(Instance);
			}
		}
		public void RemoveTags(params string[] tags)
		{
			foreach (var tag in tags)
			{
				if (ObjTags[Instance].Contains(tag) == false) continue;
				ObjTags[Instance].Remove(tag);
				TagObjs[tag].Remove(Instance);
				if (TagObjs[tag].Count == 0) TagObjs.Remove(tag);
			}
		}
		public bool HasTags(params string[] tags)
		{
			foreach (var tag in tags)
			{
				if (ObjTags[Instance].Contains(tag) == false) return false;
			}
			return true;
		}
		public string[] GetTags() => ObjTags[Instance].ToArray();

		public static string[] GetAllUniqueIDs() => UniqueIDs.Keys.ToArray();
		public static T[] GetAll() => UniqueIDs.Values.ToArray();
		public static string[] GetAllTags() => TagObjs.Keys.ToArray();
		public static bool TagsExist(params string[] tags)
		{
			foreach (var tag in tags)
			{
				if (TagObjs.ContainsKey(tag) == false) return false;
			}
			return true;
		}
		public static T PickByUniqueID(string uniqueID) => UniqueIDs.ContainsKey(uniqueID) ? UniqueIDs[uniqueID] : default;
		public static T[] PickByTag(string tag) => TagObjs.ContainsKey(tag) ?
			TagObjs[tag].ToArray() : System.Array.Empty<T>();
	}
}
