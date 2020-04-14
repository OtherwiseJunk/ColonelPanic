using ColonelPanic.Database.Contexts;
using ColonelPanic.Database.Models;
using ColonelPanic.Modules;
using ColonelPanic.Utilities.JSONClasses;
using DartsDiscordBots.Utilities;
using DartsDiscordBots.Interfaces;
using DartsDiscordBots.Modules;
using DartsDiscordBots.Services;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using FluentScheduler;
using System.Threading.Tasks;
using System.Threading;
using System.Text.RegularExpressions;
using System.Net;
using System.Linq;
using ColonelPanic.Services;
using System.Text;

namespace ColonelPanic
{
	class Program
	{
		DiscordSocketClient client;
		CommandService commands;
		IServiceProvider services;
		Random _rand = new Random();
		Registry scheduledTaskRegistry = new Registry();		
		SocketUser Me { get; set; }
		public int counter = 1;
		static void Main(string[] args) => new Program().Start().GetAwaiter().GetResult();

		Timer TopDailyTimer { get; set; }
		Regex isMentioningMeRegex = new Regex(@"(Co?l?o?n?e?l?)(\.?|\s)*(Pa?o?n?i?c?)?");
		ulong botUserId = 357910708316274688;




		private async Task Start()
		{
			LogSeverity logLevel = LogSeverity.Verbose;
#if DEBUG
			logLevel = LogSeverity.Debug;
#endif
			client = new DiscordSocketClient(new DiscordSocketConfig
			{
				LogLevel = logLevel

			});

			commands = new CommandService();

			InstallServices();
			await InstallCommands();

			client.Log += WriteLog;
			client.MessageReceived += MessageReceived;
			client.UserLeft += UserLeft;

			string token = String.Empty;


			try
			{
				token = ConfigurationHandler.GetToken();
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				if (ex.InnerException != null)
				{
					var innerException = ex.InnerException;
					while (innerException != null)
					{
						Console.WriteLine(innerException.Message);
						innerException = innerException.InnerException;
					}
				}
			}

			if (token != String.Empty)
			{
				await client.LoginAsync(TokenType.Bot, token);
				await client.StartAsync();
			}
			else
			{
				token = "";
				while (client.ConnectionState != ConnectionState.Connected)
				{
					Console.WriteLine("Please enter the bot token:");
					token = Console.ReadLine();
					try
					{
						await client.LoginAsync(TokenType.Bot, token);
						await client.StartAsync();
						ConfigurationHandler.CreateConfig(token, "", "");
					}
					catch (Exception ex)
					{
						Console.WriteLine(ex.Message);
						Console.WriteLine("That looks like a bad token maybe? or something else went wrong.");
					}
				}

			}

			scheduledTaskRegistry.Schedule(() => AnimalCrossingService.ClearTurnipSellPrices()).ToRunEvery(1).Days().At(00, 00);
			scheduledTaskRegistry.Schedule(() => AnimalCrossingService.ClearTurnipSellPrices()).ToRunEvery(1).Days().At(12, 00);
			scheduledTaskRegistry.Schedule(() => AnimalCrossingService.ClearTurnipBuyPrices()).ToRunEvery(1).Weeks().On(DayOfWeek.Sunday).At(00, 00);
			ScheduleTopDaily();

			JobManager.Initialize(scheduledTaskRegistry);

			await Task.Delay(-1);
		}

		private void ScheduleTopDaily()
		{
			List<TopDaily> topDailiesToExecute = RedditHandler.GetAllTopDailies();			
			if (topDailiesToExecute.Count > 0)
			{
				foreach(var td in topDailiesToExecute)
				{
					scheduledTaskRegistry.Schedule(() => HandleTopDaily(td)).ToRunEvery(1).Days().At(td.NextTimeToPost.Hour, td.NextTimeToPost.Minute);
				}

				foreach (TopDaily td in topDailiesToExecute)
				{
					
				}
			}

		}

		private void HandleTopDaily(TopDaily td)
		{
			var chnl = client.GetChannel(ulong.Parse(td.ChannelId)) as SocketTextChannel;
			var link = GetTopDailylink(td);
			if (link != null)
			{
				if (link is Embed)
				{
					chnl.SendMessageAsync("", false, link as Embed);
				}
				else if (link is RedditVideoLinkXMetadata)
				{
					RedditVideoLinkXMetadata videoLink = link as RedditVideoLinkXMetadata;
					chnl.SendMessageAsync(videoLink.MetadataMessage);
					chnl.SendMessageAsync(videoLink.URL);
				}
			}
			else
			{
				chnl.SendMessageAsync("Sorry, no images in the top 20 for " + td.Subreddit + ".");
			}
		}

		private Object GetTopDailylink(TopDaily td)
		{
			List<Object> urlsToPickFrom = new List<Object>();
			using (WebClient wClient = new WebClient())
			{
				string url = String.Format(Utilities.APILinkFormats.SubredditTopOneHundredPosts, td.Subreddit);
				RedditTopTwenty topTwenty = Newtonsoft.Json.JsonConvert.DeserializeObject<RedditTopTwenty>(wClient.DownloadString(url));
				foreach (var child in topTwenty.data.children)
				{
					if (child.data.url.Contains(".mp4") || child.data.url.Contains(".gifv"))
					{
						urlsToPickFrom.Add(new RedditVideoLinkXMetadata(child.data.url, $"{child.data.subreddit}'s top image (Title: {child.data.title}):"));
					}
					else if (child.data.url.Contains(".gif") || child.data.url.Contains(".jpg") || child.data.url.Contains(".png"))
					{
						urlsToPickFrom.Add(RedditModule.buildEmbedForImage(child));
					}

				}
				if (urlsToPickFrom.Count != 0)
				{
					return urlsToPickFrom.GetRandom();
				}
			}

			return null;
		}

		private async Task AddGuildStateIfMissing(string guildId, string name)
		{
			if (!ConfigurationHandler.GuildStateExists(guildId))
			{
				await ConfigurationHandler.AddGuildState(guildId, name);
			}
		}

		private async Task MessageReceived(SocketMessage arg)
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

			return;
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

		private async Task UserLeft(SocketGuildUser user)
		{
			PingGroupHandler.PurgeUser(user.Id.ToString(), user.Guild.Id.ToString());
		}

		private async Task WriteLog(LogMessage message)
		{
			Console.WriteLine($"{message.Source}: {message.Message} ");
			return;
		}

		public async Task InstallCommands()
		{
			// Hook the MessageReceived Event into our Command Handler
			client.MessageReceived += HandleCommand;
			// Discover all of the commands in this assembly and load them.
			await commands.AddModuleAsync<ServerModule>(services);
			await commands.AddModuleAsync<HelpModule>(services);
			await commands.AddModuleAsync<ConfigurationModule>(services);
			await commands.AddModuleAsync<GeosusModule>(services);
			await commands.AddModuleAsync<EnableModule>(services);
			await commands.AddModuleAsync<DisableModule>(services);
			await commands.AddModuleAsync<NoteModule>(services);
			await commands.AddModuleAsync<PingGroupModule>(services);
			await commands.AddModuleAsync<AudioModule>(services);
			await commands.AddModuleAsync<RedditModule>(services);
			await commands.AddModuleAsync<MarioMakerModule>(services);
			await commands.AddModuleAsync<PokemonModule>(services);
			await commands.AddModuleAsync<AnimalCrossingModule>(services);
		}

		public void InstallServices()
		{
			services = new ServiceCollection()
				.AddSingleton<ITidalService, TidalService>()
				.AddSingleton<IHelpConfig, HelpConfig>()
				.AddSingleton<IAudioService, AudioService>()
				.AddSingleton<IPokemonService, PokemonService>()
				.AddSingleton<IAnimalCrossingService, AnimalCrossingService>()
				.AddSingleton<ReliabilityService>()
				.AddDbContext<AnimalCrossingContext>()
				.BuildServiceProvider();
		}

		public async Task HandleCommand(SocketMessage messageParam)
		{
			char prefix = '$';
			var message = messageParam as SocketUserMessage;
			if (message == null) return;
			int argPos = 0;
			if (!message.HasCharPrefix(prefix, ref argPos) && !false) return;
			var context = new CommandContext(client, message);
			var result = await commands.ExecuteAsync(context, argPos, services);
			if (!result.IsSuccess)
			{
				CommandInfo commandFromModuleGroup = commands.Commands.FirstOrDefault(c => $"{prefix}{c.Module.Group}" == message.Content.ToLower());
				CommandInfo commandFromNameWithGroup = commands.Commands.FirstOrDefault(c => $"{prefix}{c.Module.Group} {c.Name}" == message.Content.ToLower());
				CommandInfo commandFromName = commands.Commands.FirstOrDefault(c => $"{prefix}{c.Name}" == message.Content.ToLower());
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
		
	}
	
	public class HelpConfig : IHelpConfig
	{
		public string Prefix { get; set; } = "$";
	}
	public class RedditVideoLinkXMetadata
	{
		public string URL;
		public string MetadataMessage;

		public RedditVideoLinkXMetadata(string url, string metadataMessage)
		{
			URL = url;
			MetadataMessage = metadataMessage;
		}
	}
}
