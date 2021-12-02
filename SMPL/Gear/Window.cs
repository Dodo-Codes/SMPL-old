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
		public enum Action
		{
			Resize, Close, Focus, Unfocus, Maximize, Minimize, Fullscreen
		}

		private static State currentState;
		private static Type currentType;
		private static bool resizable;
		internal static SFML.Graphics.Sprite world = new();

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
			get { return form == null || form.Text == null ? "SMPL Game" : form.Text; }
			set { window.SetTitle(value); }
		}
		public static Point Position
		{
			get { return new Point(window.Position.X, window.Position.Y); }
			set { window.Position = new Vector2i((int)value.X, (int)value.Y); }
		}
		public static Size Size
		{
			get { return new Size(window.Size.X, window.Size.Y); }
			set { window.Size = new Vector2u((uint)value.W, (uint)value.H); }
		}
		public static Size PixelSize { get; internal set; }

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
				if (Assets.textures.ContainsKey(value) == false)
				{
					Assets.NotLoadedError(1, Assets.Type.Texture, value);
					return;
				}
				iconTexPath = value;
				var t = Assets.textures[value];
				window.SetIcon(t.Size.X, t.Size.Y, t.CopyToImage().Pixels);
			}
		}

		internal static Form form;
		internal static RenderWindow window;
		internal static bool isDrawing;

		public static void Close()
      {
			Events.Notify(Game.Event.WindowClose);
			Events.Notify(Game.Event.GameStop);
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

			Performance.ResetDrawCounters();
			window.SetActive(true);
			window.Clear();
			isDrawing = true;
			Camera.DrawCameras();
			isDrawing = false;
			window.Display();
			window.SetActive(false);
			Performance.prevDrawCallsPerFr = Performance.DrawCallsPerFrame;
		}

		internal static bool DrawNotAllowed()
      {
			if (isDrawing == false)
			{
				Debug.LogError(2, "Displaying is not possible outside of the 'OnDisplay' events.");
				return true;
			}
			return false;
		}
		internal static void Initialize(Size pixelSize)
		{
			form = new Form();

			PixelSize = pixelSize;
			window = new RenderWindow(form.Handle);
			window.SetVisible(true);
			window.SetTitle($"{nameof(SMPL)} Game");

			var scrSize = Screen.PrimaryScreen.Bounds;
			var size = new Size(scrSize.Width, scrSize.Height);
			form.SetBounds(scrSize.Width / 4, scrSize.Height / 4, scrSize.Width / 2, scrSize.Height / 2);

			var ar = new Area() { Size = size };
			Camera.WorldCamera = new(new Point(0, 0), size / pixelSize);
			Camera.WorldCamera.displayUID = ar.UID;
			window.SetView(Camera.WorldCamera.view);

			CurrentType = Type.Normal;
			IsResizable = true;

			window.Closed += new EventHandler(OnWindowClose);
			window.GainedFocus += new EventHandler(OnWindowFocus);
			window.LostFocus += new EventHandler(OnWindowUnfocus);
			form.SizeChanged += new EventHandler(OnWindowResize);


			Application.DoEvents();
			window.DispatchEvents();
			Draw();
		}

		internal static void OnWindowClose(object sender, EventArgs e) => Close();
		internal static void OnWindowFocus(object sender, EventArgs e) => Events.Notify(Game.Event.WindowFocus);
		internal static void OnWindowUnfocus(object sender, EventArgs e)
		{
			Mouse.CancelInput();
			Keyboard.CancelInput();
			Events.Notify(Game.Event.WindowUnfocus);
		}
		internal static void OnWindowResize(object sender, EventArgs e)
		{
			if (CurrentState != State.Fullscreen) CurrentState = (State)form.WindowState;
			switch (CurrentState)
			{
				case State.Floating: { Mouse.CancelInput(); Keyboard.CancelInput(); Events.Notify(Game.Event.WindowResize); break; }
				case State.Minimized: { Events.Notify(Game.Event.WindowResize); Events.Notify(Game.Event.WindowMinimize); break; }
				case State.Maximized: { Events.Notify(Game.Event.WindowResize); Events.Notify(Game.Event.WindowMaximize); break; }
				case State.Fullscreen: { Events.Notify(Game.Event.WindowResize); Events.Notify(Game.Event.WindowFullscreen); break; }
			}
		}
	}
}
