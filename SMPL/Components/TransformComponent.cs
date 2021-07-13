using SFML.Graphics;

namespace SMPL
{
	public class TransformComponent
	{
		public bool PositionIsLocked { get; set; }
		private Point position;
		public Point Position
		{
			get { return position; }
			set
			{
				if (PositionIsLocked) return;
				position = value;
			}
		}
		public bool AngleIsLocked { get; set; }
		private double angle;
		public double Angle
		{
			get { return angle; }
			set
			{
				if (AngleIsLocked) return;
				angle = value;
			}
		}
		public bool SizeIsLocked { get; set; }
		private Size size;
		public Size Size
		{
			get { return size; }
			set
			{
				if (SizeIsLocked) return;
				size = value;
			}
		}

		public TransformComponent(Point position, double angle, Size size)
		{
			Position = position;
			Angle = angle;
			Size = size;
		}
	}
}
