using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SFML.System;
using SMPL.Components;
using SMPL.Gear;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SMPL.Data
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

	public static class Text
	{
		private static readonly SFML.Graphics.Text text = new();

		public static bool IsNumber(string text)
		{
			return double.TryParse(text, out _);
		}
		public static bool IsLetters(string text)
		{
			for (int i = 0; i < text.Length; i++)
			{
				var isLetter = (text[i] >= 'A' && text[i] <= 'Z') || (text[i] >= 'a' && text[i] <= 'z');
				if (isLetter == false) return false;
			}
			return true;
		}
		public static string Align(string text, int characterSpaces)
		{
			return string.Format("{0," + characterSpaces + "}", text);
		}
		public static void ClipboardCopy(string text)
		{
			System.Windows.Forms.Clipboard.SetText(text);
		}
		public static string ClipboardPaste()
		{
			var result = System.Windows.Forms.Clipboard.GetText();
			return result == string.Empty ? null : result;
		}
		public static string Repeat(string text, uint times)
		{
			var result = "";
			var intTimes = (int)Number.Round(times, 0);
			for (int i = 0; i < intTimes; i++)
			{
				result = $"{result}{text}";
			}
			return result;
		}
		/// <summary>
		/// Tries to convert a <paramref name="JSON"/> <see cref="string"/> into <typeparamref name="T"/> 
		/// <paramref name="instance"/> and returns it if successful. Otherwise returns 
		/// <paramref name="default"/>(<typeparamref name="T"/>).<br></br>
		/// A <paramref name="JSON"/> <see cref="string"/> can be created with <see cref="ToJSON(object)"/>.
		/// </summary>
		public static T FromJSON<T>(string JSON)
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
		/// and returns it if successful. Returns <paramref name="null"/> otherwise.<br></br> The <paramref name="JSON"/>
		/// <see cref="string"/> can be converted back to <paramref name="instance"/> later with
		/// <see cref="FromJSON"/>.‪‪<br></br><br></br> When <paramref name="instance"/> inherits <see cref="Thing"/>:<br></br>
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
		public static string ToJSON(object instance)
		{
			var settings = new JsonSerializerSettings
			{
				TypeNameHandling = TypeNameHandling.All,
				SerializationBinder = JsonBinder.Instance,
			};
			return JsonConvert.SerializeObject(instance, settings);
		}
		public static string Compress(string text)
		{
			var buffer = Encoding.UTF8.GetBytes(text);
			var memoryStream = new MemoryStream();
			using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Compress, true))
				gZipStream.Write(buffer, 0, buffer.Length);

			memoryStream.Position = 0;

			var compressedData = new byte[memoryStream.Length];
			memoryStream.Read(compressedData, 0, compressedData.Length);

			var gZipBuffer = new byte[compressedData.Length + 4];
			Buffer.BlockCopy(compressedData, 0, gZipBuffer, 4, compressedData.Length);
			Buffer.BlockCopy(BitConverter.GetBytes(buffer.Length), 0, gZipBuffer, 0, 4);
			return Convert.ToBase64String(gZipBuffer);
		}
		public static string Decompress(string compressedText)
		{
			var gZipBuffer = Convert.FromBase64String(compressedText);
			using var memoryStream = new MemoryStream();
			var dataLength = BitConverter.ToInt32(gZipBuffer, 0);
			memoryStream.Write(gZipBuffer, 4, gZipBuffer.Length - 4);

			var buffer = new byte[dataLength];

			memoryStream.Position = 0;
			using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
				gZipStream.Read(buffer, 0, buffer.Length);

			return Encoding.UTF8.GetString(buffer);
		}

		public static void Display(Camera camera, object text, string fontPath, Point offset = default)
		{
			if (Window.DrawNotAllowed()) return;
			if (fontPath == null || Assets.fonts.ContainsKey(fontPath) == false)
			{
				Assets.NotLoadedError(1, Assets.Type.Font, fontPath);
				return;
			}

			Text.text.LineSpacing = 0.8f;
			Text.text.Scale = Size.From(new Size(1 / Window.PixelSize.W, 1 / Window.PixelSize.H));
			Text.text.Font = Assets.fonts[fontPath];
			Text.text.DisplayedString = text.ToString();
			Text.text.Position = Point.From(camera.Position + offset);
			Text.text.OutlineThickness = 2;
			camera.rendTexture.Draw(Text.text);
			Performance.DrawCallsPerFrame++;
		}

		/*private static string Encrypt(string text, char key, bool performedTwice = false)
		{
			var result = text;
			var times = performedTwice ? 2 : 1;
			for (int i = 0; i < times; i++)
			{
				result = Encrypt(result, key);
			}
			return result;
		}
		private static string Encrypt(string text, char key)
		{
			var amplifier = Convert.ToByte(key);
			byte[] data = Encoding.UTF8.GetBytes(text);
			for (int i = 0; i < data.Length; i++)
			{
				data[i] = (byte)(data[i] ^ amplifier);
			}
			return Convert.ToBase64String(data);
		}
		private static string Decrypt(string encryptedText, char key, bool performedTwice = false)
		{
			var result = encryptedText;
			var times = performedTwice ? 2 : 1;
			for (int i = 0; i < times; i++)
			{
				result = Decrypt(result, key);
			}
			return result;
		}
		private static string Decrypt(string encryptedText, char key)
		{
			var amplifier = Convert.ToByte(key);
			byte[] data = Convert.FromBase64String(encryptedText);
			for (int i = 0; i < data.Length; i++)
			{
				data[i] = (byte)(data[i] ^ amplifier);
			}
			return Encoding.UTF8.GetString(data);
		}
		*/
	}
}
