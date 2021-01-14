using Discord;
using Discord.Commands;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DartsDiscordBots.Modules.ServerManagement.Interfaces
{
	public interface IServerManagmentService
	{
		Regex hexColorValidator { get; set; }
		Color ParseDiscordColor(string hexText);
		bool RoleExists(IReadOnlyCollection<IRole> roles, string username, out IRole usernameRole);
		Task<IRole> ModifyUserRoleColor(string hexText, IUser user, ICommandContext Context);
	}
}
