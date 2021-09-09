using SFML.Graphics;
using System;
using SMPL.Data;
using SMPL.Gear;
using Newtonsoft.Json;

namespace SMPL.Components
{
	public abstract class Visual : Component
	{
		private string areaUID;
		private string effectsUID;
		private string familyUID;
		private bool isHidden;

		// ===========

		[JsonProperty]
		internal string visualMaskingUID;

		// ===========

		[JsonProperty]
		public string AreaUniqueID
		{
			get { return ErrorIfDestroyed() ? default : areaUID; }
			set
			{
				if (ErrorIfDestroyed()) return;
				areaUID = value;
				if (areaUID == null) return;
				var area = (Area)PickByUniqueID(areaUID);
				if (area == null) Debug.LogError(1, $"No {nameof(Area)} found with uniqueID '{areaUID}'.");
			}
		}
		[JsonProperty]
		public string EffectsUniqueID
		{
			get { return ErrorIfDestroyed() ? default : effectsUID; }
			set
			{
				if (ErrorIfDestroyed()) return;
				effectsUID = value;
				if (effectsUID == null) return;
				var effects = (Effects)PickByUniqueID(effectsUID);
				if (effects == null) { Debug.LogError(1, $"No {nameof(Effects)} found with uniqueID '{effectsUID}'."); return; }
				effects.visualOwnerUID = UniqueID;
				if (effects.shader == null) effects.SetDefaults();
			}
		}
		[JsonProperty]
		public string FamilyUniqueID
		{
			get { return ErrorIfDestroyed() ? default : familyUID; }
			set
			{
				if (ErrorIfDestroyed()) return;
				familyUID = value;
				if (familyUID == null) return;
				var family = (Family)PickByUniqueID(familyUID);
				if (family == null) { Debug.LogError(1, $"No {nameof(Family)} found with uniqueID '{areaUID}'."); return; }
				var area = (Area)PickByUniqueID(areaUID);
				family.visualOwnerUID = UniqueID;
				if (area != null) area.familyUID = familyUID;
			}
		}

		[JsonProperty]
		public bool IsHidden
		{
			get { return ErrorIfDestroyed() == false && isHidden; }
			set { if (ErrorIfDestroyed() == false) isHidden = value; }
		}

		public Visual(string uniqueID) : base(uniqueID) { }
		public override void Destroy()
		{
			if (ErrorIfDestroyed()) return;
			areaUID = null;
			effectsUID = null;
			familyUID = null;
			visualMaskingUID = null;

			base.Destroy();
		}
		//public void DrawBounds
	}
}
