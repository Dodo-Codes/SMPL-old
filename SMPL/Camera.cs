using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;

namespace SMPL
{
	public class Camera
	{
		public static Camera WorldCamera { get; internal set; }
		public IdentityComponent<Camera> IdentityComponent { get; internal set; } = new();
		public TransformComponent TransformComponent { get; internal set; }
		public EffectComponent EffectComponent { get; internal set; } = new();

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
			set { view.Center = new Vector2f((float)value.X, (float)value.Y); }
		}
		public double Angle
		{
			get { return view.Rotation; }
			set { view.Rotation = (float)value; }
		}
		public Size Size
		{
			get { return new Size(view.Size.X, view.Size.Y); }
			set { view.Size = new Vector2f((float)value.Width, (float)value.Height); }
		}
		private double zoom = 1;
		public double Zoom
		{
			get { return zoom; }
			set
			{
				zoom = Number.Limit(value, new Bounds(0.001, 500));
				view.Size = new Vector2f((float)(startSize.Width / zoom), (float)(startSize.Height / zoom));
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
			rendTexture.Clear(new SFML.Graphics.Color(
				(byte)BackgroundColor.Red, (byte)BackgroundColor.Green, (byte)BackgroundColor.Blue, (byte)BackgroundColor.Alpha));
		}
		public virtual void OnDraw() { }
		internal void EndDraw()
		{
			rendTexture.Display();
			var pos = new Vector2f((float)TransformComponent.Position.X, (float)TransformComponent.Position.Y);
			var sz = new Vector2i((int)rendTexture.Size.X, (int)rendTexture.Size.Y);
			//var s = new Vector2i((int)view.Size.X, (int)view.Size.Y);
			//var tsz = rendTexture.Size;
			//var sc = new Vector2f((float)tsz.X / (float)s.X, (float)tsz.Y / (float)s.Y);
			var or = new Vector2f(rendTexture.Size.X / 2, rendTexture.Size.Y / 2);

			sprite.Origin = or;
			sprite.Texture = rendTexture.Texture;
			sprite.Rotation = (float)TransformComponent.Angle;
			sprite.Position = pos;
			sprite.TextureRect = new IntRect(new Vector2i(), sz);

			//EffectComponent.image = new Image(sprite.Texture.CopyToImage());
			//if (EffectComponent.MasksIn == false) EffectComponent.image.FlipVertically();
			//EffectComponent.rawTextureData = EffectComponent.image.Pixels;
			//EffectComponent.image.FlipVertically();
			//EffectComponent.rawTexture = new Texture(EffectComponent.image);
			//EffectComponent.shader.SetUniform("texture", Shader.CurrentTexture);
			//EffectComponent.shader.SetUniform("raw_texture", EffectComponent.rawTexture);
			//EffectComponent.shader.SetUniform("has_mask", true);
			//EffectComponent.shader.SetUniform("mask_out", false);
			//EffectComponent.shader.SetUniform("mask_red", 1);
			//EffectComponent.shader.SetUniform("mask_green", 0);
			//EffectComponent.shader.SetUniform("mask_blue", 0);

			if (this == WorldCamera) Window.window.Draw(sprite);
			else WorldCamera.rendTexture.Draw(sprite);
		}

		public Camera(Point viewPosition, Size viewSize)
		{
			if (sortedCameras.ContainsKey(0) == false) sortedCameras[0] = new List<Camera>();
			sortedCameras[0].Add(this);

			var pos = new Vector2f((float)viewPosition.X, (float)viewPosition.Y);
			TransformComponent = new()
			{
				Position = new Point(),
				Size = new Size(100, 100)
			};
			view = new View(pos, new Vector2f((float)viewSize.Width, (float)viewSize.Height));
			rendTexture = new RenderTexture((uint)viewSize.Width, (uint)viewSize.Height);
			BackgroundColor = Color.DarkGreen;
			startSize = viewSize;
			Zoom = 1;

			//var effects = (EffectComponent.Type[])Enum.GetValues(typeof(EffectComponent.Type));
			//foreach (var effect in effects) EffectComponent.effects.Add(effect, 0);
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
				var start = new Vector2f((float)line.StartPosition.X, (float)line.StartPosition.Y);
				var end = new Vector2f((float)line.EndPosition.X, (float)line.EndPosition.Y);
				var startColor = new SFML.Graphics.Color(
					(byte)line.StartPosition.Color.Red,
					(byte)line.StartPosition.Color.Green,
					(byte)line.StartPosition.Color.Blue,
					(byte)line.StartPosition.Color.Alpha);
				var endColor = new SFML.Graphics.Color(
					(byte)line.EndPosition.Color.Red,
					(byte)line.EndPosition.Color.Green,
					(byte)line.EndPosition.Color.Blue,
					(byte)line.EndPosition.Color.Alpha);

				var vert = new Vertex[]
				{
					new(start, startColor),
					new(end, endColor)
				};
				rendTexture.Draw(vert, PrimitiveType.Lines);
			}
		}
		public void DrawPoints(params Point[] points)
		{
			foreach (var p in points)
			{
				var vert = new Vertex[]
				{
					new(new Vector2f(
						(float)p.X, (float)p.Y),
						new SFML.Graphics.Color((byte)p.Color.Red, (byte)p.Color.Green, (byte)p.Color.Blue, (byte)p.Color.Alpha))
				};
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
					vert[i] = new Vertex(
						new Vector2f((float)p.X, (float)p.Y),
						new SFML.Graphics.Color((byte)p.Color.Red, (byte)p.Color.Green, (byte)p.Color.Blue, (byte)p.Color.Alpha));
				}

				rendTexture.Draw(vert, PrimitiveType.Quads);
			}
		}
	}
}
