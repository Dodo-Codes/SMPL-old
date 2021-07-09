using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

/// <summary>
/// An extension made to simplify the MonoGame framework.
/// </summary>
public abstract class Do : Game
{
	#region Sleep Prevention
	[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
	static extern EXECUTION_STATE SetThreadExecutionState(EXECUTION_STATE esFlags);
	[Flags]
	private enum EXECUTION_STATE : uint
	{
		ES_AWAYMODE_REQUIRED = 0x00000040,
		ES_CONTINUOUS = 0x80000000,
		ES_DISPLAY_REQUIRED = 0x00000002,
		ES_SYSTEM_REQUIRED = 0x00000001
		// Legacy flag, should not be used.
		// ES_USER_PRESENT = 0x00000004
	}
	#endregion

	#region Data
	private static bool playlistLoop, melodyPlayingLoop, inPlaylist, smoothRender, render, loading, debug, debugThingInfoVisible, sleepPrevented, pauseUnfocus;
	private static int fpsAverageIndex = 0, storageEncryption = 3, frame, tick, playlistIndex, frameRendered, debugRightIndex = 0, debugLeftIndex = 1;
	private static double melodyPlayingVolume, fpsAverage, deltaTime, time, playlistDelay, fps, debugRightY, debugLeftY;
	private static string title, version, directory = Path.GetDirectoryName($"{AppDomain.CurrentDomain.BaseDirectory}"), screenshotsPath = $"{AppDomain.CurrentDomain.BaseDirectory}\\screenshots", dir = AppDomain.CurrentDomain.BaseDirectory, storagePath = $"{dir}\\data.txt", thingStoragePath = $"{dir}\\data-obj.txt", storageSpace = "@", storageSeparatorStr = "#", thingStorageCompSep = "|", thingStorageValueSep = ",", thingStorageKeySep = "/", melodyPlayingName, playlistName, melodyOverKey = "melodyOver+-#;", debugFont, debugRightSelected, debugLeftSelected, sh = "=", sv = "||", sl = $"{sh}{sh}{sh}{sh}{sh}{sh}{sh}{sh}", debugThingSelected;
	private static Game.WindowTypes windowType = Game.WindowTypes.Windowed_TitleBar_TaskBar;
	private static Vector2 cameraPosition, debugScale;
	private static Color backgroundColor = Color.Black, debugColor = Color.White;
	private static DateTime lastFrameTime;
	private static Dictionary<string, List<Thing.Components>> thingComponents = new Dictionary<string, List<Thing.Components>>();
	private static Dictionary<string, List<string>> thingTags = new Dictionary<string, List<string>>(), tagThings = new Dictionary<string, List<string>>(), thingChildren = new Dictionary<string, List<string>>();
	private static Dictionary<double, List<string>> depthSortedThings = new Dictionary<double, List<string>>();
	private static Dictionary<string, List<Vector2>> thingPoints = new Dictionary<string, List<Vector2>>();
	private static Dictionary<string, List<bool>> thingParentRelatives = new Dictionary<string, List<bool>>();
	private static Dictionary<string, Color> thingSpriteColors = new Dictionary<string, Color>(), thingTextColors = new Dictionary<string, Color>(), thingSpriteFillColors = new Dictionary<string, Color>(), thingSpriteOutlineColors = new Dictionary<string, Color>();
	private static Dictionary<string, Vector2> thingParentSizeDifference = new Dictionary<string, Vector2>();
	private static Dictionary<string, int> thingCurrentAnimationIndex = new Dictionary<string, int>(), gateEntriesCount = new Dictionary<string, int>();
	private static Dictionary<string, double> thingAngles = new Dictionary<string, double>(), thingParentAngleDifference = new Dictionary<string, double>(), thingParentInDistance = new Dictionary<string, double>(), thingParentAtAngleDifference = new Dictionary<string, double>(), thingDepths = new Dictionary<string, double>(), thingCameraScrollPrevention = new Dictionary<string, double>(), thingCurrentAnimationProgress = new Dictionary<string, double>(), signalTimers = new Dictionary<string, double>(), signalStartTimes = new Dictionary<string, double>(), signalDelays = new Dictionary<string, double>(), thingTextLineHeights = new Dictionary<string, double>();
	private static Dictionary<string, Vector2> thingDirections = new Dictionary<string, Vector2>(), thingPositions = new Dictionary<string, Vector2>(), thingSizes = new Dictionary<string, Vector2>(), thingScales = new Dictionary<string, Vector2>(), thingOffsets = new Dictionary<string, Vector2>();
	private static Dictionary<string, string> thingTextFonts = new Dictionary<string, string>(), thingParents = new Dictionary<string, string>(), thingSprites = new Dictionary<string, string>(), thingTexts = new Dictionary<string, string>(), thingCurrentAnimationName = new Dictionary<string, string>(), storage = new Dictionary<string, string>(), thingStorage = new Dictionary<string, string>();
	private static Dictionary<string, bool> thingSpritesVisible = new Dictionary<string, bool>(), thingTextsVisible = new Dictionary<string, bool>(), thingOverlapsActivated = new Dictionary<string, bool>(), thingCurrentAnimationRunning = new Dictionary<string, bool>(), thingCurrentAnimationForward = new Dictionary<string, bool>(), gates = new Dictionary<string, bool>(), signalPauses = new Dictionary<string, bool>(), thingSpritesTiling = new Dictionary<string, bool>(), thingSpriteFillsVisible = new Dictionary<string, bool>(), thingSpriteOutlinesVisible = new Dictionary<string, bool>();
	private static List<string> thingUniqueNames = new List<string>(), thingsHoveredLastFrame = new List<string>(), thingsHovered = new List<string>(), thingsJustHovered = new List<string>(), thingsJustUnhovered = new List<string>(), thingsUpdatePoints = new List<string>(), debugThingInfo = new List<string>(), debugGlobalInfo = new List<string>();
	private static Dictionary<string, Thing.TextAnchor> thingTextAnchors = new Dictionary<string, Thing.TextAnchor>();
	private static Dictionary<string, Dictionary<object, object>> thingData = new Dictionary<string, Dictionary<object, object>>();
	private static Dictionary<string, Dictionary<string, List<string>>> thingAnimationSprites = new Dictionary<string, Dictionary<string, List<string>>>();
	private static Dictionary<string, Dictionary<string, double>> thingAnimationDurations = new Dictionary<string, Dictionary<string, double>>();
	private static Dictionary<string, Dictionary<string, Thing.AnimationRepeatStates>> thingAnimationRepeatStates = new Dictionary<string, Dictionary<string, Thing.AnimationRepeatStates>>();
	private static Dictionary<string, Dictionary<string, Dictionary<double[], int>>> thingAnimationChangesSpriteIndex = new Dictionary<string, Dictionary<string, Dictionary<double[], int>>>();
	private static Dictionary<string, Dictionary<string, Dictionary<double[], bool>>> thingAnimationChangesSpriteIndexTriggered = new Dictionary<string, Dictionary<string, Dictionary<double[], bool>>>();
	private static Dictionary<Thing.Components, List<string>> componentThings = new Dictionary<Thing.Components, List<string>>();
	private static Dictionary<string, Song> melodies = new Dictionary<string, Song>();
	private static Dictionary<string, SoundEffectInstance> sounds = new Dictionary<string, SoundEffectInstance>();
	private static Dictionary<string, SoundEffect> soundsRaw = new Dictionary<string, SoundEffect>();
	private static Dictionary<string, List<string>> playlists = new Dictionary<string, List<string>>();
	private static Dictionary<Direction.Directions, Vector2> directions = new Dictionary<Direction.Directions, Vector2>()
		{
				{ Direction.Directions.Up, new Vector2(0, -1) },
				{ Direction.Directions.Down, new Vector2(0, 1) },
				{ Direction.Directions.Left, new Vector2(-1, 0) },
				{ Direction.Directions.Right, new Vector2(1, 0) },
				{ Direction.Directions.Up_Left, new Vector2(-1, -1) },
				{ Direction.Directions.Up_Right, new Vector2(1, -1) },
				{ Direction.Directions.Down_Left, new Vector2(-1, 1) },
				{ Direction.Directions.Down_Right, new Vector2(1, 1) }
		};
	private static Dictionary<string, Texture2D> sprites = new Dictionary<string, Texture2D>(), spriteOutlines = new Dictionary<string, Texture2D>(), spriteFills = new Dictionary<string, Texture2D>();
	private static Dictionary<string, List<List<string>>> spriteGrids = new Dictionary<string, List<List<string>>>();
	private static Dictionary<string, SpriteFont> fonts = new Dictionary<string, SpriteFont>();
	private static Dictionary<object, object> data = new Dictionary<object, object>();
	private static List<double> fpsAverages = new List<double>();
	private static List<Input.Keys> lastFrameKeysPressed = new List<Input.Keys>(), keysJustPressed = new List<Input.Keys>(), keysJustReleased = new List<Input.Keys>();
	private static List<string> debugDefaultLinesThingInfo = new List<string>()
	{
		$"IDENTITY", $"2D SPACE", $"SPRITE", $"TEXT", $"DATA", $"FAMILY", $"OVERLAP", $"ANIMATION",
	};
	private static List<string> debugDefaultLinesGlobal = new List<string>()
	{
		$"DEBUG", $"GAME", $"CAMERA", $"THINGS", $"INPUT", $"DATA", $"SIGNALS", $"GATES", $"SPRITES", $"SPRITE GRIDS", $"FONTS", $"SOUNDS", $"MELODIES", $"PLAYLISTS",
	};
	private static List<string> debugDefaultLinesThings = new List<string>();
	#endregion
	#region Initialization
	private static Microsoft.Xna.Framework.Game game;
	private static Point screenSize;

	private class Game_Creation : Do
	{
		[STAThread]
		public static void Main()
		{
			using (var game = new Game_Creation())
			{
				game.Run();
			}
		}
		public override Do GameCreate() => this;
	}
	public Do()
	{
		graphics = new GraphicsDeviceManager(this);
		Content.RootDirectory = "Content";
		game = GameCreate();
	}
	protected override void Initialize()
	{
		painter = new SpriteBatch(game.GraphicsDevice);
		screenSize = new Point(Game.ResolutionDefaultWidth_Get(), Game.ResolutionDefaultHeight_Get());
		graphics.PreferredBackBufferWidth = screenSize.X;
		graphics.PreferredBackBufferHeight = screenSize.Y;
		graphics.ApplyChanges();

		Game.ResolutionToDefault_Set();
		Game.Title_Set("Game Title");
		Game.WindowTitle_Set("Window Title");
		Game.ComputerSleepPrevention_Activate(true);
		Input.MouseCursorVisibility_Activate(true);

		base.Initialize();
	}
	private static void LoadAllContent()
	{
		var directories = Directory.GetDirectories($"{directory}\\Content").ToList();
		for (int i = 0; i < directories.Count; i++)
		{
			LoadFolder(directories[i]);
		}
		LoadFolder($"{directory}\\Content");
	}
	private static void LoadFolder(string folder)
	{
		if (Directory.Exists(folder) == false)
		{
			return;
		}
		var path = folder.Split('\\');
		var result = "";
		var adding = false;
		for (int i = 0; i < path.Length; i++)
		{
			if (adding)
			{
				result = result.Insert(result.Length, path[i]);
				if (i != path.Length - 1)
				{
					result = result.Insert(result.Length, "\\");
				}
			}
			if (path[i] == "Content")
			{
				adding = true;
			}
		}

		var files = Directory.GetFiles(folder);
		for (int i = 0; i < files.Length; i++)
		{
			var split = files[i].Split('\\');
			try
			{
				LoadFile($"{result}\\{split[split.Length - 1]}");
			}
			catch (Exception)
			{
				continue;
			}
		}
		var currentDirectories = Directory.GetDirectories(folder).ToList();
		while (currentDirectories.Count > 0)
		{
			LoadFolder(currentDirectories[0]);
			currentDirectories.RemoveAt(0);
		}
	}
	private static void LoadFile(string name)
	{
		var split = name.Split('.');
		var key = split[0];
		key = key.Replace('\\', '/');
		if (key[0] == '/')
		{
			key = key.Remove(0, 1);
		}
		if (sprites.ContainsKey(key) || fonts.ContainsKey(key) || sounds.ContainsKey(key) || melodies.ContainsKey(key))
		{
			return;
		}
		switch (split[1])
		{
			case "png":
				{
					var sprite = game.Content.Load<Texture2D>(key);
					sprites[key] = sprite;

					var filled = new Texture2D(graphics.GraphicsDevice, sprites[key].Width, sprites[key].Height);
					var outline = new Texture2D(graphics.GraphicsDevice, sprites[key].Width, sprites[key].Height);
					var outlineData = new Color[sprite.Width * sprite.Height];
					var filledData = new Color[sprite.Width * sprite.Height];
					var pixels = new Color[sprite.Width * sprite.Height];

					sprite.GetData(pixels, 0, sprite.Width * sprite.Height);

					for (int i = 0; i < sprite.Height * sprite.Width; i++)
					{
						if (TransparentPixelHasNeighbourOpaquePixel(pixels, i, sprite.Width, sprite.Height))
						{
							outlineData[i] = Color.White;
						}
                        if (pixels[i] != Color.Transparent)
                        {
							filledData[i] = Color.White;
                        }
					}
					outline.SetData(outlineData);
					filled.SetData(filledData);

					spriteOutlines[key] = outline;
					spriteFills[key] = filled;

					break;
				}
			case "spritefont": fonts[key] = game.Content.Load<SpriteFont>(key); break;
			case "wav":
				{
					soundsRaw[key] = game.Content.Load<SoundEffect>(key);
					sounds[key] = soundsRaw[key].CreateInstance();
					break;
				}
			case "mp3": melodies[key] = game.Content.Load<Song>(key); break;
		}
		bool TransparentPixelHasNeighbourOpaquePixel(Color[] pixels, int i, int width, int height)
        {
			var y = i / height;
			var x = i % width;
			var xLeft = i - 1;
			var xRight = i + 1;
			var yUp = i - height;
			var yDown = i + height;

			if (pixels[i] == Color.Transparent)
            {
                if ((IndexIsOutOfBounds(x + 1, y - 1) == false && (pixels[xRight] != Color.Transparent || pixels[yUp] != Color.Transparent)) ||
					(IndexIsOutOfBounds(x - 1, y + 1) == false && (pixels[xLeft] != Color.Transparent || pixels[yDown] != Color.Transparent)) ||
					(IndexIsOutOfBounds(x + 1, y + 1) == false && (pixels[xRight] != Color.Transparent || pixels[yDown] != Color.Transparent)) ||
					(IndexIsOutOfBounds(x - 1, y - 1) == false && (pixels[xLeft] != Color.Transparent || pixels[yUp] != Color.Transparent)))
                {
					return true;
                }
            }
			return false;

			bool IndexIsOutOfBounds(int k, int l)
            {
                if (k < 0 || k > width - 1 || l < 0 || l > height - 1)
                {
					return true;
                }
				return false;
            }
        }
	}
	private static void SaveExportAll(bool isStorage)
	{
		try
		{
			var path = isStorage ? storagePath : thingStoragePath;
			var storage = isStorage ? Do.storage : thingStorage;
			File.Delete(path);
			using (StreamWriter stream = File.CreateText(path))
			{
				var j = 0;
				foreach (var kvp in storage)
				{
					Write(storageSpace);
					Write(kvp.Key);
					Write(storageSpace);
					Write(kvp.Value);
					j++;
					void Write(string write)
					{
						for (byte i = 0; i < write.Length; i++)
						{
							if (write == storageSpace)
							{
								stream.Write(storageSpace);
							}
							else
							{
								stream.Write(write[i] * (storageEncryption * (i + j + 1)));
								if (i != write.Length - 1)
								{
									stream.Write(storageSeparatorStr);
								}
							}
						}
					}
				}
			}
		}
		catch (Exception)
		{
			return;
		}
	}

	private static void SaveImportAll(bool isStorage)
	{
		try
		{
			var path = isStorage ? storagePath : thingStoragePath;
			using (StreamReader stream = File.OpenText(path))
			{
				var wholeFileStr = stream.ReadToEnd();
				var keysAndValuesSplit = wholeFileStr.Split(Convert.ToChar(storageSpace));
				var trimmedKeysAndValues = new List<string>();
				for (int i = 0; i < keysAndValuesSplit.Length; i++)
				{
					if (keysAndValuesSplit[i].Trim() == "")
					{
						continue;
					}
					trimmedKeysAndValues.Add(keysAndValuesSplit[i]);
				}
				var c = 0;
				for (int i = 0; i < trimmedKeysAndValues.Count; i++)
				{
					if (i % 2 != 0)
					{
						var newKeyTrimmed = trimmedKeysAndValues[i - 1].Split(Convert.ToChar(storageSeparatorStr));
						var newValueTrimmed = trimmedKeysAndValues[i].Split(Convert.ToChar(storageSeparatorStr));
						var keyResult = "";
						var valueResult = "";
						for (int j = 0; j < newKeyTrimmed.Length; j++)
						{
							var keyInt = Convert.ToInt32(newKeyTrimmed[j]) / (storageEncryption * (j + c + 1));
							keyResult = keyResult.Insert(keyResult.Length, Convert.ToChar(keyInt).ToString());
						}
						for (int j = 0; j < newValueTrimmed.Length; j++)
						{
							var valueInt = Convert.ToInt32(newValueTrimmed[j]) / (storageEncryption * (j + c + 1));
							valueResult = valueResult.Insert(valueResult.Length, Convert.ToChar(valueInt).ToString());
						}
						c++;
                        if (isStorage)
                        {
							storage.Add(keyResult, valueResult);
						}
                        else
                        {
							thingStorage.Add(keyResult, valueResult);
                        }
					}
				}
			}
		}
		catch (Exception)
		{
			return;
		}
	}
	#endregion
	#region Update
	public abstract Do GameCreate();

	protected override void Update(GameTime gameTime)
	{
        if (pauseUnfocus && game.IsActive == false)
        {
			return;
        }
		if (frame == 0)
		{
			loading = true;
			Game_Run.Loop();
		}
		else if (frame == 1)
		{
			try
			{
				SaveImportAll(true);
				SaveImportAll(false);
				LoadAllContent();
			}
			catch (Exception) { }
			loading = false;
			Game_Run.Loop();
		}
		else
		{
			UpdateOverlaps();
			AdvanceTime();
			UpdateOnKeys();
			CheckDebugInput();
			Game_Run.Loop();
			AdvanceAnimations();
			PlaylistsUpdate();
			base.Update(gameTime);
		}
		tick++;
	}
	private static void UpdateOnKeys()
	{
		var keysPressed = Input.KeysPressed_Get();

		keysJustPressed.Clear();
		keysJustReleased.Clear();
		foreach (var key in keysPressed)
		{
			if (lastFrameKeysPressed.Contains(key) == false)
			{
				keysJustPressed.Add(key);
			}
		}
		foreach (var key in lastFrameKeysPressed)
		{
			if (keysPressed.Contains(key) == false)
			{
				keysJustReleased.Add(key);
			}
		}

		lastFrameKeysPressed = Input.KeysPressed_Get();
	}
	private static void CheckDebugInput()
	{
		var key = "system-debug-key-num-";
		var leftSide = true;
		var updatesPerSec = deltaTime * fps / 30;
		updatesPerSec *= Input.KeyIsPressed_Check(Input.Keys.NumSubtract) ? 3 : 1;
		var fast = Input.KeyIsPressed_Check(Input.Keys.NumAdd);
		var speedUp = false;
		speedUp = updatesPerSec < deltaTime ? true : speedUp;

		if (Input.KeysJustPressed_Get().Contains(Input.Keys.Num5))
		{
			if (debugLeftSelected.Contains("Tag") == false || debugLeftSelected.Contains("UniqueName") == false)
			{
				if (debugThingInfoVisible == false)
				{
					return;
				}
				GoToAllThings();
				return;
			}
			GoToSpecificThing();
		}
		if (Input.KeysJustPressed_Get().Contains(Input.Keys.Delete))
		{
			Thing.Delete(debugThingSelected);
			GoToAllThings();
		}

		if (Input.PressIntoHold_Check($"{key}8", Input.KeyIsPressed_Check(Input.Keys.Num8), updateEveryTick: speedUp, updatesPerSecond: updatesPerSec)) ChangeData(true);
		if (Input.PressIntoHold_Check($"{key}4", Input.KeyIsPressed_Check(Input.Keys.Num4), updateEveryTick: speedUp, updatesPerSecond: updatesPerSec)) ChangeData(true);
		if (Input.PressIntoHold_Check($"{key}6", Input.KeyIsPressed_Check(Input.Keys.Num6), updateEveryTick: speedUp, updatesPerSecond: updatesPerSec)) ChangeData(true);
		if (Input.PressIntoHold_Check($"{key}2", Input.KeyIsPressed_Check(Input.Keys.Num2), updateEveryTick: speedUp, updatesPerSecond: updatesPerSec)) ChangeData(true);

		if (Input.PressIntoHold_Check($"{key}7", Input.KeyIsPressed_Check(Input.Keys.Num7), updateEveryTick: speedUp, updatesPerSecond: updatesPerSec)) ChangeData(false);
		if (Input.PressIntoHold_Check($"{key}1", Input.KeyIsPressed_Check(Input.Keys.Num1), updateEveryTick: speedUp, updatesPerSecond: updatesPerSec)) ChangeData(false);
		if (Input.PressIntoHold_Check($"{key}9", Input.KeyIsPressed_Check(Input.Keys.Num9), updateEveryTick: speedUp, updatesPerSecond: updatesPerSec)) ChangeData(false);
		if (Input.PressIntoHold_Check($"{key}3", Input.KeyIsPressed_Check(Input.Keys.Num3), updateEveryTick: speedUp, updatesPerSecond: updatesPerSec)) ChangeData(false);

		void ChangeData(bool edit)
		{
			var selected = leftSide ? debugLeftSelected : debugRightSelected;
			if (Input.KeysPressed_Get().Count == 0 || selected == null)
			{
				return;
			}
			var uniqueName = debugThingSelected;

			if (edit)
			{
                if (debugThingInfoVisible == false && debugThingSelected != null && debugThingSelected != "")
                {
					Thing.Duplicate_Create(debugThingSelected, $"{debugThingSelected}-{Game.FrameCount_Get()}");
					return;
                }
				var editableVectors = new Dictionary<string, Vector2>()
				{
					{ "ThingPosition",  Thing.Position_Get(uniqueName) },
					{ "ThingSize",  Thing.Size_Get(uniqueName) },
					{ "ThingScale",  Thing.Scale_Get(uniqueName) },
					{ "DebugScale",  debugScale },
					{ "GameResolution", new Vector2(Game.ResolutionWidth_Get(), Game.ResolutionHeight_Get()) },
					{ "CameraPosition", Camera.Position_Get() },
				};
				var editableValues = new Dictionary<string, double>()
			{
				{ "ThingAngle",  Thing.Angle_Get(uniqueName) },
				{ "ThingCameraScrollPrevention",  Thing.CameraScrollPreventionPercent_Get(uniqueName) },
				{ "ThingDepth",  Thing.Depth_Get(uniqueName) },
			};
				var editableInts = new Dictionary<string, int>()
				{

				};
				var editableBools = new Dictionary<string, bool>()
				{
					{ "ThingSpriteVisibility",  Thing.SpriteVisibility_IsVisible_Check(uniqueName) },
					{ "ThingSpriteTiling",  Thing.SpriteTiling_IsTiled_Check(uniqueName) },
					{ "HotkeyExitActivated", Game.HotkeyExitIsActivated_Check() },
					{ "PixelSmoothness", Game.PixelSmoothnessIsActivated_Check() },
					{ "ComputerSleepPrevention", Game.ComputerSleepPreventionIsActivated_Check() },
				};
				var speed = 1;
				speed *= fast ? 10 : 1;
				var speedX = 0f;
				var speedY = 0f;
				if (Input.KeyIsPressed_Check(Input.Keys.Num8)) speedY = -speed;
				if (Input.KeyIsPressed_Check(Input.Keys.Num6)) speedX = speed;
				if (Input.KeyIsPressed_Check(Input.Keys.Num4)) speedX = -speed;
				if (Input.KeyIsPressed_Check(Input.Keys.Num2)) speedY = speed;

				var editComponent = FindComponent(editableVectors);
				if (editComponent == "") editComponent = FindComponent(editableValues);
				if (editComponent == "") editComponent = FindComponent(editableBools);
				if (editComponent == "") editComponent = FindComponent(editableInts);
				if (editableVectors.ContainsKey(editComponent)) editableVectors[editComponent] = new Vector2(editableVectors[editComponent].X + speedX, editableVectors[editComponent].Y + speedY);
				if (editableValues.ContainsKey(editComponent)) editableValues[editComponent] = editableValues[editComponent] + speedX;
				if (editableInts.ContainsKey(editComponent)) editableInts[editComponent] = editableInts[editComponent] + (int)speedX;
				if (editableInts.ContainsKey(editComponent)) editableInts[editComponent] = editableInts[editComponent] + (int)speedX;
				if (editableBools.ContainsKey(editComponent))
				{
					editableBools[editComponent] = editableBools[editComponent] == false;
				}

				if (selected.Contains("ThingPosition")) Thing.Position_Set(uniqueName, editableVectors["ThingPosition"].X, editableVectors["ThingPosition"].Y);
				else if (selected.Contains("ThingSize")) Thing.Size_Set(uniqueName, editableVectors["ThingSize"].X, editableVectors["ThingSize"].Y);
				else if (selected.Contains("ThingScale")) Thing.Scale_Set(uniqueName, editableVectors["ThingScale"].X, editableVectors["ThingScale"].Y);
				else if (selected.Contains("ThingAngle")) Thing.Angle_Set(uniqueName, editableValues["ThingAngle"]);
				else if (selected.Contains("ThingCameraScrollPrevention")) Thing.CameraScrollPreventionPercent_Set(uniqueName, editableValues["ThingCameraScrollPrevention"]);
				else if (selected.Contains("ThingDepth")) Thing.Depth_Set(uniqueName, editableValues["ThingDepth"]);
				else if (selected.Contains("ThingSpriteVisibility")) Thing.SpriteVisibility_Activate(uniqueName, editableBools["ThingSpriteVisibility"]);
				else if (selected.Contains("ThingSpriteTiling")) Thing.SpriteTiling_Activate(uniqueName, editableBools["ThingSpriteTiling"]);
				else if (selected.Contains("DebugScale")) debugScale = editableVectors["DebugScale"];
				else if (selected.Contains("GameResolution")) Game.Resolution_Set((int)Number.Rounded_Get(editableVectors["GameResolution"].X, 0, Number.RoundType.Closest), (int)Number.Rounded_Get(editableVectors["GameResolution"].Y, 0, Number.RoundType.Closest), windowType);
				else if (selected.Contains("HotkeyExitActivated")) Game.HotkeyExit_Activate(editableBools["HotkeyExitActivated"]);
				else if (selected.Contains("PixelSmoothness")) Game.PixelSmoothness_Activate(editableBools["PixelSmoothness"]);
				else if (selected.Contains("CameraPosition")) Camera.Position_Set(editableVectors["CameraPosition"]);
				else if (selected.Contains("ComputerSleepPrevention")) Game.ComputerSleepPrevention_Activate(editableBools["ComputerSleepPrevention"]);

				string FindComponent<T>(Dictionary<string, T> editables)
				{
					foreach (var kvp in editables)
					{
						if (selected.Contains(kvp.Key))
						{
							var isScale = kvp.Key.Contains("Scale");
							speedX = isScale ? speedX / 20 : speedX;
							speedY = isScale ? speedY / 20 : speedY;

							return kvp.Key;
						}
					}
					return "";
				}
			}
			else
			{
				if (Input.KeyIsPressed_Check(Input.Keys.Num9))
				{
					if (debugRightSelected != null && debugRightIndex < 1)
					{
						return;
					}
					if (debugRightSelected == null)
					{
						debugRightIndex = 1;
					}
					else
					{
						debugRightIndex -= 1;
					}
					debugRightSelected = debugThingInfoVisible ? debugDefaultLinesThingInfo[debugRightIndex] : debugDefaultLinesGlobal[debugRightIndex];

					debugLeftIndex = 1;
					debugLeftSelected = null;

					leftSide = false;
					SelectDebugThing();
					render = true;
				}
				if (Input.KeyIsPressed_Check(Input.Keys.Num3))
				{
					var rightInfo = debugThingInfoVisible ? debugDefaultLinesThingInfo : debugDefaultLinesGlobal;
					if (debugRightSelected != null && debugRightIndex > rightInfo.Count - 2)
					{
						return;
					}
					if (debugRightSelected == null)
					{
						debugRightIndex = 1;
					}
					else
					{
						debugRightIndex += 1;
					}
					debugRightSelected = rightInfo[debugRightIndex];

					debugLeftIndex = 1;
					debugLeftSelected = null;

					leftSide = false;
					SelectDebugThing();
					render = true;
				}
				if (Input.KeyIsPressed_Check(Input.Keys.Num7))
				{
					if (debugLeftSelected == null || debugLeftIndex < 2)
					{
						return;
					}

					debugLeftIndex -= 1;
					debugLeftSelected = debugThingInfoVisible ? debugThingInfo[debugLeftIndex] : debugGlobalInfo[debugLeftIndex];

					leftSide = true;
					render = true;
					DebugInfoIndexJump(false, true);
					SelectDebugThing();
				}
				if (Input.KeyIsPressed_Check(Input.Keys.Num1))
				{
					var leftInfo = debugThingInfoVisible ? debugThingInfo : debugGlobalInfo;
					if (debugLeftSelected == null || debugLeftIndex > leftInfo.Count - 3)
					{
						return;
					}

					debugLeftIndex += 1;
					debugLeftSelected = leftInfo[debugLeftIndex];

					leftSide = true;
					render = true;
					DebugInfoIndexJump(true, true);
					SelectDebugThing();
				}
			}
		}
		void GoToAllThings()
        {
			debugLeftIndex = 1;
			debugRightIndex = 3;
			debugRightSelected = debugDefaultLinesGlobal[debugRightIndex];
			debugThingSelected = null;
			debugThingInfoVisible = false;
		}
		void GoToSpecificThing()
        {
			debugThingInfoVisible = true;
			debugRightIndex = 0;
			debugLeftIndex = 1;
		}
	}
	private static void DebugInfoIndexJump(bool down, bool controls)
	{
		var jump = 0;
		var jumpOverLines = new List<string>();
		jumpOverLines.AddRange(debugDefaultLinesGlobal);
		jumpOverLines.AddRange(debugDefaultLinesThingInfo);
		jumpOverLines.AddRange(debugDefaultLinesThings);
		jumpOverLines.Add("");
		jumpOverLines.Add($"{sv}");
		var newLeftSelected = GetLeftSelected();

		var leftInfo = debugThingInfoVisible ? debugThingInfo : debugGlobalInfo;
		if (leftInfo == null || newLeftSelected == null)
		{
			return;
		}
		if (controls && jumpOverLines.Contains(newLeftSelected))
		{
			jump = newLeftSelected == $"" ? 1 : 3;
		}
		else if (controls == false && jumpOverLines.Contains(newLeftSelected))
		{
			for (int i = 1; i < 5; i++)
			{
				var indexIsInMinusRange = debugLeftIndex - i > 1 && debugLeftIndex - i < leftInfo.Count - 2;
				var indexIsInPlusRange = debugLeftIndex + i > 1 && debugLeftIndex + i < leftInfo.Count - 2;
				jump = i;
				if (indexIsInMinusRange && jumpOverLines.Contains(leftInfo[debugLeftIndex - i]) == false)
				{
					down = false;
					break;
				}
				if (indexIsInPlusRange && jumpOverLines.Contains(leftInfo[debugLeftIndex + i]) == false)
				{
					down = true;
					break;
				}
			}
		}

		debugLeftIndex += down ? jump : -jump;
		debugLeftSelected = leftInfo[debugLeftIndex];
	}
	private static void UpdateOverlaps()
	{
		thingsHovered = new List<string>();
		thingsJustUnhovered = new List<string>();
		thingsJustHovered = new List<string>();
		var thingOverlaps = Thing.Component_All_UniqueNames_Get(Thing.Components.Overlap);
		foreach (var thing in thingOverlaps)
		{
			if (thingsUpdatePoints.Contains(thing))
			{
				PointsUpdate(thing);
			}
			var isHovered = Thing.Overlap_IsHoveredByMouseCursor_Check(thing);
			var wasHovered = thingsHoveredLastFrame.Contains(thing) && isHovered == false;
			var justHovered = thingsHoveredLastFrame.Contains(thing) == false && isHovered;
			if (isHovered)
			{
				thingsHovered.Add(thing);
			}
			if (wasHovered)
			{
				thingsJustUnhovered.Add(thing);
			}
			if (justHovered)
			{
				thingsJustHovered.Add(thing);
			}
		}
		thingsHoveredLastFrame = new List<string>(thingsHovered);
		thingsUpdatePoints = new List<string>();
	}
	private static void AdvanceTime()
	{
		var delta = lastFrameTime == default ? default : DateTime.Now - lastFrameTime;
		deltaTime = delta.TotalSeconds;
		fps = 60 / (delta.TotalSeconds * 60);
		fps = double.IsInfinity(fps) ? fpsAverage : fps;
		AdvanceFPSAverage();
		time += deltaTime;
		lastFrameTime = DateTime.Now;

		AdvanceSignalTimers();
	}
	private static void AdvanceFPSAverage()
	{
		if (fpsAverageIndex == 60)
		{
			fpsAverageIndex = 0;
		}
		if (fpsAverages.Contains(fps))
		{
			return;
		}
		if (fpsAverageIndex == fpsAverages.Count)
		{
			fpsAverages.Add(fps);
		}
		fpsAverages[fpsAverageIndex] = fps;
		fpsAverage = fpsAverages.Average();
		fpsAverageIndex++;
	}
	private static void AdvanceSignalTimers()
	{
		foreach (var kvp in signalTimers.ToArray())
		{
			signalTimers[kvp.Key] += deltaTime;
		}
	}
	private static void PlaylistsUpdate()
	{
		if (Signal.IsOccuring_Check(melodyOverKey, false))
		{
			var looping = Melody.CurrentIsLooping_Check();
			var volume = Melody.CurrentVolume_Get();
			var song = Melody.CurrentName_Get();
			if (looping)
			{
				Melody.Play(song, volume, true);
			}

			if (playlistName == default)
			{
				return;
			}
			if (playlistIndex == playlists[playlistName].Count - 1)
			{
				playlistIndex = 0;
				if (playlistLoop == false)
				{
					return;
				}
			}
			playlistIndex++;
			inPlaylist = true;
			Melody.Play(playlists[playlistName][playlistIndex], volume, false);
			inPlaylist = false;
		}
	}
	private static void AdvanceAnimations()
	{
		foreach (var kvp in thingAnimationDurations)
		{
			var uniqueName = kvp.Key;
			var animationName = Thing.Animation_CurrentName_Get(uniqueName);
			if (animationName == null)
			{
				continue;
			}
			var duration = Thing.Animation_Duration_Get(uniqueName, animationName);
			if (thingCurrentAnimationRunning[uniqueName] == false || duration == 0)
			{
				return;
			}
			var repeatState = Thing.Animation_RepeatState_Get(uniqueName, animationName);
			var currAnimForward = Thing.Animation_CurrentIsGoingForward_Check(uniqueName);

			if (thingAnimationChangesSpriteIndex.ContainsKey(uniqueName) == false ||
				thingAnimationChangesSpriteIndex[uniqueName].ContainsKey(animationName) == false)
			{
				continue;
			}
			thingCurrentAnimationProgress[uniqueName] += currAnimForward ? deltaTime : -deltaTime;
			var currAnimProgress = thingCurrentAnimationProgress[uniqueName];
			var frameIndexChange = -1;
			foreach (var kvp2 in thingAnimationChangesSpriteIndex[uniqueName][animationName])
			{
				if (currAnimProgress > kvp2.Key[0] && currAnimProgress < kvp2.Key[1] &&
					thingAnimationChangesSpriteIndexTriggered[uniqueName][animationName][kvp2.Key] == false)
				{
					thingAnimationChangesSpriteIndexTriggered[uniqueName][animationName][kvp2.Key] = true;
					frameIndexChange = kvp2.Value;
					break;
				}
			}
			thingCurrentAnimationIndex[uniqueName] = frameIndexChange != -1 ? frameIndexChange : thingCurrentAnimationIndex[uniqueName];

			if (currAnimProgress > duration)
			{
				RestartAllTriggers(uniqueName, animationName);
				if (repeatState == Thing.AnimationRepeatStates.None)
				{
					thingCurrentAnimationProgress[uniqueName] = duration;
					thingCurrentAnimationRunning[uniqueName] = false;
				}
				else if (repeatState == Thing.AnimationRepeatStates.Loop)
				{
					thingCurrentAnimationProgress[uniqueName] = 0;
				}
				else if (repeatState == Thing.AnimationRepeatStates.Mirror)
				{
					thingCurrentAnimationForward[uniqueName] = false;
				}
			}
			else if (currAnimProgress < 0)
			{
				thingCurrentAnimationProgress[uniqueName] = 0;
				thingCurrentAnimationForward[uniqueName] = true;
				RestartAllTriggers(uniqueName, animationName);
			}
			Thing.Sprite_Set(uniqueName, Thing.Animation_SpriteName_Get(uniqueName, animationName, Thing.Animation_CurrentSpriteIndex_Get(uniqueName)));
		}
	}
	private static void ChildrenUpdate(string parentUniqueName, bool position, bool angle, bool size)
	{
		if (thingChildren.ContainsKey(parentUniqueName) == false)
		{
			return;
		}
		foreach (var child in thingChildren[parentUniqueName])
		{
			if (thingParentRelatives[child][0] && position)
			{
				Thing.Position_Set(child, Thing.Position_Get(parentUniqueName));
				var targetAngle = Thing.Angle_Get(parentUniqueName) - thingParentAtAngleDifference[child];
				var speed = thingParentInDistance[child] / deltaTime;
				var targetPosition = Position.MovedAtAngle_Get(Thing.Position_Get(child), targetAngle, speed);
				Thing.Position_Set(child, targetPosition);
			}
			if (thingParentRelatives[child][1] && angle)
			{
				Thing.Angle_Set(child, Thing.Angle_Get(parentUniqueName) - thingParentAngleDifference[child]);
			}
			if (thingParentRelatives[child][2] && size)
			{
				Thing.Size_Set(child, Thing.Size_Get(parentUniqueName) - thingParentSizeDifference[child]);
			}
		}
	}
	private static void PointsUpdate(string uniqueName)
	{
		if (thingPoints.ContainsKey(uniqueName) == false)
		{
			thingPoints[uniqueName] = new List<Vector2>();
			for (int i = 0; i < 4; i++)
			{
				thingPoints[uniqueName].Add(default);
			}
		}

		var camParallax = 1 - (float)Thing.CameraScrollPreventionPercent_Get(uniqueName) / 100;
		var camPos = Camera.Position_Get() * camParallax;
		var scale = Thing.Scale_Get(uniqueName);
		var offset = Thing.OriginOffset_Get(uniqueName) * scale;
		var angle = Thing.Angle_Get(uniqueName);
		var size = Thing.Size_Get(uniqueName);
		var pos = Thing.Position_Get(uniqueName);
		var angleFromOffset = Angle.FromDirection_Get(Direction.BetweenPositions_Get(pos, pos - offset)) + angle;
		var distanceFromOffset = (float)Position.DistanceToPosition_Get(pos, pos - offset);
		var posOffset = Direction.FromAngle_Get(angleFromOffset) * distanceFromOffset;
		var topLeft = pos + posOffset;
		var toRight = Direction.FromAngle_Get(0 + angle);
		var toDown = Direction.FromAngle_Get(90 + angle);

		var topRight = topLeft + toRight * size.X;
		thingPoints[uniqueName][0] = topLeft - camPos;

		thingPoints[uniqueName][1] = topRight - camPos;
		thingPoints[uniqueName][2] = topLeft + toDown * size.Y - camPos;
		thingPoints[uniqueName][3] = topRight + toDown * size.Y - camPos;
	}
	private static void RestartAllTriggers(string uniqueName, string animationName)
	{
		if (Thing.Exists_Check(uniqueName) == false || animationName == null || thingAnimationChangesSpriteIndexTriggered.ContainsKey(uniqueName) == false ||
			thingAnimationChangesSpriteIndexTriggered[uniqueName].ContainsKey(animationName) == false)
		{
			return;
		}
		var frameIndexKeys = new List<double[]>();
		foreach (var kvp3 in thingAnimationChangesSpriteIndexTriggered[uniqueName][animationName])
		{
			frameIndexKeys.Add(kvp3.Key);
		}
		for (int i = 0; i < frameIndexKeys.Count; i++)
		{
			thingAnimationChangesSpriteIndexTriggered[uniqueName][animationName][frameIndexKeys[i]] = false;
		}
	}
	private static void SelectDebugThing()
    {
        if (debugThingInfoVisible)
        {
			return;
        }
		if (debugLeftSelected == null || debugLeftSelected.Contains("Tag") == false || debugLeftSelected.Contains("UniqueName") == false)
        {
			debugThingSelected = null;
			return;
        }
		var selected = GetLeftSelected();
		selected = selected.Split(':')[1];
		selected = selected.Replace("\"", "");
		selected = selected.Replace("}", "");
		debugThingSelected = selected;
	}
	#endregion
	#region Draw
	private static GraphicsDeviceManager graphics;
	private static SpriteBatch painter;
	private static RenderTarget2D renderTarget;

	protected override void Draw(GameTime gameTime)
	{
		frame++;
		if (render == false && debug == false)
		{
			return;
		}
		if (render)
		{
			frameRendered++;
		}
		var samplerState = Game.PixelSmoothnessIsActivated_Check() ? null : SamplerState.PointWrap;

		painter.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, samplerState, DepthStencilState.Default, RasterizerState.CullNone);
		GraphicsDevice.SetRenderTarget(renderTarget);
		GraphicsDevice.DepthStencilState = new DepthStencilState() { DepthBufferEnable = true };

		GraphicsDevice.Clear(backgroundColor);
		DrawAllThings();
		DrawDebugIndicators();
		GraphicsDevice.SetRenderTarget(null);
		var def = new Vector2(Game.ResolutionDefaultWidth_Get(), Game.ResolutionDefaultHeight_Get());
		var scale = new Vector2(def.X / screenSize.X, def.Y / screenSize.Y);
		painter.Draw(renderTarget, Vector2.Zero, new Rectangle(0, 0, (int)def.X + 200, (int)def.Y + 200), Color.White, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
		DrawDebug();
		render = false;
		painter.End();
		base.Draw(gameTime);
	}
	private static void DrawAllThings()
	{
		var depths = DictionaryKeysGet(depthSortedThings);
		var camPos = Camera.Position_Get();
		depths.Sort();
		depths.Reverse();
		for (int i = 0; i < depths.Count; i++)
		{
			if (depthSortedThings.ContainsKey(depths[i]))
			{
				for (int j = 0; j < depthSortedThings[depths[i]].Count; j++)
				{
					var uniqueName = depthSortedThings[depths[i]][j];
					var c = Thing.SpriteColor_Get(uniqueName);
					var colorOutline = Thing.SpriteOutlineColor_Get(uniqueName);
					var colorFilled = Thing.SpriteFillColor_Get(uniqueName);
					var rad = (float)Math.PI / 180 * (float)Thing.Angle_Get(uniqueName);
					var pos = Thing.Position_Get(uniqueName);
					var scale = Thing.Scale_Get(uniqueName);
					var origin = Thing.OriginOffset_Get(uniqueName) / scale;
					var camParallax = 1 - (float)Thing.CameraScrollPreventionPercent_Get(uniqueName) / 100;

					if (Thing.Sprite_Get(uniqueName) != null)
					{
						var sprite = sprites[Thing.Sprite_Get(uniqueName)];
						var spriteOutline = spriteOutlines[Thing.Sprite_Get(uniqueName)];
						var spriteFilled = spriteFills[Thing.Sprite_Get(uniqueName)];
						var size = Thing.Size_Get(uniqueName);
						var spriteTiling = Thing.SpriteTiling_IsTiled_Check(uniqueName);
						var rect = new Rectangle(0, 0, (int)size.X, (int)size.Y);
						if (spriteTiling)
						{
                            if (Thing.SpriteVisibility_IsVisible_Check(uniqueName))
                            {
								painter.Draw(sprite, pos - camPos * camParallax, rect, c, rad, origin, Vector2.One, SpriteEffects.None, 0);
							}
                            if (Thing.SpriteFillVisibility_IsVisible_Check(uniqueName))
                            {
								painter.Draw(spriteFilled, pos - camPos * camParallax, rect, colorFilled, rad, origin, Vector2.One, SpriteEffects.None, 0);
							}
                            if (Thing.SpriteOutlineVisibility_IsVisible_Check(uniqueName))
                            {
								painter.Draw(spriteOutline, pos - camPos * camParallax, rect, colorOutline, rad, origin, Vector2.One, SpriteEffects.None, 0);
							}
						}
						else
						{
							if (Thing.SpriteVisibility_IsVisible_Check(uniqueName))
							{
								painter.Draw(sprite, pos - camPos * camParallax, null, c, rad, origin, scale, SpriteEffects.None, 0);
							}
							if (Thing.SpriteFillVisibility_IsVisible_Check(uniqueName))
							{
								painter.Draw(spriteFilled, pos - camPos * camParallax, null, colorFilled, rad, origin, scale, SpriteEffects.None, 0);
							}
							if (Thing.SpriteOutlineVisibility_IsVisible_Check(uniqueName))
							{
								painter.Draw(spriteOutline, pos - camPos * camParallax, null, colorOutline, rad, origin, scale, SpriteEffects.None, 0);
							}
						}
					}
					if (Thing.TextFont_Get(uniqueName) != null && Thing.TextVisibility_IsVisible_Check(uniqueName) && Thing.Text_Get(uniqueName) != null)
					{
						var font = Thing.TextFont_Get(uniqueName);
						var anchor = Thing.TextAnchor_Get(uniqueName);
						var message = ValidateString(Thing.Text_Get(uniqueName));
						var messageLines = message.Split('\n');
						var lineHeight = (float)Thing.TextLineHeight_Get(uniqueName);
						var textColor = Thing.TextColor_Get(uniqueName);
						var y = 0f;
						if (anchor == Thing.TextAnchor.BottomLeft || anchor == Thing.TextAnchor.BottomCenter || anchor == Thing.TextAnchor.BottomRight)
						{
							y = messageLines.Length * lineHeight - lineHeight * 2;
						}
						else if (anchor == Thing.TextAnchor.CenterLeft || anchor == Thing.TextAnchor.Center || anchor == Thing.TextAnchor.CenterRight)
						{
							y = messageLines.Length * lineHeight / 2 - lineHeight;
						}
						foreach (var line in messageLines)
						{
							var newOrigin = origin + GetAnchorOffset(font, line, anchor);
							newOrigin.Y += y;
							painter.DrawString(fonts[font], line, pos - camPos * camParallax, textColor, rad, newOrigin, scale, SpriteEffects.None, 0);
							y -= lineHeight;
						}
					}
				}
			}
		}
	}
	private static void DrawDebugIndicators()
	{
		if (debugThingSelected == null)
		{
			return;
		}
		var camPos = Camera.Position_Get();
		var uniqueName = debugThingSelected;
		var spriteOutline = Thing.Sprite_Get(uniqueName) != null ? spriteOutlines[Thing.Sprite_Get(uniqueName)] : null;
		var spriteFill = Thing.Sprite_Get(uniqueName) != null ? spriteFills[Thing.Sprite_Get(uniqueName)] : null;
		var font = Thing.TextFont_Get(uniqueName) != null ? fonts[Thing.TextFont_Get(uniqueName)] : null;
		var text = Thing.Text_Get(uniqueName);
		var rad = (float)Math.PI / 180 * (float)Thing.Angle_Get(uniqueName);
		var scale = Thing.Scale_Get(uniqueName);
		var origin = Thing.OriginOffset_Get(uniqueName) / scale;
		var camParallax = 1 - (float)Thing.CameraScrollPreventionPercent_Get(uniqueName) / 100;
		var pos = Thing.Position_Get(uniqueName);

		if (spriteOutline != null)
		{
			var color = debugColor;
			color.A = 255;
			var size = Thing.Size_Get(uniqueName);
			var spriteTiling = Thing.SpriteTiling_IsTiled_Check(uniqueName);
			var rect = new Rectangle(0, 0, (int)size.X, (int)size.Y);

			if (spriteTiling)
			{
				painter.Draw(spriteOutline, pos - camPos * camParallax, rect, color, rad, origin, Vector2.One, SpriteEffects.None, 0);
				color = new Color(255 - color.R, 255 - color.G, 255 - color.B, 100);
				painter.Draw(spriteFill, pos - camPos * camParallax, rect, color, rad, origin, Vector2.One, SpriteEffects.None, 0);
			}
			else
			{
				painter.Draw(spriteOutline, pos - camPos * camParallax, null, color, rad, origin, scale, SpriteEffects.None, 0);
				color = new Color(255 - color.R, 255 - color.G, 255 - color.B, 100);
				painter.Draw(spriteFill, pos - camPos * camParallax, null, color, rad, origin, scale, SpriteEffects.None, 0);
			}
		}
		if (font != null && text != null)
		{
			var message = ValidateString(Thing.Text_Get(uniqueName));
			var messageLines = message.Split('\n');
			var lineHeight = (float)Thing.TextLineHeight_Get(uniqueName);
			var anchor = Thing.TextAnchor_Get(uniqueName);
			var y = 0f;
			if (anchor == Thing.TextAnchor.BottomLeft || anchor == Thing.TextAnchor.BottomCenter || anchor == Thing.TextAnchor.BottomRight)
			{
				y = messageLines.Length * lineHeight - lineHeight * 2;
			}
			else if (anchor == Thing.TextAnchor.CenterLeft || anchor == Thing.TextAnchor.Center || anchor == Thing.TextAnchor.CenterRight)
			{
				y = messageLines.Length * lineHeight / 2 - lineHeight;
			}
			foreach (var line in messageLines)
			{
				var newOrigin = origin + GetAnchorOffset(Thing.TextFont_Get(uniqueName), line, anchor);
				var visible = Thing.TextVisibility_IsVisible_Check(uniqueName);
				var color = debugColor;
				color.A = visible ? (byte)255 : (byte)130;

				newOrigin.Y += y;
				painter.DrawString(font, line, pos - camPos * camParallax, color, rad, newOrigin, scale, SpriteEffects.None, 0);
				y -= lineHeight;
			}
		}
	}
	private static void DrawDebug()
	{
		if (debug == false || debugFont == default)
		{
			return;
		}
		var runtime = Game.Runtime_Get();
		var seconds = runtime % 60;
		var secondsFloor = (int)seconds;
		var minutes = (int)runtime / 60 % 60;
		var hours = (int)runtime / 60 / 60;
		var ms = $"{(seconds - secondsFloor) * 1000:F0}";
		var x = Game.ResolutionDefaultWidth_Get();
		var y = Game.ResolutionDefaultHeight_Get();
		var lineHeight = GetDebugLineHeight();
		var rightLines = debugThingInfoVisible ? debugDefaultLinesThingInfo : debugDefaultLinesGlobal;
		var newLeftSelected = GetLeftSelected();
		var newRightSelected = GetRightSelected();

		debugRightY = lineHeight * -debugRightIndex;
		debugLeftY = lineHeight * -debugLeftIndex;

		debugThingInfo = new List<string>();
		debugGlobalInfo = new List<string>();
		debugDefaultLinesThings = new List<string>();
		if (debugThingInfoVisible && debugDefaultLinesThingInfo.Contains(newRightSelected))
		{
			debugThingInfo.Add($"{sv}{sh}{sh} {debugDefaultLinesThingInfo[debugRightIndex]} {sl}{sl}");
			var components = (Thing.Components[])Enum.GetValues(typeof(Thing.Components));
			for (int i = 0; i < components.Length; i++)
			{
				var component = components[i];
				var uniqueName = debugThingSelected;
				var k = $"{sv} Thing{component}";
				var identityIndex = 0; var spaceIndex = 1; var spriteIndex = 2; var textIndex = 3; var dataIndex = 4; var familyIndex = 5; var overlapIndex = 6; var animationIndex = 7;

				if (identityIndex != debugRightIndex && (component == Thing.Components.UniqueName || component == Thing.Components.Tags)) continue;
				if (spaceIndex != debugRightIndex && (component == Thing.Components.Position || component == Thing.Components.Size || component == Thing.Components.Scale || component == Thing.Components.Angle || component == Thing.Components.Direction || component == Thing.Components.Depth || component == Thing.Components.CameraScrollPrevention || component == Thing.Components.OriginOffset)) continue;
				if (spriteIndex != debugRightIndex && (component == Thing.Components.SpriteTiling || component == Thing.Components.SpriteColor || component == Thing.Components.SpriteTiling || component == Thing.Components.SpriteVisibility || component == Thing.Components.Sprite)) continue;
				if (textIndex != debugRightIndex && (component == Thing.Components.Text || component == Thing.Components.TextAnchor || component == Thing.Components.TextColor || component == Thing.Components.TextFont || component == Thing.Components.TextLineHeight || component == Thing.Components.TextVisibility)) continue;
				if (dataIndex != debugRightIndex && component == Thing.Components.Data) continue;
				if (familyIndex != debugRightIndex && component == Thing.Components.Family) continue;
				if (overlapIndex != debugRightIndex && component == Thing.Components.Overlap) continue;
				if (animationIndex != debugRightIndex && (component == Thing.Components.Animation || component == Thing.Components.AnimationChangeSprite)) continue;

				switch (component)
				{
					case Thing.Components.UniqueName: debugThingInfo.Add($"{k} = {ToStr(uniqueName)}"); break;
					case Thing.Components.Tags:
						{
							debugThingInfo.Add($"{sv}");
							if (thingTags.ContainsKey(uniqueName) == false)
							{
								debugThingInfo.Add($"{k} = {ToStr(null)}");
								break;
							}
							foreach (var item in thingTags[uniqueName])
							{
								var newC = $"{component}".Replace('s', ' ').Trim();
								debugThingInfo.Add($"{sv} {newC} = {ToStr(item)}");
							}
							break;
						}
					case Thing.Components.Angle:
						{
							debugThingInfo.Add($"{k} = {(int)Thing.Angle_Get(uniqueName)} ({Angle.DegreesFromNumber_Get(Thing.Angle_Get(uniqueName))})");
							break;
						}
					case Thing.Components.Direction: debugThingInfo.Add($"{k} = {Thing.Direction_Get(uniqueName)}");  break;
					case Thing.Components.Depth: debugThingInfo.Add($"{k} = {Thing.Depth_Get(uniqueName)}"); break;
					case Thing.Components.Sprite: debugThingInfo.Add($"{k} = {ToStr(Thing.Sprite_Get(uniqueName))}"); break;
					case Thing.Components.SpriteTiling: debugThingInfo.Add($"{k} = {Thing.SpriteTiling_IsTiled_Check(uniqueName)}"); break;
					case Thing.Components.SpriteColor: debugThingInfo.Add($"{k} = {Thing.SpriteColor_Get(uniqueName)}"); break;
					case Thing.Components.SpriteVisibility: debugThingInfo.Add($"{k} = {Thing.SpriteVisibility_IsVisible_Check(uniqueName)}"); break;
					case Thing.Components.Text: debugThingInfo.Add($"{k} = {ToStr(Thing.Text_Get(uniqueName))}"); break;
					case Thing.Components.TextFont: debugThingInfo.Add($"{k} = {ToStr(Thing.TextFont_Get(uniqueName))}"); break;
					case Thing.Components.TextColor: debugThingInfo.Add($"{k} = {Thing.TextColor_Get(uniqueName)}"); break;
					case Thing.Components.TextVisibility: debugThingInfo.Add($"{k} = {Thing.TextVisibility_IsVisible_Check(uniqueName)}"); break;
					case Thing.Components.TextAnchor: debugThingInfo.Add($"{k} = {Thing.TextAnchor_Get(uniqueName)}"); break;
					case Thing.Components.TextLineHeight: debugThingInfo.Add($"{k} = {Thing.TextLineHeight_Get(uniqueName)}"); break;
					case Thing.Components.Data:
						{
							if (thingData.ContainsKey(uniqueName) == false)
							{
								debugThingInfo.Add($"{k} = {ToStr(null)}");
								break;
							}
							foreach (var item in thingData[uniqueName])
							{
								if (item.Value is List<string>) DisplayList((List<string>)item.Value, item.Key.ToString());
								else if (item.Value is List<int>) DisplayList((List<int>)item.Value, item.Key.ToString());
								else if (item.Value is List<double>) DisplayList((List<double>)item.Value, item.Key.ToString());
								else
								{
									var keyStr = item.Key is string ? ToStr((string)item.Key) : item.Key;
									var valueStr = item.Value is string ? ToStr((string)item.Value) : item.Value;
									debugThingInfo.Add($"{k} = {{{keyStr}:{valueStr}}}");
								}
							}
							break;
							bool DisplayList<T>(List<T> list, string key)
							{
								for (int j = 0; j < list.Count; j++)
								{
									var str = list[j] is string ? "\"" : "";
									debugThingInfo.Add($"{k} = {{{key} {j}:{str}{list[j]}{str}}}");
								}
								return true;
							}
						}
					case Thing.Components.Position: debugThingInfo.Add($"{k} = {Thing.Position_Get(uniqueName)}"); break;
					case Thing.Components.Family:
						{
							var parent = Thing.Parent_UniqueName_Get(uniqueName);
							var children = Thing.Child_All_UniqueNames_Get(uniqueName);
							debugThingInfo.Add($"{k} = {{Parent:{ToStr(parent)}}}");
							if (children.Count == 0)
							{
								debugThingInfo.Add($"{k} = {{Children: {ToStr(null)}}}");
							}
							foreach (var child in children)
							{
								debugThingInfo.Add($"{k} = {{Child:{ToStr(child)}}}");
							}
							break;
						}
					case Thing.Components.Size: debugThingInfo.Add($"{k} = {Thing.Size_Get(uniqueName)}"); break;
					case Thing.Components.Scale: debugThingInfo.Add($"{k} = {Thing.Scale_Get(uniqueName)}"); break;
					case Thing.Components.OriginOffset: debugThingInfo.Add($"{k} = {Thing.OriginOffset_Get(uniqueName)}"); break;
					case Thing.Components.Overlap:
						{
							if (thingComponents.ContainsKey(uniqueName) == false || thingComponents[uniqueName].Contains(Thing.Components.Overlap) == false)
							{
								debugThingInfo.Add($"{k} = None");
								break;
							}
							debugThingInfo.Add($"{sv} MouseHovered = {thingsHovered.Contains(uniqueName)}");
							break;
						}
					case Thing.Components.Animation:
						{
							var exists = thingCurrentAnimationName.ContainsKey(uniqueName) && thingCurrentAnimationName[uniqueName] != default;
							if (exists == false)
							{
								debugThingInfo.Add($"{k} Current = {ToStr(null)}");
							}
							else
							{
								debugThingInfo.Add($"{k} Current {{{thingCurrentAnimationName[uniqueName]}}} = {{Progress:{thingCurrentAnimationProgress[uniqueName] * 100:F0}%}}");
								debugThingInfo.Add($"{k} Current {{{thingCurrentAnimationName[uniqueName]}}} = {{Index:{thingCurrentAnimationIndex[uniqueName]}}}");
								debugThingInfo.Add($"{k} Current {{{thingCurrentAnimationName[uniqueName]}}} = {{Forward:{thingCurrentAnimationForward[uniqueName]}}}");
								debugThingInfo.Add($"{k} Current {{{thingCurrentAnimationName[uniqueName]}}} = {{Paused:{thingCurrentAnimationRunning[uniqueName] == false}}}");
							}
							if (thingAnimationSprites.ContainsKey(uniqueName) == false)
							{
								debugThingInfo.Add($"{k}s = {ToStr(null)}");
								break;
							}
							foreach (var kvp in thingAnimationSprites[uniqueName])
							{
								debugThingInfo.Add($"{k} {{{ToStr(kvp.Key)}}} = {{Duration:{thingAnimationDurations[uniqueName][kvp.Key]}s}}");
								debugThingInfo.Add($"{k} {{{ToStr(kvp.Key)}}} = {{RepeatState:{thingAnimationRepeatStates[uniqueName][kvp.Key]}}}");
								var sprites = thingAnimationSprites[uniqueName][kvp.Key];
								for (int l = 0; l < sprites.Count; l++)
								{
									debugThingInfo.Add($"{k} {{{ToStr(kvp.Key)}}} = {{Sprite {l}:{ToStr(sprites[l])}}}");
								}
							}
							break;
						}
					case Thing.Components.AnimationChangeSprite:
						{
							if (thingAnimationChangesSpriteIndex.ContainsKey(uniqueName) == false)
							{
								debugThingInfo.Add($"{sv} AnimationChangesSprite = {ToStr(null)}");
								break;
							}
							foreach (var kvp in thingAnimationChangesSpriteIndex[uniqueName])
							{
								foreach (var change in kvp.Value)
								{
									debugThingInfo.Add($"{k} {{{ToStr(kvp.Key)}: {change.Key[0]:F2}s-{change.Key[1]:F2}s}} = {{Index:{change.Value}}}");
								}
							}
							break;
						}
					case Thing.Components.CameraScrollPrevention: debugThingInfo.Add($"{k} = {Number.Rounded_Get(Thing.CameraScrollPreventionPercent_Get(uniqueName), 0, Number.RoundType.Closest)}%"); break;
				}
			}
			debugThingInfo.Add($"{sv}{sh}{sh} {debugDefaultLinesThingInfo[debugRightIndex]} {sl}{sl}");
		}
		else if (debugThingInfoVisible == false)
		{
			var fpsCurrent = $"{Game.FramesPerSecond_Get(false):F1}";
			var fpsAverage = $"{Game.FramesPerSecond_Get(true):F1}";
			debugGlobalInfo.Add($"{sv}{sh}{sh} {debugDefaultLinesGlobal[debugRightIndex]} {sl}{sl}");

			var debugIndex = 0; var gameIndex = 1; var cameraIndex = 2; var thingsIndex = 3; var inputIndex = 4; var dataIndex = 5; var signalsIndex = 6; var gatesIndex = 7; var spritesIndex = 8;
			var spriteGridsIndex = 9; var fontsIndex = 10; var soundsIndex = 11; var melodiesIndex = 12; var playlistsIndex = 13;

			if (debugRightIndex == debugIndex)
			{
				debugGlobalInfo.Add($"{sv} Adjust Speed = Num[+][-]");
				debugGlobalInfo.Add($"{sv} Scroll Left Column Info = Num[1][7]");
				debugGlobalInfo.Add($"{sv} Scroll Right Column Components = Num[3][9]");
				debugGlobalInfo.Add($"{sv} Thing Edit Start/Stop = Num[5]");
				debugGlobalInfo.Add($"{sv} Thing Duplicate = Num[2][4][6][8]");
				debugGlobalInfo.Add($"{sv} Thing Delete = [Delete]");
				debugGlobalInfo.Add($"{sv} Edit Values = Num[2][4][6][8]");
				debugGlobalInfo.Add($"{sv} Edit (some) Text Values = [all keys besides Nums]");
				debugGlobalInfo.Add($"{sv}");
				debugGlobalInfo.Add($"{sv} DebugFont = {ToStr(debugFont)}");
				debugGlobalInfo.Add($"{sv}");
				debugGlobalInfo.Add($"{sv} DebugScale = {debugScale}");
				debugGlobalInfo.Add($"{sv}");
				debugGlobalInfo.Add($"{sv} DebugColor = {debugColor}");
			}
			else if (debugRightIndex == gameIndex)
			{
				debugGlobalInfo.Add($"{sv} WindowTitle = {ToStr(Game.WindowTitle_Get())}");
				debugGlobalInfo.Add($"{sv} WindowType = {Game.WindowType_Get()}");
				debugGlobalInfo.Add($"{sv} WindowResolution = {{Width:{Game.ResolutionDefaultWidth_Get()} Height:{Game.ResolutionDefaultHeight_Get()}}}");
				debugGlobalInfo.Add($"{sv}");
				debugGlobalInfo.Add($"{sv} HotkeyExitActivated (Alt+F4) = {Game.HotkeyExitIsActivated_Check()}");
				debugGlobalInfo.Add($"{sv} ComputerSleepPreventionActivated = {Game.ComputerSleepPreventionIsActivated_Check()}");
				debugGlobalInfo.Add($"{sv}");
				debugGlobalInfo.Add($"{sv} GameTitle = {ToStr(Game.Title_Get())}");
				debugGlobalInfo.Add($"{sv} GameVersion = {ToStr(Game.Version_Get())}");
				debugGlobalInfo.Add($"{sv} GameResolution = {{Width:{Game.ResolutionWidth_Get()} Height:{Game.ResolutionHeight_Get()}}}");
				debugGlobalInfo.Add($"{sv}");
				debugGlobalInfo.Add($"{sv} PixelSize = {{Width:{Game.ResolutionDefaultWidth_Get() / Game.ResolutionWidth_Get()} Height:{Game.ResolutionDefaultHeight_Get() / Game.ResolutionHeight_Get()}}}");
				debugGlobalInfo.Add($"{sv} PixelSmoothness = {Game.PixelSmoothnessIsActivated_Check()}");
				debugGlobalInfo.Add($"{sv}");
				debugGlobalInfo.Add($"{sv} BackgroundColor = {Game.BackgroundColor_Get()}");
				debugGlobalInfo.Add($"{sv}");
				debugGlobalInfo.Add($"{sv} TicksCount = {Game.Tick_Get()}");
				debugGlobalInfo.Add($"{sv} Runtime = {{h:{hours} m:{minutes} s:{secondsFloor} ms:{ms}}}");
				debugGlobalInfo.Add($"{sv} SecondsSinceLastTick = {Game.SecondsSinceLastTick_Get():F8}");
				debugGlobalInfo.Add($"{sv}");
				debugGlobalInfo.Add($"{sv} FrameCountRendered = {Game.FrameCountRendered_Get()}");
				debugGlobalInfo.Add($"{sv} FrameCount = {Game.FrameCount_Get()}");
				debugGlobalInfo.Add($"{sv} FramesPerSecond = {{Current:{fpsCurrent}}}");
				debugGlobalInfo.Add($"{sv} FramesPerSecond = {{Average:{fpsAverage}}}");
			}
			else if (debugRightIndex == cameraIndex)
			{
				debugGlobalInfo.Add($"{sv} CameraPosition = {Camera.Position_Get()}");
			}
			else if (debugRightIndex == thingsIndex)
			{
				debugGlobalInfo.Add($"{sv} ThingsCount = {thingUniqueNames.Count}");
				debugGlobalInfo.Add($"{sv}");
				debugGlobalInfo.Add($"{sv} TagsCount = {tagThings.Count}");
				var tagIndex = 0;
				foreach (var kvp in tagThings)
				{
					foreach (var thing in kvp.Value)
					{
						debugGlobalInfo.Add($"{sv} Tag {{{ToStr(kvp.Key)}}} = {{UniqueName:{ToStr(thing)}}}");
					}
					tagIndex++;
				}
				var noTagThings = new List<string>();
				var noTagThingExists = false;
				foreach (var thing in thingUniqueNames)
				{
					if (Thing.Tags_Get(thing).Count > 0)
					{
						continue;
					}
					noTagThings.Add(thing);
					noTagThingExists = true;
				}
				if (noTagThingExists)
				{
					foreach (var noTagThing in noTagThings)
					{
						debugGlobalInfo.Add($"{sv} Tag {{None}} = {{UniqueName:{ToStr(noTagThing)}}}");
					}
				}
			}
			else if (debugRightIndex == inputIndex)
			{
				var mouseButtonsPressed = new Dictionary<string, bool>()
			{
				{ "Left", Input.MouseButtonIsPressedLeft_Check() },
				{ "Middle", Input.MouseButtonIsPressedMiddle_Check() },
				{ "Right", Input.MouseButtonIsPressedRight_Check() }
			};
				if (mouseButtonsPressed["Left"] == false && mouseButtonsPressed["Middle"] == false && mouseButtonsPressed["Right"] == false)
				{
					debugGlobalInfo.Add($"{sv} MouseButtonsPressed = None");
				}
				var mouseButtonsDisplay = new List<string>();
				foreach (var kvp in mouseButtonsPressed)
				{
					if (mouseButtonsPressed[kvp.Key])
					{
						mouseButtonsDisplay.Add($"[{kvp.Key}]");
					}
				}
				foreach (var button in mouseButtonsDisplay)
				{
					debugGlobalInfo.Add($"{sv} MouseButtonPressed = {button}");
				}
				debugGlobalInfo.Add($"{sv}");
				var keysPressed = Input.KeysPressed_Get();
				if (keysPressed.Count == 0)
				{
					debugGlobalInfo.Add($"{sv} KeyboardKeysPressed = None");
				}
				foreach (var key in keysPressed)
				{
					debugGlobalInfo.Add($"{sv} KeyboardKeyPressed = {{Key:[{key}] Text: {ToStr(Input.KeyToText_Get(key))}}}");
				}
			}
			else if (debugRightIndex == signalsIndex)
			{
				if (signalDelays.Count == 0) debugGlobalInfo.Add($"{sv} Signals = None");
				else
				{
					debugGlobalInfo.Add($"{sv} SignalsCount = {signalDelays.Count}");
					debugGlobalInfo.Add($"{sv}");
				}
				foreach (var kvp in signalDelays)
				{
					debugGlobalInfo.Add($"{sv} SignalDelay {{{ToStr(kvp.Key)}}} = {kvp.Value}");
				}
			}
			else if (debugRightIndex == gatesIndex)
			{
				if (gates.Count == 0) debugGlobalInfo.Add($"{sv} Gates = None");
				else
				{
					debugGlobalInfo.Add($"{sv} GatesCount = {gates.Count}");
					debugGlobalInfo.Add($"{sv}");
				}
				foreach (var kvp in gates)
				{
					debugGlobalInfo.Add($"{sv} GateEntries {{{ToStr(kvp.Key)}}} = {gateEntriesCount[kvp.Key]}");
				}
			}
			else if (debugRightIndex == spritesIndex)
			{
				if (sprites.Count == 0) debugGlobalInfo.Add($"{sv} Sprites = None");
				else
				{
					debugGlobalInfo.Add($"{sv} SpritesCount = {sprites.Count}");
					debugGlobalInfo.Add($"{sv}");
				}
				foreach (var kvp in sprites) debugGlobalInfo.Add($"{sv} Sprite {{{ToStr(kvp.Key)}}} = {{Width:{kvp.Value.Width} Height:{kvp.Value.Height}}}");
			}
			else if (debugRightIndex == spriteGridsIndex)
			{
				if (spriteGrids.Count == 0) debugGlobalInfo.Add($"{sv} SpriteGrids = None");
				else debugGlobalInfo.Add($"{sv} SpriteGridCount = {spriteGrids.Count}");
				foreach (var kvp in spriteGrids)
				{
					debugGlobalInfo.Add($"{sv}");
					debugGlobalInfo.Add($"{sv} SpriteGridRowsCount {{Name:{ToStr(kvp.Key)}}} = {kvp.Value.Count}");
					for (int i = 0; i < kvp.Value.Count; i++)
					{
						debugGlobalInfo.Add($"{sv} SpritesCount {{Name:{ToStr(kvp.Key)} Row {i}}} = {kvp.Value[i].Count}");
						for (int j = 0; j < kvp.Value[i].Count; j++)
						{
							debugGlobalInfo.Add($"{sv} SpriteGrid {{Name:{ToStr(kvp.Key)} Row {i} Sprite {j}}} = {ToStr(kvp.Value[i][j])}");
						}
					}
				}
			}
			else if (debugRightIndex == fontsIndex)
			{
				if (fonts.Count == 0) debugGlobalInfo.Add($"{sv} Fonts = None");
				else
				{
					debugGlobalInfo.Add($"{sv} FontsCount = {fonts.Count}");
					debugGlobalInfo.Add($"{sv}");
				}
				foreach (var kvp in fonts) debugGlobalInfo.Add($"{sv} Font = {ToStr(kvp.Key)}");
			}
			else if (debugRightIndex == soundsIndex)
			{
				if (sounds.Count == 0) debugGlobalInfo.Add($"{sv} Sounds = None");
				else
				{
					debugGlobalInfo.Add($"{sv} SoundsCount = {sounds.Count}");
					debugGlobalInfo.Add($"{sv}");
				}
				foreach (var kvp in sounds) debugGlobalInfo.Add($"{sv} Sound = {ToStr(kvp.Key)}");
			}
			else if (debugRightIndex == melodiesIndex)
			{
				if (melodies.Count == 0) debugGlobalInfo.Add($"{sv} Melodies = None");
				else
				{
					debugGlobalInfo.Add($"{sv} MelodiesCount = {melodies.Count}");
					debugGlobalInfo.Add($"{sv}");
				}
				foreach (var kvp in melodies) debugGlobalInfo.Add($"{sv} Melody = {ToStr(kvp.Key)}");
			}
			else if (debugRightIndex == playlistsIndex)
			{
				if (playlists.Count == 0) debugGlobalInfo.Add($"{sv} Playlists = None");
				else
				{
					debugGlobalInfo.Add($"{sv} PlaylistsCount = {playlists.Count}");
					debugGlobalInfo.Add($"{sv}");
				}
				foreach (var kvp in playlists)
				{
					for (int i = 0; i < kvp.Value.Count; i++)
					{
						debugGlobalInfo.Add($"{sv} PlaylistSong {{{ToStr(kvp.Key)}}} = {ToStr(kvp.Value[i])}");
					}
				}
			}
			else if (debugRightIndex == dataIndex)
			{
				if (data.Count == 0) debugGlobalInfo.Add($"{sv} Data = None");
				else
				{
					debugGlobalInfo.Add($"{sv} DataCount = {data.Count}");
					debugGlobalInfo.Add($"{sv}");
				}
				foreach (var kvp in data)
				{
					var key = kvp.Key is string ? ToStr((string)kvp.Key) : kvp.Key;
					var value = kvp.Value is string ? ToStr((string)kvp.Value) : kvp.Value;
					debugGlobalInfo.Add($"{sv} Data = {{{key}: {value}}}");
				}
				debugGlobalInfo.Add($"{sv}");
				if (storage.Count == 0) debugGlobalInfo.Add($"{sv} Storage = None");
				else
				{
					debugGlobalInfo.Add($"{sv} StorageCount = {storage.Count}");
					debugGlobalInfo.Add($"{sv}");
				}
				foreach (var kvp in storage)
				{
					debugGlobalInfo.Add($"{sv} Storage = {{{ToStr(kvp.Key)}: {ToStr(kvp.Value)}}}");
				}
			}
			debugGlobalInfo.Add($"{sv}{sh}{sh} {debugDefaultLinesGlobal[debugRightIndex]} {sl}{sl}");
		}

		var leftLines = debugThingInfoVisible ? debugThingInfo : debugGlobalInfo;
		if (leftLines.Count > 0 && debugLeftIndex < leftLines.Count - 1)
		{
			debugLeftSelected = leftLines[debugLeftIndex];
		}
		else
		{
			debugLeftSelected = "";
		}
		if (debugDefaultLinesThingInfo.Contains(newLeftSelected) || debugDefaultLinesGlobal.Contains(newLeftSelected) || newLeftSelected == "")
		{
			DebugInfoIndexJump(false, false);
		}
		if (debugDefaultLinesThings.Contains(newLeftSelected) || newRightSelected == "")
		{
			DebugInfoIndexJump(false, false);
		}
		if (rightLines.Count > 0 && debugRightIndex < rightLines.Count)
		{
			debugRightSelected = rightLines[debugRightIndex];
		}

		DrawLines(leftLines, debugScale.X * 5, y / 2 - lineHeight, Thing.TextAnchor.TopLeft, "         ", "", true, debugThingInfoVisible ? "things-info" : "global");
		DrawLines(rightLines, x - debugScale.X * 5, y / 2 - lineHeight, Thing.TextAnchor.TopRight, "", "         ", true, "things");
	}
	private static void DrawLines(List<string> lines, float x, float y, Thing.TextAnchor textAnchor, string selectedLeft, string selectedRight, bool interactable, string type)
	{
		var lineHeight = GetDebugLineHeight();
		if (textAnchor == Thing.TextAnchor.BottomLeft || textAnchor == Thing.TextAnchor.BottomCenter || textAnchor == Thing.TextAnchor.BottomRight)
		{
			y -= (lines.Count - 1) * lineHeight;
		}
		foreach (var line in lines)
		{
			var message = line;
			var color = debugColor;
			var offsetY = 0.0;
			var invertColor = new Color(255 - color.R, 255 - color.G, 255 - color.B, color.A);
			var isColored = false;

			switch (type)
			{
				case "things": offsetY = debugRightY; break;
				case "things-info": offsetY = debugLeftY; break;
				case "global": offsetY = debugLeftY; break;
			}
			var newY = y + (float)offsetY;
			if (newY < -lineHeight)
			{
				y += lineHeight;
				continue;
			}
			if (newY > Game.ResolutionDefaultHeight_Get() - lineHeight * 2)
			{
				return;
			}

			if (message.Contains("\"") == false && message.Contains("{R:") && message.Contains("G:") && message.Contains("B:") && message.Contains("A:"))
			{
				var split = message.Split('=');
				if (split.Length > 1)
				{
					var a = split[1];
					a = a.Replace('{', ' ');
					a = a.Replace('R', ' ');
					a = a.Replace('G', ' ');
					a = a.Replace('B', ' ');
					a = a.Replace('A', ' ');
					a = a.Replace('}', ' ');
					var values = a.Split(':');
					for (int i = 0; i < values.Length; i++)
					{
						values[i] = values[i].Trim();
					}
					if (values.Length == 5)
					{
						color = new Color(int.Parse(values[1]), int.Parse(values[2]), int.Parse(values[3]), int.Parse(values[4]));
					}
				}
				isColored = color != default;
				color = color == default ? debugColor : color;
				invertColor = new Color(255 - color.R, 255 - color.G, 255 - color.B);
			}
			if (interactable)
			{
				if (line != debugRightSelected && line != debugLeftSelected)
				{
					message = $"{selectedLeft}{line}{selectedRight}";
				}
				else if (isColored == false)
				{
					color = invertColor;
					invertColor = new Color(255 - color.R, 255 - color.G, 255 - color.B);
				}
			}
			var origin = GetAnchorOffset(debugFont, message, textAnchor);
			var shadowOffset = debugScale / 1.1f;

			painter.DrawString(fonts[debugFont], message, new Vector2(x + shadowOffset.X, y + (float)offsetY + shadowOffset.Y), invertColor, 0, origin, debugScale, SpriteEffects.None, 0);
			painter.DrawString(fonts[debugFont], message, new Vector2(x, newY), color, 0, origin, debugScale, SpriteEffects.None, 0);
			y += lineHeight;
		}
	}
	private static float GetDebugLineHeight()
	{
		return fonts[debugFont].LineSpacing * debugScale.Y;
	}
	#endregion
	#region Static
	// [Create-Add-Set-Active-Start] [Pause-Delete-Stop-Remove] [Get-Check]

	/// <summary>
	/// Controls the system and the window and holds information about them.
	/// </summary>
	public static class Game
	{
		/// <summary>
		/// Holds all methods of displaying the window.
		/// </summary>
		public enum WindowTypes
		{
			WindowedFullscreen, Windowed_TitleBar, Windowed_TaskBar, Windowed_TitleBar_TaskBar, Fullscreen
		}
		/// <summary>
		/// Gets the current method of displaying the window.
		/// </summary>
		public static WindowTypes WindowType_Get()
		{
			return windowType;
		}

		/// <summary>
		/// Sets the resolution to a size of <paramref name="width"/> and <paramref name="height"/> using <paramref name="windowType"/> method to display the window.
		/// </summary>
		public static void Resolution_Set(int width, int height, WindowTypes windowType)
		{
			var def = new Point(ResolutionDefaultWidth_Get(), ResolutionDefaultHeight_Get());
			graphics.PreferredBackBufferWidth = def.X;
			graphics.PreferredBackBufferHeight = def.Y;

			if (width > def.X)
			{
				Resolution_Set(def.X, height, windowType);
				return;
			}
			if (height > def.Y)
			{
				Resolution_Set(width, def.Y, windowType);
				return;
			}

			Do.windowType = windowType;
			SetWindowPositionAccordingToResolution();
			var gd = game.GraphicsDevice;
			renderTarget = new RenderTarget2D(gd, def.X, def.Y, false, gd.PresentationParameters.BackBufferFormat, DepthFormat.Depth24);
			screenSize = new Point(width, height);
			graphics.ApplyChanges();
		}
		/// <summary>
		/// Gets the current width of the resolution.
		/// </summary>
		public static int ResolutionWidth_Get()
		{
			return screenSize.X;
		}
		/// <summary>
		/// Gets the current height of the resolution.
		/// </summary>
		public static int ResolutionHeight_Get()
		{
			return screenSize.Y;
		}

		/// <summary>
		/// Sets the resolution to the current resolution of the user's monitor.
		/// </summary>
		public static void ResolutionToDefault_Set()
		{
			Resolution_Set(ResolutionDefaultWidth_Get(), ResolutionDefaultHeight_Get(), WindowTypes.Windowed_TitleBar_TaskBar);
		}
		/// <summary>
		/// Gets the current width of the resolution that the user's monitor uses.
		/// </summary>
		public static int ResolutionDefaultWidth_Get()
		{
			return GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
		}
		/// <summary>
		/// Gets the current height of the resolution that the user's monitor uses.
		/// </summary>
		public static int ResolutionDefaultHeight_Get()
		{
			return GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
		}

		public static bool WindowIsFocused_Check()
        {
			return game.IsActive;
        }
		public static void WindowUnfocusedPause_Activate(bool activated)
		{
			pauseUnfocus = activated;
		}
		public static bool WindowUnfocusedPauseIsActivated_Check()
		{
			return pauseUnfocus;
		}
		/// <summary>
		/// Sets the <paramref name="title"/> of the window.
		/// </summary>
		public static void WindowTitle_Set(string title)
		{
			game.Window.Title = title;
		}
		/// <summary>
		/// Gets the current title of the window.
		/// </summary>
		public static string WindowTitle_Get()
		{
			return game.Window.Title;
		}
		/// <summary>
		/// Sets the <paramref name="title"/> of the game.
		/// </summary>
		public static void Title_Set(string title)
		{
			Do.title = title;
		}
		/// <summary>
		/// Gets the current title of the game.
		/// </summary>
		public static string Title_Get()
		{
			return title;
		}

		/// <summary>
		/// Sets the <paramref name="version"/> of the game.
		/// </summary>
		public static void Version_Set(string version)
		{
			Do.version = version;
		}
		/// <summary>
		/// Gets the current version of the game.
		/// </summary>
		public static string Version_Get()
		{
			return version;
		}

		/// <summary>
		/// Activates or deactivates the Alt+F4 functionality.
		/// </summary>
		public static void HotkeyExit_Activate(bool activate)
		{
			game.Window.AllowAltF4 = activate;
		}
		public static bool HotkeyExitIsActivated_Check()
		{
			return game.Window.AllowAltF4;
		}

		/// <summary>
		/// Sets the maximum <paramref name="framesPerSecond"/>.
		/// </summary>
		public static void FramesPerSecond_Set(double framesPerSecond)
		{
			framesPerSecond = Number.Limited_Get(framesPerSecond, 2, 1000);
			game.TargetElapsedTime = TimeSpan.FromSeconds(1d / framesPerSecond);
		}
		/// <summary>
		/// Gets the current frames per second.
		/// </summary>
		public static double FramesPerSecond_Get(bool average)
		{
			return average ? fpsAverage : fps;
		}

		public static int FrameCount_Get()
		{
			return frame;
		}
		public static int FrameCountRendered_Get()
		{
			return frameRendered;
		}

		public static int Tick_Get()
		{
			return tick;
		}

		public static void LoadingScreenFile_Set(string path, string extension)
		{
			LoadFile($"{path}.{extension}");
		}
		public static bool IsLoading_Check()
		{
			return loading;
		}

		public static double SecondsSinceLastTick_Get()
		{
			return deltaTime;
		}

		public static double Runtime_Get()
		{
			return time;
		}

		public static void BackgroundColor_Set(Color color)
		{
			backgroundColor = color;
		}
		public static void BackgroundColor1_Set(double red, double green, double blue, double alpha = 1)
		{
			BackgroundColor_Set(new Color((float)red, (float)green, (float)blue, (float)alpha));
		}
		public static void BackgroundColor255_Set(int red, int green, int blue, int alpha = 255)
		{
			BackgroundColor_Set(new Color(red, green, blue, alpha));
		}
		public static Color BackgroundColor_Get()
		{
			return backgroundColor;
		}

		public static void PixelSmoothness_Activate(bool smooth)
		{
			smoothRender = smooth;
		}
		public static bool PixelSmoothnessIsActivated_Check()
		{
			return smoothRender;
		}

		public static void ComputerSleepPrevention_Activate(bool activate)
		{
			sleepPrevented = activate;
			if (activate)
			{
				SetThreadExecutionState(EXECUTION_STATE.ES_CONTINUOUS | EXECUTION_STATE.ES_DISPLAY_REQUIRED | EXECUTION_STATE.ES_SYSTEM_REQUIRED);
			}
			else
			{
				SetThreadExecutionState(EXECUTION_STATE.ES_CONTINUOUS);
			}
		}
		public static bool ComputerSleepPreventionIsActivated_Check()
		{
			return sleepPrevented;
		}

		public static void Stop()
		{
			game.Exit();
		}
	}

	/// <summary>
	/// Holds and controls information about the game while it is running.
	/// </summary>
	public static class Debug
	{
		public static void Activate(bool activated, string fontPath, double fontScaleWidth = 1, double fontScaleHeight = 1, int fontColorRed = 255, int fontColorGreen = 255, int fontColorBlue = 255, int fontColorAlpha = 255)
		{
			debug = activated;
			if (fontPath != null && fonts.ContainsKey(fontPath))
			{
				debugFont = fontPath;
			}
			debugScale = new Vector2((float)fontScaleWidth, (float)fontScaleHeight);
			debugColor = new Color(fontColorRed, fontColorGreen, fontColorBlue, fontColorAlpha);

			render = true;
		}
	}

	/// <summary>
	/// Controls the game objects and holds information about them.
	/// </summary>
	public static class Thing
	{
		public enum Components
		{
			UniqueName, Tags, Position, Angle, Direction, Size, Scale, SpriteTiling, Sprite, SpriteColor, SpriteFillColor, SpriteOutlineColor, SpriteVisibility, SpriteFillVisiblity, SpriteOutlineVisibility, OriginOffset, Depth, TextFont, Text, TextColor, TextVisibility, TextAnchor, TextLineHeight, Data, Family, Overlap, Animation, AnimationChangeSprite, CameraScrollPrevention
		}
		public enum AnimationRepeatStates
		{
			None, Loop, Mirror
		}
		public enum TextAnchor
		{
			TopLeft, TopCenter, TopRight, CenterLeft, Center, CenterRight, BottomLeft, BottomCenter, BottomRight
		}

		public static void Create(string uniqueName, double positionX = 0, double positionY = 0, double sizeWidth = 0, double sizeHeight = 0, double scaleWidth = 0, double scaleHeight = 0, string spritePath = null, double spriteColorRed1 = 1, double spriteColorGreen1 = 1, double spriteColorBlue1 = 1, double spriteColorAlpha1 = 1, int spriteColorRed255 = 255, int spriteColorGreen255 = 255, int spriteColorBlue255 = 255, int spriteColorAlpha255 = 255, string textFont = null, string text = null, TextAnchor textAnchor = TextAnchor.TopLeft, double textColorRed1 = 1, double textColorGreen1 = 1, double textColorBlue1 = 1, double textColorAlpha1 = 1, int textColorRed255 = 255, int textColorGreen255 = 255, int textColorBlue255 = 255, int textColorAlpha255 = 255, int depth = 0, int angle = 0, Direction.Directions direction = Direction.Directions.Right)
		{
			if (thingUniqueNames.Contains(uniqueName))
			{
				return;
			}
			thingUniqueNames.Add(uniqueName);
			thingComponents[uniqueName] = new List<Components>() { Components.UniqueName };
			componentThings[Components.UniqueName] = new List<string>() { uniqueName };
			var position = new Vector2((float)positionX, (float)positionY);
			var size = new Vector2((float)sizeWidth, (float)sizeHeight);
			var scale = new Vector2((float)scaleWidth, (float)scaleHeight);
			var spriteColor1 = new Color((float)spriteColorRed1, (float)spriteColorGreen1, (float)spriteColorBlue1, (float)spriteColorAlpha1);
			var spriteColor255 = new Color(spriteColorRed255, spriteColorGreen255, spriteColorBlue255, spriteColorAlpha255);
			var textColor1 = new Color((float)textColorRed1, (float)textColorGreen1, (float)textColorBlue1, (float)textColorAlpha1);
			var textColor255 = new Color(textColorRed255, textColorGreen255, textColorBlue255, textColorAlpha255);
			var defaultColor = new Color(255, 255, 255, 255);
			if (spritePath != null)
			{
				Sprite_Set(uniqueName, spritePath);
			}
			if (position != Vector2.Zero)
			{
				Position_Set(uniqueName, position);
			}
			if (size != Vector2.Zero)
			{
				Size_Set(uniqueName, size);
			}
			if (scale != Vector2.Zero)
			{
				Scale_Set(uniqueName, scale);
			}
			if (textFont != null)
			{
				TextFont_Set(uniqueName, textFont);
			}
			if (text != null)
			{
				Text_Set(uniqueName, text);
			}
			if (textAnchor != TextAnchor.TopLeft)
			{
				TextAnchor_Set(uniqueName, textAnchor);
			}
			if (spriteColor1 != defaultColor)
			{
				SpriteColor_Set(uniqueName, spriteColor1);
			}
			else if (spriteColor255 != defaultColor)
			{
				SpriteColor_Set(uniqueName, spriteColor255);
			}
			if (textColor1 != defaultColor)
			{
				TextColor_Set(uniqueName, textColor1);
			}
			else if (textColor255 != defaultColor)
			{
				TextColor_Set(uniqueName, textColor255);
			}
			if (depth != 0)
			{
				Depth_Set(uniqueName, depth);
			}
			if (angle != 0)
			{
				Angle_Set(uniqueName, angle);
			}
			if (direction != Direction.Directions.Right)
			{
				Direction_Set(uniqueName, direction);
			}
		}
		public static void Duplicate_Create(string uniqueName, string newUniqueName)
		{
			Delete(newUniqueName);
			Create(newUniqueName);
			if (thingAngles.ContainsKey(uniqueName))
			{
				Angle_Set(newUniqueName, thingAngles[uniqueName]);
			}
			if (thingAnimationDurations.ContainsKey(uniqueName))
			{
				foreach (var animation in thingAnimationDurations[uniqueName])
				{
					Animation_Add(newUniqueName, animation.Key, animation.Value, thingAnimationRepeatStates[uniqueName][animation.Key]);
				}
			}
			if (thingAnimationSprites.ContainsKey(uniqueName))
			{
				foreach (var animation in thingAnimationSprites[uniqueName])
				{
					foreach (var sprite in animation.Value)
					{
						Animation_Sprite_Add(newUniqueName, animation.Key, sprite);
					}
				}
			}
			if (thingAnimationChangesSpriteIndex.ContainsKey(uniqueName))
			{
				foreach (var animation in thingAnimationChangesSpriteIndex[uniqueName])
				{
					foreach (var change in animation.Value)
					{
						Animation_ChangeSprite_Add(newUniqueName, animation.Key, change.Key[0], change.Key[1], change.Value);
					}
				}
			}
			if (thingChildren.ContainsKey(uniqueName))
			{
				foreach (var child in thingChildren[uniqueName])
				{
					Child_Add(newUniqueName, child);
				}
			}
			if (thingParents.ContainsKey(uniqueName))
			{
				Parent_Set(newUniqueName, thingParents[uniqueName]);
			}
			if (thingData.ContainsKey(uniqueName))
			{
				foreach (var data in thingData[uniqueName])
				{
					Data_Set(newUniqueName, data.Key, data.Value);
				}
			}
			if (thingOffsets.ContainsKey(uniqueName))
			{
				OriginOffset_Set(newUniqueName, thingOffsets[uniqueName]);
			}
			if (thingPositions.ContainsKey(uniqueName))
			{
				Position_Set(newUniqueName, thingPositions[uniqueName]);
			}
			if (thingScales.ContainsKey(uniqueName))
			{
				Scale_Set(newUniqueName, thingScales[uniqueName]);
			}
			if (thingDepths.ContainsKey(uniqueName))
			{
				Depth_Set(newUniqueName, thingDepths[uniqueName]);
			}
			if (thingSpriteColors.ContainsKey(uniqueName))
			{
				SpriteColor_Set(newUniqueName, thingSpriteColors[uniqueName]);
			}
			if (thingSpriteColors.ContainsKey(uniqueName))
			{
				SpriteColor_Set(newUniqueName, thingSpriteColors[uniqueName]);
			}
			if (thingSprites.ContainsKey(uniqueName))
			{
				Sprite_Set(newUniqueName, thingSprites[uniqueName]);
			}
			if (thingSpritesVisible.ContainsKey(uniqueName))
			{
				SpriteVisibility_Activate(newUniqueName, thingSpritesVisible[uniqueName]);
			}
			if (thingTags.ContainsKey(uniqueName))
			{
				foreach (var tag in thingTags[uniqueName])
				{
					Tag_Add(newUniqueName, tag);
				}
			}
			if (thingTextColors.ContainsKey(uniqueName))
			{
				TextColor_Set(newUniqueName, thingTextColors[uniqueName]);
			}
			if (thingTexts.ContainsKey(uniqueName))
			{
				Text_Set(newUniqueName, thingTexts[uniqueName]);
			}
			if (thingTextsVisible.ContainsKey(uniqueName))
			{
				TextVisibility_Activate(newUniqueName, thingTextsVisible[uniqueName]);
			}
			if (thingOverlapsActivated.ContainsKey(uniqueName))
			{
				Overlap_Activate(newUniqueName, thingOverlapsActivated[uniqueName]);
			}
			if (thingTextFonts.ContainsKey(uniqueName))
			{
				TextFont_Set(newUniqueName, thingTextFonts[uniqueName]);
			}
			if (thingTextAnchors.ContainsKey(uniqueName))
			{
				TextAnchor_Set(newUniqueName, thingTextAnchors[uniqueName]);
			}
			if (thingTextLineHeights.ContainsKey(uniqueName))
			{
				TextLineHeight_Set(newUniqueName, thingTextLineHeights[uniqueName]);
			}
            if (thingCameraScrollPrevention.ContainsKey(uniqueName))
            {
				CameraScrollPreventionPercent_Set(newUniqueName, thingCameraScrollPrevention[uniqueName]);
			}
			if (thingSpriteFillColors.ContainsKey(uniqueName))
			{
				var c = thingSpriteFillColors[uniqueName];
				SpriteFillColor_255_Set(newUniqueName, c.R, c.G, c.B, c.A);
			}
			if (thingSpriteOutlineColors.ContainsKey(uniqueName))
			{
				var c = thingSpriteOutlineColors[uniqueName];
				SpriteOutlineColor_255_Set(newUniqueName, c.R, c.G, c.B, c.A);
			}
			if (thingSpriteFillsVisible.ContainsKey(uniqueName))
			{
				SpriteFillVisibility_Activate(newUniqueName, thingSpriteFillsVisible[uniqueName]);
			}
			if (thingSpriteOutlinesVisible.ContainsKey(uniqueName))
			{
				SpriteOutlineVisibility_Activate(newUniqueName, thingSpriteOutlinesVisible[uniqueName]);
			}
		}
		public static void Delete(string uniqueName)
		{
			if (Exists_Check(uniqueName) == false || thingComponents.ContainsKey(uniqueName) == false)
			{
				return;
			}
			Tags_Delete(uniqueName);
			Data_Delete(uniqueName);
			Angle_Delete(uniqueName);
			Depth_Delete(uniqueName);
			Sprite_Delete(uniqueName);
			SpriteColor_Delete(uniqueName);
			SpriteVisibility_Delete(uniqueName);
			Text_Delete(uniqueName);
			TextColor_Delete(uniqueName);
			TextVisibility_Delete(uniqueName);
			Position_Delete(uniqueName);
			Family_Remove(uniqueName);
			Size_Delete(uniqueName);
			OriginOffset_Delete(uniqueName);
			Overlap_Delete(uniqueName);
			Animation_Delete(uniqueName);
			CameraScrollPrevention_Delete(uniqueName);
			SpriteFillVisibility_Delete(uniqueName);
			SpriteFillColor_Delete(uniqueName);
			SpriteOutlineVisibility_Delete(uniqueName);
			SpriteOutlineColor_Delete(uniqueName);
			TextAnchor_Delete(uniqueName);
			TextLineHeight_Delete(uniqueName);
			TextFont_Delete(uniqueName);
			thingComponents.Remove(uniqueName);
			thingUniqueNames.Remove(uniqueName);
		}
		public static bool Exists_Check(string uniqueName)
		{
			return thingUniqueNames.Contains(uniqueName);
		}

		public static List<Components> Components_Get(string uniqueName)
		{
			return thingComponents.ContainsKey(uniqueName) ? new List<Components>(thingComponents[uniqueName]) : new List<Components>();
		}
		public static List<string> Component_All_UniqueNames_Get(Components component)
		{
			return componentThings.ContainsKey(component) ? new List<string>(componentThings[component]) : new List<string>();
		}

		public static List<string> UniqueNames_All_Get()
		{
			return new List<string>(thingUniqueNames);
		}
		public static List<string> UniqueNames_All_ByTag_Get(string tag)
		{
			return tagThings.ContainsKey(tag) ? new List<string>(tagThings[tag]) : new List<string>();
		}
		public static List<string> UniqueNames_All_ByTagFromList_Get(string tag, List<string> uniqueNames)
		{
			var result = new List<string>();
			foreach (var name in uniqueNames)
			{
				var hasTag = Tags_Get(name).Contains(tag);
				if (hasTag)
				{
					result.Add(name);
				}
			}
			return result;
		}
		/// <summary>
		/// Casts a line of up to <paramref name="dotCount"/> dots with <paramref name="pixelStepSize"/> separating them.<br></br>
		/// Can be visualized with <paramref name="displaySprite"/> and <paramref name="displayColor"/>.<br></br>
		/// If any of the dots are Overlapping a Thing and that Thing is in <paramref name="uniqueNames"/> its uniqueName is added to a list in the order of hitting.<br></br>
		/// The list is then returned.
		/// </summary>
		public static List<string> UniqueNames_All_ByDotcastFromList_Get(List<string> uniqueNames, Vector2 startingPosition, Vector2 direction, int dotCount, double pixelStepSize = 1, string displayUniqueName = default, string displaySprite = default, Color displayColor = default)
		{
			var result = new List<string>();
			var currPos = startingPosition;

			direction.Normalize();
			for (int i = 0; i < dotCount; i++)
			{
				currPos += direction * (float)pixelStepSize;

				foreach (var thing in uniqueNames)
				{
					if (result.Contains(thing) == false && Overlap_IsOverlappingPosition_Check(thing, currPos))
					{
						result.Add(thing);
					}
				}
			}

			if (displayUniqueName == default || displaySprite == default || displayColor == default)
			{
				return result;
			}

			Create(displayUniqueName);
			Sprite_Set(displayUniqueName, displaySprite);
			SpriteColor_Set(displayUniqueName, displayColor);

			var distance = Position.DistanceToPosition_Get(startingPosition, currPos);
			Size_Set(displayUniqueName, distance, 1);
			Direction_Set(displayUniqueName, direction);
			Depth_Set(displayUniqueName, int.MinValue);

			return result;
		}

		public static void Tag_Add(string uniqueName, string tag)
		{
			if (Exists_Check(uniqueName) == false)
			{
				return;
			}
			if (thingTags.ContainsKey(uniqueName) == false)
			{
				thingTags[uniqueName] = new List<string>();
			}
			if (thingTags[uniqueName].Contains(tag) == false)
			{
				thingTags[uniqueName].Add(tag);
			}

			if (tagThings.ContainsKey(tag) == false)
			{
				tagThings[tag] = new List<string>();
			}
			if (tagThings[tag].Contains(uniqueName) == false)
			{
				tagThings[tag].Add(uniqueName);
			}

			AddComponent(uniqueName, Components.Tags);
		}
		public static bool Tag_Exists_Check(string tag)
		{
			return tagThings.ContainsKey(tag);
		}
		public static void Tag_Delete(string uniqueName, string tag)
		{
			if (thingTags.ContainsKey(uniqueName) == false || thingTags[uniqueName].Contains(tag) == false)
			{
				return;
			}
			thingTags[uniqueName].Remove(tag);
			tagThings[tag].Remove(uniqueName);
			if (tagThings[tag].Count == 0)
			{
				tagThings.Remove(tag);
			}
		}
		public static List<string> Tags_Get(string uniqueName)
		{
			return thingTags.ContainsKey(uniqueName) ? new List<string>(thingTags[uniqueName]) : new List<string>();
		}
		public static void Tags_Delete(string uniqueName)
		{
			if (Exists_Check(uniqueName) == false || thingTags.ContainsKey(uniqueName) == false)
			{
				return;
			}
			thingTags.Remove(uniqueName);

			var keysToRemoveAt = new List<string>();
			foreach (var kvp in tagThings)
			{
				if (kvp.Value.Contains(uniqueName))
				{
					keysToRemoveAt.Add(kvp.Key);
				}
			}
			foreach (var key in keysToRemoveAt)
			{
				tagThings[key].Remove(uniqueName);
				if (tagThings[key].Count == 0)
				{
					tagThings.Remove(key);
				}
			}
			RemoveComponent(uniqueName, Components.Tags);
		}

		public static void Data_Set<T, T1>(string uniqueName, T key, T1 value)
		{
			if (Exists_Check(uniqueName) == false)
			{
				return;
			}
			if (thingData.ContainsKey(uniqueName) == false)
			{
				thingData[uniqueName] = new Dictionary<object, object>();
			}
			thingData[uniqueName][key] = value;

			AddComponent(uniqueName, Components.Data);
		}
		public static T Data_Get<T1, T>(string uniqueName, T1 key)
		{
			if (uniqueName == null || thingData.ContainsKey(uniqueName) == false || thingData[uniqueName].ContainsKey(key) == false)
			{
				return default;
			}
			var value = thingData[uniqueName][key];
			if (value is T)
			{
				return (T)value;
			}
			return default;
		}
		public static Dictionary<K, V> Data_Dictionary_Get<T, K, V>(string uniqueName, T key)
		{
			var value = Data_Get<T, Dictionary<K, V>>(uniqueName, key);
			if (value is Dictionary<K, V>)
			{
				return new Dictionary<K, V>(value);
			}
			return new Dictionary<K, V>();
		}
		public static List<I> Data_List_Get<T, I>(string uniqueName, T key)
		{
			var value = Data_Get<T, List<I>>(uniqueName, key);
			if (value is List<I>)
			{
				return new List<I>(value);
			}
			return new List<I>();
		}
		public static void Data_Delete(string uniqueName)
		{
			if (Exists_Check(uniqueName) == false || thingData.ContainsKey(uniqueName) == false)
			{
				return;
			}
			thingData.Remove(uniqueName);

			RemoveComponent(uniqueName, Components.Data);
		}

		public static void Depth_Set(string uniqueName, double depth)
		{
			if (Exists_Check(uniqueName) == false)
			{
				return;
			}
			if (thingDepths.ContainsKey(uniqueName))
			{
				depthSortedThings[thingDepths[uniqueName]].Remove(uniqueName);
				if (depthSortedThings[thingDepths[uniqueName]].Count == 0)
				{
					depthSortedThings.Remove(thingDepths[uniqueName]);
				}
				thingDepths.Remove(uniqueName);
			}
			thingDepths[uniqueName] = depth;
			if (depthSortedThings.ContainsKey(depth) == false)
			{
				depthSortedThings[depth] = new List<string>();
			}
			depthSortedThings[depth].Add(uniqueName);

			AddComponent(uniqueName, Components.Depth);
			render = true;
		}
		public static double Depth_Get(string uniqueName)
		{
			return uniqueName != null && thingDepths.ContainsKey(uniqueName) ? thingDepths[uniqueName] : default;
		}
		public static void Depth_Delete(string uniqueName)
		{
			if (Exists_Check(uniqueName) == false || thingDepths.ContainsKey(uniqueName) == false)
			{
				return;
			}
			depthSortedThings[thingDepths[uniqueName]].Remove(uniqueName);
			thingDepths.Remove(uniqueName);

			RemoveComponent(uniqueName, Components.Depth);
			render = true;
		}

		public static void SpriteTiling_Activate(string uniqueName, bool tiled)
		{
			if (Exists_Check(uniqueName) == false)
			{
				return;
			}
			thingSpritesTiling[uniqueName] = tiled;

			AddComponent(uniqueName, Components.SpriteTiling);
			render = true;
		}
		public static bool SpriteTiling_IsTiled_Check(string uniqueName)
		{
			return uniqueName != null && thingSpritesTiling.ContainsKey(uniqueName) ? thingSpritesTiling[uniqueName] : default;
		}
		public static void Sprite_Set(string uniqueName, string spritePath)
		{
			if (Exists_Check(uniqueName) == false || spritePath == null || sprites.ContainsKey(spritePath) == false || (thingSprites.ContainsKey(uniqueName) && thingSprites[uniqueName] == spritePath))
			{
				return;
			}
			thingSprites[uniqueName] = spritePath;
			SetDefaultThing(uniqueName);
			if (thingSpriteColors.ContainsKey(uniqueName) == false)
			{
				SpriteColor_1_Set(uniqueName, 1, 1, 1);
			}
			if (thingSpritesVisible.ContainsKey(uniqueName) == false)
			{
				SpriteVisibility_Activate(uniqueName, true);
			}
			if (thingPoints.ContainsKey(uniqueName) == false)
			{
				thingPoints[uniqueName] = new List<Vector2>()
					{
						Vector2.Zero, Vector2.Zero, Vector2.Zero, Vector2.Zero
					};
			}

			AddComponent(uniqueName, Components.Sprite);
			render = true;
		}
		public static string Sprite_Get(string uniqueName)
		{
			return uniqueName != null && thingSprites.ContainsKey(uniqueName) ? thingSprites[uniqueName] : default;
		}
		public static void Sprite_Delete(string uniqueName)
		{
			if (Exists_Check(uniqueName) == false || thingSprites.ContainsKey(uniqueName) == false)
			{
				return;
			}
			thingSprites.Remove(uniqueName);

			RemoveComponent(uniqueName, Components.Sprite);
			render = true;
		}
		public static void SpriteColor_Set(string uniqueName, Color color)
		{
			if (Exists_Check(uniqueName) == false)
			{
				return;
			}
			thingSpriteColors[uniqueName] = color;

			AddComponent(uniqueName, Components.SpriteColor);
			render = true;
		}
		public static void SpriteColor_1_Set(string uniqueName, double red, double green, double blue, double alpha = 1)
		{
			SpriteColor_Set(uniqueName, new Color((float)red, (float)green, (float)blue, (float)alpha));
		}
		public static void SpriteColor_255_Set(string uniqueName, int red, int green, int blue, int alpha = 255)
		{
			SpriteColor_Set(uniqueName, new Color(red, green, blue, alpha));
		}
		public static void SpriteFillColor_255_Set(string uniqueName, int red, int green, int blue, int alpha = 255)
        {
			if (Exists_Check(uniqueName) == false)
			{
				return;
			}
			thingSpriteFillColors[uniqueName] = new Color(red, green, blue, alpha);

			AddComponent(uniqueName, Components.SpriteFillColor);
			render = true;
		}
		public static void SpriteOutlineColor_255_Set(string uniqueName, int red, int green, int blue, int alpha = 255)
		{
			if (Exists_Check(uniqueName) == false)
			{
				return;
			}
			thingSpriteOutlineColors[uniqueName] = new Color(red, green, blue, alpha);

			AddComponent(uniqueName, Components.SpriteOutlineColor);
			render = true;
		}
		public static Color SpriteColor_Get(string uniqueName)
		{
			return thingSpriteColors.ContainsKey(uniqueName) ? thingSpriteColors[uniqueName] : default;
		}
		public static Color SpriteFillColor_Get(string uniqueName)
		{
			return thingSpriteFillColors.ContainsKey(uniqueName) ? thingSpriteFillColors[uniqueName] : default;
		}
		public static void SpriteFillColor_Delete(string uniqueName)
        {
			if (Exists_Check(uniqueName) == false || thingSpriteFillColors.ContainsKey(uniqueName) == false)
			{
				return;
			}
			thingSpriteFillColors.Remove(uniqueName);

			RemoveComponent(uniqueName, Components.SpriteFillColor);
			render = true;
		}
		public static Color SpriteOutlineColor_Get(string uniqueName)
		{
			return thingSpriteOutlineColors.ContainsKey(uniqueName) ? thingSpriteOutlineColors[uniqueName] : default;
		}
		public static void SpriteOutlineColor_Delete(string uniqueName)
		{
			if (Exists_Check(uniqueName) == false || thingSpriteOutlineColors.ContainsKey(uniqueName) == false)
			{
				return;
			}
			thingSpriteOutlineColors.Remove(uniqueName);

			RemoveComponent(uniqueName, Components.SpriteOutlineColor);
			render = true;
		}
		public static void SpriteColor_Delete(string uniqueName)
		{
			if (Exists_Check(uniqueName) == false || thingSpriteColors.ContainsKey(uniqueName) == false)
			{
				return;
			}
			thingSpriteColors.Remove(uniqueName);

			RemoveComponent(uniqueName, Components.SpriteColor);
			render = true;
		}
		public static void SpriteVisibility_Activate(string uniqueName, bool visible)
		{
			if (Exists_Check(uniqueName) == false)
			{
				return;
			}
			thingSpritesVisible[uniqueName] = visible;

			AddComponent(uniqueName, Components.SpriteVisibility);
			render = true;
		}
		public static bool SpriteVisibility_IsVisible_Check(string uniqueName)
		{
			return uniqueName == null || thingSpritesVisible.ContainsKey(uniqueName) == false ? default : thingSpritesVisible[uniqueName];
		}
		public static void SpriteVisibility_Delete(string uniqueName)
		{
			if (Exists_Check(uniqueName) == false || thingSpritesVisible.ContainsKey(uniqueName) == false)
			{
				return;
			}
			thingSpritesVisible.Remove(uniqueName);

			RemoveComponent(uniqueName, Components.SpriteVisibility);
			render = true;
		}
		public static void SpriteFillVisibility_Activate(string uniqueName, bool visible)
        {
			if (Exists_Check(uniqueName) == false)
			{
				return;
			}
			thingSpriteFillsVisible[uniqueName] = visible;
			if (thingSpriteFillColors.ContainsKey(uniqueName) == false)
			{
				SpriteFillColor_255_Set(uniqueName, 255, 255, 255);
			}

			AddComponent(uniqueName, Components.SpriteFillVisiblity);
			render = true;
		}
		public static bool SpriteFillVisibility_IsVisible_Check(string uniqueName)
		{
			return uniqueName == null || thingSpriteFillsVisible.ContainsKey(uniqueName) == false ? default : thingSpriteFillsVisible[uniqueName];
		}
		public static void SpriteFillVisibility_Delete(string uniqueName)
		{
			if (Exists_Check(uniqueName) == false || thingSpriteFillsVisible.ContainsKey(uniqueName) == false)
			{
				return;
			}
			thingSpriteFillsVisible.Remove(uniqueName);

			RemoveComponent(uniqueName, Components.SpriteFillVisiblity);
			render = true;
		}
		public static void SpriteOutlineVisibility_Activate(string uniqueName, bool visible)
		{
			if (Exists_Check(uniqueName) == false)
			{
				return;
			}
			thingSpriteOutlinesVisible[uniqueName] = visible;
			if (thingSpriteOutlineColors.ContainsKey(uniqueName) == false)
			{
				SpriteOutlineColor_255_Set(uniqueName, 255, 255, 255);
			}

			AddComponent(uniqueName, Components.SpriteOutlineVisibility);
			render = true;
		}
		public static bool SpriteOutlineVisibility_IsVisible_Check(string uniqueName)
		{
			return uniqueName == null || thingSpriteOutlinesVisible.ContainsKey(uniqueName) == false ? default : thingSpriteOutlinesVisible[uniqueName];
		}
		public static void SpriteOutlineVisibility_Delete(string uniqueName)
		{
			if (Exists_Check(uniqueName) == false || thingSpriteOutlinesVisible.ContainsKey(uniqueName) == false)
			{
				return;
			}
			thingSpriteOutlinesVisible.Remove(uniqueName);

			RemoveComponent(uniqueName, Components.SpriteOutlineVisibility);
			render = true;
		}

		public static void TextFont_Set(string uniqueName, string path)
		{
			if (Exists_Check(uniqueName) == false || path == null || fonts.ContainsKey(path) == false)
			{
				return;
			}
			thingTextFonts[uniqueName] = path;

			AddComponent(uniqueName, Components.TextFont);
			render = true;
		}
		public static string TextFont_Get(string uniqueName)
		{
			return uniqueName != null && thingTextFonts.ContainsKey(uniqueName) ? thingTextFonts[uniqueName] : default;
		}
		public static void TextFont_Delete(string uniqueName)
		{
			if (Exists_Check(uniqueName) == false || thingTextFonts.ContainsKey(uniqueName) == false)
			{
				return;
			}
			thingTextFonts.Remove(uniqueName);

			RemoveComponent(uniqueName, Components.TextFont);
			render = true;
		}
		public static void Text_Set<T>(string uniqueName, T message)
		{
			if (Exists_Check(uniqueName) == false)
			{
				return;
			}
			thingTexts[uniqueName] = message.ToString();
			SetDefaultThing(uniqueName);
			SetDefaultText(uniqueName);

			AddComponent(uniqueName, Components.Text);
			render = true;
		}
		public static string Text_Get(string uniqueName)
		{
			return uniqueName != null && thingTexts.ContainsKey(uniqueName) ? thingTexts[uniqueName] : default;
		}
		public static void Text_Delete(string uniqueName)
		{
			if (Exists_Check(uniqueName) == false || thingTexts.ContainsKey(uniqueName) == false)
			{
				return;
			}
			thingTexts.Remove(uniqueName);

			RemoveComponent(uniqueName, Components.Text);
			render = true;
		}
		public static void TextLineHeight_Set(string uniqueName, double lineHeight)
		{
			if (Exists_Check(uniqueName) == false)
			{
				return;
			}
			thingTextLineHeights[uniqueName] = lineHeight;

			AddComponent(uniqueName, Components.TextLineHeight);
			render = true;
		}
		public static double TextLineHeight_Get(string uniqueName)
		{
			return thingTextLineHeights.ContainsKey(uniqueName) ? thingTextLineHeights[uniqueName] : default;
		}
		public static void TextLineHeight_Delete(string uniqueName)
		{
			if (Exists_Check(uniqueName) == false || thingTextLineHeights.ContainsKey(uniqueName) == false)
			{
				return;
			}
			thingTextLineHeights.Remove(uniqueName);

			RemoveComponent(uniqueName, Components.TextLineHeight);
			render = true;
		}
		public static void TextColor_Set(string uniqueName, Color color)
		{
			if (Exists_Check(uniqueName) == false)
			{
				return;
			}
			thingTextColors[uniqueName] = color;

			AddComponent(uniqueName, Components.TextColor);
			render = true;
		}
		public static void TextColor_1_Set(string uniqueName, double red, double green, double blue, double alpha = 1)
		{
			TextColor_Set(uniqueName, new Color((float)red, (float)green, (float)blue, (float)alpha));
		}
		public static void TextColor_255_Set(string uniqueName, int red, int green, int blue, int alpha = 255)
		{
			TextColor_Set(uniqueName, new Color(red, green, blue, alpha));
		}
		public static Color TextColor_Get(string uniqueName)
		{
			return thingTextColors.ContainsKey(uniqueName) ? thingTextColors[uniqueName] : default;
		}
		public static void TextColor_Delete(string uniqueName)
		{
			if (Exists_Check(uniqueName) == false || thingTextColors.ContainsKey(uniqueName) == false)
			{
				return;
			}
			thingTextColors.Remove(uniqueName);

			RemoveComponent(uniqueName, Components.TextColor);
			render = true;
		}
		public static void TextVisibility_Activate(string uniqueName, bool visible)
		{
			if (Exists_Check(uniqueName) == false)
			{
				return;
			}
			thingTextsVisible[uniqueName] = visible;

			AddComponent(uniqueName, Components.TextVisibility);
			render = true;
		}
		public static bool TextVisibility_IsVisible_Check(string uniqueName)
		{
			return thingTextsVisible.ContainsKey(uniqueName) == false ? default : thingTextsVisible[uniqueName];
		}
		public static void TextVisibility_Delete(string uniqueName)
		{
			if (Exists_Check(uniqueName) == false || thingTextsVisible.ContainsKey(uniqueName) == false)
			{
				return;
			}
			thingTextsVisible.Remove(uniqueName);

			RemoveComponent(uniqueName, Components.TextVisibility);
			render = true;
		}
		public static void TextAnchor_Set(string uniqueName, TextAnchor textAnchor)
		{
			if (Exists_Check(uniqueName) == false)
			{
				return;
			}
			thingTextAnchors[uniqueName] = textAnchor;

			AddComponent(uniqueName, Components.TextAnchor);
			render = true;
		}
		public static TextAnchor TextAnchor_Get(string uniqueName)
		{
			return thingTextAnchors.ContainsKey(uniqueName) ? thingTextAnchors[uniqueName] : TextAnchor.TopLeft;
		}
		public static void TextAnchor_Delete(string uniqueName)
		{
			if (Exists_Check(uniqueName) == false || thingTextAnchors.ContainsKey(uniqueName) == false)
			{
				return;
			}
			thingTextAnchors.Remove(uniqueName);

			RemoveComponent(uniqueName, Components.TextAnchor);
			render = true;
		}

		public static void Position_Set(string uniqueName, Vector2 position)
		{
			if (Exists_Check(uniqueName) == false)
			{
				return;
			}
			thingPositions[uniqueName] = position;
			ChildrenUpdate(uniqueName, true, false, false);

			AddComponent(uniqueName, Components.Position);
			AddUpdatePoint(uniqueName);
			render = true;
		}
		public static void Position_Set(string uniqueName, double x, double y)
		{
			Position_Set(uniqueName, new Vector2((float)x, (float)y));
		}
		public static Vector2 Position_Get(string uniqueName)
		{
			return uniqueName == null || thingPositions.ContainsKey(uniqueName) == false ? default : thingPositions[uniqueName];
		}
		public static void Position_Delete(string uniqueName)
		{
			if (Exists_Check(uniqueName) == false || thingPositions.ContainsKey(uniqueName) == false)
			{
				return;
			}
			thingPositions.Remove(uniqueName);

			RemoveComponent(uniqueName, Components.Position);
			render = true;
		}
		public static double Position_DistanceToThing_Get(string uniqueName, string targetUniqueName)
		{
			return Position.DistanceToPosition_Get(Position_Get(uniqueName), Position_Get(targetUniqueName));
		}

		public static void CameraScrollPreventionPercent_Set(string uniqueName, double percent)
		{
			if (Exists_Check(uniqueName) == false)
			{
				return;
			}
			percent = Number.Limited_Get(percent, -100, 100);
			var value = percent / 100;

			thingCameraScrollPrevention[uniqueName] = value;
			AddComponent(uniqueName, Components.CameraScrollPrevention);
			render = true;
		}
		public static double CameraScrollPreventionPercent_Get(string uniqueName)
		{
			return uniqueName != null && thingCameraScrollPrevention.ContainsKey(uniqueName) ? thingCameraScrollPrevention[uniqueName] * 100 : default;
		}
		public static void CameraScrollPrevention_Delete(string uniqueName)
		{
			if (Exists_Check(uniqueName) == false || thingCameraScrollPrevention.ContainsKey(uniqueName) == false)
			{
				return;
			}
			thingCameraScrollPrevention.Remove(uniqueName);
			RemoveComponent(uniqueName, Components.CameraScrollPrevention);
		}

		public static void Direction_Set(string uniqueName, Vector2 direction)
		{
			if (Exists_Check(uniqueName) == false)
			{
				return;
			}
			thingDirections[uniqueName] = direction;
			thingAngles[uniqueName] = Angle.FromDirection_Get(direction);
			ChildrenUpdate(uniqueName, false, true, false);

			AddComponent(uniqueName, Components.Direction);
			AddComponent(uniqueName, Components.Angle);
			AddUpdatePoint(uniqueName);
			render = true;
		}
		public static void Direction_Set(string uniqueName, Direction.Directions direction)
		{
			Direction_Set(uniqueName, directions[direction]);
		}
		public static Vector2 Direction_Get(string uniqueName)
		{
			return thingDirections.ContainsKey(uniqueName) ? thingDirections[uniqueName] : default;
		}
		public static void Direction_Delete(string uniqueName)
		{
			Angle_Delete(uniqueName);
		}
		public static void Angle_Set(string uniqueName, double angle)
		{
			if (Exists_Check(uniqueName) == false)
			{
				return;
			}
			thingAngles[uniqueName] = angle;
			thingDirections[uniqueName] = Direction.FromAngle_Get(angle);
			ChildrenUpdate(uniqueName, false, true, false);

			AddComponent(uniqueName, Components.Angle);
			AddComponent(uniqueName, Components.Direction);
			AddUpdatePoint(uniqueName);
			render = true;
		}
		public static double Angle_Get(string uniqueName)
		{
			return uniqueName != null && thingAngles.ContainsKey(uniqueName) ? thingAngles[uniqueName] : default;
		}
		public static void Angle_Delete(string uniqueName)
		{
			if (Exists_Check(uniqueName) == false || thingAngles.ContainsKey(uniqueName) == false)
			{
				return;
			}
			thingAngles.Remove(uniqueName);
			thingDirections.Remove(uniqueName);

			RemoveComponent(uniqueName, Components.Angle);
			RemoveComponent(uniqueName, Components.Direction);
			render = true;
		}

		public static void Scale_Set(string uniqueName, Vector2 scale)
		{
			if (Exists_Check(uniqueName) == false)
			{
				return;
			}
			thingScales[uniqueName] = scale;
			thingSizes[uniqueName] = new Vector2(GetSpriteSize(uniqueName).X * scale.X, GetSpriteSize(uniqueName).Y * scale.Y);
			ChildrenUpdate(uniqueName, false, false, true);

			AddComponent(uniqueName, Components.Scale);
			AddComponent(uniqueName, Components.Size);
			AddUpdatePoint(uniqueName);
			render = true;
		}
		public static void Scale_Set(string uniqueName, double scaleWidth, double scaleHeight)
		{
			Scale_Set(uniqueName, new Vector2((float)scaleWidth, (float)scaleHeight));
		}
		public static Vector2 Scale_Get(string uniqueName)
		{
			return uniqueName != null && thingScales.ContainsKey(uniqueName) ? thingScales[uniqueName] : default;
		}
		public static void Scale_Delete(string uniqueName)
		{
			Size_Delete(uniqueName);
		}
		public static void Size_Set(string uniqueName, Vector2 size)
		{
			if (Exists_Check(uniqueName) == false)
			{
				return;
			}
			thingSizes[uniqueName] = size;
			thingScales[uniqueName] = new Vector2(size.X / GetSpriteSize(uniqueName).X, size.Y / GetSpriteSize(uniqueName).Y);
			ChildrenUpdate(uniqueName, false, false, true);

			AddComponent(uniqueName, Components.Scale);
			AddComponent(uniqueName, Components.Size);
			AddUpdatePoint(uniqueName);
			render = true;
		}
		public static void Size_Set(string uniqueName, double width, double height)
		{
			Size_Set(uniqueName, new Vector2((float)width, (float)height));
		}
		public static Vector2 Size_Get(string uniqueName)
		{
			return uniqueName != null && thingSizes.ContainsKey(uniqueName) ? thingSizes[uniqueName] : default;
		}
		public static void Size_Delete(string uniqueName)
		{
			if (Exists_Check(uniqueName) == false || thingSizes.ContainsKey(uniqueName) == false)
			{
				return;
			}
			thingSizes.Remove(uniqueName);
			thingScales.Remove(uniqueName);

			RemoveComponent(uniqueName, Components.Size);
			RemoveComponent(uniqueName, Components.Scale);
			render = true;
		}

		public static void OriginOffset_Set(string uniqueName, Vector2 offsetPoint)
		{
			if (Exists_Check(uniqueName) == false)
			{
				return;
			}
			var scale = thingScales.ContainsKey(uniqueName) ? thingScales[uniqueName] : Vector2.One;
			thingOffsets[uniqueName] = offsetPoint;

			AddComponent(uniqueName, Components.OriginOffset);
			AddUpdatePoint(uniqueName);
			render = true;
		}
		public static void OriginOffset_Set(string uniqueName, double x, double y)
		{
			OriginOffset_Set(uniqueName, new Vector2((float)x, (float)y));
		}
		public static Vector2 OriginOffset_Get(string uniqueName)
		{
			return uniqueName == null || thingOffsets.ContainsKey(uniqueName) == false ? default : thingOffsets[uniqueName];
		}
		public static void OriginOffset_Delete(string uniqueName)
		{
			if (Exists_Check(uniqueName) == false || thingOffsets.ContainsKey(uniqueName) == false)
			{
				return;
			}

			RemoveComponent(uniqueName, Components.OriginOffset);
			render = true;
		}

		public static void Parent_Set(string uniqueName, string parentUniqueName)
		{
			Child_Add(parentUniqueName, uniqueName);
		}
		public static string Parent_UniqueName_Get(string uniqueName)
		{
			return thingParents.ContainsKey(uniqueName) ? thingParents[uniqueName] : default;
		}
		public static void Parent_Remove(string uniqueName)
		{
			Child_Remove(Parent_UniqueName_Get(uniqueName), uniqueName);
		}

		public static void Child_Add(string uniqueName, string childUniqueName)
		{
			if (Exists_Check(uniqueName) == false || Exists_Check(childUniqueName) == false || uniqueName == childUniqueName)
			{
				return;
			}
			if (thingChildren.ContainsKey(childUniqueName) || thingParents.ContainsKey(childUniqueName))
			{
				Parent_Remove(childUniqueName);
			}
			if (thingChildren.ContainsKey(uniqueName) || thingParents.ContainsKey(uniqueName))
			{
				Parent_Remove(uniqueName);
			}
			if (thingChildren.ContainsKey(uniqueName) == false)
			{
				thingChildren[uniqueName] = new List<string>();
			}
			if (thingChildren[uniqueName].Contains(childUniqueName) == false)
			{
				thingChildren[uniqueName].Add(childUniqueName);
			}

			if (thingParents.ContainsKey(childUniqueName) == false)
			{
				thingParents[childUniqueName] = uniqueName;
			}

			var childPos = Position_Get(childUniqueName);
			var parentAngle = Angle_Get(uniqueName);
			var parentPos = Position_Get(uniqueName);
			thingParentAngleDifference[childUniqueName] = parentAngle - Angle_Get(childUniqueName);
			thingParentInDistance[childUniqueName] = Vector2.Distance(parentPos, childPos);
			thingParentAtAngleDifference[childUniqueName] = parentAngle - Angle.FromDirection_Get(childPos - parentPos);
			thingParentSizeDifference[childUniqueName] = Size_Get(uniqueName) - Size_Get(childUniqueName);

			AddComponent(uniqueName, Components.Family);
			AddComponent(childUniqueName, Components.Family);
			AddUpdatePoint(uniqueName);
			render = true;
		}
		public static List<string> Child_All_UniqueNames_Get(string uniqueName)
		{
			return thingChildren.ContainsKey(uniqueName) ? new List<string>(thingChildren[uniqueName]) : new List<string>();
		}
		public static void Child_RelativityToParentPosition_Activate(string uniqueName, bool relative)
		{
			if (Exists_Check(uniqueName) == false)
			{
				return;
			}
			CheckRelativityExistence(uniqueName);
			thingParentRelatives[uniqueName][0] = relative;
			render = true;
		}
		public static void Child_RelativityToParentAngle_Activate(string uniqueName, bool relative)
		{
			if (Exists_Check(uniqueName) == false)
			{
				return;
			}
			CheckRelativityExistence(uniqueName);
			thingParentRelatives[uniqueName][1] = relative;
			render = true;
		}
		public static void Child_RelativityToParentDirection_Activate(string uniqueName, bool relative)
		{
			Child_RelativityToParentAngle_Activate(uniqueName, relative);
		}
		public static void Child_RelativityToParentSize_Activate(string uniqueName, bool relative)
		{
			if (Exists_Check(uniqueName) == false)
			{
				return;
			}
			CheckRelativityExistence(uniqueName);
			thingParentRelatives[uniqueName][2] = relative;
			render = true;
		}
		public static void Child_RelativityToParentScale_Activate(string uniqueName, bool relative)
		{
			Child_RelativityToParentSize_Activate(uniqueName, relative);
		}
		public static void Child_Remove(string uniqueName, string childUniqueName)
		{
			if (Exists_Check(uniqueName) == false || Exists_Check(childUniqueName) == false)
			{
				return;
			}
			if (thingChildren.ContainsKey(uniqueName) == false || thingChildren[uniqueName].Contains(childUniqueName) == false)
			{
				return;
			}
			thingParents.Remove(childUniqueName);
			thingChildren[uniqueName].Remove(childUniqueName);
			if (thingChildren[uniqueName].Count == 0)
			{
				thingChildren.Remove(uniqueName);
			}
			render = true;
		}
		public static void Family_Remove(string uniqueName)
		{
			if (Exists_Check(uniqueName) == false)
			{
				return;
			}
			if (thingParents.ContainsKey(uniqueName))
			{
				thingParents.Remove(uniqueName);
			}
			if (thingChildren.ContainsKey(uniqueName))
			{
				thingChildren.Remove(uniqueName);
			}
			var keyChildren = "";
			var keyParents = "";
			foreach (var kvp in thingChildren)
			{
				if (kvp.Value.Contains(uniqueName))
				{
					keyChildren = kvp.Key;
				}
			}
			foreach (var kvp in thingParents)
			{
				if (kvp.Value == uniqueName)
				{
					keyParents = kvp.Key;
				}
			}
			if (keyChildren != "")
			{
				thingChildren.Remove(keyChildren);
			}
			if (keyParents != "")
			{
				thingParents.Remove(keyParents);
			}

			RemoveComponent(uniqueName, Components.Family);
			render = true;
		}

		public static void Animation_Add(string uniqueName, string animationName, double duration = 0, AnimationRepeatStates repeatState = AnimationRepeatStates.None)
		{
			if (Animation_Exists_Check(uniqueName, animationName) == false)
			{
				return;
			}
			if (thingAnimationDurations.ContainsKey(uniqueName) == false)
			{
				thingAnimationDurations[uniqueName] = new Dictionary<string, double>();
			}
			if (thingAnimationRepeatStates.ContainsKey(uniqueName) == false)
			{
				thingAnimationRepeatStates[uniqueName] = new Dictionary<string, Thing.AnimationRepeatStates>();
			}
			thingAnimationDurations[uniqueName][animationName] = duration;
			thingAnimationRepeatStates[uniqueName][animationName] = repeatState;

			AddComponent(uniqueName, Components.Animation);
		}
		public static void Animation_Sprite_Add(string uniqueName, string animationName, string spritePath)
		{
			if (Animation_Exists_Check(uniqueName, animationName) == false)
			{
				return;
			}
			if (thingAnimationSprites.ContainsKey(uniqueName) == false)
			{
				thingAnimationSprites[uniqueName] = new Dictionary<string, List<string>>();
			}
			if (thingAnimationSprites[uniqueName].ContainsKey(animationName) == false)
			{
				thingAnimationSprites[uniqueName][animationName] = new List<string>();
			}
			thingAnimationSprites[uniqueName][animationName].Add(spritePath);
		}
		public static void Animation_ChangeSprite_Add(string uniqueName, string animationName, double durationLower, double durationUpper, int frameIndex)
		{
			if (Animation_Exists_Check(uniqueName, animationName) == false)
			{
				return;
			}
			var range = new double[] { durationLower, durationUpper };
			if (thingAnimationChangesSpriteIndex.ContainsKey(uniqueName) == false)
			{
				thingAnimationChangesSpriteIndex[uniqueName] = new Dictionary<string, Dictionary<double[], int>>();
				thingAnimationChangesSpriteIndexTriggered[uniqueName] = new Dictionary<string, Dictionary<double[], bool>>();
			}
			if (thingAnimationChangesSpriteIndex[uniqueName].ContainsKey(animationName) == false)
			{
				thingAnimationChangesSpriteIndex[uniqueName][animationName] = new Dictionary<double[], int>();
				thingAnimationChangesSpriteIndexTriggered[uniqueName][animationName] = new Dictionary<double[], bool>();
			}
			thingAnimationChangesSpriteIndex[uniqueName][animationName][range] = frameIndex;
			thingAnimationChangesSpriteIndexTriggered[uniqueName][animationName][range] = false;

			AddComponent(uniqueName, Components.AnimationChangeSprite);
		}
		public static void Animation_Start(string uniqueName, string animationName)
		{
			if (Animation_Exists_Check(uniqueName, animationName) == false)
			{
				return;
			}
			thingCurrentAnimationName[uniqueName] = animationName;
			thingCurrentAnimationRunning[uniqueName] = true;
			thingCurrentAnimationForward[uniqueName] = true;
			thingCurrentAnimationProgress[uniqueName] = 0;
			thingCurrentAnimationIndex[uniqueName] = 0;
			render = true;
		}
		public static void Animation_Current_Stop(string uniqueName)
		{
			var animationName = Animation_CurrentName_Get(uniqueName);
			if (Animation_Exists_Check(uniqueName, animationName) == false)
			{
				return;
			}
			thingCurrentAnimationName[uniqueName] = default;
			thingCurrentAnimationRunning[uniqueName] = default;
			thingCurrentAnimationForward[uniqueName] = default;
			thingCurrentAnimationProgress[uniqueName] = default;
			thingCurrentAnimationIndex[uniqueName] = default;
			RestartAllTriggers(uniqueName, animationName);
			render = true;
		}
		public static void Animation_Name_Set(string uniqueName, string animationName, string newAnimationName)
		{
			if (Animation_Exists_Check(uniqueName, animationName) == false || Animation_Exists_Check(uniqueName, newAnimationName))
			{
				return;
			}
			thingAnimationSprites[uniqueName][newAnimationName] = thingAnimationSprites[uniqueName][animationName];
			thingAnimationSprites[uniqueName].Remove(animationName);
		}
		public static void Animation_CurrentSpriteIndex_Set(string uniqueName, int index)
		{
			if (Exists_Check(uniqueName) == false)
			{
				return;
			}
			thingCurrentAnimationIndex[uniqueName] = index;
			render = true;
		}
		public static string Animation_SpriteName_Get(string uniqueName, string animationName, int frameIndex)
		{
			return Animation_Exists_Check(uniqueName, animationName) ? thingAnimationSprites[uniqueName][animationName][frameIndex] : default;
		}
		public static int Animation_SpriteCount_Get(string uniqueName, string animationName)
		{
			return Animation_Exists_Check(uniqueName, animationName) ? thingAnimationSprites[uniqueName][animationName].Count : default;
		}
		public static string Animation_CurrentName_Get(string uniqueName)
		{
			return thingCurrentAnimationName.ContainsKey(uniqueName) ? thingCurrentAnimationName[uniqueName] : default;
		}
		public static double Animation_CurrentPercentProgress_Get(string uniqueName)
		{
			return thingCurrentAnimationProgress.ContainsKey(uniqueName) ?
				thingCurrentAnimationProgress[uniqueName] / thingAnimationDurations[uniqueName][Animation_CurrentName_Get(uniqueName)] * 100 : default;
		}
		public static int Animation_CurrentSpriteIndex_Get(string uniqueName)
		{
			return thingCurrentAnimationIndex.ContainsKey(uniqueName) ? thingCurrentAnimationIndex[uniqueName] : default;
		}
		public static double Animation_CurrentProgress_Get(string uniqueName)
		{
			return thingCurrentAnimationProgress.ContainsKey(uniqueName) ? thingCurrentAnimationProgress[uniqueName] : default;
		}
		public static double Animation_Duration_Get(string uniqueName, string animationName)
		{
			return Animation_Exists_Check(uniqueName, animationName) ? thingAnimationDurations[uniqueName][animationName] : default;
		}
		public static AnimationRepeatStates Animation_RepeatState_Get(string uniqueName, string animationName)
		{
			return Animation_Exists_Check(uniqueName, animationName) ? thingAnimationRepeatStates[uniqueName][animationName] : default;
		}
		public static void Animation_CurrentPause_Activate(string uniqueName, bool paused)
		{
			if (Animation_CurrentName_Get(uniqueName) == null)
			{
				return;
			}
			thingCurrentAnimationRunning[uniqueName] = paused;
			render = true;
		}
		public static bool Animation_Exists_Check(string uniqueName, string animationName)
		{
			return Exists_Check(uniqueName) || (thingAnimationSprites.ContainsKey(uniqueName) && thingAnimationSprites[uniqueName].ContainsKey(animationName));
		}
		public static bool Animation_CurrentIsRunning_Check(string uniqueName)
		{
			return thingCurrentAnimationRunning.ContainsKey(uniqueName) == false ? default : thingCurrentAnimationRunning[uniqueName];
		}
		public static bool Animation_CurrentIsGoingForward_Check(string uniqueName)
		{
			return thingCurrentAnimationForward.ContainsKey(uniqueName) == false ? default : thingCurrentAnimationForward[uniqueName];
		}
		public static void Animation_Delete(string uniqueName)
		{
			if (Exists_Check(uniqueName) == false || thingAnimationDurations.ContainsKey(uniqueName) == false)
			{
				return;
			}
			thingAnimationDurations.Remove(uniqueName);
			thingAnimationRepeatStates.Remove(uniqueName);
			if (thingAnimationSprites.ContainsKey(uniqueName))
			{
				thingAnimationSprites.Remove(uniqueName);
			}
			if (thingAnimationChangesSpriteIndex.ContainsKey(uniqueName))
			{
				thingAnimationChangesSpriteIndex.Remove(uniqueName);
				thingAnimationChangesSpriteIndexTriggered.Remove(uniqueName);
			}

			RemoveComponent(uniqueName, Components.Animation);
			render = true;
		}
		public static List<string> Animation_Names_All_Get(string uniqueName)
        {
			return thingAnimationDurations.ContainsKey(uniqueName) ? new List<string>(DictionaryKeysGet(thingAnimationDurations[uniqueName])) : new List<string>();
        }
		public static List<string> Animation_SpriteNames_All_Get(string uniqueName, string animationName)
        {
			return thingAnimationSprites.ContainsKey(uniqueName) && thingAnimationSprites[uniqueName].ContainsKey(animationName) ? new List<string>(thingAnimationSprites[uniqueName][animationName]) : new List<string>();
		}

		public static void Overlap_Activate(string uniqueName, bool activated)
		{
			if (Exists_Check(uniqueName) == false)
			{
				return;
			}
			thingOverlapsActivated[uniqueName] = activated;

			AddComponent(uniqueName, Components.Overlap);
			AddUpdatePoint(uniqueName);
		}
		public static List<string> Overlap_All_JustHovered_Get()
		{
			return new List<string>(thingsJustHovered);
		}
		public static List<string> Overlap_All_JustUnhovered_Get()
		{
			return new List<string>(thingsJustUnhovered);
		}
		public static List<string> Overlap_All_UnderMouseCursor_Get()
		{
			return new List<string>(thingsHovered);
		}
		public static bool Overlap_AreHoveredByMouseCursor_Check(List<string> uniqueNames)
		{
			for (int i = 0; i < uniqueNames.Count; i++)
			{
				if (Components_Get(uniqueNames[i]).Contains(Components.Overlap) == false)
				{
					continue;
				}
				if (Overlap_IsHoveredByMouseCursor_Check(uniqueNames[i]) == false)
				{
					return false;
				}
			}
			return true;
		}
		public static bool Overlap_AreOverlapping_Check(string uniqueName1, string uniqueName2)
		{
			if (Components_Get(uniqueName1).Contains(Components.Overlap) == false ||
				Components_Get(uniqueName2).Contains(Components.Overlap) == false)
			{
				return false;
			}
			for (int i = 0; i < 4; i++)
			{
				var overlaps1 = Overlap_IsOverlappingPosition_Check(uniqueName1, thingPoints[uniqueName2][i]);
				var overlaps2 = Overlap_IsOverlappingPosition_Check(uniqueName2, thingPoints[uniqueName1][i]);

				if (overlaps1 || overlaps2)
				{
					return true;
				}
			}
			return false;
		}
		public static bool Overlap_IsHoveredByMouseCursor_Check(string uniqueName)
		{
			if (Exists_Check(uniqueName) == false || Components_Get(uniqueName).Contains(Components.Overlap) == false)
			{
				return false;
			}
			return Overlap_IsOverlappingPosition_Check(uniqueName, Input.MouseCursorPositionWorld_Get());
		}
		public static bool Overlap_IsOverlappingPosition_Check(string uniqueName, Vector2 position)
		{
			if (Exists_Check(uniqueName) == false || Components_Get(uniqueName).Contains(Components.Overlap) == false)
			{
				return false;
			}

			var distance = 999999;
			var ang = Angle_Get(uniqueName);

			var topMiddle = Position.PercentedTowardsPosition_Get(thingPoints[uniqueName][0], thingPoints[uniqueName][1], 50);
			var top = topMiddle + Direction.FromAngle_Get(ang + 270) * distance;

			var leftMiddle = Position.PercentedTowardsPosition_Get(thingPoints[uniqueName][0], thingPoints[uniqueName][2], 50);
			var left = leftMiddle + Direction.FromAngle_Get(ang + 180) * distance;

			var rightMiddle = Position.PercentedTowardsPosition_Get(thingPoints[uniqueName][1], thingPoints[uniqueName][3], 50);
			var right = rightMiddle + Direction.FromAngle_Get(ang) * distance;

			var bottomMiddle = Position.PercentedTowardsPosition_Get(thingPoints[uniqueName][2], thingPoints[uniqueName][3], 50);
			var bottom = bottomMiddle + Direction.FromAngle_Get(ang + 90) * distance;

			var distanceTop = Vector2.Distance(top, position);
			var distanceLeft = Vector2.Distance(left, position);
			var distanceRight = Vector2.Distance(right, position);
			var distanceBottom = Vector2.Distance(bottom, position);

			var result = distanceTop > distance && distanceLeft > distance && distanceRight > distance && distanceBottom > distance;

			return result;
		}
		public static bool Overlap_IsActivated_Check(string uniqueName)
        {
			return thingOverlapsActivated.ContainsKey(uniqueName) == false ? default : thingOverlapsActivated[uniqueName];
		}
		public static void Overlap_Delete(string uniqueName)
		{
			if (Exists_Check(uniqueName) == false || thingPoints.ContainsKey(uniqueName) == false)
			{
				return;
			}
			thingPoints.Remove(uniqueName);

			RemoveComponent(uniqueName, Components.Overlap);
		}

		public static void InStorageSave_Set(string uniqueName)
        {
			if (uniqueName == null || Exists_Check(uniqueName) == false)
            {
				return;
            }
			var key = uniqueName;
			var data = "";
			var allComponents = (Components[])Enum.GetValues(typeof(Components));
			foreach (var component in allComponents)
            {
                switch (component)
                {
					case Components.UniqueName: continue;
                    case Components.Tags:
						{
							var tags = Tags_Get(uniqueName);
							for (int i = 0; i < tags.Count; i++)
                            {
                                if (i != 0)
                                {
									data = $"{data},";
								}
								data = $"{data}{tags[i]}";
							}
							break;
						}
                    case Components.Position: data = $"{data}{Position_Get(uniqueName).X},{Position_Get(uniqueName).Y}"; break;
                    case Components.Angle: data = $"{data}{Angle_Get(uniqueName)}"; break;
					case Components.Direction: continue;
					case Components.Size: data = $"{data}{Size_Get(uniqueName).X},{Size_Get(uniqueName).Y}"; break;
					case Components.Scale: continue;
                    case Components.SpriteTiling: data = $"{data}{GetBoolString(SpriteTiling_IsTiled_Check(uniqueName))}"; break;
                    case Components.Sprite: data = $"{data}{Sprite_Get(uniqueName)}"; break;
                    case Components.SpriteColor: data = $"{data}{SpriteColor_Get(uniqueName).R},{SpriteColor_Get(uniqueName).G},{SpriteColor_Get(uniqueName).B},{SpriteColor_Get(uniqueName).A}"; break;
                    case Components.SpriteFillColor: data = $"{data}{SpriteFillColor_Get(uniqueName).R},{SpriteFillColor_Get(uniqueName).G},{SpriteFillColor_Get(uniqueName).B},{SpriteFillColor_Get(uniqueName).A}"; break;
					case Components.SpriteOutlineColor: data = $"{data}{SpriteOutlineColor_Get(uniqueName).R},{SpriteOutlineColor_Get(uniqueName).G},{SpriteOutlineColor_Get(uniqueName).B},{SpriteOutlineColor_Get(uniqueName).A}"; break;
					case Components.SpriteVisibility: data = $"{data}{GetBoolString(SpriteVisibility_IsVisible_Check(uniqueName))}"; break;
                    case Components.SpriteFillVisiblity: data = $"{data}{GetBoolString(SpriteFillVisibility_IsVisible_Check(uniqueName))}"; break;
                    case Components.SpriteOutlineVisibility: data = $"{data}{GetBoolString(SpriteOutlineVisibility_IsVisible_Check(uniqueName))}"; break;
                    case Components.OriginOffset: data = $"{data}{OriginOffset_Get(uniqueName).X},{OriginOffset_Get(uniqueName).Y}"; break;
                    case Components.Depth: data = $"{data}{Depth_Get(uniqueName)}"; break;
                    case Components.TextFont: data = $"{data}{TextFont_Get(uniqueName)}"; break;
                    case Components.Text: data = $"{data}{Text_Get(uniqueName)}"; break;
                    case Components.TextColor: data = $"{data}{TextColor_Get(uniqueName).R},{TextColor_Get(uniqueName).G},{TextColor_Get(uniqueName).B},{TextColor_Get(uniqueName).A}"; break;
                    case Components.TextVisibility: data = $"{data}{GetBoolString(TextVisibility_IsVisible_Check(uniqueName))}"; break;
                    case Components.TextAnchor: data = $"{data}{(int)TextAnchor_Get(uniqueName)}"; break;
                    case Components.TextLineHeight: data = $"{data}{TextLineHeight_Get(uniqueName)}"; break;
					case Components.Data: continue;
                    case Components.Family:
						{
							data = $"{data}{Parent_UniqueName_Get(uniqueName)}/";
                            foreach (var child in Child_All_UniqueNames_Get(uniqueName))
                            {
								data = $"{data},{child}";
                            }
							break;
						}
                    case Components.Overlap: data = $"{data}{GetBoolString(Overlap_IsActivated_Check(uniqueName))}"; break;
                    case Components.Animation:
						{
							continue;
							/*
                            foreach (var animation in Animation_Names_All_Get(uniqueName))
                            {
								data = $"{data}{animation}/{Animation_Duration_Get(uniqueName, animation)},{Animation_RepeatState_Get(uniqueName, animation)},{(int)Animation_RepeatState_Get(uniqueName, animation)}/";
                                foreach (var sprite in Animation_SpriteNames_All_Get(uniqueName, animation))
                                {
									data = $"{data},{sprite}";
								}
                            }
							break;
							*/
						}
                    case Components.AnimationChangeSprite: continue;
                    case Components.CameraScrollPrevention: data = $"{data}{CameraScrollPreventionPercent_Get(uniqueName)}"; break;
                }
				data = $"{data}|";
			}
			thingStorage[key] = data;

			SaveExportAll(false);

			string GetBoolString(bool value)
            {
				return value ? "1" : "0";
            }
        }
		public static void InStorageSave_All_Set()
        {
            foreach (var thing in thingUniqueNames)
            {
				InStorageSave_Set(thing);
            }
        }
		public static void InStorageLoad_Get(string uniqueName)
        {
            if (uniqueName == null || thingStorage.ContainsKey(uniqueName) == false)
            {
				return;
            }
			var components = thingStorage[uniqueName].Split(Convert.ToChar(thingStorageCompSep));
            if (Exists_Check(uniqueName) == false)
            {
				Create(uniqueName);
            }
			var index = 0;
            for (int i = 0; i < components.Length; i++)
            {
				var component = (Components)index;
                if (component == Components.UniqueName || component == Components.Direction || component == Components.Scale || component == Components.Data || component == Components.Animation)
                {
					index += component == Components.Animation ? 2 : 1;
					component = (Components)index;
				}

				var key = components[i].Split(Convert.ToChar(thingStorageKeySep));
				var data = components[i].Split(Convert.ToChar(thingStorageValueSep));

				switch (component)
				{
					case Components.Tags:
						{
                            if (data[0] == "")
                            {
								break;
                            }
							foreach (var tag in data)
							{
								Tag_Add(uniqueName, tag);
							}
							break;
						}
					case Components.Position:
						{
							if (data[0] == "0" && data[1] == "0")
							{
								break;
							}
							Position_Set(uniqueName, double.Parse(data[0]), double.Parse(data[1]));
							break;
						}
					case Components.Angle:
						{
							if (data[0] == "0")
							{
								break;
							}
							Angle_Set(uniqueName, double.Parse(data[0]));
							break;
						}
					case Components.Size:
						{
							if (data[0] == "0" && data[1] == "0")
							{
								break;
							}
							Size_Set(uniqueName, double.Parse(data[0]), double.Parse(data[1]));
							break;
						}
					case Components.SpriteTiling:
						{
							if (data[0] == "0")
							{
								break;
							}
							SpriteTiling_Activate(uniqueName, data[0] == "1");
							break;
						}
					case Components.Sprite:
						{
							if (data[0] == "")
							{
								break;
							}
							Sprite_Set(uniqueName, data[0]);
							var sizeData = components[3].Split(Convert.ToChar(thingStorageValueSep));
							if (sizeData[0] == "0" && sizeData[1] == "0")
							{
								break;
							}
							Size_Set(uniqueName, double.Parse(sizeData[0]), double.Parse(sizeData[1]));
							break;
						}
					case Components.SpriteColor:
						{
							if (data[0] == "0" && data[1] == "0" && data[2] == "0" && data[3] == "0")
							{
								break;
							}
							SpriteColor_255_Set(uniqueName, int.Parse(data[0]), int.Parse(data[1]), int.Parse(data[2]), int.Parse(data[3]));
							break;
						}
					case Components.SpriteFillColor:
						{
							if (data[0] == "0" && data[1] == "0" && data[2] == "0" && data[3] == "0")
							{
								break;
							}
							SpriteFillColor_255_Set(uniqueName, int.Parse(data[0]), int.Parse(data[1]), int.Parse(data[2]), int.Parse(data[3]));
							break;
						}
					case Components.SpriteOutlineColor:
						{
							if (data[0] == "0" && data[1] == "0" && data[2] == "0" && data[3] == "0")
							{
								break;
							}
							SpriteOutlineColor_255_Set(uniqueName, int.Parse(data[0]), int.Parse(data[1]), int.Parse(data[2]), int.Parse(data[3]));
							break;
						}
					case Components.SpriteVisibility:
						{
							if (data[0] == "0")
							{
								break;
							}
							SpriteVisibility_Activate(uniqueName, data[0] == "1");
							break;
						}
					case Components.SpriteFillVisiblity:
						{
							if (data[0] == "0")
							{
								break;
							}
							SpriteFillVisibility_Activate(uniqueName, data[0] == "1");
							break;
						}
					case Components.SpriteOutlineVisibility:
						{
							if (data[0] == "0")
							{
								break;
							}
							SpriteOutlineVisibility_Activate(uniqueName, data[0] == "1");
							break;
						}
					case Components.OriginOffset:
						{
							if (data[0] == "0" && data[1] == "0")
							{
								break;
							}
							OriginOffset_Set(uniqueName, double.Parse(data[0]), double.Parse(data[1]));
							break;
						}
					case Components.Depth:
						{
							if (data[0] == "0")
							{
								break;
							}
							Depth_Set(uniqueName, double.Parse(data[0]));
							break;
						}
					case Components.TextFont:
						{
							if (data[0] == "")
							{
								break;
							}
							TextFont_Set(uniqueName, data[0]);
							break;
						}
					case Components.Text:
						{
							if (data[0] == "")
							{
								break;
							}
							Text_Set(uniqueName, data[0]);
							break;
						}
					case Components.TextColor:
						{
							if (data[0] == "0" && data[1] == "0" && data[2] == "0" && data[3] == "0")
							{
								break;
							}
							TextColor_255_Set(uniqueName, int.Parse(data[0]), int.Parse(data[1]), int.Parse(data[2]), int.Parse(data[3]));
							break;
						}
					case Components.TextVisibility:
						{
							if (data[0] == "0")
							{
								break;
							}
							TextVisibility_Activate(uniqueName, data[0] == "1");
							break;
						}
					case Components.TextAnchor:
						{
							if (data[0] == "0")
							{
								break;
							}
							TextAnchor_Set(uniqueName, (TextAnchor)int.Parse(data[0]));
							break;
						}
					case Components.TextLineHeight:
						{
							if (data[0] == "0")
							{
								break;
							}
							TextLineHeight_Set(uniqueName, double.Parse(data[0]));
							break;
						}
                    case Components.Family:
                        {
							if (key[0] != "")
							{
								Parent_Set(uniqueName, key[0]);
							}
                            if (data[0] == "")
                            {
								break;
                            }
                            foreach (var child in data)
                            {
								Child_Add(uniqueName, child);
                            }
							break;
                        }
                    case Components.Overlap:
						{
							if (data[0] == "0")
							{
								break;
							}
							Overlap_Activate(uniqueName, data[0] == "1");
							break;
						}
					case Components.Animation: break;
					case Components.AnimationChangeSprite: break;
                    case Components.CameraScrollPrevention:
						{
							if (data[0] == "0")
							{
								break;
							}
							CameraScrollPreventionPercent_Set(uniqueName, double.Parse(data[0]));
							break;
						}
                }
				index++;
            }
        }
		public static void InStorageLoad_All_Get()
        {
            foreach (var kvp in thingStorage)
            {
				InStorageLoad_Get(kvp.Key);
            }
        }

		public static void InStorageSaved_Delete(string uniqueName)
        {
			if (thingStorage.ContainsKey(uniqueName) == false)
			{
				return;
			}
			thingStorage.Remove(uniqueName);
			SaveExportAll(false);
		}
		public static void InStorageSaved_Delete_All()
		{
			thingStorage.Clear();
			File.Delete(thingStoragePath);
		}
	}

	/// <summary>
	/// Controls the point of view and holds information about it.
	/// </summary>
	public static class Camera
	{
		public static void Position_Set(Vector2 position)
		{
			cameraPosition = position;
			thingsUpdatePoints = thingUniqueNames;
			render = true;
		}
		public static void Position_Set(double x, double y)
		{
			Position_Set(new Vector2((float)x, (float)y));
		}
		/// <summary>
		/// Creates a <paramref name="name"/>.png file of what is currently visible in the window then saves it in the main directory and as a sprite.<br></br>
		/// Might be <paramref name="scaledToDefaultResolution"/>.
		/// </summary>
		public static void Screenshot_Create(string name, bool scaledToDefaultResolution)
		{
			int width = scaledToDefaultResolution ? game.GraphicsDevice.PresentationParameters.BackBufferWidth : screenSize.X;
			int height = scaledToDefaultResolution ? game.GraphicsDevice.PresentationParameters.BackBufferHeight : screenSize.Y;
			var buffer = new int[width * height];
			var texture = new Texture2D(game.GraphicsDevice, width, height);

			if (Directory.Exists(screenshotsPath) == false)
			{
				Directory.CreateDirectory(screenshotsPath);
			}

			if (scaledToDefaultResolution)
			{
				game.GraphicsDevice.GetBackBufferData(buffer);
			}
			else
			{
				renderTarget.GetData(0, new Rectangle(0, 0, width, height), buffer, 0, width * height);
			}
			texture.SetData(buffer);
			using (Stream stream = File.Create($"{screenshotsPath}\\{name}.png"))
			{
				texture.SaveAsPng(stream, width, height);
			}
			sprites[name] = texture;
		}
		public static Vector2 Position_Get()
		{
			return cameraPosition;
		}
	}
	/// <summary>
	/// Controls and stores data locally both at runtime and after the game is closed.
	/// </summary>
	public static class Data
	{
		public static void Runtime_Set<T, T1>(T key, T1 data)
		{
			Do.data[key] = data;
		}
		public static object Runtime_Get<T>(T key)
		{
			return data.ContainsKey(key) == false ? default : data[key];
		}
		public static void Runtime_Delete(string key)
		{
			if (data.ContainsKey(key) == false)
			{
				return;
			}
			data.Remove(key);
		}
		public static void Runtime_DeleteAll()
		{
			data.Clear();
		}

		public static void Storage_Set(string key, string data)
		{
			storage[key] = data;
			SaveExportAll(true);
		}
		public static string Storage_Get(string key)
		{
			return storage.ContainsKey(key) == false ? null : storage[key];
		}
		public static void Storage_Delete(string key)
		{
			if (storage.ContainsKey(key) == false)
			{
				return;
			}
			storage.Remove(key);
			SaveExportAll(true);
		}
		public static void Storage_DeleteAll()
		{
			storage.Clear();
			File.Delete(storagePath);
		}
	}

	/// <summary>
	/// Holds information about the current input of the user.
	/// </summary>
	public static class Input
	{
		public enum Keys
		{
			None = 0, BackSpace = 8, Tab = 9, Enter = 13, Pause = 19, CapsLock = 20, Kana = 21, Kanji = 25, Escape = 27, ImeConvert = 28, ImeNoConvert = 29, Space = 32, PageUp = 33, PageDown = 34, End = 35, Home = 36, Left = 37, Up = 38, Right = 39, Down = 40, Select = 41, Print = 42, Execute = 43, PrintScreen = 44, Insert = 45, Delete = 46, Help = 47, _0 = 48, _1 = 49, _2 = 50, _3 = 51, _4 = 52, _5 = 53, _6 = 54, _7 = 55, _8 = 56, _9 = 57, A = 65, B = 66, C = 67, D = 68, E = 69, F = 70, G = 71, H = 72, I = 73, J = 74, K = 75, L = 76, M = 77, N = 78, O = 79, P = 80, Q = 81, R = 82, S = 83, T = 84, U = 85, V = 86, W = 87, X = 88, Y = 89, Z = 90, LeftWindows = 91, RightWindows = 92, Apps = 93, Sleep = 95, Num0 = 96, Num1 = 97, Num2 = 98, Num3 = 99, Num4 = 100, Num5 = 101, Num6 = 102, Num7 = 103, Num8 = 104, Num9 = 105, NumMultiply = 106, NumAdd = 107, Separator = 108, NumSubtract = 109, NumDecimal = 110, NumDivide = 111, F1 = 112, F2 = 113, F3 = 114, F4 = 115, F5 = 116, F6 = 117, F7 = 118, F8 = 119, F9 = 120, F10 = 121, F11 = 122, F12 = 123, F13 = 124, F14 = 125, F15 = 126, F16 = 127, F17 = 128, F18 = 129, F19 = 130, F20 = 131, F21 = 132, F22 = 133, F23 = 134, F24 = 135, NumLock = 144, Scroll = 145, ShiftLeft = 160, ShiftRight = 161, ControlLeft = 162, ControlRight = 163, AltLeft = 164, AltRight = 165, BrowserBack = 166, BrowserForward = 167, BrowserRefresh = 168, BrowserStop = 169, BrowserSearch = 170, BrowserFavorites = 171, BrowserHome = 172, VolumeMute = 173, VolumeDown = 174, VolumeUp = 175, MediaNextTrack = 176, MediaPreviousTrack = 177, MediaStop = 178, MediaPlayPause = 179, LaunchMail = 180, SelectMedia = 181, LaunchApplication1 = 182, LaunchApplication2 = 183, Semicolon = 186, Equals = 187, Comma = 188, Minus_Dash = 189, Dot = 190, Slash = 191, GraveAccent = 192, ChatPadGreen = 202, ChatPadOrange = 203, SquareBracketOpen = 219, Backslash = 220, SquareBracketClose = 221, Quote = 222, Oem8 = 223, OemBackslash = 226, ProcessKey = 229, OemCopy = 242, OemAuto = 243, OemEnlW = 244, Attn = 246, Crsel = 247, Exsel = 248, EraseEof = 249, Play = 250, Zoom = 251, Pa1 = 253, OemClear = 254
		}
		public static string KeyToText_Get(Keys key)
		{
			var shift = KeyIsPressed_Check(Keys.ShiftLeft) || KeyIsPressed_Check(Keys.ShiftRight);
			var result = "";
			switch (key)
			{
				case Keys.Space: result = " "; break;
				case Keys._0: result = shift ? ")" : "0"; break;
				case Keys._1: result = shift ? "!" : "1"; break;
				case Keys._2: result = shift ? "@" : "2"; break;
				case Keys._3: result = shift ? "#" : "3"; break;
				case Keys._4: result = shift ? "$" : "4"; break;
				case Keys._5: result = shift ? "%" : "5"; break;
				case Keys._6: result = shift ? "^" : "6"; break;
				case Keys._7: result = shift ? "&" : "7"; break;
				case Keys._8: result = shift ? "*" : "8"; break;
				case Keys._9: result = shift ? "(" : "9"; break;
				case Keys.A: result = "a"; break;
				case Keys.B: result = "b"; break;
				case Keys.C: result = "c"; break;
				case Keys.D: result = "d"; break;
				case Keys.E: result = "e"; break;
				case Keys.F: result = "f"; break;
				case Keys.G: result = "g"; break;
				case Keys.H: result = "h"; break;
				case Keys.I: result = "i"; break;
				case Keys.J: result = "j"; break;
				case Keys.K: result = "k"; break;
				case Keys.L: result = "l"; break;
				case Keys.M: result = "m"; break;
				case Keys.N: result = "n"; break;
				case Keys.O: result = "o"; break;
				case Keys.P: result = "p"; break;
				case Keys.Q: result = "q"; break;
				case Keys.R: result = "r"; break;
				case Keys.S: result = "s"; break;
				case Keys.T: result = "t"; break;
				case Keys.U: result = "u"; break;
				case Keys.V: result = "v"; break;
				case Keys.W: result = "w"; break;
				case Keys.X: result = "x"; break;
				case Keys.Y: result = "y"; break;
				case Keys.Z: result = "z"; break;
				case Keys.Num0: result = "0"; break;
				case Keys.Num1: result = "1"; break;
				case Keys.Num2: result = "2"; break;
				case Keys.Num3: result = "3"; break;
				case Keys.Num4: result = "4"; break;
				case Keys.Num5: result = "5"; break;
				case Keys.Num6: result = "6"; break;
				case Keys.Num7: result = "7"; break;
				case Keys.Num8: result = "8"; break;
				case Keys.Num9: result = "9"; break;
				case Keys.NumMultiply: result = "*"; break;
				case Keys.NumAdd: result = "+"; break;
				case Keys.NumSubtract: result = "-"; break;
				case Keys.NumDecimal: result = "."; break;
				case Keys.NumDivide: result = "/"; break;
				case Keys.Semicolon: result = shift ? ":" : ";"; break;
				case Keys.Equals: result = shift ? "+" : "="; break;
				case Keys.Comma: result = shift ? "<" : ","; break;
				case Keys.Minus_Dash: result = shift ? "_" : "-"; break;
				case Keys.Dot: result = shift ? ">" : "."; break;
				case Keys.Slash: result = shift ? "?" : "/"; break;
				case Keys.GraveAccent: result = shift ? "~" : "`"; break;
				case Keys.SquareBracketOpen: result = shift ? "{" : "["; break;
				case Keys.Backslash: result = shift ? "|" : "\\"; break;
				case Keys.SquareBracketClose: result = shift ? "}" : "]"; break;
				case Keys.Quote: result = shift ? "\"" : "'"; break;
				default: result = null; break;
			}
			result = shift && result != null ? result.ToUpper() : result;
			return result;
		}
		public static List<Keys> KeysPressed_Get()
		{
			var result = new List<Keys>();
			var keysPressed = Keyboard.GetState().GetPressedKeys();
			for (int i = 0; i < keysPressed.Length; i++)
			{
				result.Add((Keys)(int)keysPressed[i]);
			}
			return result;
		}
		public static List<Keys> KeysJustPressed_Get()
		{
			return keysJustPressed;
		}
		public static List<Keys> KeysJustReleased_Get()
		{
			return keysJustReleased;
		}
		public static bool KeyIsPressed_Check(Keys key)
		{
			return Keyboard.GetState().IsKeyDown((Microsoft.Xna.Framework.Input.Keys)(int)key);
		}

		public static Vector2 MouseCursorPositionWorld_Get()
		{
			var def = new Point(Game.ResolutionDefaultWidth_Get(), Game.ResolutionDefaultHeight_Get());
			var scale = new Vector2((float)screenSize.X / def.X, (float)screenSize.Y / def.Y);
			var pos = new Vector2(Mouse.GetState().Position.X, Mouse.GetState().Position.Y) * scale;
			return pos;
		}
		public static Vector2 MouseCursorPositionWindow_Get()
		{
			return MouseCursorPositionWorld_Get() + Camera.Position_Get();
		}
		public static void MouseCursorVisibility_Activate(bool visible)
		{
			game.IsMouseVisible = visible;
		}
		public static bool MouseCursorIsVisible_Check()
		{
			return game.IsMouseVisible == false;
		}
		public static bool MouseButtonIsPressedLeft_Check()
		{
			return Mouse.GetState().LeftButton == ButtonState.Pressed;
		}
		public static bool MouseButtonIsPressedMiddle_Check()
		{
			return Mouse.GetState().MiddleButton == ButtonState.Pressed;
		}
		public static bool MouseButtonIsPressedRight_Check()
		{
			return Mouse.GetState().RightButton == ButtonState.Pressed;
		}
		public static void MouseCursorFromSprite_Set(string spritePath, int originX, int originY)
		{
			if (spritePath == null || sprites.ContainsKey(spritePath) == false) return;

			Mouse.SetCursor(MouseCursor.FromTexture2D(sprites[spritePath], originX, originY));
		}

		public static bool PressIntoHold_Check(string name, bool condition, double delayInSeconds = 0.5, bool updateEveryTick = false, double updatesPerSecond = 0.1)
		{
			if (Gate.IsOpened_Check($"{name}-gate", condition))
			{
				Signal.Create(name, delayInSeconds);
				return true;
			}
			if (updateEveryTick)
			{
				return condition;
			}
			else if (Signal.TimerFromSignalIsOccuring_Check(name, updatesPerSecond))
			{
				return condition;
			}
			return false;
		}
	}

	/// <summary>
	/// Controls numbers in different ways.
	/// </summary>
	public static class Number
	{
		public enum RoundType
		{
			Closest, Up, Down
		}
		public static double Unsigned_Get(double number)
		{
			return Math.Abs(number);
		}
		public static double Averaged_Get(double numberA, double numberB)
		{
			return (numberA + numberB) / 2;
		}
		public static double Randomized_Get(double lowerBound, double upperBound, int precision)
		{
			precision = (int)Limited_Get(precision, 0, 5);
			if (lowerBound > upperBound)
			{
				var swap = lowerBound;
				lowerBound = upperBound;
				upperBound = swap;
			}
			var lowerInt = Convert.ToInt32(lowerBound * Math.Pow(10, CountAfterdoublePointGet(lowerBound, precision)));
			var upperInt = Convert.ToInt32(upperBound * Math.Pow(10, CountAfterdoublePointGet(upperBound, precision)));
			var randInt = new Random(Guid.NewGuid().GetHashCode()).Next(lowerInt, upperInt + 1);

			return randInt / Math.Pow(10, precision);
		}
		public static double Rounded_Get(double number, int precision, RoundType numberRoundType)
		{
			precision = (int)Limited_Get(precision, 0, 5);
			var a = Math.Pow(10, CountAfterdoublePointGet(number, precision));
			var b = Math.Pow(10, precision);
			var c = number * a;
			switch (numberRoundType)
			{
				case RoundType.Closest:
					return Convert.ToInt32(c) / b;
				case RoundType.Up:
					return Math.Ceiling(c) / b;
				default:
					return Math.Floor(c) / b;
			}
		}
		public static double Limited_Get(double number, double minimum, double maximum)
		{
			if (minimum > maximum)
			{
				var swap = minimum;
				minimum = maximum;
				maximum = swap;
			}
			if (number < minimum)
			{
				return minimum;
			}
			else if (number > maximum)
			{
				return maximum;
			}
			return number;
		}
		public static double PercentedTowardsTarget_Get(double number, double targetNumber, double percent)
		{
			var vec = new Vector2((float)number, 0);
			var targetVec = new Vector2((float)targetNumber, 0);
			var result = Vector2.Lerp(vec, targetVec, (float)percent / 100);

			return result.X;
		}
		public static double Changed_Get(double number, double numbersPerSecond)
		{
			return number + (numbersPerSecond * deltaTime);
		}
		public static double ChangedTowardsTarget_Get(double number, double targetNumber, double numbersPerSecond)
		{
			if (number <= targetNumber && targetNumber * deltaTime < 0)
			{
				return targetNumber;
			}
			else if (number >= targetNumber && targetNumber * deltaTime > 0)
			{
				return targetNumber;
			}
			return Changed_Get(number, numbersPerSecond);
		}
		public static bool ChancePercent_Check(double percent)
        {
			percent = Limited_Get(percent, 0, 100);
			var n = Randomized_Get(1, 100, 0);
			return n <= percent;
        }
	}
	/// <summary>
	/// Controls texts in different ways.
	/// </summary>
	public static class Text
	{
		public static string FromList_Get<T>(List<T> list, string separator = ", ")
		{
			var result = "";
			if (list == null || list.Count == 0)
			{
				return result;
			}
			for (int i = 0; i < list.Count; i++)
			{
				result = result.Insert(result.Length, $"{list[i]}");
				if (i == list.Count - 1)
				{
					break;
				}
				result = result.Insert(result.Length, $"{separator}");
			}
			return result;
		}
	}

	/// <summary>
	/// Controls positions in different ways.
	/// </summary>
	public static class Position
	{
		public static double DistanceToPosition_Get(Vector2 position, Vector2 targetPposition)
		{
			return Vector2.Distance(position, targetPposition);
		}
		public static double DistanceToPosition_Get(double x, double y, double targetX, double targetY)
		{
			return DistanceToPosition_Get(new Vector2((float)x, (float)y), new Vector2((float)targetX, (float)targetY));
		}
		public static Vector2 MovedInDirection_Get(Vector2 position, Vector2 direction, double pixelsPerSecond)
		{
			pixelsPerSecond *= deltaTime;
			if (direction != Vector2.Zero)
			{
				direction.Normalize();
			}
			position += direction * (float)pixelsPerSecond;
			return position;
		}
		public static Vector2 MovedInDirection_Get(double x, double y, Vector2 direction, double pixelsPerSecond)
		{
			return MovedInDirection_Get(new Vector2((float)x, (float)y), direction, pixelsPerSecond);
		}
		public static Vector2 MovedInDirection_Get(Vector2 position, Direction.Directions direction, double pixelsPerSecond)
		{
			return MovedInDirection_Get(position, directions[direction], pixelsPerSecond);
		}
		public static Vector2 MovedInDirection_Get(double x, double y, Direction.Directions direction, double pixelsPerSecond)
		{
			return MovedInDirection_Get(new Vector2((float)x, (float)y), directions[direction], pixelsPerSecond);
		}
		public static Vector2 MovedAtAngle_Get(Vector2 position, double angle, double pixelsPerSecond)
		{
			return MovedInDirection_Get(position, Direction.FromAngle_Get(angle), pixelsPerSecond);
		}
		public static Vector2 MovedAtAngle_Get(double x, double y, double angle, double pixelsPerSecond)
		{
			return MovedAtAngle_Get(new Vector2((float)x, (float)y), angle, pixelsPerSecond);
		}
		public static Vector2 MovedTowardsPosition_Get(Vector2 position, Vector2 targetPosition, double pixelsPerSecond)
		{
			var direction = targetPosition - position;
			return MovedInDirection_Get(position, direction, pixelsPerSecond);
		}
		public static Vector2 MovedTowardsPosition_Get(double x, double y, double targetX, double targetY, double pixelsPerSecond)
		{
			return MovedTowardsPosition_Get(new Vector2((float)x, (float)y), new Vector2((float)targetX, (float)targetY), pixelsPerSecond);
		}
		public static Vector2 PercentedTowardsPosition_Get(Vector2 position, Vector2 targetPosition, double percent)
		{
			return Vector2.Lerp(position, targetPosition, (float)percent / 100);
		}
		public static Vector2 PercentedTowardsPosition_Get(double x, double y, double targetX, double targetY, double percent)
		{
			return PercentedTowardsPosition_Get(new Vector2((float)x, (float)y), new Vector2((float)targetX, (float)targetY), percent);
		}
	}
	/// <summary>
	/// Controls angles in different ways.
	/// </summary>
	public static class Angle
	{
		public static double DegreesFromNumber_Get(double angle)
		{
			return ((angle % 360) + 360) % 360;
		}
		public static double RotatedTowardsTarget_Get(double angle, double targetAngle, double degreesPerSecond)
		{
			angle = DegreesFromNumber_Get(angle);
			var newAngle = DegreesFromNumber_Get(targetAngle);
			var difference = angle - newAngle;

			// stops the rotation with an else when close enough
			if (Math.Abs(difference) < degreesPerSecond * deltaTime)
			{
				// prevents the rotation from staying behind after the stop
				angle = newAngle;
			}
			else if (difference > 0 && difference < 180)
			{
				angle = Rotated_Get(angle, -degreesPerSecond);
			}
			else if (difference > -180 && difference < 0)
			{
				angle = Rotated_Get(angle, degreesPerSecond);
			}
			else if (difference > -360 && difference < -180)
			{
				angle = Rotated_Get(angle, -degreesPerSecond);
			}
			else if (difference > 180 && difference < 360)
			{
				angle = Rotated_Get(angle, degreesPerSecond);
			}

			// detects speed greater than possible
			if (Math.Abs(difference) > 360 - degreesPerSecond * deltaTime)
			{
				// prevents jiggle when passing 0-360 & 360-0 | simple to fix yet took me half a day
				angle = newAngle;
			}
			return angle;
		}
		public static double PercentedTowardsTarget_Get(double angle, double targetAngle, double percent)
		{
			angle = DegreesFromNumber_Get(angle);
			targetAngle = DegreesFromNumber_Get(targetAngle);
			var result = Number.PercentedTowardsTarget_Get(angle, targetAngle, percent);

			return DegreesFromNumber_Get(result);
		}
		public static double Rotated_Get(double angle, double degreesPerSecond)
		{
			return DegreesFromNumber_Get(Number.Changed_Get(angle, degreesPerSecond));
		}
		public static double BetweenPositions_Get(Vector2 position, Vector2 targetPosition)
		{
			return FromDirection_Get(Direction.BetweenPositions_Get(position, targetPosition));
		}
		public static double BetweenPositions_Get(double x, double y, double targetX, double targetY)
		{
			return BetweenPositions_Get(new Vector2((float)x, (float)y), new Vector2((float)targetX, (float)targetY));
		}
		public static double FromDirection_Get(Vector2 direction)
		{
			//Vector2 to Radians: atan2(Vector2.y, Vector2.x)
			//Radians to Angle: radians * (180 / Math.PI)
			if (direction != Vector2.Zero)
			{
				direction.Normalize();
			}
			var rad = (double)Math.Atan2(direction.Y, direction.X);
			var ang = rad * (180 / (double)Math.PI);
			return ang;
		}
		public static double FromDirection_Get(Direction.Directions direction)
		{
			return FromDirection_Get(directions[direction]);
		}
	}
	/// <summary>
	/// Controls directions in different ways.
	/// </summary>
	public static class Direction
	{
		public enum Directions
		{
			Up, Down, Left, Right, Up_Left, Up_Right, Down_Left, Down_Right
		}

		public static Vector2 FromAngle_Get(double angle)
		{
			//Angle to Radians : (Math.PI / 180) * angle
			//Radians to Vector2 : Vector2.x = cos(angle) | Vector2.y = sin(angle)

			var rad = Math.PI / 180 * angle;
			var dir = new Vector2((float)Math.Cos(rad), (float)Math.Sin(rad));
			if (dir != Vector2.Zero)
			{
				dir.Normalize();
			}
			return dir;
		}
		public static Vector2 RotatedTowardsTarget_Get(Vector2 direction, Vector2 targetDirection, double directionsPerSecond)
		{
			if (direction != Vector2.Zero)
			{
				direction.Normalize();
			}
			if (targetDirection != Vector2.Zero)
			{
				targetDirection.Normalize();
			}
			var angle = Angle.RotatedTowardsTarget_Get(Angle.FromDirection_Get(direction), Angle.FromDirection_Get(targetDirection), directionsPerSecond);
			return FromAngle_Get(angle);
		}
		public static Vector2 RotatedTowardsTarget_Get(Vector2 direction, Directions targetDirection, double directionsPerSecond)
		{
			return RotatedTowardsTarget_Get(direction, directions[targetDirection], directionsPerSecond);
		}
		public static Vector2 BetweenPositions_Get(Vector2 position, Vector2 targetPosition)
		{
			var dir = targetPosition - position;
			if (dir != Vector2.Zero)
			{
				dir.Normalize();
			}
			return dir;
		}
		public static Vector2 BetweenPositions_Get(double x, double y, double targetX, double targetY)
		{
			return BetweenPositions_Get(new Vector2((float)x, (float)y), new Vector2((float)targetX, (float)targetY));
		}
	}
	/// <summary>
	/// Controls sizes in different ways.
	/// </summary>
	public static class Size
	{
		public static Vector2 ChangedTowardsTarget_Get(Vector2 size, Vector2 targetSize, double pixelsPerSecond)
		{
			return Position.PercentedTowardsPosition_Get(size, targetSize, pixelsPerSecond);
		}
		public static Vector2 ChangedTowardsTarget_Get(double width, double height, double targetWidth, double targetHeight, double pixelsPerSecond)
		{
			return ChangedTowardsTarget_Get(new Vector2((float)width, (float)height), new Vector2((float)targetWidth, (float)targetHeight), pixelsPerSecond);
		}
		public static Vector2 Changed_Get(Vector2 size, double pixelsPerSecond)
		{
			pixelsPerSecond *= deltaTime;
			size += new Vector2(size.X + (float)pixelsPerSecond, size.Y + (float)pixelsPerSecond);
			return size;
		}
		public static Vector2 Changed_Get(double width, double height, double pixelsPerSecond)
		{
			return Changed_Get(new Vector2((float)width, (float)height), pixelsPerSecond);
		}
	}
	/// <summary>
	/// Controls scales in different ways.
	/// </summary>
	public static class Scale
	{
		public static Vector2 ChangedTowardsTarget_Get(Vector2 scale, Vector2 targetScale, double scalesPerSecond)
		{
			return Size.ChangedTowardsTarget_Get(scale, targetScale, scalesPerSecond);
		}
		public static Vector2 ChangedTowardsTarget_Get(double scaleWidth, double scaleHeight, double targetScaleWidth, double targetScaleHeight, double scalesPerSecond)
		{
			return ChangedTowardsTarget_Get(new Vector2((float)scaleWidth, (float)scaleHeight), new Vector2((float)targetScaleWidth, (float)targetScaleHeight), scalesPerSecond);
		}
		public static Vector2 Changed_Get(Vector2 scale, double scalesPerSecond)
		{
			return Size.Changed_Get(scale, scalesPerSecond);
		}
		public static Vector2 Changed_Get(double scaleWidth, double scaleHeight, double scalesPerSecond)
		{
			return Changed_Get(new Vector2((float)scaleWidth, (float)scaleHeight), scalesPerSecond);
		}
	}

	/// <summary>
	/// Controls Gates and holds information about them. <br></br><br></br>
	/// Gates' purpose is to convert continuous code flow into a single trigger.<br></br>
	/// They can be accessed through their names.<br></br>
	/// A Gate's state is either true or false (opened/closed).<br></br>
	/// Once the code is inside, the Gate closes until the code is manually KickedOut or the Gate is opened through its condition turning false.
	/// </summary>
	public static class Gate
	{
		public static int EntriesCount_Get(string name)
		{
			return gateEntriesCount.ContainsKey(name) ? gateEntriesCount[name] : 0;
		}
		public static void Entries_Remove(string name)
		{
			if (gateEntriesCount.ContainsKey(name) == false)
			{
				return;
			}
			gateEntriesCount[name] = default;
		}
		public static void KickOut_Remove(string name)
		{
			if (gates.ContainsKey(name) == false)
			{
				return;
			}
			gates.Remove(name);
		}
		public static bool IsOpened_Check(string name, bool condition, int entriesLimit = int.MaxValue)
		{
			if (gates.ContainsKey(name) == false && condition == false)
			{
				return false;
			}
			else if (gates.ContainsKey(name) == false && condition == true)
			{
				gates[name] = true;
				gateEntriesCount[name] = 1;
				return true;
			}
			else
			{
				if (gates[name] == true && condition == true)
				{
					return false;
				}
				else if (gates[name] == false && condition == true)
				{
					gates[name] = true;
					gateEntriesCount[name]++;
					return true;
				}
				else if (gateEntriesCount[name] < entriesLimit)
				{
					gates[name] = false;
				}
			}
			return false;
		}
	}
	/// <summary>
	/// Controls Signals and holds information about them.<br></br><br></br>
	/// Signals' purpose is to convert continuous code flow into a single trigger.<br></br>
	/// They can be accessed through their names.<br></br>
	/// The trigger of each Signal happens after a certain period of it being created.
	/// </summary>
	public static class Signal
	{
		public static void Create(string name, double delayInSeconds)
		{
			if (delayInSeconds < 0)
			{
				return;
			}
			if (signalTimers.ContainsKey(name) == false)
			{
				signalTimers.Add(name, 0);
			}
			if (signalPauses.ContainsKey(name) == false)
			{
				signalPauses.Add(name, false);
			}
			if (signalStartTimes.ContainsKey(name) == false)
			{
				signalStartTimes.Add(name, Game.Runtime_Get());
			}
			if (signalDelays.ContainsKey(name) == false)
			{
				signalDelays.Add(name, delayInSeconds);
			}
			signalTimers[name] = 0;
			signalPauses[name] = false;
			signalStartTimes[name] = Game.Runtime_Get();
			signalDelays[name] = delayInSeconds;
		}
		public static bool Exists_Check(string name)
		{
			return name != null && signalTimers.ContainsKey(name);
		}
		public static double DelayInSeconds_Get(string name)
		{
			return signalDelays.ContainsKey(name) != false ? signalDelays[name] : 0;
		}
		public static void Delay_Pause(string name, bool paused)
		{
			if (signalPauses.ContainsKey(name) == false)
			{
				return;
			}
			signalPauses[name] = paused;
		}
		public static double SecondsUntilOccurance_Get(string name)
		{
			return TimeOccur_Get(name) - Game.Runtime_Get();
		}
		public static double TimeStart_Get(string name)
		{
			return signalStartTimes.ContainsKey(name) == false ? 0 : signalStartTimes[name];
		}
		public static double TimeOccur_Get(string name)
		{
			return signalStartTimes.ContainsKey(name) == false || signalDelays.ContainsKey(name) == false ? 0 : signalStartTimes[name] + signalDelays[name];
		}
		public static bool IsOccuring_Check(string name, bool delete)
		{
			if (signalTimers.ContainsKey(name) == false || signalDelays.ContainsKey(name) == false)
			{
				return false;
			}
			if (signalPauses.ContainsKey(name) == true && signalPauses[name])
			{
				return false;
			}
			if (signalTimers[name] >= signalDelays[name])
			{
				if (delete)
				{
					Delete(name);
				}
				return true;
			}
			return false;
		}
		public static void Delete(string name)
		{
			if (signalTimers.ContainsKey(name))
			{
				signalTimers.Remove(name);
			}
			if (signalPauses.ContainsKey(name))
			{
				signalPauses.Remove(name);
			}
			if (signalStartTimes.ContainsKey(name))
			{
				signalStartTimes.Remove(name);
			}
			if (signalPauses.ContainsKey(name))
			{
				signalPauses.Remove(name);
			}
			if (signalDelays.ContainsKey(name))
			{
				signalDelays.Remove(name);
			}
		}

		public static void TimerRestart_Start(string name)
		{
			Gate.Entries_Remove(name);
		}
		public static bool TimerFromSignalIsOccuring_Check(string name, double intervalsInSeconds, int repeats = int.MaxValue)
		{
			if (Gate.IsOpened_Check(name, IsOccuring_Check(name, false), repeats))
			{
				Create(name, intervalsInSeconds);
				return true;
			}
			return false;
		}
		public static int TimerRepeatCount_Get(string name)
		{
			return Gate.EntriesCount_Get(name);
		}
		public static double Timer_Get(string name)
		{
			return TimerRepeatCount_Get(name) * DelayInSeconds_Get(name);
		}
	}

	public static class Font
	{
		public static List<string> Names_All_Get()
		{
			return DictionaryKeysGet(fonts);
		}

		public static void SpacingCharacter_Set(string fontName, double spacing)
		{
			if (fontName == null || fonts.ContainsKey(fontName) == false) return;

			fonts[fontName].Spacing = (float)spacing;
		}
		public static double SpacingCharacter_Get(string fontName)
		{
			return fontName != null && fonts.ContainsKey(fontName) ? fonts[fontName].Spacing : default;
		}
		public static void SpacingLine_Set(string fontName, int spacing)
		{
			if (fontName == null || fonts.ContainsKey(fontName) == false) return;

			fonts[fontName].LineSpacing = spacing;
		}
		public static int SpacingLine_Get(string fontName)
		{
			return fontName != null && fonts.ContainsKey(fontName) ? fonts[fontName].LineSpacing : default;
		}
	}
	/// <summary>
	/// Controls Playlists and holds information about them.
	/// </summary>
	public static class Playlist
	{
		public static List<string> Names_All_Get()
		{
			return DictionaryKeysGet(playlists);
		}
		public static string NameCurrent_Get()
		{
			return playlistName;
		}

		public static void Create(string name)
		{
			if (playlists.ContainsKey(name))
			{
				return;
			}
			playlists.Add(name, new List<string>());
		}
		public static void Melody_Add(string melodyName, string playlistName)
		{
			if (playlists.ContainsKey(playlistName) == false)
			{
				return;
			}
			playlists[playlistName].Add(melodyName);
		}
		public static void Start(string name, int startIndex = 0, double volumePercent = 50, bool loop = false, double secondsBetweenMelodies = 0)
		{
			if (playlists.ContainsKey(playlistName) == false)
			{
				return;
			}
			startIndex = (int)Number.Limited_Get(startIndex, 0, playlists[playlistName].Count - 1);
			Melody.Current_Stop();
			secondsBetweenMelodies += 0.5f;
			playlistIndex = startIndex;
			playlistName = name;
			playlistLoop = loop;
			playlistDelay = secondsBetweenMelodies;
			inPlaylist = true;
			Melody.Play(playlists[playlistName][startIndex], volumePercent, false);
			inPlaylist = false;
			Signal.Create(melodyOverKey, Do.Melody.DurationInSeconds_Get(playlists[playlistName][playlistIndex]) + playlistDelay);
		}
		public static int CurrentMelodyIndex_Get()
		{
			return playlistIndex;
		}
		public static bool CurrentIsLooping_Check()
		{
			return playlistLoop;
		}
		public static void Current_Stop()
		{
			playlistName = default;
			playlistLoop = default;
			playlistIndex = default;
			playlistDelay = default;
		}
	}
	/// <summary>
	/// Controls Melodies and holds information about them.
	/// </summary>
	public static class Melody
	{
		public static List<string> Names_All_Get()
		{
			return DictionaryKeysGet(melodies);
		}

		public static void Play(string name, double volumePercent = 50, bool loop = false)
		{
			var playlistName = Do.Playlist.NameCurrent_Get();
			var delay = 0.0;
			if (melodies.ContainsKey(name) == false)
			{
				return;
			}
			if (playlistName == default)
			{
				Melody.Current_Stop();
			}
			else if (inPlaylist == false && playlistName != default)
			{
				Playlist.Current_Stop();
			}
			else if (playlistName != default)
			{
				delay = playlistDelay;
			}
			volumePercent = Number.Limited_Get(volumePercent, 0, 100);
			melodyPlayingName = name;
			melodyPlayingLoop = loop;
			melodyPlayingVolume = volumePercent;
			MediaPlayer.Volume = (float)volumePercent / 100;
			MediaPlayer.Play(melodies[name]);
			Signal.Create(melodyOverKey, Do.Melody.DurationInSeconds_Get(name) + delay);
		}
		public static bool IsInPlaylist_Check(string name)
		{
			foreach (var kvp in playlists)
			{
				if (kvp.Key.Contains(name))
				{
					return true;
				}
			}
			return false;
		}
		public static double DurationInSeconds_Get(string name)
		{
			return name == null || melodies.ContainsKey(name) == false ? default : SongDurationInSec(melodies[name]);
		}
		public static void Current_Stop()
		{
			melodyPlayingName = default;
			if (Playlist.NameCurrent_Get() != default)
			{
				Playlist.Current_Stop();
			}
			MediaPlayer.Stop();
		}
		public static bool CurrentIsLooping_Check()
		{
			return melodyPlayingLoop;
		}
		public static string CurrentName_Get()
		{
			return melodyPlayingName;
		}
		public static double CurrentVolume_Get()
		{
			return melodyPlayingVolume;
		}
		public static double CurrentProgressInSeconds_Get()
		{
			return DurationInSeconds_Get(melodyPlayingName) - Signal.SecondsUntilOccurance_Get(melodyOverKey) + GetDelay();
		}
		public static double CurrentProgressInPercent_Get()
		{
			var dur = DurationInSeconds_Get(melodyPlayingName);
			return dur == 0 ? 0 : (dur - Signal.SecondsUntilOccurance_Get(melodyOverKey) + GetDelay()) / dur * 100;
		}
		public static void Current_Pause(bool paused)
		{
			Signal.Delay_Pause(melodyOverKey, paused);
			if (paused)
			{
				MediaPlayer.Pause();
				return;
			}
			MediaPlayer.Resume();
		}
	}
	/// <summary>
	/// Controls Sounds and holds information about them.
	/// </summary>
	public static class Sound
	{
		public static List<string> Names_All_Get()
		{
			return DictionaryKeysGet(sounds);
		}

		public static void Play(string name, double volumePercent = 50, double pitchPercent = 50, double speakerPercent = 50, bool loop = false, bool ableToOverlapSelf = false)
		{
			if (sounds.ContainsKey(name) == false)
			{
				return;
			}
			volumePercent = Number.Limited_Get(volumePercent, 0, 100);
			pitchPercent = Number.Limited_Get(pitchPercent, 0, 100);
			speakerPercent = Number.Limited_Get(speakerPercent, 0, 100);
			if (ableToOverlapSelf)
			{
				sounds[name] = soundsRaw[name].CreateInstance();
			}
			sounds[name].Pan = ((float)speakerPercent * 2 - 100) / 100;
			sounds[name].IsLooped = loop;
			sounds[name].Pitch = ((float)pitchPercent * 2 - 100) / 100;
			sounds[name].Volume = (float)volumePercent / 100;
			sounds[name].Play();
		}
		public static void Current_All_Pause(bool paused)
		{
			foreach (var kvp in sounds)
			{
				if (paused)
				{
					kvp.Value.Pause();
					continue;
				}
				kvp.Value.Resume();
			}
		}
		public static void Current_Pause(string name, bool paused)
		{
			if (sounds.ContainsKey(name) == false)
			{
				return;
			}
			if (paused)
			{
				sounds[name].Pause();
				return;
			}
			sounds[name].Resume();
		}
		public static void Current_All_Stop()
		{
			foreach (var kvp in sounds)
			{
				kvp.Value.Stop();
			}
		}
		public static void Current_Stop(string name)
		{
			if (sounds.ContainsKey(name) == false)
			{
				return;
			}
			sounds[name].Stop();
		}
	}
	/// <summary>
	/// Controls Sprites and holds information about them.
	/// </summary>
	public static class SpriteGrid
	{
		public static void Create(string name)
		{
			if (spriteGrids.ContainsKey(name))
			{
				return;
			}
			spriteGrids[name] = new List<List<string>>();
		}
		public static void ThingSprite_Add(string spriteGridName, string uniqueName, int row)
		{
			if (spriteGrids.ContainsKey(spriteGridName) == false || Thing.Exists_Check(uniqueName) == false ||
				spriteGrids[spriteGridName].Count < row)
			{
				return;
			}
			if (spriteGrids[spriteGridName].Count == row)
			{
				spriteGrids[spriteGridName].Add(new List<string>());
			}
			spriteGrids[spriteGridName][row].Add(uniqueName);
		}
		public static void ThingSprites_Set(string spriteGridName, List<List<string>> sprites)
		{
			if (spriteGrids.ContainsKey(spriteGridName) == false)
			{
				return;
			}
			for (int i = 0; i < spriteGrids[spriteGridName].Count; i++)
			{
				for (int j = 0; j < spriteGrids[spriteGridName][i].Count; j++)
				{
					if (sprites.Count <= i || sprites[i].Count <= j)
					{
						continue;
					}
					Thing.Sprite_Set(spriteGrids[spriteGridName][i][j], sprites[i][j]);
				}
			}
		}
		public static void ThingSpriteRegion_Set(string spriteGridName, int index, int row, int width, int height, double angle, Color color, bool visible, string spritePath)
		{
			if (row < 0 || index < 0 || width < 1 || height < 1 || spritePath == null || spriteGrids.ContainsKey(spriteGridName) == false)
			{
				return;
			}
			for (int i = 0; i < height; i++)
			{
				for (int j = 0; j < width; j++)
				{
					if (row + i >= spriteGrids[spriteGridName].Count || index + j >= spriteGrids[spriteGridName][row + i].Count || spriteGrids[spriteGridName][row + i][index + j] == null)
					{
						continue;
					}
					var uniqueName = spriteGrids[spriteGridName][row + i][index + j];
					Thing.Angle_Set(uniqueName, angle);
					Thing.SpriteColor_Set(uniqueName, color);
					Thing.Sprite_Set(uniqueName, spritePath);
					Thing.SpriteVisibility_Activate(uniqueName, visible);
				}
			}
		}
		public static string ThingSprite_Get(string spriteGridName, int row, int index)
		{
			return Thing.Sprite_Get(ThingUniqueName_Get(spriteGridName, row, index));
		}
		public static string ThingUniqueName_Get(string spriteGridName, int index, int row)
		{
			if (row < 0 || index < 0 || spriteGrids.ContainsKey(spriteGridName) == false || row >= spriteGrids[spriteGridName].Count ||
				index >= spriteGrids[spriteGridName][row].Count || spriteGrids[spriteGridName][row][index] == null)
			{
				return default;
			}
			return spriteGrids[spriteGridName][row][index];
		}
		public static Point ThingPosition_Get(string spriteGridName, string uniqueName)
		{
			if (spriteGrids.ContainsKey(spriteGridName) == false || uniqueName == null)
			{
				return default;
			}
			for (int i = 0; i < spriteGrids[spriteGridName].Count; i++)
			{
				for (int j = 0; j < spriteGrids[spriteGridName][i].Count; j++)
				{
					if (spriteGrids[spriteGridName][i][j] == uniqueName)
					{
						return new Point(j, i);
					}
				}
			}
			return default;
		}
		public static Point ThingGridPosition_Get(string spriteGridName, string uniqueName)
		{
			if (spriteGrids.ContainsKey(spriteGridName) == false || uniqueName == null)
			{
				return default;
			}
			for (int i = 0; i < spriteGrids[spriteGridName].Count; i++)
			{
				if (spriteGrids[spriteGridName][i].Contains(uniqueName))
				{
					return new Point(spriteGrids[spriteGridName][i].IndexOf(uniqueName), i);
				}
			}
			return default;
		}
	}
	/// <summary>
	/// Holds information about Sprites.
	/// </summary>
	public static class Sprite
	{
		public static List<string> Names_All_Get()
		{
			return DictionaryKeysGet(sprites);
		}
		public static List<string> Names_All_Get(string directory)
		{
			var result = new List<string>();
			var sprites = Names_All_Get();

			foreach (var sprite in sprites)
			{
				var path = sprite.Split('/').ToList();
				path.RemoveAt(path.Count - 1);
				var splitDirectory = directory.Split('/').ToList();
				for (int i = 0; i < path.Count; i++)
				{
					if (path[i] == splitDirectory[i])
					{
						result.Add(sprite);
					}
				}
			}
			return result;
		}

		public static double SpriteWidth_Get(string spritePath)
		{
			return spritePath != null && sprites.ContainsKey(spritePath) ? sprites[spritePath].Width : default;
		}
		public static double SpriteHeight_Get(string spritePath)
		{
			return spritePath != null && sprites.ContainsKey(spritePath) ? sprites[spritePath].Height : default;
		}
	}

	private static string ToStr(string str)
	{
		return str == null ? "None" : $"\"{str}\"";
	}
	private static void AddUpdatePoint(string uniqueName)
	{
		if (thingsUpdatePoints.Contains(uniqueName))
		{
			return;
		}
		thingsUpdatePoints.Add(uniqueName);
	}
	private static void AddComponent(string uniqueName, Thing.Components component)
	{
		if (thingComponents.ContainsKey(uniqueName) == false)
		{
			thingComponents.Add(uniqueName, new List<Thing.Components>());
		}
		if (componentThings.ContainsKey(component) == false)
		{
			componentThings.Add(component, new List<string>());
		}
		if (thingComponents[uniqueName].Contains(component))
		{
			return;
		}
		thingComponents[uniqueName].Add(component);
		componentThings[component].Add(uniqueName);
	}
	private static void RemoveComponent(string uniqueName, Thing.Components component)
	{
		if (thingComponents.ContainsKey(uniqueName) == false || thingComponents[uniqueName].Contains(component) == false)
		{
			return;
		}
		thingComponents[uniqueName].Remove(component);
		componentThings[component].Remove(uniqueName);
	}
	private static void CheckRelativityExistence(string uniqueName)
	{
		if (thingParentRelatives.ContainsKey(uniqueName) == false)
		{
			thingParentRelatives[uniqueName] = new List<bool>();
			for (int i = 0; i < 3; i++)
			{
				thingParentRelatives[uniqueName].Add(true);
			}
		}
	}
	private static void SetDefaultText(string uniqueName)
	{
		if (thingTextColors.ContainsKey(uniqueName) == false)
		{
			Thing.TextColor_1_Set(uniqueName, 1, 1, 1);
		}
		if (thingTextsVisible.ContainsKey(uniqueName) == false)
		{
			Thing.TextVisibility_Activate(uniqueName, true);
		}
		if (thingTextAnchors.ContainsKey(uniqueName) == false)
		{
			Thing.TextAnchor_Set(uniqueName, Thing.TextAnchor.TopLeft);
		}
		if (thingTextLineHeights.ContainsKey(uniqueName) == false)
		{
			Thing.TextLineHeight_Set(uniqueName, 16);
		}
	}
	private static void SetDefaultThing(string uniqueName)
	{
		if (thingDepths.ContainsKey(uniqueName) == false)
		{
			Thing.Depth_Set(uniqueName, 0);
		}
		if (thingScales.ContainsKey(uniqueName) == false)
		{
			Thing.Scale_Set(uniqueName, 1, 1);
		}
		Thing.Scale_Set(uniqueName, Thing.Scale_Get(uniqueName)); // update the scaling according to the sprite
	}
	private static Vector2 GetSpriteSize(string uniqueName)
	{
		var sprName = Thing.Sprite_Get(uniqueName);
		return new Vector2(sprName == null ? 100 : sprites[sprName].Width, sprName == null ? 100 : sprites[sprName].Height);
	}
	private static Vector2 GetAnchorOffset(string font, string text, Thing.TextAnchor anchor)
	{
		var length = fonts[font].MeasureString(text);

		switch (anchor)
		{
			case Thing.TextAnchor.TopCenter: return new Vector2(length.X / 2, 0);
			case Thing.TextAnchor.TopRight: return new Vector2(length.X, 0);
			case Thing.TextAnchor.CenterLeft: return new Vector2(0, length.Y / 2);
			case Thing.TextAnchor.Center: return length / 2;
			case Thing.TextAnchor.CenterRight: return new Vector2(length.X, length.Y / 2);
			case Thing.TextAnchor.BottomLeft: return new Vector2(0, length.Y);
			case Thing.TextAnchor.BottomCenter: return new Vector2(length.X / 2, length.Y);
			case Thing.TextAnchor.BottomRight: return length;
		}
		return Vector2.Zero;
	}
	private static double GetDelay()
	{
		return playlistName != default ? playlistDelay : default;
	}
	private static double SongDurationInSec(Song song)
	{
		var value = song.Duration.Hours * 3600 + song.Duration.Minutes * 60 + song.Duration.Seconds + song.Duration.Milliseconds / (double)1000;
		return value;
	}
	private static int CountAfterdoublePointGet(double number, int precision)
	{
		precision = (int)Number.Limited_Get(precision, 0, 5);
		var formatting = new List<string>()
			{
				"0", $"{number:F1}", $"{number:F2}", $"{number:F3}", $"{number:F4}", $"{number:F5}"
			};
		var numberStr = formatting[precision];
		var count = 0;
		var counting = false;

		for (int i = 0; i < numberStr.Length; i++)
		{
			if (counting)
			{
				count++;
			}
			if (numberStr[i] == '.')
			{
				counting = true;
			}
		}
		return count;
	}
	private static List<T> DictionaryKeysGet<T, T1>(Dictionary<T, T1> dict)
	{
		var result = new List<T>();
		foreach (var kvp in dict)
		{
			result.Add(kvp.Key);
		}
		return result;
	}
	private static string ValidateString(string message)
	{
		var notAscii = message.Any(s => s > 255);

		if (notAscii)
		{
			return null;
		}
		if (message.Contains("∞"))
		{
			message = message.Replace("∞", "Infinity");
		}
		if (message.Contains("\t"))
		{
			message = message.Replace("\t", "    ");
		}
		return message;
	}
	private static void SetWindowPositionAccordingToResolution()
	{
		switch (windowType)
		{
			case Game.WindowTypes.WindowedFullscreen:
				Set(false, true, 0, 0);
				break;
			case Game.WindowTypes.Windowed_TitleBar:
				Set(false, false, -8, 0);
				break;
			case Game.WindowTypes.Windowed_TaskBar:
				Set(false, false, -8, -31);
				break;
			case Game.WindowTypes.Windowed_TitleBar_TaskBar:
				Set(false, false, -8, 1);
				break;
			case Game.WindowTypes.Fullscreen:
				Set(true, true, 0, 0);
				break;
		}
		void Set(bool sw, bool fullscr, int x, int y)
		{
			graphics.HardwareModeSwitch = sw;
			graphics.IsFullScreen = fullscr;
			game.Window.Position = new Point(x, y);
		}
	}
	private static string GetLeftSelected()
	{
		var newLeftSelected = debugLeftSelected;
		if (newLeftSelected != null)
		{
			newLeftSelected = newLeftSelected.Replace(sv, "");
			newLeftSelected = newLeftSelected.Replace(sl, "");
			newLeftSelected = newLeftSelected.Replace(sh, "").Trim();
		}
		return newLeftSelected;
	}
	private static string GetRightSelected()
	{
		var newRightSelected = debugRightSelected;
		if (newRightSelected != null)
		{
			newRightSelected = newRightSelected.Replace(sv, "");
			newRightSelected = newRightSelected.Replace(sl, "");
			newRightSelected = newRightSelected.Replace(sh, "").Trim();
		}
		return newRightSelected;
	}
	#endregion
}