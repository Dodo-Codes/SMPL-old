using SFML.Graphics;
using SFML.Window;
using SFML.System;
using System;
using System.Security.Permissions;
using System.Threading;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using static SMPL.Events;

namespace SMPL
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

		private static State currentState;
		private static Type currentType;
		private static bool resizable;

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

		private static bool hasVsync;
		public static bool HasVerticalSync
		{
			get { return hasVsync; }
			set
			{
				hasVsync = value;
				window.SetVerticalSyncEnabled(value);
			}
		}

		internal static Form form;
		internal static RenderWindow window;
		internal static bool isDrawing;

		public static void Close()
      {
			for (int i = 0; i < instances.Count; i++) instances[i].OnEarlyWindowClose();
			for (int i = 0; i < instances.Count; i++) instances[i].OnWindowClose();
			for (int i = 0; i < instances.Count; i++) instances[i].OnLateWindowClose();
			window.Close();
		}
		public static void RequestFocus() => window.RequestFocus();

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
			Camera.WorldCamera = new(new Point(0, 0), size / pixelSize);
			Camera.WorldCamera.TransformComponent.Size = size;
			window.SetView(Camera.WorldCamera.view);

			CurrentType = Type.Normal;
			IsResizable = true;
		}
	}
}
