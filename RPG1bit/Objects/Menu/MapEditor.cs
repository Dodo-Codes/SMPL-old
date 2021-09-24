using SMPL.Data;
using SMPL.Gear;
using System.Collections.Generic;

namespace RPG1bit
{
	public class MapEditor : Object
	{
		private static readonly Dictionary<Point, Point[]> randomTiles = new()
		{
			{ new(5, 0), new Point[] { new(5, 0), new(6, 0), new(7, 0) } }
		};
		private static Point CurrentTile
		{
			get
			{
				var tiles = randomTiles[SwitchType.BrushType];
				var randomTileIndex = (int)Probability.Randomize(new Number.Range(0, tiles.Length - 1));
				var result = tiles[randomTileIndex];
				result.Color = SwitchColor.BrushColor;
				return result;
			}
		}

		public MapEditor(CreationDetails creationDetails) : base(creationDetails) { }
		protected override void OnHovered() => NavigationPanel.Info.Textbox.Text = "Start a new map edit session.";
		protected override void OnLeftClicked()
		{
			if (Map.CurrentSession != Map.Session.MapEdit) Map.DestroyAllSessionObjects();
			Map.DisplayNavigationPanel();
			Map.CurrentSession = Map.Session.MapEdit;

			Map.CameraPosition = new(8, 8);

			Map.CreateUIButtons();
			Map.Display(); // for the map iteself
			DisplayAllObjects(); // for the ui
		}

		public static void PlaceCurrentTile()
		{
			var mouseCell = Screen.GetCellAtCursorPosition();
			var pos = Map.ScreenToMapPosition(mouseCell);
			Map.Data[(int)pos.X, (int)pos.Y, SwitchHeight.BrushHeight] = CurrentTile;
			Map.Display();
		}
	}
}
