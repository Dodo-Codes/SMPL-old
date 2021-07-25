using SFML.Graphics;
using SFML.System;
using System.Collections.Generic;
using static SMPL.Events;

namespace SMPL
{
	public class ComponentSprite
	{
		private readonly uint creationFrame;
		private readonly double rand;
		internal Sprite sprite = new();

		internal Component2D transform;
		internal Image image;
		internal Texture rawTexture;
		internal byte[] rawTextureData;

		public Effects Effects { get; set; }

		internal ComponentSprite maskingSprite;
		internal ComponentText maskingText;
		private bool isHidden;
		public bool IsHidden
		{
			get { return isHidden; }
			set
			{
				if (isHidden == value) return;
				isHidden = value;

				var n = D(instances); foreach (var kvp in n) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++) e[i].OnSpriteVisibilityChangeSetup(this); }
				var n1 = D(instances); foreach (var kvp in n1) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++) e[i].OnSpriteVisibilityChange(this); }
			}
		}
		public bool IsRepeated
		{
			get { return sprite.Texture.Repeated; }
			set
			{
				if (sprite.Texture.Repeated == value) return;
				sprite.Texture.Repeated = value;

				var n = D(instances); foreach (var kvp in n) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++) e[i].OnSpriteRepeatingChangeSetup(this); }
				var n1 = D(instances); foreach (var kvp in n1) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++) e[i].OnSpriteRepeatingChange(this); }
			}
		}
		public bool IsSmooth
		{
			get { return sprite.Texture.Smooth; }
			set
			{
				if (sprite.Texture.Smooth == value) return;
				sprite.Texture.Smooth = value;

				var n = D(instances); foreach (var kvp in n) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++) e[i].OnSpriteSmoothingChangeSetup(this); }
				var n1 = D(instances); foreach (var kvp in n1) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++) e[i].OnSpriteSmoothingChange(this); }
			}
		}
		private string path;
		public string TexturePath
		{
			get { return path; }
			private set
			{
				var texture = File.textures[value];
				sprite.Texture = texture;

				image = new Image(sprite.Texture.CopyToImage());
				//image.FlipVertically();
				rawTextureData = image.Pixels;
				rawTexture = new Texture(image);
				Effects.shader.SetUniform("texture", sprite.Texture);
				path = value;
			}
		}
		private Point offsetPercent, lastFrameOffPer;
		public Point OffsetPercent
		{
			get { return offsetPercent; }
			set
			{
				if (offsetPercent == value) return;
				var delta = value - offsetPercent;
				offsetPercent = value;
				var rect = sprite.TextureRect;
				var sz = sprite.Texture.Size;
				sprite.TextureRect = new IntRect(
					(int)(sz.X * (offsetPercent.X / 100)), (int)(sz.Y * (offsetPercent.Y / 100)),
					rect.Width, rect.Height);

				var n = D(instances); foreach (var kvp in n) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++) e[i].OnSpriteOffsetSetup(this, delta); }
				var n1 = D(instances); foreach (var kvp in n1) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++) e[i].OnSpriteOffset(this, delta); }
			}
		}
		private Size sizePercent, lastFrameSzPer;
		public Size SizePercent
		{
			get { return sizePercent; }
			set
			{
				if (sizePercent == value) return;
				var delta = value - sizePercent;
				sizePercent = value;
				value /= 100;

				var sz = sprite.Texture.Size;
				var textRect = sprite.TextureRect;
				sprite.TextureRect = new IntRect(textRect.Left, textRect.Top, (int)(sz.X * value.W), (int)(sz.Y * value.H));

				var n = D(instances); foreach (var kvp in n) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++) e[i].OnSpriteResizeSetup(this, delta); }
				var n1 = D(instances); foreach (var kvp in n1) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++) e[i].OnSpriteResize(this, delta); }
			}
		}
		private Size gridSize;
		public Size GridSize
		{
			get { return gridSize; }
			set
			{
				if (gridSize == value) return;
				var delta = value - gridSize;
				gridSize = value;
				transform.OriginPercent = transform.OriginPercent;

				var n = D(instances); foreach (var kvp in n) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++) e[i].OnSpriteGridResizeSetup(this, delta); }
				var n1 = D(instances); foreach (var kvp in n1) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++) e[i].OnSpriteGridResize(this, delta); }
			}
		}

		internal void Update()
      {
			if (Gate.EnterOnceWhile($"{creationFrame}-{rand}-sprite-off-start", lastFrameOffPer != offsetPercent))
			{
				var n = D(instances); foreach (var kvp in n) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++) e[i].OnSpriteOffsetStartSetup(this, offsetPercent - lastFrameOffPer); }
				var n1 = D(instances); foreach (var kvp in n1) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++) e[i].OnSpriteOffsetStart(this, offsetPercent - lastFrameOffPer); }
			}
			if (Gate.EnterOnceWhile($"{creationFrame}-{rand}-sprite-off-end", lastFrameOffPer == offsetPercent))
			{
				if (creationFrame + 1 != Time.frameCount)
				{
					var n = D(instances); foreach (var kvp in n) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++) e[i].OnSpriteOffsetEndSetup(this); }
					var n1 = D(instances); foreach (var kvp in n1) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++) e[i].OnSpriteOffsetEnd(this); }
				}
			}
			//=============================
			if (Gate.EnterOnceWhile($"{creationFrame}-{rand}-sprite-size-start", lastFrameSzPer != sizePercent))
			{
				var n = D(instances); foreach (var kvp in n) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++) e[i].OnSpriteResizeStartSetup(this, sizePercent - lastFrameSzPer); }
				var n1 = D(instances); foreach (var kvp in n1) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++) e[i].OnSpriteResizeStart(this, sizePercent - lastFrameSzPer); }
			}
			if (Gate.EnterOnceWhile($"{creationFrame}-{rand}-sprite-size-end", lastFrameSzPer == sizePercent))
			{
				if (creationFrame + 1 != Time.frameCount)
				{
					var n = D(instances); foreach (var kvp in n) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++) e[i].OnSpriteResizeEndSetup(this); }
					var n1 = D(instances); foreach (var kvp in n1) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++) e[i].OnSpriteResizeEnd(this); }
				}
			}
			//=============================
			lastFrameOffPer = offsetPercent;
			lastFrameSzPer = sizePercent;
		}

		public ComponentSprite(Component2D component2D, string texturePath = "folder/texture.png")
		{
			if (File.textures.ContainsKey(texturePath) == false)
			{
				Debug.LogError(1, $"The texture at '{texturePath}' is not loaded.\n" +
					$"Use '{nameof(File)}.{nameof(File.LoadAsset)} ({nameof(File)}.{nameof(File.Asset)}." +
					$"{nameof(File.Asset.Texture)}, \"{texturePath}\")' to load it.");
				return;
			}
			sprites.Add(this);
			transform = component2D;
			creationFrame = Time.FrameCount;
			rand = Number.Random(new Bounds(-9999, 9999), 5);
			Effects = new(this);
			TexturePath = texturePath;
			sprite.Texture.Repeated = true;
			sizePercent = new Size(100, 100);
			lastFrameSzPer = sizePercent;

			var n = D(instances); foreach (var kvp in n) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++) e[i].OnSpriteCreateSetup(this); }
			var n1 = D(instances); foreach (var kvp in n1) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++) e[i].OnSpriteCreate(this); }
		}

		public void Draw(Camera camera)
		{
			var isMask = maskingSprite != null || maskingText != null;
			if (Window.DrawNotAllowed() || isMask || IsHidden || sprite == null ||
				sprite.Texture == null || transform == null) return;

			var w = sprite.TextureRect.Width;
			var h = sprite.TextureRect.Height;
			var p = transform.OriginPercent / 100;
			var x = w * (float)p.X * ((float)GridSize.W / 2f) + (w * (float)p.X / 2f);
			var y = h * (float)p.Y * ((float)GridSize.H / 2f) + (h * (float)p.Y / 2f);

			sprite.Origin = new Vector2f(x, y);
			sprite.Position = Point.From(transform.Position);
			sprite.Rotation = (float)transform.Angle;
			sprite.Scale = new Vector2f(
				(float)transform.Size.W / rawTexture.Size.X,
				(float)transform.Size.H / rawTexture.Size.Y);

			var drawMaskResult = Effects.DrawMasks(rawTexture);
			Effects.shader.SetUniform("raw_texture", drawMaskResult.Texture);
			sprite.Texture = drawMaskResult.Texture;

			var pos = sprite.Position;
			for (int j = 0; j < GridSize.H + 1; j++)
			{
				for (int i = 0; i < GridSize.W + 1; i++)
				{
					var p1 = sprite.Transform.TransformPoint(new Vector2f((pos.X + w) * i, (pos.Y + h) * j));

					sprite.Position = p1;
					camera.rendTexture.Draw(sprite, new RenderStates(Effects.shader));
					sprite.Position = pos;
				}
			}
			drawMaskResult.Dispose();
		}
		public void DrawBounds(Camera camera, float thickness, Color color)
		{
			var b = sprite.GetGlobalBounds();
			var c = Color.From(color);
			var left = new Vertex[]
			{
				new Vertex(new Vector2f(b.Left - thickness, b.Top - thickness), c),
				new Vertex(new Vector2f(b.Left + thickness, b.Top - thickness), c),
				new Vertex(new Vector2f(b.Left + thickness, b.Top + thickness + b.Height), c),
				new Vertex(new Vector2f(b.Left - thickness, b.Top + thickness + b.Height), c),
			};
			camera.rendTexture.Draw(left, PrimitiveType.Quads);
		}
	}
}
