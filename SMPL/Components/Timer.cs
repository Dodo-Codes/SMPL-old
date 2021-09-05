using System;
using System.Collections.Generic;
using SMPL.Data;
using SMPL.Gear;

namespace SMPL.Components
{
	public class Timer : Component
   {
      private static readonly List<Timer> timers = new();
      private static event Events.ParamsOne<Timer> OnEnd, OnPause;
      private static event Events.ParamsTwo<Timer, double> OnCreateAndStart;
      private static event Events.ParamsFour<Timer, double, double, double> OnUpdate;

      private double progress, countdown, endCount, duration;
      private bool isPaused;

      //=============

		internal static void Update()
      {
         for (int i = 0; i < timers.Count; i++)
         {
            var dt = Performance.DeltaTime;
            if (timers[i] == null) continue;
            if (timers[i].Countdown < 0) timers[i].Countdown = 0;
            if (timers[i].IsPaused || timers[i].Countdown == 0) continue;
            var prevCd = timers[i].Countdown;
            var prevPr = timers[i].Progress;
            var prevPrPer = timers[i].ProgressPercent;
            timers[i].Countdown -= dt;
            OnUpdate?.Invoke(timers[i], prevCd, prevPr, prevPrPer);
            if (Gate.EnterOnceWhile(timers[i] + "end-as;li3'f2", timers[i].Countdown <= 0) ||
               dt > timers[i].Duration)
            {
               timers[i].EndCount++;
               timers[i].Countdown = 0;
               OnEnd?.Invoke(timers[i]);
            }
         }
      }

      //=============

      public static class CallWhen
      {
         public static void CreateAndStart(Action<Timer, double> method, uint order = uint.MaxValue) =>
         OnCreateAndStart = Events.Add(OnCreateAndStart, method, order);
         public static void End(Action<Timer> method, uint order = uint.MaxValue) =>
            OnEnd = Events.Add(OnEnd, method, order);
         public static void Pause(Action<Timer> method, uint order = uint.MaxValue) =>
            OnPause = Events.Add(OnPause, method, order);
         public static void Update(Action<Timer, double, double, double> method, uint order = uint.MaxValue) =>
            OnUpdate = Events.Add(OnUpdate, method, order);
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
         get { return ErrorIfDestroyed() ? double.NaN : Number.ToPercent(progress, new Bounds(0, Duration)); }
         set
         {
            if (ErrorIfDestroyed()) return;
            progress = Number.FromPercent(Number.Limit(value, new Bounds(0, 100)), new Bounds(0, Duration));
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

      public Timer(string uniqueID, double durationInSeconds) : base(uniqueID)
      {
         timers.Add(this);
         Duration = durationInSeconds;
         Countdown = Duration;
         OnCreateAndStart?.Invoke(this, Duration);
         if (cannotCreate) { ErrorAlreadyHasUID(uniqueID); Destroy(); }
      }
		public override void Destroy()
		{
         if (ErrorIfDestroyed()) return;
         timers.Remove(this);
         base.Destroy();
		}
   }
}
