using System.Collections.Generic;

namespace SMPL
{
	public static class Gate
	{
		internal static Dictionary<string, bool> gates = new();
		internal static Dictionary<string, int> gateEntries = new();

		public static void Remove(string uniqueID)
		{
			if (Debug.DoesNotExistError(gates, uniqueID, nameof(Gate))) return;
			gates.Remove(uniqueID);
			gateEntries.Remove(uniqueID);
		}
		public static void Reset(string uniqueID)
		{
			if (Debug.DoesNotExistError(gates, uniqueID, nameof(Gate))) return;
			gateEntries[uniqueID] = 0;
		}
		public static double GetEntryCount(string uniqueID)
		{
			return
				Debug.DoesNotExistError(gates, uniqueID, nameof(Gate)) ? double.NaN :
				(gateEntries.ContainsKey(uniqueID) ? gateEntries[uniqueID] : 0);
		}
		public static bool TryEnter(string uniqueID, bool condition, uint maxEntries = uint.MaxValue)
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
	}
}
