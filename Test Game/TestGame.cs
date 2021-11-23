using SMPL.Components;
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
			var instance = new List<Test>() { new Test("uid"), new Test("2") };
			var json = Text.ToJSON(instance);

			instance[0].Destroy();
			instance[1].Destroy();

			var loaded = Text.FromJSON<List<Test>>(json);
		}
	}

	public class Test : Thing
	{
		public Test(string uniqueID) : base(uniqueID) { }

		public bool Bool;
		public string String;
		public int Int;
	}
}
