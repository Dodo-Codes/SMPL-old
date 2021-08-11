using System;

namespace SMPL
{
   public class Timer
   {
      private static event Events.ParamsOne<Timer> OnTimerEnd;
      public static void CallOnTimerEnd(Action<Timer> method, uint order = uint.MaxValue) =>
         OnTimerEnd = Events.Add(OnTimerEnd, method, order);

      public int EndCount { get; set; }
      public double Duration { get; private set; }
      private double progress;
      public double Progress
      {
         get { return progress; }
         set
         {
            progress = value;
            countdown = Duration - value;
         }
      }
      private double countdown;
      public double Countdown
      {
         get { return countdown; }
         set
         {
            countdown = value;
            progress = Duration - value;
         }
      }
      public bool IsPaused { get; set; }

      public ComponentIdentity<Timer> IdentityComponent { get; set; }

      public Timer(string uniqueID, double duration)
      {
         IdentityComponent = new(this, uniqueID);
         Duration = duration;
         Countdown = duration;
      }

      internal static void Update()
      {
         var timerUIDs = ComponentIdentity<Timer>.AllUniqueIDs;
         for (int j = 0; j < timerUIDs.Length; j++)
         {
            var timer = ComponentIdentity<Timer>.PickByUniqueID(timerUIDs[j]);
            if (timer.IsPaused) continue;
            if (timer.Countdown > 0) timer.Countdown -= Performance.DeltaTime;
            if (Gate.EnterOnceWhile(timerUIDs[j] + "as;li3'f2", timer.Countdown <= 0))
            {
               timer.EndCount++;
               timer.Countdown = 0;
               OnTimerEnd?.Invoke(timer);
            }
         }
      }
   }
}
