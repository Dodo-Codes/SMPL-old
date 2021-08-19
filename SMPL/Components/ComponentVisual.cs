using SFML.Graphics;
using System;

namespace SMPL
{
	public abstract class ComponentVisual : ComponentAccess
	{
		protected readonly uint creationFrame;
		protected readonly double rand;
		internal Component2D transform;
		internal ComponentVisual masking;

		private ComponentEffects effects;
		public ComponentEffects Effects
		{
			get { return effects; }
			set
			{
				if (effects == value || (Debug.CurrentMethodIsCalledByUser && IsCurrentlyAccessible() == false)) return;
				var prev = effects;
				effects = value;
				if (Debug.CurrentMethodIsCalledByUser == false) return;
				if (this is ComponentText) ComponentText.TriggerOnEffectsChange(this as ComponentText, prev);
				else ComponentSprite.TriggerOnEffectsChange(this as ComponentSprite, prev);
			}
		}
		private ComponentFamily family;
		public ComponentFamily Family
		{
			get { return family; }
			set
			{
				if (family == value || (Debug.CurrentMethodIsCalledByUser && IsCurrentlyAccessible() == false)) return;
				var prev = family;
				family = value;
				if (Debug.CurrentMethodIsCalledByUser == false) return;
				if (this is ComponentText) ComponentText.TriggerOnFamilyChange(this as ComponentText, prev);
				else ComponentSprite.TriggerOnFamilyChange(this as ComponentSprite, prev);
			}
		}

		private bool isHidden;
		public bool IsHidden
		{
			get { return isHidden; }
			set
			{
				if (isHidden == value || (Debug.CurrentMethodIsCalledByUser && IsCurrentlyAccessible() == false)) return;
				isHidden = value;
				if (this is ComponentText) ComponentText.TriggerOnVisibilityChange(this as ComponentText);
				else ComponentSprite.TriggerOnVisibilityChange(this as ComponentSprite);
			}
		}

		public ComponentVisual(Component2D component2D) : base()
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
