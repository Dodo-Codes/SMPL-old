using SFML.Graphics;
using SFML.System;

namespace SMPL
{
	public class Camera
	{
		public static Camera WorldCamera { get; internal set; }

		internal View view;
		internal Sprite transform;
		internal RenderTexture rendTexture;

		public Point Position
		{
			get { return new Point(transform.Position.X, transform.Position.Y); }
			set { transform.Position = new Vector2f((float)value.X, (float)value.Y); UpdateIfWorldCamera(); }
		}
		public double Angle
		{
			get { return transform.Rotation; }
			set { transform.Rotation = (float)value; UpdateIfWorldCamera(); }
		}
		private double zoom;
		public double Zoom
		{
			get { return zoom; }
			set { zoom = Number.Limit(value, new Bounds(0.001, 500)); UpdateIfWorldCamera(); }
		}

		public Camera()
		{
			Zoom = 1;
			view = new View();
			transform = new Sprite();
		}

		public bool TakePicture(string filePath = "folder/picture.png")
		{
			var rendTexture = new RenderTexture((uint)view.Size.X, (uint)view.Size.Y);
			rendTexture.SetView(view);
			rendTexture.Display();

			var img = rendTexture.Texture.CopyToImage();
			var full = File.CreateDirectoryForFile(filePath);

			if (img.SaveToFile(filePath))
			{
				img.Dispose();
				rendTexture.Dispose();
			}
			else
			{
				Debug.LogError(1, $"Could not save screenshot file '{full}'.");
				return false;
			}
			return true;
		}
		public void UpdateIfWorldCamera()
		{
			if (WorldCamera != this) return;

			Window.window.GetView().Center = transform.Position;
			Window.window.GetView().Rotation = transform.Rotation;
			var x = Window.window.Size.X / zoom;
			var y = Window.window.Size.Y / zoom;
			view.Size = new Vector2f((float)x, (float)y);
			Window.window.SetView(view);
			Window.ForceDraw = true;
		}
	}
}
