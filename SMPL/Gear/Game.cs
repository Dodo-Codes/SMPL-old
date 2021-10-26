using SFML.Window;
using System;
using System.Windows.Forms;
using System.Threading;
using SMPL.Data;
using SMPL.Components;
using SMPL.Prefabs;
using System.Collections.Generic;
using System.Reflection;

namespace SMPL.Gear
{
   public abstract class Game : Thing
   {
      ///<summary>
      ///text, <paramref name="param"/>, <see cref="char"/>, <typeparamref name="Type"/>
      ///</summary>
      private static void Main() { }
      internal static bool isStarted;

      public static class Event
      {
         public static class Subscribe
         {
            public static void Start(string thingUID, uint order = uint.MaxValue) =>
               Events.Enable(Events.Type.GameStart, thingUID, order);
            public static void Update(string thingUID, uint order = uint.MaxValue) =>
               Events.Enable(Events.Type.GameUpdate, thingUID, order);
            public static void LateUpdate(string thingUID, uint order = uint.MaxValue) =>
               Events.Enable(Events.Type.GameLateUpdate, thingUID, order);
         }
         public static class Unsubscribe
         {
            public static void Start(string thingUID) => Events.Disable(Events.Type.GameStart, thingUID);
            public static void Update(string thingUID) => Events.Disable(Events.Type.GameUpdate, thingUID);
            public static void LateUpdate(string thingUID) => Events.Disable(Events.Type.GameLateUpdate, thingUID);
         }
      }

      public Game(string uniqueID) : base(uniqueID) { }
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
         Events.Notify(Events.Type.GameStart);

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
            Events.Notify(Events.Type.GameUpdate);
            Events.Notify(Events.Type.GameLateUpdate);
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
