using System;

namespace SMPL
{
	public abstract class WindowEvents
	{
		internal static WindowEvents instance;

		internal void Subscribe()
		{
			instance = this;

			Window.window.Closed += new EventHandler(OnClose);
			Window.window.GainedFocus += new EventHandler(OnFocus);
			Window.window.LostFocus += new EventHandler(OnUnfocus);
			Window.form.SizeChanged += new EventHandler(OnResize);
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
			Window.ForceDraw = true;
			switch (Window.CurrentState)
			{
				case Window.State.Floating: OnResize(); break;
				case Window.State.Minimized: OnMinimize(); break;
				case Window.State.Maximized: OnMaximize(); break;
			}
		}
	}
}
