using System.Collections.Generic;
using System.Diagnostics;

namespace SMPL
{
	public static class Debug
	{
		/// <summary>
		/// Wether the game is started from Visual Studio or its '.exe' file.
		/// </summary>
		public static bool IsActive { get { return Debugger.IsAttached; } }
		internal static string debugString = "(This error message will not appear after the game is built)";

		public static double GetCurrentLineNumber(uint depth = 0)
		{
			var info = new StackFrame((int)depth + 1, true);
			return info.GetFileLineNumber();
		}
		public static string GetCurrentFileName(uint depth = 0)
		{
			var pathRaw = GetCurrentFilePath(depth + 1);
			if (pathRaw == null) return null;
			var path = pathRaw.Split('\\');
			var name = path[path.Length - 1].Split('.');
			return name[0];
		}
		public static string GetCurrentFilePath(uint depth = 0)
		{
			var info = new StackFrame((int)depth + 1, true);
			var a = info.GetFileName();
			return a;
		}
		public static string GetCurrentMethodName(uint depth = 0)
		{
			var info = new StackFrame((int)depth + 1, true);
			var method = info.GetMethod();
			var child = method == null ? null : method.ToString().Replace('+', '.');
			var firstSpaceIndex = child.IndexOf(' ');
			var parent = method.DeclaringType.ToString().Replace('+', '.') + ".";
			var result = child.Insert(firstSpaceIndex + 1, parent);

			return method == default ? default : result;
		}

		public static void LogError(int depth, string description, bool isDisplayingInRelease = false)
		{
			if (IsActive == false && isDisplayingInRelease == false) return;

			var d = (uint)depth;
			var debugStr = isDisplayingInRelease ? "" : debugString;
			var place = depth >= 0 && IsActive ?
				$"- At file: {GetCurrentFileName(d + 1)}\n" +
				$"- At method: {GetCurrentMethodName(d + 1)}\n" +
				$"- At line: {GetCurrentLineNumber(d + 1)} | {GetCurrentMethodName(d)}\n" : "";
			var result =
				$"ERROR {debugStr}\n" +
				$"{place}" +
				$"{description}\n";
			Console.Log(result);
		}
	}
}
