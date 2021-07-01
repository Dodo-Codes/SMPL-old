using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SMPL
{
	public abstract class MouseEvents
	{
		internal void Subscribe()
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
			var button = (Mouse.Button)buttonArgs.Button;
			OnButtonPress(button);
		}
		internal void OnButtonRelease(object sender, EventArgs e)
		{
			var buttonArgs = (MouseButtonEventArgs)e;
			var button = (Mouse.Button)buttonArgs.Button;
			OnButtonRelease(button);
		}
		internal void OnButtonDoubleClick(object sender, EventArgs e)
		{
			var buttonArgs = (MouseEventArgs)e;
			var button = Mouse.Button.Unknown;
			switch (buttonArgs.Button)
			{
				case MouseButtons.Left: button = Mouse.Button.Left; break;
				case MouseButtons.Right: button = Mouse.Button.Right; break;
				case MouseButtons.Middle: button = Mouse.Button.Middle; break;
				case MouseButtons.XButton1: button = Mouse.Button.ExtraButton1; break;
				case MouseButtons.XButton2: button = Mouse.Button.ExtraButton2; break;
			}
			OnButtonDoubleClick(button);
		}
		internal void OnWheelScroll(object sender, EventArgs e)
		{
			var arguments = (MouseWheelScrollEventArgs)e;
			var wheel = (Mouse.Wheel)arguments.Wheel;
			OnWheelScroll(arguments.Delta, wheel);
		}

		public virtual void OnCursorMove() { }
		public virtual void OnCursorEnterWindow() { }
		public virtual void OnCursorLeaveWindow() { }
		public virtual void OnButtonDoubleClick(Mouse.Button button) { }
		public virtual void OnButtonPress(Mouse.Button button) { }
		public virtual void OnButtonRelease(Mouse.Button button) { }
		public virtual void OnWheelScroll(double delta, Mouse.Wheel wheel) { }
	}
}
