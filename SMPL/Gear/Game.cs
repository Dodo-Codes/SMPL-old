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
		private static event Events.ParamsZero OnStart, OnUpdate;

		private static void SummaryExample() { }
		///<summary>
		///text, <paramref name="param"/>, <see cref="char"/>, <typeparamref name="Type"/>
		///</summary>
		private static void Main() { }

		private static void RunGame()
		{
			while (Window.window.IsOpen)
			{
				Thread.Sleep(1);

				Performance.EarlyUpdate();

				Application.DoEvents();
				Window.window.DispatchEvents();
				OnUpdate?.Invoke();

				Audio.Update();
				Hitbox.Update();
				Ropes.Update();
				Keyboard.Update();
				Mouse.Update();
				Assets.Update();

				Window.Draw();

				Components.Timer.Update();
				Performance.Update();
			}
		}

		// ==================

		public static class CallWhen
		{
			public static void Start(Action method, uint order = uint.MaxValue) =>
				OnStart = Events.Add(OnStart, method, order);
			public static void GameIsRunning(Action method, uint order = uint.MaxValue) =>
				OnUpdate = Events.Add(OnUpdate, method, order);
		}

		public static void Start(Game game, Size pixelSize)
		{
			File.Initialize();
			Time.Initialize();
			Hardware.Initialize();
			Performance.Initialize();
			Window.Initialize(pixelSize);
			Mouse.Initialize();
			Assets.Initialize();
			Keyboard.Initialize();

			game.OnGameCreated();
			OnStart?.Invoke();

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
