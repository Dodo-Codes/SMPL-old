using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace SMPL.Gear
{
	public static class Keyboard
	{
		public struct TextInput
		{
			public enum Type { Text, Backspace, Enter, Tab }

			public string Value { get; set; }
			public Type CurrentType { get; set; }
		}

		public enum Key
		{
			/// <summary>
			/// Unhandled key
			/// </summary>
			Unknown = -1,
			A = 0, B = 1, C = 2, D = 3, E = 4, F = 5, G = 6, H = 7, I = 8, J = 9, K = 10, L = 11, M = 12, N = 13, O = 14,
			P = 15, Q = 16, R = 17, S = 18, T = 19, U = 20, V = 21, W = 22, X = 23, Y = 24, Z = 25,
			_0 = 26, _1 = 27, _2 = 28, _3 = 29, _4 = 30, _5 = 31, _6 = 32, _7 = 33, _8 = 34, _9 = 35,
			Escape = 36, LeftControl = 37, LeftShift = 38, LeftAlt = 39,
			/// <summary>
			/// The left OS specific key.<br></br><br></br>
			/// Windows, Linux: <typeparamref name="window"/><br></br>
			/// MacOS X: <typeparamref name="apple"/><br></br>
			/// etc.
			/// </summary> 
			LeftSystem = 40,
			RightControl = 41, RightShift = 42, RightAlt = 43,
			/// <summary>
			/// The right OS specific key.<br></br><br></br>
			/// Windows, Linux: <typeparamref name="window"/><br></br>
			/// MacOS X: <typeparamref name="apple"/><br></br>
			/// etc.
			/// </summary> 
			RightSystem = 44, Menu = 45, LeftBracket = 46, RightBracket = 47, Semicolon = 48, Comma = 49, Period = 50,
			Dot = 50, Quote = 51, Slash = 52, Backslash = 53, Tilde = 54, Equal = 55, Dash = 56, Space = 57, Enter = 58,
			Return = 58, Backspace = 59, Tab = 60, PageUp = 61, PageDown = 62, End = 63, Home = 64, Insert = 65, Delete = 66,
			/// <summary>
			/// The Num[+] key
			/// </summary>
			Add = 67,
			/// <summary>
			/// The Num[-] key
			/// </summary>
			Minus = 68,
			/// <summary>
			/// The Num[*] key
			/// </summary>
			Multiply = 69,
			/// <summary>
			/// The Num[/] key
			/// </summary>
			Divide = 70,
			LeftArrow = 71, RightArrow = 72, UpArrow = 73, DownArrow = 74, Numpad0 = 75, Numpad1 = 76, Numpad2 = 77,
			Numpad3 = 78, Numpad4 = 79, Numpad5 = 80, Numpad6 = 81, Numpad7 = 82, Numpad8 = 83, Numpad9 = 84, F1 = 85,
			F2 = 86, F3 = 87, F4 = 88, F5 = 89, F6 = 90, F7 = 91, F8 = 92, F9 = 93, F10 = 94, F11 = 95, F12 = 96, F13 = 97,
			F14 = 98, F15 = 99, Pause = 100,
		}
		public static Key[] KeysPressed { get { return keysHeld.ToArray(); } }
		internal static List<Key> keysHeld = new();
		public static bool KeyIsPressed(Key key) => keysHeld.Contains(key);

		public static class Event
		{
			public static class Subscribe
			{
				public static void KeyPress(string thingUID, uint order = uint.MaxValue) =>
					Events.Enable(Events.Type.KeyPress, thingUID, order);
				public static void KeyHold(string thingUID, uint order = uint.MaxValue) =>
					Events.Enable(Events.Type.KeyHold, thingUID, order);
				public static void KeyRelease(string thingUID, uint order = uint.MaxValue) =>
					Events.Enable(Events.Type.KeyRelease, thingUID, order);
				public static void TextInput(string thingUID, uint order = uint.MaxValue) =>
					Events.Enable(Events.Type.TextInput, thingUID, order);
				public static void LanguageChange(string thingUID, uint order = uint.MaxValue) =>
					Events.Enable(Events.Type.LanguageChange, thingUID, order);
			}
			public static class Unsubscribe
			{
				public static void KeyPress(string thingUID) =>
					Events.Disable(Events.Type.KeyPress, thingUID);
				public static void KeyHold(string thingUID) =>
					Events.Disable(Events.Type.KeyHold, thingUID);
				public static void KeyRelease(string thingUID) =>
					Events.Disable(Events.Type.KeyRelease, thingUID);
				public static void TextInput(string thingUID) =>
					Events.Disable(Events.Type.TextInput, thingUID);
				public static void LanguageChange(string thingUID) =>
					Events.Disable(Events.Type.LanguageChange, thingUID);
			}
		}

		internal static void Update()
		{
			for (int i = 0; i < keysHeld.Count; i++)
				Events.Notify(Events.Type.KeyHold, new Events.EventArgs() { Key = keysHeld[i] });
		}
		internal static void Initialize()
		{
			Window.window.KeyPressed += new EventHandler<SFML.Window.KeyEventArgs>(OnKeyPress_);
			Window.window.KeyReleased += new EventHandler<SFML.Window.KeyEventArgs>(OnKeyRelease_);
			Window.window.SetKeyRepeatEnabled(false);
			Window.form.KeyPress += new KeyPressEventHandler(OnTextInput_);
			Window.form.InputLanguageChanged += new InputLanguageChangedEventHandler(OnLanguageChange_);
		}

		internal static void CancelInput()
		{
			for (int i = 0; i < keysHeld.Count; i++)
				Events.Notify(Events.Type.KeyRelease, new Events.EventArgs() { Key = keysHeld[i] });
			keysHeld.Clear();
		}
		internal static void OnKeyPress_(object sender, EventArgs e)
		{
			var keyArgs = (SFML.Window.KeyEventArgs)e;
			var key = (Key)keyArgs.Code;
			keysHeld.Add(key);
			Events.Notify(Events.Type.KeyPress, new Events.EventArgs() { Key = key });
		}
		internal static void OnKeyRelease_(object sender, EventArgs e)
		{
			var keyArgs = (SFML.Window.KeyEventArgs)e;
			var key = (Key)keyArgs.Code;
			keysHeld.Remove(key);
			Events.Notify(Events.Type.KeyRelease, new Events.EventArgs() { Key = key });
		}
		internal static void OnTextInput_(object sender, EventArgs e)
		{
			var keyArgs = (KeyPressEventArgs)e;
			var keyStr = keyArgs.KeyChar.ToString();
			keyStr = keyStr.Replace('\r', '\n');
			var isBackSpace = keyStr == "\b";
			var isEnter = Environment.NewLine.Contains(keyStr);
			var isTab = keyStr == "\t";
			if (keyStr == "\b") keyStr = "";
			var textInput = new TextInput() { CurrentType = TextInput.Type.Text, Value = keyStr };

			if (isBackSpace) textInput.CurrentType = TextInput.Type.Backspace;
			else if (isEnter) textInput.CurrentType = TextInput.Type.Enter;
			else if (isTab) textInput.CurrentType = TextInput.Type.Tab;
			Events.Notify(Events.Type.TextInput, new() { TextInput = textInput });
		}
		internal static void OnLanguageChange_(object sender, EventArgs e)
		{
			var langArgs = (InputLanguageChangedEventArgs)e;
			var culture = langArgs.InputLanguage.Culture;
			Events.Notify(Events.Type.LanguageChange, new()
			{ String = new string[] { culture.EnglishName, culture.NativeName, culture.Name } });
		}
	}
}
