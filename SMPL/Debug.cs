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

		public static void LogError(int depth, string description)
		{
			if (IsActive == false) return;

			var sep = Text.Repeat("~", 50);
			var d = (uint)depth;
			var place = depth >= 0 ?
				$"- At file: {GetCurrentFileName(d + 1)}\n" +
				$"- At method: {GetCurrentMethodName(d + 1)}\n" +
				$"- At line: {GetCurrentLineNumber(d + 1)} | {GetCurrentMethodName(d)}\n\n" : "";
			var result =
				$"{sep}\n" +
				$"{place}" +
				$"{description}\n\n" +
				$"(This message will not appear after the game is built)\n" +
				$"{sep}";
			Console.Log(result);
		}
		//public static bool DoesNotExistError<T, T1>(Dictionary<T, T1> dict, T key, string type)
		//{
		//	var notFound = dict.ContainsKey(key) == false;
		//	if (notFound) LogError(2, $"{type} '{key}' was not found.");
		//	return notFound;
		//}
	}
}
