using System.Collections.Generic;

namespace SMPL
{
	public abstract class TimeEvents
	{
		internal static List<TimeEvents> instances = new();

		internal static void Update()
		{
			foreach (var i in instances)
			{
				var timerUIDs = IdentityComponent<Timer>.GetAllUniqueIDs();
            foreach (var uid in timerUIDs)
            {
					var timer = IdentityComponent<Timer>.PickByUniqueID(uid);
					if (timer.IsPaused) continue;
					if (timer.Countdown > 0) timer.Countdown -= Time.DeltaTime;
					if (Gate.EnterOnceWhile(uid, timer.Countdown <= 0))
					{
						timer.EndCount++;
						timer.Countdown = 0;
						i.OnTimerEnd(uid, timer);
					}
            }
				i.OnEachFrame();
			}
		}

		public static void Subscribe(TimeEvents instance) => instances.Add(instance);
		public virtual void OnEachFrame() { }
		public virtual void OnTimerEnd(string timerUniqueID, Timer timerInstance) { }
	}
}
