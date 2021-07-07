using System.Collections.Generic;

namespace SMPL
{
	public abstract class TimeEvents
	{
		internal static List<TimeEvents> instances = new();
		internal static void Update()
		{
			foreach (var i in instances)
				i.OnEachFrame();
		}

		public static void Subscribe(TimeEvents instance) => instances.Add(instance);
		public virtual void OnEachFrame() { }
	}
}
