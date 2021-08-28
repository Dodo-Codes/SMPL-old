using SFML.Graphics;
using System;
using SMPL.Data;
using SMPL.Gear;

namespace SMPL.Components
{
	public abstract class Visual : Access
	{
		protected readonly uint creationFrame;
		protected readonly double rand;
		internal Visual masking;

		private Area area;
		public Area Area
		{
			get { return AllAccess == Extent.Removed ? default : area; }
			set
			{
				if (area == value || (Debug.CalledBySMPL == false && IsCurrentlyAccessible() == false)) return;
				var prev = area;
				area = value;
				if (Debug.CalledBySMPL) return;
				if (this is TextBox) TextBox.TriggerOnAreaChange(this as TextBox, prev);
				else Sprite.TriggerOnAreaChange(this as Sprite, prev);
			}
		}
		private Effects effects;
		public Effects Effects
		{
			get { return AllAccess == Extent.Removed ? default : effects; }
			set
			{
				if (effects == value || (Debug.CalledBySMPL == false && IsCurrentlyAccessible() == false)) return;
				var prev = effects;
				effects = value;
				effects.owner = this;
				effects.SetDefaults();
				if (Debug.CalledBySMPL) return;
				if (this is TextBox) TextBox.TriggerOnEffectsChange(this as TextBox, prev);
				else Sprite.TriggerOnEffectsChange(this as Sprite, prev);
			}
		}
		private Family family;
		public Family Family
		{
			get { return AllAccess == Extent.Removed ? default : family; }
			set
			{
				if (family == value || (Debug.CalledBySMPL == false && IsCurrentlyAccessible() == false)) return;
				var prev = family;
				family = value;
				Area.family = family;
				if (Debug.CalledBySMPL) return;
				if (this is TextBox) TextBox.TriggerOnFamilyChange(this as TextBox, prev);
				else Sprite.TriggerOnFamilyChange(this as Sprite, prev);
			}
		}

		private bool isHidden;
		public bool IsHidden
		{
			get { return AllAccess != Extent.Removed && isHidden; }
			set
			{
				if (isHidden == value || (Debug.CalledBySMPL == false && IsCurrentlyAccessible() == false)) return;
				isHidden = value;
				if (this is TextBox) TextBox.TriggerOnVisibilityChange(this as TextBox);
				else Sprite.TriggerOnVisibilityChange(this as Sprite);
			}
		}

		internal void Dispose()
		{
			if (Effects != null)
			{
				if (Effects.shader != null) Effects.shader.Dispose();
				Effects.owner = null;
				Effects = null;
			}
			if (Family != null)
			{
				if (Family.Parent != null) Family.Parent.Family.children.Remove(this);
				Family.UnparentAllChildren();
				Family.owner = null;
			}
			if (masking != null && masking.Effects != null) masking.Effects.masks.Remove(this);
			AllAccess = Extent.Removed;
		}
		public Visual() : base()
		{
			creationFrame = Performance.FrameCount;
			rand = Number.Random(new Bounds(-9999, 9999), 5);
		}
		//public abstract void DrawBounds(Camera camera, float thickness, Color color);
	}
}
