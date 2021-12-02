using SFML.Graphics;
using System;
using SMPL.Data;
using SMPL.Gear;
using Newtonsoft.Json;

namespace SMPL.Components
{
	public abstract class Visual : Thing
	{
		[JsonProperty]
		internal uint visualMaskingUID;

		private uint areaUID;
		[JsonProperty]
		public uint AreaUID
		{
			get { return ErrorIfDestroyed() ? default : areaUID; }
			set
			{
				if (ErrorIfDestroyed())
					return;
				areaUID = value;
				if (areaUID == default)
					return;
				var area = (Area)Pick(areaUID);
				if (area == null) Debug.LogError(1, $"No {nameof(Area)} found with uniqueID '{areaUID}'.");
			}
		}
		private uint effectsUID;
		[JsonProperty]
		public uint EffectsUID
		{
			get { return ErrorIfDestroyed() ? default : effectsUID; }
			set
			{
				if (ErrorIfDestroyed()) return;
				effectsUID = value;
				if (effectsUID == default) return;
				var effects = (Effects)Pick(effectsUID);
				if (effects == null) { Debug.LogError(1, $"No {nameof(Effects)} found with uniqueID '{effectsUID}'."); return; }
				effects.visualOwnerUID = UID;
				if (effects.shader == null) effects.SetDefaults();
			}
		}
		[JsonProperty]
		private uint familyUID;
		public uint FamilyUID
		{
			get { return ErrorIfDestroyed() ? default : familyUID; }
			set
			{
				if (ErrorIfDestroyed())
					return;
				familyUID = value;
				if (familyUID == default)
					return;
				var family = (Family)Pick(familyUID);
				if (family == null) { Debug.LogError(1, $"No {nameof(Family)} found with uniqueID '{areaUID}'."); return; }
				var area = (Area)Pick(areaUID);
				family.visualOwnerUID = UID;
				if (area != null) area.familyUID = familyUID;
			}
		}

		private bool isHidden;
		[JsonProperty]
		public bool IsHidden
		{
			get { return ErrorIfDestroyed() == false && isHidden; }
			set { if (ErrorIfDestroyed() == false) isHidden = value; }
		}

		public override void Destroy()
		{
			if (ErrorIfDestroyed()) return;
			areaUID = default;
			effectsUID = default;
			familyUID = default;
			visualMaskingUID = default;

			base.Destroy();
		}
		//public void DrawBounds
	}
}
