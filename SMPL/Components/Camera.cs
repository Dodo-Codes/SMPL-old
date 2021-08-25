using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using SMPL.Data;
using SMPL.Gear;

namespace SMPL.Components
{
	public class Camera : Access
	{
		internal static SortedDictionary<double, List<Camera>> sortedCameras = new();
		internal View view;
		internal SFML.Graphics.Sprite sprite = new();
		internal RenderTexture rendTexture;
		internal Size startSize;

		private static event Events.ParamsOne<Camera> OnDisplay;
		private static event Events.ParamsTwo<Camera, string> OnSnap;
		private static event Events.ParamsTwo<Camera, Area> OnDisplay2DChanged;
		private static event Events.ParamsTwo<Camera, Identity<Camera>> OnIdentityChange;

		public static class CallWhen
		{
			public static void Display(Action<Camera> method, uint order = uint.MaxValue) =>
			OnDisplay = Events.Add(OnDisplay, method, order);
			public static void Display2DChanged(Action<Camera, Area> method, uint order = uint.MaxValue) =>
				OnDisplay2DChanged = Events.Add(OnDisplay2DChanged, method, order);
			public static void Snap(Action<Camera, string> method, uint order = uint.MaxValue) =>
				OnSnap = Events.Add(OnSnap, method, order);
			public static void IdentityChange(Action<Camera, Identity<Camera>> method,
				uint order = uint.MaxValue) => OnIdentityChange = Events.Add(OnIdentityChange, method, order);
		}

		public static Camera WorldCamera { get; internal set; }

		private Area display2D;
		public Area Display2D
		{
			get { return display2D; }
			set
			{
				if (display2D == value || (Debug.CalledBySMPL == false && IsCurrentlyAccessible() == false)) return;
				var prev = display2D;
				display2D = value;
				if (Debug.CalledBySMPL == false) OnDisplay2DChanged?.Invoke(this, prev);
			}
		}

		private Identity<Camera> identity;
		public Identity<Camera> Identity
		{
			get { return identity; }
			set
			{
				if (identity == value || (Debug.CalledBySMPL == false && IsCurrentlyAccessible() == false)) return;
				var prev = identity;
				identity = value;
				if (Debug.CalledBySMPL == false) OnIdentityChange?.Invoke(this, prev);
			}
		}

		private double depth;
		public double Depth
		{
			get { return depth; }
			set
			{
				if (depth == value || (Debug.CalledBySMPL == false && IsCurrentlyAccessible() == false)) return;
				var oldDepth = depth;
				depth = value;
				sortedCameras[oldDepth].Remove(this);
				if (sortedCameras.ContainsKey(depth) == false) sortedCameras[depth] = new List<Camera>();
				sortedCameras[depth].Add(this);
			}
		}

		public Point Position
		{
			get { return new() { X = view.Center.X, Y = view.Center.Y }; }
			set
			{
				if (Position == value || (this != WorldCamera &&
					Debug.CalledBySMPL == false && IsCurrentlyAccessible() == false)) return;
				view.Center = Point.From(value);
			}
		}
		public double Angle
		{
			get { return view.Rotation; }
			set
			{
				if (Angle == value || (this != WorldCamera &&
					Debug.CalledBySMPL == false && IsCurrentlyAccessible() == false)) return;
				view.Rotation = (float)value;
			}
		}
		public Size Size
		{
			get { return new Size(view.Size.X, view.Size.Y); }
			set
			{
				if (Size == value || (this != WorldCamera &&
					Debug.CalledBySMPL == false && IsCurrentlyAccessible() == false)) return;
				view.Size = Size.From(value);
			}
		}
		private Data.Color bgColor;
		public Data.Color BackgroundColor
		{
			get { return bgColor; }
			set
			{
				if (bgColor == value || (this != WorldCamera &&
					Debug.CalledBySMPL == false && IsCurrentlyAccessible() == false)) return;
				bgColor = value;
			}
		}

		public Camera(Point viewPosition, Size viewSize)
		{
			if (sortedCameras.ContainsKey(0) == false) sortedCameras[0] = new List<Camera>();
			sortedCameras[0].Add(this);

			var pos = Point.From(viewPosition);
			Display2D = new();
			view = new View(pos, Size.From(viewSize));
			rendTexture = new RenderTexture((uint)viewSize.W, (uint)viewSize.H);
			BackgroundColor = Data.Color.Black;
			startSize = viewSize;
			//Zoom = 1;
		}

		internal static void DrawCameras()
		{
			WorldCamera.StartDraw();
			OnDisplay?.Invoke(WorldCamera);
			foreach (var kvpp in sortedCameras)
			{
				for (int j = 0; j < kvpp.Value.Count; j++)
				{
					if (kvpp.Value[j] == WorldCamera) continue;
					kvpp.Value[j].StartDraw();
					OnDisplay?.Invoke(kvpp.Value[j]);
				}
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
			var pos = Point.From(Display2D.Position);
			var sz = new Vector2i((int)rendTexture.Size.X, (int)rendTexture.Size.Y);
			//var s = new Vector2i((int)view.Size.X, (int)view.Size.Y);
			var tsz = rendTexture.Size;
			var sc = new Vector2f(
				(float)Display2D.Size.W / (float)tsz.X,
				(float)Display2D.Size.H / (float)tsz.Y);
			var or = new Vector2f(rendTexture.Size.X / 2, rendTexture.Size.Y / 2);

			sprite.Origin = or;
			sprite.Texture = rendTexture.Texture;
			sprite.Rotation = (float)Display2D.Angle;
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

		public void Snap(string filePath = "folder/picture.png")
		{
			if (Debug.CalledBySMPL == false && IsCurrentlyAccessible() == false && this != WorldCamera) return;
			var img = rendTexture.Texture.CopyToImage();
			var full = File.CreateDirectoryForFile(filePath);

			if (img.SaveToFile(filePath)) img.Dispose();
			else { Debug.LogError(1, $"Could not save picture '{full}'."); return; }
			if (Debug.CalledBySMPL == false) OnSnap?.Invoke(this, filePath);
		}
   }
}
