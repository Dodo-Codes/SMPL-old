using SFML.Graphics;
using SFML.System;

namespace SMPL
{
	public class ViewComponent
	{
		internal View view;
		internal Sprite sprite = new();
		internal RenderTexture rendTexture;
		internal Size maxSize;
		internal TransformComponent transform;

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

		internal void StartDraw()
		{
			rendTexture.SetView(view);
			rendTexture.Clear(new SFML.Graphics.Color(
				(byte)BackgroundColor.Red, (byte)BackgroundColor.Green, (byte)BackgroundColor.Blue, (byte)BackgroundColor.Alpha));
		}
		internal void EndDraw()
		{
			rendTexture.Display();
			var ms = maxSize / 2;
			var s = transform.Size / 2;
			var tpos = new Vector2i(
				(int)(ms.Width - s.Width * 2),
				(int)(ms.Height - s.Height * 2));
			var pos = new Vector2f(
				(float)(transform.Position.X - transform.Size.Width),
				(float)(transform.Position.Y - transform.Size.Height));
			var sz = new Vector2i(
				(int)transform.Size.Width,
				(int)transform.Size.Height) * 2;
			var tsz = rendTexture.Texture.Size;
			var sc = new Vector2f((float)tsz.X / (float)sz.X, (float)tsz.Y / (float)sz.Y);
			var or = new Vector2f(rendTexture.Size.X / 2, rendTexture.Size.Y / 2);

			//sprite.Origin = or;
			sprite.Texture = rendTexture.Texture;
			sprite.Rotation = (float)transform.Angle;
			sprite.Position = pos;
			sprite.TextureRect = new IntRect(tpos, sz);

			if (this == Camera.WorldCamera.ViewComponent) Window.window.Draw(sprite);
			else
			{
				sprite.Scale = sc;
				Camera.WorldCamera.ViewComponent.rendTexture.Draw(sprite);
			}
		}

		internal void DrawLines(params Line[] lines)
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
	}
}
