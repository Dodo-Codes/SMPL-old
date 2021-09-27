using Newtonsoft.Json;
using SMPL.Components;
using SMPL.Gear;
using SMPL.Prefabs;
using System.IO;

namespace SMPL.Data
{
	public struct SaveSlot
	{
		private const int VALUE_LIMIT = 1000;
		private static readonly string[] values = new string[VALUE_LIMIT + 1];

		public const string DIRECTORY = "savedata";

		public string[] Values { get; set; }
		public Area[] Areas { get; set; }
		public Audio[] Audios { get; set; }
		public Camera[] Cameras { get; set; }
		public Effects[] Effects { get; set; }
		public Family[] Families { get; set; }
		public Hitbox[] Hitboxes { get; set; }
		public Sprite[] Sprites { get; set; }
		public Textbox[] Textboxes { get; set; }
		public Timer[] Timers { get; set; }

		public Cloth[] Clothes { get; set; }
		public Ropes[] Ropes { get; set; }
		public SegmentedLine[] SegmentedLines { get; set; }

		public Probability.Table[] ProbabilityTables { get; set; }

		public void Save(string name)
		{
			if (name.Contains('\\'))
			{
				Debug.LogError(1, $"Cannot create {nameof(SaveSlot)} with an invalid name '{name}'.");
				return;
			}
			Directory.CreateDirectory(DIRECTORY);
			var str = JsonConvert.SerializeObject(this);
			str = Text.Encrypt(str, 'H', true);
			System.IO.File.WriteAllText($"{DIRECTORY}\\{name}.save", str);
		}
		public static void Load(string name)
		{
			var str = "";
			try
			{
				str = System.IO.File.ReadAllText($"{DIRECTORY}\\{name}.save");
				str = Text.Decrypt(str, 'H', true);
				var slot = JsonConvert.DeserializeObject<SaveSlot>(str);

				if (slot.Values == null || slot.Values.Length == 0) return;
				for (int i = 0; i < slot.Values.Length; i++)
					values[i] = slot.Values[i];
			}
			catch (System.Exception)
			{
				Debug.LogError(1, $"Error loading {nameof(SaveSlot)} with name '{name}'.");
				return;
			}
		}
		public static string GetValue(int index)
		{
			if (Number.IsBetween(index, new Number.Range(0, VALUE_LIMIT), true, false) == false)
			{
				Debug.LogError(1, $"The index has to be between 0 and {VALUE_LIMIT}. Only those values are loaded.");
				return default;
			}
			var result = default(string);
			if (Statics.TryCast(values[index], out result) == false)
				Debug.LogError(1, $"Could not retrieve the value from index '{index}'. Perhaps it is the wrong type?");
			return result;
		}
	}
}
