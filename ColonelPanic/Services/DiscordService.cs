using ColonelPanic.Constants;
using ColonelPanic.Database.Contexts;
using ColonelPanic.Handlers;
using DartsDiscordBots.Utilities;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Practices.EnterpriseLibrary.Common.Utility;
using System;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Victoria;
using EU = DartsDiscordBots.Utilities.EventUtilities;


namespace ColonelPanic.Services
{
    public sealed class DiscordService
    {
        private readonly IServiceProvider _serviceProvider;
        public readonly DiscordSocketClient _socketClient;
        private readonly CommandService _commandService;
        private readonly ILogger _logger;
        private string token;

        public DiscordService(IServiceProvider serviceProvider, DiscordSocketClient socketClient,
                              CommandService commandService)
        {
            _serviceProvider = serviceProvider;
            _socketClient = socketClient;
            _commandService = commandService;

            _socketClient.Log += WriteLog;
            _socketClient.Ready += OnReadyAsync;
            _socketClient.MessageReceived += OnMessageReceivedAsync;
            _socketClient.MessageReceived += HandleCommand;
            _socketClient.GuildScheduledEventCreated += OnEventCreated;
            _socketClient.GuildScheduledEventStarted += OnEventStarted;

            token = GetBotToken();

            
        }
        public async void EventReminderCheck()
        {
            DateTime fiftyNineMinutesFromNow = DateTime.Now.AddMinutes(59);
            DateTime sixtyOneMinutesFromNow = DateTime.Now.AddMinutes(61);
            Console.WriteLine($"[ColonelPanic] - Checking if there are any events firing between {fiftyNineMinutesFromNow.ToString("r")} and {sixtyOneMinutesFromNow.ToString("r")}");
            foreach (SocketGuild guild in _socketClient.Guilds)
            {
                ITextChannel announcementChnl = (ITextChannel)guild.Channels.FirstOrDefault(c => c.Name.ToLower() == "announcements");
                if (announcementChnl != null)
                {
                    new Thread(() =>
                    {
                        CheckGuildForEventsRequiringReminders(fiftyNineMinutesFromNow, sixtyOneMinutesFromNow, announcementChnl, guild);
                    }).Start();                    
                }
            }

        }

        private async void CheckGuildForEventsRequiringReminders(DateTime lowerTimeLimit, DateTime upperTImeLimit, ITextChannel announcementChannel, SocketGuild guild)
        {
            Console.WriteLine("[ColonelPanic] - Announcement channel not null! Checking events.");
            foreach (IGuildScheduledEvent guildEvent in guild.GetEventsAsync(RequestOptions.Default).Result)
            {
                Console.WriteLine($"[ColonelPanic] - Checking event '{guildEvent.Name}' which starts at {guildEvent.StartTime.ToLocalTime().ToString("r")}");
                if (guildEvent.StartTime >= lowerTimeLimit && guildEvent.StartTime <= upperTImeLimit)
                {
                    Console.WriteLine($"[ColonelPanic] - Got a hit! Alerting the media.");
                    string mentions = await EU.GetInterestedUsersMentions(guildEvent);
                    _ = announcementChannel.SendMessageAsync($"{mentions} {guildEvent.Name} is starting soon! See you all in <t:{guildEvent.StartTime.ToUniversalTime().ToUnixTimeSeconds()}:R>");
                }
            }
        }
        public async Task InitializeAsync()
        {
            await _commandService.AddModulesAsync(Assembly.LoadFrom("ColonelPanic.Modules.dll"), _serviceProvider);
            await _commandService.AddModulesAsync(Assembly.LoadFrom("DDB.dll"), _serviceProvider);

            await _socketClient.LoginAsync(TokenType.Bot, token);
            await _socketClient.StartAsync();

            new Thread(() => JackboxUtilities.EnsureDefaultGamesExist(_serviceProvider.GetService<JackboxContext>())).Start();

        }
        private string GetBotToken()
        {
            string token;
            token = Environment.GetEnvironmentVariable("COLONELPANIC");

            Console.WriteLine("Attempting to retrieve bot token from Environment...");
            if (string.IsNullOrEmpty(token))
            {
                try
                {
                    Console.WriteLine("Failed. Attempting to retrieve from local DB...");
                    token = ConfigurationHandler.GetToken();
                    Console.WriteLine("Success!");
                }
                catch (Exception ex)
                {
                    throw new ApplicationException($"Unable to retrieve Token from Database. Exiting.{Environment.NewLine}{Environment.NewLine}{ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("Success!");
            }
            return token;
        }
        private async Task OnEventCreated(SocketGuildEvent arg)
        {
            new Task(() =>
            {
                EU.AnnounceNewEvent(arg);
            }).Start();
        }
        private async Task OnEventStarted(SocketGuildEvent arg)
        {
            new Task(() =>
            {
                EU.AnnounceNewEventStarted(arg);
            }).Start();
        }
        private async Task WriteLog(LogMessage message)
        {
            Console.WriteLine($"{message.Source}: {message.Message} ");
            return;
        }
        private async Task OnReadyAsync()
        {
            await _serviceProvider.UseLavaNodeAsync();
        }
        private async Task OnMessageReceivedAsync(SocketMessage arg)
        {
            try
            {
                bool mentioningMe = BotUtilities.isMentioningMe(arg, BotInformation.isMentioningMeRegex, BotInformation.botUserId);
                string chnlId = arg.Channel.Id.ToString();
                string userId = arg.Author.Id.ToString();
                string message = arg.Content;

                SocketGuildChannel chnl = arg.Channel as SocketGuildChannel;

                Console.WriteLine($"{arg.Author.Username} on {arg.Channel.Name}: {message}");

                new Thread(() => { _ = OnMessage.AutoEmojiReactCheck(arg); }).Start();
                new Thread(() => { _ = OnMessage.TableFlipCheck(arg);}).Start();
                new Thread(() => { _ = OnMessage.BroFistCheck(arg, mentioningMe);}).Start();
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
            }


            return;
        }
        private async Task HandleCommand(SocketMessage messageParam)
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
                await HandleCommandError(prefix, message);
            }
        }
        private async Task HandleCommandError(char prefix, SocketUserMessage message)
        {
            CommandInfo commandFromModuleGroup = _commandService.Commands.FirstOrDefault(c => $"{prefix}{c.Module.Group}" == message.Content.ToLower());
            CommandInfo commandFromNameWithGroup = _commandService.Commands.FirstOrDefault(c => $"{prefix}{c.Module.Group} {c.Name}" == message.Content.ToLower());
            CommandInfo commandFromName = _commandService.Commands.FirstOrDefault(c => $"{prefix}{c.Name}" == message.Content.ToLower());
            if (commandFromModuleGroup != null)
            {
                await message.ReplyAsync(HelpUtilities.BuildModuleInfo(prefix, commandFromModuleGroup.Module));
            }
            if (commandFromNameWithGroup != null || commandFromName != null)
            {
                await message.ReplyAsync(HelpUtilities.BuildDetailedCommandInfo(prefix, (commandFromName ?? commandFromNameWithGroup)));
            }
            if (commandFromModuleGroup == null && commandFromName == null && commandFromNameWithGroup == null)
            {
                await message.AddReactionAsync(new Emoji("😕"));
            }
        }


    }
}
