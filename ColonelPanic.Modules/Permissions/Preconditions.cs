using Discord.Commands;
using ColonelPanic.Database.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;

namespace ColonelPanic.Utilities.Permissions
{
    public class RequireColPermissionAttribute : PreconditionAttribute
    {
        public string Permission;
        public RequireColPermissionAttribute(string perm)
        {
            Permission = perm;
        }
        public async override Task<PreconditionResult> CheckPermissions(ICommandContext context, CommandInfo command, IServiceProvider services)
        {


            if (PermissionHandler.CanExecute(Permission, context.Guild.Id.ToString(), context.User.Id.ToString()))
                return PreconditionResult.FromSuccess();

            else
                return PreconditionResult.FromError("That command is not available on this channel.");
        }
    }

    public class RequireTrustedUser : PreconditionAttribute
    {
        public RequireTrustedUser() { }

        public async override Task<PreconditionResult> CheckPermissions(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            if (PermissionHandler.IsTrustedUser(context.User.Id.ToString()))
            {
                return PreconditionResult.FromSuccess();
            }

            else
            {                
                return PreconditionResult.FromError("Unauthorized user detected. This action has been logged.");
            }
        }
    }

    public class RequireChannelConfiguration : PreconditionAttribute
    {
        public string ConfigurationType;
        public RequireChannelConfiguration(string configurationType)
        {
            ConfigurationType = configurationType;
        }
        public async override Task<PreconditionResult> CheckPermissions(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            if (PermissionHandler.ChannelConfigured(ConfigurationType, context.Channel.Id.ToString()))
                return PreconditionResult.FromSuccess();

            else
                return PreconditionResult.FromError("That module is not setup for this channel, I'm missing configuration details.");
        }
    }
}
