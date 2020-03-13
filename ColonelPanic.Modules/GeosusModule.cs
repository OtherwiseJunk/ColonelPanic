using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DartsDiscordBots.Permissions;
using DartsDiscordBots.Utilities;
using Discord;

namespace ColonelPanic.Modules
{
    [Summary("A module all for Geosus!"),Name("Good Music Geosus-Chan Special")]
    public class GeosusModule : ModuleBase
    {
        [Command("adventburger"), RequireRoleName("squaddie"), Summary("Selects a random Squaddie.")]
        public async Task AdventBurger([Remainder, Summary("The number of squaddies. return 1 by default.")] int numberOfSquaddies = 1)
        {
            List<IGuildUser> allUsers = Context.Guild.GetUsersAsync().Result.ToList();
            IRole squaddieRole = Context.Guild.Roles.FirstOrDefault(r => r.Name.ToLower() == "squaddie");
            List<IGuildUser> squaddies = allUsers.Where(u => u.RoleIds.Contains(squaddieRole.Id)).ToList();
            

            string msg = "";
            string chosenSquaddie = "";
            for (int i = 0; i < numberOfSquaddies; i++ )
            {
                chosenSquaddie = squaddies.GetRandom().Username;
                squaddies.RemoveAll(u => u.Username == chosenSquaddie);
                msg += chosenSquaddie + Environment.NewLine;
            }
            await Context.Channel.SendMessageAsync(msg);

        }

        [Command("bird2"),Summary("Outputs the bird2 emoji")]
        public async Task Bird2()
        {
            await Context.Channel.SendFileAsync("./bird2.png");
        }
    }
}
