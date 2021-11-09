using SMPL.Data;
using SMPL.Gear;

namespace RPG1bit
{
	public class PlayerStats : Object
	{
		private static readonly string[] thoughts = new string[]
		{
			"Words are just that...", "We miss the true number one, three and\ntwo too...",
			"\"An idiot admires complexity, a genius admires\nsimplicity.\" We miss you, Terry...", "MMMMMMagot...",
			"That took a turn...", "Easter egg?", "Souther egg!", "Wester egg!", "Norther egg!",
			"I've always felt like I am a part of a \ncontrolled environment...", "Who's dodo?", "Horses with horns have to exist...",
			"Tale of ale... or was it Tale and Ale...", "A thought that occured to me was not to forget to",
			"Doggerche...", "Medieval censorship...", "Knots are haaaard...", "Slap, sla-slap... slap, slap, slap...",
			"No one is prepared for my Mountain Blade...", "Lada-bada...", "Labladabla...", "Word, word word word... word - world...",
			"Golira..."
		};
		public static string DisplayedItemUID { get; set; }

		public PlayerStats(string uniqueID, CreationDetails creationDetails) : base(uniqueID, creationDetails) { }

		public static void Open(string say = null)
		{
			NavigationPanel.Tab.Open("player-stats", "self");
			NavigationPanel.Tab.Textbox.Scale = new(0.35, 0.35);
			NavigationPanel.Tab.Textbox.Text = say == null ? "\n. . ." : $"\n\"{say}\"";
			var player = (Player)PickByUniqueID(nameof(Player));
			if (player != null && player.Health[0] <= 0)
				NavigationPanel.Tab.Textbox.Text = "All your senses weakened as\nyou took your final breath...";
		}
		public override void OnDisplay(Point screenPos)
		{
			var player = (Player)PickByUniqueID(nameof(Player));
			var hp = Number.Limit(player.Health[0], new(0, player.Health[1]));

			Screen.DisplayText(new(20, 5), 1, Color.White, $"state {hp} {player.Health[1]}");
			Screen.EditCell(new(20 + $"state {hp}".Length, 5), new(39, 20), 1, Color.White);

			var pos = new Point();
			for (int i = 1; i < player.EffectUIDs.Count + 1; i++)
			{
				if (i == 19)
					return;
				var effect = (Effect)PickByUniqueID(player.EffectUIDs[i - 1]);
				effect.Position = new(20 + pos.X, 7 + pos.Y);
				pos.X += 2 + $"{effect.Duration[1] - effect.Duration[0]}".Length;
				if (pos.X >= 10)
				{
					pos.X = 0;
					pos.Y++;
				}
			}
		}
	}
}
