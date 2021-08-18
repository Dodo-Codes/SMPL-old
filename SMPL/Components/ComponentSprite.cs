using SFML.Graphics;
using SFML.System;
using System.Collections.Generic;
using static SMPL.Events;

namespace SMPL
{
	public class ComponentSprite : ComponentVisual
	{
		internal static List<ComponentSprite> sprites = new();

		internal Image image;
		internal Texture rawTexture;
		internal Texture rawTextureShader;
		internal byte[] rawTextureData;

		public bool IsRepeated
		{
			get { return transform.sprite.Texture.Repeated; }
			set
			{
				if (transform.sprite.Texture.Repeated == value ||
					(Debug.CurrentMethodIsCalledByUser && IsCurrentlyAccessible() == false)) return;
				transform.sprite.Texture.Repeated = value;

				var n = D(instances); foreach (var kvp in n) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++) e[i].OnSpriteRepeatingChangeSetup(this); }
				var n1 = D(instances); foreach (var kvp in n1) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++) e[i].OnSpriteRepeatingChange(this); }
			}
		}
		public bool IsSmooth
		{
			get { return transform.sprite.Texture.Smooth; }
			set
			{
				if (transform.sprite.Texture.Smooth == value ||
					(Debug.CurrentMethodIsCalledByUser && IsCurrentlyAccessible() == false)) return;
				transform.sprite.Texture.Smooth = value;

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
				// validated in constructor
				var texture = File.textures[value];
				transform.sprite.Texture = texture;

				image = new Image(transform.sprite.Texture.CopyToImage());
				rawTextureData = image.Pixels;
				rawTexture = new Texture(image);
				image.FlipVertically();
				rawTextureShader = new Texture(image);
				path = value;
			}
		}
		private Point offsetPercent, lastFrameOffPer;
		public Point OffsetPercent
		{
			get { return offsetPercent; }
			set
			{
				if (offsetPercent == value || (Debug.CurrentMethodIsCalledByUser && IsCurrentlyAccessible() == false)) return;
				var delta = value - offsetPercent;
				offsetPercent = value;
				var rect = transform.sprite.TextureRect;
				var sz = transform.sprite.Texture.Size;
				transform.sprite.TextureRect = new IntRect(
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
				if (sizePercent == value || (Debug.CurrentMethodIsCalledByUser && IsCurrentlyAccessible() == false)) return;
				var delta = value - sizePercent;
				sizePercent = value;
				value /= 100;

				var sz = transform.sprite.Texture.Size;
				var textRect = transform.sprite.TextureRect;
				transform.sprite.TextureRect = new IntRect(textRect.Left, textRect.Top, (int)(sz.X * value.W), (int)(sz.Y * value.H));

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
				if (gridSize == value || (Debug.CurrentMethodIsCalledByUser && IsCurrentlyAccessible() == false)) return;
				var delta = value - gridSize;
				gridSize = value;
				transform.OriginPercent = transform.OriginPercent;

				var n = D(instances); foreach (var kvp in n) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++) e[i].OnSpriteGridResizeSetup(this, delta); }
				var n1 = D(instances); foreach (var kvp in n1) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++) e[i].OnSpriteGridResize(this, delta); }
			}
		}

		public ComponentSprite(Component2D component2D, string texturePath = "folder/texture.png")
			: base(component2D)
		{
			// fixing the access since the ComponentAccess' constructor depth leads to here => user has no access but this file has
			// in other words - the depth gets 1 deeper with inheritence ([3]User -> [2]Sprite/Text -> [1]Visual -> [0]Access)
			// and usually it goes as [2]User -> [1]Component -> [0]Access
			GrantAccessToFile(Debug.CurrentFilePath(1)); // grant the user access
			DenyAccessToFile(Debug.CurrentFilePath(0)); // abandon ship
			if (File.textures.ContainsKey(texturePath) == false)
			{
				Debug.LogError(1, $"The texture at '{texturePath}' is not loaded.\n" +
					$"Use '{nameof(File)}.{nameof(File.LoadAsset)} ({nameof(File)}.{nameof(File.Asset)}." +
					$"{nameof(File.Asset.Texture)}, \"{texturePath}\")' to load it.");
				return;
			}
			sprites.Add(this);
			TexturePath = texturePath;
			transform.sprite.Texture.Repeated = true;
			sizePercent = new Size(100, 100);
			lastFrameSzPer = sizePercent;

			var n = D(instances); foreach (var kvp in n) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++) e[i].OnSpriteCreateSetup(this); }
			var n1 = D(instances); foreach (var kvp in n1) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++) e[i].OnSpriteCreate(this); }
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
				if (creationFrame + 1 != Performance.frameCount)
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
				if (creationFrame + 1 != Performance.frameCount)
				{
					var n = D(instances); foreach (var kvp in n) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++) e[i].OnSpriteResizeEndSetup(this); }
					var n1 = D(instances); foreach (var kvp in n1) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++) e[i].OnSpriteResizeEnd(this); }
				}
			}
			//=============================
			lastFrameOffPer = offsetPercent;
			lastFrameSzPer = sizePercent;
		}

		public void Display(Camera camera)
		{
			if (Debug.CurrentMethodIsCalledByUser && IsCurrentlyAccessible() == false) return;
			if (Window.DrawNotAllowed() || masking != null || IsHidden || transform == null || transform.sprite == null ||
				transform.sprite.Texture == null) return;

			transform.sprite.Position = new Vector2f();
			transform.sprite.Rotation = 0;
			transform.sprite.Scale = new Vector2f(1, 1);
			transform.sprite.Origin = new Vector2f(0, 0);

			transform.sprite.Texture = rawTexture;
			var drawMaskResult = Effects.DrawMasks(transform.sprite);
			transform.sprite.Texture = drawMaskResult.Texture;

			var w = transform.sprite.TextureRect.Width;
			var h = transform.sprite.TextureRect.Height;
			var p = transform.OriginPercent / 100;
			var x = w * (float)p.X * ((float)GridSize.W / 2f) + (w * (float)p.X / 2f);
			var y = h * (float)p.Y * ((float)GridSize.H / 2f) + (h * (float)p.Y / 2f);
			transform.sprite.Origin = new Vector2f(x, y);

			transform.sprite.Position = Point.From(transform.Position);
			transform.sprite.Rotation = (float)transform.Angle;
			transform.sprite.Scale = new Vector2f(
				(float)transform.Size.W / rawTexture.Size.X,
				(float)transform.Size.H / rawTexture.Size.Y);

			Effects.shader.SetUniform("Texture", transform.sprite.Texture);
			Effects.shader.SetUniform("RawTexture", rawTextureShader);

			var pos = transform.sprite.Position;
			for (int j = 0; j < GridSize.H + 1; j++)
			{
				for (int i = 0; i < GridSize.W + 1; i++)
				{
					var p1 = transform.sprite.Transform.TransformPoint(new Vector2f((pos.X + w) * i, (pos.Y + h) * j));

					transform.sprite.Position = p1;
					camera.rendTexture.Draw(transform.sprite, new RenderStates(Effects.shader));
					transform.sprite.Position = pos;
				}
			}
			drawMaskResult.Dispose();
		}
	}
}
