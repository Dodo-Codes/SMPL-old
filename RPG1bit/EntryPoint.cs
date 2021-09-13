using SMPL.Gear;
using SMPL.Data;

namespace RPG1bit
{
	class EntryPoint : Game
	{
		static void Main() => Start(new EntryPoint(), new Size(1, 1));

		public override void OnGameCreated()
		{
			Assets.Load(Assets.Type.Texture, "graphics.png");
			//Window.CurrentState = Window.State.Fullscreen;

			Map.Create();
			Object.CreateAllObjects();
		}
	}
}
