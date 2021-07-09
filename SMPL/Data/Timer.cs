namespace SMPL
{
   public class Timer
   {
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

      public IdentityComponent<Timer> IdentityComponent { get; set; }

      public Timer(string uniqueID, double duration)
      {
         IdentityComponent = new(this, uniqueID);
         Duration = duration;
         Countdown = duration;
      }
   }
}
