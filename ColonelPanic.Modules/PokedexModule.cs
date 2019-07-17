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
	public class PokedexModule : ModuleBase
	{
		/*https://pokeapi.co/api/v2/pokemon/807 is the last entry that returns right now, so there are OFFICIALLY 807 pokemon I guess.*/
		public static int MAX_POKEDEX_NUMBER = 807;
		public static string PokemonImageAddress = "https://assets.pokemon.com/assets/cms2/img/pokedex/full/{0}.png";
		public static string SerebiiPokedexLink = "https://www.serebii.net/pokedex-sm/{0}.shtml";
		public static List<string> Pokemon404Response = new List<string> {
			"I don't think there are that many Pokémon, my dude.",
			"That one isn't real, you know?",
			"I don't think I know that one.",
			"Never heard of it. Why don't you tell me more?",
			"No.",
		};

		[Command("pokedex"), Summary("Gets information for the Pokémon specified, either by National Pokédex number or species name. Links to the latest version of the Serebii Pokédex entry, currently Sun and Moon.")]
		public async Task GetPokedexEntry([Remainder] string pokemonIdentifier) {
			int pokedexNumber;
			if(int.TryParse(pokemonIdentifier, out pokedexNumber))
			{
				if (pokedexNumber <= 807 && pokedexNumber > 0)
				{
					PokemonSpecies pokemon = await DataFetcher.GetApiObject<PokemonSpecies>(pokedexNumber);

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
						await Context.Channel.SendMessageAsync(Pokemon404Response.GetRandom());
					}
				}
			}
			else
			{
				PokemonSpecies pokemon = await DataFetcher.GetNamedApiObject<PokemonSpecies>(pokemonIdentifier.ToLower());
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
