using System;
using System.Collections.Generic;

namespace SMPL
{
	public abstract class Events
	{
		public delegate void ParamsZero();
		public delegate void ParamsOne<T>(T param1);
		public delegate void ParamsTwo<T1, T2>(T1 param1, T2 param2);
		public delegate void ParamsThree<T1, T2, T3>(T1 param1, T2 param2, T3 param3);
		public delegate void ParamsFour<T1, T2, T3, T4>(T1 param1, T2 param2, T3 param3, T4 param4);

		internal static ParamsZero Add(ParamsZero pz, Action method, uint order)
		{
			if (pz == null) { pz = new ParamsZero(method); return pz; }
			var l = pz.GetInvocationList();
			for (uint i = 0; i < l.Length; i++)
			{
				var a = l[i] as ParamsZero;
				pz -= a; if (i == order) pz += new ParamsZero(method); pz += a;
			}
			if (order >= l.Length) pz += new ParamsZero(method);
			return pz;
		}
		internal static ParamsOne<T1> Add<T1>(ParamsOne<T1> pz, Action<T1> method, uint order)
		{
			if (pz == null) { pz = new ParamsOne<T1>(method); return pz; }
			var l = pz.GetInvocationList();
			for (uint i = 0; i < l.Length; i++)
			{
				var a = l[i] as ParamsOne<T1>;
				pz -= a; if (i == order) pz += new ParamsOne<T1>(method); pz += a;
			}
			if (order >= l.Length) pz += new ParamsOne<T1>(method);
			return pz;
		}
		internal static ParamsTwo<T1, T2> Add<T1, T2>(ParamsTwo<T1, T2> pz, Action<T1, T2> method, uint order)
		{
			if (pz == null) { pz = new ParamsTwo<T1, T2>(method); return pz; }
			var l = pz.GetInvocationList();
			for (uint i = 0; i < l.Length; i++)
			{
				var a = l[i] as ParamsTwo<T1, T2>;
				pz -= a; if (i == order) pz += new ParamsTwo<T1, T2>(method); pz += a;
			}
			if (order >= l.Length) pz += new ParamsTwo<T1, T2>(method);
			return pz;
		}
		internal static ParamsThree<T1, T2, T3> Add<T1, T2, T3>(
			ParamsThree<T1, T2, T3> pz, Action<T1, T2, T3> method, uint order)
		{
			if (pz == null) { pz = new ParamsThree<T1, T2, T3>(method); return pz; }
			var l = pz.GetInvocationList();
			for (uint i = 0; i < l.Length; i++)
			{
				var a = l[i] as ParamsThree<T1, T2, T3>;
				pz -= a; if (i == order) pz += new ParamsThree<T1, T2, T3>(method); pz += a;
			}
			if (order >= l.Length) pz += new ParamsThree<T1, T2, T3>(method);
			return pz;
		}
		internal static ParamsFour<T1, T2, T3, T4> Add<T1, T2, T3, T4>(
			ParamsFour<T1, T2, T3, T4> pz, Action<T1, T2, T3, T4> method, uint order)
		{
			if (pz == null) { pz = new ParamsFour<T1, T2, T3, T4>(method); return pz; }
			var l = pz.GetInvocationList();
			for (uint i = 0; i < l.Length; i++)
			{
				var a = l[i] as ParamsFour<T1, T2, T3, T4>;
				pz -= a; if (i == order) pz += new ParamsFour<T1, T2, T3, T4>(method); pz += a;
			}
			if (order >= l.Length) pz += new ParamsFour<T1, T2, T3, T4>(method);
			return pz;
		}

		internal static SortedDictionary<int, List<Events>> instances = new();
		internal static Dictionary<Events, int> instancesOrder = new();
	}
}
