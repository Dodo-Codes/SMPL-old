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
			public TimeEvents timeEvents;
			public KeyboardEvents keyboardEvents;
			public MouseEvents mouseEvents;
			public FileEvents fileEvents;
			public WorldCameraEvents worldCameraEvents;
		}

		internal static Thread resourceLoading;
		internal static Thread code;

		static void Main() { }

		public static void Start(Events gameEvents)
		{
			Time.Initialize();
			Window.Initialize();
			OS.Initialize();
			File.Initialize();

			if (gameEvents.windowEvents != null) gameEvents.windowEvents.Subscribe();
			if (gameEvents.worldCameraEvents != null) gameEvents.worldCameraEvents.Subscribe();
			if (gameEvents.timeEvents != null) gameEvents.timeEvents.Subscribe();
			if (gameEvents.keyboardEvents != null) gameEvents.keyboardEvents.Subscribe();
			if (gameEvents.mouseEvents != null) gameEvents.mouseEvents.Subscribe();
			if (gameEvents.fileEvents != null) gameEvents.fileEvents.Subscribe();

			StartThreads();
		}
		public static void Stop()
		{
			WindowEvents.instance.OnClose();
			Window.window.Close();
		}

		internal static void StartThreads()
		{
			resourceLoading = new Thread(new ThreadStart(File.LoadQueuedResources));
			code = new Thread(new ThreadStart(Time.Run));
			resourceLoading.Name = "ResourcesLoading";
			code.Name = "Code";
			resourceLoading.IsBackground = true;
			code.IsBackground = true;

			resourceLoading.Start();
			code.Start();
			Window.Draw();
		}
	}
}
