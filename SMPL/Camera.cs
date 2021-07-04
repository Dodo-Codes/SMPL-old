using SFML.Graphics;
using SFML.System;
using System.Collections.Generic;

namespace SMPL
{
	public class Camera
	{
		public static Camera WorldCamera { get; internal set; }
		public ViewComponent ViewComponent { get; internal set; }
		public IdentityComponent<Camera> IdentityComponent { get; internal set; }
		public TransformComponent TransformComponent { get; internal set; }

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

		internal static void DrawCameras()
		{
			WorldCamera.ViewComponent.StartDraw();
			WorldCameraEvents.instance.OnDraw();
			foreach (var kvp in sortedCameras)
			{
				foreach (var camera in kvp.Value)
				{
					if (camera == WorldCamera) continue;
					camera.ViewComponent.StartDraw();
					camera.OnDraw();
					camera.ViewComponent.EndDraw();
				}
			}
			WorldCamera.ViewComponent.EndDraw();
		}
		public virtual void OnDraw() { }

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
			ViewComponent = new()
			{
				transform = TransformComponent,
				view = new View(pos, new Vector2f((float)viewSize.Width, (float)viewSize.Height)),
				rendTexture = new RenderTexture((uint)viewSize.Width, (uint)viewSize.Height),
				BackgroundColor = Color.DarkGreen,
				maxSize = viewSize,
				Zoom = 1,
			};
			IdentityComponent = new();
		}
		public bool Snap(string filePath = "folder/picture.png")
		{
			var img = ViewComponent.rendTexture.Texture.CopyToImage();
			var full = File.CreateDirectoryForFile(filePath);

			if (img.SaveToFile(filePath)) img.Dispose();
			else
			{
				Debug.LogError(1, $"Could not save picture '{full}'.");
				return false;
			}
			return true;
		}

		public void DrawLines(params Line[] lines) => ViewComponent.DrawLines(lines);
	}
}
