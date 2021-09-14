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
		public enum Wheel
		{
			Vertical, Horizontal
		}

		public static class Cursor
		{
			private static event Events.ParamsZero OnCursorWindowEnter, OnCursorWindowLeave;
			public enum Type
			{
				Arrow, ArrowWait, Wait, Text, Hand, SizeHorinzontal, SizeVertical, SizeTopLeftBottomRight, SizeBottomLeftTopRight, SizeAll,
				Cross, Help, NotAllowed
			}

			public static class CallWhen
			{
				public static void WindowEnter(Action method, uint order = uint.MaxValue) =>
				OnCursorWindowEnter = Events.Add(OnCursorWindowEnter, method, order);
				public static void WindowLeave(Action method, uint order = uint.MaxValue) =>
					OnCursorWindowLeave = Events.Add(OnCursorWindowLeave, method, order);
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

			internal static void OnMouseCursorEnterWindow(object sender, EventArgs e) => OnCursorWindowEnter?.Invoke();
			internal static void OnMouseCursorLeaveWindow(object sender, EventArgs e) => OnCursorWindowLeave?.Invoke();
		}

		public static Button[] ButtonsPressed { get { return buttonsHeld.ToArray(); } }
		internal static List<Button> buttonsHeld = new();
		public static bool ButtonIsPressed(Button button) => SFML.Window.Mouse.IsButtonPressed((SFML.Window.Mouse.Button)button);

		private static event Events.ParamsOne<Button> OnButtonDoubleClick, OnButtonPress, OnButtonHold, OnButtonRelease;
		private static event Events.ParamsTwo<Wheel, double> OnWheelScroll;

		public static class CallWhen
		{
			public static void ButtonDoubleClick(Action<Button> method, uint order = uint.MaxValue) =>
				OnButtonDoubleClick = Events.Add(OnButtonDoubleClick, method, order);
			public static void ButtonPress(Action<Button> method, uint order = uint.MaxValue) =>
				OnButtonPress = Events.Add(OnButtonPress, method, order);
			public static void ButtonHold(Action<Button> method, uint order = uint.MaxValue) =>
				OnButtonHold = Events.Add(OnButtonHold, method, order);
			public static void ButtonRelease(Action<Button> method, uint order = uint.MaxValue) =>
				OnButtonRelease = Events.Add(OnButtonRelease, method, order);
			public static void WheelScroll(Action<Wheel, double> method, uint order = uint.MaxValue) =>
				OnWheelScroll = Events.Add(OnWheelScroll, method, order);
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
			for (int i = 0; i < buttonsHeld.Count; i++) OnButtonHold?.Invoke(buttonsHeld[i]);
			Cursor.lastFrameCursorPosScr = Cursor.PositionScreen;
		}
		internal static void CancelInput()
		{
			for (int i = 0; i < buttonsHeld.Count; i++) OnButtonHold?.Invoke(buttonsHeld[i]);
			buttonsHeld.Clear();
		}

		internal static void OnMouseButtonPress(object sender, EventArgs e)
		{
			var buttonArgs = (MouseButtonEventArgs)e;
			var button = (Button)buttonArgs.Button;
			buttonsHeld.Add(button);
			OnButtonPress?.Invoke(button);
		}
		internal static void OnMouseButtonRelease(object sender, EventArgs e)
		{
			var buttonArgs = (MouseButtonEventArgs)e;
			var button = (Button)buttonArgs.Button;
			buttonsHeld.Remove(button);
			OnButtonRelease?.Invoke(button);
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
			OnButtonDoubleClick?.Invoke(button);
		}
		internal static void OnMouseWheelScroll(object sender, EventArgs e)
		{
			var arguments = (MouseWheelScrollEventArgs)e;
			var wheel = (Wheel)arguments.Wheel;
			OnWheelScroll?.Invoke(wheel, arguments.Delta);
		}
	}
}
