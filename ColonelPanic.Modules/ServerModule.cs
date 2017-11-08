﻿using System.Threading.Tasks;
using Discord.Commands;
using ColonelPanic.Utilities;
using ColonelPanic.Database.Contexts;
using ColonelPanic.Permissions;
using System.Collections.Generic;
using Discord;
using System.Threading;

namespace ColonelPanic.Modules
{
    public class ServerModule : ModuleBase
    {
        [Command("say"), Summary("Echos a message."), RequireOwner]
        public async Task Say([Remainder, Summary("The text to echo")] string echo)
        {
            // ReplyAsync is a method on ModuleBase
            await ReplyAsync(echo);
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

        [Command("poop"), RequireOwner]
        public async Task poop([Remainder] string userId)
        {
            string chnlId = Context.Channel.Id.ToString();
            if (UserDataHandler.UserHasFlags(chnlId,userId))
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

        [Command("eggplant"), RequireOwner]
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

        [Command("emojilist")]
        public async Task EmojiList()
        {
            string msg = "";
            foreach (var guild in Context.Client.GetGuildsAsync().Result)
            {                                
                foreach (var emote in guild.Emotes)
                {
                    msg += emote.Name+ ": <:" + emote.Name + ":" + emote.Id + ">"+"\n";

                }
                if (msg.Length >= 1950)
                {
                    await Context.User.SendMessageAsync(msg);
                    msg = "";
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
            string emoji = "";
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
    }
}
