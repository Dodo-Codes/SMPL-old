using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMPL
{
	public abstract class TimeEvents
	{
		internal static TimeEvents instance;
		internal void Subscribe()
		{
			instance = this;
			OnStart();
		}

		public virtual void OnStart() { }
		public virtual void OnEachTick() { }
	}
}
