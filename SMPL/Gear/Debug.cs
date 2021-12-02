using SMPL.Components;
using SMPL.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SMPL.Gear
{
	public static class Debug
	{
		/// <summary>
		/// Wether the game is started from Visual Studio or its '.exe' file.
		/// </summary>
		public static bool IsActive { get { return Debugger.IsAttached; } }
		internal static string debugString = "This is a debug message, it will not appear after the game is built";
		private static string output = "";

		public static double CurrentLineNumber(uint depth = 0)
		{
			var info = new StackFrame((int)depth + 1, true);
			return info.GetFileLineNumber();
		}
		public static string CurrentMethodName(uint depth = 0)
		{
			var info = new StackFrame((int)depth + 1, true);
			var method = info.GetMethod();
			var child = method?.ToString().Replace('+', '.');
			var firstSpaceIndex = child.IndexOf(' ');
			if (method.DeclaringType == null) return null;
			var parent = method.DeclaringType.ToString().Replace('+', '.') + ".";
			var result = child.Insert(firstSpaceIndex + 1, parent);

			return method == default ? default : result;
		}
		public static string CurrentFileName(uint depth = 0)
		{
			var pathRaw = CurrentFilePath(depth + 1);
			if (pathRaw == null) return null;
			var path = pathRaw.Split('\\');
			var name = path[^1].Split('.');
			return name[0];
		}
		public static string CurrentFilePath(uint depth = 0)
		{
			var info = new StackFrame((int)depth + 1, true);
			return info.GetFileName();
		}
		public static string CurrentFileDirectory(uint depth = 0)
		{
			var fileName = new StackFrame((int)depth + 1, true).GetFileName();
			var path = fileName.Split('\\');
			var dir = "";
			for (int i = 0; i < path.Length - 1; i++)
			{
				dir += path[i];
				if (i == path.Length - 2) continue;
				dir += "\\";
			}
			return dir;
		}

		public static void LogError(int depth, string description, bool isDisplayingInRelease = false)
		{
			if (IsActive == false && isDisplayingInRelease == false) return;

			var d = (uint)depth;
			var debugStr = isDisplayingInRelease ? "!" : debugString;
			var place = depth >= 0 && IsActive ?
				$"Location\t[File: {CurrentFileName(d + 1)}.cs | Method: {CurrentMethodName(d + 1)}]\n" +
				$"Action\t\t[Line: {CurrentLineNumber(d + 1)} | Method: {CurrentMethodName(d)}]\n" : "";
			var result =
				$"Error\t\t[{debugStr}]\n" +
				$"{place}" +
				$"{Data.Text.Repeat("- ", 10)}\n" +
				$"{description}\n";
			Console.Log(result);
		}
		public static void Display()
		{
			if (Window.DrawNotAllowed()) return;
			if (Assets.fonts.Count == 0)
			{
				LogError(1, $"No loaded fonts found. Make sure there is at least one loaded font before dislpaying " +
					$"the {nameof(Debug)} info.");
				return;
			}
			var pressedKeys = "";
			var pressedButtons = "";
			var clientStr = $"{Multiplayer.ClientIsConnected}" +
				(Multiplayer.ClientIsConnected ? $" ({Multiplayer.ClientUniqueID})" : "");
			for (int i = 0; i < Keyboard.keysHeld.Count; i++)
				pressedKeys += $"{Keyboard.keysHeld[i].ToString().Replace("_", "")}" + (i < Keyboard.keysHeld.Count - 1 ? ", " : "");
			for (int i = 0; i < Mouse.buttonsHeld.Count; i++)
				pressedButtons += $"{Mouse.buttonsHeld[i]}" + (i < Mouse.buttonsHeld.Count - 1 ? ", " : "");

			if (output == "" || Performance.FrameCount % 5 == 0)
				output =
					$"FPS Average: {Performance.FPSAverage:F1} / Limit: {Performance.FPSLimit:F1} / Raw: {Performance.FPS:F1}\n" +
					$"CPU Usage: {Performance.PercentCPU:F2}%\n" +
					$"RAM Usage: {Performance.UsedRAM:F2} GB " +
					$"({Number.ToPercent(Performance.UsedRAM, new Number.Range(0, Hardware.RAM)):F1}%) / " +
					$"Available: {Performance.AvailableRAM:F2} GB " +
					$"({Number.ToPercent(Performance.AvailableRAM, new Number.Range(0, Hardware.RAM)):F1}%)\n" +
					$"GPU draw calls per frame: {Performance.prevDrawCallsPerFr} (including this text)\n" +
					$"GPU Usage: {Performance.PercentGPU:F2}%\n" +
					$"Quads drawn per frame: {Performance.QuadDrawsPerFrame} (Vertices: {Performance.VertexDrawsPerFrame})\n" +
					$"Frame count (ticks): {Performance.FrameCount}\n" +
					$"VSync Enabled: {Performance.VSyncEnabled}\n" +
					$"Performance boost: {Performance.Boost}\n" +
					$"\n" +
					$"OS: {Hardware.OperatingSystemName}\n" +
					$"GPU: {Hardware.VideoCardName}\n" +
					$"CPU: {Hardware.ProcessorName}\n" +
					$"RAM: {Hardware.RAM} GB\n" +
					$"Sound Card: {Hardware.SoundDeviceName}\n" +
					$"\n" +
					$"Window title: {Window.Title} / state: {Window.CurrentState} / type: {Window.CurrentType}\n" +
					$"Window resizable: {Window.IsResizable} / prevents PC sleep: {Window.PreventsSleep}\n" +
					$"Window position: {Window.Position} / size: {Window.Size} / pixel size: {Window.PixelSize}\n" +
					$"\n" +
					$"Clock: {Time.ToText(Time.Clock, new() { Milliseconds = new() { AreSkipped = true } })}\n" +
					$"Timezone: {Time.Zone}\n" +
					$"Time since game start (game clock): " +
					$"{Time.ToText(Time.GameClock, new() { Milliseconds = new() { AreSkipped = true } })}\n" +
					$"Time since last tick/frame (delta time): {Performance.DeltaTime:F5}s\n" +
					$"\n" +
					$"Loading: {Assets.LoadPercent}%\n" +
					$"Loaded fonts: {Assets.fonts.Count} / textures: {Assets.textures.Count} / audios: " +
					$"{Assets.sounds.Count + Assets.music.Count} / values: {Assets.values.Count}\n" +
					$"Main directory: {FileSystem.MainDirectory}\n" +
					$"Running in Visual Studio: {IsActive}\n" +
					$"\n" +
					$"Keyboard keys pressed: {pressedKeys}\n" +
					$"Mouse buttons pressed: {pressedButtons}\n" +
					$"Mouse cursor type: {Mouse.Cursor.CurrentType} / hidden: {Mouse.Cursor.IsHidden} / " +
					$"position: {Mouse.Cursor.PositionScreen}\n" +
					$"\n" +
					$"Hosting server: {Multiplayer.ServerIsRunning}\n" +
					$"Joined server: {clientStr}\n" +
					$"\n" +
					$"Things count: {Thing.uids.Count} / tags count: {Thing.uids.Count} / " +
					$"event subscriptions: {Events.notifications.Count}\n" +
					$"\n" +
					$"Gates count: {Gate.gates.Count}";
			Performance.prevDrawCallsPerFr = 0;
			foreach (var kvp in Assets.fonts)
			{
				var sz = Camera.WorldCamera.Size;
				Text.Display(Camera.WorldCamera, output, kvp.Key, new Point(-sz.W + sz.W / 100, -sz.H) / 2);
				break;
			}
		}
	}
}
