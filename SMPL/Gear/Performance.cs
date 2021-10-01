using SFML.System;
using SMPL.Data;
using System.Diagnostics;

namespace SMPL.Gear
{
	public static class Performance
	{
		private static PerformanceCounter _ramAvailable, _ramUsedPercent, _cpuPercent;
		public static double AvailableRAM { get; private set; }
		public static double PercentUsedRAM { get; private set; }
		public static double PercentCPU { get; private set; }
		public static uint DrawCallsPerFrame { get; internal set; }

		internal static Clock frameDeltaTime;
		internal static uint frameCount;
		public static uint FrameCount { get { return frameCount; } }
		private static uint frameRateLimit;
		public static uint LimitFPS
		{
			get { return frameRateLimit; }
			set
			{
				var n = (uint)Number.Limit(value, new Number.Range(1, 60));
				frameRateLimit = n;
				Window.window.SetFramerateLimit(n);
			}
		}
		public static double FPS { get { return 1 / frameDeltaTime.ElapsedTime.AsSeconds(); } }
		public static double AverageFPS { get { return frameCount / Time.time.ElapsedTime.AsSeconds(); } }
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
		internal static void EarlyUpdate()
		{
			frameCount++;
		}
		internal static void Update()
		{
			frameDeltaTime.Restart();
			UpdateCounters();
		}
		private static void UpdateCounters()
		{
			if (Gate.EnterOnceWhile("a'diuq1`45gds-0", (int)Time.time.ElapsedTime.AsSeconds() % 2 == 0))
			{
#pragma warning disable CA1416
				AvailableRAM = _ramAvailable.NextValue() / 1000;
				PercentUsedRAM = _ramUsedPercent.NextValue();
				PercentCPU = _cpuPercent.NextValue();
#pragma warning restore CA1416
			}
		}
	}
}
