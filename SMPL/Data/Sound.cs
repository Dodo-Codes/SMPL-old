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

      public Sound(string soundPath = "folder/sound.wav")
      {
         sound = new();
         Path = soundPath;
         IsGlobal = true;
      }
   }
}
