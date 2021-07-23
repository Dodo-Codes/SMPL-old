using SFML.Audio;
using SFML.System;
using System.Collections.Generic;

namespace SMPL
{
   public class Audio : Events
   {
      public enum Type { NotLoaded, Sound, Music }
      internal Sound sound;
      internal Music music;
      private bool stopped;

      public ComponentIdentity<Audio> IdentityComponent { get; set; }

      public static Point ListenerPosition
      {
         get { return new Point(Listener.Position.X, Listener.Position.Z); }
         set { Listener.Position = new Vector3f((float)value.X, 0, (float)value.Y); }
      }

      public string Path { get; private set; }
      public bool IsLooping
      {
         get { return !IsNotLoaded() && (CurrentType == Type.Sound ? sound.Loop : music.Loop); }
         set
         {
            if (IsNotLoaded()) return;
            if (CurrentType == Type.Sound) sound.Loop = value;
            else music.Loop = value;
         }
      }
      public double VolumePercent
      {
         get { return IsNotLoaded() ? default : CurrentType == Type.Sound ? sound.Volume : music.Volume; }
         set
         {
            if (IsNotLoaded()) return;
            
            if (CurrentType == Type.Sound) sound.Volume = (float)value;
            else music.Volume = (float)value;
         }
      }
      public bool IsPaused 
      {
         get
         { return !IsNotLoaded() &&
                (CurrentType == Type.Sound ? sound.Status == SoundStatus.Paused : music.Status == SoundStatus.Paused); }
         set
         {
            if (IsNotLoaded()) return;
            if (CurrentType == Type.Sound)
            {
               if (value && IsPlaying) sound.Pause();
               else if (value == false && IsPaused) sound.Play();
            }
            else
            {
               if (value && IsPlaying) music.Pause();
               else if (value == false && IsPaused) music.Play();
            }
            if (value && IsPlaying)
            {
               var n = D(instances); foreach (var kvp in n) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++)
                  e[i].OnAudioPauseSetup(this); }
               var n1 = D(instances); foreach (var kvp in n1) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++)
                  e[i].OnAudioPause(this); }
            }
            else if (value == false && IsPaused)
            {
               var n = D(instances); foreach (var kvp in n) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++)
                  e[i].OnAudioPlaySetup(this); }
               var n1 = D(instances); foreach (var kvp in n1) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++)
                  e[i].OnAudioPlay(this); }
            }
         }
      }
      public bool IsPlaying
      {
         get
         { return !IsNotLoaded() &&
               (CurrentType == Type.Sound ? sound.Status == SoundStatus.Playing : music.Status == SoundStatus.Playing); }
         set
         {
            if (IsNotLoaded()) return;
            if (CurrentType == Type.Sound)
            {
               stopped = !value;
               if (value) sound.Play();
               else if (IsPlaying || IsPaused) sound.Stop();
            }
            else
            {
               stopped = !value;
               if (value) music.Play();
               else if (IsPlaying || IsPaused) music.Stop();
            }
            if (value)
            {
               if (IsPaused == false)
               {
                  var n = D(instances); foreach (var kvp in n) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++)
                     e[i].OnAudioStartSetup(this); }
                  var n1 = D(instances); foreach (var kvp in n1) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++)
                     e[i].OnAudioStart(this); }
               }
               var n2 = D(instances); foreach (var kvp in n2) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++)
                  e[i].OnAudioPlaySetup(this); }
               var n3 = D(instances); foreach (var kvp in n3) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++)
                  e[i].OnAudioPlay(this); }
            }
            else if (IsPlaying || IsPaused)
            {
               var n1 = D(instances); foreach (var kvp in n1) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++)
                  e[i].OnAudioStopSetup(this); }
               var n2 = D(instances); foreach (var kvp in n2) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++)
                  e[i].OnAudioStop(this); }
            }
         }
      }
      public double Duration
      {
         get { return IsNotLoaded() ? default :
               CurrentType == Type.Sound ? sound.SoundBuffer.Duration.AsSeconds() : music.Duration.AsSeconds(); }
      }
      public double Progress
      {
         get { return IsNotLoaded() ? default :
               CurrentType == Type.Sound ? sound.PlayingOffset.AsSeconds() : music.PlayingOffset.AsSeconds(); }
         set
         {
            if (IsNotLoaded()) return;
            var v = SFML.System.Time.FromSeconds((float)Number.Limit(value, new Bounds(0, Duration))); ;
            if (CurrentType == Type.Sound) sound.PlayingOffset = v;
            else music.PlayingOffset = v;
         }
      }
      public double ProgressPercent
      {
         get { return 100 - Number.ToPercent(Duration - Progress, new Bounds(0, Duration)); }
         set { Progress = Number.FromPercent(value, new Bounds(0, Duration)); }
      }
      public double Speed
      {
         get { return IsNotLoaded() ? default : CurrentType == Type.Sound ? sound.Pitch : music.Pitch; }
         set
         {
            if (IsNotLoaded()) return;
            if (CurrentType == Type.Sound) sound.Pitch = (float)value;
            else music.Pitch = (float)value;
         }
      }
      public double DistanceFade
      {
         get { return IsNotLoaded() ? default : CurrentType == Type.Sound ? sound.Attenuation : music.Attenuation; }
         set
         {
            if (IsNotLoaded()) return;
            IsMono();
            if (CurrentType == Type.Sound) sound.Attenuation = (float)value;
            else music.Attenuation = (float)value;
         }
      }
      public Point Position
      {
         get
         {
				return IsNotLoaded() ? default :
               CurrentType == Type.Sound ?
               new Point(sound.Position.X, sound.Position.Z) :
               new Point(music.Position.X, music.Position.Z);
			}
			set
         {
            if (IsNotLoaded()) return;
            IsMono();
				if (CurrentType == Type.Sound) sound.Position = new Vector3f((float)value.X, 0, (float)value.Y);
				else music.Position = new Vector3f((float)value.X, 0, (float)value.Y);
         }
      }
      private Type type;
      public Type CurrentType
      {
         get { return type; }
         private set { type = value; }
      }

      public Audio(string audioPath = "folder/audio.extension")
      {
         if (File.sounds.ContainsKey(audioPath))
         {
            sound = File.sounds[audioPath];
            sound.RelativeToListener = false;
            sound.Attenuation = 0;
            CurrentType = Type.Sound;
         }
			else if (File.music.ContainsKey(audioPath))
			{
            music = File.music[audioPath];
            music.RelativeToListener = false;
            music.Attenuation = 0;
            CurrentType = Type.Music;
         }
			else { IsNotLoaded(); return; }

         Subscribe(this, 0);
         Path = audioPath;
         VolumePercent = 50;
      }

		public override void OnEachFrameSetup()
		{
			if (Gate.EnterOnceWhile($"{Path}-enda915'kf", IsPlaying == false && IsPaused == false && stopped == false))
			{
            var n = D(instances); foreach (var kvp in n) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++)
               e[i].OnAudioEndSetup(this); }
            var n2 = D(instances); foreach (var kvp in n2) { var e = L(kvp.Value); for (int i = 0; i < e.Count; i++)
               e[i].OnAudioEnd(this); }
         }
		}

		private void IsMono()
      {
         if ((CurrentType == Type.Sound && sound.SoundBuffer.ChannelCount == 1) ||
            (CurrentType == Type.Music && music.ChannelCount == 1) ||
            CurrentType == Type.NotLoaded)
            return;

         Debug.LogError(2,
            $"This will have no effect since the file '{Path}' has 2 channels (stereo) and\n" +
            $"only single channelled (mono) files support this.\n" +
            $"Please convert the channels of the file '{Path}' to mono before using environmental audio.");
      }
      private bool IsNotLoaded()
      {
         if (CurrentType != Type.NotLoaded) return false;

         Debug.LogError(2, $"This audio file is not loaded.\n" +
            $"To load it use '{nameof(File)}.{nameof(File.LoadAsset)} ({nameof(File)}.{nameof(File.Asset)}." +
            $"{nameof(File.Asset.Sound)}, \"folder/audio.extension\")' or" +
            $"'{nameof(File)}.{nameof(File.LoadAsset)} ({nameof(File)}.{nameof(File.Asset)}." +
            $"{nameof(File.Asset.Music)}, \"folder/audio.extension\")'.");
         return true;
      }
   }
}
