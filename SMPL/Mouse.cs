using SFML.Window;
using System;
using System.Windows.Forms;

namespace SMPL
{
	public abstract class Mouse
	{
		public enum MouseButton
		{
			Unknown = -1, Left = 0, Right = 1, Middle = 2, ExtraButton1 = 3, ExtraButton2 = 4
		}
		public enum MouseWheel
		{
			Vertical, Horizontal
		}
		public enum MouseWheelRoll
		{

		}

		internal void Initialize()
		{
			Window.window.MouseButtonPressed += new EventHandler<MouseButtonEventArgs>(OnButtonPress);
			Window.window.MouseButtonReleased += new EventHandler<MouseButtonEventArgs>(OnButtonRelease);
			Window.form.MouseDoubleClick += new MouseEventHandler(OnButtonDoubleClick);
			Window.window.MouseMoved += new EventHandler<MouseMoveEventArgs>(OnCursorMove);
			Window.window.MouseEntered += new EventHandler(OnCursorEnterWindow);
			Window.window.MouseLeft += new EventHandler(OnCursorLeaveWindow);
			Window.window.MouseWheelScrolled += new EventHandler<MouseWheelScrollEventArgs>(OnWheelScroll);
		}

		internal void OnCursorMove(object sender, EventArgs e)
		{
			OnCursorMove();
		}
		internal void OnCursorEnterWindow(object sender, EventArgs e)
		{
			OnCursorEnterWindow();
		}
		internal void OnCursorLeaveWindow(object sender, EventArgs e)
		{
			OnCursorLeaveWindow();
		}
		internal void OnButtonPress(object sender, EventArgs e)
		{
			var buttonArgs = (MouseButtonEventArgs)e;
			var button = (MouseButton)buttonArgs.Button;
			OnButtonPress(button);
		}
		internal void OnButtonRelease(object sender, EventArgs e)
		{
			var buttonArgs = (MouseButtonEventArgs)e;
			var button = (MouseButton)buttonArgs.Button;
			OnButtonRelease(button);
		}
		internal void OnButtonDoubleClick(object sender, EventArgs e)
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
			OnButtonDoubleClick(button);
		}
		internal void OnWheelScroll(object sender, EventArgs e)
		{
			var arguments = (MouseWheelScrollEventArgs)e;
			var wheel = (MouseWheel)arguments.Wheel;
			OnWheelScroll(arguments.Delta, wheel);
		}

		public virtual void OnCursorMove() { }
		public virtual void OnCursorEnterWindow() { }
		public virtual void OnCursorLeaveWindow() { }
		public virtual void OnButtonDoubleClick(MouseButton button) { }
		public virtual void OnButtonPress(MouseButton button) { }
		public virtual void OnButtonRelease(MouseButton button) { }
		public virtual void OnWheelScroll(double delta, MouseWheel wheel) { }
	}
}
