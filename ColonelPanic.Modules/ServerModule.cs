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

        [Command("trust"), RequireOwner, Summary("Adds the user's ID to the trusted user list. Trusted users will be able to execute commands regardless of permissions.")]
        public async Task TrustUser([Remainder, Summary("User's ID")] string userId)
        {
            ulong userUlong;
            if(ulong.TryParse(userId, out userUlong))
            {
                if (Context.Guild.GetUserAsync(userUlong) != null)
                {
                    PermissionHandler.AddTrustedUser(userId, Context.Guild.GetUserAsync(userUlong).Result.Username);
                }
                else
                {
                    await Context.Channel.SendMessageAsync(ResponseCollections.UserNotFoundResponses.GetRandom());
                }
            }
            else
            {
                await Context.Channel.SendMessageAsync("That doesn't seem to be the right input...");
            }
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
