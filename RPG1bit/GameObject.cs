using SMPL.Components;
using SMPL.Data;
using SMPL.Gear;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace RPG1bit
{
	public class GameObject : Thing
	{
		public struct CreationDetails
		{
			public string Name { get; set; }
			public Point[] TileIndexes { get; set; }
			public Point Position { get; set; }
			public int Height { get; set; }
			public bool IsDragable { get; set; }
			public bool IsLeftClickable { get; set; }
			public bool IsRightClickable { get; set; }
			public bool IsConfirmingClick { get; set; }
			public bool IsUI { get; set; }
			public bool IsInTab { get; set; }
			public bool IsKeptBetweenSessions { get; set; }
			public string AppearOnTab { get; set; }
		}

		public static readonly Dictionary<Point, List<GameObject>> objects = new();
		public static readonly Dictionary<Point, string> descriptions = new()
		{
			{ new(33, 15), "Graphics 1-Bit Pack by kenney.nl\n" +
				"Font DPComic by cody@zone38.net\n" +
				"Music by opengameart.org/users/yubatake\n" +
				"Music by opengameart.org/users/avgvsta\n" +
				$"Game {NavigationPanel.Info.GameVersion} & SFX(software: Bfxr) by dodo" },
			{ new(00, 00), "" }, // void
			{ new(01, 22), "" }, // background color
			{ new(04, 22), "" }, { new(00, 23), "" }, { new(29, 15), "" }, { new(30, 15), "" }, { new(02, 22), "" },
			{ new(03, 22), "" },	// nav panel

			{ new(13, 22), "[LEFT CLICK] to start typing..." },

			{ new(44, 16), "Start a new multiplayer game session\n  (not available in this game version)" },

			{ new(41, 13), "\t\tAbove the world...\n\t[LEFT DRAG] to paint a tile\n+[LEFT SHIFT] to paint a square\n" +
				"   +[LEFT ALT] to paint a roof" },
			{ new(42, 13), "\t\tAbove the world...\n\t[RIGHT DRAG] to erase a tile\n+[LEFT SHIFT] to erase a square" },
			{ new(43, 13), "[MIDDLE CLICK] above the world\n to pick a brush at that height" },
			{ new(06, 23), "Roofs visibility." },

			{ new(01, 07), "Sign" }, { new(0, 7),	"Sign" }, { new(2, 7), "Sign" }, { new(1, 8), "Sign" }, { new(0, 8), "Sign" },

			{ new(03, 03), "Closed fence gate" }, { new(04, 03), "Opened fence gate" },
			{ new(00, 04), "Closed fence gate" }, { new(01, 04), "Opened fence gate" },
			{ new(03, 04), "Closed fence gate" }, { new(04, 04), "Opened fence gate" },
			{ new(05, 04), "Closed fence gate" }, { new(06, 04), "Opened fence gate" },

			{ new(38, 22), "Closed door" }, { new(39, 22), "Opened door" }, { new(40, 22), "Closed door" },
			{ new(41, 22), "Opened door" }, { new(42, 22), "Closed door" }, { new(43, 22), "Opened door" },
			{ new(44, 22), "Closed door" }, { new(45, 22), "Opened door" }, { new(46, 22), "Closed door" },
			{ new(47, 22), "Opened door" },

			{ new(38, 23), "Locked door" }, { new(39, 23), "Unlocked door" }, { new(40, 23), "Locked door" },
			{ new(41, 23), "Unlocked door" }, { new(42, 23), "Locked door" }, { new(43, 23), "Unlocked door" },
			{ new(44, 23), "Locked door" }, { new(45, 23), "Unlocked door" }, { new(46, 23), "Locked door" },
			{ new(47, 23), "Unlocked door" },

			{ new(25, 00), "Self" }, { new(16, 23), "Self" }, { new(17, 23), "Self" }, { new(18, 23), "Self" },
			{ new(19, 23), "Self" }, { new(20, 23), "Self" },

			{ new(08, 19), "Boat" }, { new(09, 19), "Boat" }, { new(10, 19), "Boat" }, { new(11, 19), "Boat" },
			{ new(12, 23), "Boat" }, { new(13, 23), "Boat" }, { new(14, 23), "Boat" }, { new(15, 23), "Boat" },

			{ new(21, 23), "Locked chest" }, { new(22, 23), "Unlocked chest" },
			{ new(21, 24), "Closed chest" }, { new(22, 24), "Opened chest" },
			{ new(05, 07), "Drawer" }, { new(06, 07), "Drawer" }, { new(07, 07), "Drawer" }, { new(08, 07), "Drawer" },

			{ new(04, 08), "Bed" }, { new(05, 08), "Bed" }, { new(06, 08), "Bed" },

			{ new(06, 17), "Chariot" }, { new(07, 17), "Chariot" }, { new(08, 17), "Chariot" }, { new(09, 17), "Chariot" },
			{ new(08, 18), "Cart" }, { new(09, 18), "Cart" },

			{ new(06, 16), "Wheelbarrow" }, { new(07, 16), "Wheelbarrow" }, { new(08, 16), "Wheelbarrow" },
			{ new(09, 16), "Wheelbarrow" },

			// single
			{ new(07, 23), "...on the ground" }, { new(05, 22), "...on your head" }, { new(06, 22), "...on your body" },
			{ new(07, 22), "...on your feet" }, { new(08, 22), "...in your left hand" }, { new(09, 22), "...in your right hand" },
			{ new(10, 22), "...on your back" }, { new(11, 22), "...on your waist" }, { new(43, 06), "...in your quiver" },
			{ new(45, 04), "...in your bag" },

			{ new(08, 23), "Small pile of items" }, { new(9, 23), "Pile of items" }, { new(10, 23), "Big pile of items" },
			{ new(11, 23), "Huge pile of items" },

			{ new(42, 12), "[ENTER] to wait..." },
			{ new(04, 23), "[LEFT CLICK around self] or\n\t[W/A/S/D] to move" },
			{ new(05, 23), "\t\t  On items...\n\n[LEFT CLICK] to display info\n   [LEFT DRAG] to move\n   [RIGHT CLICK] to split" },
		};

		public static GameObject HoldingObject { get; set; }

		[JsonProperty]
		public string Name { get; set; }
		[JsonProperty]
		public Point TileIndexes { get; set; }
		[JsonProperty]
		public int Height { get; set; }
		[JsonProperty]
		public bool IsDragable { get; set; }
		[JsonProperty]
		public bool IsLeftClickable { get; set; }
		[JsonProperty]
		public bool IsRightClickable { get; set; }
		[JsonProperty]
		public bool IsConfirmingClick { get; set; }
		[JsonProperty]
		public bool IsUI { get; set; }
		[JsonProperty]
		public bool IsInTab { get; set; }
		[JsonProperty]
		public string AppearOnTab { get; set; }
		[JsonProperty]
		public bool IsKeptBetweenSessions { get; set; }
		[JsonProperty]
		public bool IsPullableByUnit { get; set; }
		[JsonProperty]
		public string PullRequiredType { get; set; }

		private Point position;
		[JsonProperty]
		public Point Position
		{
			get { return position; }
			set
			{
				if (objects.ContainsKey(position))
				{
					objects[position].Remove(this);
					if (objects[position].Count == 0) objects.Remove(position);
				}
				position = value;
				if (objects.ContainsKey(value) == false)
				{
					objects.Add(value, new List<GameObject>() { this });
					return;
				}
				objects[value].Add(this);
			}
		}

		public Point PreviousPosition { get; set; }
		public string PulledByUnitUID { get; set; }
		public string PullingUID { get; set; }

		private bool leftClicked;
		private string hoveredInfo;
		public string HoveredInfo
		{
			get { return hoveredInfo; }
			protected set
			{
				hoveredInfo = value;
				NavigationPanel.Info.Update();
			}
		}

		public GameObject(string uniqueID, CreationDetails creationDetails) : base(uniqueID)
		{
			Keyboard.Event.Subscribe.KeyPress(uniqueID);
			Keyboard.Event.Subscribe.KeyRelease(uniqueID);
			Mouse.Event.Subscribe.ButtonRelease(uniqueID);
			Game.Event.Subscribe.Update(uniqueID);

			if (this is ITypeTaggable)
				AddTags(GetType().Name);

			if (creationDetails.TileIndexes == default && creationDetails.Name == default && creationDetails.Height == default &&
				creationDetails.Position == default)
				return;

			Name = creationDetails.Name;
			if (creationDetails.TileIndexes != null)
				TileIndexes = creationDetails.TileIndexes.Length == 0 ? creationDetails.TileIndexes[0] :
				creationDetails.TileIndexes[(int)Probability.Randomize(new(0, creationDetails.TileIndexes.Length - 1))];
			Position = creationDetails.Position;
			Height = creationDetails.Height;
			IsDragable = creationDetails.IsDragable;
			IsLeftClickable = creationDetails.IsLeftClickable;
			IsRightClickable = creationDetails.IsRightClickable;
			IsConfirmingClick = creationDetails.IsConfirmingClick;
			IsUI = creationDetails.IsUI;
			IsKeptBetweenSessions = creationDetails.IsKeptBetweenSessions;
			AppearOnTab = creationDetails.AppearOnTab;
			IsInTab = creationDetails.IsInTab;
		}
		public void Display()
		{
			if (IsInTab && AppearOnTab != NavigationPanel.Tab.CurrentTabType) return;

			var worldPos = World.WorldToScreenPosition(Position);
			if (Screen.CellIsOnScreen(Position, IsUI) == false) return;

			if (Position != new Point(-10, 0))
			{
				Screen.EditCell(IsUI ? Position : worldPos, TileIndexes, Height, TileIndexes.Color);
				OnDisplay(IsUI ? Position : worldPos);
			}
		}

		public override void Destroy()
		{
			if (IsDestroyed)
				return;
			objects[Position].Remove(this);
			if (objects[Position].Count == 0)
				objects.Remove(Position);
			if (HoldingObject == this)
				HoldingObject = null;
			base.Destroy();
		}
		public override void OnGameUpdate()
		{
			if (Screen.Sprite == null || NavigationPanel.Info.Textbox == null || Window.CurrentState == Window.State.Minimized)
				return;
			if (IsInTab && AppearOnTab != NavigationPanel.Tab.CurrentTabType)
				return;

			var cursorPos = Screen.GetCellAtCursorPosition();
			var curPos = cursorPos;
			if (IsUI == false)
				curPos = World.ScreenToWorldPosition(cursorPos);
			if (Gate.EnterOnceWhile($"on-hover-{UniqueID}", curPos == Position))
			{
				leftClicked = false;
				if (this is not GameObjectList)
					OnHovered();
			}
			if (Gate.EnterOnceWhile($"on-unhover-{UniqueID}", curPos != Position))
			{
				OnUnhovered();

				if (IsDragable && Mouse.ButtonIsPressed(Mouse.Button.Left) && Position == Base.LeftClickPosition)
				{
					HoldingObject = this;
					Hoverer.CursorTileIndexes = TileIndexes;
					Hoverer.CursorColor = Position.Color == Color.White ? TileIndexes.Color : Position.Color;
					OnDragStart();
				}
			}
		}
		public override void OnMouseButtonRelease(Mouse.Button button)
		{
			if (IsInTab && AppearOnTab != NavigationPanel.Tab.CurrentTabType) return;

			var mousePos = Screen.GetCellAtCursorPosition();
			var pos = IsUI ? Position : World.WorldToScreenPosition(Position);

			if (button == Mouse.Button.Left)
			{
				if (IsLeftClickable && pos == mousePos && pos == Base.LeftClickPosition)
				{
					if (IsConfirmingClick && World.CurrentSession != World.Session.None && leftClicked == false)
					{
						leftClicked = true;
						NavigationPanel.Info.Textbox.Text = "A session is currently ongoing.\n" +
							"Any unsaved progress will be lost.\n" +
							"[LEFT CLICK] to continue";
					}
					else
					{
						leftClicked = false;
						OnHovered();
						OnLeftClicked();
					}
				}
				if (HoldingObject != null)
				{
					if (objects.ContainsKey(mousePos))
						for (int i = 0; i < objects[mousePos].Count; i++)
							if (HoldingObject != objects[mousePos][i])
								objects[mousePos][i].OnDroppedUpon();
					Drop();
				}
			}
			if (button == Mouse.Button.Right && IsRightClickable && pos == mousePos && pos == Base.RightClickPosition)
			{
				OnHovered();
				OnRightClicked();
			}
		}
		public override void OnKeyboardKeyPress(Keyboard.Key key)
		{
			var player = (Player)PickByUniqueID(nameof(Player));
			if (key == Keyboard.Key.ControlLeft && IsPullableByUnit &&
				player.CellIsInReach(Position) && player.PullingUID == null)
			{
				if (player.HasItem(PullRequiredType) || PullRequiredType == null)
				{
					PulledByUnitUID = player.UniqueID;
					player.PullingUID = UniqueID;
				}
				else PlayerStats.Open($"I need {PullRequiredType.ToLower()} to pull that.");
			}
		}
		public override void OnKeyboardKeyRelease(Keyboard.Key key)
		{
			var player = (Player)PickByUniqueID(nameof(Player));
			if (key == Keyboard.Key.ControlLeft && UniqueID == player.PullingUID &&
				PulledByUnitUID == player.UniqueID)
			{
				PulledByUnitUID = null;
				player.PullingUID = null;
			}
		}

		public static void DisplayAllObjects()
		{
			var objs = GetObjectListCopy();
			var units = new List<GameObject>();

			foreach (var kvp in objs)
				for (int i = 0; i < kvp.Value.Count; i++)
				{
					if (kvp.Value[i] is Unit)
						units.Add(kvp.Value[i]);
					else
						kvp.Value[i].Display();
				}

			for (int i = 0; i < units.Count; i++)
				units[i].Display();
		}
		public static void DestroyAllSessionObjects()
		{
			if (World.CurrentSession == World.Session.None)
				return;

			var objsToDestroy = new List<GameObject>();
			foreach (var kvp in objects)
				for (int i = 0; i < kvp.Value.Count; i++)
				{
					if (kvp.Value[i].IsKeptBetweenSessions)
						continue;
					objsToDestroy.Add(kvp.Value[i]);
				}

			for (int i = 0; i < objsToDestroy.Count; i++)
				objsToDestroy[i].Destroy();
		}
		public static List<GameObject> PickByPosition(Point position)
		{
			return objects.ContainsKey(position) ? objects[position] : new List<GameObject>();
		}
		public static void AdvanceTime()
		{
			var objs = GetObjectListCopy();
			foreach (var kvp in objs)
				for (int i = 0; i < kvp.Value.Count; i++)
						kvp.Value[i].OnAdvanceTime();

			var player = (Player)PickByUniqueID(nameof(Player));
			var objsBeneathPlayer = objects[player.position];
			for (int i = 0; i < objsBeneathPlayer.Count; i++)
			{
				if (objsBeneathPlayer[i] is ISolid)
				{
					for (int j = 0; j < objsBeneathPlayer.Count; j++)
						if (objsBeneathPlayer[j] is IRidable)
							objsBeneathPlayer[j].Position = player.PreviousPosition;
					player.Position = player.PreviousPosition;
					World.CameraPosition = player.Position;
				}
			}


			WorldObjectManager.OnAdvanceTime();
			Screen.ScheduleDisplay();
			NavigationPanel.Info.ScheduleUpdate();
		}
		public static void Drop()
		{
			if (HoldingObject == null) return;

			HoldingObject.OnDragEnd();

			HoldingObject = null;
			Hoverer.CursorTileIndexes = new(36, 10);
			Hoverer.CursorColor = Color.White;
			NavigationPanel.Info.ScheduleUpdate();
		}
		public static Dictionary<Point, List<GameObject>> GetObjectListCopy()
		{
			var objs = new Dictionary<Point, List<GameObject>>();
			foreach (var kvp in objects)
				objs[kvp.Key] = new(kvp.Value);
			return objs;
		}
		public static List<string> GetSavableObjects()
		{
			var result = new List<string>();
			foreach (var kvp in objects)
				for (int i = 0; i < kvp.Value.Count; i++)
					if (kvp.Value[i] is ISavable)
						result.Add(kvp.Value[i].UniqueID);
			return result;
		}

		public virtual void OnAdvanceTime() { }
		public virtual void OnLeftClicked() { }
		public virtual void OnRightClicked() { }
		public virtual void OnHovered() { }
		public virtual void OnUnhovered() { }
		public virtual void OnDragStart() { }
		public virtual void OnDragEnd() { }
		public virtual void OnDroppedUpon() { }
		public virtual void OnDisplay(Point screenPos) { }
	}
}
