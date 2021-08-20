using SFML.Audio;
using SFML.System;
using System;
using System.Collections.Generic;

namespace SMPL
{
   public class ComponentAudio : ComponentAccess
   {
      public enum Type { NotLoaded, Sound, Music }
      internal Sound sound;
      internal Music music;
      private bool stopped;
      private readonly static List<ComponentAudio> audios = new();

      private static event Events.ParamsTwo<ComponentAudio, ComponentIdentity<ComponentAudio>> OnIdentityChange;
      private static event Events.ParamsTwo<ComponentAudio, string> OnCreate;
      private static event Events.ParamsOne<ComponentAudio> OnAudioStart, OnAudioPlay, OnAudioPause, OnAudioStop, OnAudioEnd;

      public static class CallWhen
      {
         public static void Create(Action<ComponentAudio, string> method, uint order = uint.MaxValue) =>
            OnCreate = Events.Add(OnCreate, method, order);
         public static void IdentityChange(Action<ComponentAudio, ComponentIdentity<ComponentAudio>> method,
            uint order = uint.MaxValue) => OnIdentityChange = Events.Add(OnIdentityChange, method, order);
         public static void Start(Action<ComponentAudio> method, uint order = uint.MaxValue) =>
        OnAudioStart = Events.Add(OnAudioStart, method, order);
         public static void End(Action<ComponentAudio> method, uint order = uint.MaxValue) =>
            OnAudioEnd = Events.Add(OnAudioEnd, method, order);
         public static void Play(Action<ComponentAudio> method, uint order = uint.MaxValue) =>
            OnAudioPlay = Events.Add(OnAudioPlay, method, order);
         public static void Pause(Action<ComponentAudio> method, uint order = uint.MaxValue) =>
            OnAudioPause = Events.Add(OnAudioPause, method, order);
         public static void Stop(Action<ComponentAudio> method, uint order = uint.MaxValue) =>
            OnAudioStop = Events.Add(OnAudioPause, method, order);
      }

      private ComponentIdentity<ComponentAudio> identity;
      public ComponentIdentity<ComponentAudio> Identity
      {
         get { return identity; }
         set
         {
            if (identity == value || (Debug.CurrentMethodIsCalledByUser && IsCurrentlyAccessible() == false)) return;
            var prev = identity;
            identity = value;
            OnIdentityChange?.Invoke(this, prev);
         }
      }

      public static Point ListenerPosition
      {
         get { return new Point(Listener.Position.X, Listener.Position.Z); }
         set { Listener.Position = new Vector3f((float)value.X, 0, (float)value.Y); }
      }

      public string FilePath { get; private set; }
      public bool IsLooping
      {
         get { return !IsNotLoaded() && (CurrentType == Type.Sound ? sound.Loop : music.Loop); }
         set
         {
            if (IsNotLoaded() || (Debug.CurrentMethodIsCalledByUser && IsCurrentlyAccessible() == false)) return;
            if (CurrentType == Type.Sound) sound.Loop = value;
            else music.Loop = value;
         }
      }
      public double VolumePercent
      {
         get { return IsNotLoaded() ? default : CurrentType == Type.Sound ? sound.Volume : music.Volume; }
         set
         {
            if (IsNotLoaded() || (Debug.CurrentMethodIsCalledByUser && IsCurrentlyAccessible() == false)) return;
            
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
            if (IsNotLoaded() || (Debug.CurrentMethodIsCalledByUser && IsCurrentlyAccessible() == false)) return;
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
            if (value && IsPlaying) OnAudioPause?.Invoke(this);
            else if (value == false && IsPaused) OnAudioPlay?.Invoke(this);
         }
      }
      public bool IsPlaying
      {
         get
         { return !IsNotLoaded() &&
               (CurrentType == Type.Sound ? sound.Status == SoundStatus.Playing : music.Status == SoundStatus.Playing); }
         set
         {
            if (IsNotLoaded() || (Debug.CurrentMethodIsCalledByUser && IsCurrentlyAccessible() == false)) return;
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
               if (IsPaused == false) OnAudioStart?.Invoke(this);
               OnAudioPlay?.Invoke(this);
            }
            else if (IsPlaying || IsPaused) OnAudioStop?.Invoke(this);
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
            if (IsNotLoaded() || (Debug.CurrentMethodIsCalledByUser && IsCurrentlyAccessible() == false)) return;
            var v = SFML.System.Time.FromSeconds((float)Number.Limit(value, new Bounds(0, Duration))); ;
            if (CurrentType == Type.Sound) sound.PlayingOffset = v;
            else music.PlayingOffset = v;
         }
      }
      public double ProgressPercent
      {
         get { return 100 - Number.ToPercent(Duration - Progress, new Bounds(0, Duration)); }
         set
         {
            if (IsNotLoaded() || (Debug.CurrentMethodIsCalledByUser && IsCurrentlyAccessible() == false)) return;
            Progress = Number.FromPercent(value, new Bounds(0, Duration));
         }
      }
      public double Speed
      {
         get { return IsNotLoaded() ? default : CurrentType == Type.Sound ? sound.Pitch : music.Pitch; }
         set
         {
            if (IsNotLoaded() || (Debug.CurrentMethodIsCalledByUser && IsCurrentlyAccessible() == false)) return;
            if (CurrentType == Type.Sound) sound.Pitch = (float)value;
            else music.Pitch = (float)value;
         }
      }
      public double DistanceFade
      {
         get { return IsNotLoaded() ? default : CurrentType == Type.Sound ? sound.Attenuation : music.Attenuation; }
         set
         {
            if (IsNotLoaded() || IsMono() || (Debug.CurrentMethodIsCalledByUser && IsCurrentlyAccessible() == false)) return;
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
            if (IsNotLoaded() || IsMono() || (Debug.CurrentMethodIsCalledByUser && IsCurrentlyAccessible() == false)) return;
            if (CurrentType == Type.Sound) sound.Position = new Vector3f((float)value.X, 0, (float)value.Y);
				else music.Position = new Vector3f((float)value.X, 0, (float)value.Y);
         }
      }
      public Type CurrentType { get; private set; }

      public ComponentAudio(string audioPath = "folder/audio.extension")
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

         audios.Add(this);
         FilePath = audioPath;
         VolumePercent = 50;
         OnCreate?.Invoke(this, audioPath);
      }

		internal static void Update()
		{
			for (int i = 0; i < audios.Count; i++)
			{
            if (Gate.EnterOnceWhile($"{audios[i].FilePath}-enda915'kf",
               audios[i].IsPlaying == false && audios[i].IsPaused == false && audios[i].stopped == false))
            {
               OnAudioEnd?.Invoke(audios[i]);
            }
         }
		}

		private bool IsMono()
      {
         if ((CurrentType == Type.Sound && sound.SoundBuffer.ChannelCount == 1) ||
            (CurrentType == Type.Music && music.ChannelCount == 1) ||
            CurrentType == Type.NotLoaded)
            return false;

         Debug.LogError(2,
            $"This will have no effect since the file '{FilePath}' has 2 channels (stereo) and\n" +
            $"only single channelled (mono) files support this.\n" +
            $"Please convert to mono before using environmental audio.");
         return true;
      }
      private bool IsNotLoaded()
      {
         if (CurrentType != Type.NotLoaded) return false;

         Debug.LogError(2, $"This will have no effect due to this {nameof(ComponentAudio)}'s file is not loaded.\n" +
            $"To load it use '{nameof(File)}.{nameof(File.LoadAsset)} ({nameof(File)}.{nameof(File.Asset)}." +
            $"{nameof(File.Asset.Sound)}, \"folder/audio.extension\")' or" +
            $"'{nameof(File)}.{nameof(File.LoadAsset)} ({nameof(File)}.{nameof(File.Asset)}." +
            $"{nameof(File.Asset.Music)}, \"folder/audio.extension\")'\n" +
            $"and recreate the component with it.");
         return true;
      }
   }
}
