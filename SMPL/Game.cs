using SFML.Window;
using System;
using System.Windows.Forms;
using System.Threading;

namespace SMPL
{
	public abstract class Game
	{
		internal static Thread resourceLoading;
		internal static Game instance;

		static void Main() { }

		public static void Run(Game game, Size pixelSize)
		{
			instance = game;

			File.Initialize();
			Time.Initialize();
			Window.Initialize(pixelSize);
			OS.Initialize();

			resourceLoading = new Thread(new ThreadStart(File.LoadQueuedResources));
			resourceLoading.Name = "ResourcesLoading";
			resourceLoading.IsBackground = true;
			resourceLoading.Start();

			instance.OnStart();

			foreach (var e in Events.instances) e.OnStartEarly();
			foreach (var e in Events.instances) e.OnStart();
			foreach (var e in Events.instances) e.OnStartLate();

			Time.Run();
		}

		public virtual void OnStart() { }
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
