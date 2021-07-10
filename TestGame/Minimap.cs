using SMPL;

namespace TestGame
{
	public class Minimap : Camera
	{
		public Minimap(Point viewPosition, Size viewSize) :
			base(viewPosition, viewSize)
		{
			Subscribe(this);
			IdentityComponent = new(this, "minimap");
			BackgroundColor = Color.DarkRed;
			TransformComponent.Position = new Point(400, 0);
			TransformComponent.Size = new Size(500, 500);
		}
	}
}
