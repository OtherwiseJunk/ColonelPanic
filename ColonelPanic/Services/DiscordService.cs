using ColonelPanic.Database.Contexts;
using ColonelPanic.Handlers;
using DartsDiscordBots.Utilities;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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
            _socketClient.MessageReceived += HandleCommand;
            _socketClient.GuildScheduledEventCreated += OnEventCreated;
            _socketClient.GuildScheduledEventStarted += OnEventStarted;


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
                    Console.WriteLine("[ColonelPanic] - Announcement channel not null! Checking events.");
                    foreach (IGuildScheduledEvent guildEvent in guild.GetEventsAsync(RequestOptions.Default).Result)
                    {
                        Console.WriteLine($"[ColonelPanic] - Checking event '{guildEvent.Name}' which starts at {guildEvent.StartTime.ToLocalTime().ToString("r")}");
                        if (guildEvent.StartTime >= fiftyNineMinutesFromNow && guildEvent.StartTime <= sixtyOneMinutesFromNow)
                        {
                            Console.WriteLine($"[ColonelPanic] - Got a hit! Alerting the media.");
                            string mentions = await EU.GetInterestedUsersMentions(guildEvent);
                            _ = announcementChnl.SendMessageAsync($"{mentions} {guildEvent.Name} is starting soon! See you all in <t:{guildEvent.StartTime.ToUniversalTime().ToUnixTimeSeconds()}:R>");
                        }
                    }
                }
            }

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

        public async Task InitializeAsync()
        {
            await _commandService.AddModulesAsync(Assembly.LoadFrom("ColonelPanic.Modules.dll"), _serviceProvider);
            await _commandService.AddModulesAsync(Assembly.LoadFrom("DDB.dll"), _serviceProvider);

            await _socketClient.LoginAsync(TokenType.Bot, token);
            await _socketClient.StartAsync();

            new Thread(() => JackboxUtilities.EnsureDefaultGamesExist(_serviceProvider.GetService<JackboxContext>())).Start();

        }

        private async Task OnReadyAsync()
        {
            await _serviceProvider.UseLavaNodeAsync();
        }

        private async Task OnMessageReceivedAsync(SocketMessage arg)
        {
            try
            {
                bool mentioningMe = BotUtilities.isMentioningMe(arg, isMentioningMeRegex, botUserId);
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
                    await context.Channel.SendMessageAsync(HelpUtilities.BuildModuleInfo(prefix, commandFromModuleGroup.Module));
                }
                if (commandFromNameWithGroup != null || commandFromName != null)
                {
                    await context.Channel.SendMessageAsync(HelpUtilities.BuildDetailedCommandInfo(prefix, (commandFromName ?? commandFromNameWithGroup)));
                }
                if (commandFromModuleGroup == null && commandFromName == null && commandFromNameWithGroup == null)
                {
                    await context.Message.AddReactionAsync(new Emoji("😕"));
                }

            }
        }


    }
}
