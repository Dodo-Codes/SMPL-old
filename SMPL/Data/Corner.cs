using SFML.Graphics;
using SFML.System;

namespace SMPL.Data
{
	public struct Corner
	{
		public Point Position { get; set; }
		public Point TextureCoordinate { get; set; }
		internal Vertex Vertex =>
			new(Point.From(Position), Color.From(Position.C), Point.From(TextureCoordinate));

		public Corner(Point position, Point textureCoordinate = default)
		{
			Position = position;
			TextureCoordinate = textureCoordinate;
		}
	}
}
