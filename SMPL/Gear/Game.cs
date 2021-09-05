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

		private static void InitializeStaticClasses(Size pixelSize)
		{
			File.Initialize();
			Time.Initialize();
			Window.Initialize(pixelSize);
			Hardware.Initialize();
			Performance.Initialize();
			Keyboard.Initialize();
			Mouse.Initialize();
			Assets.Initialize();
		}
		private static void CallEvents(Game game)
		{
			game.OnGameCreated();
			OnStart?.Invoke();
		}
		private static void RunGame()
		{
			while (Window.window.IsOpen)
			{
				Thread.Sleep(1);
				EarlyUpdate();
				DoEvents();
				Update();
				Window.Draw();
				LateUpdate();
			}
		}
		private static void DoEvents()
		{
			Application.DoEvents();
			Window.window.DispatchEvents();

			OnUpdate?.Invoke();
		}
		private static void EarlyUpdate()
		{
			Performance.EarlyUpdate();
		}
		private static void Update()
		{
			Audio.Update();
			Hitbox.Update();
			Rope.Update();
			SegmentedLine.Update();
			Keyboard.Update();
			Mouse.Update();
			Assets.Update();
		}
		private static void LateUpdate()
		{
			Components.Timer.Update();
			Performance.Update();
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
			InitializeStaticClasses(pixelSize);
			CallEvents(game);
			RunGame();
		}
		public virtual void OnGameCreated() { }
	}
}
