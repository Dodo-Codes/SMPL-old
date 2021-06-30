using System;

namespace SMPL
{
	public static class OS
	{
		public enum Platform
		{
			Unknown, Windows, Linux, iOS, MacOS
		}

		private static Platform platform;
		public static Platform CurrentPlatform { get { return platform; } }

		internal static void Initialize()
		{
			platform = Platform.Unknown;
			if (OperatingSystem.IsWindows()) platform = Platform.Windows;
			if (OperatingSystem.IsLinux()) platform = Platform.Linux;
			if (OperatingSystem.IsIOS()) platform = Platform.iOS;
			if (OperatingSystem.IsMacOS()) platform = Platform.MacOS;
		}
	}
}
