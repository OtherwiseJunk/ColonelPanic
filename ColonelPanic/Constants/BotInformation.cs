using DartsDiscordBots.Modules.Bot.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace ColonelPanic.Constants
{
	public class BotInformation : IBotInformation
	{
		public string InstallationLink { get; set; } = "https://discord.com/oauth2/authorize?client_id=357910708316274688&scope=bot&permissions=8";
		public string GithubRepo { get; set; } = "https://github.com/OtherwiseJunk/ColonelPanic";
        public static Regex isMentioningMeRegex = new Regex(@"(Co?l?o?n?e?l?)(\.?|\s)*(Pa?o?n?i?c?)?");
        public const ulong botUserId = 357910708316274688;
    }
}
