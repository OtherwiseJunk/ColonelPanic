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

		public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
		{
			if (PermissionHandler.CanExecute(Permission, context.Guild.Id.ToString(), context.User.Id.ToString())) { 
				return Task.FromResult(PreconditionResult.FromSuccess()); 
			}
			return Task.FromResult(PreconditionResult.FromError("That command is not available on this channel."));
		}
	}
	public class RequireUserNotNaughty : PreconditionAttribute
	{
		public RequireUserNotNaughty() { }

		public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
		{
			if (UserDataHandler.IsUserNaughty(context.Message.Author.Id.ToString()))
			{
				return Task.FromResult(PreconditionResult.FromSuccess());
			}
			return Task.FromResult(PreconditionResult.FromError("Sorry, looks like you've been _naughty_."));
		}
	}

	public class RequireTrustedUser : PreconditionAttribute
	{
		public RequireTrustedUser() { }

		public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
		{
			if (PermissionHandler.IsTrustedUser(context.User.Id.ToString()))
			{
				return Task.FromResult(PreconditionResult.FromSuccess());
			}
			return  Task.FromResult(PreconditionResult.FromError("Unauthorized user detected. This action has been logged."));
		}
	}

	public class RequireTrustedUserOrPermission : PreconditionAttribute
	{

		RequireUserPermissionAttribute UserPermAttribute { get; set; }
		RequireTrustedUser TrustedUserAttribute { get; set; }

		public RequireTrustedUserOrPermission(ChannelPermission permission)
		{
			UserPermAttribute = new RequireUserPermissionAttribute(permission);
			TrustedUserAttribute = new RequireTrustedUser();
		}

		public RequireTrustedUserOrPermission(GuildPermission permission)
		{
			UserPermAttribute = new RequireUserPermissionAttribute(permission);
			TrustedUserAttribute = new RequireTrustedUser();
		}

		public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
		{
			if (UserPermAttribute.CheckPermissionsAsync(context, command, services).Result.IsSuccess)
			{
				return Task.FromResult(PreconditionResult.FromSuccess());
			}
			return Task.FromResult(PreconditionResult.FromError("Unauthorized user detected. This action has been logged."));
		}
	}

	public class RequireChannelConfiguration : PreconditionAttribute
    {
        public string ConfigurationType;
        public RequireChannelConfiguration(string configurationType)
        {
            ConfigurationType = configurationType;
        }

		public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
		{
			if (PermissionHandler.ChannelConfigured(ConfigurationType, context.Channel.Id.ToString()))
			{
				return Task.FromResult(PreconditionResult.FromSuccess()) as Task<PreconditionResult>;
			}
			return Task.FromResult(PreconditionResult.FromError("That module is not setup for this channel, I'm missing configuration details.")) as Task<PreconditionResult>;
		}
	}
}
