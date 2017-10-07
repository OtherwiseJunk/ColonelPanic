using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ColonelPanic.Permissions;
using ColonelPanic.Utilities;
using Discord;

namespace ColonelPanic.Modules
{
    [RequireGuild("177229913512738816"),Summary("A module all for Geosus!")]
    public class GeosusModule : ModuleBase
    {
        [Command("adventburger"), Summary("Selects a random Squaddie.")]
        public async Task AdventBurger([Remainder, Summary("The number of squaddies. return 1 by default.")] string number = "f")
        {
            int numberOfSquaddies;

            List<IGuildUser> allUsers = Context.Guild.GetUsersAsync().Result.ToList();
            List<IGuildUser> squaddies = new List<IGuildUser>();            
            foreach (IGuildUser user in allUsers)
            {
                foreach (ulong role in user.RoleIds)
                {
                    if (role == 355954896048095232)
                    {
                        squaddies.Add(user);
                    }
                }
            }

            if (Int32.TryParse(number, out numberOfSquaddies))
            {
                string msg = "";
                string user = "";
                while (numberOfSquaddies > 0 && squaddies.Count != 0)
                {
                    user = GetRandomSquaddie(squaddies);
                    squaddies.RemoveAll(u => u.Username == user);
                    msg += user + Environment.NewLine;
                    numberOfSquaddies--;
                }
                await Context.Channel.SendMessageAsync(msg);
            }
            else
            {
                await Context.Channel.SendMessageAsync(GetRandomSquaddie(squaddies));
            }
            
        }

        public string GetRandomSquaddie(List<IGuildUser> users)
        {
            return users.GetRandom().Username;
        }

    }
}
