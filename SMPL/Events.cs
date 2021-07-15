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
			for (int i = 0; i < instances.Count; i++) instances[i].OnEarlyEachFrame();
			for (int i = 0; i < instances.Count; i++) instances[i].OnEachFrame();
			for (int i = 0; i < instances.Count; i++) instances[i].OnLateEachFrame();

			var timerUIDs = IdentityComponent<Timer>.GetAllUniqueIDs();
			for (int j = 0; j < timerUIDs.Length; j++)
			{
				var timer = IdentityComponent<Timer>.PickByUniqueID(timerUIDs[j]);
				if (timer.IsPaused) continue;
				if (timer.Countdown > 0) timer.Countdown -= Time.DeltaTime;
				if (Gate.EnterOnceWhile(timerUIDs[j] + "as;li3'f2", timer.Countdown <= 0))
				{
					timer.EndCount++;
					timer.Countdown = 0;
               for (int i = 0; i < instances.Count; i++) instances[i].OnEarlyTimerEnd(timer);
               for (int i = 0; i < instances.Count; i++) instances[i].OnTimerEnd(timer);
					for (int i = 0; i < instances.Count; i++) instances[i].OnLateTimerEnd(timer);
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
		internal void OnWindowFocus(object sender, EventArgs e)
		{
			OnEarlyWindowFocus();
			OnWindowFocus();
			OnLateWindowFocus();
		}
		internal void OnWindowUnfocus(object sender, EventArgs e)
		{
			OnEarlyWindowFocus();
			OnWindowUnfocus();
			OnLateWindowFocus();
		}
		internal void OnWindowResize(object sender, EventArgs e)
		{
			switch (Window.CurrentState)
			{
				case Window.State.Floating: { OnEarlyWindowResize(); OnWindowResize(); OnLateWindowResize(); break; }
				case Window.State.Minimized: { OnEarlyWindowMinimize(); OnWindowMinimize(); OnLateWindowMinimize(); break; }
				case Window.State.Maximized: { OnEarlyWindowMaximize(); OnWindowMaximize(); OnLateWindowMaximize(); break; }
			}
		}

		internal void OnKeyPress(object sender, EventArgs e)
		{
			var keyArgs = (SFML.Window.KeyEventArgs)e;
			var key = (Keyboard.Key)keyArgs.Code;
			OnEarlyKeyPress(key);
			OnKeyPress(key);
			OnLateKeyPress(key);
		}
		internal void OnKeyRelease(object sender, EventArgs e)
		{
			var keyArgs = (SFML.Window.KeyEventArgs)e;
			var key = (Keyboard.Key)keyArgs.Code;
			OnEarlyKeyRelease(key);
			OnKeyRelease(key);
			OnLateKeyRelease(key);
		}
		internal void OnTextInput(object sender, EventArgs e)
		{
			var keyArgs = (KeyPressEventArgs)e;
			var keyStr = keyArgs.KeyChar.ToString();
			keyStr = keyStr.Replace('\r', '\n');
			if (keyStr == "\b") keyStr = "";
			var isBackSpace = keyStr == "\b";
			var isEnter = keyStr == Environment.NewLine;
			var isTab = keyStr == "\t";
			OnEarlyTextInput(keyStr, isBackSpace, isEnter, isTab);
			OnTextInput(keyStr, isBackSpace, isEnter, isTab);
			OnLateTextInput(keyStr, isBackSpace, isEnter, isTab);
		}
		internal void OnLanguageChange(object sender, EventArgs e)
		{
			var langArgs = (InputLanguageChangedEventArgs)e;
			var culture = langArgs.InputLanguage.Culture;
			OnEarlyLanguageChange(culture.EnglishName, culture.NativeName, culture.Name);
			OnLanguageChange(culture.EnglishName, culture.NativeName, culture.Name);
			OnLateLanguageChange(culture.EnglishName, culture.NativeName, culture.Name);
		}

		internal void OnMouseCursorMove(object sender, EventArgs e)
		{
			OnEarlyMouseCursorMove();
			OnMouseCursorMove();
			OnLateMouseCursorMove();
		}
		internal void OnMouseCursorEnterWindow(object sender, EventArgs e)
		{
			OnEarlyMouseCursorEnterWindow();
			OnMouseCursorEnterWindow();
			OnLateMouseCursorEnterWindow();
		}
		internal void OnMouseCursorLeaveWindow(object sender, EventArgs e)
		{
			OnEarlyMouseCursorLeaveWindow();
			OnMouseCursorLeaveWindow();
			OnLateMouseCursorLeaveWindow();
		}

		internal void OnMouseButtonPress(object sender, EventArgs e)
		{
			var buttonArgs = (MouseButtonEventArgs)e;
			var button = (Mouse.Button)buttonArgs.Button;
			OnEarlyMouseButtonPress(button);
			OnMouseButtonPress(button);
			OnLateMouseButtonPress(button);
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
		//=================================================================

		public virtual void OnEarlyStart() { }
		public virtual void OnEarlyEachFrame() { }
		public virtual void OnEarlyDraw(Camera camera) { }
		public virtual void OnEarlyTimerEnd(Timer timerInstance) { }
		public virtual void OnStart() { }
		public virtual void OnDraw(Camera camera) { }
		public virtual void OnEachFrame() { }
		public virtual void OnTimerEnd(Timer timerInstance) { }
		public virtual void OnLateStart() { }
		public virtual void OnLateDraw(Camera camera) { }
		public virtual void OnLateEachFrame() { }
		public virtual void OnLateTimerEnd(Timer timerInstance) { }

		public virtual void OnEarlyWindowClose() { }
		public virtual void OnEarlyWindowFocus() { }
		public virtual void OnEarlyWindowUnfocus() { }
		public virtual void OnEarlyWindowResize() { }
		public virtual void OnEarlyWindowMinimize() { }
		public virtual void OnEarlyWindowMaximize() { }
		public virtual void OnWindowClose() { }
		public virtual void OnWindowFocus() { }
		public virtual void OnWindowUnfocus() { }
		public virtual void OnWindowResize() { }
		public virtual void OnWindowMinimize() { }
		public virtual void OnWindowMaximize() { }
		public virtual void OnLateWindowClose() { }
		public virtual void OnLateWindowFocus() { }
		public virtual void OnLateWindowUnfocus() { }
		public virtual void OnLateWindowResize() { }
		public virtual void OnLateWindowMinimize() { }
		public virtual void OnLateWindowMaximize() { }

		public virtual void OnEarlyAssetsLoadingStart() { }
		public virtual void OnEarlyAssetsLoadingUpdate() { }
		public virtual void OnEarlyAssetsLoadingEnd() { }
		public virtual void OnAssetsLoadingStart() { }
		public virtual void OnAssetsLoadingUpdate() { }
		public virtual void OnAssetsLoadingEnd() { }
		public virtual void OnLateAssetsLoadingStart() { }
		public virtual void OnLateAssetsLoadingUpdate() { }
		public virtual void OnLateAssetsLoadingEnd() { }

		public virtual void OnEarlyKeyPress(Keyboard.Key key) { }
		public virtual void OnEarlyKeyRelease(Keyboard.Key key) { }
		public virtual void OnEarlyTextInput(string textSymbol, bool isBackspace, bool isEnter, bool isTab) { }
		public virtual void OnEarlyLanguageChange(string englishName, string nativeName, string languageCode) { }
		public virtual void OnKeyPress(Keyboard.Key key) { }
		public virtual void OnKeyRelease(Keyboard.Key key) { }
		public virtual void OnTextInput(string textSymbol, bool isBackspace, bool isEnter, bool isTab) { }
		public virtual void OnLanguageChange(string englishName, string nativeName, string languageCode) { }
		public virtual void OnLateKeyPress(Keyboard.Key key) { }
		public virtual void OnLateKeyRelease(Keyboard.Key key) { }
		public virtual void OnLateTextInput(string textSymbol, bool isBackspace, bool isEnter, bool isTab) { }
		public virtual void OnLateLanguageChange(string englishName, string nativeName, string languageCode) { }

		public virtual void OnEarlyMouseCursorMove() { }
		public virtual void OnEarlyMouseCursorEnterWindow() { }
		public virtual void OnEarlyMouseCursorLeaveWindow() { }
		public virtual void OnEarlyMouseButtonDoubleClick(Mouse.Button button) { }
		public virtual void OnEarlyMouseButtonPress(Mouse.Button button) { }
		public virtual void OnEarlyMouseButtonRelease(Mouse.Button button) { }
		public virtual void OnEarlyMouseWheelScroll(double delta, Mouse.Wheel wheel) { }
		public virtual void OnMouseCursorMove() { }
		public virtual void OnMouseCursorEnterWindow() { }
		public virtual void OnMouseCursorLeaveWindow() { }
		public virtual void OnMouseButtonDoubleClick(Mouse.Button button) { }
		public virtual void OnMouseButtonPress(Mouse.Button button) { }
		public virtual void OnMouseButtonRelease(Mouse.Button button) { }
		public virtual void OnMouseWheelScroll(double delta, Mouse.Wheel wheel) { }
		public virtual void OnLateMouseCursorMove() { }
		public virtual void OnLateMouseCursorEnterWindow() { }
		public virtual void OnLateMouseCursorLeaveWindow() { }
		public virtual void OnLateMouseButtonDoubleClick(Mouse.Button button) { }
		public virtual void OnLateMouseButtonPress(Mouse.Button button) { }
		public virtual void OnLateMouseButtonRelease(Mouse.Button button) { }
		public virtual void OnLateMouseWheelScroll(double delta, Mouse.Wheel wheel) { }

		public virtual void OnEarlyMultiplayerTakenClientUniqueID(string newClientUniqueID) { }
		public virtual void OnEarlyMultiplayerClientConnect(string clientUniqueID) { }
		public virtual void OnEarlyMultiplayerClientDisconnect(string clientUniqueID) { }
		public virtual void OnEarlyMultiplayerMessageReceived(Multiplayer.Message message) { }
		public virtual void OnMultiplayerTakenClientUniqueID(string newClientUniqueID) { }
		public virtual void OnMultiplayerClientConnect(string clientUniqueID) { }
		public virtual void OnMultiplayerClientDisconnect(string clientUniqueID) { }
		public virtual void OnMultiplayerMessageReceived(Multiplayer.Message message) { }
		public virtual void OnLateMultiplayerTakenClientUniqueID(string newClientUniqueID) { }
		public virtual void OnLateMultiplayerClientConnect(string clientUniqueID) { }
		public virtual void OnLateMultiplayerClientDisconnect(string clientUniqueID) { }
		public virtual void OnLateMultiplayerMessageReceived(Multiplayer.Message message) { }

		public virtual void OnEarlyAudioStart(Audio audio) { }
		public virtual void OnEarlyAudioPlay(Audio audio) { }
		public virtual void OnEarlyAudioPause(Audio audio) { }
		public virtual void OnEarlyAudioStop(Audio audio) { }
		public virtual void OnEarlyAudioEnd(Audio audio) { }
		public virtual void OnAudioStart(Audio audio) { }
		public virtual void OnAudioPlay(Audio audio) { }
		public virtual void OnAudioPause(Audio audio) { }
		public virtual void OnAudioStop(Audio audio) { }
		public virtual void OnAudioEnd(Audio audio) { }
		public virtual void OnLateAudioStart(Audio audio) { }
		public virtual void OnLateAudioPlay(Audio audio) { }
		public virtual void OnLateAudioPause(Audio audio) { }
		public virtual void OnLateAudioStop(Audio audio) { }
		public virtual void OnLateAudioEnd(Audio audio) { }
	}
}
