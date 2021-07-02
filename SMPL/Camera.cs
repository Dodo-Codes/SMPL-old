using SFML.Graphics;
using SFML.System;
using System.Collections.Generic;

namespace SMPL
{
	public class Camera
	{
		public static Camera WorldCamera { get; internal set; }

		internal View view;
		internal Size startingSize;
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
				sortedCameras[depth].Add(this);
			}
		}
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
				view.Size = new Vector2f((float)(startingSize.Width / zoom), (float)(startingSize.Height / zoom));
			}
		}

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
		}
		internal void DrawCycle()
		{
			var rendTexture = new RenderTexture((uint)view.Size.X, (uint)view.Size.Y);
			rendTexture.SetView(view);

			Window.window.SetView(view);
			rendTexture.Clear(new SFML.Graphics.Color(255, 0, 0));
			OnDraw();
			EndDraw(rendTexture);
		}
		internal void EndDraw(RenderTexture rendTexture)
		{
			rendTexture.Display();
			var sprite = new Sprite(rendTexture.Texture)
			{
				Rotation = (float)Angle,
				Position = new Vector2f((float)(Position.X - view.Size.X / 2), (float)(Position.Y - view.Size.Y / 2)),
			};
			Window.window.SetView(WorldCamera.view);
			Window.window.Draw(sprite);
			sprite.Dispose();
		}

		public Camera(Point position, Size viewSize)
		{
			if (sortedCameras.ContainsKey(0) == false) sortedCameras[0] = new List<Camera>();
			sortedCameras[0].Add(this);

			var pos = new Vector2f((float)position.X, (float)position.Y);
			view = new View(pos, new Vector2f((float)viewSize.Width, (float)viewSize.Height));
			startingSize = viewSize;
			Zoom = 1;
		}
		public bool TakePicture(string filePath = "folder/picture.png")
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
