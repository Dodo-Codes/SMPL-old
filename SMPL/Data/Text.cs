using Newtonsoft.Json;
using SFML.System;
using SMPL.Components;
using SMPL.Gear;
using System;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace SMPL.Data
{
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
		/// <paramref name="data"/> and returns it if successful. Otherwise returns 
		/// <paramref name="default"/>(<typeparamref name="T"/>).<br></br>
		/// A <paramref name="JSON"/> <see cref="string"/> can be created with <see cref="ToJSON(object)"/>.
		/// </summary>
		public static T FromJSON<T>(string JSON)
		{
			try
			{
				return JsonConvert.DeserializeObject<T>(JSON);
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
