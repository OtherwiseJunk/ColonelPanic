using ColonelPanic.Utilities.Permissions;
using DartsDiscordBots.Utilities;
using Discord;
using Discord.Commands;
using PokeAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColonelPanic.Modules
{
	public class PokemonModule : ModuleBase
	{
		/*https://pokeapi.co/api/v2/pokemon/807 is the last entry that returns right now, so there are OFFICIALLY 807 pokemon I guess.*/
		public static int MAX_POKEDEX_NUMBER = 807;
		public const ulong SWORD_ROLE_ID = 646176982467346440;
		public const ulong SWORD_CHAMPION_ROLE_ID = 646564069259739156;
		public const ulong SHIELD_ROLE_ID = 646177358465859612;
		public const ulong SHIELD_CHAMPION_ROLE_ID = 646564212398620694;		
		public const ulong GENERIC_CHAMPION_ROLE = 646430064287809556;
		public const ulong SHIELD_EMOTE_ID = 646179677609656320;
		public const ulong LEAGUE_CHAMPION_EMOTE_ID = 646568825965510676;
		public const ulong SWORD_EMOTE_ID = 646179677400203265;
		public const ulong POKEMON_GUILD_ID = 646036606913871882;
		
		public const string Two_Pokemon_Shield_Image = "./2 pokemon shield.png";
		public const string Two_Pokemon_Sword_Image = "./2 pokemon sword.png";
		public static string PokemonImageAddress = "https://assets.pokemon.com/assets/cms2/img/pokedex/full/{0}.png";
		public static string SerebiiPokedexLink = "https://www.serebii.net/pokedex-sm/{0}.shtml";
		public static List<string> Pokemon404Response = new List<string> {			
			"That one isn't real, you know?",
			"I don't think I know that one.",
			"Never heard of it. Why don't you tell me more?",
			"No.",
		};

		[Command("pokedex"), Summary("Gets information for the Pokémon specified, either by National Pokédex number or species name. Links to the latest version of the Serebii Pokédex entry, currently Sun and Moon.")]
		public async Task GetPokedexEntry([Remainder] string pokemonIdentifier) {
			int pokedexNumber;
			PokemonSpecies pokemon = null;

			if (int.TryParse(pokemonIdentifier, out pokedexNumber))
			{
				if (pokedexNumber <= MAX_POKEDEX_NUMBER && pokedexNumber > 0)
				{					
					try
					{						
						pokemon = await DataFetcher.GetApiObject<PokemonSpecies>(pokedexNumber);
					}
					catch (Exception ex)
					{
						Console.WriteLine(ex.Message);												
					}

					if(pokemon != null)
					{										
						await Context.Channel.SendMessageAsync("", false, buildEmbedForPokedexEntry(pokemon));
					}
					else
					{
						await Context.Channel.SendMessageAsync("I think something's wrong with the API, sorry friend.");
					}
				}
				else
				{
					if(pokedexNumber > 0)
					{
						await Context.Channel.SendMessageAsync("I don't think there are that many Pokémon, my dude.");
					}
					else
					{
						await Context.Channel.SendMessageAsync("That's not how this works!");
					}
				}
			}
			else
			{				
				try
				{
					pokemon = await DataFetcher.GetNamedApiObject<PokemonSpecies>(pokemonIdentifier.ToLower());
				}
				catch(Exception ex)
				{
					Console.WriteLine(ex.Message);
				}


				if (pokemon != null)
				{
					await Context.Channel.SendMessageAsync("", false, buildEmbedForPokedexEntry(pokemon));
				}
				else
				{
					await Context.Channel.SendMessageAsync(Pokemon404Response.GetRandom());
				}
			}
		}

		[Command("sword"), Summary("Adds you to the Pokemon Sword role."),RequireSpecificChannel(POKEMON_GUILD_ID)]
		public async Task AddSwordRole()
		{
			IRole SwordRole = Context.Guild.GetRole(SWORD_ROLE_ID);
			if (!(Context.User as IGuildUser).RoleIds.Contains(SWORD_ROLE_ID))
			{
				await (Context.User as IGuildUser).AddRoleAsync(SwordRole);
				await Context.Message.AddReactionAsync(Context.Guild.Emotes.First(emote => emote.Id == SWORD_EMOTE_ID));
				if ((Context.User as IGuildUser).RoleIds.Contains(SHIELD_ROLE_ID))
				{
					await Context.Channel.SendFileAsync(Two_Pokemon_Sword_Image);
				}
			}
			else
			{
				await Context.Channel.SendMessageAsync("You're already in the role!");
			}
		}

		[Command("shield"), Summary("Adds you to the Pokemon Shield role."), RequireSpecificChannel(POKEMON_GUILD_ID)]
		public async Task AddShieldRole()
		{
			IRole ShieldRole = Context.Guild.GetRole(SHIELD_ROLE_ID);
			if (!(Context.User as IGuildUser).RoleIds.Contains(SHIELD_ROLE_ID))
			{
				await (Context.User as IGuildUser).AddRoleAsync(ShieldRole);
				await Context.Message.AddReactionAsync(Context.Guild.Emotes.First(emote => emote.Id == SHIELD_EMOTE_ID));
				if ((Context.User as IGuildUser).RoleIds.Contains(SWORD_ROLE_ID))
				{
					await Context.Channel.SendFileAsync(Two_Pokemon_Shield_Image);
				}
			}
			else
			{
				await Context.Channel.SendMessageAsync("You're already in the role!");
			}
		}

		[Command("ididit"), Summary("Marks you as a LEAGUE CHAMPION.")]
		public async Task DesignateAsChampion()
		{
			IGuildUser user = Context.User as IGuildUser;
			if(!user.RoleIds.Contains(SHIELD_CHAMPION_ROLE_ID) && !user.RoleIds.Contains(SWORD_CHAMPION_ROLE_ID))
			{
				if (user.RoleIds.Contains(SHIELD_ROLE_ID) || user.RoleIds.Contains(SWORD_ROLE_ID))
				{
					AddVersionChampionship(user, Context);
					if (user.RoleIds.Contains(GENERIC_CHAMPION_ROLE))
					{
						await user.RemoveRoleAsync(Context.Guild.GetRole(GENERIC_CHAMPION_ROLE));
					}
				}
				else
				{
					await user.AddRoleAsync(Context.Guild.GetRole(GENERIC_CHAMPION_ROLE));
				}
				await Context.Message.AddReactionAsync(Context.Guild.Emotes.First(e => e.Id == LEAGUE_CHAMPION_EMOTE_ID));
			}
			else
			{
				await Context.Channel.SendMessageAsync("We already know you're a champ. Showoff.");
			}			
		}

		private void AddVersionChampionship(IGuildUser user, ICommandContext context)
		{
			if (user.RoleIds.Contains(SHIELD_ROLE_ID))
			{
				user.AddRoleAsync(context.Guild.Roles.First(r => r.Id == SHIELD_CHAMPION_ROLE_ID));
			}
			else
			{
				user.AddRoleAsync(context.Guild.Roles.First(r => r.Id == SWORD_CHAMPION_ROLE_ID));
			}
		}

		public static Embed buildEmbedForPokedexEntry(PokemonSpecies pokemon)
		{
			EmbedBuilder builder = new EmbedBuilder();
			int dexNum = pokemon.PokedexNumbers.First(p => p.Pokedex.Name == "national").EntryNumber;
			string dexNumString = (dexNum >= 100) ? dexNum.ToString() : ("0" + dexNum.ToString());
			string name = pokemon.Names.First(pn => pn.Language.Name == "en").Name;
			string species = pokemon.Genera.First(g => g.Language.Name == "en").Name;
			string imageUrl = string.Format(PokemonImageAddress, dexNumString);
			string serebiiUrl = string.Format(SerebiiPokedexLink, dexNumString);
			string flavorText = pokemon.FlavorTexts.First(ft => ft.Language.Name == "en").FlavorText;

			builder.WithTitle($"{name} - {dexNumString}");
			builder.WithDescription(species);
			builder.WithImageUrl(imageUrl);
			builder.WithUrl(serebiiUrl);
			builder.WithFooter(flavorText);

			/*builder.WithTitle(redditPost.data.title);
			builder.AddField("Subreddit | Score", redditPost.data.subreddit + " | " + redditPost.data.score);
			builder.WithImageUrl(redditPost.data.url);
			builder.WithColor(Color.DarkPurple);*/
			return builder.Build();
		}
	}
}
