using SFML.Graphics;
using SFML.Window;
using SFML.System;
using System;
using System.Security.Permissions;
using System.Threading;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using SMPL.Data;
using SMPL.Components;

namespace SMPL.Gear
{
	public static class Window
	{
      #region Sleep Prevent
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

      public enum State
		{
			Floating = 0, Minimized = 1, Maximized = 2, Fullscreen = 3
		}
		public enum Type
		{
			Normal, NoIcon, Tool
		}
		public enum PopUpIcon
		{
			None, Info, Error, Warning
		}
		public enum PopUpButtons
		{
			OK = 0,
			OKCancel = 1,
			AbortRetryIgnore = 2,
			YesNoCancel = 3,
			YesNo = 4,
			RetryCancel = 5
		}
		public enum PopUpResult
		{
			None = 0,
			OK = 1,
			Cancel = 2,
			Abort = 3,
			Retry = 4,
			Ignore = 5,
			Yes = 6,
			No = 7
		}

		private static State currentState;
		private static Type currentType;
		private static bool resizable;
		internal static SFML.Graphics.Sprite world = new();

		private static event Events.ParamsZero OnResize, OnClose, OnFocus, OnUnfocus, OnMaximize, OnMinimize;

		public static class CallWhen
		{
			public static void Resize(Action method, uint order = uint.MaxValue) =>
				OnResize = Events.Add(OnResize, method, order);
			public static void Close(Action method, uint order = uint.MaxValue) =>
				OnClose = Events.Add(OnClose, method, order);
			public static void Focus(Action method, uint order = uint.MaxValue) =>
				OnFocus = Events.Add(OnFocus, method, order);
			public static void Unocus(Action method, uint order = uint.MaxValue) =>
				OnUnfocus = Events.Add(OnUnfocus, method, order);
			public static void Maximize(Action method, uint order = uint.MaxValue) =>
				OnMaximize = Events.Add(OnMaximize, method, order);
			public static void Minimize(Action method, uint order = uint.MaxValue) =>
				OnMinimize = Events.Add(OnMinimize, method, order);
		}

		public static State CurrentState
		{
			get { return currentState; }
			set
			{
				currentState = value;
				Update();
			}
		}
		public static Type CurrentType
		{
			get { return currentType; }
			set
			{
				currentType = value;
				Update();
			}
		}
		public static bool HasFocus { get { return window.HasFocus(); } }
		public static bool IsHidden
		{
			get { return !form.Visible; }
			set { form.Visible = !value; }
		}
		public static bool IsResizable
		{
			get { return resizable; }
			set
			{
				if (currentType == Type.NoIcon || currentState == State.Fullscreen) return;
				resizable = value;
				Update();
			}
		}

		public static string Title
		{
			get { return form.Text; }
			set { window.SetTitle(value); }
		}
		public static Point Position
		{
			get { return new Point(window.Position.X, window.Position.Y); }
			set { window.Position = new SFML.System.Vector2i((int)value.X, (int)value.Y); }
		}

		private static bool preventsSleep;
		public static bool PreventsSleep
		{
			get { return preventsSleep; }
         set
         {
				preventsSleep = value;
				if (value)
				{
					SetThreadExecutionState(EXECUTION_STATE.ES_CONTINUOUS | EXECUTION_STATE.ES_DISPLAY_REQUIRED | EXECUTION_STATE.ES_SYSTEM_REQUIRED);
				}
				else
				{
					SetThreadExecutionState(EXECUTION_STATE.ES_CONTINUOUS);
				}
			}
		}

		private static string iconTexPath;
		public static string IconTexturePath
		{
			get { return iconTexPath; }
			set
			{
				if (value == null)
				{
					Debug.LogError(1, $"The icon texture path cannot be 'null'.");
					return;
				}
				if (File.textures.ContainsKey(value) == false)
				{
					Debug.LogError(1, $"The texture at '{value}' is not loaded.\n" +
						$"Use '{nameof(File)}.{nameof(File.LoadAssets)} ({nameof(File)}.{nameof(File.Asset)}." +
						$"{nameof(File.Asset.Texture)}, \"{value}\")' to load it.");
					return;
				}
				iconTexPath = value;
				var t = File.textures[value];
				window.SetIcon(t.Size.X, t.Size.Y, t.CopyToImage().Pixels);
			}
		}

		internal static Form form;
		internal static RenderWindow window;
		internal static bool isDrawing;

		public static void Close()
      {
			OnClose?.Invoke();
			window.Close();
		}
		public static void RequestFocus() => window.RequestFocus();
		public static PopUpResult PopUp(object message, string title = "", PopUpIcon icon = PopUpIcon.None,
			PopUpButtons buttons = PopUpButtons.OK)
		{
			var msgIcon = MessageBoxIcon.None;
			var btn = (MessageBoxButtons)buttons;
			switch (icon)
			{
				case PopUpIcon.Info: msgIcon = MessageBoxIcon.Information; break;
				case PopUpIcon.Error: msgIcon = MessageBoxIcon.Error; break;
				case PopUpIcon.Warning: msgIcon = MessageBoxIcon.Warning; break;
				case PopUpIcon.None: msgIcon = MessageBoxIcon.None; break;
			}
			return (PopUpResult)MessageBox.Show($"{message}", title, btn, msgIcon);
		}

		private static void Update()
		{
			if (currentState == State.Fullscreen)
			{
				resizable = false;
				form.WindowState = FormWindowState.Normal;
				form.FormBorderStyle = FormBorderStyle.None;
				form.Bounds = Screen.PrimaryScreen.Bounds;
			}
			else
			{
				form.WindowState = (FormWindowState)currentState;

				var style = FormBorderStyle.None;
				switch (currentType)
				{
					case Type.Normal: style = resizable ? FormBorderStyle.Sizable : FormBorderStyle.FixedSingle; break;
					case Type.NoIcon: { style = FormBorderStyle.FixedDialog; resizable = false; break; }
					case Type.Tool: style = resizable ? FormBorderStyle.SizableToolWindow : FormBorderStyle.FixedToolWindow; break;
				}
				form.FormBorderStyle = style;
			}
		}
		internal static void Draw()
		{
			if (IsHidden) return;

			window.SetActive(true);
			window.Clear();
			isDrawing = true;
			Camera.DrawCameras();
			isDrawing = false;
			window.Display();
			window.SetActive(false);
		}

		internal static bool DrawNotAllowed()
      {
			if (isDrawing == false)
			{
				Debug.LogError(2, "Drawing is not possible outside of the 'OnDraw' events.");
				return true;
			}
			return false;
		}
		internal static void Initialize(Size pixelSize)
		{
			var w = (int)VideoMode.DesktopMode.Width;
			var h = (int)VideoMode.DesktopMode.Height;
			
			form = new Form();
			form.SetBounds(w, h, w / 2, h / 2);

			window = new RenderWindow(form.Handle);
			window.SetVisible(true);
			window.SetTitle($"{nameof(SMPL)} Game");

			var scrSize = Screen.PrimaryScreen.Bounds;
			var size = new Size(scrSize.Width, scrSize.Height);
			
			Camera.WorldCamera = new("smpl-world-camera", new Point(0, 0), size / pixelSize);
			Camera.WorldCamera.DisplayArea = new("smpl-world-camera-area");
			Camera.WorldCamera.DisplayArea.Size = size;
			window.SetView(Camera.WorldCamera.view);

			CurrentType = Type.Normal;
			IsResizable = true;

			window.Closed += new EventHandler(OnWindowClose);
			window.GainedFocus += new EventHandler(OnWindowFocus);
			window.LostFocus += new EventHandler(OnWindowUnfocus);
			form.SizeChanged += new EventHandler(OnWindowResize);
		}

		internal static void OnWindowClose(object sender, EventArgs e) => Close();
		internal static void OnWindowFocus(object sender, EventArgs e) => OnFocus?.Invoke();
		internal static void OnWindowUnfocus(object sender, EventArgs e)
		{
			Mouse.CancelInput();
			Keyboard.CancelInput();
			OnUnfocus?.Invoke();
		}
		internal static void OnWindowResize(object sender, EventArgs e)
		{
			switch (CurrentState)
			{
				case State.Floating: { Mouse.CancelInput(); Keyboard.CancelInput(); OnResize?.Invoke(); break; }
				case State.Minimized: OnMinimize?.Invoke(); break;
				case State.Maximized: OnMaximize?.Invoke(); break;
			}
		}
	}
}
