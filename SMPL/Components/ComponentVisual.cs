using SFML.Graphics;
using static SMPL.Events;

namespace SMPL
{
	public abstract class ComponentVisual : ComponentAccess
	{
		protected readonly uint creationFrame;
		protected readonly double rand;
		internal Component2D transform;
		internal ComponentVisual masking;

		private Effects effects;
		public Effects Effects
		{
			get { return effects; }
			set
			{
				if (effects == value || (Debug.currentMethodIsCalledByUser && IsCurrentlyAccessible() == false)) return;
				effects = value;
			}
		}
		private ComponentFamily family;
		public ComponentFamily Family
		{
			get { return family; }
			set
			{
				if (family == value || (Debug.currentMethodIsCalledByUser && IsCurrentlyAccessible() == false)) return;
				family = value;
			}
		}

		private bool isHidden;
		public bool IsHidden
		{
			get { return isHidden; }
			set
			{
				if (isHidden == value || (Debug.currentMethodIsCalledByUser && IsCurrentlyAccessible() == false)) return;
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
			creationFrame = Performance.FrameCount;
			rand = Number.Random(new Bounds(-9999, 9999), 5);
			transform = component2D;
			Family = new(this);
			transform.family = Family;
			Effects = new(this);
		}
		//public abstract void DrawBounds(Camera camera, float thickness, Color color);
	}
}
