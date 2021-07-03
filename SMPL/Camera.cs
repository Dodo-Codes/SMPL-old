using SFML.Graphics;
using SFML.System;
using System.Collections.Generic;

namespace SMPL
{
	public class Camera
	{
		public static Camera WorldCamera { get; internal set; }
		public DrawComponent DrawComponent { get; internal set; }

		public Camera(Point position, Size size)
		{
			Window.DrawEvent += new Window.DrawHandler(OnDraw);

			DrawComponent = new();
			var pos = new Vector2f((float)position.X, (float)position.Y);
			DrawComponent.view = new View(pos, new Vector2f((float)size.Width, (float)size.Height));
			DrawComponent.RendTexture = new RenderTexture((uint)size.Width, (uint)size.Height);
			DrawComponent.startSize = size;
			DrawComponent.BackgroundColor = Color.DarkGreen;
		}

		public bool Snap(string filePath = "folder/picture.png")
		{
			var img = DrawComponent.RendTexture.Texture.CopyToImage();
			var full = File.CreateDirectoryForFile(filePath);

			if (img.SaveToFile(filePath)) img.Dispose();
			else
			{
				Debug.LogError(1, $"Could not save picture '{full}'.");
				return false;
			}
			return true;
		}

		public virtual void OnDraw() { }
		public void DrawLines(params Line[] lines)
		{
			if (DrawComponent.RendTexture == null) return;
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
				DrawComponent.RendTexture.Draw(vert, PrimitiveType.Lines);
			}
		}
	}
}
