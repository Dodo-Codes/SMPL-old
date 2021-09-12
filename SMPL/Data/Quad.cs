﻿using SMPL.Components;
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

		public Quad(Corner cornerA, Corner cornerB, Corner cornerC, Corner cornerD)
		{
			TileSize = new Size(32, 32);
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
			CornerA = new(CornerA.Position, new Point(TileSize.W, TileSize.H) * (tileIndexes + new Point(0, 0)));
			CornerB = new(CornerB.Position, new Point(TileSize.W, TileSize.H) * (tileIndexes + new Point(1, 0)));
			CornerC = new(CornerC.Position, new Point(TileSize.W, TileSize.H) * (tileIndexes + new Point(1, 1)));
			CornerD = new(CornerD.Position, new Point(TileSize.W, TileSize.H) * (tileIndexes + new Point(0, 1)));
		}
	}
}
