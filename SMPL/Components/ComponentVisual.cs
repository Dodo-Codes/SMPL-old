using static SMPL.Events;

namespace SMPL
{
	public abstract class ComponentVisual
	{
		protected readonly uint creationFrame;
		protected readonly double rand;
		internal Component2D transform;
		internal ComponentVisual masking;

		public Effects Effects { get; set; }

		private bool isHidden;
		public bool IsHidden
		{
			get { return isHidden; }
			set
			{
				if (isHidden == value) return;
				isHidden = value;
				if (this is ComponentText)
				{
					var n = D(instances); foreach (var kvp in n) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++)
							e[i].OnTextVisibilityChangeSetup(this as ComponentText); }
					var n1 = D(instances); foreach (var kvp in n1) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++)
							e[i].OnTextVisibilityChange(this as ComponentText); }
				}
				else
				{
					var n = D(instances); foreach (var kvp in n) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++) e[i].OnSpriteVisibilityChangeSetup(this as ComponentSprite); }
					var n1 = D(instances); foreach (var kvp in n1) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++) e[i].OnSpriteVisibilityChange(this as ComponentSprite); }
				}
			}
		}

		public ComponentVisual(Component2D component2D)
		{
			creationFrame = Time.FrameCount;
			rand = Number.Random(new Bounds(-9999, 9999), 5);
			transform = component2D;
			Effects = new(this);
		}

		public abstract void Draw(Camera camera);
	}
}
