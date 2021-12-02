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
		private const int FRAGMENTS = 6;
		private readonly List<string> removedQuads = new();
		private Point position;
		private Size size;

		//=============

		[JsonProperty]
		public uint RopesUID { get; set; }
		[JsonProperty]
		public string TexturePath { get; set; }
		[JsonProperty]
		public Data.Color Color { get; set; }

		public Cloth(Point position, Size size)
		{
			SetTextureCropCoordinates(new Point(0, 0), new Size(100, 100));

			var r = new Ropes();
			RopesUID = r.UID;

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
			((Ropes)Pick(RopesUID)).Destroy();
			RopesUID = default;
			base.Destroy();
		}

		public void Display(Camera camera)
		{
			if (ErrorIfDestroyed() || Window.DrawNotAllowed() || ErrorIfNoTexture() || camera.Captures(this) == false) return;

			var r = (Ropes)Pick(RopesUID);
			var fragSz = size / (FRAGMENTS - 1);
			var quads = new SortedDictionary<string, Quad>();
			var sz = GetSize();

			for (int y = 0; y < FRAGMENTS - 1; y++)
				for (int x = 0; x < FRAGMENTS - 1; x++)
				{
					var curPos = new Point(x * fragSz.W, y * fragSz.H);

					if (removedQuads.Contains($"{x} {y}")) continue;

					var p1 = r.points[$"{x} {y}"].Position;
					var p2 = r.points[$"{x + 1} {y}"].Position;
					var p3 = r.points[$"{x + 1} {y + 1}"].Position;
					var p4 = r.points[$"{x} {y + 1}"].Position;

					if (Point.Distance(p1, p2) > sz.W ||
						Point.Distance(p2, p3) > sz.H ||
						Point.Distance(p3, p4) > sz.W ||
						Point.Distance(p4, p1) > sz.H) continue;

					var q = new Quad(
						new Corner(p1, position + curPos),
						new Corner(p2, position + curPos + new Point(fragSz.W, 0)),
						new Corner(p3, position + curPos + new Point(fragSz.W, fragSz.H)),
						new Corner(p4, position + curPos + new Point(0, fragSz.H)));
					quads.Add($"{x} {y}", q);
				}
			var qs = Sprite.QuadsToVerts(quads);
			camera.rendTexture.Draw(qs.Item2, PrimitiveType.Quads, new RenderStates(Assets.textures[TexturePath]));
			Performance.DrawCallsPerFrame++;
		}

		public void Cut(int x, int y)
		{
			if (removedQuads.Contains($"{x} {y}")) return;
			removedQuads.Add($"{x} {y}");

			var rope = (Ropes)Pick(RopesUID);
			if (x != 9) rope.RemovePointConnection($"{x} {y} right");
			if (y != 9) rope.RemovePointConnection($"{x} {y} down");

			if (x == 9) Cut(8, y);
			if (y == 9) Cut(x, 8);
		}
		public void SetTextureCropDefault()
		{
			if (ErrorIfNoTexture()) return;
			var sz = GetSize();
			position = new Point(0, 0);
			size = sz;
		}
		public void SetTextureCropCoordinates(Point position, Size size)
		{
			this.position = position;
			this.size = size;
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
		private Size GetSize()
		{
			var sz = TexturePath == null || Assets.textures.ContainsKey(TexturePath) == false ? new Size(100, 100) :
				new Size(Assets.textures[TexturePath].Size.X, Assets.textures[TexturePath].Size.Y);
			return sz;
		}
	}
}
