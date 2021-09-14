using SMPL.Components;
using SMPL.Gear;
using System.Collections.Generic;
using SFML.System;
using SFML.Graphics;
using System;

namespace SMPL.Data
{
	public struct Quad
	{
		public Corner CornerA { get; set; }
		public Corner CornerB { get; set; }
		public Corner CornerC { get; set; }
		public Corner CornerD { get; set; }
		public Size TileSize { get; set; }
		public Size TileGridWidth { get; set; }

		public Quad(Corner cornerA, Corner cornerB, Corner cornerC, Corner cornerD)
		{
			TileSize = new Size(32, 32);
			TileGridWidth = new Size(0, 0);
			CornerA = cornerA;
			CornerB = cornerB;
			CornerC = cornerC;
			CornerD = cornerD;
		}
		public void Display(Camera camera)
		{
			if (Window.DrawNotAllowed()) return;
			var verts = new Vertex[] { CornerA.Vertex, CornerB.Vertex, CornerC.Vertex, CornerD.Vertex };
			camera.rendTexture.Draw(verts, PrimitiveType.Quads);
		}

		public void SetTextureCropCoordinates(Point topLeft, Point downRight)
		{
			CornerA = new(CornerA.Position, topLeft);
			CornerB = new(CornerB.Position, new Point(downRight.X, topLeft.Y));
			CornerC = new(CornerC.Position, downRight);
			CornerD = new(CornerD.Position, new Point(topLeft.X, downRight.Y));
		}
		public void SetTextureCropTile(Point tileIndexes)
		{
			var tileSizePoint = new Point(TileSize.W + TileGridWidth.W, TileSize.H + TileGridWidth.H);
			var szPoint = new Point(TileGridWidth.W, TileGridWidth.H);
			CornerA = new(CornerA.Position, tileSizePoint * (tileIndexes + new Point(0, 0)));
			CornerB = new(CornerB.Position, tileSizePoint * (tileIndexes + new Point(1, 0)) - new Point(TileGridWidth.W, 0));
			CornerC = new(CornerC.Position, tileSizePoint * (tileIndexes + new Point(1, 1)) - szPoint);
			CornerD = new(CornerD.Position, tileSizePoint * (tileIndexes + new Point(0, 1)) - new Point(0, TileGridWidth.H));
		}
		public void SetColor(Color colorCornerA, Color colorCornerB, Color colorCornerC, Color colorCornerD)
		{
			CornerA = new(new Point(CornerA.Position.X, CornerA.Position.Y) { Color = colorCornerA }, CornerA.TextureCoordinate);
			CornerB = new(new Point(CornerB.Position.X, CornerB.Position.Y) { Color = colorCornerB }, CornerB.TextureCoordinate);
			CornerC = new(new Point(CornerC.Position.X, CornerC.Position.Y) { Color = colorCornerC }, CornerC.TextureCoordinate);
			CornerD = new(new Point(CornerD.Position.X, CornerD.Position.Y) { Color = colorCornerD }, CornerD.TextureCoordinate);
		}
	}
}
