using SFML.Graphics;
using SFML.Window;
using System;
using System.Security.Permissions;
using System.Windows.Forms;

namespace SMPL
{
	public abstract class Window
	{
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
		private static Color backgroundColor;

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
		public static bool ForceDraw { get; set; }
		public static Color BackgroundColor
		{
			get { return backgroundColor; }
			set
			{
				if (backgroundColor == value) return;
				backgroundColor = value;
				ForceDraw = true;
			}
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

		}

		internal void Initialize()
		{
			var w = (int)VideoMode.DesktopMode.Width;
			var h = (int)VideoMode.DesktopMode.Height;
			
			form = new Form();
			form.SetBounds(w, h, w / 2, h / 2);

			Game.window = this;
			window = new RenderWindow(form.Handle);
			window.SetVisible(true);
			window.SetTitle($"{nameof(SMPL)} Game");

			window.Closed += new EventHandler(OnClose);
			window.GainedFocus += new EventHandler(OnFocus);
			window.LostFocus += new EventHandler(OnUnfocus);
			form.SizeChanged += new EventHandler(OnResize);

			BackgroundColor = Color.Black;
			ForceDraw = true;
		}
		public virtual void OnClose() { }
		public virtual void OnFocus() { }
		public virtual void OnUnfocus() { }
		public virtual void OnResize() { }
		public virtual void OnMinimize() { }
		public virtual void OnMaximize() { }
		public virtual void OnDraw() { }

		internal void OnClose(object sender, EventArgs e)
		{
			OnClose();
			Game.Stop();
		}
		internal void OnFocus(object sender, EventArgs e) => OnFocus();
		internal void OnUnfocus(object sender, EventArgs e) => OnUnfocus();
		internal void OnResize(object sender, EventArgs e)
		{
			switch (currentState)
			{
				case State.Floating: OnResize(); break;
				case State.Minimized: OnMinimize(); break;
				case State.Maximized: OnMaximize(); break;
			}
		}
	}
}
