using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMPL
{
	public abstract class FileEvents
	{
		internal static FileEvents instance;

		internal void Subscribe() => instance = this;
		public virtual void OnLoadingStart() { }
		public virtual void OnLoadingUpdate() { }
		public virtual void OnLoadingEnd() { }
	}
}
