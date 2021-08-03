using System.Collections.Generic;

namespace SMPL
{
	public static class Gate
	{
		internal static Dictionary<string, bool> gates = new();
		internal static Dictionary<string, int> gateEntries = new();

		public static void Remove(string uniqueID)
		{
			if (NotFound(uniqueID)) return;
			gates.Remove(uniqueID);
			gateEntries.Remove(uniqueID);
		}
		public static void Reset(string uniqueID)
		{
			if (NotFound(uniqueID)) return;
			gateEntries[uniqueID] = 0;
		}
		public static double EntryCount(string uniqueID)
		{
			return
				NotFound(uniqueID) ? double.NaN :
				(gateEntries.ContainsKey(uniqueID) ? gateEntries[uniqueID] : 0);
		}
		public static bool EnterOnceWhile(string uniqueID, bool condition, uint maxEntries = uint.MaxValue)
		{
			if (gates.ContainsKey(uniqueID) == false && condition == false) return false;
			else if (gates.ContainsKey(uniqueID) == false && condition == true)
			{
				gates[uniqueID] = true;
				gateEntries[uniqueID] = 1;
				return true;
			}
			else
			{
				if (gates[uniqueID] == true && condition == true) return false;
				else if (gates[uniqueID] == false && condition == true)
				{
					gates[uniqueID] = true;
					gateEntries[uniqueID]++;
					return true;
				}
				else if (gateEntries[uniqueID] < maxEntries) gates[uniqueID] = false;
			}
			return false;
		}

		internal static bool NotFound(string uniqueID)
      {
			if (gates.ContainsKey(uniqueID) == false)
			{
				Debug.LogError(1, $"The unique ID '{uniqueID}' was not found.");
				return true;
			}
			return false;
		}
	}
}
