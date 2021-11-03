using SMPL.Data;
using System.Collections.Generic;
using Newtonsoft.Json;
using SMPL.Gear;

namespace RPG1bit
{
	public class Chunk : Thing
	{
		[JsonProperty]
		public Point Center { get; set; }
		public Dictionary<Point, Point[]> Data { get; set; } = new();

		public Chunk(string uniqueID) : base(uniqueID)
		{
			AddTags(nameof(Chunk));
		}
		public Dictionary<string, string> GetSavableData()
		{
			var result = new Dictionary<string, string>();
			foreach (var kvp in Data)
				result[Text.ToJSON(kvp.Key)] = Text.ToJSON(kvp.Value);
			return result;
		}
	}
}
