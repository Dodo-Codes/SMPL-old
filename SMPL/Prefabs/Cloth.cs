using Newtonsoft.Json;
using SFML.Graphics;
using SMPL.Components;
using SMPL.Data;
using SMPL.Gear;
using System.Collections.Generic;
using Sprite = SMPL.Components.Sprite;

namespace SMPL.Prefabs
{
	public class Cloth : Component
	{
		private const int FRAGMENTS = 10;
		[JsonProperty]
		public string RopesUniqueID { get; set; }
		//=============

		[JsonProperty]
		public string TexturePath { get; set; }

		public Cloth(string uniqueID, Point position, Size size) : base(uniqueID)
		{
			uniqueID = $"{uniqueID}-ropes";
			RopesUniqueID = uniqueID;

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
			if (TexturePath == null || Assets.textures.ContainsKey(TexturePath) == false)
			{
				Assets.NotLoadedError(Assets.Type.Texture, TexturePath);
				return;
			}
			var r = (Ropes)PickByUniqueID(RopesUniqueID);
			var textureSize = Assets.textures[TexturePath].Size;
			var scale = new Size(textureSize.X, textureSize.Y) / FRAGMENTS;
			var quads = new Dictionary<string, Quad>();

			for (int y = 0; y < FRAGMENTS - 1; y++)
				for (int x = 0; x < FRAGMENTS - 1; x++)
				{
					var texturePos = new Point(x, y);
					texturePos = new Point(texturePos.X * scale.W, texturePos.Y * scale.H);

					var q = new Quad(
						new Corner(r.points[$"{x} {y}"].Position, texturePos.X, texturePos.Y),
						new Corner(r.points[$"{x + 1} {y}"].Position, texturePos.X + scale.W, texturePos.Y),
						new Corner(r.points[$"{x + 1} {y + 1}"].Position, texturePos.X + scale.W, texturePos.Y + scale.H),
						new Corner(r.points[$"{x} {y + 1}"].Position, texturePos.X, texturePos.Y + scale.H));
					quads.Add($"{x} {y}", q);
				}
			var qs = Sprite.QuadsToVerts(quads);
			camera.rendTexture.Draw(qs.Item2, PrimitiveType.Quads, new RenderStates(Assets.textures[TexturePath]));
		}
	}
}
