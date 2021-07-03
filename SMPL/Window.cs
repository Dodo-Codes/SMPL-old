using SFML.Graphics;
using SFML.Window;
using SFML.System;
using System;
using System.Security.Permissions;
using System.Threading;
using System.Windows.Forms;
using System.Collections.Generic;

namespace SMPL
{
	public static class Window
	{
		internal delegate void DrawHandler();
		internal static event DrawHandler DrawEvent;

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
		public static bool Hidden
		{
			get { return !form.Visible; }
			set { form.Visible = !value; }
		}
		public static bool Resizable
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

		internal static Form form;
		internal static RenderWindow window;

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
			while (window.IsOpen)
			{
				Thread.Sleep(1);
				Application.DoEvents();
				window.DispatchEvents();

				Time.frameCount++;
				window.SetActive(true);
				window.Clear();

				Camera.WorldCamera.DrawComponent.StartDraw();
				DrawEvent();
				WorldCameraEvents.instance.OnDraw();
				Camera.WorldCamera.DrawComponent.EndDraw();
				WorldCameraEvents.Display();

				window.Display();
				Time.frameDeltaTime.Restart();
				window.SetActive(false);
			}
		}

		internal static void Initialize()
		{
			var w = (int)VideoMode.DesktopMode.Width;
			var h = (int)VideoMode.DesktopMode.Height;
			
			form = new Form();
			form.SetBounds(w, h, w / 2, h / 2);

			window = new RenderWindow(form.Handle);
			window.SetVisible(true);
			window.SetTitle($"{nameof(SMPL)} Game");
		}
	}
}
