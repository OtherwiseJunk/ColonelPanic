using System.Threading.Tasks;
using Discord.Commands;
using DartsDiscordBots.Utilities;
using ColonelPanic.Utilities;
using ColonelPanic.Database.Contexts;
using ColonelPanic.Utilities.Permissions;
using System.Collections.Generic;
using Discord;
using System;
using System.IO;
using Discord.WebSocket;

namespace ColonelPanic.Modules
{
    public class ServerModule : ModuleBase
    {
        [Command("listchnl"), RequireOwner]
        public async Task listChannels()
        {
            string msg = "I'm in these guilds/channels:" + Environment.NewLine + Environment.NewLine;
            foreach (var guild in Context.Client.GetGuildsAsync().Result)
            {
                msg += "**" + guild.Name + "**" + Environment.NewLine;
                foreach (var chnl in guild.GetChannelsAsync().Result)
                {
                    if (msg.Length > 1900)
                    {
                        await Context.Channel.SendMessageAsync(msg);
                        msg = "";
                    }
                    msg += chnl.Name + Environment.NewLine;
                }
            }
            await Context.Channel.SendMessageAsync(msg);
        }

        [Command("playing"), RequireTrustedUser]
        public async Task setPlaying([Remainder]string playing)
        {
            var client = Context.Client as DiscordSocketClient;
            await client.SetGameAsync(playing);                
        }

        [Command("download"), RequireOwner]
        public async Task downloadChannelData(string chnlName)
        {
            IMessageChannel channel = null;
            IGuild channelsGuild = null;
            foreach (var guild in Context.Client.GetGuildsAsync().Result)
            {
                if (guild.Name != "good music geosus-chan")
                {
                    continue;
                }
                foreach (var chnl in guild.GetChannelsAsync().Result)
                {
                    if(chnl.Name.ToLower() == chnlName)
                    {
                        channel = chnl as IMessageChannel;
                        channelsGuild = guild;
                    }
                }
            }
            if (channel != null)
            {
                string allMessages = "";
                var msgsCollection = channel.GetMessagesAsync(1000).Flatten();
                
                foreach (var msg in msgsCollection.Result)
                {
                    allMessages += msg.Timestamp + "|" + msg.Author.Username + " on " + msg.Channel + ": " + msg.Content + Environment.NewLine;
                }
                File.WriteAllText(channelsGuild.Name + "." + channel.Name, allMessages);
            }
        }



        [Command("say"), Summary("Echos a message."), RequireOwner]
        public async Task Say([Remainder, Summary("The text to echo")] string echo)
        {
            // ReplyAsync is a method on ModuleBase
            await ReplyAsync(echo);
        }

        [Command("link"), Alias("install"), Summary("Provides a link for installing the bot on other servers. You must be an admin of the target server to use the provided link.")]
        public async Task ProvideInstallLink()
        {
            Context.Channel.SendMessageAsync("https://discordapp.com/oauth2/authorize?&client_id=357910708316274688&scope=bot");
        }

        [Command("big"), Summary("`Applies the 'Big if True'`")]
        public async Task BigIfTrue([Remainder, Summary("MsgId")]string msgId)
        {
            if (ulong.TryParse(msgId, out var mId))
            {
                var msg = Context.Channel.GetMessageAsync(mId).Result as IUserMessage;
                var emoji = new Emoji("🇧");
                await msg.AddReactionAsync(emoji);

                emoji = new Emoji("🇮");
                await msg.AddReactionAsync(emoji);

                emoji = new Emoji("🇬");
                await msg.AddReactionAsync(emoji);

                emoji = new Emoji("\U00002139");
                await msg.AddReactionAsync(emoji);

                emoji = new Emoji("🇫");
                await msg.AddReactionAsync(emoji);

                emoji = new Emoji("🇹");
                await msg.AddReactionAsync(emoji);

                emoji = new Emoji("🇷");
                await msg.AddReactionAsync(emoji);

                emoji = new Emoji("🇺");
                await msg.AddReactionAsync(emoji);

                emoji = new Emoji("🇪");
                await msg.AddReactionAsync(emoji);
            }
        }

        [Command("poop"), RequireUserPermission(ChannelPermission.ManageChannel)]
        public async Task poop([Remainder] string userId)
        {
            string chnlId = Context.Channel.Id.ToString();
            if (UserDataHandler.UserHasFlags(chnlId, userId))
            {
                if (!UserDataHandler.IsShitlistUser(chnlId, userId))
                {
                    UserDataHandler.AddShitlistUser(chnlId, userId);
                }
                else if (UserDataHandler.UserHasFlags(chnlId, userId))
                {
                    UserDataHandler.RemoveShitlistUser(chnlId, userId);
                }
            }
            else
            {
                UserDataHandler.AddShitlistUser(chnlId, userId);
            }
            await Context.Channel.SendMessageAsync("K.");
        }

        [Command("eggplant"), RequireUserPermission(ChannelPermission.ManageChannel)]
        public async Task eggplant([Remainder] string userId)
        {
            string chnlId = Context.Channel.Id.ToString();

            if (UserDataHandler.UserHasFlags(chnlId, userId))
            {
                if (!UserDataHandler.IsEggplantUser(chnlId, userId))
                {
                    UserDataHandler.AddEggplantUser(chnlId, userId);
                }
                else if (UserDataHandler.UserHasFlags(chnlId, userId))
                {
                    UserDataHandler.RemoveEggplantUser(chnlId, userId);
                }
            }
            else
            {
                UserDataHandler.AddEggplantUser(chnlId, userId);
            }
            await Context.Channel.SendMessageAsync("K.");
        }

        [Command("roll")]
        public async Task Roll([Remainder] string rollString)
        {

            var arguments = new List<string>(rollString.Split('d'));
            arguments.Remove("");
            int sides, times, modifier;

            if (arguments.Count == 1)
            {
                if (arguments[0].Contains("+"))
                {
                    arguments = new List<string>(arguments[0].Split('+'));
                    if (int.TryParse(arguments[0], out sides))
                    {
                        var dice = new Dice(sides);
                        var temp = dice.Roll();
                        if (int.TryParse(arguments[1], out modifier))
                        {
                            await Context.Channel.SendMessageAsync(
                                    string.Format("Rolled one d{0} plus {1} and got a total of {2}", sides,
                                        modifier, temp + modifier));

                        }
                        else
                        {
                            await Context.Channel.SendMessageAsync("Sorry, I don't recognize that number.");

                        }
                    }
                    else
                    {
                        await Context.Channel.SendMessageAsync("Sorry, I don't recognize that number.");

                    }
                }
                else if (arguments[0].Contains("-"))
                {
                    arguments = new List<string>(arguments[0].Split('-'));
                    if (int.TryParse(arguments[0], out sides))
                    {
                        var dice = new Dice(sides);
                        var temp = dice.Roll();
                        if (int.TryParse(arguments[1], out modifier))
                        {
                            await Context.Channel.SendMessageAsync(
                                    string.Format("Rolled one d{0} minus {1} and got a total of {2}", sides,
                                        modifier, temp - modifier));
                        }
                        else
                        {
                            await Context.Channel.SendMessageAsync("Sorry, I don't recognize that number.");

                        }
                    }
                    else
                    {
                        await Context.Channel.SendMessageAsync("Sorry, I don't recognize that number.");

                    }
                }
                else
                {
                    if (int.TryParse(arguments[0], out sides))
                    {
                        var dice = new Dice(sides);
                        await Context.Channel.SendMessageAsync(string.Format("Rolled one d{0} and got a total of {1}",
                                sides, dice.Roll()));
                    }
                    else
                    {
                        await Context.Channel.SendMessageAsync("Sorry, I don't recognize that number.");

                    }
                }
            }
            else if (arguments.Count == 2)
            {
                if (int.TryParse(arguments[0], out times))
                {
                    if (arguments[1].Contains("+"))
                    {
                        arguments = new List<string>(arguments[1].Split('+'));
                        if (int.TryParse(arguments[0], out sides))
                        {
                            var dice = new Dice(sides);
                            var temp = dice.Roll(times);
                            if (int.TryParse(arguments[1], out modifier))
                            {
                                await Context.Channel.SendMessageAsync(
                                        string.Format("Rolled {0} d{1} plus {2} and got a total of {3}",
                                            times, sides, modifier, temp.Total + modifier));
                                await Context.Channel.SendMessageAsync(string.Format("Individual Rolls: {0}",
                                        string.Join(",", temp.Rolls)));

                            }
                            else
                            {
                                await Context.Channel.SendMessageAsync("Sorry, I don't recognize that number.");

                            }
                        }
                        else
                        {
                            await Context.Channel.SendMessageAsync("Sorry, I don't recognize that number.");

                        }
                    }
                    else if (arguments[1].Contains("-"))
                    {
                        arguments = new List<string>(arguments[1].Split('-'));
                        if (int.TryParse(arguments[0], out sides))
                        {
                            var dice = new Dice(sides);
                            var temp = dice.Roll(times);
                            if (int.TryParse(arguments[1], out modifier))
                            {
                                await Context.Channel.SendMessageAsync(
                                        string.Format("Rolled {0} d{1} minus {2} and got a total of {3}", times, sides,
                                            modifier, temp.Total-modifier));
                                await Context.Channel.SendMessageAsync(string.Format("Individual Rolls: {0}",
                                        string.Join(",", temp.Rolls)));

                            }
                            else
                            {
                                await Context.Channel.SendMessageAsync("Sorry, I don't recognize that number.");

                            }
                        }
                        else
                        {
                            await Context.Channel.SendMessageAsync("Sorry, I don't recognize that number.");

                        }
                    }
                    else
                    {
                        if (int.TryParse(arguments[1], out sides))
                        {
                            var dice = new Dice(sides);
                            var temp = dice.Roll(times);
                            await Context.Channel.SendMessageAsync(string.Format(
                                    "Rolled {0} d{1} and got a total of {2}", times, sides, temp.Total));
                            await Context.Channel.SendMessageAsync(string.Format("Individual Rolls: {0}",
                                    string.Join(",", temp.Rolls)));

                        }
                        else
                        {
                            await Context.Channel.SendMessageAsync("Sorry, I don't recognize that number.");

                        }
                    }
                }
            }
        }

        [Command("emojilist")]
        public async Task EmojiList()
        {
            string msg = "";
            foreach (var guild in Context.Client.GetGuildsAsync().Result)
            {
                foreach (var emote in guild.Emotes)
                {
                    msg += emote.Name + ": <:" + emote.Name + ":" + emote.Id + ">" + "\n";
                    if (msg.Length >= 1900)
                    {
                        await Context.User.SendMessageAsync(msg);
                        msg = "";
                    }
                }
                

            }
            if (msg != "")
            {
                await Context.User.SendMessageAsync(msg);
            }

        }

        [Command("emoji")]
        public async Task Emoji([Remainder, Summary("name")]string name = "none")
        {
            bool brokeOut = false;            
            foreach (var guild in Context.Client.GetGuildsAsync().Result)
            {
                foreach (var emote in guild.Emotes)
                {
                    if (emote.Name == name)
                    {
                        await Context.Channel.SendMessageAsync("<:" + emote.Name + ":" + emote.Id + ">");
                        brokeOut = true;
                        break;
                    }
                }
                if (brokeOut)
                {
                    break;
                }
            }
        }


        [Command("trust"), RequireOwner, Summary("Adds the user's ID to the trusted user list. Trusted users will be able to execute commands regardless of permissions.")]
        public async Task TrustUser([Remainder, Summary("User's ID")] string userId)
        {
            ulong userUlong;
            if (ulong.TryParse(userId, out userUlong))
            {
                if (Context.Guild.GetUserAsync(userUlong) != null)
                {
                    PermissionHandler.AddTrustedUser(userId, Context.Guild.GetUserAsync(userUlong).Result.Username);
                    await Context.Channel.SendMessageAsync("You really trust that one? Well, _you're_ the boss. :thumbsup:");
                }
                else
                {
                    await Context.Channel.SendMessageAsync(ResponseCollections.UserNotFoundResponses.GetRandom());
                }
            }
            else
            {
                await Context.Channel.SendMessageAsync("That doesn't seem to be the right input...");
            }
        }

        [Command("distrust"), RequireOwner, Summary("Removes the user's ID from the trusted user list.")]

        public async Task DistrustUser([Remainder, Summary("User's ID")] string userId)
        {
            ulong userUlong;
            if (ulong.TryParse(userId, out userUlong))
            {
                if (Context.Guild.GetUserAsync(userUlong) != null)
                {
                    PermissionHandler.RemoveTrustedUser(userId);
                    await Context.Channel.SendMessageAsync("Yeah fuck that guy!");
                }
                else
                {
                    await Context.Channel.SendMessageAsync(ResponseCollections.UserNotFoundResponses.GetRandom());
                }
            }
            else
            {
                await Context.Channel.SendMessageAsync("That doesn't seem to be the right input...");
            }
        }

        [Command("renick"), RequireOwner, Summary("Renames the bot")]
        public async Task ChangeNickname([Remainder, Summary("what to rename the bot")] string newNick)
        {
            await Context.Guild.GetCurrentUserAsync().Result.ModifyAsync(b => b.Nickname = newNick);
        }

        [Command("8ball"), RequireColPermission("speak"), Summary("Ask Colonel Panic a true or false question.")]
        public async Task Send8BallResponse([Remainder, Summary("The question!")] string question)
        {
            await Context.Channel.SendMessageAsync(ResponseCollections._8BallResponses.GetRandom());
        }

        [Command("naughty"), RequireTrustedUserOrPermission(ChannelPermission.ManageChannel), Summary("Sets the specified user (by id) as being _NAUGHTY_")]
        public async Task NaughtyUser([Remainder, Summary("User's ID")] string userId)
        {
            ulong userUlong;
            if (ulong.TryParse(userId, out userUlong))
            {
                if (Context.Guild.GetUserAsync(userUlong) != null)
                {
                    PermissionHandler.RemoveTrustedUser(userId);
                    await Context.Channel.SendMessageAsync("Oh yes, I see exactly what you mean. They're definitely _naughty_.");
                }
                else
                {
                    await Context.Channel.SendMessageAsync(ResponseCollections.UserNotFoundResponses.GetRandom());
                }
            }
            else
            {
                await Context.Channel.SendMessageAsync("That doesn't seem to be the right input...");
            }
        }
        [Command("notnaughty"), RequireTrustedUserOrPermission(ChannelPermission.ManageChannel), Summary("Sets the specified user (by id) as being _NAUGHTY_")]
        public async Task NotNaughtyUser([Remainder, Summary("User's ID")] string userId)
        {
            ulong userUlong;
            if (ulong.TryParse(userId, out userUlong))
            {                
                if (Context.Guild.GetUserAsync(userUlong) != null)
                {
                    UserDataHandler.SetUserNaughtyState(userId,false);
                    await Context.Channel.SendMessageAsync("Ok, they're no longer on the naughty list.");
                }
                else
                {
                    await Context.Channel.SendMessageAsync(ResponseCollections.UserNotFoundResponses.GetRandom());
                }
            }
            else
            {
                await Context.Channel.SendMessageAsync("That doesn't seem to be the right input...");
            }
        }
    }
}
