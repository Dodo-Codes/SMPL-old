using System.Collections.Generic;

namespace SMPL
{
	public class IdentityComponent<T>
	{
		private static Dictionary<string, T> UniqueIDs { get; set; } = new();
		private static Dictionary<string, List<T>> TagObjs { get; set; } = new();
		private static Dictionary<T, List<string>> ObjTags { get; set; } = new();
		private T Instance { get; set; }

		public string UniqueID { get; private set; }

		public void Identify(T instance, string uniqueID, params string[] tags)
		{
			if (Instance != null)
			{
				Debug.LogError(1, $"This instance ({instance}) is already identified.");
				return;
			}
			if (UniqueIDs.ContainsKey(uniqueID))
			{
				Debug.LogError(1, $"Cannot identify this instance ({instance}). The UniqueID '{uniqueID}' already exists.");
				return;
			}
			UniqueID = uniqueID;
			UniqueIDs.Add(uniqueID, Instance);
			Instance = instance;
			ObjTags[instance] = new();
			AddTags(tags);
		}
		public bool IsIdentified()
		{
			if (Instance == null)
			{
				Debug.LogError(1, $"This instance ({Instance}) is not identified. Please do so before using its identity.");
				return false;
			}
			return true;
		}
		public void AddTags(params string[] tags)
		{
			if (IsIdentified() == false) return;
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
			if (IsIdentified() == false) return;
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
			if (IsIdentified() == false) return false;
			foreach (var tag in tags)
			{
				if (ObjTags[Instance].Contains(tag) == false) return false;
			}
			return true;
		}
		public List<string> GetTags() => IsIdentified() == false ? new() : ObjTags[Instance];

		public static bool TagsExist(params string[] tags)
		{
			foreach (var tag in tags)
			{
				if (TagObjs.ContainsKey(tag) == false) return false;
			}
			return true;
		}
		public static T PickByUniqueID(string uniqueID) => UniqueIDs.ContainsKey(uniqueID) ? UniqueIDs[uniqueID] : default;
		public static List<T> PickByTag(string tag) => TagObjs.ContainsKey(tag) ? TagObjs[tag] : new();
	}
}
