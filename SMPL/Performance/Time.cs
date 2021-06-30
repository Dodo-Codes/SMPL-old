using SFML.System;

namespace SMPL.Performance
{
	public abstract class Time
	{
		internal static Clock time, tickDeltaTime, frameDeltaTime;
		internal static uint tickCount;
		public static double TickDeltaTime { get { return tickDeltaTime.ElapsedTime.AsSeconds(); } }
		public static double FrameDeltaTime { get { return frameDeltaTime.ElapsedTime.AsSeconds(); } }
		public static double TickCount { get { return tickCount; } }

		internal void Initialize()
		{
			Game.time = this;

			time = new();
			OnStart();
		}
		public virtual void OnStart() { }
		public virtual void OnEachTick() { }
	}
}
