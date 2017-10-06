using System.Threading.Tasks;
using Discord.Commands;
using ColonelPanic.Utilities
using ColonelPanic.Database.Contexts;
using System.Collections.Generic;

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

        [Command("renick"), RequireOwner, Summary("Renames the bot")]
        public async Task ChangeNickname([Remainder, Summary("what to rename the bot")] string newNick)
        {
            await Context.Guild.GetCurrentUserAsync().Result.ModifyAsync(b => b.Nickname = newNick);
        }

        [Command("8ball"), Summary("Ask Colonel Panic a true or false question.")]
        public async Task Send8BallResponse()
        {
            await Context.Channel.SendMessageAsync(ResponseCollections._8BallResponses.GetRandom());
        }
    }
}
