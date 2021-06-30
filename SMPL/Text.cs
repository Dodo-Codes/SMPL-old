using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;

namespace SMPL
{
	public abstract class Text
	{
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
		/// <paramref name="data"/> and returns it if successful. Otherwise returns 
		/// <paramref name="default"/>(<typeparamref name="T"/>).<br></br>
		/// A <paramref name="JSON"/> <see cref="string"/> can be created with <see cref="ToJSON(object)"/>.
		/// </summary>
		public static object FromJSON(string JSON)
		{
			try
			{
				return JsonConvert.DeserializeObject<object>(JSON);
			}
			catch (Exception)
			{
				return default;
			}
		}
		/// <summary>
		/// Tries to convert <typeparamref name="T"/> <paramref name="data"/> into a <paramref name="JSON"/> <see cref="string"/> 
		/// and returns it if successful. Returns <paramref name="null"/> otherwise.<br></br>
		/// The <paramref name="JSON"/> <see cref="string"/> can be converted back to 
		/// <typeparamref name="T"/> <paramref name="data"/> later with 
		/// <see cref="FromJSON"/>.‪‪
		/// </summary>
		public static string ToJSON(object data)
		{
			return JsonConvert.SerializeObject(data);
		}
		/// <summary>
		/// Encrypts a <paramref name="text"/> with a <paramref name="key"/>. Can be <paramref name="performedTwice"/>.‪‪ 
		/// The <paramref name="text"/> can be decrypred later with <see cref="Decrypt"/>.
		/// </summary>
		public static string Encrypt(string text, char key, bool performedTwice = false)
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
		/// <summary>
		/// Decrypts an <paramref name="encryptedText"/> with a <paramref name="key"/> that could have been <paramref name="performedTwice"/> with <see cref="Encrypt"/>.‪‪
		/// </summary>
		public static string Decrypt(string encryptedText, char key, bool performedTwice = false)
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
		/// <summary>
		/// Creates or overwrites a file at <paramref name="filePath"/> and fills it with <paramref name="text"/>. Any <paramref name="text"/> can be retrieved from a file with <see cref="Load"/>.<br></br><br></br>
		/// This is a slow operation - do not call frequently.
		/// </summary>
		public static void Save(string text, string filePath = "folder/file.extension")
		{
			var full = Game.CreateDirectoryForFile(filePath);

			try
			{
				File.WriteAllText(full, text);
			}
			catch (Exception)
			{
				Debug.LogError(1, $"Could not save file '{full}'.");
				return;
			}
		}
		/// <summary>
		/// Reads all the text from the file at <paramref name="filePath"/> and returns it as a <see cref="string"/> if successful. Returns <paramref name="null"/> otherwise. A text can be saved to a file with <see cref="Save"/>.<br></br><br></br>
		/// This is a slow operation - do not call frequently.
		/// </summary>
		public static string Load(string filePath = "folder/file.extension")
		{
			filePath = filePath.Replace('/', '\\');
			var full = $"{Game.Directory}\\{filePath}";

			if (Directory.Exists(full) == false)
			{
				Console.Log($"Could not load file '{full}'. Directory/file not found.");
				return default;
			}
			try
			{
				return File.ReadAllText(full);
			}
			catch (Exception)
			{
				Console.Log($"Could not load file '{full}'.");
				return default;
			}
		}
	}
}
