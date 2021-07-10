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
			foreach (var i in instances) i.OnEachFrame();

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

		internal static void Initialize()
		{
		}


		public void Subscribe(Events instance)
		{
         if (instances.Contains(instance))
         {
				Debug.LogError(1, $"The instance of '{instance.GetType()}' is already subscribed.");
				return;
         }
			instances.Add(instance);

			Window.window.KeyPressed += new EventHandler<SFML.Window.KeyEventArgs>(OnKeyPress);
			Window.window.KeyReleased += new EventHandler<SFML.Window.KeyEventArgs>(OnKeyRelease);
			Window.window.SetKeyRepeatEnabled(false);
			Window.form.KeyPress += new KeyPressEventHandler(OnTextInput);
			Window.form.InputLanguageChanged += new InputLanguageChangedEventHandler(OnLanguageChange);
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

		public virtual void OnEachFrame() { }
		public virtual void OnTimerEnd(Timer timerInstance) { }

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

		public virtual void OnKeyPress(Keyboard.Key key) { }
		public virtual void OnKeyRelease(Keyboard.Key key) { }
		public virtual void OnTextInput(string textSymbol, bool isBackspace, bool isEnter, bool isTab) { }
		public virtual void OnLanguageChange(string englishName, string nativeName, string languageCode) { }
	}
}
