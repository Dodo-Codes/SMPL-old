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
      private static event Events.ParamsTwo<ComponentAudio, Point> OnPositionChange;
      private static event Events.ParamsOne<Point> OnListenerPositionChange;
      private static event Events.ParamsOne<ComponentAudio> OnStart, OnPlay, OnPause, OnStop, OnEnd, 
         OnLoop, OnLoopChange, OnPlayChange, OnPauseChange, OnUpdate;
      private static event Events.ParamsTwo<ComponentAudio, double> OnVolumeChange, OnProgressChange, OnProgressPercentChange,
         OnFileProgressChange, OnSpeedChange, OnDistanceFadeChange;

      public static class CallWhen
      {
         public static void Create(Action<ComponentAudio, string> method, uint order = uint.MaxValue) =>
            OnCreate = Events.Add(OnCreate, method, order);
         public static void IdentityChange(Action<ComponentAudio, ComponentIdentity<ComponentAudio>> method,
            uint order = uint.MaxValue) => OnIdentityChange = Events.Add(OnIdentityChange, method, order);
         public static void Start(Action<ComponentAudio> method, uint order = uint.MaxValue) =>
            OnStart = Events.Add(OnStart, method, order);
         public static void End(Action<ComponentAudio> method, uint order = uint.MaxValue) =>
            OnEnd = Events.Add(OnEnd, method, order);
         public static void Play(Action<ComponentAudio> method, uint order = uint.MaxValue) =>
            OnPlay = Events.Add(OnPlay, method, order);
         public static void Pause(Action<ComponentAudio> method, uint order = uint.MaxValue) =>
            OnPause = Events.Add(OnPause, method, order);
         public static void Stop(Action<ComponentAudio> method, uint order = uint.MaxValue) =>
            OnStop = Events.Add(OnPause, method, order);
         public static void Loop(Action<ComponentAudio> method, uint order = uint.MaxValue) =>
            OnLoop = Events.Add(OnLoop, method, order);
         public static void LoopChange(Action<ComponentAudio> method, uint order = uint.MaxValue) =>
            OnLoopChange = Events.Add(OnLoopChange, method, order);
         public static void PauseChange(Action<ComponentAudio> method, uint order = uint.MaxValue) =>
            OnPauseChange = Events.Add(OnPauseChange, method, order);
         public static void PlayChange(Action<ComponentAudio> method, uint order = uint.MaxValue) =>
            OnPlayChange = Events.Add(OnPlayChange, method, order);
         public static void VolumeChange(Action<ComponentAudio, double> method, uint order = uint.MaxValue) =>
            OnVolumeChange = Events.Add(OnVolumeChange, method, order);
         public static void Update(Action<ComponentAudio> method, uint order = uint.MaxValue) =>
            OnUpdate = Events.Add(OnUpdate, method, order);
         public static void ProgressChange(Action<ComponentAudio, double> method, uint order = uint.MaxValue) =>
            OnProgressChange = Events.Add(OnProgressChange, method, order);
         public static void ProgressPercentChange(Action<ComponentAudio, double> method, uint order = uint.MaxValue) =>
            OnProgressPercentChange = Events.Add(OnProgressPercentChange, method, order);
         public static void FileProgressChange(Action<ComponentAudio, double> method, uint order = uint.MaxValue) =>
            OnFileProgressChange = Events.Add(OnFileProgressChange, method, order);
         public static void SpeedChange(Action<ComponentAudio, double> method, uint order = uint.MaxValue) =>
            OnSpeedChange = Events.Add(OnSpeedChange, method, order);
         public static void DistanceFadeChange(Action<ComponentAudio, double> method, uint order = uint.MaxValue) =>
            OnDistanceFadeChange = Events.Add(OnDistanceFadeChange, method, order);
         public static void PositionChange(Action<ComponentAudio, Point> method, uint order = uint.MaxValue) =>
            OnPositionChange = Events.Add(OnPositionChange, method, order);
         public static void ListenerPositionChange(Action<Point> method, uint order = uint.MaxValue) =>
            OnListenerPositionChange = Events.Add(OnListenerPositionChange, method, order);
      }

      private ComponentIdentity<ComponentAudio> identity;
      public ComponentIdentity<ComponentAudio> Identity
      {
         get { return identity; }
         set
         {
            if (identity == value || (Debug.CalledBySMPL == false && IsCurrentlyAccessible() == false)) return;
            var prev = identity;
            identity = value;
            if (Debug.CalledBySMPL == false) OnIdentityChange?.Invoke(this, prev);
         }
      }

      public static Point ListenerPosition
      {
         get { return new Point(Listener.Position.X, Listener.Position.Z); }
         set
         {
            if (ListenerPosition == value) return;
            var prev = ListenerPosition;
            Listener.Position = new Vector3f((float)value.X, 0, (float)value.Y);
            if (Debug.CalledBySMPL == false) OnListenerPositionChange?.Invoke(prev);
         }
      }

      private bool rewindIsNotLoop;
      private double lastFrameProg;
      public string FilePath { get; private set; }
      public bool IsLooping
      {
         get { return !IsNotLoaded() && (CurrentType == Type.Sound ? sound.Loop : music.Loop); }
         set
         {
            if (IsLooping == value || IsNotLoaded() ||
               (Debug.CalledBySMPL == false && IsCurrentlyAccessible() == false)) return;
            if (CurrentType == Type.Sound) sound.Loop = value;
            else music.Loop = value;
            if (Debug.CalledBySMPL == false) OnLoopChange?.Invoke(this);
         }
      }
      public bool IsPaused 
      {
         get
         { return !IsNotLoaded() &&
                (CurrentType == Type.Sound ? sound.Status == SoundStatus.Paused : music.Status == SoundStatus.Paused); }
         set
         {
            if (IsPaused == value || IsNotLoaded() ||
               (Debug.CalledBySMPL == false && IsCurrentlyAccessible() == false)) return;
            if (Debug.CalledBySMPL == false) OnPauseChange?.Invoke(this);
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
            if (value && IsPlaying && Debug.CalledBySMPL == false) OnPause?.Invoke(this);
            else if (value == false && IsPaused && Debug.CalledBySMPL == false) OnPlay?.Invoke(this);
         }
      }
      public bool IsPlaying
      {
         get
         { return !IsNotLoaded() &&
               (CurrentType == Type.Sound ? sound.Status == SoundStatus.Playing : music.Status == SoundStatus.Playing); }
         set
         {
            if (IsPlaying == value || IsNotLoaded() ||
               (Debug.CalledBySMPL == false && IsCurrentlyAccessible() == false)) return;
            if (Debug.CalledBySMPL == false) OnPlayChange?.Invoke(this);
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
            if (value && Debug.CalledBySMPL == false)
            {
               if (IsPaused == false) OnStart?.Invoke(this);
               OnPlay?.Invoke(this);
            }
            else if ((IsPlaying || IsPaused) && Debug.CalledBySMPL == false) OnStop?.Invoke(this);
         }
      }
      public double Duration => IsNotLoaded() ? default : FileDuration / Speed;
      public double FileDuration => IsNotLoaded() ? default :
         CurrentType == Type.Sound ? sound.SoundBuffer.Duration.AsSeconds() : music.Duration.AsSeconds();
      public double VolumePercent
      {
         get { return IsNotLoaded() ? default : CurrentType == Type.Sound ? sound.Volume : music.Volume; }
         set
         {
            if (VolumePercent == value || IsNotLoaded() ||
               (Debug.CalledBySMPL == false && IsCurrentlyAccessible() == false)) return;
            var prev = VolumePercent;
            if (CurrentType == Type.Sound) sound.Volume = (float)value;
            else music.Volume = (float)value;
            if (Debug.CalledBySMPL == false) OnVolumeChange?.Invoke(this, prev);
         }
      }
      public double Progress
      {
         get { return IsNotLoaded() ? default : FileProgress / Speed; }
         set
         {
            if (Progress == value || IsNotLoaded() ||
               (Debug.CalledBySMPL == false && IsCurrentlyAccessible() == false)) return;
            var prevPrg = Progress;
            var prevFlPrg = FileProgress;
            var prevPerPrg = ProgressPercent;
            FileProgress = value * Speed;
            if (Debug.CalledBySMPL) return;
            OnProgressChange?.Invoke(this, prevPrg);
            OnFileProgressChange?.Invoke(this, prevFlPrg);
            OnProgressPercentChange?.Invoke(this, prevPerPrg);
         }
      }
      public double FileProgress
      {
         get { return IsNotLoaded() ? default :
               CurrentType == Type.Sound ? sound.PlayingOffset.AsSeconds() : music.PlayingOffset.AsSeconds(); }
         set
         {
            value = Number.Limit(value, new Bounds(0, FileDuration));
            if (FileProgress == value || IsNotLoaded() ||
               (Debug.CalledBySMPL == false && IsCurrentlyAccessible() == false)) return;
            var prevPrg = Progress;
            var prevFlPrg = FileProgress;
            var prevPerPrg = ProgressPercent;
            var v = SFML.System.Time.FromSeconds((float)value);
            if (CurrentType == Type.Sound) sound.PlayingOffset = v;
            else music.PlayingOffset = v;
            rewindIsNotLoop = true;
            if (Debug.CalledBySMPL) return;
            OnProgressChange?.Invoke(this, prevPrg);
            OnFileProgressChange?.Invoke(this, prevFlPrg);
            OnProgressPercentChange?.Invoke(this, prevPerPrg);
         }
      }
      public double ProgressPercent
      {
         get { return IsNotLoaded() ? default : 100 - Number.ToPercent(FileDuration - FileProgress,
            new Bounds(0, FileDuration)); }
         set
         {
            value = Number.FromPercent(value, new Bounds(0, FileDuration));
            if (FileProgress == value || IsNotLoaded() ||
               (Debug.CalledBySMPL == false && IsCurrentlyAccessible() == false)) return;
            var prevPrg = Progress;
            var prevFlPrg = FileProgress;
            var prevPerPrg = ProgressPercent;
            FileProgress = value;
            if (Debug.CalledBySMPL) return;
            OnProgressChange?.Invoke(this, prevPrg);
            OnFileProgressChange?.Invoke(this, prevFlPrg);
            OnProgressPercentChange?.Invoke(this, prevPerPrg);
         }
      }
      public double Speed
      {
         get { return IsNotLoaded() ? default : CurrentType == Type.Sound ? sound.Pitch : music.Pitch; }
         set
         {
            if (Speed == value || IsNotLoaded() ||
               (Debug.CalledBySMPL == false && IsCurrentlyAccessible() == false)) return;
            var prev = Speed;
            if (CurrentType == Type.Sound) sound.Pitch = (float)value;
            else music.Pitch = (float)value;
            if (Debug.CalledBySMPL == false) OnSpeedChange?.Invoke(this, prev);
         }
      }
      public double DistanceFade
      {
         get { return IsNotLoaded() ? default : CurrentType == Type.Sound ? sound.Attenuation : music.Attenuation; }
         set
         {
            if (DistanceFade == value || IsNotLoaded() || IsMono() ||
               (Debug.CalledBySMPL == false && IsCurrentlyAccessible() == false)) return;
            var prev = DistanceFade;
            if (CurrentType == Type.Sound) sound.Attenuation = (float)value;
            else music.Attenuation = (float)value;
            if (Debug.CalledBySMPL == false) OnDistanceFadeChange?.Invoke(this, prev);
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
            if (Position == value || IsNotLoaded() || IsMono() ||
               (Debug.CalledBySMPL == false && IsCurrentlyAccessible() == false)) return;
            var prev = Position;
            if (CurrentType == Type.Sound) sound.Position = new Vector3f((float)value.X, 0, (float)value.Y);
				else music.Position = new Vector3f((float)value.X, 0, (float)value.Y);
            if (Debug.CalledBySMPL == false) OnPositionChange?.Invoke(this, prev);
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
            if (audios[i].IsPlaying) OnUpdate?.Invoke(audios[i]);
            if (Gate.EnterOnceWhile($"{audios[i].FilePath}-enda915'kf",
               audios[i].IsPlaying == false && audios[i].IsPaused == false && audios[i].stopped == false))
                  OnEnd?.Invoke(audios[i]);
            if (audios[i].FileProgress + audios[i].FileDuration / 2 < audios[i].lastFrameProg &&
               audios[i].rewindIsNotLoop == false)
                  OnLoop?.Invoke(audios[i]);
            audios[i].lastFrameProg = audios[i].FileProgress;
            audios[i].rewindIsNotLoop = false;
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
