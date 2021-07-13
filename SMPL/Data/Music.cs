namespace SMPL
{
   public class Music
   {
      internal SFML.Audio.Music music;

      private string path;
      public string Path
      {
         get { return path; }
         set
         {
            if (File.sounds.ContainsKey(value) == false)
            {
               Debug.LogError(2, $"The music at '{value}' is not loaded.\n" +
                  $"Use '{nameof(File)}.{nameof(File.LoadAsset)} ({nameof(File)}.{nameof(File.Asset)}." +
                  $"{nameof(File.Asset.Music)}, \"{value}\")' to load it.");
               return;
            }
            path = value;
            music = new(path);
         }
      }

      public Music(string soundPath = "folder/sound.wav")
      {
         
         Path = soundPath;
      }
   }
}
