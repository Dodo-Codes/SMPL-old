using Newtonsoft.Json;
using SMPL.Components;
using SMPL.Data;
using SMPL.Gear;

namespace RPG1bit
{
	public class Map : Item
	{
		private readonly static Size size = new(200, 88);

		[JsonProperty]
		public Point MapPosition { get; set; }

		public static Sprite Sprite { get; set; }
		public static Area Area { get; set; }

		public Map(string uniqueID, CreationDetails creationDetails) : base(uniqueID, creationDetails)
		{
			Camera.Event.Subscribe.Display(uniqueID, 2);

			MaxQuantity = 9;

			CanCarryInBag = true;
			CanCarryInQuiver = true;

			if (Gate.EnterOnceWhile("map-initialize", true))
			{
				Area = new Area($"item-map-area")
				{
					Position = new(60 * 9 + 15, -60 * 3),
					Size = new(60 * (size.W / 16), 60 * (size.H / 16))
				};
				Sprite = new Sprite($"item-map-sprite")
				{
					AreaUniqueID = Area.UniqueID,
				};
			}
		}
		public override Item OnSplit() => CloneObject(this, $"{UniqueID}-{Performance.FrameCount}");
		public override void OnCameraDisplay(Camera camera)
		{
			if (NavigationPanel.Tab.CurrentTabType == "item-info" && ItemStats.DisplayedItemUID == UniqueID)
				Sprite.Display(camera);
		}
		public override void OnItemInfoDisplay()
		{
			NavigationPanel.Tab.Texts["item-info"] = $"Map x{Quantity}/{MaxQuantity}";
			var id = $"world-{MapPosition}";

			if (Gate.EnterOnceWhile(id, Assets.AreLoaded(id) == false))
			{
				MapPosition = ((Player)PickByUniqueID(nameof(Player))).Position;
				var pixels = new Color[(int)size.W, (int)size.H];
				for (double x = Position.X - size.W / 2; x <= Position.X + size.W / 2; x++)
					for (double y = Position.Y - size.H / 2; y <= Position.Y + size.H / 2; y++)
					{
						var px = (int)x + (int)size.W / 2 - (int)MapPosition.X;
						var py = (int)y + (int)size.H / 2 - (int)MapPosition.Y;
						for (int i = 2; i >= 0; i--)
						{
							var tile = ChunkManager.GetTile(new(x, y), i);
							if (tile != default && IsNot("boat"))
							{
								pixels[px, py] = tile.C;
								break;
							}

							bool IsNot(string type) => WorldEditor.Tiles[type].Contains(tile) == false;
						}
					}

				Assets.CreateTexture(pixels, id);
			}
			Sprite.TexturePath = id;
		}
	}
}
