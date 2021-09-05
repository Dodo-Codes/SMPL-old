using Newtonsoft.Json;
using System;
using System.Drawing;
using System.Text;

namespace SMPL.Data
{
	public static class Text
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
		/// Encrypts a <paramref name="text"/> with a <paramref name="key"/>. Can be <paramref name="performedTwice"/>.
		/// The encryption is 33% longer than the <paramref name="text"/>.<br></br> The
		/// <paramref name="text"/> can be decrypred later with <see cref="Decrypt"/>.
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
	}
}
