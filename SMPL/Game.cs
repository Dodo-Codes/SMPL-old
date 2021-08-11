using SFML.Window;
using System;
using System.Windows.Forms;
using System.Threading;

namespace SMPL
{
	public abstract class Game
	{
		///<summary>
		///text, <paramref name="param"/>, <see cref="char"/>, <typeparamref name="Type"/>
		///</summary>
		private static void SummaryExample() { }

		internal static Thread resourceLoading;
		internal static Game instance;

		private static event Events.ParamsZero OnStart;
		public static void CallOnStart(Action method, uint order = uint.MaxValue) => OnStart = Events.Add(OnStart, method, order);

		private static void Main() { }

		public static void Run(Game game, Size pixelSize)
		{
			instance = game;

			File.Initialize();
			Time.Initialize();
			Window.Initialize(pixelSize);
			Hardware.Initialize();
			Performance.Initialize();
			Keyboard.Initialize();
			Mouse.Initialize();

			resourceLoading = new Thread(new ThreadStart(File.LoadQueuedResources));
			resourceLoading.Name = "ResourcesLoading";
			resourceLoading.IsBackground = true;
			resourceLoading.Start();

			instance.OnGameCreated();
			OnStart?.Invoke();

			Time.Run();
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
