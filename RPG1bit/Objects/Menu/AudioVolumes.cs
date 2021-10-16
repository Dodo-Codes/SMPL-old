using SMPL.Data;

namespace RPG1bit
{
	public class AudioVolumes : AdjustVolume
	{
		public static int PercentSound { get; set; } = 50;
		public static int PercentMusic { get; set; } = 50;

		public AudioVolumes(string uniqueID, CreationDetails creationDetails) : base(uniqueID, creationDetails) { }

		public override void ValueChanged()
		{
			if (UniqueID == "adjust-sound-volume") PercentSound = Percent;
			else if (UniqueID == "adjust-music-volume") PercentMusic = Percent;
		}
	}
}
