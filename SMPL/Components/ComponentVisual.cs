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
			get { return IsNotLoaded() ? default : effects; }
			set
			{
				if (effects == value || (Debug.CurrentMethodIsCalledByUser && IsCurrentlyAccessible() == false)) return;
				if (Debug.CurrentMethodIsCalledByUser && IsNotLoaded()) return;
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
			get { return IsNotLoaded() ? default : family; }
			set
			{
				if (family == value || (Debug.CurrentMethodIsCalledByUser && IsCurrentlyAccessible() == false)) return;
				if (Debug.CurrentMethodIsCalledByUser && IsNotLoaded()) return;
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
			get { return !IsNotLoaded() && isHidden; }
			set
			{
				if (isHidden == value || IsNotLoaded() || (Debug.CurrentMethodIsCalledByUser && IsCurrentlyAccessible() == false))
					return;
				isHidden = value;
				if (this is ComponentText) ComponentText.TriggerOnVisibilityChange(this as ComponentText);
				else ComponentSprite.TriggerOnVisibilityChange(this as ComponentSprite);
			}
		}

		public bool HasLoadedAssetFile { get; internal set; }

		internal bool IsNotLoaded()
		{
			if (HasLoadedAssetFile) return false;

			var comp = this is ComponentSprite ? nameof(ComponentSprite) : nameof(ComponentText);
			var asset = this is ComponentSprite ? nameof(File.Asset.Texture) : nameof(File.Asset.Font);
			Debug.LogError(2, $"This will have no effect due to this {comp}'s file not being loaded.\n" +
				$"To load it use '{nameof(File)}.{nameof(File.LoadAsset)}({nameof(File)}.{nameof(File.Asset)}." +
				$"{asset}, \"folder/file.extension\")'\n" +
				$"and recreate the component with it.");
			return true;
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
