using DartsDiscordBots.Modules.Jackbox.Interfaces;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DartsDiscordBots.Modules.Jackbox
{
	public class JackboxModule : ModuleBase
	{
		IJackboxConstants _jackboxConstants { get; set; }

		public JackboxModule(IJackboxConstants jackboxConstants)
		{
			_jackboxConstants = jackboxConstants;
		}

		[Command("jackbox")]
		[Summary("Makes a jackbox poll, and will announce a winner after 5 mintues. User must provide a comma separated list of the jack.")]
		public async Task Jackbox([Summary("A comma seperated list of the versions of jackbox to make the list for")] string versions)
		{
			List<int> versionList;
			try
			{
				versionList = versions.Split(',').ToList().ConvertAll(int.Parse);
			}
			catch
			{
				await Context.Channel.SendMessageAsync("Sorry, I was unable to parse one of those numbers.");
				return;
			}

			StringBuilder sb = new StringBuilder($"Here's the list of games for that! We're using approval voting, react to this message with all the games you'd be OK with playing!{Environment.NewLine}");

			foreach(int versionNum in versionList)
			{
				sb.AppendLine($"`Jackbox Party Pack {versionNum} games:`");
				foreach(string game in _jackboxConstants.JackboxGameListByNumber[versionNum])
				{
					sb.AppendLine(game);
				}			
			}

			await Context.Channel.SendMessageAsync(sb.ToString());
		}
	}
}
