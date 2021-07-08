using SFML.Window;
using System;
using System.Windows.Forms;
using System.Threading;

namespace SMPL
{
	public class Game
	{
		public struct Events
		{
			public WindowEvents windowEvents;
			public KeyboardEvents keyboardEvents;
			public MouseEvents mouseEvents;
			public FileEvents fileEvents;
			public WorldCameraEvents worldCameraEvents;
		}

		internal static Thread resourceLoading;

		static void Main() { }

		public static void Start(Events gameEvents, Size pixelSize)
		{
			File.Initialize();
			Time.Initialize();
			Window.Initialize();
			OS.Initialize();

			if (gameEvents.windowEvents != null) gameEvents.windowEvents.Subscribe();
			if (gameEvents.worldCameraEvents != null) gameEvents.worldCameraEvents.Subscribe(pixelSize);
			if (gameEvents.keyboardEvents != null) gameEvents.keyboardEvents.Subscribe();
			if (gameEvents.mouseEvents != null) gameEvents.mouseEvents.Subscribe();
			if (gameEvents.fileEvents != null) gameEvents.fileEvents.Subscribe();

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
