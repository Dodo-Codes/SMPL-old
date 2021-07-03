using SFML.Graphics;
using SFML.System;

namespace SMPL
{
	public class DrawComponent
	{
		internal RenderTexture RendTexture { get; set; }
		internal View view;

		public double Depth { get; set; }
		public Color BackgroundColor { get; set; }
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

		internal Size startSize;

		internal void StartDraw()
		{
			if (RendTexture == null)
			{
				RendTexture = new RenderTexture((uint)startSize.Width, (uint)startSize.Height);
			}

			RendTexture.SetView(view);
			RendTexture.Clear(new SFML.Graphics.Color(
				(byte)BackgroundColor.Red,
				(byte)BackgroundColor.Green,
				(byte)BackgroundColor.Blue,
				(byte)BackgroundColor.Alpha));
		}
		internal void EndDraw()
		{
			if (RendTexture == null) return;
			RendTexture.Display();
		}
	}
}
