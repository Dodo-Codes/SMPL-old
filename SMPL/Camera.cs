using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;

namespace SMPL
{
	public class Camera : Events
	{
		public static Camera WorldCamera { get; internal set; }
		public ComponentIdentity<Camera> IdentityComponent { get; set; }
		public Component2D TransformComponent { get; set; }

		internal static SortedDictionary<double, List<Camera>> sortedCameras = new();

		private double depth;
		public double Depth
		{
			get { return depth; }
			set
			{
				var oldDepth = depth;
				depth = value;
				sortedCameras[oldDepth].Remove(this);
				if (sortedCameras.ContainsKey(depth) == false) sortedCameras[depth] = new List<Camera>();
				sortedCameras[depth].Add(this);
			}
		}
		internal View view;
		internal Sprite sprite = new();

		internal RenderTexture rendTexture;
		internal Size startSize;

		public Point Position
		{
			get { return new Point(view.Center.X, view.Center.Y); }
			set { view.Center = Point.From(value); }
		}
		public double Angle
		{
			get { return view.Rotation; }
			set { view.Rotation = (float)value; }
		}
		public Size Size
		{
			get { return new Size(view.Size.X, view.Size.Y); }
			set { view.Size = Size.From(value); }
		}
		private double zoom = 1;
		public double Zoom
		{
			get { return zoom; }
			set
			{
				zoom = Number.Limit(value, new Bounds(0.001, 500));
				view.Size = Size.From(startSize / zoom);
			}
		}
		public Color BackgroundColor { get; set; }

		public Camera(Point viewPosition, Size viewSize)
		{
			if (sortedCameras.ContainsKey(0) == false) sortedCameras[0] = new List<Camera>();
			sortedCameras[0].Add(this);

			var pos = Point.From(viewPosition);
			TransformComponent = new();
			view = new View(pos, Size.From(viewSize));
			rendTexture = new RenderTexture((uint)viewSize.W, (uint)viewSize.H);
			BackgroundColor = Color.GreenDark;
			startSize = viewSize;
			Zoom = 1;
		}

		internal static void DrawCameras()
		{
			WorldCamera.StartDraw();
			{ var n = D(instances); foreach (var kvp in n) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++)
						e[i].OnDrawSetup(WorldCamera); }
				var n1 = D(instances); foreach (var kvp in n1) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++)
						e[i].OnDraw(WorldCamera); } }
			foreach (var kvpp in sortedCameras)
			{
				for (int j = 0; j < kvpp.Value.Count; j++)
				{
					if (kvpp.Value[j] == WorldCamera) continue;
					kvpp.Value[j].StartDraw();
					var n = D(instances); foreach (var kvp in n) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++)
							e[i].OnDrawSetup(kvpp.Value[j]); }
					var n1 = D(instances); foreach (var kvp in n1) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++)
							e[i].OnDraw(kvpp.Value[j]); }
					kvpp.Value[j].EndDraw();
				}
			}
			WorldCamera.EndDraw();
		}
		internal void StartDraw()
		{
			rendTexture.SetView(view);
			rendTexture.Clear(Color.From(BackgroundColor));
		}
		internal void EndDraw()
		{
			rendTexture.Display();
			var pos = Point.From(TransformComponent.Position);
			var sz = new Vector2i((int)rendTexture.Size.X, (int)rendTexture.Size.Y);
			//var s = new Vector2i((int)view.Size.X, (int)view.Size.Y);
			var tsz = rendTexture.Size;
			var sc = new Vector2f(
				(float)TransformComponent.Size.W / (float)tsz.X,
				(float)TransformComponent.Size.H / (float)tsz.Y);
			var or = new Vector2f(rendTexture.Size.X / 2, rendTexture.Size.Y / 2);

			sprite.Origin = or;
			sprite.Texture = rendTexture.Texture;
			sprite.Rotation = (float)TransformComponent.Angle;
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

		public bool Snap(string filePath = "folder/picture.png")
		{
			var img = rendTexture.Texture.CopyToImage();
			var full = File.CreateDirectoryForFile(filePath);

			if (img.SaveToFile(filePath)) img.Dispose();
			else
			{
				Debug.LogError(1, $"Could not save picture '{full}'.");
				return false;
			}
			return true;
		}
   }
}
