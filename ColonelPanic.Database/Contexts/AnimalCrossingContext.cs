using DartsDiscordBots.Models;
using DartsDiscordBots.Modules.AnimalCrossing.Models;
using Microsoft.EntityFrameworkCore;

namespace ColonelPanic.Database.Contexts
{
	public class AnimalCrossingContext : DbContext
	{
		public AnimalCrossingContext(DbContextOptions<AnimalCrossingContext> options): base(options) { }

		public DbSet<Town> Towns { get; set; }
		public DbSet<Fruit> Fruits { get; set; }
		public DbSet<TurnipBuyPrice> BuyPrices { get; set; }
		public DbSet<TurnipSellPrice> SellPrices { get; set; }
		public DbSet<Item> WishlistItems { get; set; }
	}
}
