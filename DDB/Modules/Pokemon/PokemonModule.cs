using DartsDiscordBots.Modules.Pokemon.Interfaces;
using DartsDiscordBots.Modules.Pokemon.Models;
using DartsDiscordBots.Utilities;
using Discord;
using Discord.Commands;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DartsDiscordBots.Modules.Pokemon
{
	public class PokemonModule : ModuleBase
	{
		public IPokemonService _pokemon;
		public PokemonModule(IPokemonService pokemon) { _pokemon = pokemon; }

		[Command("sword"), Summary("Adds you to the Pokemon Sword role.")]
		public async Task AddSwordRole()
		{			
			if (_pokemon.ConfiguredGuilds.ContainsKey(Context.Guild.Id))
			{
				PokemonGuildConfiguration pokemonConfig = _pokemon.ConfiguredGuilds[Context.Guild.Id];
				IRole SwordRole = Context.Guild.GetRole(pokemonConfig.SwordRoleId);
				if (!(Context.User as IGuildUser).RoleIds.Contains(pokemonConfig.SwordRoleId))
				{
					await (Context.User as IGuildUser).AddRoleAsync(SwordRole);
					await Context.Message.AddReactionAsync(Context.Guild.Emotes.First(emote => emote.Id == pokemonConfig.SwordEmoteId));
					if ((Context.User as IGuildUser).RoleIds.Contains(pokemonConfig.ShieldRoleId))
					{
						await Context.Channel.SendFileAsync(Converters.GetImageStreamFromBase64(_pokemon.TwoPokemonSwordImage), "TWOPokemonz! - Sword.png");
					}
				}
				else
				{
					await Context.Channel.SendMessageAsync("You're already in the role!");
				}
			}
			else await Context.Channel.SendMessageAsync("Sorry, this guild is not configured for the Pokemon module.");
		}

		[Command("shield"), Summary("Adds you to the Pokemon Shield role.")]
		public async Task AddShieldRole()
		{
			if (_pokemon.ConfiguredGuilds.ContainsKey(Context.Guild.Id))
			{
				PokemonGuildConfiguration pokemonConfig = _pokemon.ConfiguredGuilds[Context.Guild.Id];
				IRole ShieldRole = Context.Guild.GetRole(pokemonConfig.ShieldRoleId);
				if (!(Context.User as IGuildUser).RoleIds.Contains(pokemonConfig.ShieldRoleId))
				{
					await (Context.User as IGuildUser).AddRoleAsync(ShieldRole);
					await Context.Message.AddReactionAsync(Context.Guild.Emotes.First(emote => emote.Id == pokemonConfig.ShieldEmoteId));
					if ((Context.User as IGuildUser).RoleIds.Contains(pokemonConfig.SwordRoleId))
					{
						await Context.Channel.SendFileAsync(Converters.GetImageStreamFromBase64(_pokemon.TwoPokemonShieldImage), "TWOPokemonz! - Shield.png");
					}
				}
				else
				{
					await Context.Channel.SendMessageAsync("You're already in the role!");
				}
			}
			else await Context.Channel.SendMessageAsync("Sorry, this guild is not configured for the Pokemon module.");
		}

		[Command("ididit"), Summary("Marks you as a LEAGUE CHAMPION.")]
		public async Task DesignateAsChampion()
		{
			if (_pokemon.ConfiguredGuilds.ContainsKey(Context.Guild.Id))
			{
				PokemonGuildConfiguration pokemonConfig = _pokemon.ConfiguredGuilds[Context.Guild.Id];
				IGuildUser user = Context.User as IGuildUser;
				if (!user.RoleIds.Contains(pokemonConfig.ShieldChampionRoleId) && !user.RoleIds.Contains(pokemonConfig.SwordChampionRoleId))
				{
					if (user.RoleIds.Contains(pokemonConfig.ShieldRoleId) || user.RoleIds.Contains(pokemonConfig.SwordRoleId))
					{
						AddVersionChampionship(user, Context, pokemonConfig);
						if (user.RoleIds.Contains(pokemonConfig.GenericChampionRoleId))
						{
							await user.RemoveRoleAsync(Context.Guild.GetRole(pokemonConfig.GenericChampionRoleId));
						}
					}
					else
					{
						await user.AddRoleAsync(Context.Guild.GetRole(pokemonConfig.GenericChampionRoleId));
					}
					await Context.Message.AddReactionAsync(Context.Guild.Emotes.First(e => e.Id == pokemonConfig.LeagueChampionEmoteId));
				}
				else
				{
					await Context.Channel.SendMessageAsync("We already know you're a champ. Showoff.");
				}
			}
			else await Context.Channel.SendMessageAsync("Sorry, this guild is not configured for the Pokemon module.");
		}

		private void AddVersionChampionship(IGuildUser user, ICommandContext context, PokemonGuildConfiguration pokemonConfig)
		{
			if (user.RoleIds.Contains(pokemonConfig.ShieldRoleId))
			{
				user.AddRoleAsync(context.Guild.Roles.First(r => r.Id == pokemonConfig.ShieldChampionRoleId));
			}
			else
			{
				user.AddRoleAsync(context.Guild.Roles.First(r => r.Id == pokemonConfig.SwordChampionRoleId));
			}
		}
	}
}
