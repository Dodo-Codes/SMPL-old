using SFML.Graphics;
using SFML.Audio;
using SFML.System;
using SFML.Window;
using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using Newtonsoft.Json;
using System.Drawing.Design;
using System.Collections.Generic;
using TcpClient = NetCoreServer.TcpClient;
using System.Net.Sockets;
using System.Net;
using System.Linq;
using System.Text;
using System.IO;
using NetCoreServer;

namespace SMPL
{
	public class Game
	{
		internal static SMPL.Window window;
		internal static SMPL.Input.Keyboard keyboard;
		internal static SMPL.Input.Mouse mouse;
		internal static SMPL.Performance.Time time;

		static void Main() { }

		public static void Start(Window window,
			Performance.Time time = null,
			Input.Keyboard keyboard = null,
			Input.Mouse mouse = null)
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

				Performance.Time.tickDeltaTime = new();
				Performance.Time.tickCount++;
				time.OnEachTick();

				Window.window.Clear();
				Draw();
				Window.window.Display();
				Performance.Time.frameDeltaTime = new();
			}
		}
		internal static void Draw()
		{

		}
	}
}
