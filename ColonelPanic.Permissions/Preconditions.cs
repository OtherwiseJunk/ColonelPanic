using ColonelPanic.Database.Contexts;
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
            
            
            if (context.Channel.Id.ToString() == ChannelId)
                return PreconditionResult.FromSuccess();
            
            else
                return PreconditionResult.FromError("That command is not available on this channel.");
        }
    }

    public class RequireColPermissionAttribute : PreconditionAttribute
    {
        public string Permission;
        public RequireColPermissionAttribute(string perm)
        {
            Permission = perm;
        }
        public async override Task<PreconditionResult> CheckPermissions(ICommandContext context, CommandInfo command, IServiceProvider services)
        {

            
            if (PermissionHandler.CanExecute(Permission,context.Channel.Id.ToString(),context.User.Id.ToString()))
                return PreconditionResult.FromSuccess();
            
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

            
            if (context.Guild.Id.ToString() == GuildId)
                return PreconditionResult.FromSuccess();
            
            else
                return PreconditionResult.FromError("That command is not available on this channel.");
        }
    }
}
