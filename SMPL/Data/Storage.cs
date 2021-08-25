using SMPL.Gear;
using System.Collections.Generic;

namespace SMPL.Data
{
	public struct Storage<UniqueKeyT, ValueT>
	{
		private List<int> indexes;
		private List<UniqueKeyT> keys;
		private List<ValueT> values;
		private Dictionary<UniqueKeyT, ValueT> dict;

		public void Expand(int index, UniqueKeyT uniqueKey, ValueT value)
		{
			if (keys == null) keys = new List<UniqueKeyT>();
			if (indexes == null) indexes = new List<int>();
			if (dict == null) dict = new Dictionary<UniqueKeyT, ValueT>();
			if (values == null) values = new List<ValueT>();

			if (index >= values.Count)
			{
				var oldListK = new List<UniqueKeyT>(keys);
				var oldListV = new List<ValueT>(values);
				values = new List<ValueT>();
				keys = new List<UniqueKeyT>();
				for (int i = 0; i < index; i++)
				{
					values.Add(default);
					keys.Add(default);
				}
				for (int i = 0; i < oldListV.Count; i++)
				{
					values[i] = oldListV[i];
					keys[i] = oldListK[i];
				}
			}

			dict.Add(uniqueKey, value);
			values.Insert(index, value);
			keys.Insert(index, uniqueKey);
			indexes.Add(index);

			var sameIndexMet = false;
			for (int i = 0; i < indexes.Count; i++)
			{
				if (sameIndexMet && indexes[i] == index)
				{
					indexes[i]++;
					continue;
				}
				if (indexes[i] > index) indexes[i]++;
				else if (sameIndexMet == false && indexes[i] == index) sameIndexMet = true;
			}
		}
		public void ShrinkAt(int index)
		{
			if (IndexNotFound(index)) return;
			indexes.Remove(index);
			values.RemoveAt(index);
			dict.Remove(keys[index]);
			keys.RemoveAt(index);
		}
		public void ShrinkAt(UniqueKeyT uniqueKey)
		{
			if (UniqueKeyNotFound(uniqueKey)) return;
			indexes.Remove(keys.IndexOf(uniqueKey));
			values.RemoveAt(keys.IndexOf(uniqueKey));
			dict.Remove(uniqueKey);
			keys.Remove(uniqueKey);
		}
		public void ReplaceAt(int index, ValueT value)
		{
			if (IndexNotFound(index)) return;
			values[index] = value;
			dict[keys[index]] = value;
		}
		public void ReplaceAt(UniqueKeyT uniqueKey, ValueT value)
		{
			if (UniqueKeyNotFound(uniqueKey)) return;
			dict[uniqueKey] = value;
			values[keys.IndexOf(uniqueKey)] = value;
		}
		public void Free()
		{
			if (indexes == null || keys == null || values == null || dict == null || indexes.Count == 0) return;
			indexes.Clear();
			keys.Clear();
			values.Clear();
			dict.Clear();
		}
		public void Shuffle()
		{
			for (int i = 0; i < indexes.Count - 1; i++)
			{
				var j = (int)Number.Random(new Bounds(i, indexes.Count - 1));
				var tempIndex = indexes[i];
				//var tempKey = keys[tempIndex];
				var tempValue = values[tempIndex];

				dict[keys[indexes[i]]] = values[indexes[j]];
				//keys[indexes[i]] = keys[indexes[j]];
				values[indexes[i]] = values[indexes[j]];
				indexes[i] = indexes[j];

				dict[keys[indexes[j]]] = tempValue;
				//keys[indexes[j]] = tempKey;
				values[indexes[j]] = tempValue;
				indexes[j] = tempIndex;
			}
		}

		public int DataAmount => indexes == null ? 0 : indexes.Count;
		public ValueT ValueAt(UniqueKeyT uniqueKey)
		{
         return UniqueKeyNotFound(uniqueKey) ? default : dict[uniqueKey];
      }
      public ValueT ValueAt(int index)
		{
         return IndexNotFound(index) ? default : values[index];
      }
      public UniqueKeyT UniqueKeyAt(int index)
		{
         return IndexNotFound(index) ? default : keys[index];
      }
      public int IndexAt(UniqueKeyT uniqueKey)
		{
         return UniqueKeyNotFound(uniqueKey) ? default : keys.IndexOf(uniqueKey);
      }

      public int[] Indexes => indexes == null ? System.Array.Empty<int>() : indexes.ToArray();
		public UniqueKeyT[] UniqueKeys
		{
			get
			{
				if (indexes == null) return System.Array.Empty<UniqueKeyT>();
				var result = new List<UniqueKeyT>();
				for (int i = 0; i < indexes.Count; i++)
				{
					result.Add(keys[indexes[i]]);
				}
				return result.ToArray();
			}
		}
		public ValueT[] Values
		{
			get
			{
				if (indexes == null) return System.Array.Empty<ValueT>();
				var result = new List<ValueT>();
				for (int i = 0; i < indexes.Count; i++)
				{
					result.Add(values[indexes[i]]);
				}
				return result.ToArray();
			}
		}

		public bool HasIndex(int index) => indexes != null && indexes.Contains(index);
		public bool HasUniqueKey(UniqueKeyT uniqueKey) => keys != null && keys.Contains(uniqueKey);
		public bool HasValue(ValueT value) => values != null && values.Contains(value);

		internal bool UniqueKeyNotFound(UniqueKeyT uniqueKey)
      {
			if (HasUniqueKey(uniqueKey)) return false;
			Debug.LogError(1, $"The unique key '{uniqueKey}' was not found.");
			return true;
		}
		internal bool IndexNotFound(int index)
		{
			if (HasIndex(index)) return false;
			Debug.LogError(1, $"The index '{index}' was not found.");
			return true;
		}
	}
}
