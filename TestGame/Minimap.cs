using SMPL;

namespace TestGame
{
	public class Minimap : Camera
	{
		public Minimap(Point viewPosition, Size viewSize) :
			base(viewPosition, viewSize)
		{
			Subscribe(this, 0);
			IdentityComponent = new(this, "minimap");
			BackgroundColor = Color.BlueDark;
			TransformComponent.Position = new Point(400, 0);
			TransformComponent.LocalSize = new Size(500, 500);
		}
   }
}
