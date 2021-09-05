using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using SMPL.Data;
using SMPL.Gear;

namespace SMPL.Components
{
	public class Camera : Component
	{
		private static event Events.ParamsOne<Camera> OnDisplay;
		private double depth;
		private Data.Color bgColor;

		// =============

		internal static SortedDictionary<double, List<Camera>> sortedCameras = new();
		internal View view;
		internal SFML.Graphics.Sprite sprite = new();
		internal RenderTexture rendTexture;
		internal Size startSize;
		internal Area display2D;

		internal static void DrawCameras()
		{
			WorldCamera.StartDraw();
			OnDisplay?.Invoke(WorldCamera);
			foreach (var kvpp in sortedCameras)
				for (int j = 0; j < kvpp.Value.Count; j++)
				{
					if (kvpp.Value[j] == WorldCamera) continue;
					kvpp.Value[j].StartDraw();
					OnDisplay?.Invoke(kvpp.Value[j]);
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

		// ============

		public static class CallWhen
		{
			public static void Display(Action<Camera> method, uint order = uint.MaxValue) =>
				OnDisplay = Events.Add(OnDisplay, method, order);
		}
		public static Camera WorldCamera { get; internal set; }

		public Area DisplayArea
		{
			get { return ErrorIfDestroyed() ? default : display2D; }
			set { if (ErrorIfDestroyed() == false && this != WorldCamera) display2D = value; }
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
			get { return ErrorIfDestroyed() ? Point.Invalid : new() { X = view.Center.X, Y = view.Center.Y }; }
			set { if (ErrorIfDestroyed() == false && this != WorldCamera) view.Center = Point.From(value); }
		}
		public double Angle
		{
			get { return ErrorIfDestroyed() ? double.NaN : view.Rotation; }
			set { if (ErrorIfDestroyed() == false && this != WorldCamera) view.Rotation = (float)value; }
		}
		public Size Size
		{
			get { return ErrorIfDestroyed() ? Size.Invalid : new Size(view.Size.X, view.Size.Y); }
			set { if (ErrorIfDestroyed() == false && this != WorldCamera) view.Size = Size.From(value); }
		}
		public Data.Color BackgroundColor
		{
			get { return ErrorIfDestroyed() ? Data.Color.Invalid : bgColor; }
			set { if (ErrorIfDestroyed() == false && this != WorldCamera) bgColor = value; }
		}

		public Camera(string uniqueID, Point viewPosition, Size viewSize) : base(uniqueID)
		{
			if (sortedCameras.ContainsKey(0) == false) sortedCameras[0] = new List<Camera>();
			sortedCameras[0].Add(this);

			var pos = Point.From(viewPosition);
			view = new View(pos, Size.From(viewSize));
			rendTexture = new RenderTexture((uint)viewSize.W, (uint)viewSize.H);
			BackgroundColor = Data.Color.Black;
			startSize = viewSize;
			if (cannotCreate) { ErrorAlreadyHasUID(uniqueID); Destroy(); }
		}
		public override void Destroy()
		{
			if (ErrorIfDestroyed() || this == WorldCamera) return;
			sortedCameras[depth].Remove(this);
			view.Dispose();
			view = null;
			sprite.Dispose();
			sprite = null;
			rendTexture.Dispose();
			rendTexture = null;
			DisplayArea = null;
			base.Destroy();
		}

		public void Snap(string filePath = "folder/picture.png")
		{
			if (ErrorIfDestroyed()) return;
			var img = rendTexture.Texture.CopyToImage();
			var full = File.CreateDirectoryForFile(filePath);

			if (img.SaveToFile(filePath)) img.Dispose();
			else { Debug.LogError(1, $"Could not save picture '{full}'."); return; }
		}
	}
}
