using SFML.Window;
using System;
using System.Windows.Forms;
using System.Threading;
using SMPL.Data;
using SMPL.Components;
using SMPL.Prefabs;

namespace SMPL.Gear
{
	public abstract class Game
	{
		///<summary>
		///text, <paramref name="param"/>, <see cref="char"/>, <typeparamref name="Type"/>
		///</summary>
		private static void Main() { }

		public static class Event
		{
			public static class Subscribe
			{
				public static void Update(string thingUID, uint order = uint.MaxValue) =>
					Events.NotificationEnable(Events.Type.GameUpdate, thingUID, order);
			}
			public static class Unsubscribe
			{
				public static void Update(string thingUID) =>
					Events.NotificationDisable(Events.Type.GameUpdate, thingUID);
			}
		}

		private static void RunGame()
		{
			while (Window.window.IsOpen)
			{
				Thread.Sleep(1);

				Performance.EarlyUpdate();

				Audio.Update();
				Hitbox.Update();
				Ropes.Update();
				Assets.Update();

				Keyboard.Update();
				Mouse.Update();

				Application.DoEvents();
				Window.window.DispatchEvents();
				Events.Notify(Events.Type.GameUpdate);
				Window.Draw();

				Components.Timer.Update();
				Performance.Update();
			}
		}

		public static void Start(Game game, Size pixelSize)
		{
			FileSystem.Initialize();
			Time.Initialize();
			Hardware.Initialize();
			Performance.Initialize();
			Window.Initialize(pixelSize);
			Mouse.Initialize();
			Assets.Initialize();
			Keyboard.Initialize();

			game.OnGameCreated();

			RunGame();
		}
		public virtual void OnGameCreated() { }
	}
	internal static class Statics
	{
		internal static bool TryCast<T>(this object obj, out T result)
		{
			if (obj is T t)
			{
				result = t;
				return true;
			}

			result = default;
			return false;
		}
	}
}
