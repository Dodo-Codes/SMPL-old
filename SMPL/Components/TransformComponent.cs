namespace SMPL
{
	public class TransformComponent
	{
		public Point Position { get; set; }
		public double Angle { get; set; }
		public Size Size { get; set; }

		public TransformComponent(Point position, double angle, Size size)
		{
			Position = position;
			Angle = angle;
			Size = size;
		}
	}
}
