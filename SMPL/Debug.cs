using System.Diagnostics;

namespace SMPL
{
	public static class Debug
	{
		private readonly static string engineKey = "_tas8lf1d5ku346";
		public static bool currentMethodIsCalledByUser
		{
			get { return System.IO.File.Exists($"{CurrentFileDirectory(2)}\\{engineKey}.txt") == false; }
		}

		/// <summary>
		/// Wether the game is started from Visual Studio or its '.exe' file.
		/// </summary>
		public static bool IsActive { get { return Debugger.IsAttached; } }
		internal static string debugString = "(This error message will not appear after the game is built)";

		public static double CurrentLineNumber(uint depth = 0)
		{
			var info = new StackFrame((int)depth + 1, true);
			return info.GetFileLineNumber();
		}
		public static string CurrentFileName(uint depth = 0)
		{
			var pathRaw = CurrentFilePath(depth + 1);
			if (pathRaw == null) return null;
			var path = pathRaw.Split('\\');
			var name = path[path.Length - 1].Split('.');
			return name[0];
		}
		public static string CurrentFilePath(uint depth = 0)
		{
			var info = new StackFrame((int)depth + 1, true);
			var a = info.GetFileName();
			return a;
		}
		public static string CurrentMethodName(uint depth = 0)
		{
			var info = new StackFrame((int)depth + 1, true);
			var method = info.GetMethod();
			var child = method == null ? null : method.ToString().Replace('+', '.');
			var firstSpaceIndex = child.IndexOf(' ');
			var parent = method.DeclaringType.ToString().Replace('+', '.') + ".";
			var result = child.Insert(firstSpaceIndex + 1, parent);

			return method == default ? default : result;
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
			var debugStr = isDisplayingInRelease ? "" : debugString;
			var place = depth >= 0 && IsActive ?
				$"- At file: {CurrentFileName(d + 1)}\n" +
				$"- At method: {CurrentMethodName(d + 1)}\n" +
				$"- At line: {CurrentLineNumber(d + 1)} | {CurrentMethodName(d)}\n" : "";
			var result =
				$"ERROR {debugStr}\n" +
				$"{place}" +
				$"{description}\n";
			Console.Log(result);
		}
	}
}
