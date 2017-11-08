using ColonelPanic.Database.Contexts;
using ColonelPanic.Database.Models;
using ColonelPanic.Modules;
using Discord;
using Quobject.SocketIoClientDotNet.Client;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using System.Threading;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using System.Text;
using Org.BouncyCastle.Crypto;
using System.IO;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Encodings;
using Org.BouncyCastle.OpenSsl;
using TextMarkovChains;

using System.Xml;

namespace ColonelPanic
{
    class Program
    {        
        string MarkovChainFileName = "MarkovChain.xml";
        DiscordSocketClient client;
        CommandService commands;
        IServiceProvider services;
        
        static void Main(string[] args) => new Program().Start().GetAwaiter().GetResult();

        public MultiDeepMarkovChain MarkovChainRepository { get; private set; }

        Timer ScrumUpdateTimer { get; set; }
        Timer MarkovSaveTimer { get; set; }
        

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

            if (File.Exists(MarkovChainFileName))
            {
                var XML = new XmlDocument();
                XML.Load(MarkovChainFileName);
                MarkovChainRepository = new MultiDeepMarkovChain(4);
                MarkovChainRepository.feed(XML);
            }
            else
            {
                MarkovChainRepository = new MultiDeepMarkovChain(4);
                MarkovChainRepository.save(MarkovChainFileName);
            }

            commands = new CommandService();

            services = new ServiceCollection().BuildServiceProvider();

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
            MarkovSaveTimer = new Timer(MarkovSaveTimerCallback, null, 1000 * 60, 1000 * 60 * 15);

            await Task.Delay(-1);
        }        

        private async Task AddGuildStateIfMissing(string guildId, string name)
        {
            if (!ConfigurationHandler.GuildStateExists(guildId))
            {
                await ConfigurationHandler.AddGuildState(guildId, name);
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

        public void MarkovSaveTimerCallback(object state)
        {
            MarkovChainRepository.save(MarkovChainFileName);
        }

        private async Task MessageReceived(SocketMessage arg)
        {
            string chnlId = arg.Channel.Id.ToString();
            string userId = arg.Author.Id.ToString();
            SocketGuildChannel chnl = arg.Channel as SocketGuildChannel;
            await AddGuildStateIfMissing(chnl.Guild.Id.ToString(), chnl.Guild.Name);
            Console.WriteLine($"{arg.Author.Username} on {arg.Channel.Name}: {arg.Content}");
            if (!arg.Author.IsBot && !arg.Content.Contains("$") && !arg.Content.Contains("http"))
            {
                string msg = arg.Content.Replace("💩", ":poop:");
                MarkovChainRepository.feed(arg.Content);
            }
            if (arg.Author.Id == 94545463906144256 && arg.Content.Length % 8 == 0)
            {
                await arg.Channel.SendMessageAsync(GetMarkovSentence());
            }
            if (UserDataHandler.IsEggplantUser(chnlId,userId))
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
            return;
        }

        private string GetMarkovSentence()
        {
            bool msgNotSet = true;
            string msg = "";
            int rerollAttempts = 0;
            while (msgNotSet)
            {                
                try
                {
                    //Check For French server
                        msg = MarkovChainRepository.generateSentence();
                        msgNotSet = false;                    
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Failed to generate a sentence, trying again...");
                    Console.WriteLine(ex.Message);
                }
                if (rerollAttempts > 10 && msgNotSet)
                {
                    msg = "I'm sorry, it looks like I'm unable to generate a sentence at this time.";
                    msgNotSet = false;
                }
                rerollAttempts++;
            }
            return msg;
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
            await commands.AddModulesAsync(Assembly.GetEntryAssembly());
            await commands.AddModuleAsync<ServerModule>();
            await commands.AddModuleAsync<HelpModule>();
            await commands.AddModuleAsync<ConfigurationModule>();
            await commands.AddModuleAsync<ScrumModule>();
            await commands.AddModuleAsync<GeosusModule>();
            await commands.AddModuleAsync<EnableModule>();
            await commands.AddModuleAsync<DisableModule>();
            await commands.AddModuleAsync<NoteModule>();
            await commands.AddModuleAsync<PingGroupModule>();
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
