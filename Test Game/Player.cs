using SMPL;
using System;

namespace TestGame
{
	public class Player : Events
	{
		public Component2D Component2D { get; set; } = new();

		public Player()
		{
			Subscribe(this, 0);

			Component2D.Position = new Point(100, 0);
			Component2D.GrantAccessToFile(Debug.CurrentFilePath(1));
			GrantAccessToFile(Debug.CurrentFilePath(1));
		}
	}
}
