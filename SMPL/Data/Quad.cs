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

		public Quad(Corner cornerA, Corner cornerB, Corner cornerC, Corner cornerD)
		{
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
	}
}
