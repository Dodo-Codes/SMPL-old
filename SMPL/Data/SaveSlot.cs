using Newtonsoft.Json;
using SMPL.Components;
using SMPL.Gear;
using System.Collections.Generic;
using System.IO;

namespace SMPL.Data
{
	public struct SaveSlot
	{
		private static Dictionary<string, object> values = new();
		[JsonProperty]
		private Dictionary<string, object> slotValues;

		public const string DIRECTORY = "savedata";

		public Area[] Areas { get; set; }
		public Audio[] Audios { get; set; }
		public Camera[] Cameras { get; set; }
		public Effects[] Effects { get; set; }
		public Family[] Families { get; set; }
		public Hitbox[] Hitboxes { get; set; }
		public Sprite[] Sprites { get; set; }
		public Textbox[] Textboxes { get; set; }
		public Timer[] Timers { get; set; }

		public void Save(string name)
		{
			if (name.Contains('\\'))
			{
				Debug.LogError(1, $"Cannot create {nameof(SaveSlot)} with an invalid name '{name}'.");
				return;
			}
			Directory.CreateDirectory(DIRECTORY);
			var str = JsonConvert.SerializeObject(this);
			str = Text.Encrypt(str, 'J', true);
			System.IO.File.WriteAllText($"{DIRECTORY}\\{name}.save", str);
		}
		public static void Load(string name)
		{
			var str = "";
			try { str = System.IO.File.ReadAllText($"{DIRECTORY}\\{name}.save"); }
			catch (System.Exception)
			{
				Debug.LogError(1, $"No {nameof(SaveSlot)} with name '{name}' was found.");
				return;
			}
			str = Text.Decrypt(str, 'J', true);
			var slot = JsonConvert.DeserializeObject<SaveSlot>(str);

			if (slot.slotValues == null || slot.slotValues.Count == 0) return;
			foreach (var kvp in slot.slotValues)
				values[kvp.Key] = kvp.Value;
		}
		public void SetValue(string key, object value)
		{
			if (value is Component)
			{
				Debug.LogError(1, $"{nameof(SaveSlot)} values should not be {nameof(Component)}s, use the {nameof(SaveSlot)}'s " +
					$"properties for that.\n" +
					$"This is because {nameof(Component)}s can be loaded but retrieved later.\n" +
					$"Also their order of saving/loading matters because of how they reference one another.");
				return;
			}
			if (slotValues == null) slotValues = new();
			slotValues[key] = value;
		}
		public static object GetValue(string key)
		{
			if (values.ContainsKey(key) == false)
			{
				Debug.LogError(1, $"Could not retrieve value because the key '{key}' was not found.");
				return default;
			}
			return values[key];
		}
	}
}
