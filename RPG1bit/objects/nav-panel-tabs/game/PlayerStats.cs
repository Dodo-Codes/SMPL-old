using SMPL.Data;
using SMPL.Gear;

namespace RPG1bit
{
	public class PlayerStats : GameObject
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
			NavigationPanel.Tab.Textbox.Text = say == null ? ". . ." : $"\"{say}\"";
			var player = (Player)PickByUniqueID(nameof(Player));
			if (player != null && player.Health[0] <= 0)
				NavigationPanel.Tab.Textbox.Text = "You died.";
		}
		public override void OnDisplay(Point screenPos)
		{
			for (int y = 0; y < 6; y++)
				for (int x = 0; x < 13; x++)
					Screen.EditCell(new(19 + x, 4 + y), new Point(12, 22), 0, Color.Brown * 0.7);

			var player = (Player)PickByUniqueID(nameof(Player));
			var hpValue = Number.Limit(player.Health[0], new(0, player.Health[1]));
			var hp = $"{hpValue}";
			var maxHp = $"{player.Health[1]}";
			var maxHpStr = maxHp.Length == 1 ? $"0{maxHp}" : maxHp;
			var hpStr = hp.Length == 1 ? $"0{hp}" : hp;
			var skills = PickByTag(nameof(PlayerSkill));
			var hpRed = Number.Map(hpValue, new(0, player.Health[1]), new(255, 0));
			var hpGreen = Number.Map(hpValue, new(0, player.Health[1]), new(0, 255));
			var col = new Color(hpRed, hpGreen, 0);

			Screen.DisplayText(new(19, 3), 1, col, $"health {hpStr} {maxHpStr}");
			Screen.EditCell(new(28, 3), new(39, 20), 1, col);

			if (skills.Length == 0)
			{
				Screen.DisplayText(new(21, 6), 1, Color.Gray, "no skills");
				Screen.DisplayText(new(22, 7), 1, Color.Gray, "learned");
			}
			else
				Screen.DisplayText(new(19, 4), 1, Color.Gray, "skills");

			if (player.EffectUIDs.Count == 0)
			{
				Screen.DisplayText(new(21, 11), 1, Color.Gray, "no status");
				Screen.DisplayText(new(22, 12), 1, Color.Gray, "effects");
			}
			else
				Screen.DisplayText(new(19, 10), 1, Color.Gray, "effects");
		}
		public static void UpdateContent()
		{
			var player = (Player)PickByUniqueID(nameof(Player));
			var skills = PickByTag(nameof(PlayerSkill));
			var p = new Point();
			for (int i = 1; i < skills.Length + 1; i++)
			{
				if (p.Y == 5)
				{
					Screen.EditCell(new(31, 9), new(41, 20), 1, Color.White);
					continue;
				}
				var skill = (PlayerSkill)skills[i - 1];
				skill.Position = new(20 + p.X, 5 + p.Y);
				p.X += 4;
				if (p.X >= 10)
				{
					p.X = 0;
					p.Y++;
				}
			}

			var pos = new Point();
			for (int i = 1; i < player.EffectUIDs.Count + 1; i++)
			{
				if (pos.Y == 3)
				{
					Screen.EditCell(new(31, 13), new(41, 20), 1, Color.White);
					continue;
				}
				var effect = (Effect)PickByUniqueID(player.EffectUIDs[i - 1]);
				effect.Position = new(20 + pos.X, 11 + pos.Y);
				pos.X += 4;
				if (pos.X >= 10)
				{
					pos.X = 0;
					pos.Y++;
				}
			}
		}
	}
}
