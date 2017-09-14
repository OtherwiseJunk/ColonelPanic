using System.Threading.Tasks;
using Discord.Commands;
using ColonelPanic.Database.Contexts;


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

        [Command("config"), Summary("Sets the config in the database."), RequireOwner]
        public async Task SetConfig([Summary("Bot Secret Token")] string token, [Summary("GitHub Secret Token")] string githubToken, [Summary("Last Github Commit")] string githubCommit)
        {
            ConfigurationHandler.CreateConfig(token, githubToken, githubCommit);
        }
    }
}
