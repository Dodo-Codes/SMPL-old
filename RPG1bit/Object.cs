using SMPL.Components;
using SMPL.Data;
using SMPL.Gear;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace RPG1bit
{
	public class Object : Thing
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

		public static readonly Dictionary<Point, List<Object>> objects = new();
		public static readonly Dictionary<Point, string> descriptions = new()
		{
			{ new(33, 15), "Graphics 1-Bit Pack by kenney.nl\n" +
				"Font DPComic by cody@zone38.net\n" +
				"Music by opengameart.org/users/yubatake\n" +
				"Music by opengameart.org/users/avgvsta\n" +
				$"Game {NavigationPanel.Info.GameVersion} & SFX(software: Bfxr) by dodo" },
			{ new(00, 00), "The Unknown?" },
			{ new(01, 22), "" }, // background color
			{ new(04, 22), "Game navigation panel" }, { new(00, 23), "Game navigation panel" },
			{ new(29, 15), "Game navigation panel" }, { new(30, 15), "Game navigation panel" },
			{ new(02, 22), "Game navigation panel" }, { new(03, 22), "Game navigation panel" },
			{ new(01, 23), "Information box" },

			{ new(42, 12), "[LEFT CLICK] Wait..." },
			{ new(13, 22), "[LEFT CLICK] Type anything..." },

			{ new(44, 16), "Start a new multiplayer game session\n  (not available in this game version)" },

			{ new(41, 13), "          [LEFT DRAG] Paint a tile\n[SHIFT + LEFT DRAG] Paint multiple tiles" },
			{ new(42, 13), "          [RIGHT DRAG] Erase a tile\n[SHIFT + RIGHT DRAG] Erase multiple tiles" },
			{ new(43, 13), "[MIDDLE CLICK] Pick a brush from the tile\n\t\tat that particular height" },
			{ new(06, 23), "Roofs visibility." },

			{ new(04, 00), "Grass" }, { new(05, 00), "Grass" }, { new(06, 00), "Grass" }, { new(07, 00), "Grass" },
			{ new(00, 01), "Tree" }, { new(1, 1), "Tree" }, { new(2, 1), "Trees" }, { new(3, 1), "Trees" }, { new(4, 1), "Tree" },
			{ new(05, 01), "Tree" }, { new(6, 1), "Tree" }, { new(7, 1), "Tree" },
			{ new(05, 02), "Cactuses" }, { new(6, 2), "Cactus" },
			{ new(07, 02), "Palm" },
			{ new(04, 02), "Rocks" },
			{ new(00, 02), "Bush" }, { new(1, 2), "Bush" }, { new(2, 2), "Bush" }, { new(3, 2), "Bush" },

			{ new(15, 3), "Flowers" }, { new(16, 3), "Flowers" },

			{ new(07, 03), "Water" }, { new(8, 3), "Water" }, { new(9, 3), "Water" }, { new(10, 3), "Water" },
			{ new(11, 03), "Water" }, { new(12, 3), "Water" }, { new(7, 4), "Water" }, { new(8, 4), "Water" },
			{ new(09, 04), "Water" }, { new(10, 4), "Water" }, { new(11, 4), "Water" }, { new(12, 4), "Water" },
			{ new(13, 04), "Water" }, { new(14, 4), "Water" }, { new(7, 5), "Water" }, { new(8, 5), "Water" },
			{ new(09, 05), "Water" }, { new(10, 5), "Water" }, { new(11, 5), "Water" },

			{ new(12, 05), "Bridge" }, { new(13, 5), "Bridge" }, { new(14, 5), "Broken bridge" }, { new(15, 5), "Bridge" },
			{ new(16, 5), "Bridge" },

			{ new(09, 00), "Dirt road" }, { new(10, 0), "Dirt road" }, { new(11, 0), "Dirt road" }, { new(12, 0), "Dirt road" },
			{ new(13, 00), "Dirt road" }, { new(14, 0), "Dirt road" }, { new(15, 0), "Dirt road" }, { new(9, 1), "Dirt road" },
			{ new(10, 01), "Dirt road" }, { new(11, 1), "Dirt road" }, { new(12, 1), "Dirt road" }, { new(13, 1), "Dirt road" },
			{ new(14, 01), "Dirt road" }, { new(8, 0), "Dirt road" }, { new(8, 1), "Dirt road" },

			{ new(15, 01), "Stone road" }, { new(16, 1), "Stone road" }, { new(17, 1), "Stone road" }, { new(8, 2), "Stone road" },
			{ new(09, 02), "Stone road" }, { new(10, 2), "Stone road" }, { new(11, 2), "Stone road" }, { new(12, 2), "Stone road" },
			{ new(13, 02), "Stone road" }, { new(14, 2), "Stone road" }, { new(15, 2), "Stone road" }, { new(16, 2), "Stone road" },
			{ new(17, 02), "Stone road" }, { new(13, 3), "Stone road" }, { new(14, 3), "Stone road" },

			{ new(01, 07), "Sign" }, { new(0, 7),	"Sign" }, { new(2, 7), "Sign" }, { new(1, 8), "Sign" }, { new(0, 8), "Sign" },

			{ new(00, 03), "Fence" }, { new(01, 03), "Fence" }, { new(02, 03), "Fence" }, { new(05, 03), "Fence" },
			{ new(06, 03), "Fence" }, { new(03, 04), "Closed fence gate" }, { new(02, 04), "Fence" },
			{ new(03, 03), "Closed fence gate" }, { new(05, 04), "Closed fence gate" }, { new(04, 04), "Closed fence gate" },
			{ new(00, 04), "Closed fence gate" }, { new(04, 03), "Opened fence gate" }, { new(06, 04), "Opened fence gate" },
			{ new(06, 05), "Fence" }, { new(01, 04), "Opened fence gate" },

			{ new(28, 22), "Roof" }, { new(29, 22), "Roof" }, { new(30, 22), "Roof" }, { new(28, 23), "Roof" },
			{ new(29, 23), "Roof" }, { new(30, 23), "Roof" }, { new(37, 22), "Roof" },
			{ new(31, 22), "Chimney" },

			{ new(33, 22), "Structure" }, { new(34, 22), "Structure" }, { new(35, 22), "Structure" }, { new(36, 22), "Structure" },
			{ new(37, 23), "Structure" }, { new(24, 23), "Structure" }, { new(25, 23), "Structure" }, { new(26, 23), "Structure" },

			{ new(38, 22), "Closed door" }, { new(39, 22), "Opened door" }, { new(40, 22), "Closed door" },
			{ new(41, 22), "Opened door" }, { new(42, 22), "Closed door" }, { new(43, 22), "Opened door" },
			{ new(44, 22), "Closed door" }, { new(45, 22), "Opened door" }, { new(46, 22), "Closed door" },
			{ new(47, 22), "Opened door" },

			{ new(38, 23), "Locked door" }, { new(39, 23), "Unlocked door" }, { new(40, 23), "Locked door" },
			{ new(41, 23), "Unlocked door" }, { new(42, 23), "Locked door" }, { new(43, 23), "Unlocked door" },
			{ new(44, 23), "Locked door" }, { new(45, 23), "Unlocked door" }, { new(46, 23), "Locked door" },
			{ new(47, 23), "Unlocked door" },

			// single
			{ new(07, 23), "...on the ground" }, { new(05, 22), "...on your head" }, { new(06, 22), "...on your body" },
			{ new(07, 22), "...on your feet" }, { new(08, 22), "...in your left hand" }, { new(09, 22), "...in your right hand" },
			{ new(10, 22), "...on your back" }, { new(11, 22), "...on your waist" }, { new(43, 06), "...in your quiver" },
			{ new(45, 04), "...in your bag" },

			{ new(08, 23), "Small pile of items" }, { new(9, 23), "Pile of items" }, { new(10, 23), "Big pile of items" },
			{ new(11, 23), "Huge pile of items" },

			{ new(04, 23), "[LEFT CLICK] Move the character" },
			{ new(05, 23), "\t\tOn items...\n\n[LEFT CLICK] Info\n[LEFT DRAG] Pickup/Drop\n" +
				"[RIGHT CLICK] Split" },

			{ new(25, 00), "Player" }, { new(16, 23), "Player" }, { new(17, 23), "Player" }, { new(18, 23), "Player" },
			{ new(19, 23), "Player" }, { new(20, 23), "Player" },

			{ new(08, 19), "Boat" }, { new(09, 19), "Boat" }, { new(10, 19), "Boat" }, { new(11, 19), "Boat" },
			{ new(12, 23), "Boat" }, { new(13, 23), "Boat" }, { new(14, 23), "Boat" }, { new(15, 23), "Boat" },

			{ new(21, 23), "Locked chest" }, { new(22, 23), "Unlocked chest" },
			{ new(21, 24), "Closed chest" }, { new(22, 24), "Opened chest" },
			{ new(05, 07), "Drawer" }, { new(06, 07), "Drawer" }, { new(07, 07), "Drawer" }, { new(08, 07), "Drawer" },

			{ new(31, 23), "Window" }, { new(32, 23), "Window" }, { new(33, 23), "Window" }, { new(34, 23), "Window" },
			{ new(35, 23), "Window" }, { new(36, 23), "Window" },

			{ new(11, 07), "Table" }, { new(12, 07), "Table" }, { new(13, 07), "Table" }, { new(14, 07), "Table" },
			{ new(15, 07), "Table" }, { new(16, 07), "Table" }, { new(13, 08), "Table" }, { new(13, 09), "Table" },
			{ new(13, 10), "Table" },

			{ new(10, 08), "Chair" }, { new(11, 08), "Chair" }, { new(12, 08), "Chair" },

			{ new(04, 08), "Bed" }, { new(05, 08), "Bed" }, { new(06, 08), "Bed" },

			{ new(03, 07), "Shelf" }, { new(04, 07), "Shelf" },
			{ new(09, 07), "Counter" }, { new(10, 07), "Counter" },

			{ new(07, 08), "Fireplace" }, { new(08, 08), "Fireplace" }, { new(09, 08), "Fireplace" },
		};
		public static Object HoldingObject { get; set; }

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
		public bool IsKeptBetweenSessions { get; set; }
		[JsonProperty]
		public string AppearOnTab { get; set; }

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
					objects.Add(value, new List<Object>() { this });
					return;
				}
				objects[value].Add(this);
			}
		}

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

		public Object(string uniqueID, CreationDetails creationDetails) : base(uniqueID)
		{
			Mouse.Event.Subscribe.ButtonRelease(uniqueID);
			Game.Event.Subscribe.Update(uniqueID);

			if (this is ITypeTaggable)
				AddTags(GetType().Name);

			if (creationDetails.TileIndexes == null && creationDetails.Name == null && creationDetails.Height == 0)
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
		public static void DisplayAllObjects()
		{
			var objs = GetObjectList();
			foreach (var kvp in objs)
				for (int i = 0; i < kvp.Value.Count; i++)
					kvp.Value[i].Display();
		}
		public static void DestroyAllSessionObjects()
		{
			if (World.CurrentSession == World.Session.None)
				return;

			var objsToDestroy = new List<Object>();
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
		public static List<Object> PickByPosition(Point position)
		{
			return objects.ContainsKey(position) ? objects[position] : new List<Object>();
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
				if (this is not ObjectList)
					OnHovered();
			}
			if (Gate.EnterOnceWhile($"on-unhover-{UniqueID}", curPos != Position))
			{
				OnUnhovered();

				if (IsDragable && Mouse.ButtonIsPressed(Mouse.Button.Left) && Position == Base.LeftClickPosition)
				{
					HoldingObject = this;
					Hoverer.CursorTileIndexes = TileIndexes;
					Hoverer.CursorColor = Position.C == Color.White ? TileIndexes.C : Position.C;
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
							"[LEFT CLICK] Continue";
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
		public void Display()
		{
			if (IsInTab && AppearOnTab != NavigationPanel.Tab.CurrentTabType) return;

			var worldPos = World.WorldToScreenPosition(Position);
			if (Screen.CellIsOnScreen(Position, IsUI) == false) return;

			if (Position != new Point(-10, 0))
			{
				Screen.EditCell(IsUI ? Position : worldPos, TileIndexes, Height, TileIndexes.C);
				OnDisplay(IsUI ? Position : worldPos);
			}
		}
		public static void AdvanceTime()
		{
			var objs = GetObjectList();

			foreach (var kvp in objs)
				for (int i = 0; i < kvp.Value.Count; i++)
					objs[kvp.Key][i].OnAdvanceTime();

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
		public static Dictionary<Point, List<Object>> GetObjectList()
		{
			var objs = new Dictionary<Point, List<Object>>();
			foreach (var kvp in objects)
				objs[kvp.Key] = new(kvp.Value);
			return objs;
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
