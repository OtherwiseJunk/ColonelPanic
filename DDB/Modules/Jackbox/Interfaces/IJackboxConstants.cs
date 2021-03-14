using System;
using System.Collections.Generic;
using System.Text;

namespace DartsDiscordBots.Modules.Jackbox.Interfaces
{
	public interface IJackboxConstants
	{
		public int JackboxMaxVersion { get; set; }
		public Dictionary<int, List<string>> JackboxGameListByNumber { get; set; }
		public List<string> JackboxOneGames { get; set; }
		public List<string> JackboxTwoGames { get; set; }
		public List<string> JackboxThreeGames { get; set; }
		public List<string> JackboxFourGames { get; set; }
		public List<string> JackboxFiveGames { get; set; }
		public List<string> JackboxSixGames { get; set; }
		public List<string> JackboxSevenGames { get; set; }
	}
}
