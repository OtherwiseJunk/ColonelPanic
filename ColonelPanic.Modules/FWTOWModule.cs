using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using ColonelPanic.Permissions;
using Discord;
using Discord.WebSocket;

namespace ColonelPanic.Database.Models
{
    [Name("Freeworld's Role Management"),RequireGuild("93924120042934272")]
    public class FWTOWModule : ModuleBase
    {
        public static ulong NR = 94475585576775680;
        public static ulong GE = 94475600810483712;
        public static ulong HS = 286687880351580162;
        public static ulong CP = 268183236310597633;

        [Command("nr")]
        public async Task AddNRRole()
        {
            SetRole("nr", Context.User as SocketGuildUser);
            await Context.Channel.SendMessageAsync(":thumbsup:");
        }
        [Command("ge")]
        public async Task AddGERole()
        {
            SetRole("ge", Context.User as SocketGuildUser);
            await Context.Channel.SendMessageAsync(":thumbsup:");
        }
        [Command("hs")]
        public async Task AddHSRole()
        {
            SetRole("hs", Context.User as SocketGuildUser);
            await Context.Channel.SendMessageAsync(":thumbsup:");
        }
        [Command("cp")]
        public async Task AddCPRole()
        {            
            SetRole("cp", Context.User as SocketGuildUser);
            await Context.Channel.SendMessageAsync(":thumbsup:");
        }

        public void SetRole(string role, SocketGuildUser user)
        {
            IRole Role = null;
            switch (role)
            {
                case "nr":
                    user.AddRoleAsync(Role = user.Guild.GetRole(NR));
                    break;
                case "ge":
                    user.AddRoleAsync(Role = user.Guild.GetRole(GE));
                    break;
                case "hs":
                    user.AddRoleAsync(Role = user.Guild.GetRole(HS));
                    break;
                case "cp":
                    user.AddRoleAsync(Role = user.Guild.GetRole(CP));
                    break;
            }
            Console.WriteLine(".");
        }
    }
}
