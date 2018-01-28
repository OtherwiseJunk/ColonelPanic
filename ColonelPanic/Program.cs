using ColonelPanic.Database.Contexts;
using ColonelPanic.Database.Models;
using ColonelPanic.Modules;
using ColonelPanic.Utilities;
using ColonelPanic.Utilities.JSONClasses;
using DartsDiscordBots.Modules;
using DartsDiscordBots.Utilities;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using System.Threading;
using System.Text;
using System.IO;
using TextMarkovChains;
using System.Xml;
using System.Text.RegularExpressions;
using System.Net;
using System.Linq;

namespace ColonelPanic
{
    class Program
    {        
        string MarkovChainFileName = "MarkovChain.xml";
        DiscordSocketClient client;
        CommandService commands;
        IServiceProvider services;
        Random _rand = new Random();
        SocketUser Me { get; set; } 
        
        static void Main(string[] args) => new Program().Start().GetAwaiter().GetResult();

        public MultiDeepMarkovChain MarkovChainRepository { get; private set; }

        Timer ScrumUpdateTimer { get; set; }
        Timer TopDailyTimer { get; set; }
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

            services = new ServiceCollection().AddSingleton<AudioService>().BuildServiceProvider();
            

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
            TopDailyTimer = new Timer(TopDailyCallback, null, 1000 * 60, 1000 * 60);
            MarkovSaveTimer = new Timer(MarkovSaveTimerCallback, null, 1000 * 60, 1000 * 60 * 15);

            

            await Task.Delay(-1);
        }

        private void TopDailyCallback(object state)
        {
            List<TopDaily> topDailiesToExecute = RedditHandler.GetSubredditsToCheck();
            if (topDailiesToExecute.Count > 0)
            {
                foreach (TopDaily td in topDailiesToExecute)
                {
                    var chnl = client.GetChannel(ulong.Parse(td.ChannelId)) as SocketTextChannel;
                    chnl.SendMessageAsync(GetTopDailylink(td));
                }
            }
            
        }

        private string GetTopDailylink(TopDaily td)
        {
            using(WebClient wClient = new WebClient())
            {
                string url = String.Format(Utilities.APILinkFormats.SubredditTopTwentyPosts, td.Subreddit);
                RedditTopTwenty topTwenty = Newtonsoft.Json.JsonConvert.DeserializeObject<RedditTopTwenty>(wClient.DownloadString(url));
                foreach (var child in topTwenty.data.children)
                {
                    if(child.data.url.Contains(".gif") || child.data.url.Contains(".jpg") || child.data.url.Contains(".png") || child.data.url.Contains(".mp4") || child.data.url.Contains(".gifv"))
                    {
                        return child.data.url;
                    }
                }
            }
            
            return "No image posts in the top 20, sorry";
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
            List<SocketUser> MentionedUsers = new List<SocketUser>();
            foreach (SocketUser user in arg.MentionedUsers)
            {
                MentionedUsers.Add(user);
            }
            bool MentioningMe = MentionedUsers.FirstOrDefault(u => u.Id == 357910708316274688) != null;
            string chnlId = arg.Channel.Id.ToString();
            string userId = arg.Author.Id.ToString();
            SocketGuildChannel chnl = arg.Channel as SocketGuildChannel;
            await AddGuildStateIfMissing(chnl.Guild.Id.ToString(), chnl.Guild.Name);
            if (!UserDataHandler.UserStateExists(arg.Author.Id.ToString()))
            {
                UserDataHandler.AddUserState(arg.Author.Id.ToString(), arg.Author.Username);
            }
            if(arg.Content == "image")
            {
                await client.CurrentUser.ModifyAsync(x => {
                    x.Avatar = new Discord.Image("ColPan.jpg");
                });
            }
            Console.WriteLine($"{arg.Author.Username} on {arg.Channel.Name}: {arg.Content}");
            if (!arg.Author.IsBot && !arg.Content.Contains("$") && !arg.Content.Contains("http"))
            {
                string msg = arg.Content.Replace("💩", ":poop:");
                MarkovChainRepository.feed(arg.Content);
            }
            if (arg.Author.Id == 94545463906144256 && arg.Content.Length % 32 == 0)
            {
                //await arg.Channel.SendMessageAsync(GetMarkovSentence());
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
            else if (arg.Content.Contains("🤛") && MentioningMe)
            {
                await arg.Channel.SendMessageAsync(":right_facing_fist:");

            }
            else if (arg.Content.Contains("🤜") && MentioningMe)
            {
                await arg.Channel.SendMessageAsync(":left_facing_fist:");
            }
            if (arg.Content.ToLower().Contains("```haskell"))
            {
                //removes the 10 characters for ```haskell as well as the newline
                string snippet = arg.Content.Remove(0, 11);
                //removes the final three ``` as well as the new line.
                snippet = snippet.Remove(snippet.Length - 4, 4);

                AudioService.SendUdpStatic(9999, "138.197.42.213", 9999, Encoding.ASCII.GetBytes(snippet));
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
            await commands.AddModuleAsync<AudioModule>();
            await commands.AddModuleAsync<RedditModule>();
            await commands.AddModuleAsync<FWTOWModule>();
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
            // Execute the command. (result does not indicate a return value, 
            // rather an object stating if the command executed successfully)
            var result = await commands.ExecuteAsync(context, argPos, services);
            if (!result.IsSuccess)
                await context.Channel.SendMessageAsync(result.ErrorReason);
        }
    }
}
