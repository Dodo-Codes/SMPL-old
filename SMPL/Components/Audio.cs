using SFML.Audio;
using SFML.System;
using System;
using System.Collections.Generic;
using SMPL.Data;
using SMPL.Gear;

namespace SMPL.Components
{
   public class Audio : Component
   {
      private static event Events.ParamsOne<Audio> OnStart, OnPlay, OnPause, OnStop, OnEnd, OnLoop;
      private readonly static List<Audio> audios = new();
      private bool stopped, rewindIsNotLoop;
      private double lastFrameProg;

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

         Assets.NotLoadedError(Assets.Type.Audio, FilePath);
         return true;
      }

      //===============

      internal Sound sound;
      internal Music music;
		internal static void Update()
		{
			for (int i = 0; i < audios.Count; i++)
			{
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

      //=============

      public enum Type { NotLoaded, Sound, Music }

      public static class CallWhen
      {
         public static void Start(Action<Audio> method, uint order = uint.MaxValue) =>
            OnStart = Events.Add(OnStart, method, order);
         public static void End(Action<Audio> method, uint order = uint.MaxValue) =>
            OnEnd = Events.Add(OnEnd, method, order);
         public static void Play(Action<Audio> method, uint order = uint.MaxValue) =>
            OnPlay = Events.Add(OnPlay, method, order);
         public static void Pause(Action<Audio> method, uint order = uint.MaxValue) =>
            OnPause = Events.Add(OnPause, method, order);
         public static void Stop(Action<Audio> method, uint order = uint.MaxValue) =>
            OnStop = Events.Add(OnPause, method, order);
         public static void Loop(Action<Audio> method, uint order = uint.MaxValue) =>
            OnLoop = Events.Add(OnLoop, method, order);
      }
      public static Point ListenerPosition
      {
         get { return new() { X = Listener.Position.X, Y = Listener.Position.Z }; }
         set { Listener.Position = new Vector3f((float)value.X, 0, (float)value.Y); }
      }

      public string FilePath { get; private set; }
      public bool IsLooping
      {
         get { return !IsNotLoaded() && (CurrentType == Type.Sound ? sound.Loop : music.Loop); }
         set
         {
            if (ErrorIfDestroyed()) return;
            if (CurrentType == Type.Sound) sound.Loop = value;
            else music.Loop = value;
         }
      }
      public bool IsPaused 
      {
         get
         { return !IsNotLoaded() &&
                (CurrentType == Type.Sound ? sound.Status == SoundStatus.Paused : music.Status == SoundStatus.Paused); }
         set
         {
            if (ErrorIfDestroyed()) return;
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
            if (value && IsPlaying) OnPause?.Invoke(this);
            else if (value == false && IsPaused) OnPlay?.Invoke(this);
         }
      }
      public bool IsPlaying
      {
         get
         { return !IsNotLoaded() &&
               (CurrentType == Type.Sound ? sound.Status == SoundStatus.Playing : music.Status == SoundStatus.Playing); }
         set
         {
            if (ErrorIfDestroyed()) return;
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
               if (IsPaused == false) OnStart?.Invoke(this);
               OnPlay?.Invoke(this);
            }
            else if ((IsPlaying || IsPaused)) OnStop?.Invoke(this);
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
            if (ErrorIfDestroyed()) return;
            if (CurrentType == Type.Sound) sound.Volume = (float)value;
            else music.Volume = (float)value;
         }
      }
      public double Progress
      {
         get { return IsNotLoaded() ? default : FileProgress / Speed; }
         set { if (ErrorIfDestroyed() == false) FileProgress = value * Speed; }
      }
      public double FileProgress
      {
         get { return IsNotLoaded() ? default :
               CurrentType == Type.Sound ? sound.PlayingOffset.AsSeconds() : music.PlayingOffset.AsSeconds(); }
         set
         {
            if (ErrorIfDestroyed()) return;
            var v = SFML.System.Time.FromSeconds((float)Number.Limit(value, new Bounds(0, FileDuration)));
            if (CurrentType == Type.Sound) sound.PlayingOffset = v;
            else music.PlayingOffset = v;
            rewindIsNotLoop = true;
         }
      }
      public double ProgressPercent
      {
         get { return IsNotLoaded() ? default : 100 - Number.ToPercent(FileDuration - FileProgress,
            new Bounds(0, FileDuration)); }
         set { if (ErrorIfDestroyed() == false)  FileProgress = Number.FromPercent(value, new Bounds(0, FileDuration)); }
      }
      public double Speed
      {
         get { return IsNotLoaded() ? default : CurrentType == Type.Sound ? sound.Pitch : music.Pitch; }
         set
         {
            if (ErrorIfDestroyed()) return;
            if (CurrentType == Type.Sound) sound.Pitch = (float)value;
            else music.Pitch = (float)value;
         }
      }
      public double DistanceFade
      {
         get { return IsNotLoaded() ? default : CurrentType == Type.Sound ? sound.Attenuation : music.Attenuation; }
         set
         {
            if (ErrorIfDestroyed()) return;
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
            if (ErrorIfDestroyed() || IsMono()) return;
            if (CurrentType == Type.Sound) sound.Position = new Vector3f((float)value.X, 0, (float)value.Y);
				else music.Position = new Vector3f((float)value.X, 0, (float)value.Y);
         }
      }
      public Type CurrentType { get; private set; }

      public Audio(string uniqueID, string audioPath = "folder/audio.extension") : base(uniqueID)
      {
         if (Assets.sounds.ContainsKey(audioPath))
         {
            sound = Assets.sounds[audioPath];
            sound.RelativeToListener = false;
            sound.Attenuation = 0;
            CurrentType = Type.Sound;
         }
			else if (Assets.music.ContainsKey(audioPath))
			{
            music = Assets.music[audioPath];
            music.RelativeToListener = false;
            music.Attenuation = 0;
            CurrentType = Type.Music;
         }
			else { IsNotLoaded(); return; }

         audios.Add(this);
         FilePath = audioPath;
         VolumePercent = 50;
         if (cannotCreate) { ErrorAlreadyHasUID(uniqueID); Destroy(); }
      }
		public override void Destroy()
		{
         if (ErrorIfDestroyed()) return;
         audios.Remove(this);
         if (sound != null) sound.Dispose();
         if (music != null) music.Dispose();
         sound = null;
         music = null;
         base.Destroy();
		}
   }
}
