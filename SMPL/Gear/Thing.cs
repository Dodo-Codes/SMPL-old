using Newtonsoft.Json;
using SMPL.Components;
using SMPL.Data;
using SMPL.Gear;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SMPL.Gear
{
	[JsonObject(MemberSerialization.OptIn)]
	public class Thing
	{
		private static uint newObjUID;
		internal static Dictionary<uint, Thing> uids = new();

		public static List<uint> UIDs => new(uids.Keys);
		public static Thing Pick(uint uid) => uid != default && uids.ContainsKey(uid) ? uids[uid] : default;
		public static List<Thing> Pick(string tag)
		{
			var result = new List<Thing>();
			foreach (var kvp in uids)
				if (kvp.Value.Tags.Contains(tag))
					result.Add(kvp.Value);
			return result;
		}

		internal bool ErrorIfDestroyed()
		{
			if (IsDestroyed)
			{
				Debug.LogError(2, $"This {nameof(Thing)} is destroyed.");
				return true;
			}
			return false;
		}

		[JsonProperty]
		public bool IsDestroyed { get; private set; }
		[JsonProperty]
		public List<string> Tags { get; set; }

		public uint UID { get; }
		public List<Game.Event> Subscriptions { get; private set; } = new();

		public Thing()
		{
			newObjUID++;
			UID = newObjUID;
			uids[newObjUID] = this;
		}
		public virtual void Destroy()
		{
			if (ErrorIfDestroyed() || UID == default)
				return;
			uids.Remove(UID);
			Unsubscribe();
			IsDestroyed = true;
		}

		public void Subscribe(Game.Event gameEvent, uint order = uint.MaxValue)
		{
			if (Events.Enable(gameEvent, UID, order))
				Subscriptions.Add(gameEvent);
		}
		public void Unsubscribe(Game.Event gameEvent)
		{
			if (Events.Disable(gameEvent, UID))
				Subscriptions.Remove(gameEvent);
		}
		public void Unsubscribe()
		{
			for (int i = 0; i < Subscriptions.Count; i++)
				Events.Disable(Subscriptions[i], UID);
			Subscriptions.Clear();
		}

		public virtual void OnGameAction(Game.Action action) { }
		public virtual void OnCameraDisplay(Camera camera) { }
		public virtual void OnWindowAction(Window.Action action) { }

		public virtual void OnMouseButtonAction(Mouse.ButtonAction action, Mouse.Button button) { }
		public virtual void OnMouseWheelScroll(Mouse.Wheel wheel, double delta) { }
		public virtual void OnMouseCursorAction(Mouse.Cursor.Action action) { }

		public virtual void OnAssetLoadEnd(string path) { }
		public virtual void OnAssetDataSlotSaveEnd(string path) { }

		public virtual void OnKeyboardKeyAction(Keyboard.KeyAction action, Keyboard.Key key) { }
		public virtual void OnKeyboardTextInput(Keyboard.TextInput textInput) { }
		public virtual void OnKeyboardLanguageChange(string englishName, string nativeName, string languageCode) { }

		public virtual void OnMultiplayerServerAction(Multiplayer.Action action) { }
		public virtual void OnMultiplayerClientAction(Multiplayer.Action action, string clientUniqueID) { }
		public virtual void OnMultiplayerMessageReceived(Multiplayer.Message message) { }

		public virtual void OnAudioAction(Audio.Action action, Audio audio) { }
		public virtual void OnTimerAction(Timer.Action action, Timer timer) { }
	}
}
