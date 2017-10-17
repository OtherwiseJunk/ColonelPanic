using ColonelPanic.Permissions;
using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonelPanic.Modules
{
    [Group("group"),RequireColPermission("ping"),Summary("Commands for creating, managing, and pinging groups of users.")]
    class PingGroupModule : ModuleBase
    {
        [Command("ping"),Summary("Pings all users that belong to the specified Ping Group. Requires Manage Channel permission."),RequireUserPermission(ChannelPermission.ManageChannel)]
        public async Task PingUsers([Remainder, Summary("Name of the Ping Group to ping.")] string pingGroupName)
        {
            return;
        }

        [Command("create"), Summary("Creates the specified Ping Group. Requires Manage Channel permission."), RequireUserPermission(ChannelPermission.ManageChannel)]
        public async Task CreatePingGroup([Remainder, Summary("Name of the Ping Group to create.")] string pingGroupName)
        {

        }

        [Command("list"), Summary("List all available Ping Groups for this guild.")]
        public async Task ListPingGroups([Remainder, Summary("arguments for the list command. Currently accepts \"members\"")] string args = "null")
        {
            if (args == "null")
            {

            }
            else if (args == "members")
            {

            }
            else
            {
                await ReplyAsync("Sorry, I don't recognize that argument. Either type \"$group list\" or \"$group list members\"");
            }
        }                        

        [Command("join"), Summary("Adds you to the specified Ping Group.")]
        public async Task JoinPingGroup([Remainder, Summary("The Ping Group to join.")])
        {

        }

        [Command("adduser"), Summary("Adds the specified user (by UserID) to the specified Ping Group. Example \"$group adduser 000000000000000 SomeGroup\"")]
        public async Task AddUserToPingGroup()
        {

        }
    }
}
