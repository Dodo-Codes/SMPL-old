using SFML.Window;
using SMPL.Data;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using SFML.System;
using SFML.Graphics;

namespace SMPL.Gear
{
	public static class Mouse
	{
		public enum Button
		{
			Unknown = -1, Left = 0, Right = 1, Middle = 2, ExtraButton1 = 3, ExtraButton2 = 4
		}
		public enum ButtonAction
		{
			Press, Release, Hold, DoubleClick
		}
		public enum Wheel
		{
			Vertical, Horizontal
		}

		public static class Cursor
		{
			public enum Type
			{
				Arrow, ArrowWait, Wait, Text, Hand, SizeHorinzontal, SizeVertical, SizeTopLeftBottomRight, SizeBottomLeftTopRight, SizeAll,
				Cross, Help, NotAllowed
			}
			public enum Action
			{
				WindowEnter, WindowLeave, Move
			}

			internal static Point lastFrameCursorPosScr;
			public static Point PositionWindow
			{
				get
				{
					var mousePos = SFML.Window.Mouse.GetPosition(Window.window);
					var pos = Window.window.MapPixelToCoords(mousePos);
					return new Point((int)pos.X, (int)pos.Y);
				}
			}
			public static Point PositionScreen
			{
				get { var pos = System.Windows.Forms.Cursor.Position; return new Point(pos.X, pos.Y); }
				set { System.Windows.Forms.Cursor.Position = new System.Drawing.Point((int)value.X, (int)value.Y); }
			}
			private static bool isHidden;
			public static bool IsHidden
			{
				get { return isHidden; }
				set { isHidden = value; Window.window.SetMouseCursorVisible(!value); }
			}
			private static Type currentType;
			public static Type CurrentType
			{
				get { return currentType; }
				set { currentType = value; Window.window.SetMouseCursor(new SFML.Window.Cursor((SFML.Window.Cursor.CursorType)value)); }
			}
			public static void SetTextureFromFile(string filePath, Size size, Point origin = default)
			{
				var img = default(Image);
				try { img = new Image(filePath); }
				catch (Exception)
				{
					Debug.LogError(1, $"Could not set {nameof(Cursor)} texture from file '{filePath}'.");
					return;
				}

				var cursor = new SFML.Window.Cursor(img.Pixels,
					new Vector2u((uint)size.W, (uint)size.H),
					new Vector2u((uint)origin.X, (uint)origin.Y));
				Window.window.SetMouseCursor(cursor);
			}

			internal static void OnMouseCursorEnterWindow(object sender, EventArgs e) =>
				Events.Notify(Game.Event.MouseCursorEnterWindow);
			internal static void OnMouseCursorLeaveWindow(object sender, EventArgs e) =>
				Events.Notify(Game.Event.MouseCursorLeaveWindow);
		}

		internal static List<Button> buttonsHeld = new();
		private static bool pressEventHappened;
		public static Button[] ButtonsPressed { get { return buttonsHeld.ToArray(); } }

		public static bool ButtonIsPressed(Button button)
		{
			return Window.HasFocus && SFML.Window.Mouse.IsButtonPressed((SFML.Window.Mouse.Button)button);
		}
		internal static void Initialize()
		{
			Window.window.MouseButtonPressed += new EventHandler<MouseButtonEventArgs>(OnMouseButtonPress);
			Window.window.MouseButtonReleased += new EventHandler<MouseButtonEventArgs>(OnMouseButtonRelease);
			Window.form.MouseDoubleClick += new MouseEventHandler(OnMouseButtonDoubleClick);
			Window.window.MouseEntered += new EventHandler(Cursor.OnMouseCursorEnterWindow);
			Window.window.MouseLeft += new EventHandler(Cursor.OnMouseCursorLeaveWindow);
			Window.window.MouseWheelScrolled += new EventHandler<MouseWheelScrollEventArgs>(OnMouseWheelScroll);
		}
		internal static void Update()
		{
			for (int i = 0; i < buttonsHeld.Count; i++)
				Events.Notify(Game.Event.MouseButtonHold, new() { Button = buttonsHeld[i] });
			Cursor.lastFrameCursorPosScr = Cursor.PositionScreen;
		}
		internal static void CancelInput()
		{
			for (int i = 0; i < buttonsHeld.Count; i++)
				Events.Notify(Game.Event.MouseButtonRelease, new() { Button = buttonsHeld[i]});
			buttonsHeld.Clear();
		}

		internal static void OnMouseButtonPress(object sender, EventArgs e)
		{
			var buttonArgs = (MouseButtonEventArgs)e;
			var button = (Button)buttonArgs.Button;
			buttonsHeld.Add(button);
			Events.Notify(Game.Event.MouseButtonPress, new Events.EventArgs() { Button = button });
			pressEventHappened = true;
		}
		internal static void OnMouseButtonRelease(object sender, EventArgs e)
		{
			var buttonArgs = (MouseButtonEventArgs)e;
			var button = (Button)buttonArgs.Button;
			buttonsHeld.Remove(button);
			// sometimes the press does press event does not trigger when double clicking but thankfully the release does,
			// allowing for a dirty workaroundy hacky fixy dixy
			if (pressEventHappened == false)
				Events.Notify(Game.Event.MouseButtonPress, new Events.EventArgs() { Button = button });
			Events.Notify(Game.Event.MouseButtonRelease, new Events.EventArgs() { Button = button });
			pressEventHappened = false;
		}
		internal static void OnMouseButtonDoubleClick(object sender, EventArgs e)
		{
			var buttonArgs = (MouseEventArgs)e;
			var button = Button.Unknown;
			switch (buttonArgs.Button)
			{
				case MouseButtons.Left: button = Button.Left; break;
				case MouseButtons.Right: button = Button.Right; break;
				case MouseButtons.Middle: button = Button.Middle; break;
				case MouseButtons.XButton1: button = Button.ExtraButton1; break;
				case MouseButtons.XButton2: button = Button.ExtraButton2; break;
			}
			Events.Notify(Game.Event.MouseButtonDoubleClick, new Events.EventArgs() { Button = button });
		}
		internal static void OnMouseWheelScroll(object sender, EventArgs e)
		{
			var arguments = (MouseWheelScrollEventArgs)e;
			var wheel = (Wheel)arguments.Wheel;
			Events.Notify(Game.Event.MouseWheelScroll, new() { Wheel = wheel, Double = new double[] { arguments.Delta } });
		}
	}
}
