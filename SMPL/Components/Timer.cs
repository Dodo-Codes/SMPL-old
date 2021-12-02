using System;
using System.Collections.Generic;
using SMPL.Data;
using SMPL.Gear;

namespace SMPL.Components
{
	public class Timer : Thing
   {
      public enum Action
      {
         CreateAndStart, Update, End
      }

      private static readonly List<Timer> timers = new();
      private double progress, countdown, endCount, duration;
      private bool isPaused;

		internal static void Update()
      {
         for (int i = 0; i < timers.Count; i++)
         {
            var dt = Performance.DeltaTime;
            if (timers[i] == null) continue;
            if (timers[i].Countdown < 0) timers[i].Countdown = 0;
            if (timers[i].IsPaused || timers[i].Countdown == 0) continue;
            timers[i].Countdown -= dt;
            Events.Notify(Game.Event.TimerUpdate, new() { Timer = timers[i] });
            if (Gate.EnterOnceWhile(timers[i] + "end-as;li3'f2", timers[i].Countdown <= 0) ||
               dt > timers[i].Duration)
            {
               timers[i].EndCount++;
               timers[i].Countdown = 0;
               Events.Notify(Game.Event.TimerEnd, new() { Timer = timers[i] });
            }
         }
      }

      public double EndCount
      {
         get { return ErrorIfDestroyed() ? double.NaN : endCount; }
         set { if (ErrorIfDestroyed() == false) endCount = value; }
      }
      public double Duration
      {
         get { return ErrorIfDestroyed() ? double.NaN : duration; }
         private set { if (ErrorIfDestroyed() == false) duration = value; }
      }
      public double Progress
      {
         get { return ErrorIfDestroyed() ? double.NaN : progress; }
         set
         {
            if (ErrorIfDestroyed()) return;
            progress = value;
            countdown = Duration - value;
         }
      }
      public double ProgressPercent
      {
         get { return ErrorIfDestroyed() ? double.NaN : Number.ToPercent(progress, new Number.Range(0, Duration)); }
         set
         {
            if (ErrorIfDestroyed()) return;
            progress = Number.FromPercent(Number.Limit(value, new Number.Range(0, 100)), new Number.Range(0, Duration));
         }
      }
      public double Countdown
      {
         get { return ErrorIfDestroyed() ? double.NaN : countdown; }
         set
         {
            if (ErrorIfDestroyed()) return;
            countdown = value;
            progress = Duration - value;
         }
      }
      public bool IsPaused
      {
         get { return ErrorIfDestroyed() == false && isPaused; }
         set { if (ErrorIfDestroyed() == false)  isPaused = value; }
      }

      public Timer(double durationInSeconds)
      {
         timers.Add(this);
         Duration = durationInSeconds;
         Countdown = Duration;
         Events.Notify(Game.Event.TimerCreateAndStart, new() { Timer = this });
      }
		public override void Destroy()
		{
         if (ErrorIfDestroyed()) return;
         timers.Remove(this);
         base.Destroy();
		}
   }
}
