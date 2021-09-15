using SMPL.Gear;
using SMPL.Data;

namespace RPG1bit
{
	class EntryPoint : Game
	{
		static void Main() => Start(new EntryPoint(), new Size(1, 1));
		public override void OnGameCreated()
		{
			Assets.Load(Assets.Type.Texture, "Assets\\graphics.png");
			Assets.Load(Assets.Type.Font, "Assets\\font.ttf");
			Window.CallWhen.Focus(OnFocus);
			Assets.CallWhen.LoadEnd(OnAssetsLoaded);
			Window.Title = "Combathrone";
			Window.CurrentState = Window.State.Fullscreen;
			Mouse.Cursor.IsHidden = true;
		}

		private void OnAssetsLoaded()
		{
			if (Assets.AreLoaded("Assets\\graphics.png", "Assets\\font.ttf") == false) return;

			Screen.Create();
			Object.CreateAllObjects();
			Info.Create();
			Hoverer.Create();

			Screen.Display();
		}

		private static void OnFocus()
		{
			Window.CurrentState = Window.State.Fullscreen;
		}
	}
}
