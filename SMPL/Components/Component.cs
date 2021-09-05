using SMPL.Gear;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SMPL.Components
{
	public class Component
	{
		private string uniqueID;

		//===========

		internal static readonly Dictionary<string, Component> uniqueIDs = new();
		internal static readonly Dictionary<string, List<Component>> tagObjs = new();
		internal static readonly Dictionary<Component, List<string>> objTags = new();
		internal bool cannotCreate;

		internal bool ErrorIfDestroyed()
		{
			if (IsDestroyed)
			{
				Debug.LogError(2, $"This {nameof(Component)} is destroyed.");
				return true;
			}
			return false;
		}
		internal static void ErrorAlreadyHasUID(string uniqueID)
		{
			Debug.LogError(2, $"A {nameof(Component)} with uniqueID '{uniqueID}' already exists.\n" +
				$"The newly created {nameof(Component)} was destroyed because of this.");
		}

		// ======

		public static string[] AllUniqueIDs => uniqueIDs.Keys.ToArray();
		public static Component[] AllInstances => uniqueIDs.Values.ToArray();
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
				if (Component.uniqueIDs.ContainsKey(uniqueIDs[i]) == false)
					return false;
			return true;
		}
		public static Component PickByUniqueID(string uniqueID) => uniqueIDs.ContainsKey(uniqueID) ? uniqueIDs[uniqueID] : default;
		public static Component[] PickByTag(string tag) => tagObjs.ContainsKey(tag) ? tagObjs[tag].ToArray() : Array.Empty<Component>();

		// ======

		public bool IsDestroyed { get; private set; }
		public string UniqueID
		{
			get { return ErrorIfDestroyed() ? default : uniqueID; }
			private set { uniqueID = value; }
		}
		public string[] Tags => ErrorIfDestroyed() ? Array.Empty<string>() : objTags[this].ToArray();

		public Component(string uniqueID)
		{
			if (uniqueIDs.ContainsKey(uniqueID)) { cannotCreate = true; return; }

			UniqueID = uniqueID;
			uniqueIDs[uniqueID] = this;
			objTags[this] = new();
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
				if (tagObjs.ContainsKey(tags[j]) == false) tagObjs[tags[j]] = new List<Component>();
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
	}
}
