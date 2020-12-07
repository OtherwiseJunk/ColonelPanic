using ColonelPanic.Database.Contexts;
using ColonelPanic.Database.Models;
using ColonelPanic.Modules;
using ColonelPanic.Utilities.JSONClasses;
using DartsDiscordBots.Utilities;
using DartsDiscordBots.Interfaces;
using DartsDiscordBots.Services;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using FluentScheduler;
using System.Threading.Tasks;
using System.Net;
using ColonelPanic.Services;
using Victoria;


namespace ColonelPanic
{
	class Program
	{
		IServiceProvider services;
		
		Registry scheduledTaskRegistry = new Registry();		
		static void Main(string[] args) => new Program().Start().GetAwaiter().GetResult();

		private async Task Start()
		{
			InstallServices();

			scheduledTaskRegistry.Schedule(() => AnimalCrossingService.Cleanup()).ToRunEvery(1).Days().At(00, 00);
			ScheduleTopDaily();

			JobManager.Initialize(scheduledTaskRegistry);
			await InitializeDiscordAsync();

			await Task.Delay(-1);
		}

		private async Task InitializeDiscordAsync()
		{
			var discordService = services.GetRequiredService<DiscordService>();
			await discordService.InitializeAsync();
		}

		private void ScheduleTopDaily()
		{
			List<TopDaily> topDailiesToExecute = RedditHandler.GetAllTopDailies();			
			if (topDailiesToExecute.Count > 0)
			{
				foreach(TopDaily td in topDailiesToExecute)
				{
					scheduledTaskRegistry.Schedule(() => HandleTopDaily(td)).ToRunEvery(1).Days().At(td.NextTimeToPost.Hour, td.NextTimeToPost.Minute);
				}
			}

		}

		private void HandleTopDaily(TopDaily td)
		{
			var chnl = services.GetRequiredService<DiscordService>()._socketClient.GetChannel(ulong.Parse(td.ChannelId)) as SocketTextChannel;
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

		public void InstallServices()
		{
			services = new ServiceCollection()
				.AddLavaNode(node =>
				{
					node.Port = 2333;
					node.Authorization = "TheFuckOuttaHereM4ng!";
				})
				.AddLogging()
				.AddSingleton<IHelpConfig, HelpConfig>()
				.AddSingleton<IPokemonService, PokemonService>()
				.AddSingleton<IAnimalCrossingService, AnimalCrossingService>()
				.AddSingleton<ReliabilityService>()
				.AddDbContext<AnimalCrossingContext>()
				.AddSingleton<DiscordService>()
				.AddSingleton<CommandService>()
				.AddSingleton<DiscordSocketClient>()
				.AddSingleton<AudioService>()
				.BuildServiceProvider();
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
