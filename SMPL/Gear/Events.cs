using System;
using System.Collections.Generic;

namespace SMPL.Gear
{
	internal static class Events
	{
		internal enum Type
		{
			ButtonDoubleClick, ButtonPress, ButtonHold, ButtonRelease, WheelScroll, CursorLeaveWindow, CursorEnterWindow,

			LoadStart, LoadUpdate, LoadEnd, DataSlotSaveStart, DataSlotSaveUpdate, DataSlotSaveEnd,

			KeyPress, KeyHold, KeyRelease, TextInput, LanguageChange,

			Resize, Close, Focus, Unfocus, Maximize, Minimize,

			ServerStart, ServerStop, ClientConnect, ClientDisconnet, ClientTakenUniqueID, MessageReceived
		}
		internal struct EventArgs
		{
			public Multiplayer.Message Message { get; set; }
			public Keyboard.Key Key { get; set; }
			public Mouse.Button Button { get; set; }
			public Mouse.Wheel Wheel { get; set; }
			public double[] Double { get; set; }
			public int[] Int { get; set; }
			public string[] String { get; set; }
			public bool[] Bool { get; set; }
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
						case Type.ButtonDoubleClick: thing.OnMouseButtonDoubleClick(eventArgs.Button); break;
						case Type.ButtonPress: thing.OnMouseButtonPress(eventArgs.Button); break;
						case Type.ButtonHold: thing.OnMouseButtonHold(eventArgs.Button); break;
						case Type.ButtonRelease: thing.OnMouseButtonRelease(eventArgs.Button); break;
						case Type.WheelScroll: thing.OnMouseWheelScroll(eventArgs.Wheel, eventArgs.Double[0]); break;
						case Type.CursorEnterWindow: thing.OnMouseCursorWindowEnter(); break;
						case Type.CursorLeaveWindow: thing.OnMouseCursorWindowLeave(); break;
					}
				}
		}
	}
}
