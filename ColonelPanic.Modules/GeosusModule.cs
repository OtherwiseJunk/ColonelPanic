﻿using Discord.Commands;
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
	[Summary("A module all for Geosus!"), Name("Good Music Geosus-Chan Special")]
	public class GeosusModule : ModuleBase
	{
		public static string bird2ImageBase64Encoded = "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";

		[Command("adventburger"), RequireRoleName("squaddie"), Summary("Selects a random Squaddie.")]
		public async Task AdventBurger([Remainder, Summary("The number of squaddies. return 1 by default.")] int numberOfSquaddies = 1)
		{
			List<IGuildUser> allUsers = Context.Guild.GetUsersAsync().Result.ToList();
			IRole squaddieRole = Context.Guild.Roles.FirstOrDefault(r => r.Name.ToLower() == "squaddie");
			List<IGuildUser> squaddies = allUsers.Where(u => u.RoleIds.Contains(squaddieRole.Id)).ToList();


			string msg = "";
			string chosenSquaddie = "";
			for (int i = 0; i < numberOfSquaddies; i++)
			{
				chosenSquaddie = squaddies.GetRandom().Username;
				squaddies.RemoveAll(u => u.Username == chosenSquaddie);
				msg += chosenSquaddie + Environment.NewLine;
			}
			await Context.Channel.SendMessageAsync(msg);

		}

		[Command("bird2"), Summary("Outputs the bird2 emoji")]
		public async Task Bird2()
		{
			await Context.Channel.SendFileAsync(Converters.GetImageStreamFromBase64(bird2ImageBase64Encoded), "bird2.png");
		}

		[Command("scratchy")]
		public async Task scratchy()
        {
			await Context.Channel.SendMessageAsync("You are boring, and small.");
        }
	}
}
