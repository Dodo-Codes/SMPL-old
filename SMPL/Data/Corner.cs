using SFML.Graphics;
using SFML.System;

namespace SMPL.Data
{
	public struct Corner
	{
		public Point Position { get; set; }
		public double TextureX { get; set; }
		public double TextureY { get; set; }
		internal Vertex Vertex =>
			new(Point.From(Position), Color.From(Position.Color), new Vector2f((float)TextureX, (float)TextureY));

		public Corner(Point position, double textureX = 0, double textureY = 0)
		{
			Position = position;
			TextureX = textureX;
			TextureY = textureY;
		}
	}
}
