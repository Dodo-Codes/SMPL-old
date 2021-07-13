using SMPL;

namespace TestGame
{
	public class Game : SMPL.Game
	{
		public static void Main() => Run(new Game(), new Size(1, 1));

      public override void OnStart()
      {
         new Player();
         var m = new Minimap(new Point(0, 0), new Size(500, 500));
         m.TransformComponent.Position = new Point(-600, 0);
      }
   }
}
