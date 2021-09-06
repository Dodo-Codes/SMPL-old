using SMPL.Components;
using SMPL.Data;
using SMPL.Gear;
using SMPL.Prefabs;

namespace TestGame
{
	public class Player
	{
		public Player()
		{
			var slot = new SaveSlot();
			slot.SetValue("test", new Area("test"));
			slot.Save("test-slot");

			SaveSlot.Load("test-slot");
			var value = SaveSlot.GetValue("test");
		}
	}
}