using System;
using System.Collections.Generic;
using System.Text;

namespace DartsDiscordBots.Modules.Bot.Interfaces
{
	public interface IBotInformation
	{
		public string InstallationLink { get; set; }
		public string GithubRepo { get; set; }
	}
}
