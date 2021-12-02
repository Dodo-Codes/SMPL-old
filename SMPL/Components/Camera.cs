using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using SMPL.Data;
using SMPL.Gear;
using SMPL.Prefabs;
using System.IO;

namespace SMPL.Components
{
	public class Camera : Thing
	{
		private double depth;
		private Data.Color bgColor;

		internal static SortedDictionary<double, List<Camera>> sortedCameras = new();
		internal View view;
		internal SFML.Graphics.Sprite sprite = new();
		internal RenderTexture rendTexture;
		internal Size startSize;
		internal uint displayUID;

		internal static void DrawCameras()
		{
			WorldCamera.StartDraw();
			Events.Notify(Game.Event.CameraDisplay, new() { Camera = WorldCamera });
			foreach (var kvpp in sortedCameras)
				for (int j = 0; j < kvpp.Value.Count; j++)
				{
					if (kvpp.Value[j] == WorldCamera) continue;
					kvpp.Value[j].StartDraw();
					Events.Notify(Game.Event.CameraDisplay, new() { Camera = kvpp.Value[j] });
				}
			WorldCamera.EndDraw();
		}
		internal void StartDraw()
		{
			rendTexture.SetView(view);
			rendTexture.Clear(Data.Color.From(BackgroundColor));
		}
		internal void EndDraw()
		{
			rendTexture.Display();
			var DisplayArea = (Area)Pick(displayUID);
			var pos = Point.From(DisplayArea.Position);
			var sz = new Vector2i((int)rendTexture.Size.X, (int)rendTexture.Size.Y);
			var tsz = rendTexture.Size;
			var sc = new Vector2f(
				(float)DisplayArea.Size.W / (float)tsz.X,
				(float)DisplayArea.Size.H / (float)tsz.Y);
			var or = new Vector2f(rendTexture.Size.X / 2, rendTexture.Size.Y / 2);

			sprite.Origin = or;
			sprite.Texture = rendTexture.Texture;
			sprite.Rotation = (float)DisplayArea.Angle;
			sprite.Position = pos;
			sprite.TextureRect = new IntRect(new Vector2i(), sz);

			if (this == WorldCamera)
			{
				sprite.Position = new Vector2f();
				sprite.Rotation = 0;
				Window.window.Draw(sprite);
			}
			else
			{
				sprite.Scale = sc;
				WorldCamera.rendTexture.Draw(sprite);
			}
		}

		public static Camera WorldCamera { get; internal set; }

		public uint AreaDisplayUID
		{
			get { return ErrorIfDestroyed() ? default : displayUID; }
			set { if (ErrorIfDestroyed() == false) displayUID = value; }
		}
		public double Depth
		{
			get { return ErrorIfDestroyed() ? double.NaN : depth; }
			set
			{
				if (ErrorIfDestroyed() || this == WorldCamera) return;
				var oldDepth = depth;
				depth = value;
				sortedCameras[oldDepth].Remove(this);
				if (sortedCameras.ContainsKey(depth) == false) sortedCameras[depth] = new List<Camera>();
				sortedCameras[depth].Add(this);
			}
		}
		public Point Position
		{
			get { return ErrorIfDestroyed() ? Point.Invalid : Point.To(view.Center); }
			set { if (ErrorIfDestroyed() == false) view.Center = Point.From(value); UpdateSprite(); }
		}
		public double Angle
		{
			get { return ErrorIfDestroyed() ? double.NaN : view.Rotation; }
			set { if (ErrorIfDestroyed() == false) view.Rotation = (float)value; UpdateSprite(); }
		}
		public Size Size
		{
			get { return ErrorIfDestroyed() ? Size.Invalid : new Size(view.Size.X, view.Size.Y); }
			set { if (ErrorIfDestroyed() == false) view.Size = Size.From(value); UpdateSprite(); }
		}
		public Data.Color BackgroundColor
		{
			get { return ErrorIfDestroyed() ? Data.Color.Invalid : bgColor; }
			set { if (ErrorIfDestroyed() == false) bgColor = value; }
		}

		public Camera(Point viewPosition, Size viewSize)
		{
			if (sortedCameras.ContainsKey(0) == false) sortedCameras[0] = new List<Camera>();
			sortedCameras[0].Add(this);

			var pos = Point.From(viewPosition);
			view = new View(pos, Size.From(viewSize));
			rendTexture = new RenderTexture((uint)viewSize.W, (uint)viewSize.H);
			BackgroundColor = Data.Color.Black;
			startSize = viewSize;
			UpdateSprite();
		}
		public override void Destroy()
		{
			if (ErrorIfDestroyed() || this == WorldCamera) return;
			sortedCameras[depth].Remove(this);
			view.Dispose();
			view = default;
			sprite.Dispose();
			sprite = default;
			rendTexture.Dispose();
			rendTexture = default;
			AreaDisplayUID = default;
			base.Destroy();
		}

		public void Snap(string filePath = "folder/picture.png")
		{
			if (ErrorIfDestroyed())
				return;
			var img = rendTexture.Texture.CopyToImage();
			Directory.CreateDirectory(Path.GetDirectoryName(filePath));

			if (img.SaveToFile(filePath))
				img.Dispose();
			else
				Debug.LogError(1, $"Could not save picture '{filePath}'.");
		}
		public bool Captures(Thing thing)
		{
			UpdateSprite();
			if (thing is Cloth cloth) return RopeCheck(cloth.RopesUID);
			else if (thing is Ropes) return RopeCheck(thing.UID);
			else if (thing is SegmentedLine sl)
			{
				for (int i = 0; i < sl.Points.Length; i++)
					if (ContainsPoint(sl.Points[i]))
						return true;
			}
			else if (thing is Shape3D shape)
			{
				shape.UpdateLines(this);
				for (int i = 0; i < shape.lines.Length; i++)
					if (ContainsPoint(shape.lines[i].StartPosition) || ContainsPoint(shape.lines[i].EndPosition))
						return true;
			}
			else if (thing is LayeredShape3D ls3d)
			{
				if (ls3d.AreaUID == default || uids.ContainsKey(ls3d.AreaUID) == false)
					return false;
				var area = (Area)Pick(ls3d.AreaUID);
				var first = AreaCheck(area.UID);
				var prevPos = area.Position;
				area.Position = Point.MoveAtAngle(area.Position, ls3d.LayerStackAngle, ls3d.LayerStackCount * ls3d.LayerStackSpacing,
					Gear.Time.Unit.Frame);
				var second = AreaCheck(area.UID);
				area.Position = prevPos;
				return first || second;
			}
			else if (thing is Area) return AreaCheck(thing.UID);
			else if (thing is Sprite spr) return AreaCheck(spr.AreaUID);
			return false;

			bool RopeCheck(uint uid)
			{
				var rope = (Ropes)Pick(uid);
				foreach (var kvp in rope.points)
					if (ContainsPoint(kvp.Value.Position))
						return true;
				return false;
			}
			bool AreaCheck(uint uid)
			{
				var area = (Area)Pick(uid);
				if (area == null) return false;
				area.UpdateSprite();
				return area.sprite.GetGlobalBounds().Intersects(sprite.GetGlobalBounds());
			}
		}

		private void UpdateSprite()
		{
			sprite.Position = view.Center;
			sprite.Rotation = view.Rotation;
			sprite.Origin = view.Size / 2;
		}
		private bool ContainsPoint(Point point)
		{
			return sprite.GetGlobalBounds().Contains((float)point.X, (float)point.Y);
		}
	}
}
