using SFML.System;
using System.Diagnostics;

namespace SMPL
{
	public static class Performance
	{
		private static PerformanceCounter _ramAvailable, _ramUsedPercent, _cpuPercent;
		public static double AvailableRAM { get; private set; }
		public static double PercentUsedRAM { get; private set; }
		public static double PercentCPU { get; private set; }

		internal static Clock frameDeltaTime;
		internal static uint frameCount;
		public static uint FrameCount { get { return frameCount; } }
		private static uint frameRateLimit;
		public static uint FrameRateLimit
		{
			get { return frameRateLimit; }
			set
			{
				var n = (uint)Number.Limit(value, new Bounds(1, 60));
				frameRateLimit = n;
				Window.window.SetFramerateLimit(n);
			}
		}
		public static double FrameRate { get { return 1 / frameDeltaTime.ElapsedTime.AsSeconds(); } }
		public static double FrameRateAverage { get { return frameCount / Time.time.ElapsedTime.AsSeconds(); } }
		public static double DeltaTime { get { return frameDeltaTime.ElapsedTime.AsSeconds(); } }
		private static bool vsync;
		public static bool VerticalSyncEnabled
		{
			get { return vsync; }
			set
			{
				vsync = value;
				Window.window.SetVerticalSyncEnabled(value);
			}
		}

		internal static void Initialize()
		{
			frameDeltaTime = new();

#pragma warning disable CA1416
			_ramAvailable = new PerformanceCounter("Memory", "Available MBytes");
			_ramUsedPercent = new PerformanceCounter("Memory", "% Committed Bytes In Use");
			_cpuPercent = new PerformanceCounter("Processor", "% Processor Time", "_Total");
#pragma warning restore CA1416
		}
		internal static void UpdateCounters()
		{
#pragma warning disable CA1416
			AvailableRAM = _ramAvailable.NextValue() / 1000;
			PercentUsedRAM = _ramUsedPercent.NextValue();
			PercentCPU = _cpuPercent.NextValue();
#pragma warning restore CA1416
		}
	}
}
