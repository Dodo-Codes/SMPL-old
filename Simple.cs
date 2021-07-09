using SFML.Graphics;
using SFML.Audio;
using SFML.System;
using SFML.Window;
using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using Newtonsoft.Json;
using System.Drawing.Design;
using System.Collections.Generic;
using TcpClient = NetCoreServer.TcpClient;
using System.Net.Sockets;
using System.Net;
using System.Linq;
using System.Text;
using System.IO;
using NetCoreServer;

/// <summary>
/// A <see cref="Simple"/> way of creating Windows games.<br></br><br></br>
/// A brief introduction of how that's done:<br></br>
/// There are two <see cref="Simple"/> structures of something happening:<br></br>
/// 'When' to happen: <see cref="OverrideMethodExample"/><br></br>
/// 'What' to happen: <see cref="Action"/><br></br>
/// Those <see cref="Simple"/> structures use the following data types:<br></br>
/// <see cref="object"/>, <see cref="object"/>[], <see cref="bool"/>, 
/// <see cref="double"/>, <see cref="double"/>[], <see cref="string"/>, 
/// <see cref="string"/>[] and <paramref name="enum"/> <br></br><br></br>
/// Overriding a method is like subscribing for an event.<br></br>
/// Each time this event happens - the code inside the overridden method executes.<br></br>
/// Some information about the event is carried inside the method's parameters.<br></br>
/// The code inside the method may contain <see cref="Action"/> methods.<br></br>
/// Further information and examples are provided when hovering over methods and classes.<br></br><br></br>
/// Code example:<br></br>
/// <paramref name="class"/> <typeparamref name="MyGame"/> : <see cref="Simple"/><br></br>
/// {<br></br>
/// <paramref name="    "/><paramref name="static"/> <paramref name="void"/> <typeparamref name="Main"/>() => 
/// <see cref="Game.Running.DoStart"/>; // starting the game<br></br><br></br>
/// <paramref name="    "/><paramref name="public"/> <paramref name="override"/> <paramref name="void"/> 
/// <typeparamref name="OnGameProgressChanged"/>(<see cref="Progress"/> <paramref name="progress"/>, 
/// <see cref="double"/> <paramref name="runtimeTickCount"/>, <see cref="double"/> <paramref name="totalTickCount"/>)<br></br>
/// <paramref name="    "/>{<br></br>
/// <paramref name="        "/><paramref name="if"/> (<paramref name="progress"/> == <see cref="Progress.Start"/>)<br></br>
/// <paramref name="        "/>{<br></br>
/// <paramref name="            "/><see cref="Console.Log"/>.<typeparamref name="DoMessage"/>("Hello World!"); 
/// // print "Hello World!" at start<br></br>
/// <paramref name="        "/>}<br></br>
/// <paramref name="        "/><paramref name="else"/> <paramref name="if"/> (<paramref name="progress"/> == 
/// <see cref="Progress.Updating"/>)<br></br>
/// <paramref name="        "/>{<br></br>
/// <paramref name="            "/><see cref="Console.Log"/>.<typeparamref name="DoMessage"/>(<paramref name=
/// "runtimeTickCount"/>); // keep printing the amount of executions of this if statement<br></br>
/// <paramref name="        "/>}<br></br>
/// <paramref name="    "/>}<br></br>
/// }
/// </summary>
public class Simple
{
	///<summary>
	///text, <paramref name="param"/>, <see cref="char"/>, <typeparamref name="Type"/>
	///</summary>
	private static void SummaryExample() { }
	#region Enums
	public enum Direction
	{
		Up, Down, Left, Right, UpRight, UpLeft, DownRight, DownLeft
	}
	public enum Motion
	{
		PerTick, PerSecond
	}
	public enum Color
	{
		White, Black,
		LightGray, Gray, DarkGray,
		LightRed, Red, DarkRed,
		LightGreen, Green, DarkGreen,
		LightBlue, Blue, DarkBlue,
		LightYellow, Yellow, DarkYellow,
		LightMagenta, Magenta, DarkMagenta,
		LightCyan, Cyan, DarkCyan,
		LightOrange, Orange, DarkOrange
	}
	public enum Progress
	{
		Start, Updating, End
	}
	public enum WindowState
	{
		/// <summary>
		/// Same as <see cref="WindowState.Normal"/>
		/// </summary>
		Unmaximized = 0,
		/// <summary>
		/// Same as <see cref="WindowState.Unmaximized"/>
		/// </summary>
		Normal = 0,
		Minimized = 1, Maximized = 2
	}
	public enum RAMUsage
	{
		Percent, GB
	}
	public enum KeyboardKey
	{
		/// <summary>
		/// Unhandled key
		/// </summary>
		Unknown = -1,
		A = 0, B = 1, C = 2, D = 3, E = 4, F = 5, G = 6, H = 7, I = 8, J = 9, K = 10, L = 11, M = 12, N = 13, O = 14,
		P = 15, Q = 16, R = 17, S = 18, T = 19, U = 20, V = 21, W = 22, X = 23, Y = 24, Z = 25,
		Num0 = 26, Num1 = 27, Num2 = 28, Num3 = 29, Num4 = 30, Num5 = 31, Num6 = 32, Num7 = 33, Num8 = 34, Num9 = 35,
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
	public enum KeyboardAction
	{
		Pressed, Released
	}
	public enum MouseButton
	{
		Unknown = -1, Left = 0, Right = 1, Middle = 2, ExtraButton1 = 3, ExtraButton2 = 4
	}
	public enum MouseWheel
	{
		Vertical, Horizontal
	}
	public enum MouseAction
	{
		CursorMoved, CursorHoveredWindow, CursorUnhoveredWindow
	}
	public enum MouseButtonAction
	{
		ButtonClicked, ButtonReleased, ButtonDoubleClicked
	}
	public enum PopUpIcon
	{
		None, Info, Error, Warning
	}
	public enum WindowAction
	{
		Focused, Unfocused, Resized, Closed
	}
	public enum AssetType
	{
		Texture, Font, Sound, Music
	}
	public enum MessageReceiver
	{
		Server, AllClients, ServerAndAllClients
	}
	public enum NumberTimeConvertion
	{
		MillisecondsToSeconds,
		SecondsToMilliseconds, SecondsToMinutes, SecondsToHours,
		MinutesToMilliseconds, MinutesToSeconds, MinutesToHours, MinutesToDays,
		HoursToSeconds, HoursToMinutes, HoursToDays, HoursToWeeks,
		DaysToMinutes, DaysToHours, DaysToWeeks,
		WeeksToHours, WeeksToDays
	}
	public enum NumberLimitation
	{
		ClosestBound, Overflow
	}
	public enum NumberRoundToward
	{
		Closest, Up, Down
	}
	public enum NumberRoundWhen5
	{
		TowardEven, AwayFromZero, TowardZero, TowardNegativeInfinity, TowardPositiveInfinity
	}
	public enum TextStyle
	{
		Regular, Bold, Italic, Strikethrough, Underline
	}

	private enum UnitEffect
	{
		TintRed, TintGreen, TintBlue, Opacity,
		Gamma, Desaturation, Inversion, Contrast, Brightness,
		Outline, OutlineOffset, OutlineRed, OutlineGreen, OutlineBlue,
		Fill, FillRed, FillGreen, FillBlue,
		Blink, BlinkSpeed,
		Blur, BlurStrengthX, BlurStrengthY,
		Earthquake, EarthquakeX, EarthquakeY,
		Stretch, StretchX, StretchY, StretchSpeedX, StretchSpeedY,
		Water, WaterStrengthX, WaterStrengthY, WaterSpeedX, WaterSpeedY,
		Edge, EdgeThreshold, EdgeThickness, EdgeRed, EdgeGreen, EdgeBlue,
		Pixelate, PixelateThreshold,
		GridX, GridY, GridCellWidth, GridCellHeight, GridCellSpacingX, GridCellSpacingY,
		GridRedX, GridRedY, GridGreenX, GridGreenY, GridBlueX, GridBlueY,
		WindX, WindY, WindSpeedX, WindSpeedY,
		VibrateX, VibrateY,
		WaveSinX, WaveSinY, WaveCosX, WaveCosY, WaveSinSpeedX, WaveSinSpeedY, WaveCosSpeedX, WaveCosSpeedY,
		MaskRed, MaskGreen, MaskBlue,
	}
	private enum UnitNumber
	{
		Depth, LocalAngle, GlobalAngle, LocalScaleWidth, GlobalScaleWidth, LocalScaleHeight, GlobalScaleHeight, LocalX, LocalY,
		GlobalX, GlobalY, OriginPercentX, OriginPercentY, ParallaxX, ParallaxY, ParallaxScale, ParallaxAngle,
		DisplayPercentX, DisplayPercentY, DisplayPercentWidth, DisplayPercentHeight, RepeatAmountX, RepeatAmountY,
		TextStyle, TextOutlineSize, TextOutlineRed, TextOutlineGreen, TextOutlineBlue, TextOutlineOpacity,
		TextSpacingLine, TextSpacingSymbol, TextCharacterSize
	}
	private enum UnitText
	{
		ID, ParentID, FontPath, Text, TexturePath, MaskTargetID
	}
	private enum UnitFact
	{
		Exists, IsHidden, IsDisabled, HasSmoothTexture, MasksIn, IsRepeated
	}
	private enum UnitTexts
	{
		AllObjectIDs, ChildrenIDs, MaskIDs
	}
	private enum CameraNumber
	{
		X, Y, Angle, Zoom
	}
	#endregion
	#region Run instance
	private static Simple game;
	private static Form form;
	private static RenderWindow window;
	private static Clock time, tickDeltaTime, frameDeltaTime;
	private static List<string> pickedIDs;
	private static string directory, connectToServerInfo, multiplayerMsgSep, multiplayerMsgComponentSep;
	private static PerformanceCounter _ramAvailable;
	private static PerformanceCounter _ramUsedPercent;
	private static PerformanceCounter _cpuPercent;
	private static double totalRam, ramAvailable, ramUsedPercent, cpuPercent, serverPort, percentLoaded,
		runtimeTickCount, runtimeFrameCount;
	private static bool render, clientIsConnected, serverIsRunning, assetLoadBegin, assetLoadUpdate, assetLoadEnd;
	private static Dictionary<string, string> clientRealIDs;
	private static List<string> clientIDs;
	private static Server server;
	private static Client client;
	private static Thread resourceLoading;
	private static List<QueuedAsset> queuedAssets;
	private static Dictionary<string, Texture> textures;
	private static Dictionary<string, Font> fonts;
	private static Dictionary<string, Music> music;
	private static Dictionary<string, Sound> sounds;
	private static Sprite world, camera;
	private static SFML.Graphics.View view;

	private class QueuedAsset
	{
		public string path;
		public AssetType asset;
		public string error;
	}
	#endregion
	private class Data
	{
		[JsonProperty]
		public bool consoleIsShown, pauseOnWindowUnfocused, renderSuspended, vSync, sleepPrevented, multiplayerLogMessagesToConsole;
		[JsonProperty]
		public string clientID, ip;
		[JsonProperty]
		public double frameRateLimit, totalTickCount, totalFrameCount, totalTime;
		[JsonProperty]
		public SFML.Graphics.Color backgroundColor;
		[JsonProperty]
		public Dictionary<string, List<object>> storage;
		[JsonProperty]
		public Dictionary<string, bool> gates, timerPauses, animationReversed;
		[JsonProperty]
		public Dictionary<string, double> timers, timerDurations, gateEntriesCount, animationTicks;
		[JsonProperty]
		public Dictionary<string, UnitInstance> objects;
		[JsonProperty]
		public SortedDictionary<double, List<string>> objectsDepthSorted;
		[JsonProperty]
		public Dictionary<CameraNumber, double> cameraNumbers;
		[JsonProperty]
		public List<string> animationIDs;
	}
	private static Data data;

	#region Some complicated shit
	#region Sleep Prevention
	[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
	static extern EXECUTION_STATE SetThreadExecutionState(EXECUTION_STATE esFlags);
	[Flags]
	private enum EXECUTION_STATE : uint
	{
		ES_AWAYMODE_REQUIRED = 0x00000040,
		ES_CONTINUOUS = 0x80000000,
		ES_DISPLAY_REQUIRED = 0x00000002,
		ES_SYSTEM_REQUIRED = 0x00000001
		// Legacy flag, should not be used.
		// ES_USER_PRESENT = 0x00000004
	}
	#endregion
	#region Window Maximize/Minimize
	[DllImport("SDL2.dll", CallingConvention = CallingConvention.Cdecl)]
	private static extern void SDL_MaximizeWindow(IntPtr window);
	#endregion
	#region Show Console
	private static void Form1_Load(object sender, EventArgs e)
	{
		AllocConsole();
	}

	[DllImport("kernel32.dll", SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	static extern bool AllocConsole();
	#endregion
	#region Select/Edit Mode Console
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
	#region Total RAM
	[DllImport("kernel32.dll")]
	[return: MarshalAs(UnmanagedType.Bool)]
	static extern bool GetPhysicallyInstalledSystemMemory(out long TotalMemoryInKilobytes);
	#endregion
	#endregion
	#region Events
	private static void CreateEvents()
	{
		window.Closed += new EventHandler(game._Stop);
		window.GainedFocus += new EventHandler(game._OnWindowFocused);
		window.LostFocus += new EventHandler(game._OnWindowUnfocused);
		window.Resized += new EventHandler<SizeEventArgs>(game._OnWindowResized);
		window.KeyPressed += new EventHandler<SFML.Window.KeyEventArgs>(game._OnKeyboardKeyPressed);
		window.KeyReleased += new EventHandler<SFML.Window.KeyEventArgs>(game._OnKeyboardKeyReleased);
		window.SetKeyRepeatEnabled(false);
		form.KeyPress += new KeyPressEventHandler(game._OnTextInput);
		window.MouseButtonPressed += new EventHandler<MouseButtonEventArgs>(game._OnMouseButtonClicked);
		window.MouseButtonReleased += new EventHandler<MouseButtonEventArgs>(game._OnMouseButtonReleased);
		form.MouseDoubleClick += new MouseEventHandler(game._OnMouseButtonDoubleClicked);
		form.InputLanguageChanged += new InputLanguageChangedEventHandler(game._OnInputLanguageChanged);
		window.MouseMoved += new EventHandler<MouseMoveEventArgs>(game._OnMouseMoved);
		window.MouseEntered += new EventHandler(game._OnMouseHoveredWindow);
		window.MouseLeft += new EventHandler(game._OnMouseUnoveredWindow);
		window.MouseWheelScrolled += new EventHandler<MouseWheelScrollEventArgs>(game._OnMouseWheelScrolled);
	}
	#region Game
	private void _Stop(object sender, EventArgs e)
	{
		Game.Running.DoStop();
	}

	public virtual void OnGameProgressChanged(Progress progress, double runtimeTickCount, double totalTickCount) { }
	public virtual void OnFrameDrawn(double runtimeFrameCount, double totalFrameCount) { }
	#endregion
	#region Window
	public virtual void OnWindowAction(WindowAction action) { }
	private void _OnWindowFocused(object sender, EventArgs e)
	{
		render = true;
		OnWindowAction(WindowAction.Focused);
	}
	private void _OnWindowUnfocused(object sender, EventArgs e)
	{
		OnWindowAction(WindowAction.Unfocused);
	}
	private void _OnWindowResized(object sender, EventArgs e)
	{
		render = true;
		OnWindowAction(WindowAction.Resized);
	}
	#endregion
	#region Keyboard
	private void _OnKeyboardKeyPressed(object sender, EventArgs e)
	{
		var keyArgs = (SFML.Window.KeyEventArgs)e;
		var key = (KeyboardKey)keyArgs.Code;
		OnKeyboardKey(key, KeyboardAction.Pressed);
	}
	private void _OnKeyboardKeyReleased(object sender, EventArgs e)
	{
		var keyArgs = (SFML.Window.KeyEventArgs)e;
		var key = (KeyboardKey)keyArgs.Code;
		OnKeyboardKey(key, KeyboardAction.Released);
	}
	private void _OnTextInput(object sender, EventArgs e)
	{
		var keyArgs = (KeyPressEventArgs)e;
		var keyStr = keyArgs.KeyChar.ToString();
		keyStr = keyStr.Replace('\r', '\n');
		if (keyStr == "\b") keyStr = "";
		OnKeyboardTextInput(keyStr, keyStr == "\b", keyStr == Environment.NewLine, keyStr == "\t");
	}
	public virtual void OnKeyboardKey(KeyboardKey key, KeyboardAction action) { }
	public virtual void OnKeyboardTextInput(string textSymbol, bool isBackspace, bool isEnter, bool isTab) { }
	private void _OnInputLanguageChanged(object sender, EventArgs e)
	{
		var langArgs = (InputLanguageChangedEventArgs)e;
		var culture = langArgs.InputLanguage.Culture;
		OnInputLanguageChanged(culture.EnglishName, culture.NativeName, culture.Name);
	}
	public virtual void OnInputLanguageChanged(string englishName, string nativeName, string languageCode) { }
	#endregion
	#region Mouse
	private void _OnMouseButtonClicked(object sender, EventArgs e)
	{
		var buttonArgs = (MouseButtonEventArgs)e;
		var button = (MouseButton)buttonArgs.Button;
		OnMouseButtonAction(button, MouseButtonAction.ButtonClicked);
	}
	private void _OnMouseButtonReleased(object sender, EventArgs e)
	{
		var buttonArgs = (MouseButtonEventArgs)e;
		var button = (MouseButton)buttonArgs.Button;
		OnMouseButtonAction(button, MouseButtonAction.ButtonReleased);
	}
	private void _OnMouseButtonDoubleClicked(object sender, EventArgs e)
	{
		var buttonArgs = (MouseEventArgs)e;
		var button = MouseButton.Unknown;
		switch (buttonArgs.Button)
		{
			case MouseButtons.Left: button = MouseButton.Left; break;
			case MouseButtons.Right: button = MouseButton.Right; break;
			case MouseButtons.Middle: button = MouseButton.Middle; break;
			case MouseButtons.XButton1: button = MouseButton.ExtraButton1; break;
			case MouseButtons.XButton2: button = MouseButton.ExtraButton2; break;
		}
		OnMouseButtonAction(button, MouseButtonAction.ButtonDoubleClicked);
	}
	private void _OnMouseMoved(object sender, EventArgs e)
	{
		OnMouseAction(MouseAction.CursorMoved);
	}
	private void _OnMouseHoveredWindow(object sender, EventArgs e)
	{
		OnMouseAction(MouseAction.CursorHoveredWindow);
	}
	private void _OnMouseUnoveredWindow(object sender, EventArgs e)
	{
		OnMouseAction(MouseAction.CursorUnhoveredWindow);
	}
	private void _OnMouseWheelScrolled(object sender, EventArgs e)
	{
		var arguments = (MouseWheelScrollEventArgs)e;
		var wheel = (MouseWheel)arguments.Wheel;
		OnMouseWheelScrolled(arguments.Delta == 1 ? Direction.Up : Direction.Down, wheel);
	}
	public virtual void OnMouseAction(MouseAction action) { }
	public virtual void OnMouseButtonAction(MouseButton button, MouseButtonAction action) { }
	public virtual void OnMouseWheelScrolled(Direction direction, MouseWheel wheel) { }
	#endregion
	#region Multiplayer
	public virtual void OnMultiplayerMessageReceived(string fromID, string message) { }
	public virtual void OnMultiplayerClientConnected(string clientID) { }
	public virtual void OnMultiplayerClientDisconnected(string clientID) { }
	public virtual void OnMultiplayerConnected() { }
	public virtual void OnMultiplayerDisconnected() { }
	public virtual void OnMultiplayerTakenID(string newID) { }
	#endregion
	#region Other
	/// <summary>
	/// An example of the "When to happen" <see cref="Simple"/> structure.
	/// </summary>
	public virtual void OverrideMethodExample(object parameter) { }
	public virtual void OnTimerOver(string timerID) { }
	public virtual void OnAssetLoadingProgressChanged(Progress progress, double percentLoaded) { }
	public virtual void OnAnimationTick(string animationID, double tick, bool reversed) { }
	#endregion
	#endregion
	#region Main
	private static void CreateWindow()
	{
		form = new Form();
		var w = (int)VideoMode.DesktopMode.Width;
		var h = (int)VideoMode.DesktopMode.Height;
		form.SetBounds(w, h, w / 2, h / 2);
		window = new RenderWindow(form.Handle);
		window.SetVisible(true);
		window.SetTitle("Simple Game");
	}
	private static void SetStartDefaults()
	{
		Game.BackgroundColor.SetToSample(Color.Black);
		render = true;

		view = new SFML.Graphics.View(new Vector2f(0, 0), new Vector2f(window.Size.X, window.Size.Y));
		window.SetView(view);

		data.storage = new Dictionary<string, List<object>>();
		directory = AppDomain.CurrentDomain.BaseDirectory;
		data.gates = new Dictionary<string, bool>();
		data.gateEntriesCount = new Dictionary<string, double>();
		data.timers = new Dictionary<string, double>();
		data.timerDurations = new Dictionary<string, double>();
		data.timerPauses = new Dictionary<string, bool>();
		data.multiplayerLogMessagesToConsole = true;
		data.objects = new Dictionary<string, UnitInstance>();
		data.objectsDepthSorted = new SortedDictionary<double, List<string>>();
		data.cameraNumbers = new Dictionary<CameraNumber, double>();
		data.ip = Multiplayer.Client.Connection.IP.SameDevice.Get();
		data.animationReversed = new Dictionary<string, bool>();
		data.animationTicks = new Dictionary<string, double>();
		data.animationIDs = new List<string>();

		pickedIDs = new List<string>();
		clientRealIDs = new Dictionary<string, string>();
		clientIDs = new List<string>();
		queuedAssets = new List<QueuedAsset>();
		textures = new Dictionary<string, Texture>();
		fonts = new Dictionary<string, Font>();
		sounds = new Dictionary<string, Sound>();
		music = new Dictionary<string, Music>();

		world = new Sprite();
		camera = new Sprite();

		serverPort = 1234;
		multiplayerMsgComponentSep = "j#@%f";
		multiplayerMsgSep = "_#!~xc";
	}

	private static void Run()
	{
		while (window.IsOpen)
		{
			Thread.Sleep(1); // calming down the cpu
			if (data.pauseOnWindowUnfocused && window.HasFocus() == false) continue; // pause on window focus
			render = runtimeFrameCount == 1; // always render the first frame, otherwise reset each frame to false
			Application.DoEvents();
			window.DispatchEvents();

			data.totalTickCount++;
			runtimeTickCount++;
			data.totalTime += tickDeltaTime.ElapsedTime.AsSeconds();
			UpdateTimers();

			if (assetLoadBegin) game.OnAssetLoadingProgressChanged(Progress.Start, 0);
			assetLoadBegin = false;

			if (assetLoadUpdate) game.OnAssetLoadingProgressChanged(Progress.Updating, Asset.Loading.Get());
			assetLoadUpdate = false;

			if (assetLoadEnd) game.OnAssetLoadingProgressChanged(Progress.End, 100);
			assetLoadEnd = false;

			game.OnGameProgressChanged(Progress.Updating, runtimeTickCount, data.totalTickCount);

			tickDeltaTime.Restart();

			// calm down performance counters to avoid wrong values
			var picked = pickedIDs.ToArray();
			Instance.PickedIDs.Set("performance-counters-gate-key-;;;asflij");
			if (Instance.Gate.Opened.DoGet((int)time.ElapsedTime.AsSeconds() % 2 == 0)) UpdatePerformanceCounters();
			Instance.PickedIDs.Set(picked);

			if (render == false || data.renderSuspended) continue;
			data.totalFrameCount++;
			runtimeFrameCount++;
			frameDeltaTime.Restart();

			window.Clear(data.backgroundColor);
			Draw();
			game.OnFrameDrawn(runtimeFrameCount, data.totalFrameCount);
			window.Display();
		}
	}
	private static void Draw()
	{
		var reversed = data.objectsDepthSorted.Reverse();
		foreach (var kvp in reversed)
		{
			foreach (var obj in kvp.Value)
			{
				data.objects[obj].Update();
				data.objects[obj].Draw();
			}
		}
	}

	private static void UpdatePerformanceCounters()
	{
		ramAvailable = _ramAvailable.NextValue() / 1000;
		ramUsedPercent = _ramUsedPercent.NextValue();
		cpuPercent = _cpuPercent.NextValue();
	}
	private static void UpdateTimers()
	{
		var IDsToPause = new List<string>();
		foreach (var kvp in data.timers)
		{
			if (data.timerPauses[kvp.Key]) continue;

			var delta = tickDeltaTime.ElapsedTime.AsSeconds();
			data.timers[kvp.Key] -= delta;
			if (kvp.Value - delta * 2 <= 0)
			{
				IDsToPause.Add(kvp.Key);
			}
		}
		foreach (var ID in IDsToPause)
		{
			data.timers[ID] = data.timerDurations[ID];
			if (data.animationIDs.Contains(ID))
			{
				if (data.animationTicks.ContainsKey(ID) == false) data.animationTicks[ID] = 0;
				var reversed = data.animationReversed.ContainsKey(ID) && data.animationReversed[ID];
				data.animationTicks[ID] += reversed ? -1 : 1;
				game.OnAnimationTick(ID, data.animationTicks[ID], reversed);
				continue;
			}
			data.timerPauses[ID] = true;
			game.OnTimerOver(ID);
		}
	}
	#endregion

	/// <summary>
	/// An example of the <see cref="Simple"/>.<see cref="Action"/> structure.<br></br><br></br>
	/// This is one of the two structures in <see cref="Simple"/>. It covers the 'What' and 'How' things should happen in
	/// the game.<br></br>
	/// To check the other structure that covers 'When' something should happen, hover <see cref="Simple"/> and
	/// <see cref="OverrideMethodExample"/>.<br></br><br></br>
	/// The <see cref="Action"/> consists of 3 types of methods<br></br>
	/// where the 'What' aspect is replaced by <see cref="Action"/> and<br></br>
	/// the 'How' aspect is replaced by <see cref="object"/> (can be multiple or optional, see example bellow):<br></br>
	/// <see cref="Get"/><br></br>
	/// <see cref="Set"/><br></br>
	/// <see cref="Do"/><br></br><br></br>
	/// <see cref="Action"/> examples in code:<br></br>
	/// <paramref name="var"/> <paramref name="cameraZoom"/> = <see cref="Camera.Zoom.Get"/>;
	/// <paramref name="                               "/>
	/// // take the camera zoom as a scale number, default is 1 <br></br>
	/// <paramref name="var"/> <paramref name="fps"/> = <see cref="Performance.Frame.Rate"/>.<typeparamref name="Get"/>(averaged:
	/// true);<paramref name="       "/> // get the average frames per second since the start<br></br>
	/// <see cref="Instance.PickedIDs"/>.<typeparamref name="Set"/>("player", "enemy");
	/// <paramref name="                            "/>// select some instances by their IDs
	/// <br></br>
	/// <see cref="Instance.Unit.Appearance.Opacity"/>.<typeparamref name="Set"/>(opacityPercent: 50);
	/// // make the selected instances 50% transparent, default is 100% <br></br>
	/// <see cref="Game.Running.DoStop"/>;<paramref name="                                                          "/>
	/// // stop the game<br></br>
	/// <br></br><br></br>
	/// Hover <see cref="Simple"/>, <see cref="Get"/>, <see cref="Set"/> and <see cref="Do"/> for more information.
	/// </summary>
	public static class Action
	{
		public static object Get(object value) => default;
		public static void Set(object value) { }
		public static void Do(object value) { }
	}
	public static class Instance
	{
		/// <summary>
		/// A way to select on which <see cref="Instance"/> to perform <see cref="Action"/> methods on.<br></br><br></br>
		/// Make sure picking happens before the <see cref="Action"/> methods.<br></br>
		/// Make sure to pick only one ID before <see cref="Action.Get"/> methods.<br></br>
		/// Hover <see cref="Action"/> and <see cref="Instance"/> for more information.
		/// </summary>
		public static class PickedIDs
		{
			public static void Set(params string[] IDs)
			{
				pickedIDs.Clear();
				if (IDs == null) return;
				for (int i = 0; i < IDs.Length; i++)
				{
					if (IDs[i] == null) continue; // skip null
					if (pickedIDs.Contains(IDs[i])) continue; // already picked

					pickedIDs.Add(IDs[i]);
				}
			}
			public static string[] Get()
			{
				return pickedIDs.ToArray();
			}
		}
		public static class Unit
		{
			public static class Appearance
			{
				public static class Parallax
				{
					public static class Position
					{
						public static void Set(double percentX, double percentY)
						{
							SetX(percentX);
							SetY(percentY);
						}
						public static void SetX(double percent)
						{
							if (Statics.NoIDPickedError()) return;

							var picked = PickedIDs.Get();
							foreach (var objectID in picked)
							{
								PickedIDs.Set(objectID);
								if (ExistanceCheck()) continue;

								data.objects[objectID].numbers[UnitNumber.ParallaxX] = percent;
								render = true; // TODO validate
							}
							PickedIDs.Set(picked);
						}
						public static void SetY(double percent)
						{
							if (Statics.NoIDPickedError()) return;

							var picked = PickedIDs.Get();
							foreach (var objectID in picked)
							{
								PickedIDs.Set(objectID);
								if (ExistanceCheck()) continue;

								data.objects[objectID].numbers[UnitNumber.ParallaxY] = percent;
								render = true; // TODO validate
							}
							PickedIDs.Set(picked);
						}
						public static double GetX()
						{
							return GetNumber(UnitNumber.ParallaxX);
						}
						public static double GetY()
						{
							return GetNumber(UnitNumber.ParallaxY);
						}
					}
					public static class Scale
					{
						public static void Set(double percent)
						{
							if (Statics.NoIDPickedError()) return;

							var picked = PickedIDs.Get();
							foreach (var objectID in picked)
							{
								PickedIDs.Set(objectID);
								if (ExistanceCheck()) continue;

								data.objects[objectID].numbers[UnitNumber.ParallaxScale] = percent;
								render = true; // TODO validate
							}
							PickedIDs.Set(picked);
						}
						public static double Get()
						{
							return GetNumber(UnitNumber.ParallaxScale);
						}
					}
					public static class Angle
					{
						public static void Set(double percent)
						{
							if (Statics.NoIDPickedError()) return;

							var picked = PickedIDs.Get();
							foreach (var objectID in picked)
							{
								PickedIDs.Set(objectID);
								if (ExistanceCheck()) continue;

								data.objects[objectID].numbers[UnitNumber.ParallaxAngle] = percent;
								render = true; // TODO validate
							}
							PickedIDs.Set(picked);
						}
						public static double Get()
						{
							return GetNumber(UnitNumber.ParallaxAngle);
						}
					}
				}
				public static class Mask
				{
					public static class IDs
					{
						public static class Target
						{
							public static void Set(string unitID)
							{
								if (Statics.NoIDPickedError()) return;

								var picked = PickedIDs.Get();

								PickedIDs.Set(unitID);
								if (Statics.DoesNotExistError(data.objects, "Unit")) return;

								foreach (var objectID in picked)
								{
									PickedIDs.Set(objectID);
									if (ExistanceCheck()) continue;

									if (data.objects[objectID].textCollections.ContainsKey(UnitTexts.MaskIDs) == false)
									{
										data.objects[objectID].textCollections.Add(UnitTexts.MaskIDs, new List<string>());
									}
									data.objects[unitID].shader.SetUniform("has_mask", false);
									data.objects[objectID].shader.SetUniform("has_mask", true);

									data.objects[objectID].textCollections[UnitTexts.MaskIDs].Add(unitID);
									data.objects[unitID].texts[UnitText.MaskTargetID] = objectID;

									render = true; // TODO validate
								}
								PickedIDs.Set(picked);
							}
							public static string Get()
							{
								return GetText(UnitText.MaskTargetID);
							}
						}
						public static class Masks
						{
							public static string[] Get()
							{
								return GetTexts(UnitTexts.MaskIDs);
							}
						}
					}
					public static class Extracted
					{
						public static void Set(bool extracted)
						{
							if (Statics.NoIDPickedError()) return;

							var picked = PickedIDs.Get();
							foreach (var objectID in picked)
							{
								PickedIDs.Set(objectID);
								if (ExistanceCheck()) continue;

								data.objects[objectID].facts[UnitFact.MasksIn] = extracted;
								data.objects[objectID].shader.SetUniform("mask_out", extracted == false);

								Texture.FilePath.Set(Texture.FilePath.Get()); // update the texture to prevent the weird upside-down flipping
							}
							PickedIDs.Set(picked);
						}
						public static bool Get()
						{
							return GetFact(UnitFact.MasksIn) == false;
						}
					}
					public static class Color
					{
						public static void Set(double percentRed, double percentGreen, double percentBlue)
						{
							SetRed(percentRed);
							SetGreen(percentGreen);
							SetBlue(percentBlue);
						}
						public static void SetToSample(Simple.Color color)
						{
							var c = Statics.GetColorFromSample(color);
							Set(
								Number.Converted.To.Percent.Get(c.R, 0, 255),
								Number.Converted.To.Percent.Get(c.G, 0, 255),
								Number.Converted.To.Percent.Get(c.B, 0, 255));
						}
						public static void SetRed(double percentRed)
						{
							SetShaderArg(UnitEffect.MaskRed, "mask_red", percentRed, percentRed, true);
						}
						public static void SetGreen(double percentGreen)
						{
							SetShaderArg(UnitEffect.MaskGreen, "mask_green", percentGreen, percentGreen, true);
						}
						public static void SetBlue(double percentBlue)
						{
							SetShaderArg(UnitEffect.MaskBlue, "mask_blue", percentBlue, percentBlue, true);
						}
						public static double GetRed()
						{
							return GetEffect(UnitEffect.MaskRed);
						}
						public static double GetGreen()
						{
							return GetEffect(UnitEffect.MaskGreen);
						}
						public static double GetBlue()
						{
							return GetEffect(UnitEffect.MaskBlue);
						}
					}
				}
				public static class Opacity
				{
					public static void Set(double opacityPercent)
					{
						if (Statics.NoIDPickedError()) return;

						opacityPercent = Number.Limited.Get(opacityPercent, 0, 100);
						var p255 = Number.Converted.From.Percent.Get(opacityPercent, 0, 255);

						var picked = PickedIDs.Get();
						foreach (var objectID in picked)
						{
							PickedIDs.Set(objectID);
							if (ExistanceCheck()) continue;

							var c = data.objects[objectID].sprite.Color;
							data.objects[objectID].sprite.Color = new SFML.Graphics.Color(c.R, c.G, c.B, (byte)p255);
							data.objects[objectID].shaderArgs[UnitEffect.Opacity] = opacityPercent;
							render = true; // TODO validate
						}
						PickedIDs.Set(picked);
					}
					public static double Get()
					{
						return GetEffect(UnitEffect.Opacity);
					}
				}
				public static class Hidden
				{
					public static void Set(bool hidden)
					{
						if (Statics.NoIDPickedError()) return;

						var picked = PickedIDs.Get();
						foreach (var objectID in picked)
						{
							PickedIDs.Set(objectID);
							if (ExistanceCheck()) continue;

							if (Get() == hidden) continue; // same value, skip re-rendering the screen
							data.objects[objectID].facts[UnitFact.IsHidden] = hidden;
							render = true;
						}
						PickedIDs.Set(picked);
					}
					public static bool Get()
					{
						return GetFact(UnitFact.IsHidden);
					}
				}
				public static class Origin
				{
					public static void Set(double percentX, double percentY)
					{
						SetX(percentX);
						SetY(percentY);
					}
					public static void SetX(double percentX)
					{
						if (Statics.NoIDPickedError()) return;

						var picked = PickedIDs.Get();
						foreach (var objectID in picked)
						{
							PickedIDs.Set(objectID);
							if (ExistanceCheck()) continue;

							if (data.objects[objectID].sprite == null) data.objects[objectID].sprite = new Sprite();

							var sprite = data.objects[objectID].sprite;
							var w = data.objects[objectID].sprite.TextureRect.Width;

							var repX = GetNumber(UnitNumber.RepeatAmountX);

							percentX = Simple.Number.Limited.Get(percentX, 0, 100);
							var pX = (float)percentX / 100;
							var value = sprite.Texture == null ? 0 : w * pX * ((float)repX / 2) + (w * pX / 2);

							data.objects[objectID].numbers[UnitNumber.OriginPercentX] = percentX;
							if (sprite.Origin.X == value) return; // skip re-rendering the screen & replacing same value

							data.objects[objectID].sprite.Origin = new Vector2f(value, sprite.Origin.Y);
							render = true;
						}
						PickedIDs.Set(picked);
					}
					public static void SetY(double percentY)
					{
						if (Statics.NoIDPickedError()) return;

						var picked = PickedIDs.Get();
						foreach (var objectID in picked)
						{
							PickedIDs.Set(objectID);
							if (ExistanceCheck()) continue;

							if (data.objects[objectID].sprite == null) data.objects[objectID].sprite = new Sprite();

							var sprite = data.objects[objectID].sprite;
							var h = data.objects[objectID].sprite.TextureRect.Height;

							var repY = GetNumber(UnitNumber.RepeatAmountY);

							percentY = Number.Limited.Get(percentY, 0, 100);
							var pY = (float)percentY / 100;
							var value = sprite.Texture == null ? 0 : h * pY * ((float)repY / 2) + (h * pY / 2);

							data.objects[objectID].numbers[UnitNumber.OriginPercentY] = percentY;
							if (sprite.Origin.Y == value) return; // skip re-rendering the screen & replacing same value

							data.objects[objectID].sprite.Origin = new Vector2f(sprite.Origin.X, value);
							render = true;
						}
						PickedIDs.Set(picked);
					}
					public static double GetX()
					{
						return GetNumber(UnitNumber.OriginPercentX);
					}
					public static double GetY()
					{
						return GetNumber(UnitNumber.OriginPercentY);
					}
				}
				public static class Depth
				{
					public static void Set(double depth)
					{
						if (Statics.NoIDPickedError()) return;

						var picked = PickedIDs.Get();
						foreach (var objectID in picked)
						{
							PickedIDs.Set(objectID);
							if (ExistanceCheck()) continue;

							var numbers = data.objects[objectID].numbers;
							var oldDepth = numbers.ContainsKey(UnitNumber.Depth) ? numbers[UnitNumber.Depth] : 0;
							data.objects[objectID].numbers[UnitNumber.Depth] = depth;

							if (data.objectsDepthSorted.ContainsKey(depth) == false)
							{
								data.objectsDepthSorted[depth] = new List<string>();
							}
							if (data.objectsDepthSorted[depth].Contains(objectID) == false)
							{
								data.objectsDepthSorted[depth].Add(objectID);
							}

							if (oldDepth == depth) return;

							render = true;

							if (data.objectsDepthSorted.ContainsKey(oldDepth) &&
								data.objectsDepthSorted[oldDepth].Contains(objectID))
							{
								data.objectsDepthSorted[oldDepth].Remove(objectID);
								if (data.objectsDepthSorted[oldDepth].Count == 0)
								{
									data.objectsDepthSorted.Remove(oldDepth);
								}
							}
						}
						PickedIDs.Set(picked);
					}
					public static double Get()
					{
						return GetNumber(UnitNumber.Depth);
					}
				}
			}
			public static class Texture
			{
				public static class Offset
				{
					public static void Set(double percentX, double percentY)
					{
						SetX(percentX);
						SetY(percentY);
					}
					public static void SetX(double percentX)
					{
						if (Statics.NoIDPickedError()) return;

						var picked = PickedIDs.Get();
						foreach (var objectID in picked)
						{
							PickedIDs.Set(objectID);
							if (ExistanceCheck()) continue;

							data.objects[objectID].numbers[UnitNumber.DisplayPercentX] = percentX;

							percentX /= 100;

							var textRect = data.objects[objectID].sprite.TextureRect;
							data.objects[objectID].sprite.TextureRect =
								new IntRect((int)(textRect.Width * percentX), textRect.Top, textRect.Width, textRect.Height);

							// update origin according to how many times the texture is repeated
							Appearance.Origin.Set(Appearance.Origin.GetX(), Appearance.Origin.GetY());

							render = true; // TODO validate
						}
						PickedIDs.Set(picked);
					}
					public static void SetY(double percentY)
					{
						if (Statics.NoIDPickedError()) return;

						var picked = PickedIDs.Get();
						foreach (var objectID in picked)
						{
							PickedIDs.Set(objectID);
							if (ExistanceCheck()) continue;

							data.objects[objectID].numbers[UnitNumber.DisplayPercentX] = percentY;

							percentY /= 100;

							var textRect = data.objects[objectID].sprite.TextureRect;
							data.objects[objectID].sprite.TextureRect =
								new IntRect(textRect.Left, (int)(textRect.Height * percentY), textRect.Width, textRect.Height);

							// update origin according to how many times the texture is repeated
							Appearance.Origin.Set(Appearance.Origin.GetX(), Appearance.Origin.GetY());

							render = true; // TODO validate
						}
						PickedIDs.Set(picked);
					}
					public static double GetX()
					{
						return GetNumber(UnitNumber.DisplayPercentY);
					}
					public static double GetY()
					{
						return GetNumber(UnitNumber.DisplayPercentY);
					}
				}
				public static class Size
				{
					public static void Set(double percentWidth, double percentHeight)
					{
						SetWidth(percentWidth);
						SetHeight(percentHeight);
					}
					public static void SetWidth(double percentWidth)
					{
						if (Statics.NoIDPickedError()) return;

						var picked = PickedIDs.Get();
						foreach (var objectID in picked)
						{
							PickedIDs.Set(objectID);
							if (ExistanceCheck()) continue;

							data.objects[objectID].numbers[UnitNumber.DisplayPercentWidth] = percentWidth;

							percentWidth /= 100;

							var sz = data.objects[objectID].sprite.Texture.Size;
							var textRect = data.objects[objectID].sprite.TextureRect;
							data.objects[objectID].sprite.TextureRect =
								new IntRect(textRect.Left, textRect.Top, (int)(sz.X * percentWidth), textRect.Height);

							render = true; // TODO validate
						}
						PickedIDs.Set(picked);
					}
					public static void SetHeight(double percentHeight)
					{
						if (Statics.NoIDPickedError()) return;

						var picked = PickedIDs.Get();
						foreach (var objectID in picked)
						{
							PickedIDs.Set(objectID);
							if (ExistanceCheck()) continue;

							data.objects[objectID].numbers[UnitNumber.DisplayPercentWidth] = percentHeight;

							percentHeight /= 100;

							var sz = data.objects[objectID].sprite.Texture.Size;
							var textRect = data.objects[objectID].sprite.TextureRect;
							data.objects[objectID].sprite.TextureRect =
								new IntRect(textRect.Left, textRect.Top, textRect.Width, (int)(sz.Y * percentHeight));

							render = true; // TODO validate
						}
						PickedIDs.Set(picked);
					}
					public static double GetWidth()
					{
						return GetNumber(UnitNumber.DisplayPercentWidth);
					}
					public static double GetHeight()
					{
						return GetNumber(UnitNumber.DisplayPercentHeight);
					}
				}
				public static class Repeats
				{
					public static void Set(double repeatsX, double repeatsY)
					{
						SetX(repeatsX);
						SetY(repeatsY);
					}
					public static void SetX(double repeatsX)
					{
						if (Statics.NoIDPickedError()) return;

						var picked = PickedIDs.Get();
						foreach (var objectID in picked)
						{
							PickedIDs.Set(objectID);
							if (ExistanceCheck()) continue;

							data.objects[objectID].numbers[UnitNumber.RepeatAmountX] = Number.Rounded.Get(repeatsX);

							// update origin, according to the repeat amount
							Appearance.Origin.Set(Appearance.Origin.GetX(), Appearance.Origin.GetY());
						}
						PickedIDs.Set(picked);
					}
					public static void SetY(double repeatsY)
					{
						if (Statics.NoIDPickedError()) return;

						var picked = PickedIDs.Get();
						foreach (var objectID in picked)
						{
							PickedIDs.Set(objectID);
							if (ExistanceCheck()) continue;

							data.objects[objectID].numbers[UnitNumber.RepeatAmountY] = Number.Rounded.Get(repeatsY);

							// update origin, according to the repeat amount
							Appearance.Origin.Set(Appearance.Origin.GetX(), Appearance.Origin.GetY());
						}
						PickedIDs.Set(picked);
					}
					public static double GetX()
					{
						return GetNumber(UnitNumber.RepeatAmountX);
					}
					public static double GetY()
					{
						return GetNumber(UnitNumber.RepeatAmountY);
					}
				}
				public static class FilePath
				{
					public static void Set(string filePath)
					{
						if (Statics.NoIDPickedError()) return;

						var picked = PickedIDs.Get();
						foreach (var objectID in picked)
						{
							PickedIDs.Set(objectID);
							if (ExistanceCheck()) continue;

							if (textures.ContainsKey(filePath) == false)
							{
								Statics.ShowError(1,
									$"Texture not found from file '{filePath}'.\n\n" +
									$"Loading assets advices:\n" +
									$"- Make sure the Texture is loaded with\n" +
									$"  '{nameof(Simple)}.{nameof(Asset)}.{nameof(Asset.Loading)}.{nameof(Asset.Loading.Do)}" +
									$"(\"textures\\\\myTexture.png\")' before using it.\n" +
									$"- Override the '{nameof(game.OnAssetLoadingProgressChanged)}' method to follow\n" +
									$"  the progress of the currently loading assets. This is useful for\n" +
									$"  loading screens, as well as using the assets once they are loaded.\n" +
									$"- Make sure the file path is written correctly.\n" +
									$"  Example in code: \"folder\\\\folder\\\\file.extension\"\n" +
									$"  (The path should start inside the game directory & the rest is skipped, \n" +
									$"  just like inbetween the [] \"D:\\Games\\MyGame\\[Textures\\texture.png]\")");
								continue;
							}
							SetTexture(objectID, filePath, textures[filePath]);
						}
						PickedIDs.Set(picked);
					}
					public static string Get()
					{
						return GetText(UnitText.TexturePath);
					}
				}
				public static class Smoothed
				{
					public static void Set(bool smoothed)
					{
						if (Statics.NoIDPickedError()) return;

						var picked = PickedIDs.Get();
						foreach (var objectID in picked)
						{
							PickedIDs.Set(objectID);
							if (ExistanceCheck()) continue;

							data.objects[objectID].facts[UnitFact.HasSmoothTexture] = smoothed;
							if (data.objects[objectID].sprite.Texture == null) return;

							data.objects[objectID].sprite.Texture.Smooth = smoothed;
							render = true; // TODO validate
						}
						PickedIDs.Set(picked);
					}
					public static bool Get()
					{
						return GetFact(UnitFact.HasSmoothTexture);
					}
				}
				public static class Repeated
				{
					public static void Set(bool repeated)
					{
						if (Statics.NoIDPickedError()) return;

						var picked = PickedIDs.Get();
						foreach (var objectID in picked)
						{
							PickedIDs.Set(objectID);
							if (ExistanceCheck()) continue;

							data.objects[objectID].facts[UnitFact.IsRepeated] = repeated;
							if (data.objects[objectID].sprite.Texture == null) return;

							data.objects[objectID].sprite.Texture.Repeated = repeated;
							render = true; // TODO validate
						}
						PickedIDs.Set(picked);
					}
					public static bool Get()
					{
						return GetFact(UnitFact.IsRepeated);
					}
				}
			}
			public static class Effect
			{
				public static class Tint
				{
					public static void Set(double percentRed, double percentGreen, double percentBlue)
					{
						SetRed(percentRed);
						SetGreen(percentGreen);
						SetBlue(percentBlue);
					}
					public static void SetRed(double percentRed)
					{
						if (Statics.NoIDPickedError()) return;

						percentRed = Number.Limited.Get(percentRed, 0, 100);
						var p255 = Number.Converted.From.Percent.Get(percentRed, 0, 255);

						var picked = PickedIDs.Get();
						foreach (var objectID in picked)
						{
							PickedIDs.Set(objectID);
							if (ExistanceCheck()) continue;

							var c = data.objects[objectID].sprite.Color;
							data.objects[objectID].sprite.Color = new SFML.Graphics.Color((byte)p255, c.G, c.B, c.A);
							data.objects[objectID].text.FillColor = new SFML.Graphics.Color((byte)p255, c.G, c.B, c.A);
							data.objects[objectID].shaderArgs[UnitEffect.TintRed] = percentRed;
							render = true; // TODO validate
						}
						PickedIDs.Set(picked);
					}
					public static void SetGreen(double percentGreen)
					{
						if (Statics.NoIDPickedError()) return;

						percentGreen = Number.Limited.Get(percentGreen, 0, 100);
						var p255 = Number.Converted.From.Percent.Get(percentGreen, 0, 255);

						var picked = PickedIDs.Get();
						foreach (var objectID in picked)
						{
							PickedIDs.Set(objectID);
							if (ExistanceCheck()) continue;

							var c = data.objects[objectID].sprite.Color;
							data.objects[objectID].sprite.Color = new SFML.Graphics.Color(c.R, (byte)p255, c.B, c.A);
							data.objects[objectID].text.FillColor = new SFML.Graphics.Color(c.R, (byte)p255, c.B, c.A);
							data.objects[objectID].shaderArgs[UnitEffect.TintGreen] = percentGreen;
							render = true; // TODO validate
						}
						PickedIDs.Set(picked);
					}
					public static void SetBlue(double percentBlue)
					{
						if (Statics.NoIDPickedError()) return;

						percentBlue = Number.Limited.Get(percentBlue, 0, 100);
						var p255 = Number.Converted.From.Percent.Get(percentBlue, 0, 255);

						var picked = PickedIDs.Get();
						foreach (var objectID in picked)
						{
							PickedIDs.Set(objectID);
							if (ExistanceCheck()) continue;

							var c = data.objects[objectID].sprite.Color;
							data.objects[objectID].sprite.Color = new SFML.Graphics.Color(c.R, c.G, (byte)p255, c.A);
							data.objects[objectID].text.FillColor = new SFML.Graphics.Color(c.R, c.G, (byte)p255, c.A);
							data.objects[objectID].shaderArgs[UnitEffect.TintBlue] = percentBlue;
							render = true; // TODO validate
						}
						PickedIDs.Set(picked);
					}
					public static double GetRed()
					{
						return GetEffect(UnitEffect.TintRed);
					}
					public static double GetGreen()
					{
						return GetEffect(UnitEffect.TintGreen);
					}
					public static double GetBlue()
					{
						return GetEffect(UnitEffect.TintBlue);
					}
				}
				public static class Adjust
				{
					public static class Gamma
					{
						public static void Set(double percentGamma)
						{
							SetShaderArg(UnitEffect.Gamma, "adjust_gamma", percentGamma, percentGamma, true);
						}
						public static double Get()
						{
							return GetEffect(UnitEffect.Gamma);
						}
					}
					public static class Desaturation
					{
						public static void Set(double percentDesaturation)
						{
							SetShaderArg(UnitEffect.Desaturation, "adjust_desaturation", percentDesaturation, percentDesaturation, true);
						}
						public static double Get()
						{
							return GetEffect(UnitEffect.Desaturation);
						}
					}
					public static class Inversion
					{
						public static void Set(double percentInversion)
						{
							SetShaderArg(UnitEffect.Inversion, "adjust_inversion", percentInversion, percentInversion, true);
						}
						public static double Get()
						{
							return GetEffect(UnitEffect.Inversion);
						}
					}
					public static class Contrast
					{
						public static void Set(double percentContrast)
						{
							SetShaderArg(UnitEffect.Contrast, "adjust_contrast", percentContrast, percentContrast, true);
						}
						public static double Get()
						{
							return GetEffect(UnitEffect.Contrast);
						}
					}
					public static class Brightness
					{
						public static void Set(double percentBrightness)
						{
							SetShaderArg(UnitEffect.Brightness, "adjust_brightness", percentBrightness, percentBrightness, true);
						}
						public static double Get()
						{
							return GetEffect(UnitEffect.Brightness);
						}
					}
				}
				public static class Outline
				{
					public static class Opacity
					{
						public static void Set(double opacityPercent)
						{
							SetShaderArg(UnitEffect.Outline, "outline_effect", opacityPercent, opacityPercent, true);
						}
						public static double Get()
						{
							return GetEffect(UnitEffect.Outline);
						}
					}
					public static class Offset
					{
						public static void Set(double offset)
						{
							SetShaderArg(UnitEffect.OutlineOffset, "outline_offset", offset, offset / 500, false);
						}
						public static double Get()
						{
							return GetEffect(UnitEffect.OutlineOffset);
						}
					}
					public static class Color
					{
						public static void Set(double percentRed, double percentGreen, double percentBlue)
						{
							SetRed(percentRed);
							SetGreen(percentGreen);
							SetBlue(percentBlue);
						}
						public static void SetToSample(Simple.Color color)
						{
							var c = Statics.GetColorFromSample(color);
							Set(
								Number.Converted.To.Percent.Get(c.R, 0, 255),
								Number.Converted.To.Percent.Get(c.G, 0, 255),
								Number.Converted.To.Percent.Get(c.B, 0, 255));
						}
						public static void SetRed(double percentRed)
						{
							SetShaderArg(UnitEffect.OutlineRed, "outline_red", percentRed, percentRed, true);
						}
						public static void SetGreen(double percentGreen)
						{
							SetShaderArg(UnitEffect.OutlineGreen, "outline_green", percentGreen, percentGreen, true);
						}
						public static void SetBlue(double percentBlue)
						{
							SetShaderArg(UnitEffect.OutlineBlue, "outline_blue", percentBlue, percentBlue, true);
						}
						public static double SetRed()
						{
							return GetEffect(UnitEffect.OutlineRed);
						}
						public static double SetGreen()
						{
							return GetEffect(UnitEffect.OutlineGreen);
						}
						public static double SetBlue()
						{
							return GetEffect(UnitEffect.OutlineBlue);
						}
					}

				}
				public static class Fill
				{
					public static class Opacity
					{
						public static void Set(double percentOpacity)
						{
							SetShaderArg(UnitEffect.Fill, "fill_effect", percentOpacity, percentOpacity, true);
						}
						public static double Get()
						{
							return GetEffect(UnitEffect.Fill);
						}
					}
					public static class Color
					{
						public static void Set(double percentRed, double percentGreen, double percentBlue)
						{
							SetRed(percentRed);
							SetGreen(percentGreen);
							SetBlue(percentBlue);
						}
						public static void SetToSample(Simple.Color color)
						{
							var c = Statics.GetColorFromSample(color);
							Set(
								Number.Converted.To.Percent.Get(c.R, 0, 255),
								Number.Converted.To.Percent.Get(c.G, 0, 255),
								Number.Converted.To.Percent.Get(c.B, 0, 255));
						}
						public static void SetRed(double percentRed)
						{
							SetShaderArg(UnitEffect.FillRed, "fill_red", percentRed, percentRed, true);
						}
						public static void SetGreen(double percentGreen)
						{
							SetShaderArg(UnitEffect.FillGreen, "fill_green", percentGreen, percentGreen, true);
						}
						public static void SetBlue(double percentBlue)
						{
							SetShaderArg(UnitEffect.FillBlue, "fill_blue", percentBlue, percentBlue, true);
						}
						public static double SetRed()
						{
							return GetEffect(UnitEffect.FillRed);
						}
						public static double SetGreen()
						{
							return GetEffect(UnitEffect.FillGreen);
						}
						public static double SetBlue()
						{
							return GetEffect(UnitEffect.FillBlue);
						}
					}

				}
				public static class Blink
				{
					public static class Opacity
					{
						public static void Set(double percentOpacity)
						{
							SetShaderArg(UnitEffect.Blink, "blink_effect", percentOpacity, percentOpacity, true);
						}
						public static double Get()
						{
							return GetEffect(UnitEffect.Blink);
						}
					}
					public static class Speed
					{
						public static void Set(double speed)
						{
							SetShaderArg(UnitEffect.BlinkSpeed, "blink_speed", speed, speed, true);
						}
						public static double Get()
						{
							return GetEffect(UnitEffect.BlinkSpeed);
						}
					}

				}
				public static class Blur
				{
					public static class Opacity
					{
						public static void Set(double percentOpacity)
						{
							SetShaderArg(UnitEffect.Blur, "blur_effect", percentOpacity, percentOpacity, true);
						}
						public static double Get()
						{
							return GetEffect(UnitEffect.Blur);
						}
					}
					public static class Strength
					{
						public static void Set(double strengthX, double strengthY)
						{
							SetX(strengthX);
							SetY(strengthY);
						}
						public static void SetX(double strengthX)
						{
							SetShaderArg(UnitEffect.BlurStrengthX, "blur_strength_x", strengthX, strengthX / 200, true);
						}
						public static void SetY(double strengthY)
						{
							SetShaderArg(UnitEffect.BlurStrengthY, "blur_strength_y", strengthY, strengthY / 200, true);
						}
						public static double GetX()
						{
							return GetEffect(UnitEffect.BlurStrengthX);
						}
						public static double GetY()
						{
							return GetEffect(UnitEffect.BlurStrengthY);
						}
					}
				}
				public static class Earthquake
				{
					public static class Opacity
					{
						public static void Set(double opacityPercent)
						{
							SetShaderArg(UnitEffect.Earthquake, "earthquake_effect", opacityPercent, opacityPercent, true);
						}
						public static double Get()
						{
							return GetEffect(UnitEffect.Earthquake);
						}
					}
					public static class Strength
					{
						public static void Set(double strengthX, double strengthY)
						{
							SetX(strengthX);
							SetY(strengthY);
						}
						public static void SetX(double strengthX)
						{
							SetShaderArg(UnitEffect.EarthquakeX, "earthquake_effect_x", strengthX, strengthX / 100, true);
						}
						public static void SetY(double strengthY)
						{
							SetShaderArg(UnitEffect.EarthquakeY, "earthquake_effect_y", strengthY, strengthY / 100, true);
						}
						public static double GetX()
						{
							return GetEffect(UnitEffect.EarthquakeX);
						}
						public static double GetY()
						{
							return GetEffect(UnitEffect.EarthquakeY);
						}
					}
				}
				public static class Stretch
				{
					public static class Opacity
					{
						public static void Set(double opacityPercent)
						{
							SetShaderArg(UnitEffect.Stretch, "stretch_effect", opacityPercent, opacityPercent, true);
						}
						public static double Get()
						{
							return GetEffect(UnitEffect.Stretch);
						}
					}
					public static class Strength
					{
						public static void Set(double strengthX, double strengthY)
						{
							SetX(strengthX);
							SetY(strengthY);
						}
						public static void SetX(double strengthX)
						{
							SetShaderArg(UnitEffect.StretchX, "stretch_effect_x", strengthX, strengthX / 100, true);
						}
						public static void SetY(double strengthY)
						{
							SetShaderArg(UnitEffect.StretchY, "stretch_effect_y", strengthY, strengthY / 100, true);
						}
						public static double GetX()
						{
							return GetEffect(UnitEffect.StretchX);
						}
						public static double GetY()
						{
							return GetEffect(UnitEffect.StretchY);
						}
					}
					public static class Speed
					{
						public static void Set(double speedX, double speedY)
						{
							SetX(speedX);
							SetY(speedY);
						}
						public static void SetX(double speedX)
						{
							SetShaderArg(UnitEffect.StretchSpeedX, "stretch_speed_x", speedX, speedX * 5, true);
						}
						public static void SetY(double speedY)
						{
							SetShaderArg(UnitEffect.StretchSpeedY, "stretch_speed_y", speedY, speedY * 5, true);
						}
						public static double GetX()
						{
							return GetEffect(UnitEffect.StretchSpeedX);
						}
						public static double GetY()
						{
							return GetEffect(UnitEffect.StretchSpeedY);
						}
					}
				}
				public static class Water
				{
					public static class Opacity
					{
						public static void Set(double percentOpacity)
						{
							SetShaderArg(UnitEffect.Water, "water_effect", percentOpacity, percentOpacity, true);
						}
						public static double Get()
						{
							return GetEffect(UnitEffect.Water);
						}
					}
					public static class Strength
					{
						public static void Set(double strengthX, double strengthY)
						{
							SetX(strengthX);
							SetY(strengthY);
						}
						public static void SetX(double strengthX)
						{
							SetShaderArg(UnitEffect.WaterStrengthX, "water_strength_x", strengthX, strengthX, true);
						}
						public static void SetY(double strengthY)
						{
							SetShaderArg(UnitEffect.WaterStrengthY, "water_strength_y", strengthY, strengthY, true);
						}
						public static double GetX()
						{
							return GetEffect(UnitEffect.WaterStrengthX);
						}
						public static double GetY()
						{
							return GetEffect(UnitEffect.WaterStrengthY);
						}
					}
					public static class Speed
					{
						public static void Set(double speedX, double speedY)
						{
							SetX(speedX);
							SetY(speedY);
						}
						public static void SetX(double speedX)
						{
							SetShaderArg(UnitEffect.WaterSpeedX, "water_speed_x", speedX, speedX / 10, true);
						}
						public static void SetY(double speedY)
						{
							SetShaderArg(UnitEffect.WaterSpeedY, "water_speed_y", speedY, speedY / 10, true);
						}
						public static double GetX()
						{
							return GetEffect(UnitEffect.WaterSpeedX);
						}
						public static double GetY()
						{
							return GetEffect(UnitEffect.WaterSpeedY);
						}
					}
				}
				public static class Edge
				{
					public static class Opacity
					{
						public static void Set(double percentOpacity)
						{
							SetShaderArg(UnitEffect.Edge, "edge_effect", percentOpacity, percentOpacity, true);
						}
						public static double Get()
						{
							return GetEffect(UnitEffect.Edge);
						}
					}
					public static class Sensitivity
					{
						public static void Set(double percentSensitivity)
						{
							SetShaderArg(UnitEffect.EdgeThreshold, "edge_threshold", percentSensitivity, (100 - percentSensitivity) / 160, false);
						}
						public static double Get()
						{
							return GetEffect(UnitEffect.EdgeThreshold);
						}
					}
					public static class Thickness
					{
						public static void Set(double percentThickness)
						{
							SetShaderArg(UnitEffect.EdgeThickness, "edge_thickness", percentThickness, 1 - percentThickness / 500, true);
						}
						public static double Get()
						{
							return GetEffect(UnitEffect.EdgeThickness);
						}
					}
					public static class Color
					{
						public static void Set(double percentRed, double percentGreen, double percentBlue)
						{
							SetRed(percentRed);
							SetGreen(percentGreen);
							SetBlue(percentBlue);
						}
						public static void SetToSample(Simple.Color color)
						{
							var c = Statics.GetColorFromSample(color);
							Set(
								Number.Converted.To.Percent.Get(c.R, 0, 255),
								Number.Converted.To.Percent.Get(c.G, 0, 255),
								Number.Converted.To.Percent.Get(c.B, 0, 255));
						}
						public static void SetRed(double percentRed)
						{
							SetShaderArg(UnitEffect.EdgeRed, "edge_red", percentRed, percentRed, true);
						}
						public static void SetGreen(double percentGreen)
						{
							SetShaderArg(UnitEffect.EdgeGreen, "edge_green", percentGreen, percentGreen, true);
						}
						public static void SetBlue(double percentBlue)
						{
							SetShaderArg(UnitEffect.EdgeBlue, "edge_blue", percentBlue, percentBlue, true);
						}
						public static double GetRed()
						{
							return GetEffect(UnitEffect.EdgeRed);
						}
						public static double GetGreen()
						{
							return GetEffect(UnitEffect.EdgeGreen);
						}
						public static double GetBlue()
						{
							return GetEffect(UnitEffect.EdgeBlue);
						}
					}

				}
				public static class Pixelate
				{
					public static class Opacity
					{
						public static void Set(double percentOpacity)
						{
							SetShaderArg(UnitEffect.Pixelate, "pixel_effect", percentOpacity, percentOpacity, true);
						}
						public static double Get()
						{
							return GetEffect(UnitEffect.Pixelate);
						}
					}
					public static class Strength
					{
						public static void Set(double percentStrength)
						{
							SetShaderArg(UnitEffect.PixelateThreshold, "pixel_threshold", percentStrength, percentStrength / 100, true);
						}
						public static double Get()
						{
							return GetEffect(UnitEffect.PixelateThreshold);
						}
					}
				}
				public static class Grid
				{
					public static class Opacity
					{
						public static void Set(double percentOpacityX, double percentOpacityY)
						{
							SetX(percentOpacityX);
							SetY(percentOpacityY);
						}
						public static void SetX(double percentOpacity)
						{
							SetShaderArg(UnitEffect.GridX, "grid_effect_x", percentOpacity, percentOpacity, true);
						}
						public static double GetX()
						{
							return GetEffect(UnitEffect.GridX);
						}
						public static void SetY(double percentOpacity)
						{
							SetShaderArg(UnitEffect.GridY, "grid_effect_y", percentOpacity, percentOpacity, true);
						}
						public static double GetY()
						{
							return GetEffect(UnitEffect.GridY);
						}
					}
					public static class CellSize
					{
						public static void Set(double width, double height)
						{
							SetX(width);
							SetY(height);
						}
						public static void SetX(double width)
						{
							SetShaderArg(UnitEffect.GridCellWidth, "grid_thickness_y", width, width * 2, false);
						}
						public static double GetX()
						{
							return GetEffect(UnitEffect.GridCellWidth);
						}
						public static void SetY(double height)
						{
							SetShaderArg(UnitEffect.GridCellHeight, "grid_thickness_x", height, height * 2, false);
						}
						public static double GetY()
						{
							return GetEffect(UnitEffect.GridCellHeight);
						}
					}
					public static class CellSpacing
					{
						public static void Set(double spacingX, double spacingY)
						{
							SetX(spacingX);
							SetY(spacingY);
						}
						public static void SetX(double spacing)
						{
							SetShaderArg(UnitEffect.GridCellSpacingX, "grid_spacing_y", spacing, spacing / 5, false);
						}
						public static double GetX()
						{
							return GetEffect(UnitEffect.GridCellSpacingX);
						}
						public static void SetY(double spacing)
						{
							SetShaderArg(UnitEffect.GridCellSpacingY, "grid_spacing_x", spacing, spacing / 5, false);
						}
						public static double GetY()
						{
							return GetEffect(UnitEffect.GridCellSpacingY);
						}
					}
					public static class Color
					{
						public static void Set(double percentXRed, double percentXGreen, double percentXBlue,
							double percentYRed, double percentYGreen, double percentYBlue)
						{
							SetX(percentXRed, percentXGreen, percentXBlue);
							SetY(percentYRed, percentYGreen, percentYBlue);
						}
						public static void SetToSample(Simple.Color colorX, Simple.Color colorY)
						{
							SetXToSample(colorX);
							SetYToSample(colorY);
						}
						public static void SetX(double percentRed, double percentGreen, double percentBlue)
						{
							SetXRed(percentRed);
							SetXGreen(percentGreen);
							SetXBlue(percentBlue);
						}
						public static void SetXToSample(Simple.Color color)
						{
							var c = Statics.GetColorFromSample(color);
							SetX(
								Number.Converted.To.Percent.Get(c.R, 0, 255),
								Number.Converted.To.Percent.Get(c.G, 0, 255),
								Number.Converted.To.Percent.Get(c.B, 0, 255));
						}
						public static void SetXRed(double percentRed)
						{
							SetShaderArg(UnitEffect.GridRedX, "grid_y_red", percentRed, percentRed, true);
						}
						public static void SetXGreen(double percentGreen)
						{
							SetShaderArg(UnitEffect.GridGreenX, "grid_y_green", percentGreen, percentGreen, true);
						}
						public static void SetXBlue(double percentBlue)
						{
							SetShaderArg(UnitEffect.GridBlueX, "grid_y_blue", percentBlue, percentBlue, true);
						}
						public static double GetXRed()
						{
							return GetEffect(UnitEffect.GridRedX);
						}
						public static double GetXGreen()
						{
							return GetEffect(UnitEffect.GridGreenX);
						}
						public static double GetXBlue()
						{
							return GetEffect(UnitEffect.GridBlueX);
						}
						public static void SetY(double percentRed, double percentGreen, double percentBlue)
						{
							SetYRed(percentRed);
							SetYGreen(percentGreen);
							SetYBlue(percentBlue);
						}
						public static void SetYToSample(Simple.Color color)
						{
							var c = Statics.GetColorFromSample(color);
							SetY(
								Number.Converted.To.Percent.Get(c.R, 0, 255),
								Number.Converted.To.Percent.Get(c.G, 0, 255),
								Number.Converted.To.Percent.Get(c.B, 0, 255));
						}
						public static void SetYRed(double percentRed)
						{
							SetShaderArg(UnitEffect.GridRedY, "grid_x_red", percentRed, percentRed, true);
						}
						public static void SetYGreen(double percentGreen)
						{
							SetShaderArg(UnitEffect.GridGreenY, "grid_x_green", percentGreen, percentGreen, true);
						}
						public static void SetYBlue(double percentBlue)
						{
							SetShaderArg(UnitEffect.GridBlueY, "grid_x_blue", percentBlue, percentBlue, true);
						}
						public static double GetYRed()
						{
							return GetEffect(UnitEffect.GridRedY);
						}
						public static double GetYGreen()
						{
							return GetEffect(UnitEffect.GridGreenY);
						}
						public static double GetYBlue()
						{
							return GetEffect(UnitEffect.GridBlueY);
						}
					}
				}
				public static class Wind
				{
					public static class Strength
					{
						public static void Set(double strengthX, double strengthY)
						{
							SetX(strengthX);
							SetY(strengthY);
						}
						public static void SetX(double strength)
						{
							SetShaderArg(UnitEffect.WindX, "wind_strength_x", strength, strength / 5, false);
						}
						public static void SetY(double strength)
						{
							SetShaderArg(UnitEffect.WindY, "wind_strength_y", strength, strength / 5, false);
						}
						public static double GetX()
						{
							return GetEffect(UnitEffect.WindX);
						}
						public static double GetY()
						{
							return GetEffect(UnitEffect.WindY);
						}
					}
					public static class Speed
					{
						public static void Set(double speedX, double speedY)
						{
							SetX(speedX);
							SetY(speedY);
						}
						public static void SetX(double speed)
						{
							SetShaderArg(UnitEffect.WindSpeedX, "wind_speed_x", speed, speed / 8, false);
						}
						public static void SetY(double speed)
						{
							SetShaderArg(UnitEffect.WindSpeedY, "wind_speed_y", speed, speed / 8, false);
						}
						public static double GetX()
						{
							return GetEffect(UnitEffect.WindSpeedX);
						}
						public static double GetY()
						{
							return GetEffect(UnitEffect.WindSpeedY);
						}
					}
				}
				public static class Vibrate
				{
					public static class Strength
					{
						public static void Set(double strengthX, double strengthY)
						{
							SetX(strengthX);
							SetY(strengthY);
						}
						public static void SetX(double strength)
						{
							SetShaderArg(UnitEffect.VibrateX, "vibrate_strength_x", strength, strength / 10, false);
						}
						public static void SetY(double strength)
						{
							SetShaderArg(UnitEffect.VibrateY, "vibrate_strength_y", strength, strength / 10, false);
						}
						public static double GetX()
						{
							return GetEffect(UnitEffect.VibrateX);
						}
						public static double GetY()
						{
							return GetEffect(UnitEffect.VibrateY);
						}
					}
				}
				public static class Wave
				{
					public static class Strength
					{
						public static void Set(double strengthCosX, double strengthCosY, double strengthSinX, double strengthSinY)
						{
							SetCos(strengthCosX, strengthCosY);
							SetSin(strengthSinX, strengthSinY);
						}
						public static void SetCos(double strengthX, double strengthY)
						{
							SetCosX(strengthX);
							SetCosY(strengthY);
						}
						public static void SetCosX(double strength)
						{
							SetShaderArg(UnitEffect.WaveCosX, "cos_strength_x", strength, strength, false);
						}
						public static void SetCosY(double strength)
						{
							SetShaderArg(UnitEffect.WaveCosY, "cos_strength_y", strength, strength, false);
						}
						public static double GetCosX()
						{
							return GetEffect(UnitEffect.WaveCosX);
						}
						public static double GetCosY()
						{
							return GetEffect(UnitEffect.WaveCosY);
						}
						public static void SetSin(double strengthX, double strengthY)
						{
							SetSinX(strengthX);
							SetSinY(strengthY);
						}
						public static void SetSinX(double strength)
						{
							SetShaderArg(UnitEffect.WaveSinX, "sin_strength_x", strength, strength, false);
						}
						public static void SetSinY(double strength)
						{
							SetShaderArg(UnitEffect.WaveSinY, "sin_strength_y", strength, strength, false);
						}
						public static double GetSinX()
						{
							return GetEffect(UnitEffect.WaveSinX);
						}
						public static double GetSinY()
						{
							return GetEffect(UnitEffect.WaveSinY);
						}
					}
					public static class Speed
					{
						public static void Set(double speedCosX, double speedCosY, double speedSinX, double speedSinY)
						{
							SetCos(speedCosX, speedCosY);
							SetSin(speedSinX, speedSinY);
						}
						public static void SetCos(double speedX, double speedY)
						{
							SetCosX(speedX);
							SetCosY(speedY);
						}
						public static void SetCosX(double speed)
						{
							SetShaderArg(UnitEffect.WaveCosSpeedX, "cos_speed_x", speed, speed / 10, false);
						}
						public static void SetCosY(double speed)
						{
							SetShaderArg(UnitEffect.WaveCosSpeedY, "cos_speed_y", speed, speed / 10, false);
						}
						public static double GetCosX()
						{
							return GetEffect(UnitEffect.WaveCosSpeedX);
						}
						public static double GetCosY()
						{
							return GetEffect(UnitEffect.WaveCosSpeedY);
						}
						public static void SetSin(double speedX, double speedY)
						{
							SetSinX(speedX);
							SetSinY(speedY);
						}
						public static void SetSinX(double speed)
						{
							SetShaderArg(UnitEffect.WaveSinSpeedX, "sin_speed_x", speed, speed / 10, false);
						}
						public static void SetSinY(double speed)
						{
							SetShaderArg(UnitEffect.WaveSinSpeedY, "sin_speed_y", speed, speed / 10, false);
						}
						public static double GetSinX()
						{
							return GetEffect(UnitEffect.WaveSinSpeedX);
						}
						public static double GetSinY()
						{
							return GetEffect(UnitEffect.WaveSinSpeedY);
						}
					}
				}
			}
			public static class Motion
			{
				public static class Move
				{
					public static class Constantly
					{
						public static class AtAngle
						{
							public static void Do(double angle, double speed, bool relativeToParemt,
								Simple.Motion motion = Simple.Motion.PerSecond)
							{
								if (Statics.NoIDPickedError()) return;

								// for all transform parent dependent components:
								// create the object since this can be the first Set method called on this object
								// and we open with Parent.Get() that throws object not found error
								//ExistanceCheck();

								var direction = Statics.AngleToVec(angle);
								MoveInDirection(direction, speed, relativeToParemt, motion);
							}
						}
						public static class InDirection
						{
							public static void Do(Direction direction, double speed, bool relativeToParemt,
								Simple.Motion motion = Simple.Motion.PerSecond)
							{
								if (Statics.NoIDPickedError()) return;

								// for all transform parent dependent components:
								// create the object since this can be the first Set method called on this object
								// and we open with Parent.Get() that throws object not found error
								ExistanceCheck();

								MoveInDirection(Statics.GetDirectionFromSample(direction), speed, relativeToParemt, motion);
							}
						}
					}
					public static class TowardPosition
					{
						public static void Do(double x, double y, float speed, bool relativeToParent,
							Simple.Motion motion = Simple.Motion.PerSecond)
						{
							DoX(x, speed, relativeToParent, motion);
							DoY(y, speed, relativeToParent, motion);
						}
						public static void DoX(double x, float speed, bool relativeToParent,
							Simple.Motion motion = Simple.Motion.PerSecond)
						{
							if (Statics.NoIDPickedError()) return;

							var testSpeed = speed;
							if (motion == Simple.Motion.PerSecond)
							{
								testSpeed *= tickDeltaTime.ElapsedTime.AsSeconds() * 2;
							}
							var picked = PickedIDs.Get();
							foreach (var ID in picked)
							{
								PickedIDs.Set(ID);

								// for all transform parent dependent components:
								// create the object since this can be the first Set method called on this object
								// and we open with Parent.Get() that throws object not found error
								ExistanceCheck();

								var pos = new Vector2f(
									(float)Area.Position.GetX(relativeToParent),
									(float)Area.Position.GetY(relativeToParent));
								var targetPos = new Vector2f((float)x, pos.Y);

								if (pos == targetPos) continue;

								var dir = Statics.Normalize(targetPos - pos);
								MoveInDirection(dir, speed, relativeToParent, motion);

								var dist = Statics.Distance(pos, targetPos);
								if (dist < testSpeed)
								{
									Area.Position.Set(targetPos.X, targetPos.Y, relativeToParent);
								}
							}
							PickedIDs.Set(picked);
						}
						public static void DoY(double y, float speed, bool relativeToParent,
							Simple.Motion motion = Simple.Motion.PerSecond)
						{
							if (Statics.NoIDPickedError()) return;

							var testSpeed = speed;
							if (motion == Simple.Motion.PerSecond)
							{
								testSpeed *= tickDeltaTime.ElapsedTime.AsSeconds() * 2;
							}
							var picked = PickedIDs.Get();
							foreach (var ID in picked)
							{
								PickedIDs.Set(ID);

								// for all transform parent dependent components:
								// create the object since this can be the first Set method called on this object
								// and we open with Parent.Get() that throws object not found error
								ExistanceCheck();

								var pos = new Vector2f(
									(float)Area.Position.GetX(relativeToParent),
									(float)Area.Position.GetY(relativeToParent));
								var targetPos = new Vector2f(pos.X, (float)y);

								if (pos == targetPos) continue;

								var dir = Statics.Normalize(targetPos - pos);
								MoveInDirection(dir, speed, relativeToParent, motion);

								var dist = Statics.Distance(pos, targetPos);
								if (dist < testSpeed)
								{
									Area.Position.Set(targetPos.X, targetPos.Y, relativeToParent);
								}
							}
							PickedIDs.Set(picked);
						}
					}
					public static class TowardUnit
					{
						public static void Do(string unitID, float speed, bool relativeToParent,
							Simple.Motion motion = Simple.Motion.PerSecond)
						{
							if (Statics.NoIDPickedError() || Statics.DoesNotExistError(data.objects, "Unit")) return;

							var picked = PickedIDs.Get();
							PickedIDs.Set(unitID);
							if (Statics.DoesNotExistError(data.objects, "Unit"))
							{
								PickedIDs.Set(picked);
								return;
							}
							var targetPos = new Vector2f(
								(float)Area.Position.GetX(relativeToParent),
								(float)Area.Position.GetY(relativeToParent));
							PickedIDs.Set(picked);

							TowardPosition.Do(targetPos.X, targetPos.Y, speed, relativeToParent, motion);

						}
					}
				}
				public static class Rotate
				{
					public static class Constantly
					{
						public static void Do(float speed, Simple.Motion motion = Simple.Motion.PerSecond)
						{
							if (Statics.NoIDPickedError()) return;

							var picked = PickedIDs.Get();
							foreach (var ID in picked)
							{
								PickedIDs.Set(ID);

								// for all transform parent dependent components:
								// create the object since this can be the first Set method called on this object
								// and we open with Parent.Get() that throws object not found error
								ExistanceCheck();

								var ang = Area.Angle.Get(true);
								var newAng = Number.Changed.Constantly.Get(ang, speed, motion);
								Area.Angle.Set(newAng, true);
							}
							PickedIDs.Set(picked);
						}
					}
					public static class TowardAngle
					{
						public static void Do(double angle, float speed, bool relativeToParent,
							Simple.Motion motion = Simple.Motion.PerSecond)
						{
							if (Statics.NoIDPickedError()) return;

							var testSpeed = speed;
							if (motion == Simple.Motion.PerSecond)
							{
								testSpeed *= tickDeltaTime.ElapsedTime.AsSeconds() * 2;
							}
							var picked = PickedIDs.Get();
							foreach (var ID in picked)
							{
								PickedIDs.Set(ID);

								// for all transform parent dependent components:
								// create the object since this can be the first Set method called on this object
								// and we open with Parent.Get() that throws object not found error
								ExistanceCheck();

								speed = Math.Abs(speed);
								var ang = Area.Angle.Get(relativeToParent);
								var diff = ang - angle;

								// stops the rotation with an else when close enough
								// prevents the rotation from staying behind after the stop

								if (Math.Abs(diff) < testSpeed) Area.Angle.Set(angle, relativeToParent);
								else if (diff > 0 && diff < 180) Constantly.Do(-speed, motion);
								else if (diff > -180 && diff < 0) Constantly.Do(speed, motion);
								else if (diff > -360 && diff < -180) Constantly.Do(-speed, motion);
								else if (diff > 180 && diff < 360) Constantly.Do(speed, motion);

								// detects speed greater than possible
								// prevents jiggle when passing 0-360 & 360-0 | simple to fix yet took me half a day
								if (Math.Abs(diff) > 360 - testSpeed) Area.Angle.Set(angle, relativeToParent);
							}
							PickedIDs.Set(picked);
						}
					}
					public static class TowardUnit
					{
						public static void Do(string unitID, float speed,
							Simple.Motion motion = Simple.Motion.PerSecond)
						{
							if (Statics.NoIDPickedError() || Statics.DoesNotExistError(data.objects, "Unit")) return;

							var picked = PickedIDs.Get();
							PickedIDs.Set(unitID);
							if (Statics.DoesNotExistError(data.objects, "Unit"))
							{
								PickedIDs.Set(picked);
								return;
							}

							PickedIDs.Set(picked);
							foreach (var ID in picked)
							{
								PickedIDs.Set(ID);
								var targetAng = ToUnit.Angle.Get(unitID);

								TowardAngle.Do(targetAng, speed, false, motion);
							}
							PickedIDs.Set(picked);
						}
					}
				}
				public static class Rescale
				{

				}
			}
			public static class Percent
			{
				public static class ToUnit
				{
					public static class Position
					{
						public static void Set(string unitID, double percent)
						{
							if (Statics.NoIDPickedError()) return;

							var picked = PickedIDs.Get();
							PickedIDs.Set(unitID);
							var targetPos = new Vector2f((float)Area.Position.GetX(false), (float)Area.Position.GetY(false));
							PickedIDs.Set(picked);

							ToPosition.Position.Set(targetPos.X, targetPos.Y, percent);
						}
					}
					public static class Angle
					{
						public static void Set(string unitID, double percent)
						{
							if (Statics.NoIDPickedError()) return;

							// for all transform parent dependent components:
							// create the object since this can be the first Set method called on this object
							// and we open with Parent.Get() that throws object not found error
							ExistanceCheck();

							var picked = PickedIDs.Get();
							PickedIDs.Set(unitID);
							var targetAng = Area.Angle.Get(false);
							PickedIDs.Set(picked);

							ToAngle.Set(targetAng, percent);
						}
					}
					public static class Scale
					{
						public static void Set(string unitID, double percent)
						{
							if (Statics.NoIDPickedError()) return;

							// for all transform parent dependent components:
							// create the object since this can be the first Set method called on this object
							// and we open with Parent.Get() that throws object not found error
							ExistanceCheck();

							var picked = PickedIDs.Get();
							PickedIDs.Set(unitID);
							var targetSc = new Vector2f((float)Area.Scale.GetWidth(false), (float)Area.Scale.GetHeight(false));
							PickedIDs.Set(picked);

							ToScale.Set(targetSc.X, targetSc.Y, percent);
						}
					}
				}
				public static class ToPosition
				{
					public static class Position
					{
						public static void Set(double x, double y, double percent)
						{
							if (Statics.NoIDPickedError()) return;

							// for all transform parent dependent components:
							// create the object since this can be the first Set method called on this object
							// and we open with Parent.Get() that throws object not found error
							ExistanceCheck();

							var picked = PickedIDs.Get();
							var targetPos = new Vector2f((float)x, (float)y);

							foreach (var objectID in picked)
							{
								PickedIDs.Set(objectID);
								if (ExistanceCheck()) continue;

								var pos = new Vector2f((float)Area.Position.GetX(false), (float)Area.Position.GetY(false));
								pos.X = (float)Number.Converted.From.Percent.Get(percent, pos.X, targetPos.X);
								pos.Y = (float)Number.Converted.From.Percent.Get(percent, pos.Y, targetPos.Y);

								Area.Position.Set(pos.X, pos.Y, false);
							}
							PickedIDs.Set(picked);
						}
					}
				}
				public static class ToAngle
				{
					public static void Set(double angle, double percent)
					{
						if (Statics.NoIDPickedError()) return;

						// for all transform parent dependent components:
						// create the object since this can be the first Set method called on this object
						// and we open with Parent.Get() that throws object not found error
						ExistanceCheck();

						var picked = PickedIDs.Get();
						foreach (var objectID in picked)
						{
							PickedIDs.Set(objectID);
							if (ExistanceCheck()) continue;

							var ang = Area.Angle.Get(false);
							ang = (float)Number.Converted.From.Percent.Get(percent, ang, angle);

							Area.Angle.Set(ang, false);
						}
						PickedIDs.Set(picked);
					}
				}
				public static class ToScale
				{
					public static void Set(double scaleWidth, double scaleHeight, double percent)
					{
						if (Statics.NoIDPickedError()) return;

						// for all transform parent dependent components:
						// create the object since this can be the first Set method called on this object
						// and we open with Parent.Get() that throws object not found error
						ExistanceCheck();

						var picked = PickedIDs.Get();
						var targetSc = new Vector2f((float)scaleWidth, (float)scaleHeight);

						foreach (var objectID in picked)
						{
							PickedIDs.Set(objectID);
							if (ExistanceCheck()) continue;

							var sc = new Vector2f((float)Area.Scale.GetWidth(false), (float)Area.Scale.GetHeight(false));
							sc.X = (float)Number.Converted.From.Percent.Get(percent, sc.X, targetSc.X);
							sc.Y = (float)Number.Converted.From.Percent.Get(percent, sc.Y, targetSc.Y);

							Area.Position.Set(sc.X, sc.Y, false);
						}
						PickedIDs.Set(picked);
					}
				}
			}
			public static class ToUnit
			{
				public static class Distance
				{
					public static double Get(string unitID)
					{
						if (Statics.NoIDPickedError() || Statics.MultipleIDsPickedError() ||
							Statics.DoesNotExistError(data.objects, "Unit")) return default;

						var picked = PickedIDs.Get();
						PickedIDs.Set(unitID);
						if (Statics.DoesNotExistError(data.objects, "Unit")) return default;
						var targetPos = new Vector2f((float)Area.Position.GetX(false), (float)Area.Position.GetY(false));

						PickedIDs.Set(picked);
						var pos = new Vector2f((float)Area.Position.GetX(false), (float)Area.Position.GetY(false));

						return Statics.Distance(pos, targetPos);
					}
				}
				public static class Angle
				{
					public static double Get(string unitID)
					{
						if (Statics.NoIDPickedError() || Statics.MultipleIDsPickedError() ||
							Statics.DoesNotExistError(data.objects, "Unit")) return default;

						var picked = PickedIDs.Get();
						PickedIDs.Set(unitID);
						if (Statics.DoesNotExistError(data.objects, "Unit")) return default;
						var targetPos = new Vector2f((float)Area.Position.GetX(false), (float)Area.Position.GetY(false));

						PickedIDs.Set(picked);
						var pos = new Vector2f((float)Area.Position.GetX(false), (float)Area.Position.GetY(false));

						var dir = Statics.Normalize(targetPos - pos);
						
						return Statics.VecToAngle(dir);
					}
				}
				public static class NearestIDs
				{
					public static string[] Get(params string[] fromUnitIDs)
					{
						if (Statics.NoIDPickedError() || Statics.MultipleIDsPickedError()) return default;

						var result = new SortedDictionary<double, string>();
						var picked = PickedIDs.Get();
						foreach (var ID in fromUnitIDs)
						{
							PickedIDs.Set(ID);
							if (Statics.DoesNotExistError(data.objects, "Unit")) continue;
							result[Distance.Get(picked[0])] = ID;
						}
						return result.Values.ToArray();
					}
				}
			}
			public static class ToGrid
			{
				public static class Position
				{
					public static void Set(double cellWidth, double cellHeight)
					{
						if (Statics.NoIDPickedError()) return;

						// for all transform parent dependent components:
						// create the object since this can be the first Set method called on this object
						// and we open with Parent.Get() that throws object not found error
						//ExistanceCheck();

						var w = Number.Limited.Get(cellWidth, 0, view.Size.X);
						var h = Number.Limited.Get(cellHeight, 0, view.Size.Y);

						var picked = PickedIDs.Get();
						foreach (var objectID in picked)
						{
							PickedIDs.Set(objectID);
							if (ExistanceCheck()) continue;

							var pos = new Vector2f((float)Area.Position.GetX(false), (float)Area.Position.GetY(false));

							if (cellWidth > 0) Area.Position.SetX(w * (float)Math.Round(pos.X / w), false);
							if (cellHeight > 0) Area.Position.SetY(h * (float)Math.Round(pos.Y / h), false);
						}
						PickedIDs.Set(picked);
					}
				}
				public static class Angle
				{
					public static void Set(double cellSize)
					{
						if (Statics.NoIDPickedError()) return;

						// for all transform parent dependent components:
						// create the object since this can be the first Set method called on this object
						// and we open with Parent.Get() that throws object not found error
						//ExistanceCheck();

						var a = Number.Limited.Get(cellSize, 0, 360);

						var picked = PickedIDs.Get();
						foreach (var objectID in picked)
						{
							PickedIDs.Set(objectID);
							if (ExistanceCheck()) continue;

							var ang = Area.Angle.Get(false);

							if (cellSize > 0) Area.Angle.Set(a * (float)Math.Round(ang / a), false);
						}
						PickedIDs.Set(picked);
					}
				}
				public static class Scale
				{
					public static void Set(double cellWidth, double cellHeight)
					{
						if (Statics.NoIDPickedError()) return;

						// for all transform parent dependent components:
						// create the object since this can be the first Set method called on this object
						// and we open with Parent.Get() that throws object not found error
						//ExistanceCheck();

						var w = Number.Limited.Get(cellWidth, 0, view.Size.X);
						var h = Number.Limited.Get(cellHeight, 0, view.Size.Y);

						var picked = PickedIDs.Get();
						foreach (var objectID in picked)
						{
							PickedIDs.Set(objectID);
							if (ExistanceCheck()) continue;

							var sc = new Vector2f((float)Area.Scale.GetWidth(false), (float)Area.Scale.GetHeight(false));

							if (cellWidth > 0) Area.Position.SetX(w * (float)Math.Round(sc.X / w), false);
							if (cellHeight > 0) Area.Position.SetY(h * (float)Math.Round(sc.Y / h), false);
						}
						PickedIDs.Set(picked);
					}
				}
			}
			public static class Area
			{
				public static class Position
				{
					public static void Set(double x, double y, bool relativeToParent)
					{
						SetX(x, relativeToParent);
						SetY(y, relativeToParent);
					}
					public static void SetX(double x, bool relativeToParent)
					{
						if (Statics.NoIDPickedError()) return;

						// for all transform parent dependent components:
						// create the object since this can be the first Set method called on this object
						// and we open with Parent.Get() that throws object not found error
						ExistanceCheck();

						var picked = PickedIDs.Get();
						var parentID = Identity.FamilyIDs.Parent.Get();
						if (relativeToParent == false) Identity.FamilyIDs.Parent.Set(null);

						foreach (var objectID in picked)
						{
							PickedIDs.Set(objectID);
							if (ExistanceCheck()) continue;

							var pos = new Vector2f((float)x, data.objects[objectID].sprite.Position.Y);

							data.objects[objectID].sprite.Position = pos;
							data.objects[objectID].numbers[UnitNumber.LocalX] = pos.X;
							if (Identity.FamilyIDs.Parent.Get() == null) data.objects[objectID].numbers[UnitNumber.GlobalX] = pos.X;

							data.objects[objectID].UpdateChildrenGlobalAngle();
							data.objects[objectID].UpdateChildrenGlobalPos();
							render = true; // TODO validate
						}
						if (relativeToParent == false) Identity.FamilyIDs.Parent.Set(parentID);
						PickedIDs.Set(picked);
					}
					public static void SetY(double y, bool relativeToParent)
					{
						if (Statics.NoIDPickedError()) return;

						// for all transform parent dependent components:
						// create the object since this can be the first Set method called on this object
						// and we open with Parent.Get() that throws object not found error
						ExistanceCheck();

						var picked = PickedIDs.Get();
						var parentID = Identity.FamilyIDs.Parent.Get();
						if (relativeToParent == false) Identity.FamilyIDs.Parent.Set(null);

						foreach (var objectID in picked)
						{
							PickedIDs.Set(objectID);
							if (ExistanceCheck()) continue;

							var pos = new Vector2f(data.objects[objectID].sprite.Position.X, (float)y);

							data.objects[objectID].sprite.Position = pos;
							data.objects[objectID].numbers[UnitNumber.LocalY] = pos.Y;
							if (Identity.FamilyIDs.Parent.Get() == null) data.objects[objectID].numbers[UnitNumber.GlobalY] = pos.Y;

							data.objects[objectID].UpdateChildrenGlobalAngle();
							data.objects[objectID].UpdateChildrenGlobalPos();
							render = true; // TODO validate
						}
						if (relativeToParent == false) Identity.FamilyIDs.Parent.Set(parentID);
						PickedIDs.Set(picked);
					}
					public static double GetX(bool relativeToParent)
					{
						if (Statics.NoIDPickedError() || Statics.MultipleIDsPickedError() ||
							Statics.DoesNotExistError(data.objects, "Unit")) return default;

						data.objects[PickedIDs.Get()[0]].ApplyParallax(true);
						var result = GetNumber(relativeToParent ? UnitNumber.LocalX : UnitNumber.GlobalX);
						data.objects[PickedIDs.Get()[0]].ApplyParallax(false);
						return result;
					}
					public static double GetY(bool relativeToParent)
					{
						if (Statics.NoIDPickedError() || Statics.MultipleIDsPickedError() ||
							Statics.DoesNotExistError(data.objects, "Unit")) return default;

						data.objects[PickedIDs.Get()[0]].ApplyParallax(true);
						var result = GetNumber(relativeToParent ? UnitNumber.LocalY : UnitNumber.GlobalY);
						data.objects[PickedIDs.Get()[0]].ApplyParallax(false);
						return result;
					}
				}
				public static class Angle
				{
					public static void Set(double angle, bool relativeToParent)
					{
						if (Statics.NoIDPickedError()) return;

						// for all transform parent dependent components:
						// create the object since this can be the first Set method called on this object
						// and we open with Parent.Get() that throws object not found error
						ExistanceCheck();

						var picked = PickedIDs.Get();
						var parentID = Identity.FamilyIDs.Parent.Get();
						if (relativeToParent == false) Identity.FamilyIDs.Parent.Set(null);

						foreach (var objectID in picked)
						{
							PickedIDs.Set(objectID);
							if (ExistanceCheck()) continue;

							var a = (float)Number.Limited.Get(angle, 0, 359, NumberLimitation.Overflow);
							var parAng = Identity.FamilyIDs.Parent.Get() == null ? world.Rotation :
								data.objects[Identity.FamilyIDs.Parent.Get()].sprite.Rotation;
							var global = Number.Limited.Get(parAng + a, 0, 359, NumberLimitation.Overflow);

							data.objects[objectID].sprite.Rotation = a;
							data.objects[objectID].numbers[UnitNumber.LocalAngle] = a;
							data.objects[objectID].numbers[UnitNumber.GlobalAngle] = global;

							data.objects[objectID].UpdateChildrenGlobalAngle();
							data.objects[objectID].UpdateChildrenGlobalPos();
							render = true; // TODO validate
						}
						if (relativeToParent == false) Identity.FamilyIDs.Parent.Set(parentID);
						PickedIDs.Set(picked);
					}
					public static double Get(bool relativeToParent)
					{
						if (Statics.NoIDPickedError() || Statics.MultipleIDsPickedError() ||
							Statics.DoesNotExistError(data.objects, "Unit")) return default;

						data.objects[PickedIDs.Get()[0]].ApplyParallax(true);
						var result = GetNumber(relativeToParent ? UnitNumber.LocalAngle : UnitNumber.GlobalAngle);
						data.objects[PickedIDs.Get()[0]].ApplyParallax(false);
						return result;
					}
				}
				public static class Scale
				{
					public static void Set(double scaleWidth, double scaleHeight, bool relativeToParent)
					{
						SetWidth(scaleWidth, relativeToParent);
						SetHeight(scaleHeight, relativeToParent);
					}
					public static void SetWidth(double scaleWidth, bool relativeToParent)
					{
						if (Statics.NoIDPickedError()) return;

						// for all transform parent dependent components:
						// create the object since this can be the first Set method called on this object
						// and we open with Parent.Get() that throws object not found error
						ExistanceCheck();

						var picked = PickedIDs.Get();
						var parentID = Identity.FamilyIDs.Parent.Get();
						if (relativeToParent == false) Identity.FamilyIDs.Parent.Set(null);

						foreach (var objectID in picked)
						{
							PickedIDs.Set(objectID);
							if (ExistanceCheck()) continue;

							var parSc = Identity.FamilyIDs.Parent.Get() == null ? world.Scale : data.objects[Identity.FamilyIDs.Parent.Get()].sprite.Scale;
							var scale = new Vector2f((float)scaleWidth, data.objects[objectID].sprite.Scale.Y);
							var sc = new Vector2f(parSc.X * scale.X, parSc.Y * scale.Y);

							data.objects[objectID].sprite.Scale = scale;
							data.objects[objectID].numbers[UnitNumber.LocalScaleWidth] = scale.X;
							data.objects[objectID].numbers[UnitNumber.GlobalScaleWidth] = sc.X;

							data.objects[objectID].UpdateChildrenGlobalScale();
							data.objects[objectID].UpdateChildrenGlobalPos();
							render = true; // TODO validate
						}
						if (relativeToParent == false) Identity.FamilyIDs.Parent.Set(parentID);
						PickedIDs.Set(picked);
					}
					public static void SetHeight(double scaleHeight, bool relativeToParent)
					{
						if (Statics.NoIDPickedError()) return;

						// for all transform parent dependent components:
						// create the object since this can be the first Set method called on this object
						// and we open with Parent.Get() that throws object not found error
						ExistanceCheck();

						var picked = PickedIDs.Get();
						var parentID = Identity.FamilyIDs.Parent.Get();
						if (relativeToParent == false) Identity.FamilyIDs.Parent.Set(null);

						foreach (var objectID in picked)
						{
							PickedIDs.Set(objectID);
							if (ExistanceCheck()) continue;

							var parSc = Identity.FamilyIDs.Parent.Get() == null ? world.Scale :
								data.objects[Identity.FamilyIDs.Parent.Get()].sprite.Scale;
							var scale = new Vector2f(data.objects[objectID].sprite.Scale.X, (float)scaleHeight);
							var sc = new Vector2f(parSc.X * scale.X, parSc.Y * scale.Y);

							data.objects[objectID].sprite.Scale = scale;
							data.objects[objectID].numbers[UnitNumber.LocalScaleHeight] = scale.Y;
							data.objects[objectID].numbers[UnitNumber.GlobalScaleHeight] = sc.Y;

							data.objects[objectID].UpdateChildrenGlobalScale();
							data.objects[objectID].UpdateChildrenGlobalPos();
							render = true; // TODO validate
						}
						if (relativeToParent == false) Identity.FamilyIDs.Parent.Set(parentID);
						PickedIDs.Set(picked);
					}
					public static double GetWidth(bool relativeToParent)
					{
						if (Statics.NoIDPickedError() || Statics.MultipleIDsPickedError() ||
							Statics.DoesNotExistError(data.objects, "Unit")) return default;

						data.objects[PickedIDs.Get()[0]].ApplyParallax(true);
						var result = GetNumber(relativeToParent ? UnitNumber.LocalScaleWidth : UnitNumber.GlobalScaleWidth);
						data.objects[PickedIDs.Get()[0]].ApplyParallax(false);
						return result == 0 ? 1 : result;
					}
					public static double GetHeight(bool relativeToParent)
					{
						if (Statics.NoIDPickedError() || Statics.MultipleIDsPickedError() ||
							Statics.DoesNotExistError(data.objects, "Unit")) return default;

						data.objects[PickedIDs.Get()[0]].ApplyParallax(true);
						var result = GetNumber(relativeToParent ? UnitNumber.LocalScaleHeight : UnitNumber.GlobalScaleHeight);
						data.objects[PickedIDs.Get()[0]].ApplyParallax(false);
						return result == 0 ? 1 : result;
					}
				}
			}
			public static class Text
			{
				public static class Display
				{
					public static void Set(string text)
					{
						if (Statics.NoIDPickedError()) return;

						var picked = PickedIDs.Get();
						foreach (var objectID in picked)
						{
							PickedIDs.Set(objectID);
							if (ExistanceCheck()) continue;

							data.objects[objectID].text.DisplayedString = text;
							data.objects[objectID].texts[UnitText.Text] = text;
							var fontPath = FontFilePath.Get();
							if (fontPath != null)
							{
								var t = data.objects[objectID].text;
								// offset left side to prevent cutting the text outline (changing position before drawing doesn't work?)
								var widestLineIndex = 0;
								var maxLineChars = 0;
								var chars = 0;
								for (int i = 0; i < t.DisplayedString.Length; i++)
								{
									var c = t.DisplayedString[i];
									chars++;
									if (c == '\n' || i == t.DisplayedString.Length - 1)
									{
										if (maxLineChars <= chars)
										{
											widestLineIndex = c == '\n' ? i : i + 1;
											maxLineChars = chars;
										}
										chars = 0;
									}
								}

								var w = t.FindCharacterPos((uint)widestLineIndex).X;

								var key = "asjlkl;qwer";
								var oldText = t.DisplayedString;
								t.DisplayedString += $"\n{key}";
								var index = (uint)t.DisplayedString.IndexOf(key);
								var h = t.FindCharacterPos(index).Y;
								var scW = Area.Scale.GetWidth(false);

								t.DisplayedString = oldText;

								SetTexture(objectID, null, new SFML.Graphics.Texture(
									(uint)(w + (t.OutlineThickness + t.CharacterSize * 0.8f) * scW),
									(uint)(h + t.OutlineThickness)));

								// update origin according to how many times the texture is repeated
								Appearance.Origin.Set(Appearance.Origin.GetX(), Appearance.Origin.GetY());
							}

							render = true;
						}
						PickedIDs.Set(picked);
					}
					public static string Get()
					{
						return GetText(UnitText.Text);
					}
				}
				public static class FontFilePath
				{
					public static void Set(string fontPath)
					{
						if (Statics.NoIDPickedError()) return;

						var picked = PickedIDs.Get();
						foreach (var objectID in picked)
						{
							PickedIDs.Set(objectID);
							if (ExistanceCheck()) continue;

							data.objects[objectID].text.Font = fonts[fontPath]; // TODO validate
							data.objects[objectID].texts[UnitText.FontPath] = fontPath;
							if (data.objects[objectID].texts.ContainsKey(UnitText.Text))
							{
								Display.Set(data.objects[objectID].texts[UnitText.Text]);
							}

							// update origin according to how many times the texture is repeated
							Appearance.Origin.Set(Appearance.Origin.GetX(), Appearance.Origin.GetY());

							render = true; // TODO validate
						}
						PickedIDs.Set(picked);
					}
					public static string Get()
					{
						return GetText(UnitText.FontPath);
					}
				}
				public static class Style
				{
					public static void Set(TextStyle style)
					{
						if (Statics.NoIDPickedError()) return;

						var picked = PickedIDs.Get();
						foreach (var objectID in picked)
						{
							PickedIDs.Set(objectID);
							if (ExistanceCheck()) continue;

							var st = SFML.Graphics.Text.Styles.Regular;
							switch (style)
							{
								case TextStyle.Bold: st = SFML.Graphics.Text.Styles.Bold; break;
								case TextStyle.Italic: st = SFML.Graphics.Text.Styles.Italic; break;
								case TextStyle.Strikethrough: st = SFML.Graphics.Text.Styles.StrikeThrough; break;
								case TextStyle.Underline: st = SFML.Graphics.Text.Styles.Underlined; break;
							}
							data.objects[objectID].numbers[UnitNumber.TextStyle] = (double)style;
							data.objects[objectID].text.Style = st; // TODO validate

							render = true; // TODO validate
						}
						PickedIDs.Set(picked);
					}
					public static TextStyle Get()
					{
						return (TextStyle)GetNumber(UnitNumber.TextStyle);
					}
				}
				public static class Outline
				{
					public static class Size
					{
						public static void Set(double size)
						{
							if (Statics.NoIDPickedError()) return;

							var picked = PickedIDs.Get();
							foreach (var objectID in picked)
							{
								PickedIDs.Set(objectID);
								if (ExistanceCheck()) continue;

								data.objects[objectID].numbers[UnitNumber.TextOutlineSize] = size;
								data.objects[objectID].text.OutlineThickness = (float)size; // TODO validate

								render = true; // TODO validate
							}
							PickedIDs.Set(picked);
						}
						public static double Get()
						{
							return GetNumber(UnitNumber.TextOutlineSize);
						}
					}
					public static class Color
					{
						public static void Set(double percentRed, double percentGreen, double percentBlue)
						{
							SetRed(percentRed);
							SetGreen(percentGreen);
							SetBlue(percentBlue);
						}
						public static void SetRed(double percentRed)
						{
							if (Statics.NoIDPickedError()) return;

							percentRed = Number.Limited.Get(percentRed, 0, 100);
							var p255 = Number.Converted.From.Percent.Get(percentRed, 0, 255);

							var picked = PickedIDs.Get();
							foreach (var objectID in picked)
							{
								PickedIDs.Set(objectID);
								if (ExistanceCheck()) continue;

								var c = data.objects[objectID].text.OutlineColor;
								data.objects[objectID].text.OutlineColor = new SFML.Graphics.Color((byte)p255, c.G, c.B, c.A);
								data.objects[objectID].numbers[UnitNumber.TextOutlineRed] = percentRed;
								render = true; // TODO validate
							}
							PickedIDs.Set(picked);
						}
						public static void SetGreen(double percentGreen)
						{
							if (Statics.NoIDPickedError()) return;

							percentGreen = Number.Limited.Get(percentGreen, 0, 100);
							var p255 = Number.Converted.From.Percent.Get(percentGreen, 0, 255);

							var picked = PickedIDs.Get();
							foreach (var objectID in picked)
							{
								PickedIDs.Set(objectID);
								if (ExistanceCheck()) continue;

								var c = data.objects[objectID].text.OutlineColor;
								data.objects[objectID].text.OutlineColor = new SFML.Graphics.Color(c.R, (byte)p255, c.B, c.A);
								data.objects[objectID].numbers[UnitNumber.TextOutlineGreen] = percentGreen;
								render = true; // TODO validate
							}
							PickedIDs.Set(picked);
						}
						public static void SetBlue(double percentBlue)
						{
							if (Statics.NoIDPickedError()) return;

							percentBlue = Number.Limited.Get(percentBlue, 0, 100);
							var p255 = Number.Converted.From.Percent.Get(percentBlue, 0, 255);

							var picked = PickedIDs.Get();
							foreach (var objectID in picked)
							{
								PickedIDs.Set(objectID);
								if (ExistanceCheck()) continue;

								var c = data.objects[objectID].text.OutlineColor;
								data.objects[objectID].text.OutlineColor = new SFML.Graphics.Color(c.R, c.G, (byte)p255, c.A);
								data.objects[objectID].numbers[UnitNumber.TextOutlineBlue] = percentBlue;
								render = true; // TODO validate
							}
							PickedIDs.Set(picked);
						}
						public static double GetRed()
						{
							return GetNumber(UnitNumber.TextOutlineRed);
						}
						public static double GetGreen()
						{
							return GetNumber(UnitNumber.TextOutlineGreen);
						}
						public static double GetBlue()
						{
							return GetNumber(UnitNumber.TextOutlineBlue);
						}
					}
					public static class Opacity
					{
						public static void Set(double percentOpacity)
						{
							if (Statics.NoIDPickedError()) return;

							percentOpacity = Number.Limited.Get(percentOpacity, 0, 100);
							var p255 = Number.Converted.From.Percent.Get(percentOpacity, 0, 255);

							var picked = PickedIDs.Get();
							foreach (var objectID in picked)
							{
								PickedIDs.Set(objectID);
								if (ExistanceCheck()) continue;

								var c = data.objects[objectID].text.OutlineColor;
								data.objects[objectID].text.OutlineColor = new SFML.Graphics.Color(c.R, c.G, c.B, (byte)p255);
								data.objects[objectID].numbers[UnitNumber.TextOutlineOpacity] = percentOpacity;
								render = true; // TODO validate
							}
							PickedIDs.Set(picked);
						}
						public static double Get()
						{
							return GetNumber(UnitNumber.TextOutlineOpacity);
						}
					}
				}
				public static class Spacing
				{
					public static class Line
					{
						public static void Set(double spacing)
						{
							if (Statics.NoIDPickedError()) return;

							var picked = PickedIDs.Get();
							foreach (var objectID in picked)
							{
								PickedIDs.Set(objectID);
								if (ExistanceCheck()) continue;

								data.objects[objectID].numbers[UnitNumber.TextSpacingLine] = spacing;
								data.objects[objectID].text.LineSpacing = (float)spacing; // TODO validate

								render = true; // TODO validate
							}
							PickedIDs.Set(picked);
						}
						public static double Get()
						{
							return GetNumber(UnitNumber.TextSpacingLine);
						}
					}
					public static class Letter
					{
						public static void Set(double spacing)
						{
							if (Statics.NoIDPickedError()) return;

							var picked = PickedIDs.Get();
							foreach (var objectID in picked)
							{
								PickedIDs.Set(objectID);
								if (ExistanceCheck()) continue;

								data.objects[objectID].numbers[UnitNumber.TextSpacingSymbol] = spacing;
								data.objects[objectID].text.LetterSpacing = (float)spacing; // TODO validate

								render = true; // TODO validate
							}
							PickedIDs.Set(picked);
						}
						public static double Get()
						{
							return GetNumber(UnitNumber.TextSpacingLine);
						}
					}
				}
				public static class Size
				{
					public static void Set(double spacing)
					{
						if (Statics.NoIDPickedError()) return;

						spacing = Number.Rounded.Get(spacing);

						var picked = PickedIDs.Get();
						foreach (var objectID in picked)
						{
							PickedIDs.Set(objectID);
							if (ExistanceCheck()) continue;

							data.objects[objectID].numbers[UnitNumber.TextCharacterSize] = spacing;
							data.objects[objectID].text.CharacterSize = (uint)spacing; // TODO validate

							render = true; // TODO validate
						}
						PickedIDs.Set(picked);
					}
					public static double Get()
					{
						return GetNumber(UnitNumber.TextCharacterSize);
					}
				}
			}
			public static class Existance
			{
				public static class Disabled
				{
					public static void Set(bool disabled)
					{
						if (Statics.NoIDPickedError()) return;

						var picked = PickedIDs.Get();
						foreach (var objectID in picked)
						{
							PickedIDs.Set(objectID);
							// no existance check since it checks wether the unit is disabled or not => will never turn true
							//if (ExistanceCheck(objectID)) continue;
							if (Get() == disabled) continue; // same value, skip re-rendering the screen
							data.objects[objectID].facts[UnitFact.IsDisabled] = disabled;
							render = true;
						}
						PickedIDs.Set(picked);
					}
					public static bool Get()
					{
						return GetFact(UnitFact.IsDisabled);
					}
				}

				// Duplicate Delete
			}
			public static class Identity
			{
				public static class AllIDs
				{
					public static string[] Get()
					{
						return data.objects.Keys.ToArray();
					}
				}
				public static class ID
				{
					public static void Set(string ID)
					{
						var picked = PickedIDs.Get();
						foreach (var objectID in picked)
						{
							PickedIDs.Set(objectID);
							if (ExistanceCheck()) continue;

							if (data.objects.ContainsKey(objectID) == false)
							{
								data.objects[ID] = new UnitInstance(ID);
								return;
							}

							if (Existance.Disabled.Get()) return;

							data.objects.Remove(objectID);
							data.objects[ID] = data.objects[objectID];
							PickedIDs.Set(ID);

							var depth = Appearance.Depth.Get();
							if (data.objectsDepthSorted[depth].Contains(objectID))
							{
								data.objectsDepthSorted[depth].Remove(objectID);
								if (data.objectsDepthSorted[depth].Count == 0)
								{
									data.objectsDepthSorted.Remove(depth);
								}
							}

							Appearance.Depth.Set(depth);
						}
						PickedIDs.Set(picked);
					}
				}
				public static class FamilyIDs
				{
					public static class Parent
					{
						public static void Set(string parentID)
						{
							if (Statics.NoIDPickedError()) return;

							var picked = PickedIDs.Get();
							foreach (var objectID in picked)
							{
								PickedIDs.Set(objectID);
								if (ExistanceCheck()) continue;
								// TODO validate
								var currentParent = Get();
								if (parentID == currentParent) return;

								var pos = data.objects[objectID].sprite.Position;
								var scale = data.objects[objectID].sprite.Scale;
								var angle = data.objects[objectID].sprite.Rotation;
								var parentScale = currentParent == null ? world.Scale : data.objects[currentParent].sprite.Scale;
								var futureParentScale = parentID == null ? world.Scale : data.objects[parentID].sprite.Scale;

								data.objects[objectID].texts[UnitText.ParentID] = parentID;
								// unparent
								if (parentID == null)
								{
									var newUnparentPos = currentParent == null ? pos : data.objects[currentParent].sprite.Transform.TransformPoint(pos);
									var newUnparentScaleW = currentParent == null ? scale.X : scale.X * parentScale.X;
									var newUnparentScaleH = currentParent == null ? scale.Y : scale.Y * parentScale.Y;
									var parAng = data.objects[currentParent].sprite.Rotation;

									data.objects[currentParent].textCollections[UnitTexts.ChildrenIDs].Remove(objectID);
									Area.Position.Set(newUnparentPos.X, newUnparentPos.Y, true);
									Area.Angle.Set(parAng + angle, true);
									Area.Scale.Set(newUnparentScaleW, newUnparentScaleH, true);
								}
								// parent
								else
								{
									var parAng = data.objects[parentID].sprite.Rotation;
									var newPos = data.objects[parentID].sprite.InverseTransform.TransformPoint(pos);
									var scaleW = scale.X / futureParentScale.X;
									var scaleH = scale.Y / futureParentScale.Y;

									data.objects[parentID].textCollections[UnitTexts.ChildrenIDs].Add(objectID);
									Area.Position.Set(newPos.X, newPos.Y, true);
									Area.Scale.Set(scaleW, scaleH, true);
									Area.Angle.Set(-(parAng - angle), true);
								}
							}
							PickedIDs.Set(picked);
						}
						public static string Get()
						{
							return GetText(UnitText.ParentID);
						}
					}
					public static class Children
					{
						public static string[] Get()
						{
							return GetTexts(UnitTexts.ChildrenIDs);
						}
					}
				}
				// Tags
			}

			//=====================

			private static bool GetFact(UnitFact fact)
			{
				return Statics.NoIDPickedError() || Statics.MultipleIDsPickedError() ||
					Statics.DoesNotExistError(data.objects, "Unit") ||
					data.objects[PickedIDs.Get()[0]].facts.ContainsKey(fact) == false ? default :
					data.objects[PickedIDs.Get()[0]].facts[fact];
			}
			private static double GetNumber(UnitNumber number)
			{
				return Statics.NoIDPickedError() || Statics.MultipleIDsPickedError() ||
					Statics.DoesNotExistError(data.objects, "Unit") ||
					data.objects[PickedIDs.Get()[0]].numbers.ContainsKey(number) == false ? default :
					data.objects[PickedIDs.Get()[0]].numbers[number];
			}
			private static string GetText(UnitText text)
			{
				return Statics.NoIDPickedError() || Statics.MultipleIDsPickedError() ||
					Statics.DoesNotExistError(data.objects, "Unit") ||
					data.objects[PickedIDs.Get()[0]].texts.ContainsKey(text) == false ? default :
					data.objects[PickedIDs.Get()[0]].texts[text];
			}
			private static string[] GetTexts(UnitTexts texts)
			{
				return Statics.NoIDPickedError() || Statics.MultipleIDsPickedError() ||
					Statics.DoesNotExistError(data.objects, "Unit") ||
					data.objects[PickedIDs.Get()[0]].textCollections.ContainsKey(texts) == false ? default :
					data.objects[PickedIDs.Get()[0]].textCollections[texts].ToArray();
			}
			private static double GetEffect(UnitEffect effect)
			{
				return Statics.NoIDPickedError() || Statics.MultipleIDsPickedError() ||
					Statics.DoesNotExistError(data.objects, "Unit") ||
					data.objects[PickedIDs.Get()[0]].shaderArgs.ContainsKey(effect) == false ? default :
					data.objects[PickedIDs.Get()[0]].shaderArgs[effect];
			}

			private static void MoveInDirection(Vector2f dir, double speed, bool relativeToParent,
				Simple.Motion motion = Simple.Motion.PerSecond)
			{
				if (motion == Simple.Motion.PerSecond)
				{
					speed *= tickDeltaTime.ElapsedTime.AsSeconds();
				}
				var picked = PickedIDs.Get();
				foreach (var ID in picked)
				{
					PickedIDs.Set(ID);
					var x = Area.Position.GetX(relativeToParent);
					var y = Area.Position.GetY(relativeToParent);
					Area.Position.SetX(x + dir.X * speed, relativeToParent);
					Area.Position.SetY(y + dir.Y * speed, relativeToParent);
				}
				PickedIDs.Set(picked);
			}
			private static bool ExistanceCheck()
			{
				var objectID = PickedIDs.Get()[0];
				if (data.objects.ContainsKey(objectID) == false)
				{
					data.objects[objectID] = new UnitInstance(objectID);
					return false;
				}
				return Existance.Disabled.Get();
			}
			private static void SetTexture(string objectID, string filePath, SFML.Graphics.Texture texture)
			{
				//if (sprite != null && sprite.Texture == texture) return; // same texture is present, skip re-rendering the screen
				// ^^^ this is disabled so that the texture can be recreated/updated from other methods

				render = true; // TODO validate

				var sprite = data.objects[objectID].sprite;
				var origin = sprite == null ? new Vector2f() : sprite.Origin;
				var scale = sprite == null ? new Vector2f(1, 1) : sprite.Scale;
				var pos = sprite == null ? new Vector2f(0, 0) : sprite.Position;
				var rot = sprite == null ? 0 : sprite.Rotation;
				var col = sprite == null ? new SFML.Graphics.Color(0, 0, 0, 255) : sprite.Color;
				var rep = sprite == null || sprite.Texture == null ? false : sprite.Texture.Repeated;
				var smooth = sprite == null || sprite.Texture == null ? false : sprite.Texture.Smooth;

				data.objects[objectID].sprite = new Sprite(texture);
				if (filePath != null) data.objects[objectID].texts[UnitText.TexturePath] = filePath;
				data.objects[objectID].sprite.Origin = origin;
				data.objects[objectID].sprite.Scale = scale;
				data.objects[objectID].sprite.Texture = texture;
				data.objects[objectID].sprite.Position = pos;
				data.objects[objectID].sprite.Rotation = rot;
				data.objects[objectID].sprite.Color = col;
				data.objects[objectID].sprite.Texture.Repeated = rep;
				data.objects[objectID].sprite.Texture.Smooth = smooth;

				// render texture for sprite masking
				//if (data.objects[objectID].renderTexture != null) data.objects[objectID].renderTexture.Dispose();
				//if (data.objects[objectID].rawTexture != null) data.objects[objectID].rawTexture.Dispose();
				//if (data.objects[objectID].image != null) data.objects[objectID].image.Dispose();
				// enable if memory leak (?)

				data.objects[objectID].renderTexture = new RenderTexture(texture.Size.X, texture.Size.Y);

				data.objects[objectID].image = new Image(data.objects[objectID].sprite.Texture.CopyToImage());
				if (GetFact(UnitFact.MasksIn) == false) data.objects[objectID].image.FlipVertically();
				data.objects[objectID].rawTextureData = data.objects[objectID].image.Pixels;

				data.objects[objectID].image.FlipVertically();
				data.objects[objectID].rawTexture = new SFML.Graphics.Texture(data.objects[objectID].image);

				data.objects[objectID].shader.SetUniform("texture", Shader.CurrentTexture);
				data.objects[objectID].shader.SetUniform("raw_texture", data.objects[objectID].rawTexture);
			}
			private static void SetShaderArg(UnitEffect effect, string arg, double rawValue, double value, bool usePercent1)
			{
				if (Statics.NoIDPickedError()) return;

				var picked = PickedIDs.Get();
				foreach (var objectID in picked)
				{
					PickedIDs.Set(objectID);
					if (ExistanceCheck()) continue;

					var percent1 = Number.Converted.From.Percent.Get(value, 0, 1);
					data.objects[objectID].shaderArgs[effect] = rawValue;
					data.objects[objectID].SetShaderArg(arg, usePercent1 ? percent1 : value);
					render = true; // TODO validate
				}
				PickedIDs.Set(picked);
			}
		}
		public static class Storage
		{
			public static class Duplicate
			{
				public static void Do(object[] values)
				{
					foreach (var ID in pickedIDs)
					{
						Create(ID);
						data.storage[ID] = values.ToList();
					}
				}
			}
			public static class Expand
			{
				public static void Do(double atIndex, params object[] values)
				{
					var atEnd = false;
					foreach (var ID in pickedIDs)
					{
						Create(ID);
						for (int i = 0; i < values.Length; i++)
						{
							var atEndIndex = (int)Number.Limited.Get(atIndex, 0, data.storage[ID].Count, NumberLimitation.Overflow);
							if (atEndIndex == data.storage[ID].Count) atEnd = true;
							if (atEnd)
							{
								data.storage[ID].Add(values[i]);
								continue;
							}

							var vIndex = GetIndex(ID, atIndex + i);
							data.storage[ID].Insert(vIndex, values[i]);
						}
					}
				}
			}
			public static class Shrink
			{
				public static void Do(double atIndex, double times)
				{
					times = Number.Rounded.Get(times);
					foreach (var ID in pickedIDs)
					{
						if (data.storage.ContainsKey(ID) == false || data.storage[ID].Count == 0) continue;
						Create(ID);
						for (int i = 0; i < times; i++)
						{
							var vIndex = GetIndex(ID, atIndex);
							data.storage[ID].RemoveAt(vIndex);
						}
					}
				}
			}

			public static class Indexes
			{
				public static double[] Get(object value)
				{
					if (Statics.NoIDPickedError() || Statics.MultipleIDsPickedError() ||
						Statics.DoesNotExistError(data.storage, "Storage")) return new double[0];

					var storageID = pickedIDs[0];
					var result = new List<double>();
					for (int i = 0; i < data.storage[storageID].Count; i++)
					{
						if (data.storage[storageID][i] == value)
						{
							result.Add(i);
						}
					}
					return result.ToArray();
				}
			}
			public static class Values
			{
				public static class Single
				{
					public static object Get(double index)
					{
						if (Statics.NoIDPickedError() || Statics.MultipleIDsPickedError() ||
							Statics.DoesNotExistError(data.storage, "Storage")) return new double[0];

						var storageID = pickedIDs[0];
						var i = GetIndex(storageID, index);
						var value = data.storage[storageID][i];

						return value;
					}
				}
				public static class All
				{
					public static object[] Get()
					{
						if (Statics.NoIDPickedError() || Statics.MultipleIDsPickedError() ||
							Statics.DoesNotExistError(data.storage, "Storage")) return new object[0];

						var storageID = pickedIDs[0];
						var result = new List<object>();
						var list = data.storage[storageID];
						foreach (var value in list)
						{
							result.Add(value);
						}
						return result.ToArray();
					}
				}
				public static class Replace
				{
					public static void Do(double index, params object[] values)
					{
						foreach (var ID in pickedIDs)
						{
							Create(ID);
							for (int i = 0; i < values.Length; i++)
							{
								var vIndex = GetIndex(ID, index + i);
								data.storage[ID][vIndex] = values[i];
							}
						}
					}
				}
				public static class Amount
				{
					public static double Get()
					{
						if (Statics.NoIDPickedError() || Statics.MultipleIDsPickedError() ||
						Statics.DoesNotExistError(data.storage, "Storage")) return double.NaN;

						var storageID = pickedIDs[0];
						return data.storage[storageID].Count;
					}
				}
			}

			private static void Create(string storageID)
			{
				if (data.storage.ContainsKey(storageID) == false)
				{
					data.storage[storageID] = new List<object>();
				}
			}
			private static int GetIndex(string storageID, double index)
			{
				var n = Number.Limited.Get(index, 0, data.storage[storageID].Count - 1, NumberLimitation.Overflow);
				n = Number.Rounded.Get(n, 0, NumberRoundToward.Closest);
				return (int)n;
			}
		}
		public static class Gate
		{
			public static class Existance
			{
				public static void DoDelete()
				{
					foreach (var ID in pickedIDs)
					{
						if (data.gates.ContainsKey(ID) == false) continue;
						data.gates.Remove(ID);
					}
				}
			}
			public static class Entries
			{
				public static void DoDelete()
				{
					foreach (var ID in pickedIDs)
					{
						if (data.gateEntriesCount.ContainsKey(ID) == false) continue;
						data.gateEntriesCount[ID] = default;
					}
				}
				public static double Get()
				{
					if (Statics.NoIDPickedError() || Statics.MultipleIDsPickedError() ||
						Statics.DoesNotExistError(data.gates, "Gate")) return double.NaN;

					return data.gateEntriesCount.ContainsKey(pickedIDs[0]) ? data.gateEntriesCount[pickedIDs[0]] : 0;
				}
			}
			public static class Opened
			{
				public static bool DoGet(bool condition, double maxEntries = 999_999_999_999)
				{
					var result = false;
					foreach (var ID in pickedIDs)
					{
						if (data.gates.ContainsKey(ID) == false && condition == false) return false;
						else if (data.gates.ContainsKey(ID) == false && condition == true)
						{
							data.gates[ID] = true;
							data.gateEntriesCount[ID] = 1;
							result = true;
						}
						else
						{
							if (data.gates[ID] == true && condition == true) return false;
							else if (data.gates[ID] == false && condition == true)
							{
								data.gates[ID] = true;
								data.gateEntriesCount[ID]++;
								result = true;
							}
							else if (data.gateEntriesCount[ID] < maxEntries) data.gates[ID] = false;
						}
					}
					return result;
				}
			}
		}
		public static class Timer
		{
			public static class Duration
			{
				public static void Set(double durationInSeconds)
				{
					foreach (var ID in pickedIDs)
					{
						data.timerDurations[ID] = durationInSeconds;
						data.timers[ID] = durationInSeconds;
					}
				}
				public static double Get(bool countdown = true)
				{
					if (Statics.NoIDPickedError() || Statics.MultipleIDsPickedError()) return double.NaN;

					var timerID = PickedIDs.Get()[0];
					if (data.timers.ContainsKey(timerID) == false) return double.NaN;

					return countdown ? data.timers[timerID] : data.timerDurations[timerID] - data.timers[timerID];
				}
			}
			public static class Running
			{
				public static void Set(bool running)
				{
					if (running)
					{
						foreach (var ID in pickedIDs)
						{
							data.timers[ID] = data.timerDurations[ID];
							data.timerPauses[ID] = false;
						}
						return;
					}
					foreach (var ID in pickedIDs)
					{
						data.timers[ID] = data.timerDurations[ID];
						data.timerPauses[ID] = true;
					}
				}
				public static bool Get()
				{
					if (Statics.NoIDPickedError()) return false;

					foreach (var ID in pickedIDs)
					{
						if (data.timers.ContainsKey(ID) == false) return false;
						if (data.timerPauses[ID]) return false;
					}
					return true;
				}
			}
		}
		public static class Animation
		{
			public static class SecondsBetweenTicks
			{
				public static void Set(double seconds)
				{
					Timer.Duration.Set(seconds);
				}
				public static double Get()
				{
					return Timer.Duration.Get();
				}
			}
			public static class Running
			{
				public static void Set(bool running)
				{
					Timer.Running.Set(running);
					foreach (var ID in pickedIDs)
					{
						data.animationIDs.Add(ID);
					}
				}
				public static bool Get()
				{
					return Timer.Running.Get();
				}
			}
			public static class Reversed
			{
				public static void Set(bool reversed)
				{
					foreach (var ID in pickedIDs)
					{
						data.animationReversed[ID] = reversed;
					}
				}
				public static bool Get()
				{
					if (Statics.NoIDPickedError()) return false;

					foreach (var ID in pickedIDs)
					{
						if (data.animationReversed.ContainsKey(ID) == false ||
							data.animationReversed[ID] == false) return false;
					}
					return true;
				}
			}
			public static class Tick
			{
				public static void Set(double tick)
				{
					foreach (var ID in pickedIDs)
					{
						data.animationTicks[ID] = tick;
					}
				}
				public static double Get()
				{
					if (Statics.NoIDPickedError() || Statics.MultipleIDsPickedError()) return double.NaN;

					var animationID = PickedIDs.Get()[0];
					if (data.animationTicks.ContainsKey(animationID) == false) return double.NaN;

					return data.animationTicks[animationID];
				}
			}
		}
	}
	public static class Game
	{
		public static class Running
		{
			public static void DoStart(Simple game)
			{
				var isWindows = Environment.OSVersion.Platform == PlatformID.Win32NT;
				if (isWindows)
				{
					var processName = Process.GetCurrentProcess().ProcessName;
					_ramAvailable = new PerformanceCounter("Memory", "Available MBytes");
					_ramUsedPercent = new PerformanceCounter("Memory", "% Committed Bytes In Use");
					_cpuPercent = new PerformanceCounter("Processor", "% Processor Time", "_Total");
				}
				else return;

				var result = 0L;
				GetPhysicallyInstalledSystemMemory(out result);
				totalRam = result / 1024f / 1024f;

				RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
				if (data != null && Simple.game != null) return;
				data = new Data();
				Simple.game = game;
				CreateWindow();
				CreateEvents();

				resourceLoading = new Thread(new ThreadStart(LoadQueuedResources));
				resourceLoading.Start();

				SetStartDefaults();
				game.OnGameProgressChanged(Progress.Start, 0, data.totalTickCount);

				time = new Clock();
				tickDeltaTime = new Clock();
				frameDeltaTime = new Clock();
				Run();
			}
			public static void DoStop()
			{
				window.Close();
				game.OnWindowAction(WindowAction.Closed);
				game.OnGameProgressChanged(Progress.End, runtimeTickCount, data.totalTickCount);
			}
		}
		public static class Title
		{
			public static void Set(string title)
			{
				window.SetTitle(title);
			}
			public static string Get()
			{
				return form.Text;
			}
		}
		public static class Pause
		{
			public static class WhenUnfocused
			{
				public static void Set(bool paused)
				{
					data.pauseOnWindowUnfocused = paused;
				}
				public static bool Get()
				{
					return data.pauseOnWindowUnfocused;
				}
			}
			public static class Active
			{
				public static bool Get()
				{
					return data.pauseOnWindowUnfocused && window.HasFocus() == false;
				}
			}
		}
		public static class Focused
		{
			public static bool Get()
			{
				return window.HasFocus();
			}
		}
		public static class Hidden
		{
			public static void Set(bool hidden)
			{
				form.Visible = !hidden;
			}
			public static bool Get()
			{
				return form.Visible;
			}
		}
		public static class Message
		{
			public static void DoPopUp(object message, string title = "Simple PopUp", PopUpIcon icon = PopUpIcon.None)
			{
				var msgIcon = MessageBoxIcon.None;
				switch (icon)
				{
					case PopUpIcon.Info: msgIcon = MessageBoxIcon.Information; break;
					case PopUpIcon.Error: msgIcon = MessageBoxIcon.Error; break;
					case PopUpIcon.Warning: msgIcon = MessageBoxIcon.Warning; break;
				}
				MessageBox.Show($"{message}", title, MessageBoxButtons.OK, msgIcon);
			}
		}
		public static class WindowState
		{
			public static void Set(Simple.WindowState state)
			{
				form.WindowState = (FormWindowState)state;
			}
			public static Simple.WindowState Get()
			{
				return (Simple.WindowState)form.WindowState;
			}
		}
		public static class BackgroundColor
		{
			public static void Set(double percentRed, double percentGreen, double percentBlue)
			{
				SetRed(percentRed);
				SetGreen(percentGreen);
				SetBlue(percentBlue);
			}
			public static void SetToSample(Color color)
			{
				var c = Statics.GetColorFromSample(color);
				Set(
					Number.Converted.To.Percent.Get(c.R, 0, 255),
					Number.Converted.To.Percent.Get(c.G, 0, 255),
					Number.Converted.To.Percent.Get(c.B, 0, 255));
			}
			public static void SetRed(double percentRed)
			{
				percentRed = Number.Converted.From.Percent.Get(percentRed, 0, 255);
				data.backgroundColor = new SFML.Graphics.Color((byte)percentRed, data.backgroundColor.G, data.backgroundColor.B);
				render = true;
			}
			public static void SetGreen(double percentGreen)
			{
				percentGreen = Number.Converted.From.Percent.Get(percentGreen, 0, 255);
				data.backgroundColor = new SFML.Graphics.Color(data.backgroundColor.R, (byte)percentGreen, data.backgroundColor.B);
				render = true;
			}
			public static void SetBlue(double percentBlue)
			{
				percentBlue = Number.Converted.From.Percent.Get(percentBlue, 0, 255);
				data.backgroundColor = new SFML.Graphics.Color(data.backgroundColor.R, data.backgroundColor.G, (byte)percentBlue);
				render = true;
			}
			public static double GetRed()
			{
				return Number.Converted.From.Percent.Get(data.backgroundColor.R, 0, 255);
			}
			public static double GetGreen()
			{
				return Number.Converted.From.Percent.Get(data.backgroundColor.G, 0, 255);
			}
			public static double GetBlue()
			{
				return Number.Converted.From.Percent.Get(data.backgroundColor.B, 0, 255);
			}
		}

	}
	public static class Hardware
	{
		public static class RAM
		{
			public static class Total
			{
				public static double Get()
				{
					return totalRam;
				}
			}
			public static class Available
			{
				public static double Get()
				{
					return ramAvailable;
				}
			}
			public static class Used
			{
				public static double Get(RAMUsage usage)
				{
					return usage == RAMUsage.GB ? Total.Get() - Available.Get() : ramUsedPercent;
				}
			}
		}
		public static class PercentUsedCPU
		{
			public static double Get()
			{
				return cpuPercent;
			}
		}
		public static class ComputerSleepPrevented
		{
			/// <summary>
			/// When <paramref name="activated"/> the user's computer will stay active at all times, even when left idle.<br></br><br></br>
			/// A check wether sleep prevention is activated can be done via <see cref="ComputerSleepIsPrevented"/>.
			/// </summary>
			public static void Set(bool prevented)
			{
				data.sleepPrevented = prevented;
				if (data.sleepPrevented)
				{
					SetThreadExecutionState(EXECUTION_STATE.ES_CONTINUOUS | EXECUTION_STATE.ES_DISPLAY_REQUIRED | EXECUTION_STATE.ES_SYSTEM_REQUIRED);
				}
				else
				{
					SetThreadExecutionState(EXECUTION_STATE.ES_CONTINUOUS);
				}
			}
			/// <summary>
			/// Checks wether the user's computer is prevented from sleeping and returns the result.<br></br><br></br>
			/// Sleep prevention can be activated or deactivated through <see cref="PreventComputerSleep"/>.
			/// </summary>
			public static bool Get()
			{
				return data.sleepPrevented;
			}
		}
	}
	public static class Performance
	{
		public static class Tick
		{
			public static class Rate
			{
				public static double Get(bool averaged = false)
				{
					return averaged ? runtimeTickCount / time.ElapsedTime.AsSeconds() : 1 / tickDeltaTime.ElapsedTime.AsSeconds();
				}
			}
			public static class Count
			{
				public static class Running
				{
					public static double Get()
					{
						return runtimeTickCount;
					}
				}
				public static class TotalEver
				{
					public static double Get()
					{
						return data.totalTickCount;
					}
				}
			}
		}
		public static class Frame
		{
			public static class Rate
			{
				public static double Get(bool averaged = false)
				{
					return averaged ? runtimeFrameCount / time.ElapsedTime.AsSeconds() : 1 / frameDeltaTime.ElapsedTime.AsSeconds();
				}
			}
			public static class RateLimit
			{
				public static void Set(double framesPerSecond)
				{
					var n = Number.Limited.Get(framesPerSecond, 1, 60);
					data.frameRateLimit = n;
					window.SetFramerateLimit((uint)n);
				}
				public static double Get()
				{
					return data.frameRateLimit;
				}
			}
			public static class Count
			{
				public static class Running
				{
					public static double Get()
					{
						return runtimeFrameCount;
					}
				}
				public static class TotalEver
				{
					public static double Get()
					{
						return data.totalFrameCount;
					}
				}
			}
			public static class Drawing
			{
				public static class Suspended
				{
					public static void Set(bool suspended)
					{
						data.renderSuspended = suspended;
					}
					public static bool Get()
					{
						return data.renderSuspended;
					}
				}
				public static class Forced
				{
					public static void Set(bool forced)
					{
						render = forced;
					}
					public static bool Get()
					{
						return render;
					}
				}
			}
		}
		public static class SecondsSince
		{
			public static class GameStart
			{
				public static class Running
				{
					public static double Get()
					{
						return time.ElapsedTime.AsSeconds();
					}
				}
				public static class FirstEver
				{
					public static double Get()
					{
						return data.totalTime;
					}
				}
			}
			public static class Last
			{
				public static class Frame
				{
					public static double Get()
					{
						return frameDeltaTime.ElapsedTime.AsSeconds();
					}
				}
				public static class Tick
				{
					public static double Get()
					{
						return tickDeltaTime.ElapsedTime.AsSeconds();
					}
				}
			}
		}
		public static class VerticalSynced
		{
			public static void Set(bool vSynced)
			{
				data.vSync = vSynced;
				window.SetVerticalSyncEnabled(vSynced);
			}
			public static bool Get()
			{
				return data.vSync;
			}
		}
	}
	public static class Camera
	{
		// TODO multiple cameras for minimap etc.
		public static class Angle
		{
			public static void Set(double angle)
			{
				data.cameraNumbers[CameraNumber.Angle] = angle;
				camera.Rotation = (float)angle;
				view.Rotation = camera.Rotation;
				window.SetView(view);

				render = true; // TODO validate
			}
			public static double Get()
			{
				return GetNumber(CameraNumber.Angle);
			}
		}
		public static class Position
		{
			public static void Set(double x, double y)
			{
				SetX(x);
				SetY(y);
			}
			public static void SetX(double x)
			{
				data.cameraNumbers[CameraNumber.X] = x;
				camera.Position = new Vector2f((float)x, (float)camera.Position.Y);
				view.Center = camera.Position;
				window.SetView(view);

				render = true; // TODO validate
			}
			public static void SetY(double y)
			{
				data.cameraNumbers[CameraNumber.Y] = y;
				camera.Position = new Vector2f(camera.Position.X, (float)y);
				view.Center = camera.Position;
				window.SetView(view);

				render = true; // TODO validate
			}
			public static double GetX()
			{
				return GetNumber(CameraNumber.X);
			}
			public static double GetY()
			{
				return GetNumber(CameraNumber.Y);
			}
		}
		public static class Zoom
		{
			public static void Set(double zoom)
			{
				data.cameraNumbers[CameraNumber.Zoom] = zoom;
				camera.Scale = new Vector2f((float)zoom, (float)zoom);

				render = true; // TODO validate
			}
			public static double Get()
			{
				var result = GetNumber(CameraNumber.Zoom);
				return result == 0 ? 1 : result;
			}
		}
		public static class Screenshot
		{
			public static void Do(string filePath = "folder/file.png")
			{
				var rendTexture = new RenderTexture((uint)view.Size.X, (uint)view.Size.Y);
				rendTexture.Texture.Update(window);
				rendTexture.Display();

				var img = rendTexture.Texture.CopyToImage();
				var full = Statics.CreateDirectoryForFile(filePath);

				if (img.SaveToFile(filePath))
				{
					img.Dispose();
					rendTexture.Dispose();
				}
				else
				{
					Statics.ShowError(1, $"Could not save screenshot file '{full}'.");
				}
			}
		}

		private static double GetNumber(CameraNumber number)
		{
			return data.cameraNumbers.ContainsKey(number) == false ? default : data.cameraNumbers[number];
		}
	}
	public static class Console
	{
		public static class Title
		{
			public static void Set(object title)
			{
				Statics.ConsoleShow();
				System.Console.Title = $"{title}";
			}
			public static string Get()
			{
				return System.Console.Title;
			}
		}
		public static class Input
		{
			public static string Get()
			{
				Statics.ConsoleShow();

				Statics.ConsoleSelectionEnable(true);
				var result = System.Console.ReadLine();
				Statics.ConsoleSelectionEnable(false);

				return result;
			}
		}
		public static class Log
		{
			public static class Message
			{
				public static void Do(object message, bool newLine = true)
				{
					Statics.ConsoleShow();
					var strNewLine = newLine ? "\n" : "";
					System.Console.Write($"{message}{strNewLine}");
				}
			}
			public static class Storage
			{
				public static void Do(string separator = "\n", bool showStorageID = true, bool showValueIndexes = true)
				{
					foreach (var ID in pickedIDs)
					{
						if (data.storage.ContainsKey(ID) == false) continue;
						var isEmpty = data.storage[ID].Count == 0;
						var empty = isEmpty ? $"(Empty)" : "";
						var storage = showStorageID ? $"Storage ID: ~{ID}~" : "";
						Message.Do(storage);
						Message.Do(empty);
						if (isEmpty) continue;
						for (int i = 0; i < data.storage[ID].Count; i++)
						{
							var valueIndexes = showValueIndexes ? $"{i}. " : "";
							Message.Do($"{valueIndexes}{data.storage[ID][i]}{separator}", false);
						}
						Message.Do("");
					}
				}
			}
			public static class Clear
			{
				public static void Do()
				{
					Statics.ConsoleShow();
					System.Console.Clear();
				}
			}
		}
	}
	/// <summary>
	/// Controls <see cref="string"/> in different ways.
	/// </summary>
	public static class Text
	{
		public static class Copied
		{
			/// <summary>
			/// Adds <paramref name="text"/> to the clipboard (copies it). It can be accessed later via <typeparamref name="Ctrl"/> + <typeparamref name="V"/> or <see cref="GetCopied"/>.
			/// </summary>
			public static void Set(string text)
			{
				System.Windows.Forms.Clipboard.SetText(text);
			}
			/// <summary>
			/// Gets the clipboard data and returns it if it is a <see cref="string"/>, otherwise returns <paramref name="null"/>.‪‪
			/// </summary>
			public static string Get()
			{
				var result = System.Windows.Forms.Clipboard.GetText();
				return result == string.Empty ? null : result;
			}
		}
		public static class Repeated
		{
			public static string Get(string text, double times)
			{
				var result = "";
				var intTimes = (int)Number.Rounded.Get(times, 0);
				for (int i = 0; i < intTimes; i++)
				{
					result = $"{result}{text}";
				}
				return result;
			}
		}
		public static class Converted
		{
			public static class To
			{
				public static class Data
				{
					/// <summary>
					/// Converts an already formatted <paramref name="text"/> (<paramref name="JSON"/>) into <typeparamref name="T"/> <paramref name="data"/> and returns it if the <paramref name="text"/> is in the correct format. Otherwise returns <paramref name="default"/>(<typeparamref name="T"/>).
					/// </summary>
					public static object Get(string text)
					{
						try
						{
							return JsonConvert.DeserializeObject<object>(text);
						}
						catch (Exception)
						{
							return default;
						}
					}
				}
				public static class Encryption
				{
					/// <summary>
					/// Returns a new <see cref="string"/> after a simple encryption on <paramref name="text"/> with a <paramref name="key"/> that can be <paramref name="performedTwice"/>.‪‪ The encryption can be decrypred later and the text can be retrieved back with <see cref="GetDecrypted"/>.
					/// </summary>
					public static string Get(string text, char key, bool performedTwice = false)
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
				}
			}
			public static class From
			{
				public static class Data
				{
					/// <summary>
					/// Converts <typeparamref name="T"/> <paramref name="data"/> into a <see cref="string"/> (<paramref name="JSON"/>) and returns it. It can be converted and retrieved back to <typeparamref name="T"/> <paramref name="data"/> later with <see cref="GetData"/>.‪‪
					/// </summary>
					public static string Get(object data)
					{
						return JsonConvert.SerializeObject(data);
					}
				}
				public static class Encryption
				{
					/// <summary>
					/// Returns the decrypted version of an encrypted <paramref name="text"/> with a <paramref name="key"/> that could have been <paramref name="performedTwice"/> with <see cref="GetEncrypted"/>.‪‪
					/// </summary>
					public static string Get(string encryptedText, char key, bool performedTwice = false)
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
		}
		public static class Formated
		{
			public static class Array
			{
				/// <summary>
				/// Converts an <paramref name="array"/> into a <see cref="string"/>. The elements are separated by a <paramref name="separator"/>. Then the <see cref="string"/> is returned.
				/// </summary>
				public static string Get<T>(T[] array, string separator = ", ", bool includeIndex = true,
					string indexLeft = "[", string indexRight = "] ")
				{
					var result = "";

					for (int i = 0; i < array.Length; i++)
					{
						var index = includeIndex ? $"{i}" : "";
						var indexR = includeIndex ? $"{indexRight}" : "";
						var indexL = includeIndex ? $"{indexLeft}" : "";
						result = result.Insert(result.Length, $"{indexL}{index}{indexR}{array[i]}");
						if (i == array.Length - 1) break;
						result = result.Insert(result.Length, $"{separator}");
					}
					return result;
				}
			}
			public static class Time
			{
				public static string Get(double seconds, string separator = ":", bool msShow = false, string msFormat = "ms",
					bool secShow = true, string secFormat = "s", bool minShow = true, string minFormat = "m", bool hrShow = true,
					string hrFormat = "h")
				{
					seconds = Number.Signed.Get(seconds, false);
					var secondsStr = seconds.ToString();
					var ms = 0;
					if (secondsStr.Contains('.'))
					{
						var spl = secondsStr.Split('.');
						ms = int.Parse(spl[1]) * 100;
						seconds = Number.Rounded.Get(seconds, toward: NumberRoundToward.Down);
					}
					var sec = seconds % 60;
					var min = Number.Rounded.Get(seconds / 60 % 60, toward: NumberRoundToward.Down);
					var hr = Number.Rounded.Get(seconds / 3600, toward: NumberRoundToward.Down);
					var msStr = msShow ? $"{ms}" : "";
					var secStr = secShow ? $"{sec}" : "";
					var minStr = minShow ? $"{min}" : "";
					var hrStr = hrShow ? $"{hr}" : "";
					var msF = msShow ? $"{msFormat}" : "";
					var secF = secShow ? $"{secFormat}" : "";
					var minF = minShow ? $"{minFormat}" : "";
					var hrF = hrShow ? $"{hrFormat}" : "";
					var secMsSep = msShow && (secShow || minShow || hrShow) ? $"{separator}" : "";
					var minSecSep = secShow && (minShow || hrShow) ? $"{separator}" : "";
					var hrMinSep = minShow && hrShow ? $"{separator}" : "";

					return $"{hrStr}{hrF}{hrMinSep}{minStr}{minF}{minSecSep}{secStr}{secF}{secMsSep}{msStr}{msF}";
				}
			}
		}
		public static class Save
		{
			/// <summary>
			/// Creates or overwrites a file on <paramref name="filePath"/> with <paramref name="fileName"/> and <paramref name="fileExtension"/> then fills it with <paramref name="text"/>. This <paramref name="text"/> can be accessed later with <see cref="Get"/>.<br></br><br></br>
			/// This is a slow operation - do not call frequently.
			/// </summary>
			public static void Set(string text, string filePath = "folder/file.extension")
			{
				var full = Statics.CreateDirectoryForFile(filePath);

				try
				{
					File.WriteAllText(full, text);
				}
				catch (Exception)
				{
					Statics.ShowError(1, $"Could not save file '{full}'.");
					return;
				}
			}
			/// <summary>
			/// Reads the text from the file at <paramref name="filePath"/> with <paramref name="fileName"/> and <paramref name="fileExtension"/> and returns it as a <see cref="string"/> if successful. Returns <paramref name="null"/> otherwise. A text can be saved to a file with <see cref="Set"/>.<br></br><br></br>
			/// This is a slow operation - do not call frequently.
			/// </summary>
			public static string Get(string filePath = "folder/file.extension")
			{
				filePath = filePath.Replace('/', '\\');
				var full = $"{directory}\\{filePath}";

				if (Directory.Exists(full) == false)
				{
					Console.Log.Message.Do($"Could not load file '{full}'. Directory/file not found.");
					return default;
				}
				try
				{
					return File.ReadAllText(full);
				}
				catch (Exception)
				{
					Console.Log.Message.Do($"Could not load file '{full}'.");
					return default;
				}
			}
		}
	}
	/// <summary>
	/// Controls <see cref="double"/> in different ways.
	/// </summary>
	public static class Number
	{
		public static class Limited
		{
			public static double Get(double number, double boundA, double boundB, NumberLimitation limitType = NumberLimitation.ClosestBound)
			{
				if (boundA > boundB)
				{
					var swap = boundA;
					boundA = boundB;
					boundB = swap;
				}
				if (limitType == NumberLimitation.ClosestBound)
				{
					if (number < boundA)
					{
						return boundA;
					}
					else if (number > boundB)
					{
						return boundB;
					}
					return number;
				}
				else
				{
					boundB += 1;
					var a = number;
					a = Map(a);
					while (a < boundA)
					{
						a = Map(a);
					}
					return a;
					double Map(double b)
					{
						b = ((b % boundB) + boundB) % boundB;
						if (b < boundA) b = boundB - (boundA - b);
						return b;
					}
				}
			}
		}
		public static class Signed
		{
			public static double Get(double number, bool signed)
			{
				return signed ? -Math.Abs(number) : Math.Abs(number);
			}
		}
		public static class Randomized
		{
			public static double Get(double boundA, double boundB, double precision = 0, double seed = double.NaN)
			{
				precision = (int)Limited.Get(precision, 0, 5, NumberLimitation.ClosestBound);
				if (boundA > boundB)
				{
					var swap = boundA;
					boundA = boundB;
					boundB = swap;
				}
				var precisionValue = (double)Math.Pow(10, precision);
				var lowerInt = Convert.ToInt32(boundA * Math.Pow(10, Precision.Get(boundA)));
				var upperInt = Convert.ToInt32(boundB * Math.Pow(10, Precision.Get(boundB)));
				var s = new Random(double.IsNaN(seed) ? Guid.NewGuid().GetHashCode() : (int)Rounded.Get(seed));
				var randInt = s.Next((int)(lowerInt * precisionValue), (int)(upperInt * precisionValue) + 1);
				var result = randInt / precisionValue;

				return result;
			}
		}
		public static class Averaged
		{
			public static double Get(params double[] numbers)
			{
				return numbers == null ? double.NaN : numbers.Sum() / numbers.Length;
			}
		}
		public static class Precision
		{
			public static double Get(double number)
			{
				var cultDecPoint = Statics.CultureDecimalPoint();
				var split = number.ToString(cultDecPoint).Split();
				return split.Length > 1 ? split[1].Length : 0;
			}
		}
		public static class Rounded
		{
			public static double Get(double number, double precision = 0, NumberRoundToward toward = NumberRoundToward.Closest,
				NumberRoundWhen5 priority = NumberRoundWhen5.AwayFromZero)
			{
				var midpoint = (MidpointRounding)priority;
				precision = (int)Limited.Get(precision, 0, 5, NumberLimitation.ClosestBound);

				if (toward == NumberRoundToward.Down || toward == NumberRoundToward.Up)
				{
					var numStr = number.ToString();
					var prec = Precision.Get(number);
					if (prec > 0 && prec > precision)
					{
						var digit = toward == NumberRoundToward.Down ? "1" : "9";
						numStr = numStr.Remove(numStr.Length - 1);
						numStr = $"{numStr}{digit}";
						number = double.Parse(numStr);
					}
				}

				return Math.Round(number, (int)precision, midpoint);
			}
		}
		public static class Converted
		{
			public static class To
			{
				public static class Time
				{
					public static double Get(double time, NumberTimeConvertion convertType)
					{
						switch (convertType)
						{
							case NumberTimeConvertion.MillisecondsToSeconds: return time / 1000;
							case NumberTimeConvertion.SecondsToMilliseconds: return time * 1000;
							case NumberTimeConvertion.SecondsToMinutes: return time / 60;
							case NumberTimeConvertion.SecondsToHours: return time / 3600;
							case NumberTimeConvertion.MinutesToMilliseconds: return time * 60000;
							case NumberTimeConvertion.MinutesToSeconds: return time * 60;
							case NumberTimeConvertion.MinutesToHours: return time / 60;
							case NumberTimeConvertion.MinutesToDays: return time / 1440;
							case NumberTimeConvertion.HoursToSeconds: return time * 3600;
							case NumberTimeConvertion.HoursToMinutes: return time * 60;
							case NumberTimeConvertion.HoursToDays: return time / 24;
							case NumberTimeConvertion.HoursToWeeks: return time / 168;
							case NumberTimeConvertion.DaysToMinutes: return time * 1440;
							case NumberTimeConvertion.DaysToHours: return time * 24;
							case NumberTimeConvertion.DaysToWeeks: return time / 7;
							case NumberTimeConvertion.WeeksToHours: return time * 168;
							case NumberTimeConvertion.WeeksToDays: return time * 7;
						}
						return 0;
					}
				}
				public static class Percent
				{
					public static double Get(double number, double boundA, double boundB)
					{
						if (boundA > boundB)
						{
							var swap = boundA;
							boundA = boundB;
							boundB = swap;
						}
						return (number - boundA) * 100.0 / (boundB - boundA);
					}
				}
			}
			public static class From
			{
				public static class Text
				{
					public static double Get(string text)
					{
						var result = 0.0;
						text = text.Replace(',', '.');
						var parsed = double.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out result);

						return parsed ? result : double.NaN;
					}
				}
				public static class Percent
				{
					public static double Get(double percent, double boundA, double boundB)
					{
						// apparently the formula accounts for swapped min/max
						//if (boundA > boundB)
						//{
						//	var swap = boundA;
						//	boundA = boundB;
						//	boundB = swap;
						//}
						return (percent * (boundB - boundA) / 100) + boundA;
					}
				}
			}
		}
		public static class Checked
		{
			public static class Chance
			{
				public static bool Get(double percent)
				{
					percent = Limited.Get(percent, 0, 100, NumberLimitation.ClosestBound);
					var n = Randomized.Get(1, 100, 0); // should not roll 0 so it doesn't return true with 0% (outside of roll)
					return n <= percent;
				}
			}
			public static class BetweenBounds
			{
				public static bool Get(double number, double boundA, double boundB, bool inclusiveA = false, bool inclusiveB = false)
				{
					if (boundA > boundB)
					{
						var swap = boundA;
						boundA = boundB;
						boundB = swap;
					}
					var lower = false;
					var upper = false;
					if (inclusiveA) lower = boundA <= number;
					else lower = boundA < number;
					if (inclusiveB) upper = boundB >= number;
					else upper = boundB > number;

					return lower && upper;
				}
			}
		}
		public static class Changed
		{
			public static class Constantly
			{
				public static double Get(double number, double speed, Motion motion = Motion.PerSecond)
				{
					if (motion == Motion.PerSecond)
					{
						var delta = tickDeltaTime.ElapsedTime.AsSeconds();
						speed *= delta;
					}
					return number + speed;
				}
			}
			public static class TowardTarget
			{
				public static class Direction
				{
					public static double Get(double number, double targetNumber, double speed, Motion motion = Motion.PerSecond)
					{
						var goingPos = number < targetNumber;
						var result = Constantly.Get(number, goingPos ? Signed.Get(speed, false) : Signed.Get(speed, true), motion);

						if (goingPos && result > targetNumber) return targetNumber;
						else if (goingPos == false && result < targetNumber) return targetNumber;
						return result;
					}
				}
				public static class Percent
				{
					public static double Get(double number, double targetNumber, double percent)
					{
						return number + (percent / 100) * (targetNumber - number);
					}
				}
			}
		}
	}
	public static class Input
	{
		public static class Mouse
		{
			public static class Cursor
			{
				public static class Position
				{
					public static double GetX()
					{
						var mousePos = SFML.Window.Mouse.GetPosition(window);
						return window.MapPixelToCoords(mousePos).X;
					}
					public static double GetY()
					{
						var mousePos = SFML.Window.Mouse.GetPosition(window);
						return window.MapPixelToCoords(mousePos).Y;
					}
				}
			}
		}
	}
	public static class Debug
	{
		public static class Running
		{
			public static bool Get()
			{
				return Debugger.IsAttached;
			}
		}
		public static class Code
		{
			public static class LineNumber
			{
				public static double Get(double depth = 0)
				{
					depth = Number.Limited.Get(depth, 0, 99999);
					var info = new StackFrame((int)depth + 1, true);
					return info.GetFileLineNumber();
				}
			}
			public static class File
			{
				public static class Name
				{
					public static string Get(double depth = 0)
					{
						depth = Number.Limited.Get(depth, 0, 99999);
						var pathRaw = Path.Get(depth + 1);
						if (pathRaw == null) return null;
						var path = pathRaw.Split('\\');
						var name = path[path.Length - 1].Split('.');
						return name[0];
					}
				}
				public static class Path
				{
					public static string Get(double depth = 0)
					{
						depth = Number.Limited.Get(depth, 0, 99999);
						var info = new StackFrame((int)depth + 1, true);
						var a = info.GetFileName();
						return a;
					}
				}
			}
			public static class MethodName
			{
				public static string Get(double depth = 0)
				{
					depth = Number.Limited.Get(depth, 0, 99999);
					var info = new StackFrame((int)depth + 1, true);
					var method = info.GetMethod();
					var child = method == null ? null : method.ToString().Replace('+', '.');
					var firstSpaceIndex = child.IndexOf(' ');
					var parent = method.DeclaringType.ToString().Replace('+', '.') + ".";
					var result = child.Insert(firstSpaceIndex + 1, parent);

					return method == default ? default : result;
				}
			}
		}
	}
	public static class Multiplayer
	{
		public static class Server
		{
			public static class Running
			{
				public static void Set(bool running)
				{
					if (running)
					{
						try
						{
							if (serverIsRunning)
							{
								Console.Log.Message.Do("Server is already starting/started.");
								return;
							}
							if (clientIsConnected)
							{
								Console.Log.Message.Do("Cannot start a server while a Client.");
								return;
							}
							server = new Simple.Server(IPAddress.Any, (int)serverPort);
							server.Start();
							serverIsRunning = true;

							var hostName = Dns.GetHostName();
							var hostEntry = Dns.GetHostEntry(hostName);
							connectToServerInfo = "Clients can connect through those IPs if they are in the same multiplayer\n" +
								"(device / router / Virtual Private Network programs like Hamachi or Radmin):\nSame device: 127.0.0.1";
							foreach (var ip in hostEntry.AddressList)
							{
								if (ip.AddressFamily == AddressFamily.InterNetwork)
								{
									var ipParts = ip.ToString().Split('.');
									var ipType = ipParts[0] == "192" && ipParts[1] == "168" ? "Same router: " : "Same VPN: ";
									connectToServerInfo = $"{connectToServerInfo}\n{ipType}{ip}";
								}
							}
							Console.Log.Message.Do($"Started a {Game.Title.Get()} LAN Server on port {serverPort}.\n\n" +
								$"{connectToServerInfo}");
							Console.Log.Message.Do("");
						}
						catch (Exception ex)
						{
							serverIsRunning = false;
							Console.Log.Message.Do($"Error: {ex.Message}");
							return;
						}
						return;
					}
					try
					{
						if (serverIsRunning == false)
						{
							Console.Log.Message.Do("Server is not running.");
							return;
						}
						if (clientIsConnected)
						{
							Console.Log.Message.Do("Cannot stop a server while a client.");
							return;
						}
						serverIsRunning = false;
						server.Stop();
						Console.Log.Message.Do($"The LAN Server on port {serverPort} was stopped.");
					}
					catch (Exception ex)
					{
						serverIsRunning = false;
						Console.Log.Message.Do($"Error: {ex.Message}");
						return;
					}
				}
				public static bool Get()
				{
					return serverIsRunning;
				}
			}
			public static class MessagesLogged
			{
				public static void Set(bool logged)
				{
					data.multiplayerLogMessagesToConsole = logged;
				}
				public static bool Get()
				{
					return data.multiplayerLogMessagesToConsole;
				}
			}
		}
		public static class Client
		{
			public static class Connection
			{
				public static class IP
				{
					public static class SameDevice
					{
						public static string Get()
						{
							return "127.0.0.1";
						}
					}
					public static class Current
					{
						public static void Set(string ip)
						{
							data.ip = ip;
							if (string.IsNullOrEmpty(data.clientID.Trim()))
							{
								Console.Log.Message.Do("IPs cannot be 'null' or empty.");
								return;
							}
						}
						public static string Get()
						{
							return data.ip;
						}
					}
				}
				public static class Entered
				{
					public static void Set(bool entered)
					{
						if (entered)
						{
							if (clientIsConnected)
							{
								Console.Log.Message.Do("Already connecting/connected.");
								return;
							}
							if (serverIsRunning)
							{
								Console.Log.Message.Do("Cannot connect as Client while hosting a Server.");
								return;
							}

							try
							{
								client = new Simple.Client(data.ip, (int)serverPort);
							}
							catch (Exception)
							{
								Console.Log.Message.Do($"The IP '{data.ip}' is invalid.");
								return;
							}
							Console.Log.Message.Do($"Entering connection '{data.ip}:{serverPort}' as Client...");
							client.ConnectAsync();
							return;
						}
						if (clientIsConnected == false)
						{
							Console.Log.Message.Do("Cannot disconnect when not connected as Client.");
							return;
						}
						client.DisconnectAndStop();
					}
					public static bool Get()
					{
						return clientIsConnected;
					}
				}
				public static class IDs
				{
					public static int Get()
					{
						return clientIDs.Count;
					}
				}
			}
			public static class ID
			{
				public static void Set(string clientID)
				{
					data.clientID = clientID;
					if (string.IsNullOrEmpty(data.clientID.Trim()))
					{
						Console.Log.Message.Do("Clients' IDs cannot be 'null' or empty.");
						return;
					}
				}
				public static string Get()
				{
					return data.clientID;
				}
			}
		}
		public static class SendMessage
		{
			public static class Public
			{
				public static void Do(MessageReceiver receiver, string message)
				{
					if (MessageDisconnected()) return;

					var log = "";
					switch (receiver)
					{
						case MessageReceiver.Server:
							{
								if (clientIsConnected)
								{
									client.SendAsync($"{multiplayerMsgSep}{(int)MessageType.ClientToServer}{multiplayerMsgComponentSep}" +
										$"{data.clientID}{multiplayerMsgComponentSep}{message}");
									log = $"Message sent to Server: {message}";
								}
								break;
							}
						case MessageReceiver.AllClients:
							{
								if (clientIsConnected)
								{
									client.SendAsync($"{multiplayerMsgSep}{(int)MessageType.ClientToAll}{multiplayerMsgComponentSep}" +
										$"{data.clientID}{multiplayerMsgComponentSep}{message}");
								}
								else if (serverIsRunning)
								{
									server.Multicast($"{multiplayerMsgSep}{(int)MessageType.ServerToAll}{multiplayerMsgComponentSep}{message}");
								}
								log = $"Message sent to all Clients: {message}";
								break;
							}
						case MessageReceiver.ServerAndAllClients:
							{
								if (clientIsConnected)
								{
									client.SendAsync($"{multiplayerMsgSep}{(int)MessageType.ClientToAllAndServer}{multiplayerMsgComponentSep}" +
										$"{data.clientID}{multiplayerMsgComponentSep}{message}");
								}
								else if (serverIsRunning)
								{
									server.Multicast($"{multiplayerMsgSep}{(int)MessageType.ServerToAll}{multiplayerMsgComponentSep}{message}");
								}
								log = $"Message sent to Server & all Clients: {message}";
								break;
							}
					}
					Console.Log.Message.Do(log, log != "");
				}
			}
			public static class Private
			{
				public static void Do(string clientID, string message)
				{
					if (MessageDisconnected() || clientID == data.clientID) return;

					if (clientIsConnected)
					{
						client.SendAsync($"{multiplayerMsgSep}{(int)MessageType.ClientToClient}{multiplayerMsgComponentSep}" +
							$"{clientID}{multiplayerMsgComponentSep}{clientID}{multiplayerMsgComponentSep}{message}");
					}
					else if (serverIsRunning)
					{
						server.Multicast($"{multiplayerMsgSep}{(int)MessageType.ServerToClient}{multiplayerMsgComponentSep}" +
							$"{clientID}{multiplayerMsgComponentSep}{message}");
					}
					Console.Log.Message.Do($"Message sent to Client [{clientID}]: {message}");
				}
			}
		}

		private static bool MessageDisconnected()
		{
			if (clientIsConnected == false && serverIsRunning == false)
			{
				if (data.multiplayerLogMessagesToConsole == false) return true;
				Console.Log.Message.Do("Cannot send a message while disconnected.");
				return true;
			}
			return false;
		}
	}
	public static class Asset
	{
		public static class Loading
		{
			public static double Get()
			{
				return percentLoaded;
			}
			public static void Do(AssetType type, string filePath)
			{
				queuedAssets.Add(new QueuedAsset()
				{
					asset = type,
					path = filePath,
					error =
					$"{Text.Repeated.Get("~", 50)}\n" +
					$"- At file: {Debug.Code.File.Path.Get(1)}\n" +
					$"- At method: {Debug.Code.MethodName.Get(1)}\n" +
					$"- At line: {Debug.Code.LineNumber.Get(1)} | {Debug.Code.MethodName.Get(0)}\n\n" +
					$"Failed to load asset {type} from file '{filePath}'.\n\n" +
					$"(This message will not appear after the game is built)\n" +
					$"{Text.Repeated.Get("~", 50)}"
				});
			}
		}
		public static class Loaded
		{
			public static bool Get(string path)
			{
				return textures.ContainsKey(path) || fonts.ContainsKey(path) ||
					sounds.ContainsKey(path) || music.ContainsKey(path);
			}
		}
	}

	private class UnitInstance
	{
		[JsonProperty]
		public Dictionary<UnitEffect, double> shaderArgs;
		[JsonProperty]
		public Dictionary<UnitFact, bool> facts;
		[JsonProperty]
		public Dictionary<UnitNumber, double> numbers;
		[JsonProperty]
		public Dictionary<UnitText, string> texts;
		[JsonProperty]
		public Dictionary<UnitTexts, List<string>> textCollections;
		[JsonProperty]
		public double preParaAng;
		[JsonProperty]
		public Vector2f preParaSc, preParaPos;

		public Shader shader;
		public Sprite sprite;
		public SFML.Graphics.Text text;

		public RenderTexture renderTexture;
		public byte[] rawTextureData;
		public Texture rawTexture;
		public Image image;

		public UnitInstance(string ID)
		{
			data.objects[ID] = this;
			textCollections = new Dictionary<UnitTexts, List<string>>();
			textCollections[UnitTexts.ChildrenIDs] = new List<string>();
			shaderArgs = new Dictionary<UnitEffect, double>();
			numbers = new Dictionary<UnitNumber, double>();
			texts = new Dictionary<UnitText, string>();
			facts = new Dictionary<UnitFact, bool>();
			sprite = new Sprite();
			text = new SFML.Graphics.Text();

			shader = new Shader("shaders.vert", null, "shaders.frag");

			texts[UnitText.ID] = ID;

			var picked = Instance.PickedIDs.Get();
			Instance.PickedIDs.Set(ID);

			Instance.Unit.Appearance.Depth.Set(0);
			Instance.Unit.Area.Scale.Set(1, 1, false);
			Instance.Unit.Text.Style.Set(TextStyle.Regular);
			Instance.Unit.Text.Spacing.Letter.Set(1);
			Instance.Unit.Text.Spacing.Line.Set(1);
			Instance.Unit.Text.Size.Set(32);
			Instance.Unit.Text.Outline.Opacity.Set(0);
			Instance.Unit.Text.Outline.Size.Set(2);

			Instance.Unit.Effect.Blink.Speed.Set(100);
			Instance.Unit.Effect.Outline.Offset.Set(20);
			Instance.Unit.Effect.Stretch.Speed.Set(50, 50);
			Instance.Unit.Effect.Blur.Strength.Set(50, 50);
			Instance.Unit.Effect.Water.Strength.Set(50, 50);
			Instance.Unit.Effect.Water.Speed.Set(50, 50);
			Instance.Unit.Effect.Edge.Sensitivity.Set(50);
			Instance.Unit.Effect.Edge.Thickness.Set(50);
			Instance.Unit.Effect.Pixelate.Strength.Set(50);
			Instance.Unit.Effect.Earthquake.Strength.Set(20, 30);
			Instance.Unit.Effect.Grid.CellSize.Set(5, 5);
			Instance.Unit.Effect.Grid.CellSpacing.Set(20, 20);
			Instance.Unit.Effect.Wind.Speed.Set(10, 10);
			Instance.Unit.Effect.Wave.Speed.Set(20, 20, 20, 20);
			Instance.Unit.Appearance.Mask.Color.Set(100, 0, 100);

			Instance.PickedIDs.Set(picked);
		}

		public void SetShaderArg(string arg, double value)
		{
			shader.SetUniform(arg, (float)value);
			render = true; // TODO validate
		}
		public void Update()
		{
			var picked = Instance.PickedIDs.Get();
			var objectID = texts[UnitText.ID];
			Instance.PickedIDs.Set(objectID);
			if (Instance.Unit.Existance.Disabled.Get() || Instance.Unit.Appearance.Hidden.Get()) return;

			shader.SetUniform("time", time.ElapsedTime.AsSeconds());
			Instance.PickedIDs.Set(picked);
		}

		public void Draw()
		{
			var picked = Instance.PickedIDs.Get();
			var objectID = texts[UnitText.ID];
			Instance.PickedIDs.Set(objectID);
			if (Instance.Unit.Existance.Disabled.Get() || Instance.Unit.Appearance.Hidden.Get())
			{
				Instance.PickedIDs.Set(picked);
				return;
			}

			var parentID = Instance.Unit.Identity.FamilyIDs.Parent.Get();

			var parent = parentID == null ? world.Transform :
				data.objects[texts[UnitText.ParentID]].sprite.Transform;

			// apply parallax by Get (parallax is only applied on Get to avoid accumulating lerp)
			var paraPos = new Vector2f(
				(float)Instance.Unit.Area.Position.GetX(false),
				(float)Instance.Unit.Area.Position.GetY(false));
			var newX = camera.Transform.TransformPoint(paraPos).X;
			var newY = camera.Transform.TransformPoint(paraPos).Y;

			var oldPos = sprite.Position;
			var oldSc = sprite.Scale;
			var oldAng = sprite.Rotation;

			var newPos = new Vector2f((float)newX, (float)newY);
			sprite.Position = newPos;
			sprite.Scale = new Vector2f(
				(float)Instance.Unit.Area.Scale.GetWidth(false),
				(float)Instance.Unit.Area.Scale.GetHeight(false));
			sprite.Rotation = (float)Instance.Unit.Area.Angle.Get(false);

			if (sprite != null && sprite.Texture != null) shader.SetUniform("Texture", sprite.Texture);

			var rend = new RenderStates(BlendMode.Alpha, parent, null, shader);
			var isMask = Instance.Unit.Appearance.Mask.IDs.Target.Get() != null;

			DrawText(rend);
			DrawMasks();

			if (isMask == false && sprite != null)
			{
				var repX = Instance.Unit.Texture.Repeats.GetX() + 1;
				var repY = Instance.Unit.Texture.Repeats.GetY() + 1;

				for (int j = 0; j < repY; j++)
				{
					for (int i = 0; i < repX; i++)
					{
						var w = sprite.TextureRect.Width;
						var h = sprite.TextureRect.Height;
						var p = sprite.Transform.TransformPoint(new Vector2f((newPos.X + w) * i, (newPos.Y + h) * j));

						sprite.Position = newPos;
						sprite.Position = p;
						window.Draw(sprite, rend);
					}
				}
			}

			sprite.Position = oldPos;
			sprite.Scale = oldSc;
			sprite.Rotation = oldAng;

			Instance.PickedIDs.Set(picked);
		}
		public void DrawText(RenderStates rend)
		{
			if (texts.ContainsKey(UnitText.Text) == false || texts.ContainsKey(UnitText.FontPath) == false) return;

			renderTexture.Clear(new SFML.Graphics.Color()); //255, 0, 0));

			text.Position = new Vector2f((text.OutlineThickness + text.CharacterSize) * sprite.Scale.X / 3, 0);
			renderTexture.Draw(text);

			renderTexture.Display();
			sprite.Texture = renderTexture.Texture;
		}
		public void DrawMasks()
		{
			if (textCollections.ContainsKey(UnitTexts.MaskIDs) == false) return;

			renderTexture.Clear();
			renderTexture.Texture.Update(rawTextureData);

			var maskIDs = textCollections[UnitTexts.MaskIDs];
			foreach (var id in maskIDs)
			{
				var obj = data.objects[id];
				var posOld = obj.sprite.Position;
				var scOld = obj.sprite.Scale;
				var aOld = obj.sprite.Rotation;
				var orOld = obj.sprite.Origin;

				var sc = new Vector2f(scOld.X / sprite.Scale.X, scOld.Y / sprite.Scale.Y);
				var a = -(sprite.Rotation - aOld);
				obj.sprite.Scale = sc;
				obj.sprite.Rotation = a;
				obj.sprite.Origin *= 2;
				var p2 = new Vector2f(posOld.X, posOld.Y);
				var p = sprite.InverseTransform.TransformPoint(p2);
				//p += new Vector2f(sprite.TextureRect.Width / 4, sprite.TextureRect.Height / 4);
				obj.sprite.Position = p;

				var parentID = Instance.Unit.Identity.FamilyIDs.Parent.Get();
				var parent = parentID == null ? world.Transform :
					data.objects[obj.texts[UnitText.ParentID]].sprite.Transform;
				var rend = new RenderStates(BlendMode.Alpha, parent, null, obj.shader);

				renderTexture.Draw(obj.sprite, rend);

				obj.sprite.Position = posOld;
				obj.sprite.Scale = scOld;
				obj.sprite.Rotation = aOld;
				obj.sprite.Origin = orOld;
			}
			renderTexture.Display();
			sprite.Texture = renderTexture.Texture;
		}
		public void UpdateChildrenGlobalAngle()
		{
			var children = textCollections[UnitTexts.ChildrenIDs];
			foreach (var child in children)
			{
				var globalAngle = sprite.Rotation + data.objects[child].sprite.Rotation;
				data.objects[child].numbers[UnitNumber.GlobalAngle] =
					Number.Limited.Get(globalAngle, 0, 359, NumberLimitation.Overflow);
			}
		}
		public void UpdateChildrenGlobalPos()
		{
			var children = textCollections[UnitTexts.ChildrenIDs];
			foreach (var child in children)
			{
				var globalPos = data.objects[texts[UnitText.ID]].sprite.Transform.TransformPoint(
					data.objects[child].sprite.Position);
				data.objects[child].numbers[UnitNumber.GlobalX] = globalPos.X;
				data.objects[child].numbers[UnitNumber.GlobalY] = globalPos.Y;
			}
		}
		public void UpdateChildrenGlobalScale()
		{
			var children = textCollections[UnitTexts.ChildrenIDs];
			foreach (var child in children)
			{
				var globalScale = sprite.Scale + data.objects[child].sprite.Scale;
				data.objects[child].numbers[UnitNumber.GlobalScaleWidth] = globalScale.X;
				data.objects[child].numbers[UnitNumber.GlobalScaleHeight] = globalScale.Y;
			}
		}
		public void ApplyParallax(bool apply)
		{
			var picked = Instance.PickedIDs.Get();
			Instance.PickedIDs.Set(texts[UnitText.ID]);

			if (apply)
			{
				preParaPos = sprite.Position;
				preParaAng = sprite.Rotation;
				preParaSc = sprite.Scale;
			}

			var paraX = 100 - Instance.Unit.Appearance.Parallax.Position.GetX();
			var paraY = 100 - Instance.Unit.Appearance.Parallax.Position.GetY();
			var paraAng = 100 - Instance.Unit.Appearance.Parallax.Angle.Get();
			var paraSc = 100 - Instance.Unit.Appearance.Parallax.Scale.Get();

			var newX = apply ? Number.Converted.From.Percent.Get(paraX, -camera.Position.X, sprite.Position.X) : preParaPos.X;
			var newY = apply ? Number.Converted.From.Percent.Get(paraY, -camera.Position.Y, sprite.Position.Y) : preParaPos.Y;
			var newAng = apply ? Number.Converted.From.Percent.Get(paraAng, -camera.Rotation, sprite.Rotation) : preParaAng;
			var newW = apply ? Number.Converted.From.Percent.Get(paraSc,
				camera.Scale.X * sprite.Scale.X, sprite.Scale.X) : preParaSc.X;
			var newH = apply ? Number.Converted.From.Percent.Get(paraSc,
				camera.Scale.Y * sprite.Scale.Y, sprite.Scale.Y) : preParaSc.Y;

			Instance.Unit.Area.Position.Set(newX, newY, false);
			Instance.Unit.Area.Angle.Set(newAng, false);
			Instance.Unit.Area.Scale.Set(newW, newH, false);

			Instance.PickedIDs.Set(picked);
		}
	}
	#region Multiplayer
	private enum MessageType
	{
		Connection, ChangeID, ClientConnected, ClientDisconnected, ClientOnline, ClientToAll, ClientToClient, ClientToServer, ServerToAll, ServerToClient, ClientToAllAndServer
	}
	private class Session : TcpSession
	{
		public Session(TcpServer server) : base(server) { }

		protected override void OnConnected()
		{
			// Send invite message
			//string message = "Hello from TCP! Please send a message!";
			//SendAsync(message);
		}
		protected override void OnDisconnected()
		{
			var disconnectedClient = clientRealIDs[Id.ToString()];
			clientIDs.Remove(disconnectedClient);
			server.Multicast($"{multiplayerMsgSep}{(int)MessageType.ClientDisconnected}{multiplayerMsgComponentSep}{disconnectedClient}");

			Console.Log.Message.Do($"Client [{disconnectedClient}] disconnected.");
			game.OnMultiplayerClientDisconnected(disconnectedClient);
		}
		protected override void OnReceived(byte[] buffer, long offset, long size)
		{
			var rawMessages = Encoding.UTF8.GetString(buffer, (int)offset, (int)size);
			var messages = rawMessages.Split(multiplayerMsgSep, StringSplitOptions.RemoveEmptyEntries);
			var messageBack = "";
			foreach (var message in messages)
			{
				var components = message.Split(multiplayerMsgComponentSep);
				var messageType = (MessageType)int.Parse(components[0]);
				switch (messageType)
				{
					case MessageType.Connection: // A client just connected and sent his ID & unique name
						{
							var id = components[1];
							var clientID = components[2];
							if (clientIDs.Contains(clientID)) // Is the unique name free?
							{
								clientID = ChangeID(clientID);
								// Send a message back with a free one toward the same ID so the client can recognize it's for him
								messageBack = $"{multiplayerMsgSep}{(int)MessageType.ChangeID}{multiplayerMsgComponentSep}" +
									$"{id}{multiplayerMsgComponentSep}{clientID}";
							}
							clientRealIDs[Id.ToString()] = clientID;
							clientIDs.Add(clientID);

							// Sticking another message to update the newcoming client about online clients
							messageBack = $"{messageBack}{multiplayerMsgSep}{(int)MessageType.ClientOnline}{multiplayerMsgComponentSep}" +
								$"{clientID}";
							foreach (var ID in clientIDs)
							{
								messageBack = $"{messageBack}{multiplayerMsgComponentSep}{ID}";
							}

							// Sticking a third message to update online clients about the newcomer.
							messageBack =
								$"{messageBack}{multiplayerMsgSep}{(int)MessageType.ClientConnected}{multiplayerMsgComponentSep}" +
								$"{clientID}";
							Console.Log.Message.Do($"Client [{clientID}] connected.");
							game.OnMultiplayerClientConnected(clientID);
							break;
						}
					case MessageType.ClientToAll: // A client wants to send a message to everyone
						{
							messageBack = $"{messageBack}{multiplayerMsgSep}{message}";
							break;
						}
					case MessageType.ClientToClient: // A client wants to send a message to another client
						{
							messageBack = $"{messageBack}{multiplayerMsgSep}{message}";
							break;
						}
					case MessageType.ClientToServer: // A client sent me (the server) a message
						{
							if (data.multiplayerLogMessagesToConsole)
							{
								Console.Log.Message.Do($"Message received from Client [{components[1]}]: {components[2]}");
							}
							game.OnMultiplayerMessageReceived(components[1], components[2]);
							break;
						}
					case MessageType.ClientToAllAndServer: // A client is sending me (the server) and all other clients a message
						{
							if (data.multiplayerLogMessagesToConsole)
							{
								Console.Log.Message.Do($"Message received from Client [{components[1]}]: {components[2]}");
							}
							game.OnMultiplayerMessageReceived(components[1], components[2]);
							messageBack = $"{messageBack}{multiplayerMsgSep}{message}";
							break;
						}
				}
			}
			if (messageBack != "") server.Multicast(messageBack);
		}
		protected override void OnError(SocketError error)
		{
			Console.Log.Message.Do($"Error: {error}");
		}
		private static string ChangeID(string ID)
		{
			var i = 0;
			while (true)
			{
				i++;
				if (clientIDs.Contains(ID + i) == false) break;
			}
			return $"{ID}{i}";
		}
	}
	private class Server : TcpServer
	{
		public Server(IPAddress address, int port) : base(address, port) { }
		protected override TcpSession CreateSession() { return new Session(this); }
		protected override void OnError(SocketError error)
		{
			serverIsRunning = false;
			Console.Log.Message.Do($"Error: {error}");
		}
	}
	private class Client : TcpClient
	{
		private bool stop;

		public Client(string address, int port) : base(address, port) { }

		public void DisconnectAndStop()
		{
			stop = true;
			DisconnectAsync();
			while (IsConnected) Thread.Yield();
		}
		protected override void OnConnected()
		{
			clientIsConnected = true;
			clientIDs.Add(data.clientID);
			Console.Log.Message.Do($"Connected as Client [{data.clientID}] to {client.Socket.RemoteEndPoint}.");
			game.OnMultiplayerConnected();
			client.SendAsync($"{multiplayerMsgSep}{(int)MessageType.Connection}{multiplayerMsgComponentSep}" +
				$"{client.Id}{multiplayerMsgComponentSep}{data.clientID}");
		}
		protected override void OnDisconnected()
		{
			if (clientIsConnected)
			{
				clientIsConnected = false;
				Console.Log.Message.Do("Disconnected.");
				clientIDs.Clear();
				game.OnMultiplayerDisconnected();
				if (stop == true) return;
			}

			// Wait for a while...
			Thread.Sleep(1000);

			// Try to connect again
			Console.Log.Message.Do("Lost connection. Trying to reconnect...");
			ConnectAsync();
		}
		protected override void OnReceived(byte[] buffer, long offset, long size)
		{
			var rawMessages = Encoding.UTF8.GetString(buffer, (int)offset, (int)size);
			var messages = rawMessages.Split(multiplayerMsgSep, StringSplitOptions.RemoveEmptyEntries);
			var messageBack = "";
			foreach (var message in messages)
			{
				var components = message.Split(multiplayerMsgComponentSep);
				var messageType = (MessageType)int.Parse(components[0]);
				switch (messageType)
				{
					case MessageType.ChangeID: // Server said someone's ID is taken and sent a free one
						{
							if (components[1] == client.Id.ToString()) // Is this for me?
							{
								var oldID = data.clientID;
								var newID = components[2];
								clientIDs.Remove(oldID);
								clientIDs.Add(newID);

								Console.Log.Message.Do($"Client ID [{oldID}] is taken. New Client ID is [{newID}].");
								game.OnMultiplayerTakenID(newID);
							}
							break;
						}
					case MessageType.ClientConnected: // Server said some client connected
						{
							var ID = components[1];
							if (ID != data.clientID) // If not me
							{
								clientIDs.Add(ID);
								Console.Log.Message.Do($"Client [{components[1]}] connected.");
								game.OnMultiplayerClientConnected(ID);
							}
							break;
						}
					case MessageType.ClientDisconnected: // Server said some client disconnected
						{
							var ID = components[1];
							clientIDs.Remove(ID);
							Console.Log.Message.Do($"Client [{components[1]}] disconnected.");
							game.OnMultiplayerClientDisconnected(ID);
							break;
						}
					case MessageType.ClientOnline: // Someone just connected and is getting updated on who is already online
						{
							var ID = components[1];
							if (ID == data.clientID) // For me?
							{
								for (int i = 2; i < components.Length; i++)
								{
									var curClientID = components[i];

									if (clientIDs.Contains(curClientID) == false)
									{
										clientIDs.Add(curClientID);
									}
								}
							}
							Console.Log.Message.Do("");
							break;
						}
					case MessageType.ClientToAll: // A client is sending a message to all clients
						{
							var ID = components[1];
							if (ID == data.clientID) break; // Is this my message coming back to me?
							if (data.multiplayerLogMessagesToConsole)
							{
								Console.Log.Message.Do($"Message received from Client [{components[1]}]: {components[2]}");
							}
							game.OnMultiplayerMessageReceived(ID, components[2]);
							break;
						}
					case MessageType.ClientToAllAndServer: // A client is sending a message to the server and all clients
						{
							var ID = components[1];
							if (ID == data.clientID) break; // Is this my message coming back to me?
							if (data.multiplayerLogMessagesToConsole)
							{
								Console.Log.Message.Do($"Message received from Client [{components[1]}]: {components[2]}");
							}
							game.OnMultiplayerMessageReceived(ID, components[2]);
							break;
						}
					case MessageType.ClientToClient: // A client is sending a message to another client
						{
							var ID = components[1];
							if (ID == data.clientID) return; // Is this my message coming back to me? (unlikely)
							if (components[2] != data.clientID) return; // Not for me?

							if (data.multiplayerLogMessagesToConsole)
							{
								Console.Log.Message.Do($"Message received from Client [{components[1]}]: {components[3]}");
							}
							game.OnMultiplayerMessageReceived(ID, components[3]);
							break;
						}
					case MessageType.ServerToAll: // The server sent everyone a message
						{
							if (data.multiplayerLogMessagesToConsole)
							{
								Console.Log.Message.Do($"Message received from Server: {components[1]}");
							}
							game.OnMultiplayerMessageReceived(null, components[1]);
							break;
						}
					case MessageType.ServerToClient: // The server sent some client a message
						{
							if (components[1] != data.clientID) return; // Not for me?

							if (data.multiplayerLogMessagesToConsole)
							{
								Console.Log.Message.Do($"Message received from Server: {components[1]}");
							}
							game.OnMultiplayerMessageReceived(null, components[1]);
							break;
						}
				}
			}
			if (messageBack != "") client.SendAsync(messageBack);
		}
		protected override void OnError(SocketError error)
		{
			clientIsConnected = false;
			Console.Log.Message.Do($"Error: {error}");
		}
	}
	#endregion

	private static class Statics
	{
		public static SFML.Graphics.Color GetColorFromSample(Color color)
		{
			byte r = 0;
			byte g = 0;
			byte b = 0;
			switch (color)
			{
				case Color.White: r = 255; g = 255; b = 255; break;
				case Color.Black: r = 0; g = 0; b = 0; break;

				case Color.LightGray: r = 175; g = 175; b = 175; break;
				case Color.Gray: r = 125; g = 125; b = 125; break;
				case Color.DarkGray: r = 75; g = 75; b = 75; break;

				case Color.LightRed: r = 255; g = 125; b = 125; break;
				case Color.Red: r = 255; g = 0; b = 0; break;
				case Color.DarkRed: r = 125; g = 0; b = 0; break;

				case Color.LightGreen: r = 125; g = 255; b = 125; break;
				case Color.Green: r = 0; g = 255; b = 0; break;
				case Color.DarkGreen: r = 0; g = 125; b = 0; break;

				case Color.LightBlue: r = 125; g = 125; b = 255; break;
				case Color.Blue: r = 0; g = 0; b = 255; break;
				case Color.DarkBlue: r = 0; g = 0; b = 125; break;

				case Color.LightYellow: r = 255; g = 255; b = 125; break;
				case Color.Yellow: r = 255; g = 255; b = 0; break;
				case Color.DarkYellow: r = 125; g = 125; b = 0; break;

				case Color.LightMagenta: r = 255; g = 125; b = 255; break;
				case Color.Magenta: r = 255; g = 0; b = 255; break;
				case Color.DarkMagenta: r = 125; g = 0; b = 125; break;

				case Color.LightCyan: r = 125; g = 255; b = 255; break;
				case Color.Cyan: r = 0; g = 255; b = 255; break;
				case Color.DarkCyan: r = 0; g = 125; b = 125; break;
			}
			return new SFML.Graphics.Color(r, g, b);
		}
		public static string CultureDecimalPoint()
		{
			return CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
		}
		public static void ConsoleShow()
		{
			if (data.consoleIsShown) return;

			AllocConsole();

			ConsoleSelectionEnable(false);
			System.Console.Title = $"Simple Console | {Game.Title.Get()}";
			data.consoleIsShown = true;
		}
		public static void ConsoleSelectionEnable(bool enabled)
		{
			//QuickEdit lets the user select text in the console window with the mouse, to copy to the windows clipboard.
			//But selecting text stops the console process (e.g. unzipping). This may not be always wanted.
			IntPtr consoleHandle = GetStdHandle((int)StdHandle.STD_INPUT_HANDLE);
			UInt32 consoleMode;

			GetConsoleMode(consoleHandle, out consoleMode);
			if (enabled) consoleMode |= ((uint)ConsoleMode.ENABLE_QUICK_EDIT_MODE);
			else consoleMode &= ~((uint)ConsoleMode.ENABLE_QUICK_EDIT_MODE);

			consoleMode |= ((uint)ConsoleMode.ENABLE_EXTENDED_FLAGS);

			SetConsoleMode(consoleHandle, consoleMode);
		}
		public static bool TryCast<T>(object obj, out T result)
		{
			if (obj is T)
			{
				result = (T)obj;
				return true;
			}

			result = typeof(T) == typeof(double) ? (T)Convert.ChangeType(double.NaN, typeof(T)) : default(T);
			return false;
		}
		public static Vector2f Normalize(Vector2f vec)
		{
			var distance = (float)Math.Sqrt(vec.X * vec.X + vec.Y * vec.Y);
			var result = new Vector2f(vec.X / distance, vec.Y / distance);
			result.X = double.IsNaN(result.X) ? 0 : result.X;
			result.Y = double.IsNaN(result.Y) ? 0 : result.Y;
			return result;
		}
		public static double Distance(Vector2f vec1, Vector2f vec2)
		{
			return Math.Sqrt(Math.Pow((vec2.X - vec1.X), 2) + Math.Pow((vec2.Y - vec1.Y), 2));
		}
		public static Vector2f AngleToVec(double ang)
		{
			var rad = Math.PI / 180 * ang;
			var dir = new Vector2f((float)Math.Cos(rad), (float)Math.Sin(rad));
			dir = Normalize(dir);
			return dir;
		}
		public static double VecToAngle(Vector2f vec)
		{
			vec = Normalize(vec);
			var rad = (double)Math.Atan2(vec.Y, vec.X);
			return Number.Limited.Get(rad * (180 / Math.PI), 0, 359, NumberLimitation.Overflow);
		}
		public static void ShowError(int depth, string description)
		{
			if (Debug.Running.Get() == false) return;

			var result =
				$"{Text.Repeated.Get("~", 50)}\n" +
				$"- At file: {Debug.Code.File.Path.Get(depth + 1)}\n" +
				$"- At method: {Debug.Code.MethodName.Get(depth + 1)}\n" +
				$"- At line: {Debug.Code.LineNumber.Get(depth + 1)} | {Debug.Code.MethodName.Get(depth)}\n\n" +
				$"{description}\n\n" +
				$"(This message will not appear after the game is built)\n" +
				$"{Text.Repeated.Get("~", 50)}";
			Console.Log.Message.Do(result);
		}
		public static bool NoIDPickedError()
		{
			var picked = Instance.PickedIDs.Get();
			Instance.PickedIDs.Set("");
			if (picked.Length == 0) ShowError(2,
				$"No ID is picked to perform actions on.\n\n{GetPickAdvices()}");
			Instance.PickedIDs.Set(picked);
			return picked.Length == 0;
		}
		public static bool MultipleIDsPickedError()
		{
			var picked = Instance.PickedIDs.Get();
			if (picked.Length > 1) Statics.ShowError(2,
				"Multiple IDs are picked for this Get method.\n" +
				$"Only the first ID will be used to return a value.\n\n{GetPickAdvices()}");
			return picked.Length > 1;
		}
		public static bool DoesNotExistError<T, T1>(Dictionary<T, T1> dict, string type)
		{
			var ID = Instance.PickedIDs.Get()[0];
			var notFound = data.objects.ContainsKey(ID) == false;
			if (notFound) ShowError(2, $"{type} with ID '{ID}' was not found.\n\n{GetNotFoundAdvices()}");
			return notFound;
		}
		public static string CreateDirectoryForFile(string filePath)
		{
			filePath = filePath.Replace('/', '\\');
			var path = filePath.Split('\\');
			var full = $"{directory}{filePath}";
			var curPath = directory;
			for (int i = 0; i < path.Length - 1; i++)
			{
				var p = $"{curPath}\\{path[i]}";
				if (Directory.Exists(p) == false) Directory.CreateDirectory(p);

				curPath = p;
			}
			return full;
		}
		public static Vector2f GetDirectionFromSample(Direction sample)
		{
			var result = new Vector2f();
			switch (sample)
			{
				case Direction.Up: result = new Vector2f(0, -1); break;
				case Direction.Down: result = new Vector2f(0, 1); break;
				case Direction.Left: result = new Vector2f(-1, 0); break;
				case Direction.Right: result = new Vector2f(1, 0); break;
				case Direction.UpRight: result = new Vector2f(1, -1); break;
				case Direction.UpLeft: result = new Vector2f(-1, -1); break;
				case Direction.DownRight: result = new Vector2f(1, 1); break;
				case Direction.DownLeft: result = new Vector2f(-1, 1); break;
			}
			result = Normalize(result);
			return result;
		}

		private static string GetPickAdvices()
		{
			return
				"'Picking' advices:\n" +
				"- Make sure picking happens before the action methods.\n" +
				$"- Use '{nameof(Instance.PickedIDs)}{nameof(Instance.PickedIDs.Set)}(\"myObjectID\", \"myObjectID2\", ...)' " +
				$"to pick objects.\n" +
				"- Make sure to pick only one ID before Get methods.";
		}
		private static string GetNotFoundAdvices()
		{
			return
				"'Finding object' advices:\n" +
				"- Make sure the object is created by calling any of its non-Get methods.\n" +
				"- Make sure the ID is typed correctly.\n";
		}
	}
}
