using ColonelPanic.Database.Contexts;
using ColonelPanic.Utilities.Permissions;
using DartsDiscordBots.Utilities;
using ColonelPanic.Utilities;
using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ColonelPanic.Modules
{
    [Group("group"), RequireColPermission("ping"), Summary("Commands for creating, managing, and pinging groups of users.")]
    public class PingGroupModule : ModuleBase
    {
        [Command("ping"), Summary("Pings all users that belong to the specified Ping Group. Requires Manage Channel permission."), RequireUserPermission(ChannelPermission.ManageChannel)]
        public async Task PingUsers([Remainder, Summary("Name of the Ping Group to ping.")] string pingGroupName)
        {
            if (PingGroupHandler.PingGroupExists(pingGroupName, Context.Guild.Id.ToString()))
            {
                List<string> userIds = PingGroupHandler.GetPingUsers(pingGroupName, Context.Guild.Id.ToString());

                switch (userIds[0])
                {
                    case "empty":
                        await ReplyAsync(ResponseCollections.PingGroupEmpty.GetRandom());
                        break;
                    default:
                        string msg = "";
                        foreach (string userId in userIds)
                        {
                            IUser user = Context.Guild.GetUserAsync(ulong.Parse(userId)).Result;
                            msg += user.Mention + " ";
                            while(msg.Length >= 2000)
                            {
                                IMessage tempMessage = ReplyAsync(msg).Result;
                                await tempMessage.DeleteAsync();
                                msg = "";
                            }
                        }
                        IMessage message = ReplyAsync(msg).Result;
                        await message.DeleteAsync();
                        await ReplyAsync($"Group {pingGroupName} Pinged");
                        break;
                }
            }
            else
            {
                await ReplyAsync(ResponseCollections.PingGroupNotFound.GetRandom());
            }
        }

        [Command("create"), Summary("Creates the specified Ping Group. Requires Manage Channel permission."), RequireUserPermission(ChannelPermission.ManageChannel)]
        public async Task CreatePingGroup([Remainder, Summary("Name of the Ping Group to create.")] string pingGroupName)
        {
            if (!PingGroupHandler.PingGroupExists(pingGroupName, Context.Guild.Id.ToString()))
            {
                PingGroupHandler.CreatePingGroup(pingGroupName, Context.Guild.Id.ToString());
                await ReplyAsync($"Ok, I've created the {pingGroupName} group!");
            }
            else
            {
                await ReplyAsync("Sorry, looks like we already have a ping group by that name!");
            }
        }

        [Command("list"), Summary("List all available Ping Groups for this guild.")]
        public async Task ListPingGroups()
        {
            await ReplyAsync(PingGroupHandler.GetPingGroups(Context.Guild.Id.ToString()));
        }

        [Command("members")]
        public async Task ListPingGroupMembers([Remainder, Summary("The name of the ping group to get the members of.")] string pingGroupName)
        {
            if (PingGroupHandler.PingGroupExists(pingGroupName, Context.Guild.Id.ToString()))
            {
                List<string> users = PingGroupHandler.GetPingUsers(pingGroupName, Context.Guild.Id.ToString());
                if (users[0] != "empty")
                {
                    string msg = $"**Members of {pingGroupName}**:" + Environment.NewLine;
                    foreach (string user in users)
                    {
                        msg += Context.Guild.GetUserAsync(ulong.Parse(user)).Result.Username;
                        msg += Environment.NewLine;
                        
                    }
                    msg.TrimEnd(Environment.NewLine.ToCharArray());
                    await ReplyAsync(msg);
                }
                else
                {
                    await ReplyAsync(ResponseCollections.PingGroupEmpty.GetRandom());
                }
            }
            else
            {
                await ReplyAsync(ResponseCollections.PingGroupNotFound.GetRandom());
            }

        }


        [Command("join"), Summary("Adds you to the specified Ping Group.")]
        public async Task JoinPingGroup([Remainder, Summary("The Ping Group to join.")] string pingGroupName)
        {
            if (PingGroupHandler.PingGroupExists(pingGroupName, Context.Guild.Id.ToString()))
            {
                int pingGroupId = PingGroupHandler.GetPingGroupId(pingGroupName, Context.Guild.Id.ToString());
                if(!PingGroupHandler.PingGroupUserExists(pingGroupId, Context.User.Id.ToString(), Context.Guild.Id.ToString()))
                {
                    PingGroupHandler.AddPingGroupUser(pingGroupId, Context.User.Id.ToString(), Context.Guild.Id.ToString());
                    await ReplyAsync($"Ok, I've added you to that {pingGroupName}.");
                }
                else
                {
                    await ReplyAsync("You're already in it.");
                }
            }
            else
            {
                await ReplyAsync(ResponseCollections.PingGroupNotFound.GetRandom());
            }
        }

        [Command("adduser"), Summary("Adds the specified user (by UserID) to the specified Ping Group. Example \"$group adduser 000000000000000 SomeGroup\". Requires Manage Channel permission."), RequireUserPermission(ChannelPermission.ManageChannel)]
        public async Task AddUserToPingGroup([Summary("The userId")]string userId,[Remainder, Summary("The Ping Group to ass the user to")]string pingGroupName)
        {
            ulong uId;
            if(ulong.TryParse(userId,out uId))
            {
                if(Context.Guild.GetUserAsync(uId) != null)
                {
                    if (PingGroupHandler.PingGroupExists(pingGroupName, Context.Guild.Id.ToString()))
                    {
                        int groupId = PingGroupHandler.GetPingGroupId(pingGroupName, Context.Guild.Id.ToString());
                        if (!PingGroupHandler.PingGroupUserExists(groupId, uId.ToString(), Context.Guild.Id.ToString()))
                        {
                            PingGroupHandler.AddPingGroupUser(groupId, userId, Context.Guild.Id.ToString());
                            await ReplyAsync($"Ok, I've added that user to {pingGroupName}.");
                        }
                        else
                        {
                            await ReplyAsync("That user is already in there.");
                        }
                    }
                    else
                    {
                        await ReplyAsync(ResponseCollections.PingGroupNotFound.GetRandom()); ;
                    }
                }
                else
                {
                    await ReplyAsync("That's not the userId of a user in this guild.");
                }
            }
            else
            {
                await ReplyAsync("That's not a valid userId, I need digits!");
            }
        }
    }
}
