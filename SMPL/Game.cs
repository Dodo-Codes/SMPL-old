using SFML.Window;
using System;
using System.Windows.Forms;
using System.Threading;

namespace SMPL
{
	public class Game
	{
		internal static Thread resourceLoading;

		static void Main() { }

		public static void Start(Size pixelSize)
		{
			File.Initialize();
			Time.Initialize();
			Window.Initialize(pixelSize);
			OS.Initialize();
			Events.Initialize();

			resourceLoading = new Thread(new ThreadStart(File.LoadQueuedResources));
			resourceLoading.Name = "ResourcesLoading";
			resourceLoading.IsBackground = true;
			resourceLoading.Start();

			Time.Run();
		}
		public static void Stop()
		{
			WindowEvents.instance.OnClose();
			Window.window.Close();
		}
	}
}
