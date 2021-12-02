using SFML.Window;
using System;
using System.Windows.Forms;
using System.Threading;
using SMPL.Data;
using SMPL.Components;
using SMPL.Prefabs;
using System.Collections.Generic;
using System.Reflection;
using Timer = SMPL.Components.Timer;

namespace SMPL.Gear
{
   internal static class Events
   {
      internal struct EventArgs
      {
         public Keyboard.TextInput TextInput { get; set; }
         public Timer Timer { get; set; }
         public Camera Camera { get; set; }
         public Audio Audio { get; set; }
         public Multiplayer.Message Message { get; set; }
         public Keyboard.Key Key { get; set; }
         public Mouse.Button Button { get; set; }
         public Mouse.Wheel Wheel { get; set; }
         public double[] Double { get; set; }
         public int[] Int { get; set; }
         public string[] String { get; set; }
         public bool[] Bool { get; set; }
      }
      internal static Dictionary<Game.Event, SortedDictionary<uint, List<uint>>> notifications = new();

      internal static bool Enable(Game.Event notificationType, uint thingUID, uint order)
      {
         if (Game.NotStartedError(3))
            return false;
         if (notifications.ContainsKey(notificationType) && notifications[notificationType].ContainsKey(order) &&
            notifications[notificationType][order].Contains(thingUID))
         {
            Debug.LogError(2, $"The {nameof(Thing)} '{thingUID}' is already subscribed to this event.");
            return false;
         }

         if (notifications.ContainsKey(notificationType) == false) notifications[notificationType] = new();
         if (notifications[notificationType].ContainsKey(order) == false) notifications[notificationType][order] = new();
         if (notifications[notificationType][order].Contains(thingUID) == false) notifications[notificationType][order].Add(thingUID);
         return true;
      }
      internal static bool Disable(Game.Event notificationType, uint thingUID)
      {
         if (Game.NotStartedError(3))
            return false;

         var orders = notifications[notificationType];
         var error = false;
         foreach (var kvp in orders)
            if (kvp.Value.Remove(thingUID) == false)
               error = true;
         if (notifications.ContainsKey(notificationType) == false || error)
         {
            Debug.LogError(2, $"The {nameof(Thing)} '{thingUID}' has to be subscribed in order to unsubscribe from this event.");
            return false;
         }
         return true;
      }
      internal static void Notify(Game.Event notificationType, EventArgs eventArgs = default)
      {
         if (notifications.ContainsKey(notificationType) == false) return;
         var orders = new SortedDictionary<uint, List<uint>>(notifications[notificationType]);
         foreach (var kvp in orders)
            for (int i = 0; i < kvp.Value.Count; i++)
            {
               var thing = Thing.Pick(kvp.Value[i]);
               switch (notificationType)
               {
                  case Game.Event.GameStart: thing.OnGameAction(Game.Action.Start); break;
                  case Game.Event.GameStop: thing.OnGameAction(Game.Action.Stop); break;
                  case Game.Event.GameUpdate: thing.OnGameAction(Game.Action.Update); break;
                  case Game.Event.GameLateUpdate: thing.OnGameAction(Game.Action.LateUpdate); break;
                  case Game.Event.MouseButtonDoubleClick:
                     thing.OnMouseButtonAction(Mouse.ButtonAction.DoubleClick, eventArgs.Button); break;
                  case Game.Event.MouseButtonPress: thing.OnMouseButtonAction(Mouse.ButtonAction.Press, eventArgs.Button); break;
                  case Game.Event.MouseButtonHold: thing.OnMouseButtonAction(Mouse.ButtonAction.Hold, eventArgs.Button); break;
                  case Game.Event.MouseButtonRelease: thing.OnMouseButtonAction(Mouse.ButtonAction.Release, eventArgs.Button); break;
                  case Game.Event.MouseWheelScroll: thing.OnMouseWheelScroll(eventArgs.Wheel, eventArgs.Double[0]); break;
                  case Game.Event.MouseCursorEnterWindow: thing.OnMouseCursorAction(Mouse.Cursor.Action.WindowEnter); break;
                  case Game.Event.MouseCursorLeaveWindow: thing.OnMouseCursorAction(Mouse.Cursor.Action.WindowLeave); break;
                  case Game.Event.AssetLoadEnd: thing.OnAssetLoadEnd(eventArgs.String[0]); break;
                  case Game.Event.AssetDataSlotSaveEnd: thing.OnAssetDataSlotSaveEnd(eventArgs.String[0]); break;
                  case Game.Event.KeyboardKeyPress: thing.OnKeyboardKeyAction(Keyboard.KeyAction.Press, eventArgs.Key); break;
                  case Game.Event.KeyboardKeyHold: thing.OnKeyboardKeyAction(Keyboard.KeyAction.Hold, eventArgs.Key); break;
                  case Game.Event.KeyboardKeyRelease: thing.OnKeyboardKeyAction(Keyboard.KeyAction.Release, eventArgs.Key); break;
                  case Game.Event.KeyboardTextInput: thing.OnKeyboardTextInput(eventArgs.TextInput); break;
                  case Game.Event.KeyboardLanguageChange:
                     thing.OnKeyboardLanguageChange(eventArgs.String[0], eventArgs.String[1], eventArgs.String[2]); break;
                  case Game.Event.WindowResize: thing.OnWindowAction(Window.Action.Resize); break;
                  case Game.Event.WindowClose: thing.OnWindowAction(Window.Action.Close); break;
                  case Game.Event.WindowFocus: thing.OnWindowAction(Window.Action.Focus); break;
                  case Game.Event.WindowUnfocus: thing.OnWindowAction(Window.Action.Unfocus); break;
                  case Game.Event.WindowMaximize: thing.OnWindowAction(Window.Action.Maximize); break;
                  case Game.Event.WindowMinimize: thing.OnWindowAction(Window.Action.Minimize); break;
                  case Game.Event.WindowFullscreen: thing.OnWindowAction(Window.Action.Fullscreen); break;
                  case Game.Event.MultiplayerServerStart: thing.OnMultiplayerServerAction(Multiplayer.Action.ServerStart); break;
                  case Game.Event.MultiplayerServerStop: thing.OnMultiplayerServerAction(Multiplayer.Action.ServerStop); break;
                  case Game.Event.MultiplayerClientConnect:
                     thing.OnMultiplayerClientAction(Multiplayer.Action.ClientConnect, eventArgs.String[0]); break;
                  case Game.Event.MultiplayerClientDisconnect:
                     thing.OnMultiplayerClientAction(Multiplayer.Action.ClientDisconnect, eventArgs.String[0]); break;
                  case Game.Event.MultiplayerClientTakenUID:
                     thing.OnMultiplayerClientAction(Multiplayer.Action.ClientTakenUID, eventArgs.String[0]); break;
                  case Game.Event.MultiplayerMessageReceived: thing.OnMultiplayerMessageReceived(eventArgs.Message); break;
                  case Game.Event.AudioStart: thing.OnAudioAction(Audio.Action.Start, eventArgs.Audio); break;
                  case Game.Event.AudioPlay: thing.OnAudioAction(Audio.Action.Play, eventArgs.Audio); break;
                  case Game.Event.AudioPause: thing.OnAudioAction(Audio.Action.Pause, eventArgs.Audio); break;
                  case Game.Event.AudioStop: thing.OnAudioAction(Audio.Action.Stop, eventArgs.Audio); break;
                  case Game.Event.AudioEnd: thing.OnAudioAction(Audio.Action.End, eventArgs.Audio); break;
                  case Game.Event.AudioLoop: thing.OnAudioAction(Audio.Action.Loop, eventArgs.Audio); break;
                  case Game.Event.TimerCreateAndStart: thing.OnTimerAction(Timer.Action.CreateAndStart, eventArgs.Timer); break;
                  case Game.Event.TimerUpdate: thing.OnTimerAction(Timer.Action.Update, eventArgs.Timer); break;
                  case Game.Event.TimerEnd: thing.OnTimerAction(Timer.Action.End, eventArgs.Timer); break;
                  case Game.Event.CameraDisplay: thing.OnCameraDisplay(eventArgs.Camera); break;
               }
            }
      }
   }

   public abstract class Game : Thing
   {
      public enum Event
      {
         GameStart, GameUpdate, GameLateUpdate, GameStop,
         MouseButtonDoubleClick, MouseButtonPress, MouseButtonHold, MouseButtonRelease, MouseWheelScroll, MouseCursorLeaveWindow,
         MouseCursorEnterWindow,
         AssetLoadEnd, AssetDataSlotSaveEnd,
         KeyboardKeyPress, KeyboardKeyHold, KeyboardKeyRelease, KeyboardTextInput, KeyboardLanguageChange,
         WindowResize, WindowClose, WindowFocus, WindowUnfocus, WindowMaximize, WindowMinimize, WindowFullscreen,
         MultiplayerServerStart, MultiplayerServerStop, MultiplayerClientConnect, MultiplayerClientDisconnect, MultiplayerClientTakenUID, MultiplayerMessageReceived,
         AudioStart, AudioPlay, AudioPause, AudioStop, AudioEnd, AudioLoop,
         TimerCreateAndStart, TimerUpdate, TimerEnd,
         CameraDisplay
      }
      public enum Action
      {
         Start, Stop, Update, LateUpdate
      }

      ///<summary>
      ///text, <paramref name="param"/>, <see cref="char"/>, <typeparamref name="Type"/>
      ///</summary>
      private static void Main() { }
      internal static bool isStarted;

      public static void Start(Game game, Size pixelSize)
      {
         if (isStarted)
         {
            Debug.LogError(1, "The game is already started.");
            return;
         }
         isStarted = true;

         FileSystem.Initialize();
         Time.Initialize();
         Hardware.Initialize();
         Performance.Initialize();
         Window.Initialize(pixelSize);
         Mouse.Initialize();
         Assets.Initialize();
         Keyboard.Initialize();

         Window.PreventsSleep = true;

         game.OnGameCreate();
         Events.Notify(Event.GameStart);

         RunGame();
      }
      private static void RunGame()
      {
         while (Window.window.IsOpen)
         {
            if (Performance.Boost == false) Thread.Sleep(1);

            Performance.EarlyUpdate();

            Audio.Update();
            Hitbox.Update();
            Ropes.Update();
            Assets.Update();

            Keyboard.Update();
            Mouse.Update();

            Application.DoEvents();
            Window.window.DispatchEvents();
            Events.Notify(Event.GameUpdate);
            Events.Notify(Event.GameLateUpdate);
            Window.Draw();

            Components.Timer.Update();
            Performance.Update();
         }
      }
      public static void Stop()
      {
         Window.Close();
      }

      public abstract void OnGameCreate();

      internal static bool NotStartedError(int depth)
      {
         if (isStarted == false)
         {
            Debug.LogError(depth + 1, "The game has not been started yet.");
            return true;
         }
         return false;
      }
   }
   internal static class Statics
   {
      internal static bool TryCast<T>(this object obj, out T result)
      {
         if (obj is T t)
         {
            result = t;
            return true;
         }

         result = default;
         return false;
      }
   }
}
