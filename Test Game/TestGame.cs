﻿using SMPL.Components;
using SMPL.Data;
using SMPL.Gear;
using SMPL.Prefabs;
using System.Collections.Generic;

namespace TestGame
{
   public class TestGame : Game
   {
		public TestGame(string uniqueID) : base(uniqueID) { }
      public static void Main() => Start(new TestGame("test-game"), new(1, 1));

		public override void OnGameCreate()
		{

		}
	}
}
