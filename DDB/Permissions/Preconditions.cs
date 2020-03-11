using Discord.Commands;
using System;
using System.Threading.Tasks;

namespace DartsDiscordBots.Permissions
{
    public class RequireChannelAttribute : PreconditionAttribute
    {
        public string ChannelId;
        public RequireChannelAttribute(string channelId)
        {
            ChannelId = channelId;
        }

		public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
		{

			if (context.Channel.Id.ToString() == ChannelId)
				return new Task(() => PreconditionResult.FromSuccess()) as Task<PreconditionResult>;

			else
				return new Task(() => PreconditionResult.FromError("That command is not available on this channel.")) as Task<PreconditionResult>;
		}
	}

    

    public class RequireGuildAttribute : PreconditionAttribute
    {        
        public string GuildId;
        public RequireGuildAttribute(string guildId)
        {
            GuildId = guildId;
        }

		public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
		{
			if (context.Guild.Id.ToString() == GuildId)
				return new Task (() => PreconditionResult.FromSuccess()) as Task<PreconditionResult>;

			else
				return new Task(() => PreconditionResult.FromError("That command is not available on this channel.")) as Task<PreconditionResult>;
		}
	}
}
