using SFML.Graphics;
using SFML.System;
using System.Collections.Generic;

namespace SMPL
{
	public class Camera
	{
		public static Camera WorldCamera { get; internal set; }

		internal View view;
		internal RenderTexture rendTexture;
		internal Size maxSize;
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
		public Point ViewPosition
		{
			get { return new Point(view.Center.X, view.Center.Y); }
			set { view.Center = new Vector2f((float)value.X, (float)value.Y); }
		}
		public double ViewAngle
		{
			get { return view.Rotation; }
			set { view.Rotation = (float)value; }
		}
		private double zoom;
		public double Zoom
		{
			get { return zoom; }
			set
			{
				zoom = Number.Limit(value, new Bounds(0.001, 500));
				view.Size = new Vector2f((float)(maxSize.Width / zoom), (float)(maxSize.Height / zoom));
			}
		}
		private Size size;
		public Size Size
		{
			get { return size; }
			set
			{
				if (WorldCamera == this) return;
				size = value;
			}
		}
		private Point position;
		public Point Position
		{
			get { return position; }
			set
			{
				if (WorldCamera == this) return;
				position = value;
			}
		}
		private double angle;
		public double Angle
		{
			get { return angle; }
			set
			{
				if (WorldCamera == this) return;
				angle = value;
			}
		}
		public Color BackgroundColor { get; set; }

		internal static void DrawCameras()
		{
			WorldCamera.DrawCycle();
			foreach (var kvp in sortedCameras)
			{
				var depth = kvp.Key;
				var cameras = kvp.Value;
				foreach (var camera in cameras)
				{
					if (camera == WorldCamera) continue;
					camera.DrawCycle();
				}
			}
			WorldCamera.EndDraw();
		}
		internal void DrawCycle()
		{
			rendTexture.SetView(view);
			rendTexture.Clear(new SFML.Graphics.Color(
				(byte)BackgroundColor.Red, (byte)BackgroundColor.Green, (byte)BackgroundColor.Blue, (byte)BackgroundColor.Alpha));
			if (WorldCamera == this) WorldCameraEvents.instance.OnDraw();
			else
			{
				OnDraw();
				EndDraw();
			}
		}
		internal void EndDraw()
		{
			rendTexture.Display();
			var ms = maxSize / 2;
			var s = Size / 2;
			var tpos = new Vector2i((int)(ms.Width - s.Width * 2), (int)(ms.Height - s.Height * 2));
			var pos = new Vector2f((float)(Position.X - Size.Width), (float)(Position.Y - Size.Height));
			var sz = new Vector2i((int)Size.Width, (int)Size.Height) * 2;
			var sprite = new Sprite(rendTexture.Texture)
			{
				Rotation = (float)Angle,
				Position = pos,
				TextureRect = new IntRect(tpos, sz),
			};
			if (WorldCamera == this)
			{
				rendTexture.Display();
				Window.window.Draw(sprite);
			}
			else WorldCamera.rendTexture.Draw(sprite);
			sprite.Dispose();
		}

		public Camera(Point position, Size size, Point viewPosition, Size viewSize)
		{
			if (sortedCameras.ContainsKey(0) == false) sortedCameras[0] = new List<Camera>();
			sortedCameras[0].Add(this);

			var pos = new Vector2f((float)viewPosition.X, (float)viewPosition.Y);
			view = new View(pos, new Vector2f((float)viewSize.Width, (float)viewSize.Height));
			rendTexture = new RenderTexture((uint)viewSize.Width, (uint)viewSize.Height);
			maxSize = viewSize;
			Position = position;
			Size = size;
			Zoom = 1;
			BackgroundColor = Color.DarkGreen;
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
					(byte)line.StartColor.Red, (byte)line.StartColor.Green, (byte)line.StartColor.Blue, (byte)line.StartColor.Alpha);
				var endColor = new SFML.Graphics.Color(
					(byte)line.EndColor.Red, (byte)line.EndColor.Green, (byte)line.EndColor.Blue, (byte)line.EndColor.Alpha);

				var vert = new Vertex[]
				{
					new(start, startColor),
					new(end, endColor)
				};
				rendTexture.Draw(vert, PrimitiveType.Lines);
			}
		}
		public virtual void OnDraw() { }
	}
}
