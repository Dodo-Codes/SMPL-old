using System;
using System.Collections.Generic;

namespace SMPL.Gear
{
	internal static class Events
	{
		internal delegate void ParamsZero();
		internal delegate void ParamsOne<T>(T param1);
		internal delegate void ParamsTwo<T1, T2>(T1 param1, T2 param2);
		internal delegate void ParamsThree<T1, T2, T3>(T1 param1, T2 param2, T3 param3);
		internal delegate void ParamsFour<T1, T2, T3, T4>(T1 param1, T2 param2, T3 param3, T4 param4);

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

		internal enum Type
		{
			MouseButtonDoubleClick, MouseButtonPress, MouseButtonHold, MouseButtonRelease, MouseWheelScroll,
			MouseCursorLeaveWindow, MouseCursorEnterWindow
		}
		internal struct EventArgs
		{
			public Mouse.Button MouseButton { get; set; }
			public Mouse.Wheel Wheel { get; set; }
			public double[] Double { get; set; }
			public int[] Int { get; set; }
			public int[] String { get; set; }
		}
		internal static Dictionary<Type, SortedDictionary<uint, List<string>>> notifications = new();

		internal static void NotificationEnable(Type notificationType, string thingUID, uint order)
		{
			if (notifications.ContainsKey(notificationType) == false) notifications[notificationType] = new();
			if (notifications[notificationType].ContainsKey(order) == false) notifications[notificationType][order] = new();
			if (notifications[notificationType][order].Contains(thingUID) == false) notifications[notificationType][order].Add(thingUID);
		}
		internal static void NotificationDisable(Type notificationType, string thingUID)
		{
			if (notifications.ContainsKey(notificationType) == false) return;
			var orders = notifications[notificationType];
			foreach (var kvp in orders)
				kvp.Value.Remove(thingUID);
		}
		internal static void Notify(Type notificationType, EventArgs eventArgs = default)
		{
			if (notifications.ContainsKey(notificationType) == false) return;
			var orders = notifications[notificationType];
			foreach (var kvp in orders)
				for (int i = 0; i < kvp.Value.Count; i++)
				{
					var thing = Thing.PickByUniqueID(kvp.Value[i]);
					switch (notificationType)
					{
						case Type.MouseButtonDoubleClick: thing.OnMouseButtonDoubleClick(eventArgs.MouseButton); break;
						case Type.MouseButtonPress: break;
						case Type.MouseButtonHold: break;
						case Type.MouseButtonRelease: break;
						case Type.MouseWheelScroll: break;
						case Type.MouseCursorEnterWindow: break;
						case Type.MouseCursorLeaveWindow: break;
					}
				}
		}
	}
}
