using SFML.Window;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace SMPL
{
	public abstract class Events
	{
		internal static SortedDictionary<int, List<Events>> instances = new();
		internal static Dictionary<Events, int> instancesOrder = new();
		internal static List<Component2D> transforms = new();
		internal static List<ComponentText> texts = new();
		internal static List<ComponentSprite> sprites = new();
		internal static List<Keyboard.Key> keysHeld = new();

		private int order;
		public int Order
		{
			get { return order; }
         set
         {
				if (instancesOrder.ContainsKey(this))
            {
					if (order == value) return;
					instancesOrder.Remove(this);
					instances[order].Remove(this);
            }

				order = value;

				if (instances.ContainsKey(order) == false) instances[order] = new List<Events>();
				instances[order].Add(this);
				instancesOrder.Add(this, order);
         }
		}

		internal static void Update()
		{
			var timerUIDs = ComponentIdentity<Timer>.AllUniqueIDs;
			for (int j = 0; j < timerUIDs.Length; j++)
			{
				var timer = ComponentIdentity<Timer>.PickByUniqueID(timerUIDs[j]);
				if (timer.IsPaused) continue;
				if (timer.Countdown > 0) timer.Countdown -= Time.DeltaTime;
				if (Gate.EnterOnceWhile(timerUIDs[j] + "as;li3'f2", timer.Countdown <= 0))
				{
					timer.EndCount++;
					timer.Countdown = 0;
					var n = D(instances); foreach (var kvp in n) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++)
						e[i].OnTimerEndSetup(timer); }
					var n1 = D(instances); foreach (var kvp in n1) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++)
						e[i].OnTimerEnd(timer); }
				}
			}

			{ var n = D(instances); foreach (var kvp in n) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++)
					e[i].OnEachFrameSetup(); }
			var n1 = D(instances); foreach (var kvp in n1) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++)
					e[i].OnEachFrame(); } }

			for (int i = 0; i < transforms.Count; i++) transforms[i].Update();
			for (int i = 0; i < sprites.Count; i++) sprites[i].Update();
			for (int i = 0; i < texts.Count; i++) texts[i].Update();
         for (int j = 0; j < keysHeld.Count; j++)
         {
				var n = D(instances); foreach (var kvp in n) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++)
						e[i].OnKeyHoldSetup(keysHeld[j]); }
				var n1 = D(instances); foreach (var kvp in n1) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++)
						e[i].OnKeyHold(keysHeld[j]); }
			}

			Mouse.lastFrameCursorPosScr = Mouse.CursorPositionScreen;
		}

		public void Subscribe(Events instance, int order)
		{
         if (instance == null)
         {
				Debug.LogError(1, $"The instance cannot be 'null'.");
				return;
			}
         else if (instancesOrder.ContainsKey(instance))
         {
				Debug.LogError(1, $"The instance of '{instance.GetType()}' is already subscribed.");
				return;
         }
			Order = order;

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
			if (instance == null)
			{
				Debug.LogError(1, $"The instance cannot be 'null'.");
				return;
			}
			else if (instancesOrder.ContainsKey(instance) == false)
			{
				Debug.LogError(1, $"The instance of '{instance.GetType()}' is not subscribed.");
				return;
			}
			instances[instance.Order].Remove(instance);
			instancesOrder.Remove(instance);
		}

		internal void OnWindowClose(object sender, EventArgs e) => Window.Close();
		internal void OnWindowFocus(object sender, EventArgs e)
		{
			OnWindowFocusSetup();
			OnWindowFocus();
		}
		internal void OnWindowUnfocus(object sender, EventArgs e)
		{
			CancelInput();
			OnWindowUnfocusSetup();
			OnWindowUnfocus();
		}
		internal void OnWindowResize(object sender, EventArgs e)
		{
			switch (Window.CurrentState)
			{
				case Window.State.Floating: { CancelInput(); OnWindowResizeSetup(); OnWindowResize(); break; }
				case Window.State.Minimized: { OnWindowMinimizeSetup(); OnWindowMinimize(); break; }
				case Window.State.Maximized: { OnWindowMaximizeSetup(); OnWindowMaximize(); break; }
			}
		}

		internal void OnKeyPress(object sender, EventArgs e)
		{
			var keyArgs = (SFML.Window.KeyEventArgs)e;
			var key = (Keyboard.Key)keyArgs.Code;
			keysHeld.Add(key);
			OnKeyPressSetup(key);
			OnKeyPress(key);
		}
		internal void OnKeyRelease(object sender, EventArgs e)
		{
			var keyArgs = (SFML.Window.KeyEventArgs)e;
			var key = (Keyboard.Key)keyArgs.Code;
			keysHeld.Remove(key);
			OnKeyReleaseSetup(key);
			OnKeyRelease(key);
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
			OnTextInputSetup(keyStr, isBackSpace, isEnter, isTab);
			OnTextInput(keyStr, isBackSpace, isEnter, isTab);
		}
		internal void OnLanguageChange(object sender, EventArgs e)
		{
			var langArgs = (InputLanguageChangedEventArgs)e;
			var culture = langArgs.InputLanguage.Culture;
			OnLanguageChangeSetup(culture.EnglishName, culture.NativeName, culture.Name);
			OnLanguageChange(culture.EnglishName, culture.NativeName, culture.Name);
		}

		internal void OnMouseCursorMove(object sender, EventArgs e)
		{
			var delta = Mouse.CursorPositionScreen - Mouse.lastFrameCursorPosScr;
			OnMouseCursorPositionChangeSetup(delta);
			OnMouseCursorPositionChange(delta);
		}
		internal void OnMouseCursorEnterWindow(object sender, EventArgs e)
		{
			OnMouseCursorEnterWindowSetup();
			OnMouseCursorEnterWindow();
		}
		internal void OnMouseCursorLeaveWindow(object sender, EventArgs e)
		{
			OnMouseCursorLeaveWindowSetup();
			OnMouseCursorLeaveWindow();
		}

		internal void OnMouseButtonPress(object sender, EventArgs e)
		{
			var buttonArgs = (MouseButtonEventArgs)e;
			var button = (Mouse.Button)buttonArgs.Button;
			OnMouseButtonPressSetup(button);
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
			OnMouseWheelScrollSetup(wheel, arguments.Delta);
			OnMouseWheelScroll(wheel, arguments.Delta);
		}

		internal void CancelInput()
      {
			for (int i = 0; i < keysHeld.Count; i++)
			{
				OnKeyReleaseSetup(keysHeld[i]);
				OnKeyRelease(keysHeld[i]);
			}
			keysHeld.Clear();
		}
		internal static List<T> L<T>(List<T> list) => new List<T>(list);
		internal static SortedDictionary<T, T1> D<T, T1>(SortedDictionary<T, T1> dict) => new SortedDictionary<T, T1>(dict);
		//=================================================================

		public virtual void OnStartSetup() { }
		public virtual void OnStart() { }
		public virtual void OnEachFrameSetup() { }
		public virtual void OnEachFrame() { }
		public virtual void OnDrawSetup(Camera instance) { }
		public virtual void OnDraw(Camera instance) { }
		public virtual void OnTimerEndSetup(Timer instance) { }
		public virtual void OnTimerEnd(Timer instance) { }

		public virtual void OnWindowCloseSetup() { }
		public virtual void OnWindowFocusSetup() { }
		public virtual void OnWindowUnfocusSetup() { }
		public virtual void OnWindowResizeSetup() { }
		public virtual void OnWindowMinimizeSetup() { }
		public virtual void OnWindowMaximizeSetup() { }
		public virtual void OnWindowClose() { }
		public virtual void OnWindowFocus() { }
		public virtual void OnWindowUnfocus() { }
		public virtual void OnWindowResize() { }
		public virtual void OnWindowMinimize() { }
		public virtual void OnWindowMaximize() { }

		public virtual void OnAssetsLoadingStartSetup() { }
		public virtual void OnAssetsLoadingUpdateSetup() { }
		public virtual void OnAssetsLoadingEndSetup() { }
		public virtual void OnAssetsLoadingStart() { }
		public virtual void OnAssetsLoadingUpdate() { }
		public virtual void OnAssetsLoadingEnd() { }

		public virtual void OnKeyPressSetup(Keyboard.Key key) { }
		public virtual void OnKeyHoldSetup(Keyboard.Key key) { }
		public virtual void OnKeyReleaseSetup(Keyboard.Key key) { }
		public virtual void OnTextInputSetup(string textSymbol, bool isBackspace, bool isEnter, bool isTab) { }
		public virtual void OnLanguageChangeSetup(string englishName, string nativeName, string languageCode) { }
		public virtual void OnKeyPress(Keyboard.Key key) { }
		public virtual void OnKeyHold(Keyboard.Key key) { }
		public virtual void OnKeyRelease(Keyboard.Key key) { }
		public virtual void OnTextInput(string textSymbol, bool isBackspace, bool isEnter, bool isTab) { }
		public virtual void OnLanguageChange(string englishName, string nativeName, string languageCode) { }

		public virtual void OnMouseCursorPositionChangeSetup(Point delta) { }
		public virtual void OnMouseCursorPositionChange(Point delta) { }
		public virtual void OnMouseCursorEnterWindowSetup() { }
		public virtual void OnMouseCursorEnterWindow() { }
		public virtual void OnMouseCursorLeaveWindowSetup() { }
		public virtual void OnMouseCursorLeaveWindow() { }
		public virtual void OnMouseButtonDoubleClickSetup(Mouse.Button button) { }
		public virtual void OnMouseButtonDoubleClick(Mouse.Button button) { }
		public virtual void OnMouseButtonPressSetup(Mouse.Button button) { }
		public virtual void OnMouseButtonPress(Mouse.Button button) { }
		public virtual void OnMouseButtonReleaseSetup(Mouse.Button button) { }
		public virtual void OnMouseButtonRelease(Mouse.Button button) { }
		public virtual void OnMouseWheelScrollSetup(Mouse.Wheel wheel, double delta) { }
		public virtual void OnMouseWheelScroll(Mouse.Wheel wheel, double delta) { }

		public virtual void OnMultiplayerTakenClientUniqueIDSetup(string newClientUniqueID) { }
		public virtual void OnMultiplayerClientConnectSetup(string clientUniqueID) { }
		public virtual void OnMultiplayerClientDisconnectSetup(string clientUniqueID) { }
		public virtual void OnMultiplayerMessageReceivedSetup(Multiplayer.Message message) { }
		public virtual void OnMultiplayerTakenClientUniqueID(string newClientUniqueID) { }
		public virtual void OnMultiplayerClientConnect(string clientUniqueID) { }
		public virtual void OnMultiplayerClientDisconnect(string clientUniqueID) { }
		public virtual void OnMultiplayerMessageReceived(Multiplayer.Message message) { }

		public virtual void OnAudioStartSetup(Audio instance) { }
		public virtual void OnAudioPlaySetup(Audio instance) { }
		public virtual void OnAudioPauseSetup(Audio instance) { }
		public virtual void OnAudioStopSetup(Audio instance) { }
		public virtual void OnAudioEndSetup(Audio instance) { }
		public virtual void OnAudioStart(Audio instance) { }
		public virtual void OnAudioPlay(Audio instance) { }
		public virtual void OnAudioPause(Audio instance) { }
		public virtual void OnAudioStop(Audio instance) { }
		public virtual void OnAudioEnd(Audio instance) { }

		public virtual void OnIdentityCreateSetup<T>(ComponentIdentity<T> instance) { }
		public virtual void OnIdentityCreate<T>(ComponentIdentity<T> instance) { }
		public virtual void OnIdentityTagAddSetup<T>(ComponentIdentity<T> instance, string tag) { }
		public virtual void OnIdentityTagAdd<T>(ComponentIdentity<T> instance, string tag) { }
		public virtual void OnIdentityTagRemoveSetup<T>(ComponentIdentity<T> instance, string tag) { }
		public virtual void OnIdentityTagRemove<T>(ComponentIdentity<T> instance, string tag) { }

		public virtual void OnTextCreateSetup(ComponentText instance) { }
		public virtual void OnTextCreate(ComponentText instance) { }
		public virtual void OnTextMoveSetup(ComponentText instance, Point delta) { }
		public virtual void OnTextMove(ComponentText instance, Point delta) { }
		public virtual void OnTextMoveStartSetup(ComponentText instance, Point delta) { }
		public virtual void OnTextMoveStart(ComponentText instance, Point delta) { }
		public virtual void OnTextMoveEndSetup(ComponentText instance) { }
		public virtual void OnTextMoveEnd(ComponentText instance) { }
		public virtual void OnTextRotateSetup(ComponentText instance, double delta) { }
		public virtual void OnTextRotate(ComponentText instance, double delta) { }
		public virtual void OnTextRotateStartSetup(ComponentText instance, double delta) { }
		public virtual void OnTextRotateStart(ComponentText instance, double delta) { }
		public virtual void OnTextRotateEndSetup(ComponentText instance) { }
		public virtual void OnTextRotateEnd(ComponentText instance) { }
		public virtual void OnTextRescaleSetup(ComponentText instance, Size delta) { }
		public virtual void OnTextRescale(ComponentText instance, Size delta) { }
		public virtual void OnTextRescaleStartSetup(ComponentText instance, Size delta) { }
		public virtual void OnTextRescaleStart(ComponentText instance, Size delta) { }
		public virtual void OnTextRescaleEndSetup(ComponentText instance) { }
		public virtual void OnTextRescaleEnd(ComponentText instance) { }
		public virtual void OnTextOriginateSetup(ComponentText instance, Point delta) { }
		public virtual void OnTextOriginate(ComponentText instance, Point delta) { }
		public virtual void OnTextOriginateStartSetup(ComponentText instance, Point delta) { }
		public virtual void OnTextOriginateStart(ComponentText instance, Point delta) { }
		public virtual void OnTextOriginateEndSetup(ComponentText instance) { }
		public virtual void OnTextOriginateEnd(ComponentText instance) { }
		public virtual void OnTextVisibilityChangeSetup(ComponentText instance) { }
		public virtual void OnTextVisibilityChange(ComponentText instance) { }
		public virtual void OnTextCharacterResizeSetup(ComponentText instance, int delta) { }
		public virtual void OnTextCharacterResize(ComponentText instance, int delta) { }
		public virtual void OnTextChangeSetup(ComponentText instance, string prevoious) { }
		public virtual void OnTextChange(ComponentText instance, string previous) { }
		public virtual void OnTextRecolorSetup(ComponentText instance, Color previous) { }
		public virtual void OnTextRecolor(ComponentText instance, Color previous) { }
		public virtual void OnTextBackgroundRecolorSetup(ComponentText instance, Color previous) { }
		public virtual void OnTextBackgroundRecolor(ComponentText instance, Color previous) { }
		public virtual void OnTextOutlineRecolorSetup(ComponentText instance, Color previous) { }
		public virtual void OnTextOutlineRecolor(ComponentText instance, Color previous) { }
		public virtual void OnTextOutlineResizeSetup(ComponentText instance, double delta) { }
		public virtual void OnTextOutlineResize(ComponentText instance, double delta) { }
		public virtual void OnTextSpacingResizeSetup(ComponentText instance, Size delta) { }
		public virtual void OnTextSpacingResize(ComponentText instance, Size delta) { }
		public virtual void OnTextChangeStartSetup(ComponentText instance, string prevoious) { }
		public virtual void OnTextChangeStart(ComponentText instance, string previous) { }
		public virtual void OnTextRecolorStartSetup(ComponentText instance, Color previous) { }
		public virtual void OnTextRecolorStart(ComponentText instance, Color previous) { }
		public virtual void OnTextBackgroundRecolorStartSetup(ComponentText instance, Color previous) { }
		public virtual void OnTextBackgroundRecolorStart(ComponentText instance, Color previous) { }
		public virtual void OnTextOutlineRecolorStartSetup(ComponentText instance, Color previous) { }
		public virtual void OnTextOutlineRecolorStart(ComponentText instance, Color previous) { }
		public virtual void OnTextOutlineResizeStartSetup(ComponentText instance, double delta) { }
		public virtual void OnTextOutlineResizeStart(ComponentText instance, double delta) { }
		public virtual void OnTextSpacingResizeStartSetup(ComponentText instance, Size delta) { }
		public virtual void OnTextSpacingResizeStart(ComponentText instance, Size delta) { }
		public virtual void OnTextChangeEndSetup(ComponentText instance) { }
		public virtual void OnTextChangeEnd(ComponentText instance) { }
		public virtual void OnTextRecolorEndSetup(ComponentText instance) { }
		public virtual void OnTextRecolorEnd(ComponentText instance) { }
		public virtual void OnTextBackgroundRecolorEndSetup(ComponentText instance) { }
		public virtual void OnTextBackgroundRecolorEnd(ComponentText instance) { }
		public virtual void OnTextOutlineRecolorEndSetup(ComponentText instance) { }
		public virtual void OnTextOutlineRecolorEnd(ComponentText instance) { }
		public virtual void OnTextOutlineResizeEndSetup(ComponentText instance) { }
		public virtual void OnTextOutlineResizeEnd(ComponentText instance) { }
		public virtual void OnTextSpacingResizeEndSetup(ComponentText instance) { }
		public virtual void OnTextSpacingResizeEnd(ComponentText instance) { }

		public virtual void On2DCreateSetup(Component2D instance) { }
		public virtual void On2DCreate(Component2D instance) { }
		public virtual void On2DMoveSetup(Component2D instance, Point delta) { }
		public virtual void On2DMove(Component2D instance, Point delta) { }
		public virtual void On2DMoveStartSetup(Component2D instance, Point delta) { }
		public virtual void On2DMoveStart(Component2D instance, Point delta) { }
		public virtual void On2DMoveEndSetup(Component2D instance) { }
		public virtual void On2DMoveEnd(Component2D instance) { }
		public virtual void On2DRotateSetup(Component2D instance, double delta) { }
		public virtual void On2DRotate(Component2D instance, double delta) { }
		public virtual void On2DRotateStartSetup(Component2D instance, double delta) { }
		public virtual void On2DRotateStart(Component2D instance, double delta) { }
		public virtual void On2DRotateEndSetup(Component2D instance) { }
		public virtual void On2DRotateEnd(Component2D instance) { }
		public virtual void On2DResizeSetup(Component2D instance, Size delta) { }
		public virtual void On2DResize(Component2D instance, Size delta) { }
		public virtual void On2DResizeStartSetup(Component2D instance, Size delta) { }
		public virtual void On2DResizeStart(Component2D instance, Size delta) { }
		public virtual void On2DResizeEndSetup(Component2D instance) { }
		public virtual void On2DResizeEnd(Component2D instance) { }
		public virtual void On2DOriginateSetup(Component2D instance, Point delta) { }
		public virtual void On2DOriginate(Component2D instance, Point delta) { }
		public virtual void On2DOriginateStartSetup(Component2D instance, Point delta) { }
		public virtual void On2DOriginateStart(Component2D instance, Point delta) { }
		public virtual void On2DOriginateEndSetup(Component2D instance) { }
		public virtual void On2DOriginateEnd(Component2D instance) { }
	}
}
