using SFML.Window;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace SMPL
{
	public abstract class Events
	{
		internal static List<Events> instances = new();

		internal static void Update()
		{
			foreach (var i in instances) i.OnEachFrameEarly();
			foreach (var i in instances) i.OnEachFrame();
			foreach (var i in instances) i.OnEachFrameLate();

			var timerUIDs = IdentityComponent<Timer>.GetAllUniqueIDs();
			foreach (var uid in timerUIDs)
			{
				var timer = IdentityComponent<Timer>.PickByUniqueID(uid);
				if (timer.IsPaused) continue;
				if (timer.Countdown > 0) timer.Countdown -= Time.DeltaTime;
				if (Gate.EnterOnceWhile(uid, timer.Countdown <= 0))
				{
					timer.EndCount++;
					timer.Countdown = 0;
               foreach (var i in instances) i.OnTimerEnd(timer);
				}
			}
		}

		public void Subscribe(Events instance)
		{
         if (instances.Contains(instance))
         {
				Debug.LogError(1, $"The instance of '{instance.GetType()}' is already subscribed.");
				return;
         }
			instances.Add(instance);

			Window.window.Closed += new EventHandler(OnWindowClose);
			Window.window.GainedFocus += new EventHandler(OnWindowFocus);
			Window.window.LostFocus += new EventHandler(OnWindowUnfocus);
			Window.form.SizeChanged += new EventHandler(OnWindowResize);

			Window.window.KeyPressed += new EventHandler<SFML.Window.KeyEventArgs>(OnKeyPress);
			Window.window.KeyReleased += new EventHandler<SFML.Window.KeyEventArgs>(OnKeyRelease);
			Window.window.SetKeyRepeatEnabled(false);
			Window.form.KeyPress += new KeyPressEventHandler(OnTextInput);
			Window.form.InputLanguageChanged += new InputLanguageChangedEventHandler(OnLanguageChange);

			Window.window.MouseButtonPressed += new EventHandler<MouseButtonEventArgs>(OnMouseButtonPress);
			Window.window.MouseButtonReleased += new EventHandler<MouseButtonEventArgs>(OnMouseButtonRelease);
			Window.form.MouseDoubleClick += new MouseEventHandler(OnMouseButtonDoubleClick);
			Window.window.MouseMoved += new EventHandler<MouseMoveEventArgs>(OnMouseCursorMove);
			Window.window.MouseEntered += new EventHandler(OnMouseCursorEnterWindow);
			Window.window.MouseLeft += new EventHandler(OnMouseCursorLeaveWindow);
			Window.window.MouseWheelScrolled += new EventHandler<MouseWheelScrollEventArgs>(OnMouseWheelScroll);
		}
		public static void Unsubscribe(Events instance)
		{
			if (instances.Contains(instance) == false)
			{
				Debug.LogError(1, $"The instance of '{instance.GetType()}' is not subscribed.");
				return;
			}
			instances.Remove(instance);
		}

		internal void OnWindowClose(object sender, EventArgs e) => Window.Close();
		internal void OnWindowFocus(object sender, EventArgs e) => OnWindowFocus();
		internal void OnWindowUnfocus(object sender, EventArgs e) => OnWindowUnfocus();
		internal void OnWindowResize(object sender, EventArgs e)
		{
			switch (Window.CurrentState)
			{
				case Window.State.Floating: OnWindowResize(); break;
				case Window.State.Minimized: OnWindowMinimize(); break;
				case Window.State.Maximized: OnWindowMaximize(); break;
			}
		}

		internal void OnKeyPress(object sender, EventArgs e)
		{
			var keyArgs = (SFML.Window.KeyEventArgs)e;
			var key = (Keyboard.Key)keyArgs.Code;
			OnKeyPress(key);
		}
		internal void OnKeyRelease(object sender, EventArgs e)
		{
			var keyArgs = (SFML.Window.KeyEventArgs)e;
			var key = (Keyboard.Key)keyArgs.Code;
			OnKeyRelease(key);
		}
		internal void OnTextInput(object sender, EventArgs e)
		{
			var keyArgs = (KeyPressEventArgs)e;
			var keyStr = keyArgs.KeyChar.ToString();
			keyStr = keyStr.Replace('\r', '\n');
			if (keyStr == "\b") keyStr = "";
			OnTextInput(keyStr, keyStr == "\b", keyStr == Environment.NewLine, keyStr == "\t");
		}
		internal void OnLanguageChange(object sender, EventArgs e)
		{
			var langArgs = (InputLanguageChangedEventArgs)e;
			var culture = langArgs.InputLanguage.Culture;
			OnLanguageChange(culture.EnglishName, culture.NativeName, culture.Name);
		}

		public virtual void OnStartEarly() { }
		public virtual void OnStart() { }
		public virtual void OnStartLate() { }
		public virtual void OnDraw(Camera camera) { }
		public virtual void OnEachFrameEarly() { }
		public virtual void OnEachFrame() { }
		public virtual void OnEachFrameLate() { }
		public virtual void OnTimerEnd(Timer timerInstance) { }

		public virtual void OnWindowClose() { }
		public virtual void OnWindowFocus() { }
		public virtual void OnWindowUnfocus() { }
		public virtual void OnWindowResize() { }
		public virtual void OnWindowMinimize() { }
		public virtual void OnWindowMaximize() { }

		public virtual void OnAssetsLoadingStart() { }
		public virtual void OnAssetsLoadingUpdate() { }
		public virtual void OnAssetsLoadingEnd() { }

		public virtual void OnKeyPress(Keyboard.Key key) { }
		public virtual void OnKeyRelease(Keyboard.Key key) { }
		public virtual void OnTextInput(string textSymbol, bool isBackspace, bool isEnter, bool isTab) { }
		public virtual void OnLanguageChange(string englishName, string nativeName, string languageCode) { }

		internal void OnMouseCursorMove(object sender, EventArgs e) => OnMouseCursorMove();
		internal void OnMouseCursorEnterWindow(object sender, EventArgs e) => OnMouseCursorEnterWindow();
		internal void OnMouseCursorLeaveWindow(object sender, EventArgs e) => OnMouseCursorLeaveWindow();
		internal void OnMouseButtonPress(object sender, EventArgs e)
		{
			var buttonArgs = (MouseButtonEventArgs)e;
			var button = (Mouse.Button)buttonArgs.Button;
			OnMouseButtonPress(button);
		}
		internal void OnMouseButtonRelease(object sender, EventArgs e)
		{
			var buttonArgs = (MouseButtonEventArgs)e;
			var button = (Mouse.Button)buttonArgs.Button;
			OnMouseButtonRelease(button);
		}
		internal void OnMouseButtonDoubleClick(object sender, EventArgs e)
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
			OnMouseButtonDoubleClick(button);
		}
		internal void OnMouseWheelScroll(object sender, EventArgs e)
		{
			var arguments = (MouseWheelScrollEventArgs)e;
			var wheel = (Mouse.Wheel)arguments.Wheel;
			OnMouseWheelScroll(arguments.Delta, wheel);
		}

		public virtual void OnMouseCursorMove() { }
		public virtual void OnMouseCursorEnterWindow() { }
		public virtual void OnMouseCursorLeaveWindow() { }
		public virtual void OnMouseButtonDoubleClick(Mouse.Button button) { }
		public virtual void OnMouseButtonPress(Mouse.Button button) { }
		public virtual void OnMouseButtonRelease(Mouse.Button button) { }
		public virtual void OnMouseWheelScroll(double delta, Mouse.Wheel wheel) { }

		public virtual void OnMultiplayerTakenClientUniqueID(string newClientUniqueID) { }
		public virtual void OnMultiplayerClientConnect(string clientUniqueID) { }
		public virtual void OnMultiplayerClientDisconnect(string clientUniqueID) { }
		public virtual void OnMultiplayerMessageReceived(Multiplayer.Message message) { }

		public virtual void OnAudioStart(Audio audio) { }
		public virtual void OnAudioPlay(Audio audio) { }
		public virtual void OnAudioPause(Audio audio) { }
		public virtual void OnAudioStop(Audio audio) { }
		public virtual void OnAudioEnd(Audio audio) { }
	}
}
