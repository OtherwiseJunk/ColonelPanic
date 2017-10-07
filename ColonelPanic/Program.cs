using ColonelPanic.Database.Contexts;
using ColonelPanic.Database.Models;
using ColonelPanic.Modules;
using Discord;

using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using System.Threading;

namespace ColonelPanic
{
    class Program
    {
        DiscordSocketClient client;
        CommandService commands;
        IServiceProvider services;
        static void Main(string[] args) => new Program().Start().GetAwaiter().GetResult();

        Timer ScrumUpdateTimer { get; set; }


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

            services = new ServiceCollection().BuildServiceProvider();

            await InstallCommands();

            client.Log += WriteLog;
            client.MessageReceived += MessageReceived;

            string token = String.Empty;


            try
            {
                token = ConfigurationHandler.GetToken();
            }
            catch (Exception ex)
            {

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

            ScrumUpdateTimer = new System.Threading.Timer(ScrumCheckCallback, null, 1000 * 60, 1000 * 60 * 60);

            await Task.Delay(-1);
        }

        private async Task AddChannelStateIfMissing(string chnlId, string name)
        {
            if (!ConfigurationHandler.ChannelStateExists(chnlId))
            {
                await ConfigurationHandler.AddChannelState(chnlId, name);
            }
        }

        public void ScrumCheckCallback(object state)
        {
            List<ScrumUser> usersToHarass = ScrumHandler.GetUsersToHarass();
            if (usersToHarass.Count > 0)
            {
                foreach (ScrumUser user in usersToHarass)
                {
                    var usr = client.GetUser(ulong.Parse(user.UserId));
                    var chnl = client.GetChannel(ulong.Parse(user.UserChannelId)) as SocketTextChannel;
                    chnl.SendMessageAsync(usr.Mention + "! You haven't given me an update for this week!");
                }
            }
        }

        private async Task MessageReceived(SocketMessage arg)
        {
            await AddChannelStateIfMissing(arg.Channel.Id.ToString(), arg.Channel.Name);
            Console.WriteLine($"{arg.Author.Username} on {arg.Channel.Name}: {arg.Content}");
            return;
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
            await commands.AddModulesAsync(Assembly.GetEntryAssembly());
            await commands.AddModuleAsync<ServerModule>();
            await commands.AddModuleAsync<HelpModule>();
            await commands.AddModuleAsync<ConfigurationModule>();
            await commands.AddModuleAsync<ScrumModule>();
        }

        public async Task HandleCommand(SocketMessage messageParam)
        {
            // Don't process the command if it was a System Message
            var message = messageParam as SocketUserMessage;
            if (message == null) return;
            // Create a number to track where the prefix ends and the command begins
            int argPos = 0;
            // Determine if the message is a command, based on if it starts with '!' or a mention prefix
            if (!(message.HasCharPrefix('$', ref argPos) || message.HasMentionPrefix(client.CurrentUser, ref argPos))) return;
            // Create a Command Context
            var context = new CommandContext(client, message);
            // Execute the command. (result does not indicate a return value, 
            // rather an object stating if the command executed successfully)
            var result = await commands.ExecuteAsync(context, argPos, services);
            if (!result.IsSuccess)
                await context.Channel.SendMessageAsync(result.ErrorReason);
        }
    }
}
