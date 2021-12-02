using SFML.System;
using SMPL.Components;
using SMPL.Data;
using System.Collections.Generic;
using System.Diagnostics;

namespace SMPL.Gear
{
	public static class Performance
	{
		private static PerformanceCounter ramAvailable, cpuPercent;
		private static List<PerformanceCounter> gpuPercents;
		internal static uint prevDrawCallsPerFr = 0;

		public static double AvailableRAM { get; private set; }
		public static double UsedRAM => Hardware.RAM - AvailableRAM;
		public static double PercentCPU { get; private set; }
		public static double PercentGPU { get; private set; }
		public static uint DrawCallsPerFrame { get; internal set; }
		public static uint VertexDrawsPerFrame { get; internal set; }
		public static uint QuadDrawsPerFrame { get; internal set; }
		public static bool Boost { get; set; }

		internal static Clock frameDeltaTime;
		public static uint FrameCount { get; private set; }
		private static uint frameRateLimit = 9999;
		public static uint FPSLimit
		{
			get { return frameRateLimit; }
			set
			{
				var n = (uint)Number.Limit(value, new Number.Range(1, 60));
				frameRateLimit = n;
				Window.window.SetFramerateLimit(n);
			}
		}
		public static double FPS => 1 / frameDeltaTime.ElapsedTime.AsSeconds();
		public static double FPSAverage => FrameCount / Time.time.ElapsedTime.AsSeconds();
		public static double DeltaTime => frameDeltaTime.ElapsedTime.AsSeconds();
		private static bool vsync;
		public static bool VSyncEnabled
		{
			get { return vsync; }
			set
			{
				vsync = value;
				Window.window.SetVerticalSyncEnabled(value);
			}
		}

		internal static void ResetDrawCounters()
		{
			DrawCallsPerFrame = 0;
			VertexDrawsPerFrame = 0;
			QuadDrawsPerFrame = 0;
		}
		internal static void Initialize()
		{
			frameDeltaTime = new();

#pragma warning disable CA1416
			ramAvailable = new PerformanceCounter("Memory", "Available MBytes");
			cpuPercent = new PerformanceCounter("Processor", "% Processor Time", "_Total");

			gpuPercents = new List<PerformanceCounter>();
			var category = new PerformanceCounterCategory("GPU Engine");
			var names = category.GetInstanceNames();
			foreach (var name in names)
				if (name.EndsWith("engtype_3D"))
					foreach (var counter in category.GetCounters(name))
						if (counter.CounterName == "Utilization Percentage")
							gpuPercents.Add(counter);
#pragma warning restore CA1416
		}
		internal static void EarlyUpdate()
		{
			FrameCount++;
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
				AvailableRAM = ramAvailable.NextValue() / 1024;
				PercentCPU = cpuPercent.NextValue();

				PercentGPU = 0;
				try
				{
					for (int i = 0; i < gpuPercents.Count; i++)
						PercentGPU += gpuPercents[i].NextValue();
				}
				catch (System.Exception) { return; }
#pragma warning restore CA1416
			}
		}
	}
}
