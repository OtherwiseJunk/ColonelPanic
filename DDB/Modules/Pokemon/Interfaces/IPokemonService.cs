using DartsDiscordBots.Modules.Pokemon.Models;
using System.Collections.Generic;

namespace DartsDiscordBots.Modules.Pokemon.Interfaces
{
	public interface IPokemonService
	{
		Dictionary<ulong,PokemonGuildConfiguration> ConfiguredGuilds { get; set; }
		int MaxPokemonNumber { get; set; }
		string TwoPokemonShieldImage { get; set; }
		string TwoPokemonSwordImage { get; set; }
		string PokemonImageAddress { get; set; }
		string SerebiiAddress { get; set; }
		List<string> Pokemon404Response { get; set; }
	}
}
