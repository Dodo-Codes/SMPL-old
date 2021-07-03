using SFML.Graphics;
using SFML.System;
using System.Collections.Generic;

namespace SMPL
{
	public class Camera : Identifiable<Camera>
	{
		public static Camera WorldCamera { get; internal set; }

		internal View view;
		internal RenderTexture rendTexture;
		internal Size maxSize;

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
		public Color BackgroundColor { get; set; }

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
			if (WorldCamera == this)
			{
				var ms = maxSize / 2;
				var Size = view.Size / 2;
				var s = Size / 2;
				var tpos = new Vector2i((int)(ms.Width - s.X * 2), (int)(ms.Height - s.Y * 2));
				var sz = new Vector2i((int)Size.X, (int)Size.Y) * 2;
				var sprite = new Sprite(rendTexture.Texture)
				{
					Position = -Size,
					TextureRect = new IntRect(tpos, sz)
				};
				Window.window.Draw(sprite);
				sprite.Dispose();
			}
		}

		public Camera(Point position, Size size)
		{
			Window.DrawEvent += new Window.DrawHandler(DrawCycle);

			var pos = new Vector2f((float)position.X, (float)position.Y);
			view = new View(pos, new Vector2f((float)size.Width, (float)size.Height));
			rendTexture = new RenderTexture((uint)size.Width, (uint)size.Height);
			maxSize = size;
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
