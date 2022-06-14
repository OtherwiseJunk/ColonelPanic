using ColonelPanic.Database.Contexts;
using DartsDiscordBots.Services;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using FluentScheduler;
using System.Threading.Tasks;
using ColonelPanic.Services;
using Victoria;
using DartsDiscordBots.Modules.ServerManagement.Interfaces;
using DartsDiscordBots.Modules.Help.Interfaces;
using DartsDiscordBots.Modules.Pokemon.Interfaces;
using DartsDiscordBots.Modules.AnimalCrossing.Interfaces;
using DartsDiscordBots.Modules.Bot.Interfaces;
using ColonelPanic.Constants;
using DartsDiscordBots.Services.Interfaces;
using DartsDiscordBots.Modules.Jackbox.Interfaces;

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
			Console.WriteLine($"Starting with Connection String of : {Environment.GetEnvironmentVariable("DATABASE")}");

            scheduledTaskRegistry.Schedule(AnimalCrossingService.Cleanup).ToRunEvery(1).Days().At(00, 00);

			await InitializeDiscordAsync();
			JobManager.Initialize(scheduledTaskRegistry);			

			await Task.Delay(-1);
		}

		private async Task InitializeDiscordAsync()
        {
            var discordService = services.GetRequiredService<DiscordService>();
            scheduledTaskRegistry.Schedule(discordService.EventReminderCheck).ToRunEvery(1).Hours().At(0);
			scheduledTaskRegistry.Schedule(discordService.EventReminderCheck).ToRunEvery(1).Hours().At(30);
			await discordService.InitializeAsync();
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
				.AddSingleton<IServerManagmentService, ServerManagementService>()
				.AddSingleton<IMessageReliabilityService, MessageReliabilityService>()
				.AddSingleton<IBotInformation, BotInformation>()
				.AddSingleton<IJackboxService, JackboxService>()
				.AddSingleton<ReliabilityService>()
				.AddDbContext<AnimalCrossingContext>()
				.AddSingleton<DiscordService>()
				.AddSingleton<CommandService>()
				.AddSingleton<DiscordSocketClient>()
				.AddSingleton<AudioService>()
				.AddSingleton(new ImagingService(Environment.GetEnvironmentVariable("DOPUBLIC"),
					Environment.GetEnvironmentVariable("DOSECRET"),
					Environment.GetEnvironmentVariable("DOURL"),
					Environment.GetEnvironmentVariable("DOBUCKET"))
				)
				.AddDbContextFactory<JackboxContext>()
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
