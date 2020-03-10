using DartsDiscordBots.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DartsDiscordBots.Interfaces
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
