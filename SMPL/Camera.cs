using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;

namespace SMPL
{
	public class Camera
	{
		public static Camera WorldCamera { get; internal set; }
		public IdentityComponent<Camera> IdentityComponent { get; set; }
		public TransformComponent TransformComponent { get; set; }

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

		internal static void DrawCameras()
		{
			WorldCamera.StartDraw();
			WorldCameraEvents.instance.OnDraw();
			foreach (var kvp in sortedCameras)
			{
				foreach (var camera in kvp.Value)
				{
					if (camera == WorldCamera) continue;
					camera.StartDraw();
					camera.OnDraw();
					camera.EndDraw();
				}
			}
			WorldCamera.EndDraw();
		}
		internal void StartDraw()
		{
			rendTexture.SetView(view);
			rendTexture.Clear(Color.From(BackgroundColor));
		}
		public virtual void OnDraw() { }
		internal void EndDraw()
		{
			rendTexture.Display();
			var pos = Point.From(TransformComponent.Position);
			var sz = new Vector2i((int)rendTexture.Size.X, (int)rendTexture.Size.Y);
			//var s = new Vector2i((int)view.Size.X, (int)view.Size.Y);
			var tsz = rendTexture.Size;
			var sc = new Vector2f(
				(float)TransformComponent.Size.Width / (float)tsz.X,
				(float)TransformComponent.Size.Height / (float)tsz.Y);
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

		public Camera(Point viewPosition, Size viewSize)
		{
			if (sortedCameras.ContainsKey(0) == false) sortedCameras[0] = new List<Camera>();
			sortedCameras[0].Add(this);

			var pos = Point.From(viewPosition);
			TransformComponent = new(new Point(), 0, new Size(100, 100));
			view = new View(pos, Size.From(viewSize));
			rendTexture = new RenderTexture((uint)viewSize.Width, (uint)viewSize.Height);
			BackgroundColor = Color.DarkGreen;
			startSize = viewSize;
			Zoom = 1;
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

		public void DrawLines(params Line[] lines)
		{
			foreach (var line in lines)
			{
				var vert = new Vertex[]
				{
					new(Point.From(line.StartPosition), Color.From(line.StartPosition.Color)),
					new(Point.From(line.EndPosition), Color.From(line.EndPosition.Color))
				};
				rendTexture.Draw(vert, PrimitiveType.Lines);
			}
		}
		public void DrawPoints(params Point[] points)
		{
			foreach (var p in points)
			{
				var vert = new Vertex[] { new(Point.From(p), Color.From(p.Color)) };
				rendTexture.Draw(vert, PrimitiveType.Points);
			}
		}
		public void DrawShapes(params Shape[] shapes)
		{
			foreach (var shape in shapes)
			{
				var points = new List<Point>() { shape.PointA, shape.PointB, shape.PointC, shape.PointD };
				var vert = new Vertex[points.Count];
				for (int i = 0; i < points.Count ; i++)
				{
					var p = points[i];
					vert[i] = new Vertex(Point.From(p), Color.From(p.Color));
				}

				rendTexture.Draw(vert, PrimitiveType.Quads);
			}
		}
		public void DrawSprites(params SpriteComponent[] spriteComponents)
		{
			foreach (var s in spriteComponents)
			{
				if (s == null || s.sprite == null || s.sprite.Texture == null || s.TransformComponent == null) continue;
				s.sprite.Position = Point.From(s.TransformComponent.Position);
				s.sprite.Rotation = (float)s.TransformComponent.Angle;
				s.sprite.Scale = new Vector2f(
					(float)s.TransformComponent.Size.Width / s.sprite.Texture.Size.X,
					(float)s.TransformComponent.Size.Height / s.sprite.Texture.Size.Y);

				rendTexture.Draw(s.sprite, new RenderStates(s.Effects.shader));
			}
		}
	}
}
