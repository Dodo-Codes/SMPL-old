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
			NavigationPanel.Tab.Textbox.Text = say == null ? "" : $"\n\"{say}\"";
		}
		public override void OnDisplay(Point screenPos)
		{
			var player = (Player)PickByUniqueID(nameof(Player));

			Screen.DisplayText(new(19, 5), 1, Color.White, $"state {player.Health[0]} {player.Health[1]}");
			Screen.EditCell(new(19 + $"state {player.Health[0]}".Length, 5), new(39, 20), 1, Color.White);

			for (int i = 0; i < player.EffectUIDs.Count; i++)
			{
				var effect = (Effect)PickByUniqueID(player.EffectUIDs[i]);
				effect.OnDisplay(new(19, 6 + i));
			}
		}
	}
}
