using DartsDiscordBots.Modules.ServerManagement.Interfaces;
using Discord;
using Discord.Commands;
using System.Threading.Tasks;

namespace DartsDiscordBots.Modules.ServerManagement
{
	public class ServerManagementModule : ModuleBase
	{
		public IServerManagmentService _server;
		public ServerManagementModule(IServerManagmentService server) { _server = server; }

        [Command("rolecolor"), Summary("Creates a role with your name with the specified (in hex) color. Ex) `rolecolor #000000`"), RequireBotPermission(GuildPermission.ManageRoles)]
        public async Task RoleColorChange([Remainder, Summary("The hexcode for your desired color.")] string hexText)
        {
            if (_server.hexColorValidator.Match(hexText).Success && (hexText.Length == 7 || hexText.Length == 4))
            {
                var role = _server.ModifyUserRoleColor(hexText, this.Context.User, Context).Result;
                await (this.Context.User as IGuildUser).AddRoleAsync(role);
            }
            else
            {
                await this.Context.Channel.SendMessageAsync("Sorry, I can't recognize that hexcode. Maybe I'm an idiot, iunno.");
            }
        }
    }
}
