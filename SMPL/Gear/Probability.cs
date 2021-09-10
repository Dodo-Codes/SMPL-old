using Newtonsoft.Json;
using SMPL.Components;
using SMPL.Data;
using System;
using System.Collections.Generic;

namespace SMPL.Gear
{
	public static class Probability
	{
		public struct Case
		{
			public Number.Range ChanceRange { get; set; }
			public double ChancePercent { get; set; }

			public Case(double chancePercent = 0, Number.Range chanceRange = new())
			{
				ChanceRange = chanceRange;
				ChancePercent = chancePercent;
			}
		}
		public class Table : Thing
		{
			[JsonProperty]
			private readonly Dictionary<string, Case> cases = new();

			public Table(string uniqueID) : base(uniqueID)
			{
				if (cannotCreate) { ErrorAlreadyHasUID(uniqueID); Destroy(); }
			}
			public override void Destroy()
			{
				cases.Clear();
				base.Destroy();
			}

			public Case GetCase(string uniqueID)
			{
				if (cases.ContainsKey(uniqueID) == false)
				{
					Debug.LogError(1, $"No {nameof(Probability)}.{nameof(Table)}.{nameof(Case)} with uniqueID '{uniqueID}' was found.");
					return default;
				}
				return cases[uniqueID];
			}
			public void SetCase(string uniqueID, Case _case)
			{
				if (uniqueID == null)
				{
					Debug.LogError(1, $"{nameof(Case)}'s uniqueID cannot be 'null'.");
					return;
				}
				cases[uniqueID] = _case;
			}
		}

		public static double Randomize(Number.Range range, double precision = 0, double seed = double.NaN)
		{
			precision = (int)Number.Limit(precision, new Number.Range(0, 5));
			precision = Number.Power(10, precision);

			range.Lower *= precision;
			range.Upper *= precision;

			var s = new Random(double.IsNaN(seed) ? Guid.NewGuid().GetHashCode() : (int)Number.Round(seed));
			var randInt = s.Next((int)range.Lower, (int)range.Upper + 1);

			return Number.Limit(randInt / precision, range);
		}
		public static bool HasChance(double percent)
		{
			percent = Number.Limit(percent, new Number.Range(0, 100), Number.Limitation.ClosestBound);
			var n = Randomize(new Number.Range(1, 100), 0); // should not roll 0 so it doesn't return true with 0% (outside of roll)
			return n <= percent;
		}
	}
}
