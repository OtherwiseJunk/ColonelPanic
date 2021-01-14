namespace DartsDiscordBots.Modules.Pokemon.Models
{
	public class PokemonGuildConfiguration
	{
		public ulong SwordRoleId { get; set; }
		public ulong SwordChampionRoleId { get; set; }
		public ulong ShieldRoleId { get; set; }
		public ulong ShieldChampionRoleId { get; set; }
		public ulong GenericChampionRoleId { get; set; }
		public ulong LeagueChampionEmoteId { get; set; }
		public ulong ShieldEmoteId { get; set; }
		public ulong SwordEmoteId { get; set; }
		public PokemonGuildConfiguration (ulong swordRole, ulong swordChampionRole, ulong shieldRole,
			ulong shieldChampionRole, ulong genericChampionRole, ulong leagueChampionEmote,
			ulong shieldEmote, ulong swordEmote)
		{
			SwordRoleId = swordRole;
			SwordChampionRoleId = swordChampionRole;
			ShieldRoleId = shieldRole;
			ShieldChampionRoleId = shieldChampionRole;
			GenericChampionRoleId = genericChampionRole;
			LeagueChampionEmoteId = leagueChampionEmote;
			ShieldEmoteId = shieldEmote;
			SwordEmoteId = swordEmote;
		}
	}
}
