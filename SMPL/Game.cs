using SFML.Window;
using System;
using System.Windows.Forms;
using System.Threading;

namespace SMPL
{
	public class Game
	{
		internal static Window window;
		internal static Time time;

		public static string Directory { get { return AppDomain.CurrentDomain.BaseDirectory; } }

		static void Main() { }

		public static void Start(Window window,
			Time time = null,
			Keyboard keyboard = null,
			Mouse mouse = null)
		{
			if (window == null) return;
			window.Initialize();
			if (time != null) time.Initialize();
			if (keyboard != null) keyboard.Initialize();
			if (mouse != null) mouse.Initialize();
			OS.Initialize();

			Run();
		}
		public static void Stop()
		{
			window.OnClose();
			Window.window.Close();
		}

		internal static void Run()
		{
			while (Window.window.IsOpen)
			{
				Thread.Sleep(1); // calming down the cpu
				Application.DoEvents();
				Window.window.DispatchEvents();

				Time.tickCount++;
				time.OnEachTick();
				Time.tickDeltaTime.Restart();

				if (Window.ForceDraw == false) continue;
				Time.frameCount++;
				var bg = Window.BackgroundColor;
				Window.window.Clear(new SFML.Graphics.Color((byte)bg.Red, (byte)bg.Green, (byte)bg.Blue));
				Window.Draw();
				window.OnDraw();
				Window.window.Display();
				Time.frameDeltaTime.Restart();
				Window.ForceDraw = false;
			}
		}

		internal static string CreateDirectoryForFile(string filePath)
		{
			filePath = filePath.Replace('/', '\\');
			var path = filePath.Split('\\');
			var full = $"{Directory}{filePath}";
			var curPath = Directory;
			for (int i = 0; i < path.Length - 1; i++)
			{
				var p = $"{curPath}\\{path[i]}";
				if (System.IO.Directory.Exists(p) == false) System.IO.Directory.CreateDirectory(p);

				curPath = p;
			}
			return full;
		}
	}
}
