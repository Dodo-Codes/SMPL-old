using SMPL.Components;
using System;
using System.Collections.Generic;

namespace SMPL.Gear
{
	internal static class Events
	{
		internal enum Type
		{
			GameStart, GameUpdate, GameStop,
			ButtonDoubleClick, ButtonPress, ButtonHold, ButtonRelease, WheelScroll, CursorLeaveWindow, CursorEnterWindow,
			LoadStart, LoadUpdate, LoadEnd, DataSlotSaveStart, DataSlotSaveUpdate, DataSlotSaveEnd,
			KeyPress, KeyHold, KeyRelease, TextInput, LanguageChange,
			Resize, Close, Focus, Unfocus, Maximize, Minimize, Fullscreen,
			ServerStart, ServerStop, ClientConnect, ClientDisconnect, ClientTakenUniqueID, MessageReceived,
			AudioStart, AudioPlay, AudioPause, AudioStop, AudioEnd, AudioLoop,
			TimerCreateAndStart, TimerUpdate, TimerEnd,
			CameraDisplay
		}
		internal struct EventArgs
		{
			public Keyboard.TextInput TextInput { get; set; }
			public Timer Timer { get; set; }
			public Camera Camera { get; set; }
			public Audio Audio { get; set; }
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

		internal static void Enable(Type notificationType, string thingUID, uint order)
		{
			if (Game.NotStartedError(3)) return;
			if (notifications.ContainsKey(notificationType) && notifications[notificationType].ContainsKey(order) &&
				notifications[notificationType][order].Contains(thingUID))
			{
				Debug.LogError(2, $"The {nameof(Thing)} '{thingUID}' is already subscribed to this event.");
				return;
			}
			if (notifications.ContainsKey(notificationType) == false) notifications[notificationType] = new();
			if (notifications[notificationType].ContainsKey(order) == false) notifications[notificationType][order] = new();
			if (notifications[notificationType][order].Contains(thingUID) == false) notifications[notificationType][order].Add(thingUID);
		}
		internal static void Disable(Type notificationType, string thingUID)
		{
			if (Game.NotStartedError(3)) return;

			var orders = notifications[notificationType];
			var error = false;
			foreach (var kvp in orders)
				if (kvp.Value.Remove(thingUID) == false)
					error = true;
			if (notifications.ContainsKey(notificationType) == false || error)
			{
				Debug.LogError(2, $"The {nameof(Thing)} '{thingUID}' has to be subscribed in order to unsubscribe from this event.");
				return;
			}
		}
		internal static void Notify(Type notificationType, EventArgs eventArgs = default)
		{
			if (notifications.ContainsKey(notificationType) == false) return;
			var orders = new SortedDictionary<uint, List<string>>(notifications[notificationType]);
			foreach (var kvp in orders)
				for (int i = 0; i < kvp.Value.Count; i++)
				{
					var thing = Thing.PickByUniqueID(kvp.Value[i]);
					switch (notificationType)
					{
						case Type.GameStart: thing.OnGameStart(); break;
						case Type.GameUpdate: thing.OnGameUpdate(); break;
						case Type.ButtonDoubleClick: thing.OnMouseButtonDoubleClick(eventArgs.Button); break;
						case Type.ButtonPress: thing.OnMouseButtonPress(eventArgs.Button); break;
						case Type.ButtonHold: thing.OnMouseButtonHold(eventArgs.Button); break;
						case Type.ButtonRelease: thing.OnMouseButtonRelease(eventArgs.Button); break;
						case Type.WheelScroll: thing.OnMouseWheelScroll(eventArgs.Wheel, eventArgs.Double[0]); break;
						case Type.CursorEnterWindow: thing.OnMouseCursorWindowEnter(); break;
						case Type.CursorLeaveWindow: thing.OnMouseCursorWindowLeave(); break;
						case Type.LoadStart: thing.OnAssetsLoadStart(); break;
						case Type.LoadUpdate: thing.OnAssetsLoadUpdate(); break;
						case Type.LoadEnd: thing.OnAssetsLoadEnd(); break;
						case Type.DataSlotSaveStart: thing.OnAssetsDataSlotSaveStart(); break;
						case Type.DataSlotSaveUpdate: thing.OnAssetsDataSlotSaveUpdate(); break;
						case Type.DataSlotSaveEnd: thing.OnAssetsDataSlotSaveEnd(); break;
						case Type.KeyPress: thing.OnKeyboardKeyPress(eventArgs.Key); break;
						case Type.KeyHold: thing.OnKeyboardKeyHold(eventArgs.Key); break;
						case Type.KeyRelease: thing.OnKeyboardKeyRelease(eventArgs.Key); break;
						case Type.TextInput: thing.OnKeyboardTextInput(eventArgs.TextInput); break;
						case Type.LanguageChange:
							thing.OnKeyboardLanguageChange(eventArgs.String[0], eventArgs.String[1], eventArgs.String[2]); break;
						case Type.Resize: thing.OnWindowResize(); break;
						case Type.Close: thing.OnWindowClose(); break;
						case Type.Focus: thing.OnWindowFocus(); break;
						case Type.Unfocus: thing.OnWindowUnfocus(); break;
						case Type.Maximize: thing.OnWindowMaximize(); break;
						case Type.Minimize: thing.OnWindowMinimize(); break;
						case Type.Fullscreen: thing.OnWindowFullscreen(); break;
						case Type.ServerStart: thing.OnMultiplayerServerStart(); break;
						case Type.ServerStop: thing.OnMultiplayerServerStop(); break;
						case Type.ClientConnect: thing.OnMultiplayerClientConnect(eventArgs.String[0]); break;
						case Type.ClientDisconnect: thing.OnMultiplayerClientDisconnect(eventArgs.String[0]); break;
						case Type.ClientTakenUniqueID: thing.OnMultiplayerClientTakenUniqueID(eventArgs.String[0]); break;
						case Type.MessageReceived: thing.OnMultiplayerMessageReceived(eventArgs.Message); break;
						case Type.AudioStart: thing.OnAudioStart(eventArgs.Audio); break;
						case Type.AudioPlay: thing.OnAudioPlay(eventArgs.Audio); break;
						case Type.AudioPause: thing.OnAudioPause(eventArgs.Audio); break;
						case Type.AudioStop: thing.OnAudioStop(eventArgs.Audio); break;
						case Type.AudioEnd: thing.OnAudioEnd(eventArgs.Audio); break;
						case Type.AudioLoop: thing.OnAudioLoop(eventArgs.Audio); break;
						case Type.TimerCreateAndStart: thing.OnTimerCreateAndStart(eventArgs.Timer); break;
						case Type.TimerUpdate: thing.OnTimerUpdate(eventArgs.Timer); break;
						case Type.TimerEnd: thing.OnTimerEnd(eventArgs.Timer); break;
						case Type.CameraDisplay: thing.OnCameraDisplay(eventArgs.Camera); break;
					}
				}
		}
	}
}
