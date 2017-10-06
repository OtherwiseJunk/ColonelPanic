using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonelPanic.Permissions
{
    public class RequireChannelAttribute : PreconditionAttribute
    {
        public string ChannelId;
        public RequireChannelAttribute(string channelId)
        {
            ChannelId = channelId;
        }
        public async override Task<PreconditionResult> CheckPermissions(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            
            // If this command was executed by that user, return a success
            if (context.Channel.Id.ToString() == ChannelId)
                return PreconditionResult.FromSuccess();
            // Since it wasn't, fail
            else
                return PreconditionResult.FromError("That command is not available on this channel.");
        }
    }

    public class RequireGuildAttribute : PreconditionAttribute
    {        
        public string GuildId;
        public RequireGuildAttribute(string guildId)
        {
            GuildId = guildId;
        }
        public async override Task<PreconditionResult> CheckPermissions(ICommandContext context, CommandInfo command, IServiceProvider services)
        {

            // If this command was executed by that user, return a success
            if (context.Guild.Id.ToString() == GuildId)
                return PreconditionResult.FromSuccess();
            // Since it wasn't, fail
            else
                return PreconditionResult.FromError("That command is not available on this channel.");
        }
    }
}
