using SFML.Graphics;
using System;
using SMPL.Data;
using SMPL.Gear;

namespace SMPL.Components
{
	public abstract class Visual : Component
	{
		private Area area;
		private Effects effects;
		private Family family;
		private bool isHidden;

		// ===========

		internal Visual masking;

		// ===========

		public Area Area
		{
			get { return ErrorIfDestroyed() ? default : area; }
			set { if (ErrorIfDestroyed() == false) area = value; }
		}
		public Effects Effects
		{
			get { return ErrorIfDestroyed() ? default : effects; }
			set
			{
				if (ErrorIfDestroyed()) return;
				effects = value;
				effects.owner = this;
				effects.SetDefaults();
			}
		}
		public Family Family
		{
			get { return ErrorIfDestroyed() ? default : family; }
			set
			{
				if (ErrorIfDestroyed()) return;
				family = value;
				family.owner = this;
				Area.family = family;
			}
		}

		public bool IsHidden
		{
			get { return ErrorIfDestroyed() == false && isHidden; }
			set { if (ErrorIfDestroyed() == false) isHidden = value; }
		}

		public Visual(string uniqueID) : base(uniqueID) { }
		public override void Destroy()
		{
			if (ErrorIfDestroyed()) return;
			if (Effects != null) Effects.Destroy();
			if (Family != null) Family.Destroy();
			if (Area != null) Area.Destroy();
			if (masking != null && masking.Effects != null) masking.Effects.masks.Remove(this);
			area = null;
			effects = null;
			family = null;
			masking = null;

			base.Destroy();
		}
		//public void DrawBounds
	}
}
