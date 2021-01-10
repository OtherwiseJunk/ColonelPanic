using DartsDiscordBots.Utilities;
using Discord.Commands;
using System.Threading.Tasks;

namespace DartsDiscordBots.Modules.ChatModule
{
	public class ChatModule : ModuleBase
	{
		[Command("8ball"), Summary("Ask the bot a true or false question.")]
		public async Task Send8BallResponse([Remainder, Summary("The question!")] string question)
		{
			await Context.Channel.SendMessageAsync(ResponseCollections._8BallResponses.GetRandom());
		}

		
	}
}
