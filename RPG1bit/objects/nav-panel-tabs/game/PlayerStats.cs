using SMPL.Data;
using SMPL.Gear;

namespace RPG1bit
{
	public class PlayerStats : Object
	{
		private static string[] thoughts = new string[]
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
			NavigationPanel.Tab.Textbox.Scale = new(0.3, 0.3);
			NavigationPanel.Tab.Textbox.OriginPercent = new(5, 0);
			NavigationPanel.Tab.Textbox.Text = say == null ?
				$"\n*{thoughts[(int)Probability.Randomize(new(0, thoughts.Length - 1))]}*" : $"\"{say}\"";
		}
		public override void OnDisplay(Point screenPos)
		{
			Screen.DisplayText(new(19, 2), 1, Color.White, "thought");
			Screen.DisplayText(new(19, 10), 1, Color.White, "condition");
		}
	}
}
