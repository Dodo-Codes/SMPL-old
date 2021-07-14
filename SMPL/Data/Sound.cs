namespace SMPL
{
   public class Sound
   {
      internal SFML.Audio.Sound sound;

      private string path;
      public string Path
      {
         get { return path; }
         set
         {
            if (File.sounds.ContainsKey(value) == false)
            {
               Debug.LogError(2, $"The sound at '{value}' is not loaded.\n" +
                  $"Use '{nameof(File)}.{nameof(File.LoadAsset)} ({nameof(File)}.{nameof(File.Asset)}." +
                  $"{nameof(File.Asset.Sound)}, \"{value}\")' to load it.");
               return;
            }
            path = value;
            sound.SoundBuffer = new SFML.Audio.SoundBuffer(path);
         }
      }
      public bool IsGlobal
      {
         get { return !sound.RelativeToListener; }
         set { sound.RelativeToListener = !value; }
      }
      public bool IsLooping
      {
         get { return sound.Loop; }
         set { sound.Loop = value; }
      }
      public double VolumePercent
      {
         get { return sound.Volume; }
         set { sound.Volume = (float)value; }
      }
      public bool IsPaused 
      {
         get { return sound.Status == SFML.Audio.SoundStatus.Paused; }
         set
         {
            if (IsPaused && value == false) sound.Play();
            else if (IsPaused == false && value) sound.Pause();
         }
      }
      public bool IsPlaying
      {
         get { return sound.Status == SFML.Audio.SoundStatus.Playing; }
         set
         {
            if (IsPlaying && value == false) sound.Stop();
            else if (IsPlaying == false && value) sound.Play();
         }
      }
      public double Duration { get { return sound.SoundBuffer.Duration.AsSeconds(); } }
      public double CurrentTime
      {
         get { return sound.PlayingOffset.AsSeconds(); }
         set
         {
            var t = new SFML.System.Time();
            t = SFML.System.Time.FromSeconds((float)Number.Limit(value, new Bounds(0, Duration)));
            sound.PlayingOffset = t;
         }
      }
      public double ProgressPercent
      {
         get { return Number.ToPercent(Duration - CurrentTime, new Bounds(0, Duration)); }
         set { CurrentTime = Number.FromPercent(value, new Bounds(0, Duration)); }
      }
      public double PitchPercent
      {
         get { return sound.Pitch; }
         set { sound.Pitch = (float)value; }
      }

      public Sound(string soundPath = "folder/sound.wav")
      {
         sound = new();
         Path = soundPath;
         IsGlobal = true;
      }
   }
}
