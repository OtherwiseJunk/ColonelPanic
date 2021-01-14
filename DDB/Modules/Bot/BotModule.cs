using DartsDiscordBots.Modules.Bot.Interfaces;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;

namespace DartsDiscordBots.Modules.Bot
{
	public class BotModule : ModuleBase
	{
		public IBotInformation _info { get; set; }
		[Command("listchnl"), RequireOwner]
		public async Task listChannels()
		{
			string msg = "I'm in these guilds/channels:" + Environment.NewLine + Environment.NewLine;
			foreach (var guild in Context.Client.GetGuildsAsync().Result)
			{
				msg += "**" + guild.Name + "**" + Environment.NewLine;
				foreach (var chnl in guild.GetChannelsAsync().Result)
				{
					if (msg.Length > 1900)
					{
						await Context.Channel.SendMessageAsync(msg);
						msg = "";
					}
					msg += chnl.Name + Environment.NewLine;
				}
			}
			await Context.Channel.SendMessageAsync(msg);
		}

		[Command("playing"), RequireOwner]
		public async Task setPlaying([Remainder] string playing)
		{
			var client = Context.Client as DiscordSocketClient;
			await client.SetGameAsync(playing);
		}

		[Command("say"), Summary("Echos a message."), RequireOwner]
		public async Task Say([Remainder, Summary("The text to echo")] string echo)
		{
			// ReplyAsync is a method on ModuleBase
			await ReplyAsync(echo);
			Context.Message.DeleteAsync();
		}

		[Command("link"), Alias("install"), Summary("Provides a link for installing the bot on other servers. You must be an admin of the target server to use the provided link.")]
		public async Task ProvideInstallLink()
		{
			await Context.Channel.SendMessageAsync(_info.InstallationLink);
		}

		[Command("repo"), Alias("github"), Summary("Provides a link for the bot's.")]
		public async Task ProvideRepoLink()
		{
			await Context.Channel.SendMessageAsync(_info.GithubRepo);
		}
		[Command("renick"), RequireOwner, Summary("Renames the bot")]
		public async Task ChangeNickname([Remainder, Summary("what to rename the bot")] string newNick)
		{
			await Context.Guild.GetCurrentUserAsync().Result.ModifyAsync(b => b.Nickname = newNick);
		}
	}
}
