using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SMPL.Data;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace SMPL.Gear
{
	internal class JsonBinder : ISerializationBinder
	{
		internal static JsonBinder Instance = new();
		public List<Type> KnownTypes { get; set; } = new();
		public JsonBinder()
		{
			AddTypes(Assembly.GetEntryAssembly().GetTypes());
			AddTypes(Assembly.GetCallingAssembly().GetTypes());

			KnownTypes.Add(typeof(bool)); KnownTypes.Add(typeof(char)); KnownTypes.Add(typeof(string)); KnownTypes.Add(typeof(double));
			KnownTypes.Add(typeof(float)); KnownTypes.Add(typeof(decimal)); KnownTypes.Add(typeof(long)); KnownTypes.Add(typeof(ulong));
			KnownTypes.Add(typeof(byte)); KnownTypes.Add(typeof(sbyte)); KnownTypes.Add(typeof(short)); KnownTypes.Add(typeof(ushort));
			KnownTypes.Add(typeof(int)); KnownTypes.Add(typeof(uint));

			KnownTypes.Add(typeof(Point)); KnownTypes.Add(typeof(Color)); KnownTypes.Add(typeof(Corner)); KnownTypes.Add(typeof(Line));
			KnownTypes.Add(typeof(Quad)); KnownTypes.Add(typeof(Size)); KnownTypes.Add(typeof(Assets.DataSlot));

			var types = new List<Type>(KnownTypes);
			for (int i = 0; i < types.Count; i++)
			{
				var listType = Activator.CreateInstance(typeof(List<>).MakeGenericType(types[i])).GetType();
				var arrType = types[i].MakeArrayType();
				KnownTypes.Add(listType);
				KnownTypes.Add(arrType);
			}
			KnownTypes.Add(typeof(Dictionary<string, string>)); KnownTypes.Add(typeof(Dictionary<string, int>));
			KnownTypes.Add(typeof(Dictionary<string, bool>)); KnownTypes.Add(typeof(Dictionary<string, double>));
			KnownTypes.Add(typeof(Dictionary<bool, string>)); KnownTypes.Add(typeof(Dictionary<bool, int>));
			KnownTypes.Add(typeof(Dictionary<bool, bool>)); KnownTypes.Add(typeof(Dictionary<bool, double>));
			KnownTypes.Add(typeof(Dictionary<int, string>)); KnownTypes.Add(typeof(Dictionary<int, int>));
			KnownTypes.Add(typeof(Dictionary<int, bool>)); KnownTypes.Add(typeof(Dictionary<int, double>));
			KnownTypes.Add(typeof(Dictionary<double, string>)); KnownTypes.Add(typeof(Dictionary<double, int>));
			KnownTypes.Add(typeof(Dictionary<double, bool>)); KnownTypes.Add(typeof(Dictionary<double, double>));

			void AddTypes(Type[] types)
			{
				for (int i = 0; i < types.Length; i++)
					if (types[i] == typeof(Thing) || types[i].IsSubclassOf(typeof(Thing)))
						KnownTypes.Add(types[i]);
			}
		}
		public Type BindToType(string assemblyName, string typeName)
		{
			for (int i = 0; i < KnownTypes.Count; i++)
				if (KnownTypes[i].ToString() == typeName)
					return KnownTypes[i];
			return default;
		}
		public void BindToName(Type serializedType, out string assemblyName, out string typeName)
		{
			assemblyName = null;
			typeName = serializedType.ToString();
		}
	}
	public static class Extensions
	{
		private static readonly Random rng = new();
      public static void Shuffle<T>(this IList<T> list)
      {
         var n = list.Count;
         while (n > 1)
         {
            n--;
            var k = rng.Next(n + 1);
            var value = list[k];
            list[k] = list[n];
            list[n] = value;
         }
      }
      public static T Duplicate<T>(this T obj) => ToObject<T>(ToJSON(obj));
		/// <summary>
		/// Tries to convert a <paramref name="JSON"/> <see cref="string"/> into <typeparamref name="T"/> 
		/// <paramref name="instance"/> and returns it if successful. Otherwise returns 
		/// <paramref name="default"/>(<typeparamref name="T"/>).
		/// </summary>
		public static T ToObject<T>(this string JSON)
		{
			try
			{
				var settings = new JsonSerializerSettings
				{
					TypeNameHandling = TypeNameHandling.All,
					SerializationBinder = JsonBinder.Instance,
				};
				return JsonConvert.DeserializeObject<T>(JSON, settings);
			}
			catch (Exception) { return default; }
		}
		/// <summary>
		/// Tries to convert <paramref name="instance"/> into a <paramref name="JSON"/> <see cref="string"/> 
		/// and returns it if successful. Returns <paramref name="null"/> otherwise.‪‪<br></br><br></br> When <paramref name="instance"/> inherits <see cref="Thing"/>:<br></br>
		/// - Fields and properties (both <paramref name="public"/> and <paramref name="private"/>) require the attribute
		/// [<see cref="JsonProperty"/>] in order for them to be included in the <paramref name="JSON"/> <see cref="string"/>.<br></br>
		/// When <paramref name="instance"/> is anything else:<br></br>
		/// - All <paramref name="public"/> members are included.<br></br><br></br>
		/// Only the following <see cref="Type"/>s are allowed for security reasons
		/// (known <paramref name="JSON"/> deserialization code injection vulnerability):<br></br>
		/// - <see cref="bool"/>, <see cref="char"/>, <see cref="string"/>, <see cref="sbyte"/>, <see cref="byte"/>, <see cref="short"/>,
		/// <see cref="ushort"/>, <see cref="int"/>, <see cref="uint"/>, <see cref="long"/>, <see cref="ulong"/>, <see cref="float"/>,
		/// <see cref="double"/>, <see cref="decimal"/> (can be in a <see cref="List{T}"/> or <see cref="Array"/>)<br></br>
		/// - <see cref="Point"/>, <see cref="Color"/>, <see cref="Size"/>, <see cref="Line"/>, <see cref="Corner"/>, <see cref="Quad"/>,
		/// <see cref="Assets.DataSlot"/> (can be in a <see cref="List{T}"/> or <see cref="Array"/>)<br></br>
		/// - <see cref="Dictionary{TKey, TValue}"/> (<typeparamref name="TKey"/> = <see cref="bool"/>, <see cref="string"/>,
		/// <see cref="int"/>, <see cref="double"/>, <typeparamref name="TValue"/> = <see cref="bool"/>, <see cref="string"/>,
		/// <see cref="int"/>, <see cref="double"/>), <paramref name="JSON"/> <see cref="string"/>s can be used if more
		/// <see cref="Type"/>s are needed. <br></br>
		/// - As well as all of the <paramref name="classes"/> in <see cref="SMPL"/> and the <see cref="Assembly"/> calling this
		/// function (can be in a <see cref="List{T}"/> or <see cref="Array"/>).<br></br><br></br>
		/// <see cref="FromJSON{T}(string)"/> will return <paramref name="null"/> if the <paramref name="instance"/>
		/// or any of its members is a <see cref="Type"/> that is not listed above.
		/// </summary>
		public static string ToJSON(this object instance)
		{
			var settings = new JsonSerializerSettings
			{
				TypeNameHandling = TypeNameHandling.All,
				SerializationBinder = JsonBinder.Instance,
			};
			return JsonConvert.SerializeObject(instance, settings);
		}
	}
}
