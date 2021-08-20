using System;
using System.Runtime.InteropServices;

namespace SMPL
{
	public static class Console
	{
		#region smol brain
		private static void Form1_Load(object sender, EventArgs e) => AllocConsole();
		[DllImport("kernel32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool AllocConsole();
		private enum StdHandle : int
		{
			STD_INPUT_HANDLE = -10,
			STD_OUTPUT_HANDLE = -11,
			STD_ERROR_HANDLE = -12,
		}
		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern IntPtr GetStdHandle(int nStdHandle); //returns Handle
		public enum ConsoleMode : uint
		{
			ENABLE_ECHO_INPUT = 0x0004,
			ENABLE_EXTENDED_FLAGS = 0x0080,
			ENABLE_INSERT_MODE = 0x0020,
			ENABLE_LINE_INPUT = 0x0002,
			ENABLE_MOUSE_INPUT = 0x0010,
			ENABLE_PROCESSED_INPUT = 0x0001,
			ENABLE_QUICK_EDIT_MODE = 0x0040,
			ENABLE_WINDOW_INPUT = 0x0008,
			ENABLE_VIRTUAL_TERMINAL_INPUT = 0x0200,

			//screen buffer handle
			ENABLE_PROCESSED_OUTPUT = 0x0001,
			ENABLE_WRAP_AT_EOL_OUTPUT = 0x0002,
			ENABLE_VIRTUAL_TERMINAL_PROCESSING = 0x0004,
			DISABLE_NEWLINE_AUTO_RETURN = 0x0008,
			ENABLE_LVB_GRID_WORLDWIDE = 0x0010
		}
		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);
		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);
		#endregion

		internal static bool shown;

		public static string Title
		{
#pragma warning disable CA1416
			get { return System.Console.Title; }
#pragma warning restore CA1416
			set
			{
				Show();
				System.Console.Title = $"{value}";
			}
		}
		public static string Input
		{
			get
			{
				Show();
				SelectionEnable(true);
				var result = System.Console.ReadLine();
				SelectionEnable(false);

				return result;
			}
		}

		public static void Log(object message, bool newLine = true)
		{
			Show();
			var strNewLine = newLine ? "\n" : "";
			System.Console.Write($"{message}{strNewLine}");
		}
		public static void Clear()
		{
			Show();
			System.Console.Clear();
		}

		internal static void Show()
		{
			if (shown) return;

			AllocConsole();

			SelectionEnable(false);
			System.Console.Title = $"{Window.Title} Console";
			shown = true;
		}
		internal static void SelectionEnable(bool enabled)
		{
			IntPtr consoleHandle = GetStdHandle((int)StdHandle.STD_INPUT_HANDLE);
			UInt32 consoleMode;

			GetConsoleMode(consoleHandle, out consoleMode);
			if (enabled) consoleMode |= ((uint)ConsoleMode.ENABLE_QUICK_EDIT_MODE);
			else consoleMode &= ~((uint)ConsoleMode.ENABLE_QUICK_EDIT_MODE);

			consoleMode |= ((uint)ConsoleMode.ENABLE_EXTENDED_FLAGS);

			SetConsoleMode(consoleHandle, consoleMode);
		}
	}
}
