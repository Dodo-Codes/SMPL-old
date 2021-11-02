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
		private static bool isClone;

		internal static Dictionary<string, Thing> uniqueIDs = new();
		internal static Dictionary<string, List<Thing>> tagObjs = new();
		internal static Dictionary<Thing, List<string>> objTags = new();

		public static string[] AllUniqueIDs => uniqueIDs.Keys.ToArray();
		public static Thing[] AllInstances => uniqueIDs.Values.ToArray();
		public static string[] AllTags => tagObjs.Keys.ToArray();

		internal bool cannotCreate;
		private string uniqueID;
		[JsonProperty]
		public bool IsDestroyed { get; private set; }
		[JsonProperty]
		public string UniqueID
		{
			get { return ErrorIfDestroyed() ? default : uniqueID; }
			set
			{
				var old = uniqueID;
				uniqueID = value;
				if (old != null && isClone == false) uniqueIDs.Remove(old);
				uniqueIDs[value] = this;
			}
		}
		[JsonProperty]
		public string[] Tags => ErrorIfDestroyed() || objTags.ContainsKey(this) == false ?
			Array.Empty<string>() : objTags[this].ToArray();

		public Thing(string uniqueID)
		{
			if (uniqueIDs.ContainsKey(uniqueID) && isClone == false)
			{
				cannotCreate = true;

				Debug.LogError(-1, $"The {nameof(Thing)} with uniqueID '{uniqueID}' cannot be created because " +
					$"another {nameof(Thing)} with the same uniqueID exists.");
				return;
			}

			UniqueID = uniqueID;
			objTags[this] = new();
		}
		public static T CloneObject<T>(T obj, string newUniqueID = default)
		{
			isClone = true;
			var oldUID = "";
			if (obj is Thing thing)
			{
				oldUID = thing.UniqueID;
				thing.UniqueID = newUniqueID;
			}
			var clone = JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(obj));
			if (clone is Thing clonedThing && obj is Thing thing2)
			{
				clonedThing.UniqueID = newUniqueID;
				thing2.UniqueID = oldUID;
				objTags[clonedThing] = new(objTags[clonedThing]);
			}
			isClone = false;
			return clone;
		}
		public virtual void Destroy()
		{
			if (ErrorIfDestroyed() || uniqueID == null) return;
			uniqueIDs.Remove(uniqueID);
			if (objTags.ContainsKey(this))
			{
				RemoveAllTags();
				objTags.Remove(this);
			}

			foreach (var kvp in Events.notifications)
				foreach (var kvp2 in kvp.Value)
					if (kvp2.Value.Contains(uniqueID))
						kvp2.Value.Remove(uniqueID);

			IsDestroyed = true;
		}

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
				if (Thing.uniqueIDs.ContainsKey(uniqueIDs[i]) == false)
					return false;
			return true;
		}
		public static Thing PickByUniqueID(string uniqueID)
		{
			return uniqueID != null && uniqueIDs.ContainsKey(uniqueID) ? uniqueIDs[uniqueID] : default;
		}
		public static Thing[] PickByTag(string tag) => tagObjs.ContainsKey(tag) ? tagObjs[tag].ToArray() : Array.Empty<Thing>();

		public void AddTags(params string[] tags)
		{
			if (ErrorIfDestroyed())
				return;
			if (objTags.ContainsKey(this) == false)
				return;
			for (int j = 0; j < tags.Length; j++)
			{
				if (objTags[this].Contains(tags[j])) continue;
				objTags[this].Add(tags[j]);
				if (tagObjs.ContainsKey(tags[j]) == false) tagObjs[tags[j]] = new List<Thing>();
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
			for (int i = 0; i < objTags[this].Count; i++)
				tagObjs[objTags[this][i]].Remove(this);
			objTags.Remove(this);
		}
		public bool HasTags(params string[] tags)
		{
			if (tags == null) { Debug.LogError(1, "The tag collection cannot be 'null'."); return false; }
			for (int i = 0; i < tags.Length; i++)
				if (objTags[this].Contains(tags[i]) == false)
					return false;
			return true;
		}

		public virtual void OnGameStart() { }
		public virtual void OnGameUpdate() { }
		public virtual void OnGameLateUpdate() { }
		public virtual void OnCameraDisplay(Camera camera) { }

		public virtual void OnWindowResize() { }
		public virtual void OnWindowClose() { }
		public virtual void OnWindowFocus() { }
		public virtual void OnWindowUnfocus() { }
		public virtual void OnWindowMaximize() { }
		public virtual void OnWindowMinimize() { }
		public virtual void OnWindowFullscreen() { }

		public virtual void OnMouseButtonDoubleClick(Mouse.Button button) { }
		public virtual void OnMouseButtonPress(Mouse.Button button) { }
		public virtual void OnMouseButtonRelease(Mouse.Button button) { }
		public virtual void OnMouseButtonHold(Mouse.Button button) { }
		public virtual void OnMouseWheelScroll(Mouse.Wheel wheel, double delta) { }
		public virtual void OnMouseCursorWindowEnter() { }
		public virtual void OnMouseCursorWindowLeave() { }

		public virtual void OnAssetsLoadStart() { }
		public virtual void OnAssetsLoadUpdate() { }
		public virtual void OnAssetsLoadEnd() { }
		public virtual void OnAssetsDataSlotSaveStart() { }
		public virtual void OnAssetsDataSlotSaveUpdate() { }
		public virtual void OnAssetsDataSlotSaveEnd() { }

		public virtual void OnKeyboardKeyPress(Keyboard.Key key) { }
		public virtual void OnKeyboardKeyHold(Keyboard.Key key) { }
		public virtual void OnKeyboardKeyRelease(Keyboard.Key key) { }
		public virtual void OnKeyboardTextInput(Keyboard.TextInput textInput) { }
		public virtual void OnKeyboardLanguageChange(string englishName, string nativeName, string languageCode) { }

		public virtual void OnMultiplayerServerStart() { }
		public virtual void OnMultiplayerServerStop() { }
		public virtual void OnMultiplayerClientConnect(string clientUniqueID) { }
		public virtual void OnMultiplayerClientDisconnect(string clientUniqueID) { }
		public virtual void OnMultiplayerClientTakenUniqueID(string oldClientUniqueID) { }
		public virtual void OnMultiplayerMessageReceived(Multiplayer.Message message) { }

		public virtual void OnAudioStart(Audio audio) { }
		public virtual void OnAudioPlay(Audio audio) { }
		public virtual void OnAudioPause(Audio audio) { }
		public virtual void OnAudioStop(Audio audio) { }
		public virtual void OnAudioEnd(Audio audio) { }
		public virtual void OnAudioLoop(Audio audio) { }

		public virtual void OnTimerCreateAndStart(Timer timer) { }
		public virtual void OnTimerUpdate(Timer timer) { }
		public virtual void OnTimerEnd(Timer timer) { }

		internal bool ErrorIfDestroyed()
		{
			if (IsDestroyed)
			{
				Debug.LogError(2, $"This {nameof(Thing)} is destroyed.");
				return true;
			}
			return false;
		}
		internal static void ErrorAlreadyHasUID(string uniqueID)
		{
			Debug.LogError(2, $"A {nameof(Thing)} with uniqueID '{uniqueID}' already exists.\n" +
				$"The newly created {nameof(Thing)} was destroyed because of this.");
		}
	}
}
