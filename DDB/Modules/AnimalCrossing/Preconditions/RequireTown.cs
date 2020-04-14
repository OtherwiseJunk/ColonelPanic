using DartsDiscordBots.Interfaces;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace DartsDiscordBots.Preconditions
{
	public class RequireTown : PreconditionAttribute
	{
		public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
		{
			var obj = services.GetService<IAnimalCrossingService>();
			if (services.GetService<IAnimalCrossingService>().UserHasTown(context.User.Id))
			{
				return Task.FromResult(PreconditionResult.FromSuccess());
			}
			return Task.FromResult(PreconditionResult.FromError("You haven't registered your town yet, so you can't tell me about your towns prices or fruits, or write letters to other mayors!"));
		}
	}
}
