using SMPL.Gear;
using SMPL.Data;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace RPG1bit
{
	public class Player : Unit
	{
		public Player(string uniqueID, CreationDetails creationDetails) : base(uniqueID, creationDetails)
		{
			CreateSkill("goalkeep", 10, "penka", new(25, 0));
			CreateSkill("goalkeep1", 10, "penka", new(25, 0));
			CreateSkill("goalkeep2", 10, "penka", new(25, 0));
			CreateSkill("goalkeep3", 10, "penka", new(25, 0));
			CreateSkill("goalkeep4", 10, "penka", new(25, 0));
			CreateSkill("goalkeep5", 10, "penka", new(25, 0));
			CreateSkill("goalkeep6", 10, "penka", new(25, 0));
			CreateSkill("goalkeep7", 10, "penka", new(25, 0));
			CreateSkill("goalkeep8", 10, "penka", new(25, 0));
			CreateSkill("goalkeep9", 10, "penka", new(25, 0));
		}

		public static void CreateSkill(string name, int value, string description, Point tileIndexes)
		{
			new PlayerSkill($"player-skill-{name}", new()
			{
				Name = name,
				Position = new(-20, 0),
				Height = 1,
				TileIndexes = new Point[] { tileIndexes },
				IsUI = true,
				IsInTab = true,
				AppearOnTab = "player-stats",
			})
			{
				Value = value,
				Description = description
			};
			PlayerStats.UpdateContent();
			Screen.ScheduleDisplay();
		}
		public static PlayerSkill GetSkill(string name)
		{
			var skill = (PlayerSkill)PickByUniqueID($"player-skill-{name}");
			return skill;
		}

		public override void OnKeyboardKeyPress(Keyboard.Key key)
		{
			if (Health[0] <= 0)
				return;

			var dir = new Point();
			if (key == Keyboard.Key.A) dir = new(-1, 0);
			else if (key == Keyboard.Key.D) dir = new(1, 0);
			else if (key == Keyboard.Key.W) dir = new(0, -1);
			else if (key == Keyboard.Key.S) dir = new(0, 1);
			else if (key == Keyboard.Key.Enter)
			{
				PreviousPosition = Position;
				AdvanceTime();
				NavigationPanel.Tab.Textbox.Text = "Some time passes...";
				return;
			}

			TryMove(dir);
		}
		public override void OnGameUpdate()
		{
			base.OnGameUpdate();

			var mousePosWorld = World.ScreenToWorldPosition(Screen.GetCellAtCursorPosition());
			Hoverer.TileIndexes = Hoverer.DefaultTileIndexes;

			if (World.IsHovered() && CellIsInReach(mousePosWorld))
			{
				if (CanMoveIntoCell(mousePosWorld))
				{
					Hoverer.TileIndexes = new(4, 23);
					Hoverer.Area.Angle = 0;
					if (CellIsInLeftReach(mousePosWorld)) Hoverer.Area.Angle = 270;
					if (CellIsInRightReach(mousePosWorld)) Hoverer.Area.Angle = 90;
					if (CellIsInUpReach(mousePosWorld)) Hoverer.Area.Angle = 0;
					if (CellIsInDownReach(mousePosWorld)) Hoverer.Area.Angle = 180;
				}

				var objs = objects.ContainsKey(mousePosWorld) ? objects[mousePosWorld] : new();
				for (int i = 0; i < objs.Count; i++)
					if (objs[i] is IInteractable)
					{
						Hoverer.TileIndexes = new(27, 14);
						return;
					}
			}
		}
		public override void OnMouseButtonRelease(Mouse.Button button)
		{
			base.OnMouseButtonRelease(button);
			if (Health[0] <= 0 || button != Mouse.Button.Left)
				return;

			var mousePos = Screen.GetCellAtCursorPosition();

			if (World.CurrentSession == World.Session.Single && mousePos.X < 18)
				PlayerStats.Open();

			if (Base.LeftClickPosition != mousePos)
				return;

			var mousePosWorld = World.ScreenToWorldPosition(mousePos);
			var movement = new Point(0, 0);

			if (CellIsInLeftReach(mousePosWorld)) movement = new Point(-1, 0);
			else if (CellIsInRightReach(mousePosWorld)) movement = new Point(1, 0);
			else if (CellIsInUpReach(mousePosWorld)) movement = new Point(0, -1);
			else if (CellIsInDownReach(mousePosWorld)) movement = new Point(0, 1);

			TryMove(movement);
		}
		private void TryMove(Point movement)
		{
			if (movement != new Point() && Move(movement))
			{
				PlayerStats.Open();
				AdvanceTime();
				TileIndexes = new(25, 0);

				if (World.PositionHasWaterAsHighest(Position))
				{
					TileIndexes = new(20, 23);
					for (int i = 0; i < objects[Position].Count; i++)
						if (objects[Position][i] is Boat boat)
						{
							var tile = boat.GetPlayerTile();
							TileIndexes = new(tile.X, tile.Y) { C = TileIndexes.C };
						}
				}
			}
			World.IsShowingRoofs = World.TileHasRoof(Position) == false;
		}
	}
}
