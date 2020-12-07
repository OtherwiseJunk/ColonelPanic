﻿using ColonelPanic.Database.Contexts;
using DartsDiscordBots.Utilities;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Victoria;
using CEC = ColonelPanic.Constants.CustomEmoteConstants;

namespace ColonelPanic.Services
{
    public sealed class DiscordService
    {
        private readonly IServiceProvider _serviceProvider;
        public readonly DiscordSocketClient _socketClient;
        private readonly CommandService _commandService;
        private readonly ILogger _logger;
		private string token;
		Random _rand = new Random((int)DateTime.Now.Ticks);
		Regex isMentioningMeRegex = new Regex(@"(Co?l?o?n?e?l?)(\.?|\s)*(Pa?o?n?i?c?)?");
		ulong botUserId = 357910708316274688;

		public DiscordService(IServiceProvider serviceProvider, DiscordSocketClient socketClient,
							  CommandService commandService)
		{
			_serviceProvider = serviceProvider;
			_socketClient = socketClient;
			_commandService = commandService;

			_socketClient.Log += WriteLog;
			_socketClient.Ready += OnReadyAsync;
			_socketClient.MessageReceived += OnMessageReceivedAsync;
			_socketClient.UserLeft += UserLeft;

			token = String.Empty;


			try
			{
				token = ConfigurationHandler.GetToken();
			}
			catch (Exception ex)
			{
                throw new ApplicationException("Unable to retrieve Token from Database. Exiting.");
			}
		}
        private async Task UserLeft(SocketGuildUser user)
        {
            PingGroupHandler.PurgeUser(user.Id.ToString(), user.Guild.Id.ToString());
        }

		private async Task WriteLog(LogMessage message)
		{
			Console.WriteLine($"{message.Source}: {message.Message} ");
			return;
		}

        public async Task InitializeAsync()
        {
			_socketClient.MessageReceived += HandleCommand;
			await _commandService.AddModulesAsync(Assembly.LoadFrom("ColonelPanic.Modules.dll"), _serviceProvider);
			await _commandService.AddModulesAsync(Assembly.LoadFrom("DDB.dll"), _serviceProvider);

			await _socketClient.LoginAsync(TokenType.Bot, token);
            await _socketClient.StartAsync();
        }

        private async Task OnReadyAsync()
        {
            await _serviceProvider.UseLavaNodeAsync();
        }

		private async Task OnMessageReceivedAsync(SocketMessage arg)
		{
			bool MentioningMe = BotUtilities.isMentioningMe(arg, isMentioningMeRegex, botUserId);
			string chnlId = arg.Channel.Id.ToString();
			string userId = arg.Author.Id.ToString();
			SocketGuildChannel chnl = arg.Channel as SocketGuildChannel;
			await AddGuildStateIfMissing(chnl.Guild.Id.ToString(), chnl.Guild.Name);
			UserDataHandler.AddUserStateIfMising(arg.Author.Id.ToString(), arg.Author.Username);
			Console.WriteLine($"{arg.Author.Username} on {arg.Channel.Name}: {arg.Content}");
			if (_rand.Next(1000) == 777)
			{
				var msg = arg.Channel.GetMessageAsync(arg.Id).Result as IUserMessage;
				await msg.AddReactionAsync(new Emoji("💩"));
			}
			if (_rand.Next(1000) == 777)
			{
				var msg = arg.Channel.GetMessageAsync(arg.Id).Result as IUserMessage;
				await msg.AddReactionAsync(new Emoji("👍"));
			}
			if (_rand.Next(1000) == 777)
			{
				var msg = arg.Channel.GetMessageAsync(arg.Id).Result as IUserMessage;
				//if Geo
				if (arg.Author.Id == 140620251976040449)
				{
					await msg.AddReactionAsync(new Emoji("🍠"));
				}
				//Everyone else
				else
				{
					await msg.AddReactionAsync(new Emoji("🍆"));
				}

			}
			if (Regex.IsMatch(arg.Content, @"[)ʔ）][╯ノ┛].+┻━┻"))
			{
				await arg.Channel.SendMessageAsync("┬─┬  ノ( º _ ºノ) ");
				await arg.Channel.SendMessageAsync(GetTableFlipResponse(arg.Author));
			}
			else if (arg.Content == "(ノಠ益ಠ)ノ彡┻━┻")
			{
				await arg.Channel.SendMessageAsync("┬─┬  ノ(ಠ益ಠノ)");
				await arg.Channel.SendMessageAsync(GetTableFlipResponse(arg.Author));
			}
			else if (arg.Content == "┻━┻ ︵ヽ(`Д´)ﾉ︵ ┻━┻")
			{
				await arg.Channel.SendMessageAsync("┬─┬  ノ(`Д´ノ)");
				await arg.Channel.SendMessageAsync("(/¯`Д´ )/¯ ┬─┬");
				await arg.Channel.SendMessageAsync(GetTableFlipResponse(arg.Author));
			}
			if (MentioningMe)
			{
				if (arg.Content.Contains("🤛"))
				{
					await arg.Channel.SendMessageAsync(":right_facing_fist:");

				}
				else if (arg.Content.Contains("🤜"))
				{
					await arg.Channel.SendMessageAsync(":left_facing_fist:");
				}
			}
			if (arg.Content.ToLower().Contains("pogger") && !arg.Content.Contains(" "))
			{
				await arg.AddReactionAsync(Emote.Parse(CEC.PoggersEmoteCode));		
			}

			return;
		}
		private async Task AddGuildStateIfMissing(string guildId, string name)
		{
			if (!ConfigurationHandler.GuildStateExists(guildId))
			{
				await ConfigurationHandler.AddGuildState(guildId, name);
			}
		}

		public async Task HandleCommand(SocketMessage messageParam)
		{
			char prefix = '$';
			var message = messageParam as SocketUserMessage;
			if (message == null) return;
			int argPos = 0;
			if (!message.HasCharPrefix(prefix, ref argPos) && !false) return;
			var context = new CommandContext(_socketClient, message);
			var result = await _commandService.ExecuteAsync(context, argPos, _serviceProvider);
			if (!result.IsSuccess)
			{
				CommandInfo commandFromModuleGroup = _commandService.Commands.FirstOrDefault(c => $"{prefix}{c.Module.Group}" == message.Content.ToLower());
				CommandInfo commandFromNameWithGroup = _commandService.Commands.FirstOrDefault(c => $"{prefix}{c.Module.Group} {c.Name}" == message.Content.ToLower());
				CommandInfo commandFromName = _commandService.Commands.FirstOrDefault(c => $"{prefix}{c.Name}" == message.Content.ToLower());
				if (commandFromModuleGroup != null)
				{
					await context.Channel.SendMessageAsync(BotUtilities.BuildModuleInfo(prefix, commandFromModuleGroup.Module));
				}
				if (commandFromNameWithGroup != null || commandFromName != null)
				{
					await context.Channel.SendMessageAsync(BotUtilities.BuildDetailedCommandInfo(prefix, (commandFromName ?? commandFromNameWithGroup)));
				}
				if (commandFromModuleGroup == null && commandFromName == null && commandFromNameWithGroup == null)
				{
					await context.Message.AddReactionAsync(new Emoji("😕"));
				}

			}
		}

		private string GetTableFlipResponse(SocketUser author)
		{
			int points = UserDataHandler.IncrementTableFlipPoints(author.Id.ToString(), author.Username);
			if (points >= 81) return String.Format(ResponseCollections.TableFlipResponses[4].GetRandom(), author.Username);
			if (points >= 61) return String.Format(ResponseCollections.TableFlipResponses[3].GetRandom(), author.Username);
			if (points >= 41) return String.Format(ResponseCollections.TableFlipResponses[2].GetRandom(), author.Username);
			if (points >= 21) return String.Format(ResponseCollections.TableFlipResponses[1].GetRandom(), author.Username);
			return String.Format(ResponseCollections.TableFlipResponses[0].GetRandom(), author.Username);
		}
	}
}
