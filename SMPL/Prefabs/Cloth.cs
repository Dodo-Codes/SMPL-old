using Newtonsoft.Json;
using SFML.Graphics;
using SMPL.Components;
using SMPL.Data;
using SMPL.Gear;
using System.Collections.Generic;
using Sprite = SMPL.Components.Sprite;

namespace SMPL.Prefabs
{
	public class Cloth : Thing
	{
		private const int FRAGMENTS = 10;
		private readonly List<string> removedQuads = new();
		private Point topLeft, downRight;

		//=============

		[JsonProperty]
		public string RopesUniqueID { get; set; }
		[JsonProperty]
		public string TexturePath { get; set; }
		[JsonProperty]
		public Data.Color Color { get; set; }

		public Cloth(string uniqueID, Point position, Size size) : base(uniqueID)
		{
			uniqueID = $"{uniqueID}-ropes";
			RopesUniqueID = uniqueID;

			SetTextureCoordinates(new Point(0, 0), new Point(100, 100));

			var r = new Ropes(RopesUniqueID);

			for (int y = 0; y < FRAGMENTS; y++)
				for (int x = 0; x < FRAGMENTS; x++)
					r.SetPoint($"{x} {y}", new RopePoint(new Point(x, y) * new Point(size.W, size.H) / FRAGMENTS + position, false));

			r.GetPoint("0 0").IsLocked = true;
			r.GetPoint($"{FRAGMENTS - 1} 0").IsLocked = true;

			for (int y = 0; y < FRAGMENTS; y++)
				for (int x = 0; x < FRAGMENTS; x++)
				{
					if (x != FRAGMENTS - 1) r.SetPointConnection($"{x} {y} right", $"{x} {y}", $"{x + 1} {y}");
					if (y != FRAGMENTS - 1) r.SetPointConnection($"{x} {y} down", $"{x} {y}", $"{x} {y + 1}");
				}
		}
		public override void Destroy()
		{
			if (ErrorIfDestroyed()) return;
			((Ropes)PickByUniqueID(RopesUniqueID)).Destroy();
			RopesUniqueID = null;
			base.Destroy();
		}

		public void Display(Camera camera)
		{
			if (ErrorIfNoTexture()) return;

			var r = (Ropes)PickByUniqueID(RopesUniqueID);
			var textureSize = downRight;
			var scale = new Size(textureSize.X, textureSize.Y) / FRAGMENTS;
			var quads = new Dictionary<string, Quad>();

			for (int y = 0; y < FRAGMENTS - 1; y++)
				for (int x = 0; x < FRAGMENTS - 1; x++)
				{
					var texturePos = new Point(x, y);
					texturePos = new Point(texturePos.X * scale.W, texturePos.Y * scale.H);

					if (removedQuads.Contains($"{x} {y}")) continue;

					var p1 = r.points[$"{x} {y}"].Position;
					var p2 = r.points[$"{x + 1} {y}"].Position;
					var p3 = r.points[$"{x + 1} {y + 1}"].Position;
					var p4 = r.points[$"{x} {y + 1}"].Position;

					if (Point.Distance(p1, p2) > scale.W * 5 ||
						Point.Distance(p2, p3) > scale.H * 5 ||
						Point.Distance(p3, p4) > scale.W * 5 ||
						Point.Distance(p4, p1) > scale.H * 5) continue;

					var q = new Quad(
						new Corner(p1, texturePos.X, texturePos.Y),
						new Corner(p2, texturePos.X + scale.W, texturePos.Y),
						new Corner(p3, texturePos.X + scale.W, texturePos.Y + scale.H),
						new Corner(p4, texturePos.X, texturePos.Y + scale.H));
					quads.Add($"{x} {y}", q);
				}
			var qs = Sprite.QuadsToVerts(quads);
			camera.rendTexture.Draw(qs.Item2, PrimitiveType.Quads, new RenderStates(Assets.textures[TexturePath]));
		}

		public void Cut(int x, int y)
		{
			if (removedQuads.Contains($"{x} {y}")) return;
			removedQuads.Add($"{x} {y}");

			var rope = (Ropes)PickByUniqueID(RopesUniqueID);
			if (x != 9) rope.RemovePointConnection($"{x} {y} right");
			if (y != 9) rope.RemovePointConnection($"{x} {y} down");

			if (x == 9) Cut(8, y);
			if (y == 9) Cut(x, 8);
		}
		public void SetTextureCoordinatesDefault()
		{
			if (ErrorIfNoTexture()) return;

			var sz = TexturePath == null || Assets.textures.ContainsKey(TexturePath) == false ? new Size(100, 100) :
				new Size(Assets.textures[TexturePath].Size.X, Assets.textures[TexturePath].Size.Y);

			topLeft = new Point(0, 0);
			downRight = new Point(sz.W, sz.H);
		}
		public void SetTextureCoordinates(Point topLeft, Point downRight)
		{
			this.topLeft = topLeft;
			this.downRight = downRight;
		}

		private bool ErrorIfNoTexture()
		{
			if (TexturePath == null)
			{
				Debug.LogError(2, $"No texture is taken into account. Use the {nameof(TexturePath)} property if texture is needed.");
				return true;
			}
			if (Assets.textures.ContainsKey(TexturePath) == false)
			{
				Assets.NotLoadedError(2, Assets.Type.Texture, TexturePath);
				return true;
			}
			return false;
		}
	}
}
