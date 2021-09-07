﻿using System.Diagnostics;

namespace SMPL.Gear
{
	public static class Debug
	{
		/// <summary>
		/// Wether the game is started from Visual Studio or its '.exe' file.
		/// </summary>
		public static bool IsActive { get { return Debugger.IsAttached; } }
		internal static string debugString = "This is a debug message, it will not appear after the game is built";

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
				$"Location\t[File: {CurrentFileName(d + 1)}.cs | Method:{CurrentMethodName(d + 1)}]\n" +
				$"Action\t\t[Line: {CurrentLineNumber(d + 1)} | Method: {CurrentMethodName(d)}]\n" : "";
			var result =
				$"Error\t\t[{debugStr}]\n" +
				$"{place}" +
				$"{Data.Text.Repeat("- ", 10)}\n" +
				$"{description}\n";
			Console.Log(result);
		}
	}
}
