using ColonelPanic.Database.Contexts;
using ColonelPanic.Database.Models;
using ColonelPanic.Modules;
using ColonelPanic.Utilities.JSONClasses;
using DartsDiscordBots.Utilities;
using DartsDiscordBots.Interfaces;
using DartsDiscordBots.Modules;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using System.Threading;
using System.Text.RegularExpressions;
using System.Net;
using System.Linq;
using ColonelPanic.Services;

namespace ColonelPanic
{
	class Program
	{
		DiscordSocketClient client;
		CommandService commands;
		IServiceProvider services;
		Random _rand = new Random();
		SocketUser Me { get; set; }
		public int counter = 1;
		static void Main(string[] args) => new Program().Start().GetAwaiter().GetResult();

		Timer TopDailyTimer { get; set; }
		string isMentioningMeRegex = @"(Co?l?o?n?e?l?)(\.?|\s)*(Pa?o?n?i?c?)?";




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

#if DEBUG
			TopDailyTimer = new Timer(TopDailyCallback, null, 1000, 1000 * 60);
#else
            TopDailyTimer = new Timer(TopDailyCallback, null, 1000 * 60, 1000 * 60);                        
#endif


			await Task.Delay(-1);
		}

		private void TopDailyCallback(object state)
		{
			List<TopDaily> topDailiesToExecute = new List<TopDaily>();
			try
			{
				topDailiesToExecute = RedditHandler.GetSubredditsToCheck();
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				while (ex.InnerException != null)
				{
					Console.WriteLine(ex.InnerException.Message);
					ex = ex.InnerException;
				}
				Console.WriteLine("Looks like there was a hiccup saving a date change maybe?");
			}
			if (topDailiesToExecute.Count > 0)
			{
				List<String> topDailyLinks = new List<string>();

				foreach (TopDaily td in topDailiesToExecute)
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
			List<SocketUser> MentionedUsers = new List<SocketUser>();
			foreach (SocketUser user in arg.MentionedUsers)
			{
				MentionedUsers.Add(user);
			}
			bool MentioningMe = isMentioningMe(arg, MentionedUsers);
			string chnlId = arg.Channel.Id.ToString();
			string userId = arg.Author.Id.ToString();
			SocketGuildChannel chnl = arg.Channel as SocketGuildChannel;
			await AddGuildStateIfMissing(chnl.Guild.Id.ToString(), chnl.Guild.Name);
			if (!UserDataHandler.UserStateExists(arg.Author.Id.ToString()))
			{
				UserDataHandler.AddUserState(arg.Author.Id.ToString(), arg.Author.Username);
			}
			if (arg.Content.ToLower() == "*unzips*" || arg.Content.ToLower() == "*unfastens belt*")
			{
				Random rand = new Random();
				if (rand.Next(0, 100) % 2 == 0)
				{
					await arg.Channel.SendMessageAsync("🍆");
				}
				else
				{
					await arg.Channel.SendMessageAsync("🍄");
				}
			}
			if (arg.Content == "image")
			{
				await client.CurrentUser.ModifyAsync(x =>
				{
					x.Avatar = new Discord.Image("ColPan3.gif");
				});
			}
			Console.WriteLine($"{arg.Author.Username} on {arg.Channel.Name}: {arg.Content}");

			if (UserDataHandler.IsEggplantUser(chnlId, userId))
			{
				var msg = arg.Channel.GetMessageAsync(arg.Id).Result as IUserMessage;
				await msg.AddReactionAsync(new Emoji("🍆"));
			}
			if (UserDataHandler.IsShitlistUser(chnlId, userId))
			{
				var msg = arg.Channel.GetMessageAsync(arg.Id).Result as IUserMessage;
				await msg.AddReactionAsync(new Emoji("💩"));
			}
			if (arg.Content.ToLower().Contains("communism"))
			{
				var msg = arg.Channel.GetMessageAsync(arg.Id).Result as IUserMessage;
				await msg.AddReactionAsync(new Emoji("💩"));
			}
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

		private bool isMentioningMe(SocketMessage message, List<SocketUser> mentionedUsers)
		{
			Regex regex = new Regex(isMentioningMeRegex);
			return regex.IsMatch(message.Content) || mentionedUsers.FirstOrDefault(u => u.Id == 357910708316274688) != null;
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
		}

		public void InstallServices()
		{
			services = new ServiceCollection()
				.AddSingleton<ITidalService, TidalService>()
				.AddSingleton<IHelpConfig, HelpConfig>()
				.AddSingleton<IAudioService, AudioService>()
				.AddSingleton<IPokemonService, PokemonService>()
				.AddSingleton<ReliabilityService>()
				.BuildServiceProvider();
		}

		public async Task HandleCommand(SocketMessage messageParam)
		{
			// Don't process the command if it was a System Message
			var message = messageParam as SocketUserMessage;
			if (message == null) return;
			// Create a number to track where the prefix ends and the command begins
			int argPos = 0;
			// Determine if the message is a command, based on if it starts with '$' or a mention prefix
			if (!message.HasCharPrefix('$', ref argPos) && !false) return;
			// Create a Command Context
			var context = new CommandContext(client, message);
			bool commandExists = commands.Commands.FirstOrDefault(c => c.Name == message.Content.Replace("$",string.Empty).Split()[0]) != null;
			// Execute the command. (result does not indicate a return value, 
			// rather an object stating if the command executed successfully)
			var result = await commands.ExecuteAsync(context, argPos, services);
			if (!result.IsSuccess && commandExists)
				await context.Channel.SendMessageAsync(result.ErrorReason);
			else if (!commandExists)
			{
				await context.Message.AddReactionAsync(new Emoji("😕"));
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
