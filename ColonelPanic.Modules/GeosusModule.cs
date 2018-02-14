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
    [RequireGuild("177229913512738816"),Summary("A module all for Geosus!")]
    public class GeosusModule : ModuleBase
    {
        [Command("geo"), Summary("`Applies the 'Big if True'`")]
        public async Task Geo([Remainder, Summary("MsgId")]string msgId)
        {
            if (ulong.TryParse(msgId, out var mId))
            {
                var msg = Context.Channel.GetMessageAsync(mId).Result as IUserMessage;
                var emoji =new Emoji("\U0001f46c");
                await msg.AddReactionAsync(emoji);

                var emote = Emote.Parse("<:GeoFace:400771246599438337>");
                await msg.AddReactionAsync(emote);

                emoji = new Emoji("\U0001f46d");
                await msg.AddReactionAsync(emoji);


            }
        }
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
                string msg = "";
                squaddies.Shuffle();
                foreach (var squaddie in squaddies)
                {
                    msg += squaddie.Username + Environment.NewLine;
                }
                await Context.Channel.SendMessageAsync(msg);
            }
            
        }

        [Command("bird2"),Summary("Outputs the bird2 emoji")]
        public async Task Bird2()
        {
            Context.Channel.SendFileAsync("./bird2.png");
        }

        public string GetRandomSquaddie(List<IGuildUser> users)
        {
            return users.GetRandom().Username;
        }

    }
}
