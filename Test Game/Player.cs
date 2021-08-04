using SMPL;
using System;

namespace TestGame
{
	public class Player : Events
	{
		public Player()
		{
			Subscribe(this, 0);
		}
	}
}
