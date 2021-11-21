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
		// no need for [JsonProperty] cuz
		// this is saved separately as chunk-data dict<point json, point[] json> and reconstructed upon load
		// since the json is confused with the point struct and casts it ToString() rather than to a json format :D
		public Dictionary<Point, Point[]> Data { get; } = new();
		[JsonProperty]
		public Dictionary<string, string> SignsJSON { get; } = new();

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
