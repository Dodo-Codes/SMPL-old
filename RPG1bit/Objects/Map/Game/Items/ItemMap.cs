using Newtonsoft.Json;
using SMPL.Components;
using SMPL.Data;
using SMPL.Gear;

namespace RPG1bit
{
	public class ItemMap : Item
	{
		private readonly static Size size = new(200, 88);
		private bool display;
		[JsonProperty]
		public Sprite Sprite { get; set; }
		[JsonProperty]
		public Area Area { get; set; }

		public ItemMap(string uniqueID, CreationDetails creationDetails) : base(uniqueID, creationDetails)
		{
			Camera.Event.Subscribe.Display(uniqueID, 2);

			CanCarryInBag = true;
			CanCarryInQuiver = true;
		}
		public override void Destroy()
		{
			Sprite.Destroy();
			Area.Destroy();
			Assets.Unload(Assets.Type.Texture, $"map-{UniqueID}");
			base.Destroy();
		}
		public override void OnCameraDisplay(Camera camera)
		{
			if (display)
				Sprite.Display(camera);
		}
		public override void OnItemInfoDisplay()
		{
			NavigationPanel.Tab.Texts["item-info"] = $"Map";
			display = true;

			if (Gate.EnterOnceWhile($"{UniqueID}-mapp-creation", true))
			{
				var pixels = new Color[(int)size.W, (int)size.H];
				var playerPos = ((Player)PickByUniqueID(nameof(Player))).Position;
				for (double x = Position.X - size.W / 2; x <= Position.X + size.W / 2; x++)
					for (double y = Position.Y - size.H / 2; y <= Position.Y + size.H / 2; y++)
					{
						var px = (int)x + (int)size.W / 2 - (int)playerPos.X;
						var py = (int)y + (int)size.H / 2 - (int)playerPos.Y;
						if (Map.RawData.ContainsKey(new(x, y)))
							for (int i = 2; i >= 0; i--)
							{
								var tile = Map.RawData[new(x, y)][i];
								if (tile != default && IsNot("boat"))
								{
									pixels[px, py] = tile.C;
									break;
								}

								bool IsNot(string type) => MapEditor.Tiles[type].Contains(tile) == false;
							}
					}

				Assets.CreateTexture(pixels, $"map-{UniqueID}");

				Area = new Area($"{UniqueID}-area")
				{
					Position = new(60 * 9 + 15, -60 * 3),
					Size = new(60 * (size.W / 16), 60 * (size.H / 16))
				};
				Sprite = new Sprite($"{UniqueID}-sprite")
				{
					AreaUniqueID = Area.UniqueID,
					TexturePath = $"map-{UniqueID}",
				};
			}
		}
		public override void OnAdvanceTime() => display = false;
	}
}
